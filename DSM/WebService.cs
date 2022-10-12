using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Windows.Forms;
using System.Xml;
//using System.Xml.Linq;
//using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Linq;
using System.Data.SqlClient;
using DSMData;
using DSM;
using iTextSharp.text;

namespace DSM
{
    //class WebService
    public class WebService
    {
        public string Url { get; private set; }
        public string Method { get; private set; }
        public Dictionary<string, string> Params = new Dictionary<string, string>();
        public XDocument ResponseSOAP = XDocument.Parse("<root/>");
        public XDocument ResultXML = XDocument.Parse("<root/>");
        public string ResultString = String.Empty;
        DSMModelData objDSMModelData = new DSMModelData();
        private Cursor InitialCursorState;

        public WebService()
        {
            Url = String.Empty;
            Method = String.Empty;
        }
        public WebService(string baseUrl)
        {
            Url = baseUrl;
            Method = String.Empty;
        }
        public WebService(string baseUrl, string methodName)
        {
            Url = baseUrl;
            Method = methodName;
        }

        // Public API

        /// <summary>
        /// Adds a parameter to the WebMethod invocation.
        /// </summary>
        /// <param name="name">Name of the WebMethod parameter (case sensitive)</param>
        /// <param name="value">Value to pass to the paramenter</param>
        public void AddParameter(string name, string value)
        {
            Params.Add(name, value);
        }

        public void Invoke()
        {
            Invoke(Method, true);
        }

        /// <summary>
        /// Using the base url, invokes the WebMethod with the given name
        /// </summary>
        /// <param name="methodName">Web Method name</param>
        public void Invoke(string methodName)
        {
            Invoke(methodName, true);
        }

        /// <summary>
        /// Cleans all internal data used in the last invocation, except the WebService's URL.
        /// This avoids creating a new WebService object when the URL you want to use is the same.
        /// </summary>
        public void CleanLastInvoke()
        {
            ResponseSOAP = ResultXML = null;
            ResultString = Method = String.Empty;
            Params = new Dictionary<string, string>();
        }

        #region Helper Methods

        /// <summary>
        /// Checks if the WebService's URL and the WebMethod's name are valid. If not, throws ArgumentNullException.
        /// </summary>
        /// <param name="methodName">Web Method name (optional)</param>
        private void AssertCanInvoke(string methodName = "")
        {
            if (Url == String.Empty)
                throw new ArgumentNullException("You tried to invoke a webservice without specifying the WebService's URL.");
            if ((methodName == "") && (Method == String.Empty))
                throw new ArgumentNullException("You tried to invoke a webservice without specifying the WebMethod.");
        }

        private void ExtractResult(string methodName)
        {
            //MessageBox.Show("Coming ExtractResult");
            // Selects just the elements with namespace http://tempuri.org/ (i.e. ignores SOAP namespace)
            XmlNamespaceManager namespMan = new XmlNamespaceManager(new NameTable());
            namespMan.AddNamespace("foo", "http://tempuri.org/");

            //XElement webMethodResult = ResponseSOAP.XPathSelectElement("//foo:" + methodName + "Result", namespMan);
            XElement webMethodResult = ResponseSOAP.XPathSelectElement("//foo:" + methodName + "Result", namespMan);
            //res
            // If the result is an XML, return it and convert it to string
            if (webMethodResult.FirstNode.NodeType == XmlNodeType.Element)
            {
                ResultXML = XDocument.Parse(webMethodResult.FirstNode.ToString());
                ResultXML = Utils.RemoveNamespaces(ResultXML);
                ResultString = ResultXML.ToString();
            }
            // If the result is a string, return it and convert it to XML (creating a root node to wrap the result)
            else
            {
                ResultString = webMethodResult.FirstNode.ToString();
                ResultXML = XDocument.Parse("<root>" + ResultString + "</root>");
            }
        }

        /// <summary>
        /// Invokes a Web Method, with its parameters encoded or not.
        /// </summary>
        /// <param name="methodName">Name of the web method you want to call (case sensitive)</param>
        /// <param name="encode">Do you want to encode your parameters? (default: true)</param>

        private void Invoke(string methodName, bool encode)
        {
            try
            {
                AssertCanInvoke(methodName);
                byte[] file;
                string hexvalue = "";
                objDSMModelData = new DSMModelData();
                file = objDSMModelData.GetHexValueByInvoiceNumber(Global.APIInvoiceNumber);
                string pdfBase64file = Convert.ToBase64String(file);
                hexvalue = BitConverter.ToString(file).Replace("-", "");
                int Finyear = 0;
                string prefix_invoice_no = "";
                int fin_mon = 0;
                //string _sQry_Prefix_invoice_no = "select month(getdate()) as cur_month, FORMAT(getdate(), 'yy') as cur_year";
                string _sQry_Prefix_invoice_no = "select month(getdate()) as cur_month, year(getdate()) as cur_year";
                SqlDataReader _rdr_Prefix_invoice_no = Global.executeQuery(_sQry_Prefix_invoice_no);
                if (_rdr_Prefix_invoice_no.Read())
                {
                    string year = _rdr_Prefix_invoice_no["cur_year"].ToString().Substring(2, 2);
                    fin_mon = 3;
                    if (int.Parse(_rdr_Prefix_invoice_no["cur_month"].ToString()) > fin_mon)
                        prefix_invoice_no = year + (int.Parse(year) + 1);
                    else
                        prefix_invoice_no = (int.Parse(year) - 1) + (year);
                }
                //_inv.Finyear = Convert.ToInt32(prefix_invoice_no.ToString().Trim());
                Finyear = Convert.ToInt32(prefix_invoice_no.ToString().Trim());

                Global.APIFinYear = Finyear;
                int count_rec = objDSMModelData.GetInvoiceDetailsByInvoiceNumber(Global.APIInvoiceNumber).Count;

                string soapStr = "";

                if (count_rec > 0)
                {

                    if (count_rec == 1)
                    {
                        //int i = 0;

                        soapStr =
                   @"<?xml version=""1.0"" encoding=""utf-8""?>
                            <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
                               xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
                               xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                              <soap:Body>
                              <getData xmlns=""{APISOAP}"">
      <HEXADECIMAL>{0}</HEXADECIMAL>
      <data01>
        <GET_DATA>
          <TAB_DATA_HEADER>
            <IVNUM>{1}</IVNUM>
            <IVDAT>{2}</IVDAT>
            <LIFNR>{3}</LIFNR>
            <ZSHOP>{4}</ZSHOP>
            <EBELN>{5}</EBELN>
            <MATNR>{6}</MATNR>
            <IVQTY>{7}</IVQTY>
            <ZAIVAMT>{8}</ZAIVAMT>
            <ZANETPR>{9}</ZANETPR>
            <ZANETWR>{10}</ZANETWR>
            <ZCGST>{11}</ZCGST>
            <ZSGST>{12}</ZSGST>
            <ZIGST>{13}</ZIGST>
            <ZADTC2>{14}</ZADTC2>
            <ZACNMC>{15}</ZACNMC>
            <ZACNPC>{16}</ZACNPC>
            <ZAASVL>{17}</ZAASVL>
            <ZHSNSAC>{18}</ZHSNSAC>
            <ZGSTIN>{19}</ZGSTIN>
            <VEHNO>{20}</VEHNO>
            <RETDC>{21}</RETDC>
<ZATCS>{TCS1}</ZATCS>
<ZATOLC>0</ZATOLC>
<ZLOTCODE>{LOT}</ZLOTCODE>
<EWAYBILL>{EWAY}</EWAYBILL>
<EINVNO>{EINVNO}</EINVNO>
<ZMFGDT>{2}</ZMFGDT>
<ZNUM1>{1N1}</ZNUM1>
<ZNUM2>{1N2}</ZNUM2>
<ZNUM3>{1N3}</ZNUM3>
<ZNUM4>{1N4}</ZNUM4>
<ZNUM5>{1N5}</ZNUM5>
<ZNUM6>{1N6}</ZNUM6>
<ZNUM7>{1N7}</ZNUM7>
<ZCHAR1>{1C1}</ZCHAR1>
<ZCHAR2>{1C2}</ZCHAR2>
<ZCHAR3>{1C3}</ZCHAR3>
<ZCHAR4>{1C4}</ZCHAR4>
<ZCHAR5>{1C5}</ZCHAR5>
<ZDATE1>{2}</ZDATE1>
<ZDATE2>{2}</ZDATE2>
<ZDATE3>{2}</ZDATE3>
<ZDATE4>{2}</ZDATE4>
<ZDATE5>{2}</ZDATE5>
          </TAB_DATA_HEADER>
        </GET_DATA>
      </data01>
    </getData>
                              </soap:Body>
                            </soap:Envelope>";
                    }

                    else if (count_rec == 2)
                    {
                        soapStr =
                        @"<?xml version=""1.0"" encoding=""utf-8""?>
                            <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
                               xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
                               xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                              <soap:Body>
                              <getData xmlns=""{APISOAP}"">
      <HEXADECIMAL>{0}</HEXADECIMAL>
      <data01>
        <GET_DATA>
          <TAB_DATA_HEADER>
            <IVNUM>{1}</IVNUM>
            <IVDAT>{2}</IVDAT>
            <LIFNR>{3}</LIFNR>
            <ZSHOP>{4}</ZSHOP>
            <EBELN>{5}</EBELN>
            <MATNR>{6}</MATNR>
            <IVQTY>{7}</IVQTY>
            <ZAIVAMT>{8}</ZAIVAMT>
            <ZANETPR>{9}</ZANETPR>
            <ZANETWR>{10}</ZANETWR>
            <ZCGST>{11}</ZCGST>
            <ZSGST>{12}</ZSGST>
            <ZIGST>{13}</ZIGST>
            <ZADTC2>{14}</ZADTC2>
            <ZACNMC>{15}</ZACNMC>
            <ZACNPC>{16}</ZACNPC>
            <ZAASVL>{17}</ZAASVL>
            <ZHSNSAC>{18}</ZHSNSAC>
            <ZGSTIN>{19}</ZGSTIN>
            <VEHNO>{20}</VEHNO>
            <RETDC>{21}</RETDC>
<ZATCS>{TCS1}</ZATCS>
<ZATOLC>0</ZATOLC>
<ZLOTCODE>{LOT}</ZLOTCODE>
<EWAYBILL>{EWAY}</EWAYBILL>
<EINVNO>{EINVNO}</EINVNO>
<ZMFGDT>{2}</ZMFGDT>
<ZNUM1>{1N1}</ZNUM1>
<ZNUM2>{1N2}</ZNUM2>
<ZNUM3>{1N3}</ZNUM3>
<ZNUM4>{1N4}</ZNUM4>
<ZNUM5>{1N5}</ZNUM5>
<ZNUM6>{1N6}</ZNUM6>
<ZNUM7>{1N7}</ZNUM7>
<ZCHAR1>{1C1}</ZCHAR1>
<ZCHAR2>{1C2}</ZCHAR2>
<ZCHAR3>{1C3}</ZCHAR3>
<ZCHAR4>{1C4}</ZCHAR4>
<ZCHAR5>{1C5}</ZCHAR5>
<ZDATE1>{2}</ZDATE1>
<ZDATE2>{2}</ZDATE2>
<ZDATE3>{2}</ZDATE3>
<ZDATE4>{2}</ZDATE4>
<ZDATE5>{2}</ZDATE5>
          </TAB_DATA_HEADER>
          <TAB_DATA_HEADER>
            <IVNUM>{22}</IVNUM>
            <IVDAT>{23}</IVDAT>
            <LIFNR>{24}</LIFNR>
            <ZSHOP>{25}</ZSHOP>
            <EBELN>{26}</EBELN>
            <MATNR>{27}</MATNR>
            <IVQTY>{28}</IVQTY>
            <ZAIVAMT>{29}</ZAIVAMT>
            <ZANETPR>{30}</ZANETPR>
            <ZANETWR>{31}</ZANETWR>
            <ZCGST>{32}</ZCGST>
            <ZSGST>{33}</ZSGST>
            <ZIGST>{34}</ZIGST>
            <ZADTC2>{35}</ZADTC2>
            <ZACNMC>{36}</ZACNMC>
            <ZACNPC>{37}</ZACNPC>
            <ZAASVL>{38}</ZAASVL>
            <ZHSNSAC>{39}</ZHSNSAC>
            <ZGSTIN>{40}</ZGSTIN>
            <VEHNO>{41}</VEHNO>
            <RETDC>{42}</RETDC>
<ZATCS>{TCS2}</ZATCS>
<ZATOLC>0</ZATOLC>
<ZLOTCODE>{LOT}</ZLOTCODE>
<EWAYBILL>{EWAY}</EWAYBILL>
<EINVNO>{EINVNO}</EINVNO>
<ZMFGDT>{2}</ZMFGDT>
<ZNUM1>{2N1}</ZNUM1>
<ZNUM2>{2N2}</ZNUM2>
<ZNUM3>{2N3}</ZNUM3>
<ZNUM4>{2N4}</ZNUM4>
<ZNUM5>{2N5}</ZNUM5>
<ZNUM6>{2N6}</ZNUM6>
<ZNUM7>{2N7}</ZNUM7>
<ZCHAR1>{2C1}</ZCHAR1>
<ZCHAR2>{2C2}</ZCHAR2>
<ZCHAR3>{2C3}</ZCHAR3>
<ZCHAR4>{2C4}</ZCHAR4>
<ZCHAR5>{2C5}</ZCHAR5>
<ZDATE1>{2}</ZDATE1>
<ZDATE2>{2}</ZDATE2>
<ZDATE3>{2}</ZDATE3>
<ZDATE4>{2}</ZDATE4>
<ZDATE5>{2}</ZDATE5>
          </TAB_DATA_HEADER>
         </GET_DATA>
      </data01>
    </getData>
                              </soap:Body>
                            </soap:Envelope>";
                    }
                    else if (count_rec == 3)
                    {
                        soapStr =
                        @"<?xml version=""1.0"" encoding=""utf-8""?>
                            <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
                               xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
                               xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                              <soap:Body>
                              <getData xmlns=""{APISOAP}"">
      <HEXADECIMAL>{0}</HEXADECIMAL>
      <data01>
        <GET_DATA>
          <TAB_DATA_HEADER>
            <IVNUM>{1}</IVNUM>
            <IVDAT>{2}</IVDAT>
            <LIFNR>{3}</LIFNR>
            <ZSHOP>{4}</ZSHOP>
            <EBELN>{5}</EBELN>
            <MATNR>{6}</MATNR>
            <IVQTY>{7}</IVQTY>
            <ZAIVAMT>{8}</ZAIVAMT>
            <ZANETPR>{9}</ZANETPR>
            <ZANETWR>{10}</ZANETWR>
            <ZCGST>{11}</ZCGST>
            <ZSGST>{12}</ZSGST>
            <ZIGST>{13}</ZIGST>
            <ZADTC2>{14}</ZADTC2>
            <ZACNMC>{15}</ZACNMC>
            <ZACNPC>{16}</ZACNPC>
            <ZAASVL>{17}</ZAASVL>
            <ZHSNSAC>{18}</ZHSNSAC>
            <ZGSTIN>{19}</ZGSTIN>
            <VEHNO>{20}</VEHNO>
            <RETDC>{21}</RETDC>
<ZATCS>{TCS1}</ZATCS>
<ZATOLC>0</ZATOLC>
<ZLOTCODE>{LOT}</ZLOTCODE>
<EWAYBILL>{EWAY}</EWAYBILL>
<EINVNO>{EINVNO}</EINVNO>
<ZMFGDT>{2}</ZMFGDT>
<ZNUM1>{1N1}</ZNUM1>
<ZNUM2>{1N2}</ZNUM2>
<ZNUM3>{1N3}</ZNUM3>
<ZNUM4>{1N4}</ZNUM4>
<ZNUM5>{1N5}</ZNUM5>
<ZNUM6>{1N6}</ZNUM6>
<ZNUM7>{1N7}</ZNUM7>
<ZCHAR1>{1C1}</ZCHAR1>
<ZCHAR2>{1C2}</ZCHAR2>
<ZCHAR3>{1C3}</ZCHAR3>
<ZCHAR4>{1C4}</ZCHAR4>
<ZCHAR5>{1C5}</ZCHAR5>
<ZDATE1>{2}</ZDATE1>
<ZDATE2>{2}</ZDATE2>
<ZDATE3>{2}</ZDATE3>
<ZDATE4>{2}</ZDATE4>
<ZDATE5>{2}</ZDATE5>
          </TAB_DATA_HEADER>
          <TAB_DATA_HEADER>
            <IVNUM>{22}</IVNUM>
            <IVDAT>{23}</IVDAT>
            <LIFNR>{24}</LIFNR>
            <ZSHOP>{25}</ZSHOP>
            <EBELN>{26}</EBELN>
            <MATNR>{27}</MATNR>
            <IVQTY>{28}</IVQTY>
            <ZAIVAMT>{29}</ZAIVAMT>
            <ZANETPR>{30}</ZANETPR>
            <ZANETWR>{31}</ZANETWR>
            <ZCGST>{32}</ZCGST>
            <ZSGST>{33}</ZSGST>
            <ZIGST>{34}</ZIGST>
            <ZADTC2>{35}</ZADTC2>
            <ZACNMC>{36}</ZACNMC>
            <ZACNPC>{37}</ZACNPC>
            <ZAASVL>{38}</ZAASVL>
            <ZHSNSAC>{39}</ZHSNSAC>
            <ZGSTIN>{40}</ZGSTIN>
            <VEHNO>{41}</VEHNO>
            <RETDC>{42}</RETDC>
<ZATCS>{TCS2}</ZATCS>
<ZATOLC>0</ZATOLC>
<ZLOTCODE>{LOT}</ZLOTCODE>
<EWAYBILL>{EWAY}</EWAYBILL>
<EINVNO>{EINVNO}</EINVNO>
<ZMFGDT>{2}</ZMFGDT>
<ZNUM1>{2N1}</ZNUM1>
<ZNUM2>{2N2}</ZNUM2>
<ZNUM3>{2N3}</ZNUM3>
<ZNUM4>{2N4}</ZNUM4>
<ZNUM5>{2N5}</ZNUM5>
<ZNUM6>{2N6}</ZNUM6>
<ZNUM7>{2N7}</ZNUM7>
<ZCHAR1>{2C1}</ZCHAR1>
<ZCHAR2>{2C2}</ZCHAR2>
<ZCHAR3>{2C3}</ZCHAR3>
<ZCHAR4>{2C4}</ZCHAR4>
<ZCHAR5>{2C5}</ZCHAR5>
<ZDATE1>{2}</ZDATE1>
<ZDATE2>{2}</ZDATE2>
<ZDATE3>{2}</ZDATE3>
<ZDATE4>{2}</ZDATE4>
<ZDATE5>{2}</ZDATE5>
          </TAB_DATA_HEADER>
          <TAB_DATA_HEADER>
            <IVNUM>{43}</IVNUM>
            <IVDAT>{44}</IVDAT>
            <LIFNR>{45}</LIFNR>
            <ZSHOP>{46}</ZSHOP>
            <EBELN>{47}</EBELN>
            <MATNR>{48}</MATNR>
            <IVQTY>{49}</IVQTY>
            <ZAIVAMT>{50}</ZAIVAMT>
            <ZANETPR>{51}</ZANETPR>
            <ZANETWR>{52}</ZANETWR>
            <ZCGST>{53}</ZCGST>
            <ZSGST>{54}</ZSGST>
            <ZIGST>{55}</ZIGST>
            <ZADTC2>{56}</ZADTC2>
            <ZACNMC>{57}</ZACNMC>
            <ZACNPC>{58}</ZACNPC>
            <ZAASVL>{59}</ZAASVL>
            <ZHSNSAC>{60}</ZHSNSAC>
            <ZGSTIN>{61}</ZGSTIN>
            <VEHNO>{62}</VEHNO>
            <RETDC>{63}</RETDC>
<ZATCS>{TCS3}</ZATCS>
<ZATOLC>0</ZATOLC>
<ZLOTCODE>{LOT}</ZLOTCODE>
<EWAYBILL>{EWAY}</EWAYBILL>
<EINVNO>{EINVNO}</EINVNO>
<ZMFGDT>{2}</ZMFGDT>
<ZNUM1>{3N1}</ZNUM1>
<ZNUM2>{3N2}</ZNUM2>
<ZNUM3>{3N3}</ZNUM3>
<ZNUM4>{3N4}</ZNUM4>
<ZNUM5>{3N5}</ZNUM5>
<ZNUM6>{3N6}</ZNUM6>
<ZNUM7>{3N7}</ZNUM7>
<ZCHAR1>{3C1}</ZCHAR1>
<ZCHAR2>{3C2}</ZCHAR2>
<ZCHAR3>{3C3}</ZCHAR3>
<ZCHAR4>{3C4}</ZCHAR4>
<ZCHAR5>{3C5}</ZCHAR5>
<ZDATE1>{2}</ZDATE1>
<ZDATE2>{2}</ZDATE2>
<ZDATE3>{2}</ZDATE3>
<ZDATE4>{2}</ZDATE4>
<ZDATE5>{2}</ZDATE5>
          </TAB_DATA_HEADER>
         </GET_DATA>
      </data01>
    </getData>
                              </soap:Body>
                            </soap:Envelope>";
                    }
                    else if (count_rec == 4)
                    {
                        soapStr =
                        @"<?xml version=""1.0"" encoding=""utf-8""?>
                            <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
                               xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
                               xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                              <soap:Body>
                              <getData xmlns=""{APISOAP}"">
      <HEXADECIMAL>{0}</HEXADECIMAL>
      <data01>
        <GET_DATA>
          <TAB_DATA_HEADER>
            <IVNUM>{1}</IVNUM>
            <IVDAT>{2}</IVDAT>
            <LIFNR>{3}</LIFNR>
            <ZSHOP>{4}</ZSHOP>
            <EBELN>{5}</EBELN>
            <MATNR>{6}</MATNR>
            <IVQTY>{7}</IVQTY>
            <ZAIVAMT>{8}</ZAIVAMT>
            <ZANETPR>{9}</ZANETPR>
            <ZANETWR>{10}</ZANETWR>
            <ZCGST>{11}</ZCGST>
            <ZSGST>{12}</ZSGST>
            <ZIGST>{13}</ZIGST>
            <ZADTC2>{14}</ZADTC2>
            <ZACNMC>{15}</ZACNMC>
            <ZACNPC>{16}</ZACNPC>
            <ZAASVL>{17}</ZAASVL>
            <ZHSNSAC>{18}</ZHSNSAC>
            <ZGSTIN>{19}</ZGSTIN>
            <VEHNO>{20}</VEHNO>
            <RETDC>{21}</RETDC>
<ZATCS>{TCS1}</ZATCS>
<ZATOLC>0</ZATOLC>
<ZLOTCODE>{LOT}</ZLOTCODE>
<EWAYBILL>{EWAY}</EWAYBILL>
<EINVNO>{EINVNO}</EINVNO>
<ZMFGDT>{2}</ZMFGDT>
<ZNUM1>{1N1}</ZNUM1>
<ZNUM2>{1N2}</ZNUM2>
<ZNUM3>{1N3}</ZNUM3>
<ZNUM4>{1N4}</ZNUM4>
<ZNUM5>{1N5}</ZNUM5>
<ZNUM6>{1N6}</ZNUM6>
<ZNUM7>{1N7}</ZNUM7>
<ZCHAR1>{1C1}</ZCHAR1>
<ZCHAR2>{1C2}</ZCHAR2>
<ZCHAR3>{1C3}</ZCHAR3>
<ZCHAR4>{1C4}</ZCHAR4>
<ZCHAR5>{1C5}</ZCHAR5>
<ZDATE1>{2}</ZDATE1>
<ZDATE2>{2}</ZDATE2>
<ZDATE3>{2}</ZDATE3>
<ZDATE4>{2}</ZDATE4>
<ZDATE5>{2}</ZDATE5>
          </TAB_DATA_HEADER>
          <TAB_DATA_HEADER>
            <IVNUM>{22}</IVNUM>
            <IVDAT>{23}</IVDAT>
            <LIFNR>{24}</LIFNR>
            <ZSHOP>{25}</ZSHOP>
            <EBELN>{26}</EBELN>
            <MATNR>{27}</MATNR>
            <IVQTY>{28}</IVQTY>
            <ZAIVAMT>{29}</ZAIVAMT>
            <ZANETPR>{30}</ZANETPR>
            <ZANETWR>{31}</ZANETWR>
            <ZCGST>{32}</ZCGST>
            <ZSGST>{33}</ZSGST>
            <ZIGST>{34}</ZIGST>
            <ZADTC2>{35}</ZADTC2>
            <ZACNMC>{36}</ZACNMC>
            <ZACNPC>{37}</ZACNPC>
            <ZAASVL>{38}</ZAASVL>
            <ZHSNSAC>{39}</ZHSNSAC>
            <ZGSTIN>{40}</ZGSTIN>
            <VEHNO>{41}</VEHNO>
            <RETDC>{42}</RETDC>
<ZATCS>{TCS2}</ZATCS>
<ZATOLC>0</ZATOLC>
<ZLOTCODE>{LOT}</ZLOTCODE>
<EWAYBILL>{EWAY}</EWAYBILL>
<EINVNO>{EINVNO}</EINVNO>
<ZMFGDT>{2}</ZMFGDT>
<ZNUM1>{2N1}</ZNUM1>
<ZNUM2>{2N2}</ZNUM2>
<ZNUM3>{2N3}</ZNUM3>
<ZNUM4>{2N4}</ZNUM4>
<ZNUM5>{2N5}</ZNUM5>
<ZNUM6>{2N6}</ZNUM6>
<ZNUM7>{2N7}</ZNUM7>
<ZCHAR1>{2C1}</ZCHAR1>
<ZCHAR2>{2C2}</ZCHAR2>
<ZCHAR3>{2C3}</ZCHAR3>
<ZCHAR4>{2C4}</ZCHAR4>
<ZCHAR5>{2C5}</ZCHAR5>
<ZDATE1>{2}</ZDATE1>
<ZDATE2>{2}</ZDATE2>
<ZDATE3>{2}</ZDATE3>
<ZDATE4>{2}</ZDATE4>
<ZDATE5>{2}</ZDATE5>
          </TAB_DATA_HEADER>
          <TAB_DATA_HEADER>
            <IVNUM>{43}</IVNUM>
            <IVDAT>{44}</IVDAT>
            <LIFNR>{45}</LIFNR>
            <ZSHOP>{46}</ZSHOP>
            <EBELN>{47}</EBELN>
            <MATNR>{48}</MATNR>
            <IVQTY>{49}</IVQTY>
            <ZAIVAMT>{50}</ZAIVAMT>
            <ZANETPR>{51}</ZANETPR>
            <ZANETWR>{52}</ZANETWR>
            <ZCGST>{53}</ZCGST>
            <ZSGST>{54}</ZSGST>
            <ZIGST>{55}</ZIGST>
            <ZADTC2>{56}</ZADTC2>
            <ZACNMC>{57}</ZACNMC>
            <ZACNPC>{58}</ZACNPC>
            <ZAASVL>{59}</ZAASVL>
            <ZHSNSAC>{60}</ZHSNSAC>
            <ZGSTIN>{61}</ZGSTIN>
            <VEHNO>{62}</VEHNO>
            <RETDC>{63}</RETDC>
<ZATCS>{TCS3}</ZATCS>
<ZATOLC>0</ZATOLC>
<ZLOTCODE>{LOT}</ZLOTCODE>
<EWAYBILL>{EWAY}</EWAYBILL>
<EINVNO>{EINVNO}</EINVNO>
<ZMFGDT>{2}</ZMFGDT>
<ZNUM1>{3N1}</ZNUM1>
<ZNUM2>{3N2}</ZNUM2>
<ZNUM3>{3N3}</ZNUM3>
<ZNUM4>{3N4}</ZNUM4>
<ZNUM5>{3N5}</ZNUM5>
<ZNUM6>{3N6}</ZNUM6>
<ZNUM7>{3N7}</ZNUM7>
<ZCHAR1>{3C1}</ZCHAR1>
<ZCHAR2>{3C2}</ZCHAR2>
<ZCHAR3>{3C3}</ZCHAR3>
<ZCHAR4>{3C4}</ZCHAR4>
<ZCHAR5>{3C5}</ZCHAR5>
<ZDATE1>{2}</ZDATE1>
<ZDATE2>{2}</ZDATE2>
<ZDATE3>{2}</ZDATE3>
<ZDATE4>{2}</ZDATE4>
<ZDATE5>{2}</ZDATE5>
          </TAB_DATA_HEADER>
          <TAB_DATA_HEADER>
            <IVNUM>{64}</IVNUM>
            <IVDAT>{65}</IVDAT>
            <LIFNR>{66}</LIFNR>
            <ZSHOP>{67}</ZSHOP>
            <EBELN>{68}</EBELN>
            <MATNR>{69}</MATNR>
            <IVQTY>{70}</IVQTY>
            <ZAIVAMT>{71}</ZAIVAMT>
            <ZANETPR>{72}</ZANETPR>
            <ZANETWR>{73}</ZANETWR>
            <ZCGST>{74}</ZCGST>
            <ZSGST>{75}</ZSGST>
            <ZIGST>{76}</ZIGST>
            <ZADTC2>{77}</ZADTC2>
            <ZACNMC>{78}</ZACNMC>
            <ZACNPC>{79}</ZACNPC>
            <ZAASVL>{80}</ZAASVL>
            <ZHSNSAC>{81}</ZHSNSAC>
            <ZGSTIN>{82}</ZGSTIN>
            <VEHNO>{83}</VEHNO>
            <RETDC>{84}</RETDC>
<ZATCS>{TCS4}</ZATCS>
<ZATOLC>0</ZATOLC>
<ZLOTCODE>{LOT}</ZLOTCODE>
<EWAYBILL>{EWAY}</EWAYBILL>
<EINVNO>{EINVNO}</EINVNO>
<ZMFGDT>{2}</ZMFGDT>
<ZNUM1>{4N1}</ZNUM1>
<ZNUM2>{4N2}</ZNUM2>
<ZNUM3>{4N3}</ZNUM3>
<ZNUM4>{4N4}</ZNUM4>
<ZNUM5>{4N5}</ZNUM5>
<ZNUM6>{4N6}</ZNUM6>
<ZNUM7>{4N7}</ZNUM7>
<ZCHAR1>{4C1}</ZCHAR1>
<ZCHAR2>{4C2}</ZCHAR2>
<ZCHAR3>{4C3}</ZCHAR3>
<ZCHAR4>{4C4}</ZCHAR4>
<ZCHAR5>{4C5}</ZCHAR5>
<ZDATE1>{2}</ZDATE1>
<ZDATE2>{2}</ZDATE2>
<ZDATE3>{2}</ZDATE3>
<ZDATE4>{2}</ZDATE4>
<ZDATE5>{2}</ZDATE5>     
          </TAB_DATA_HEADER>
         </GET_DATA>
      </data01>
    </getData>
                              </soap:Body>
                            </soap:Envelope>";
                    }
                    else if (count_rec == 5)
                    {
                        soapStr =
                        @"<?xml version=""1.0"" encoding=""utf-8""?>
                            <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
                               xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
                               xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                              <soap:Body>
                              <getData xmlns=""{APISOAP}"">
      <HEXADECIMAL>{0}</HEXADECIMAL>
      <data01>
        <GET_DATA>
          <TAB_DATA_HEADER>
            <IVNUM>{1}</IVNUM>
            <IVDAT>{2}</IVDAT>
            <LIFNR>{3}</LIFNR>
            <ZSHOP>{4}</ZSHOP>
            <EBELN>{5}</EBELN>
            <MATNR>{6}</MATNR>
            <IVQTY>{7}</IVQTY>
            <ZAIVAMT>{8}</ZAIVAMT>
            <ZANETPR>{9}</ZANETPR>
            <ZANETWR>{10}</ZANETWR>
            <ZCGST>{11}</ZCGST>
            <ZSGST>{12}</ZSGST>
            <ZIGST>{13}</ZIGST>
            <ZADTC2>{14}</ZADTC2>
            <ZACNMC>{15}</ZACNMC>
            <ZACNPC>{16}</ZACNPC>
            <ZAASVL>{17}</ZAASVL>
            <ZHSNSAC>{18}</ZHSNSAC>
            <ZGSTIN>{19}</ZGSTIN>
            <VEHNO>{20}</VEHNO>
            <RETDC>{21}</RETDC>
<ZATCS>{TCS1}</ZATCS>
<ZATOLC>0</ZATOLC>
<ZLOTCODE>{LOT}</ZLOTCODE>
<EWAYBILL>{EWAY}</EWAYBILL>
<EINVNO>{EINVNO}</EINVNO>
<ZMFGDT>{2}</ZMFGDT>
<ZNUM1>{1N1}</ZNUM1>
<ZNUM2>{1N2}</ZNUM2>
<ZNUM3>{1N3}</ZNUM3>
<ZNUM4>{1N4}</ZNUM4>
<ZNUM5>{1N5}</ZNUM5>
<ZNUM6>{1N6}</ZNUM6>
<ZNUM7>{1N7}</ZNUM7>
<ZCHAR1>{1C1}</ZCHAR1>
<ZCHAR2>{1C2}</ZCHAR2>
<ZCHAR3>{1C3}</ZCHAR3>
<ZCHAR4>{1C4}</ZCHAR4>
<ZCHAR5>{1C5}</ZCHAR5>
<ZDATE1>{2}</ZDATE1>
<ZDATE2>{2}</ZDATE2>
<ZDATE3>{2}</ZDATE3>
<ZDATE4>{2}</ZDATE4>
<ZDATE5>{2}</ZDATE5>
          </TAB_DATA_HEADER>
          <TAB_DATA_HEADER>
            <IVNUM>{22}</IVNUM>
            <IVDAT>{23}</IVDAT>
            <LIFNR>{24}</LIFNR>
            <ZSHOP>{25}</ZSHOP>
            <EBELN>{26}</EBELN>
            <MATNR>{27}</MATNR>
            <IVQTY>{28}</IVQTY>
            <ZAIVAMT>{29}</ZAIVAMT>
            <ZANETPR>{30}</ZANETPR>
            <ZANETWR>{31}</ZANETWR>
            <ZCGST>{32}</ZCGST>
            <ZSGST>{33}</ZSGST>
            <ZIGST>{34}</ZIGST>
            <ZADTC2>{35}</ZADTC2>
            <ZACNMC>{36}</ZACNMC>
            <ZACNPC>{37}</ZACNPC>
            <ZAASVL>{38}</ZAASVL>
            <ZHSNSAC>{39}</ZHSNSAC>
            <ZGSTIN>{40}</ZGSTIN>
            <VEHNO>{41}</VEHNO>
            <RETDC>{42}</RETDC>
<ZATCS>{TCS2}</ZATCS>
<ZATOLC>0</ZATOLC>
<ZLOTCODE>{LOT}</ZLOTCODE>
<EWAYBILL>{EWAY}</EWAYBILL>
<EINVNO>{EINVNO}</EINVNO>
<ZMFGDT>{2}</ZMFGDT>
<ZNUM1>{2N1}</ZNUM1>
<ZNUM2>{2N2}</ZNUM2>
<ZNUM3>{2N3}</ZNUM3>
<ZNUM4>{2N4}</ZNUM4>
<ZNUM5>{2N5}</ZNUM5>
<ZNUM6>{2N6}</ZNUM6>
<ZNUM7>{2N7}</ZNUM7>
<ZCHAR1>{2C1}</ZCHAR1>
<ZCHAR2>{2C2}</ZCHAR2>
<ZCHAR3>{2C3}</ZCHAR3>
<ZCHAR4>{2C4}</ZCHAR4>
<ZCHAR5>{2C5}</ZCHAR5>
<ZDATE1>{2}</ZDATE1>
<ZDATE2>{2}</ZDATE2>
<ZDATE3>{2}</ZDATE3>
<ZDATE4>{2}</ZDATE4>
<ZDATE5>{2}</ZDATE5>
          </TAB_DATA_HEADER>
          <TAB_DATA_HEADER>
            <IVNUM>{43}</IVNUM>
            <IVDAT>{44}</IVDAT>
            <LIFNR>{45}</LIFNR>
            <ZSHOP>{46}</ZSHOP>
            <EBELN>{47}</EBELN>
            <MATNR>{48}</MATNR>
            <IVQTY>{49}</IVQTY>
            <ZAIVAMT>{50}</ZAIVAMT>
            <ZANETPR>{51}</ZANETPR>
            <ZANETWR>{52}</ZANETWR>
            <ZCGST>{53}</ZCGST>
            <ZSGST>{54}</ZSGST>
            <ZIGST>{55}</ZIGST>
            <ZADTC2>{56}</ZADTC2>
            <ZACNMC>{57}</ZACNMC>
            <ZACNPC>{58}</ZACNPC>
            <ZAASVL>{59}</ZAASVL>
            <ZHSNSAC>{60}</ZHSNSAC>
            <ZGSTIN>{61}</ZGSTIN>
            <VEHNO>{62}</VEHNO>
            <RETDC>{63}</RETDC>
<ZATCS>{TCS3}</ZATCS>
<ZATOLC>0</ZATOLC>
<ZLOTCODE>{LOT}</ZLOTCODE>
<EWAYBILL>{EWAY}</EWAYBILL>
<EINVNO>{EINVNO}</EINVNO>
<ZMFGDT>{2}</ZMFGDT>
<ZNUM1>{3N1}</ZNUM1>
<ZNUM2>{3N2}</ZNUM2>
<ZNUM3>{3N3}</ZNUM3>
<ZNUM4>{3N4}</ZNUM4>
<ZNUM5>{3N5}</ZNUM5>
<ZNUM6>{3N6}</ZNUM6>
<ZNUM7>{3N7}</ZNUM7>
<ZCHAR1>{3C1}</ZCHAR1>
<ZCHAR2>{3C2}</ZCHAR2>
<ZCHAR3>{3C3}</ZCHAR3>
<ZCHAR4>{3C4}</ZCHAR4>
<ZCHAR5>{3C5}</ZCHAR5>
<ZDATE1>{2}</ZDATE1>
<ZDATE2>{2}</ZDATE2>
<ZDATE3>{2}</ZDATE3>
<ZDATE4>{2}</ZDATE4>
<ZDATE5>{2}</ZDATE5>
          </TAB_DATA_HEADER>
          <TAB_DATA_HEADER>
            <IVNUM>{64}</IVNUM>
            <IVDAT>{65}</IVDAT>
            <LIFNR>{66}</LIFNR>
            <ZSHOP>{67}</ZSHOP>
            <EBELN>{68}</EBELN>
            <MATNR>{69}</MATNR>
            <IVQTY>{70}</IVQTY>
            <ZAIVAMT>{71}</ZAIVAMT>
            <ZANETPR>{72}</ZANETPR>
            <ZANETWR>{73}</ZANETWR>
            <ZCGST>{74}</ZCGST>
            <ZSGST>{75}</ZSGST>
            <ZIGST>{76}</ZIGST>
            <ZADTC2>{77}</ZADTC2>
            <ZACNMC>{78}</ZACNMC>
            <ZACNPC>{79}</ZACNPC>
            <ZAASVL>{80}</ZAASVL>
            <ZHSNSAC>{81}</ZHSNSAC>
            <ZGSTIN>{82}</ZGSTIN>
            <VEHNO>{83}</VEHNO>
            <RETDC>{84}</RETDC>
<ZATCS>{TCS4}</ZATCS>
<ZATOLC>0</ZATOLC>
<ZLOTCODE>{LOT}</ZLOTCODE>
<EWAYBILL>{EWAY}</EWAYBILL>
<EINVNO>{EINVNO}</EINVNO>
<ZMFGDT>{2}</ZMFGDT>
<ZNUM1>{4N1}</ZNUM1>
<ZNUM2>{4N2}</ZNUM2>
<ZNUM3>{4N3}</ZNUM3>
<ZNUM4>{4N4}</ZNUM4>
<ZNUM5>{4N5}</ZNUM5>
<ZNUM6>{4N6}</ZNUM6>
<ZNUM7>{4N7}</ZNUM7>
<ZCHAR1>{4C1}</ZCHAR1>
<ZCHAR2>{4C2}</ZCHAR2>
<ZCHAR3>{4C3}</ZCHAR3>
<ZCHAR4>{4C4}</ZCHAR4>
<ZCHAR5>{4C5}</ZCHAR5>
<ZDATE1>{2}</ZDATE1>
<ZDATE2>{2}</ZDATE2>
<ZDATE3>{2}</ZDATE3>
<ZDATE4>{2}</ZDATE4>
<ZDATE5>{2}</ZDATE5>     
          </TAB_DATA_HEADER>
          <TAB_DATA_HEADER>
            <IVNUM>{85}</IVNUM>
            <IVDAT>{86}</IVDAT>
            <LIFNR>{87}</LIFNR>
            <ZSHOP>{88}</ZSHOP>
            <EBELN>{89}</EBELN>
            <MATNR>{90}</MATNR>
            <IVQTY>{91}</IVQTY>
            <ZAIVAMT>{92}</ZAIVAMT>
            <ZANETPR>{93}</ZANETPR>
            <ZANETWR>{94}</ZANETWR>
            <ZCGST>{95}</ZCGST>
            <ZSGST>{96}</ZSGST>
            <ZIGST>{97}</ZIGST>
            <ZADTC2>{98}</ZADTC2>
            <ZACNMC>{99}</ZACNMC>
            <ZACNPC>{100}</ZACNPC>
            <ZAASVL>{101}</ZAASVL>
            <ZHSNSAC>{102}</ZHSNSAC>
            <ZGSTIN>{103}</ZGSTIN>
            <VEHNO>{104}</VEHNO>
            <RETDC>{105}</RETDC>
<ZATCS>{TCS5}</ZATCS>
<ZATOLC>0</ZATOLC>
<ZLOTCODE>{LOT}</ZLOTCODE>
<EWAYBILL>{EWAY}</EWAYBILL>
<EINVNO>{EINVNO}</EINVNO>
<ZMFGDT>{2}</ZMFGDT>
<ZNUM1>{5N1}</ZNUM1>
<ZNUM2>{5N2}</ZNUM2>
<ZNUM3>{5N3}</ZNUM3>
<ZNUM4>{5N4}</ZNUM4>
<ZNUM5>{5N5}</ZNUM5>
<ZNUM6>{5N6}</ZNUM6>
<ZNUM7>{5N7}</ZNUM7>
<ZCHAR1>{5C1}</ZCHAR1>
<ZCHAR2>{5C2}</ZCHAR2>
<ZCHAR3>{5C3}</ZCHAR3>
<ZCHAR4>{5C4}</ZCHAR4>
<ZCHAR5>{5C5}</ZCHAR5>
<ZDATE1>{2}</ZDATE1>
<ZDATE2>{2}</ZDATE2>
<ZDATE3>{2}</ZDATE3>
<ZDATE4>{2}</ZDATE4>
<ZDATE5>{2}</ZDATE5>     
          </TAB_DATA_HEADER>
         </GET_DATA>
      </data01>
    </getData>
                              </soap:Body>
                            </soap:Envelope>";
                    }
                    else if (count_rec == 6)
                    {
                        soapStr =
                        @"<?xml version=""1.0"" encoding=""utf-8""?>
                            <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
                               xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
                               xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                              <soap:Body>
                              <getData xmlns=""{APISOAP}"">
      <HEXADECIMAL>{0}</HEXADECIMAL>
      <data01>
        <GET_DATA>
          <TAB_DATA_HEADER>
            <IVNUM>{1}</IVNUM>
            <IVDAT>{2}</IVDAT>
            <LIFNR>{3}</LIFNR>
            <ZSHOP>{4}</ZSHOP>
            <EBELN>{5}</EBELN>
            <MATNR>{6}</MATNR>
            <IVQTY>{7}</IVQTY>
            <ZAIVAMT>{8}</ZAIVAMT>
            <ZANETPR>{9}</ZANETPR>
            <ZANETWR>{10}</ZANETWR>
            <ZCGST>{11}</ZCGST>
            <ZSGST>{12}</ZSGST>
            <ZIGST>{13}</ZIGST>
            <ZADTC2>{14}</ZADTC2>
            <ZACNMC>{15}</ZACNMC>
            <ZACNPC>{16}</ZACNPC>
            <ZAASVL>{17}</ZAASVL>
            <ZHSNSAC>{18}</ZHSNSAC>
            <ZGSTIN>{19}</ZGSTIN>
            <VEHNO>{20}</VEHNO>
            <RETDC>{21}</RETDC>
<ZATCS>{TCS1}</ZATCS>
<ZATOLC>0</ZATOLC>
<ZLOTCODE>{LOT}</ZLOTCODE>
<EWAYBILL>{EWAY}</EWAYBILL>
<EINVNO>{EINVNO}</EINVNO>
<ZMFGDT>{2}</ZMFGDT>
<ZNUM1>{1N1}</ZNUM1>
<ZNUM2>{1N2}</ZNUM2>
<ZNUM3>{1N3}</ZNUM3>
<ZNUM4>{1N4}</ZNUM4>
<ZNUM5>{1N5}</ZNUM5>
<ZNUM6>{1N6}</ZNUM6>
<ZNUM7>{1N7}</ZNUM7>
<ZCHAR1>{1C1}</ZCHAR1>
<ZCHAR2>{1C2}</ZCHAR2>
<ZCHAR3>{1C3}</ZCHAR3>
<ZCHAR4>{1C4}</ZCHAR4>
<ZCHAR5>{1C5}</ZCHAR5>
<ZDATE1>{2}</ZDATE1>
<ZDATE2>{2}</ZDATE2>
<ZDATE3>{2}</ZDATE3>
<ZDATE4>{2}</ZDATE4>
<ZDATE5>{2}</ZDATE5>
          </TAB_DATA_HEADER>
          <TAB_DATA_HEADER>
            <IVNUM>{22}</IVNUM>
            <IVDAT>{23}</IVDAT>
            <LIFNR>{24}</LIFNR>
            <ZSHOP>{25}</ZSHOP>
            <EBELN>{26}</EBELN>
            <MATNR>{27}</MATNR>
            <IVQTY>{28}</IVQTY>
            <ZAIVAMT>{29}</ZAIVAMT>
            <ZANETPR>{30}</ZANETPR>
            <ZANETWR>{31}</ZANETWR>
            <ZCGST>{32}</ZCGST>
            <ZSGST>{33}</ZSGST>
            <ZIGST>{34}</ZIGST>
            <ZADTC2>{35}</ZADTC2>
            <ZACNMC>{36}</ZACNMC>
            <ZACNPC>{37}</ZACNPC>
            <ZAASVL>{38}</ZAASVL>
            <ZHSNSAC>{39}</ZHSNSAC>
            <ZGSTIN>{40}</ZGSTIN>
            <VEHNO>{41}</VEHNO>
            <RETDC>{42}</RETDC>
<ZATCS>{TCS2}</ZATCS>
<ZATOLC>0</ZATOLC>
<ZLOTCODE>{LOT}</ZLOTCODE>
<EWAYBILL>{EWAY}</EWAYBILL>
<EINVNO>{EINVNO}</EINVNO>
<ZMFGDT>{2}</ZMFGDT>
<ZNUM1>{2N1}</ZNUM1>
<ZNUM2>{2N2}</ZNUM2>
<ZNUM3>{2N3}</ZNUM3>
<ZNUM4>{2N4}</ZNUM4>
<ZNUM5>{2N5}</ZNUM5>
<ZNUM6>{2N6}</ZNUM6>
<ZNUM7>{2N7}</ZNUM7>
<ZCHAR1>{2C1}</ZCHAR1>
<ZCHAR2>{2C2}</ZCHAR2>
<ZCHAR3>{2C3}</ZCHAR3>
<ZCHAR4>{2C4}</ZCHAR4>
<ZCHAR5>{2C5}</ZCHAR5>
<ZDATE1>{2}</ZDATE1>
<ZDATE2>{2}</ZDATE2>
<ZDATE3>{2}</ZDATE3>
<ZDATE4>{2}</ZDATE4>
<ZDATE5>{2}</ZDATE5>
          </TAB_DATA_HEADER>
          <TAB_DATA_HEADER>
            <IVNUM>{43}</IVNUM>
            <IVDAT>{44}</IVDAT>
            <LIFNR>{45}</LIFNR>
            <ZSHOP>{46}</ZSHOP>
            <EBELN>{47}</EBELN>
            <MATNR>{48}</MATNR>
            <IVQTY>{49}</IVQTY>
            <ZAIVAMT>{50}</ZAIVAMT>
            <ZANETPR>{51}</ZANETPR>
            <ZANETWR>{52}</ZANETWR>
            <ZCGST>{53}</ZCGST>
            <ZSGST>{54}</ZSGST>
            <ZIGST>{55}</ZIGST>
            <ZADTC2>{56}</ZADTC2>
            <ZACNMC>{57}</ZACNMC>
            <ZACNPC>{58}</ZACNPC>
            <ZAASVL>{59}</ZAASVL>
            <ZHSNSAC>{60}</ZHSNSAC>
            <ZGSTIN>{61}</ZGSTIN>
            <VEHNO>{62}</VEHNO>
            <RETDC>{63}</RETDC>
<ZATCS>{TCS3}</ZATCS>
<ZATOLC>0</ZATOLC>
<ZLOTCODE>{LOT}</ZLOTCODE>
<EWAYBILL>{EWAY}</EWAYBILL>
<EINVNO>{EINVNO}</EINVNO>
<ZMFGDT>{2}</ZMFGDT>
<ZNUM1>{3N1}</ZNUM1>
<ZNUM2>{3N2}</ZNUM2>
<ZNUM3>{3N3}</ZNUM3>
<ZNUM4>{3N4}</ZNUM4>
<ZNUM5>{3N5}</ZNUM5>
<ZNUM6>{3N6}</ZNUM6>
<ZNUM7>{3N7}</ZNUM7>
<ZCHAR1>{3C1}</ZCHAR1>
<ZCHAR2>{3C2}</ZCHAR2>
<ZCHAR3>{3C3}</ZCHAR3>
<ZCHAR4>{3C4}</ZCHAR4>
<ZCHAR5>{3C5}</ZCHAR5>
<ZDATE1>{2}</ZDATE1>
<ZDATE2>{2}</ZDATE2>
<ZDATE3>{2}</ZDATE3>
<ZDATE4>{2}</ZDATE4>
<ZDATE5>{2}</ZDATE5>
          </TAB_DATA_HEADER>
          <TAB_DATA_HEADER>
            <IVNUM>{64}</IVNUM>
            <IVDAT>{65}</IVDAT>
            <LIFNR>{66}</LIFNR>
            <ZSHOP>{67}</ZSHOP>
            <EBELN>{68}</EBELN>
            <MATNR>{69}</MATNR>
            <IVQTY>{70}</IVQTY>
            <ZAIVAMT>{71}</ZAIVAMT>
            <ZANETPR>{72}</ZANETPR>
            <ZANETWR>{73}</ZANETWR>
            <ZCGST>{74}</ZCGST>
            <ZSGST>{75}</ZSGST>
            <ZIGST>{76}</ZIGST>
            <ZADTC2>{77}</ZADTC2>
            <ZACNMC>{78}</ZACNMC>
            <ZACNPC>{79}</ZACNPC>
            <ZAASVL>{80}</ZAASVL>
            <ZHSNSAC>{81}</ZHSNSAC>
            <ZGSTIN>{82}</ZGSTIN>
            <VEHNO>{83}</VEHNO>
            <RETDC>{84}</RETDC>
<ZATCS>{TCS4}</ZATCS>
<ZATOLC>0</ZATOLC>
<ZLOTCODE>{LOT}</ZLOTCODE>
<EWAYBILL>{EWAY}</EWAYBILL>
<EINVNO>{EINVNO}</EINVNO>
<ZMFGDT>{2}</ZMFGDT>
<ZNUM1>{4N1}</ZNUM1>
<ZNUM2>{4N2}</ZNUM2>
<ZNUM3>{4N3}</ZNUM3>
<ZNUM4>{4N4}</ZNUM4>
<ZNUM5>{4N5}</ZNUM5>
<ZNUM6>{4N6}</ZNUM6>
<ZNUM7>{4N7}</ZNUM7>
<ZCHAR1>{4C1}</ZCHAR1>
<ZCHAR2>{4C2}</ZCHAR2>
<ZCHAR3>{4C3}</ZCHAR3>
<ZCHAR4>{4C4}</ZCHAR4>
<ZCHAR5>{4C5}</ZCHAR5>
<ZDATE1>{2}</ZDATE1>
<ZDATE2>{2}</ZDATE2>
<ZDATE3>{2}</ZDATE3>
<ZDATE4>{2}</ZDATE4>
<ZDATE5>{2}</ZDATE5>     
          </TAB_DATA_HEADER>
          <TAB_DATA_HEADER>
            <IVNUM>{85}</IVNUM>
            <IVDAT>{86}</IVDAT>
            <LIFNR>{87}</LIFNR>
            <ZSHOP>{88}</ZSHOP>
            <EBELN>{89}</EBELN>
            <MATNR>{90}</MATNR>
            <IVQTY>{91}</IVQTY>
            <ZAIVAMT>{92}</ZAIVAMT>
            <ZANETPR>{93}</ZANETPR>
            <ZANETWR>{94}</ZANETWR>
            <ZCGST>{95}</ZCGST>
            <ZSGST>{96}</ZSGST>
            <ZIGST>{97}</ZIGST>
            <ZADTC2>{98}</ZADTC2>
            <ZACNMC>{99}</ZACNMC>
            <ZACNPC>{100}</ZACNPC>
            <ZAASVL>{101}</ZAASVL>
            <ZHSNSAC>{102}</ZHSNSAC>
            <ZGSTIN>{103}</ZGSTIN>
            <VEHNO>{104}</VEHNO>
            <RETDC>{105}</RETDC>
<ZATCS>{TCS5}</ZATCS>
<ZATOLC>0</ZATOLC>
<ZLOTCODE>{LOT}</ZLOTCODE>
<EWAYBILL>{EWAY}</EWAYBILL>
<EINVNO>{EINVNO}</EINVNO>
<ZMFGDT>{2}</ZMFGDT>
<ZNUM1>{5N1}</ZNUM1>
<ZNUM2>{5N2}</ZNUM2>
<ZNUM3>{5N3}</ZNUM3>
<ZNUM4>{5N4}</ZNUM4>
<ZNUM5>{5N5}</ZNUM5>
<ZNUM6>{5N6}</ZNUM6>
<ZNUM7>{5N7}</ZNUM7>
<ZCHAR1>{5C1}</ZCHAR1>
<ZCHAR2>{5C2}</ZCHAR2>
<ZCHAR3>{5C3}</ZCHAR3>
<ZCHAR4>{5C4}</ZCHAR4>
<ZCHAR5>{5C5}</ZCHAR5>
<ZDATE1>{2}</ZDATE1>
<ZDATE2>{2}</ZDATE2>
<ZDATE3>{2}</ZDATE3>
<ZDATE4>{2}</ZDATE4>
<ZDATE5>{2}</ZDATE5>    
          </TAB_DATA_HEADER>
          <TAB_DATA_HEADER>
            <IVNUM>{106}</IVNUM>
            <IVDAT>{107}</IVDAT>
            <LIFNR>{108}</LIFNR>
            <ZSHOP>{109}</ZSHOP>
            <EBELN>{110}</EBELN>
            <MATNR>{111}</MATNR>
            <IVQTY>{112}</IVQTY>
            <ZAIVAMT>{113}</ZAIVAMT>
            <ZANETPR>{114}</ZANETPR>
            <ZANETWR>{115}</ZANETWR>
            <ZCGST>{116}</ZCGST>
            <ZSGST>{117}</ZSGST>
            <ZIGST>{118}</ZIGST>
            <ZADTC2>{119}</ZADTC2>
            <ZACNMC>{120}</ZACNMC>
            <ZACNPC>{121}</ZACNPC>
            <ZAASVL>{122}</ZAASVL>
            <ZHSNSAC>{123}</ZHSNSAC>
            <ZGSTIN>{124}</ZGSTIN>
            <VEHNO>{125}</VEHNO>
            <RETDC>{126}</RETDC>
<ZATCS>{TCS6}</ZATCS>
<ZATOLC>0</ZATOLC>
<ZLOTCODE>{LOT}</ZLOTCODE>
<EWAYBILL>{EWAY}</EWAYBILL>
<EINVNO>{EINVNO}</EINVNO>
<ZMFGDT>{2}</ZMFGDT>
<ZNUM1>{6N1}</ZNUM1>
<ZNUM2>{6N2}</ZNUM2>
<ZNUM3>{6N3}</ZNUM3>
<ZNUM4>{6N4}</ZNUM4>
<ZNUM5>{6N5}</ZNUM5>
<ZNUM6>{6N6}</ZNUM6>
<ZNUM7>{6N7}</ZNUM7>
<ZCHAR1>{6C1}</ZCHAR1>
<ZCHAR2>{6C2}</ZCHAR2>
<ZCHAR3>{6C3}</ZCHAR3>
<ZCHAR4>{6C4}</ZCHAR4>
<ZCHAR5>{6C5}</ZCHAR5>
<ZDATE1>{2}</ZDATE1>
<ZDATE2>{2}</ZDATE2>
<ZDATE3>{2}</ZDATE3>
<ZDATE4>{2}</ZDATE4>
<ZDATE5>{2}</ZDATE5>    
          </TAB_DATA_HEADER>
         </GET_DATA>
      </data01>
    </getData>
                              </soap:Body>
                            </soap:Envelope>";
                    }
                    else if (count_rec == 7)
                    {
                        soapStr =
                        @"<?xml version=""1.0"" encoding=""utf-8""?>
                            <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
                               xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
                               xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                              <soap:Body>
                              <getData xmlns=""{APISOAP}"">
      <HEXADECIMAL>{0}</HEXADECIMAL>
      <data01>
        <GET_DATA>
          <TAB_DATA_HEADER>
            <IVNUM>{1}</IVNUM>
            <IVDAT>{2}</IVDAT>
            <LIFNR>{3}</LIFNR>
            <ZSHOP>{4}</ZSHOP>
            <EBELN>{5}</EBELN>
            <MATNR>{6}</MATNR>
            <IVQTY>{7}</IVQTY>
            <ZAIVAMT>{8}</ZAIVAMT>
            <ZANETPR>{9}</ZANETPR>
            <ZANETWR>{10}</ZANETWR>
            <ZCGST>{11}</ZCGST>
            <ZSGST>{12}</ZSGST>
            <ZIGST>{13}</ZIGST>
            <ZADTC2>{14}</ZADTC2>
            <ZACNMC>{15}</ZACNMC>
            <ZACNPC>{16}</ZACNPC>
            <ZAASVL>{17}</ZAASVL>
            <ZHSNSAC>{18}</ZHSNSAC>
            <ZGSTIN>{19}</ZGSTIN>
            <VEHNO>{20}</VEHNO>
            <RETDC>{21}</RETDC>
<ZATCS>{TCS1}</ZATCS>
<ZATOLC>0</ZATOLC>
<ZLOTCODE>{LOT}</ZLOTCODE>
<EWAYBILL>{EWAY}</EWAYBILL>
<EINVNO>{EINVNO}</EINVNO>
<ZMFGDT>{2}</ZMFGDT>
<ZNUM1>{1N1}</ZNUM1>
<ZNUM2>{1N2}</ZNUM2>
<ZNUM3>{1N3}</ZNUM3>
<ZNUM4>{1N4}</ZNUM4>
<ZNUM5>{1N5}</ZNUM5>
<ZNUM6>{1N6}</ZNUM6>
<ZNUM7>{1N7}</ZNUM7>
<ZCHAR1>{1C1}</ZCHAR1>
<ZCHAR2>{1C2}</ZCHAR2>
<ZCHAR3>{1C3}</ZCHAR3>
<ZCHAR4>{1C4}</ZCHAR4>
<ZCHAR5>{1C5}</ZCHAR5>
<ZDATE1>{2}</ZDATE1>
<ZDATE2>{2}</ZDATE2>
<ZDATE3>{2}</ZDATE3>
<ZDATE4>{2}</ZDATE4>
<ZDATE5>{2}</ZDATE5>          </TAB_DATA_HEADER>
          <TAB_DATA_HEADER>
            <IVNUM>{22}</IVNUM>
            <IVDAT>{23}</IVDAT>
            <LIFNR>{24}</LIFNR>
            <ZSHOP>{25}</ZSHOP>
            <EBELN>{26}</EBELN>
            <MATNR>{27}</MATNR>
            <IVQTY>{28}</IVQTY>
            <ZAIVAMT>{29}</ZAIVAMT>
            <ZANETPR>{30}</ZANETPR>
            <ZANETWR>{31}</ZANETWR>
            <ZCGST>{32}</ZCGST>
            <ZSGST>{33}</ZSGST>
            <ZIGST>{34}</ZIGST>
            <ZADTC2>{35}</ZADTC2>
            <ZACNMC>{36}</ZACNMC>
            <ZACNPC>{37}</ZACNPC>
            <ZAASVL>{38}</ZAASVL>
            <ZHSNSAC>{39}</ZHSNSAC>
            <ZGSTIN>{40}</ZGSTIN>
            <VEHNO>{41}</VEHNO>
            <RETDC>{42}</RETDC>
<ZATCS>{TCS2}</ZATCS>
<ZATOLC>0</ZATOLC>
<ZLOTCODE>{LOT}</ZLOTCODE>
<EWAYBILL>{EWAY}</EWAYBILL>
<EINVNO>{EINVNO}</EINVNO>
<ZMFGDT>{2}</ZMFGDT>
<ZNUM1>{2N1}</ZNUM1>
<ZNUM2>{2N2}</ZNUM2>
<ZNUM3>{2N3}</ZNUM3>
<ZNUM4>{2N4}</ZNUM4>
<ZNUM5>{2N5}</ZNUM5>
<ZNUM6>{2N6}</ZNUM6>
<ZNUM7>{2N7}</ZNUM7>
<ZCHAR1>{2C1}</ZCHAR1>
<ZCHAR2>{2C2}</ZCHAR2>
<ZCHAR3>{2C3}</ZCHAR3>
<ZCHAR4>{2C4}</ZCHAR4>
<ZCHAR5>{2C5}</ZCHAR5>
<ZDATE1>{2}</ZDATE1>
<ZDATE2>{2}</ZDATE2>
<ZDATE3>{2}</ZDATE3>
<ZDATE4>{2}</ZDATE4>
<ZDATE5>{2}</ZDATE5>
          </TAB_DATA_HEADER>
          <TAB_DATA_HEADER>
            <IVNUM>{43}</IVNUM>
            <IVDAT>{44}</IVDAT>
            <LIFNR>{45}</LIFNR>
            <ZSHOP>{46}</ZSHOP>
            <EBELN>{47}</EBELN>
            <MATNR>{48}</MATNR>
            <IVQTY>{49}</IVQTY>
            <ZAIVAMT>{50}</ZAIVAMT>
            <ZANETPR>{51}</ZANETPR>
            <ZANETWR>{52}</ZANETWR>
            <ZCGST>{53}</ZCGST>
            <ZSGST>{54}</ZSGST>
            <ZIGST>{55}</ZIGST>
            <ZADTC2>{56}</ZADTC2>
            <ZACNMC>{57}</ZACNMC>
            <ZACNPC>{58}</ZACNPC>
            <ZAASVL>{59}</ZAASVL>
            <ZHSNSAC>{60}</ZHSNSAC>
            <ZGSTIN>{61}</ZGSTIN>
            <VEHNO>{62}</VEHNO>
            <RETDC>{63}</RETDC>
<ZATCS>{TCS3}</ZATCS>
<ZATOLC>0</ZATOLC>
<ZLOTCODE>{LOT}</ZLOTCODE>
<EWAYBILL>{EWAY}</EWAYBILL>
<EINVNO>{EINVNO}</EINVNO>
<ZMFGDT>{2}</ZMFGDT>
<ZNUM1>{3N1}</ZNUM1>
<ZNUM2>{3N2}</ZNUM2>
<ZNUM3>{3N3}</ZNUM3>
<ZNUM4>{3N4}</ZNUM4>
<ZNUM5>{3N5}</ZNUM5>
<ZNUM6>{3N6}</ZNUM6>
<ZNUM7>{3N7}</ZNUM7>
<ZCHAR1>{3C1}</ZCHAR1>
<ZCHAR2>{3C2}</ZCHAR2>
<ZCHAR3>{3C3}</ZCHAR3>
<ZCHAR4>{3C4}</ZCHAR4>
<ZCHAR5>{3C5}</ZCHAR5>
<ZDATE1>{2}</ZDATE1>
<ZDATE2>{2}</ZDATE2>
<ZDATE3>{2}</ZDATE3>
<ZDATE4>{2}</ZDATE4>
<ZDATE5>{2}</ZDATE5>
          </TAB_DATA_HEADER>
          <TAB_DATA_HEADER>
            <IVNUM>{64}</IVNUM>
            <IVDAT>{65}</IVDAT>
            <LIFNR>{66}</LIFNR>
            <ZSHOP>{67}</ZSHOP>
            <EBELN>{68}</EBELN>
            <MATNR>{69}</MATNR>
            <IVQTY>{70}</IVQTY>
            <ZAIVAMT>{71}</ZAIVAMT>
            <ZANETPR>{72}</ZANETPR>
            <ZANETWR>{73}</ZANETWR>
            <ZCGST>{74}</ZCGST>
            <ZSGST>{75}</ZSGST>
            <ZIGST>{76}</ZIGST>
            <ZADTC2>{77}</ZADTC2>
            <ZACNMC>{78}</ZACNMC>
            <ZACNPC>{79}</ZACNPC>
            <ZAASVL>{80}</ZAASVL>
            <ZHSNSAC>{81}</ZHSNSAC>
            <ZGSTIN>{82}</ZGSTIN>
            <VEHNO>{83}</VEHNO>
<ZATCS>{TCS4}</ZATCS>
<ZATOLC>0</ZATOLC>
<ZLOTCODE>{LOT}</ZLOTCODE>
<EWAYBILL>{EWAY}</EWAYBILL>
<EINVNO>{EINVNO}</EINVNO>
<ZMFGDT>{2}</ZMFGDT>
<ZNUM1>{4N1}</ZNUM1>
<ZNUM2>{4N2}</ZNUM2>
<ZNUM3>{4N3}</ZNUM3>
<ZNUM4>{4N4}</ZNUM4>
<ZNUM5>{4N5}</ZNUM5>
<ZNUM6>{4N6}</ZNUM6>
<ZNUM7>{4N7}</ZNUM7>
<ZCHAR1>{4C1}</ZCHAR1>
<ZCHAR2>{4C2}</ZCHAR2>
<ZCHAR3>{4C3}</ZCHAR3>
<ZCHAR4>{4C4}</ZCHAR4>
<ZCHAR5>{4C5}</ZCHAR5>
<ZDATE1>{2}</ZDATE1>
<ZDATE2>{2}</ZDATE2>
<ZDATE3>{2}</ZDATE3>
<ZDATE4>{2}</ZDATE4>
<ZDATE5>{2}</ZDATE5>     
    </TAB_DATA_HEADER>
          <TAB_DATA_HEADER>
            <IVNUM>{85}</IVNUM>
            <IVDAT>{86}</IVDAT>
            <LIFNR>{87}</LIFNR>
            <ZSHOP>{88}</ZSHOP>
            <EBELN>{89}</EBELN>
            <MATNR>{90}</MATNR>
            <IVQTY>{91}</IVQTY>
            <ZAIVAMT>{92}</ZAIVAMT>
            <ZANETPR>{93}</ZANETPR>
            <ZANETWR>{94}</ZANETWR>
            <ZCGST>{95}</ZCGST>
            <ZSGST>{96}</ZSGST>
            <ZIGST>{97}</ZIGST>
            <ZADTC2>{98}</ZADTC2>
            <ZACNMC>{99}</ZACNMC>
            <ZACNPC>{100}</ZACNPC>
            <ZAASVL>{101}</ZAASVL>
            <ZHSNSAC>{102}</ZHSNSAC>
            <ZGSTIN>{103}</ZGSTIN>
            <VEHNO>{104}</VEHNO>
            <RETDC>{105}</RETDC>
<ZATCS>{TCS5}</ZATCS>
<ZATOLC>0</ZATOLC>
<ZLOTCODE>{LOT}</ZLOTCODE>
<EWAYBILL>{EWAY}</EWAYBILL>
<EINVNO>{EINVNO}</EINVNO>
<ZMFGDT>{2}</ZMFGDT>
<ZNUM1>{5N1}</ZNUM1>
<ZNUM2>{5N2}</ZNUM2>
<ZNUM3>{5N3}</ZNUM3>
<ZNUM4>{5N4}</ZNUM4>
<ZNUM5>{5N5}</ZNUM5>
<ZNUM6>{5N6}</ZNUM6>
<ZNUM7>{5N7}</ZNUM7>
<ZCHAR1>{5C1}</ZCHAR1>
<ZCHAR2>{5C2}</ZCHAR2>
<ZCHAR3>{5C3}</ZCHAR3>
<ZCHAR4>{5C4}</ZCHAR4>
<ZCHAR5>{5C5}</ZCHAR5>
<ZDATE1>{2}</ZDATE1>
<ZDATE2>{2}</ZDATE2>
<ZDATE3>{2}</ZDATE3>
<ZDATE4>{2}</ZDATE4>
<ZDATE5>{2}</ZDATE5>    
          </TAB_DATA_HEADER>
          <TAB_DATA_HEADER>
            <IVNUM>{106}</IVNUM>
            <IVDAT>{107}</IVDAT>
            <LIFNR>{108}</LIFNR>
            <ZSHOP>{109}</ZSHOP>
            <EBELN>{110}</EBELN>
            <MATNR>{111}</MATNR>
            <IVQTY>{112}</IVQTY>
            <ZAIVAMT>{113}</ZAIVAMT>
            <ZANETPR>{114}</ZANETPR>
            <ZANETWR>{115}</ZANETWR>
            <ZCGST>{116}</ZCGST>
            <ZSGST>{117}</ZSGST>
            <ZIGST>{118}</ZIGST>
            <ZADTC2>{119}</ZADTC2>
            <ZACNMC>{120}</ZACNMC>
            <ZACNPC>{121}</ZACNPC>
            <ZAASVL>{122}</ZAASVL>
            <ZHSNSAC>{123}</ZHSNSAC>
            <ZGSTIN>{124}</ZGSTIN>
            <VEHNO>{125}</VEHNO>
            <RETDC>{126}</RETDC>
<ZATCS>{TCS6}</ZATCS>
<ZATOLC>0</ZATOLC>
<ZLOTCODE>{LOT}</ZLOTCODE>
<EWAYBILL>{EWAY}</EWAYBILL>
<EINVNO>{EINVNO}</EINVNO>
<ZMFGDT>{2}</ZMFGDT>
<ZNUM1>{6N1}</ZNUM1>
<ZNUM2>{6N2}</ZNUM2>
<ZNUM3>{6N3}</ZNUM3>
<ZNUM4>{6N4}</ZNUM4>
<ZNUM5>{6N5}</ZNUM5>
<ZNUM6>{6N6}</ZNUM6>
<ZNUM7>{6N7}</ZNUM7>
<ZCHAR1>{6C1}</ZCHAR1>
<ZCHAR2>{6C2}</ZCHAR2>
<ZCHAR3>{6C3}</ZCHAR3>
<ZCHAR4>{6C4}</ZCHAR4>
<ZCHAR5>{6C5}</ZCHAR5>
<ZDATE1>{2}</ZDATE1>
<ZDATE2>{2}</ZDATE2>
<ZDATE3>{2}</ZDATE3>
<ZDATE4>{2}</ZDATE4>
<ZDATE5>{2}</ZDATE5> 
          </TAB_DATA_HEADER>
          <TAB_DATA_HEADER>
            <IVNUM>{127}</IVNUM>
            <IVDAT>{128}</IVDAT>
            <LIFNR>{129}</LIFNR>
            <ZSHOP>{130}</ZSHOP>
            <EBELN>{131}</EBELN>
            <MATNR>{132}</MATNR>
            <IVQTY>{133}</IVQTY>
            <ZAIVAMT>{134}</ZAIVAMT>
            <ZANETPR>{135}</ZANETPR>
            <ZANETWR>{136}</ZANETWR>
            <ZCGST>{137}</ZCGST>
            <ZSGST>{138}</ZSGST>
            <ZIGST>{139}</ZIGST>
            <ZADTC2>{140}</ZADTC2>
            <ZACNMC>{141}</ZACNMC>
            <ZACNPC>{142}</ZACNPC>
            <ZAASVL>{143}</ZAASVL>
            <ZHSNSAC>{144}</ZHSNSAC>
            <ZGSTIN>{145}</ZGSTIN>
            <VEHNO>{146}</VEHNO>
            <RETDC>{147}</RETDC>
<ZATCS>{TCS7}</ZATCS>
<ZATOLC>0</ZATOLC>
<ZLOTCODE>{LOT}</ZLOTCODE>
<EWAYBILL>{EWAY}</EWAYBILL>
<EINVNO>{EINVNO}</EINVNO>
<ZMFGDT>{2}</ZMFGDT>
<ZNUM1>{7N1}</ZNUM1>
<ZNUM2>{7N2}</ZNUM2>
<ZNUM3>{7N3}</ZNUM3>
<ZNUM4>{7N4}</ZNUM4>
<ZNUM5>{7N5}</ZNUM5>
<ZNUM6>{7N6}</ZNUM6>
<ZNUM7>{7N7}</ZNUM7>
<ZCHAR1>{7C1}</ZCHAR1>
<ZCHAR2>{7C2}</ZCHAR2>
<ZCHAR3>{7C3}</ZCHAR3>
<ZCHAR4>{7C4}</ZCHAR4>
<ZCHAR5>{7C5}</ZCHAR5>
<ZDATE1>{2}</ZDATE1>
<ZDATE2>{2}</ZDATE2>
<ZDATE3>{2}</ZDATE3>
<ZDATE4>{2}</ZDATE4>
<ZDATE5>{2}</ZDATE5> 
          </TAB_DATA_HEADER>
         </GET_DATA>
      </data01>
    </getData>
                              </soap:Body>
                            </soap:Envelope>";
                    }
                    else if (count_rec == 8)
                    {
                        soapStr =
                        @"<?xml version=""1.0"" encoding=""utf-8""?>
                            <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
                               xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
                               xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                              <soap:Body>
                              <getData xmlns=""{APISOAP}"">
      <HEXADECIMAL>{0}</HEXADECIMAL>
      <data01>
        <GET_DATA>
          <TAB_DATA_HEADER>
            <IVNUM>{1}</IVNUM>
            <IVDAT>{2}</IVDAT>
            <LIFNR>{3}</LIFNR>
            <ZSHOP>{4}</ZSHOP>
            <EBELN>{5}</EBELN>
            <MATNR>{6}</MATNR>
            <IVQTY>{7}</IVQTY>
            <ZAIVAMT>{8}</ZAIVAMT>
            <ZANETPR>{9}</ZANETPR>
            <ZANETWR>{10}</ZANETWR>
            <ZCGST>{11}</ZCGST>
            <ZSGST>{12}</ZSGST>
            <ZIGST>{13}</ZIGST>
            <ZADTC2>{14}</ZADTC2>
            <ZACNMC>{15}</ZACNMC>
            <ZACNPC>{16}</ZACNPC>
            <ZAASVL>{17}</ZAASVL>
            <ZHSNSAC>{18}</ZHSNSAC>
            <ZGSTIN>{19}</ZGSTIN>
            <VEHNO>{20}</VEHNO>
            <RETDC>{21}</RETDC>
<ZATCS>{TCS1}</ZATCS>
<ZATOLC>0</ZATOLC>
<ZLOTCODE>{LOT}</ZLOTCODE>
<EWAYBILL>{EWAY}</EWAYBILL>
<EINVNO>{EINVNO}</EINVNO>
<ZMFGDT>{2}</ZMFGDT>
<ZNUM1>{1N1}</ZNUM1>
<ZNUM2>{1N2}</ZNUM2>
<ZNUM3>{1N3}</ZNUM3>
<ZNUM4>{1N4}</ZNUM4>
<ZNUM5>{1N5}</ZNUM5>
<ZNUM6>{1N6}</ZNUM6>
<ZNUM7>{1N7}</ZNUM7>
<ZCHAR1>{1C1}</ZCHAR1>
<ZCHAR2>{1C2}</ZCHAR2>
<ZCHAR3>{1C3}</ZCHAR3>
<ZCHAR4>{1C4}</ZCHAR4>
<ZCHAR5>{1C5}</ZCHAR5>
<ZDATE1>{2}</ZDATE1>
<ZDATE2>{2}</ZDATE2>
<ZDATE3>{2}</ZDATE3>
<ZDATE4>{2}</ZDATE4>
<ZDATE5>{2}</ZDATE5>
          </TAB_DATA_HEADER>
          <TAB_DATA_HEADER>
            <IVNUM>{22}</IVNUM>
            <IVDAT>{23}</IVDAT>
            <LIFNR>{24}</LIFNR>
            <ZSHOP>{25}</ZSHOP>
            <EBELN>{26}</EBELN>
            <MATNR>{27}</MATNR>
            <IVQTY>{28}</IVQTY>
            <ZAIVAMT>{29}</ZAIVAMT>
            <ZANETPR>{30}</ZANETPR>
            <ZANETWR>{31}</ZANETWR>
            <ZCGST>{32}</ZCGST>
            <ZSGST>{33}</ZSGST>
            <ZIGST>{34}</ZIGST>
            <ZADTC2>{35}</ZADTC2>
            <ZACNMC>{36}</ZACNMC>
            <ZACNPC>{37}</ZACNPC>
            <ZAASVL>{38}</ZAASVL>
            <ZHSNSAC>{39}</ZHSNSAC>
            <ZGSTIN>{40}</ZGSTIN>
            <VEHNO>{41}</VEHNO>
            <RETDC>{42}</RETDC>
<ZATCS>{TCS2}</ZATCS>
<ZATOLC>0</ZATOLC>
<ZLOTCODE>{LOT}</ZLOTCODE>
<EWAYBILL>{EWAY}</EWAYBILL>
<EINVNO>{EINVNO}</EINVNO>
<ZMFGDT>{2}</ZMFGDT>
<ZNUM1>{2N1}</ZNUM1>
<ZNUM2>{2N2}</ZNUM2>
<ZNUM3>{2N3}</ZNUM3>
<ZNUM4>{2N4}</ZNUM4>
<ZNUM5>{2N5}</ZNUM5>
<ZNUM6>{2N6}</ZNUM6>
<ZNUM7>{2N7}</ZNUM7>
<ZCHAR1>{2C1}</ZCHAR1>
<ZCHAR2>{2C2}</ZCHAR2>
<ZCHAR3>{2C3}</ZCHAR3>
<ZCHAR4>{2C4}</ZCHAR4>
<ZCHAR5>{2C5}</ZCHAR5>
<ZDATE1>{2}</ZDATE1>
<ZDATE2>{2}</ZDATE2>
<ZDATE3>{2}</ZDATE3>
<ZDATE4>{2}</ZDATE4>
<ZDATE5>{2}</ZDATE5>
          </TAB_DATA_HEADER>
          <TAB_DATA_HEADER>
            <IVNUM>{43}</IVNUM>
            <IVDAT>{44}</IVDAT>
            <LIFNR>{45}</LIFNR>
            <ZSHOP>{46}</ZSHOP>
            <EBELN>{47}</EBELN>
            <MATNR>{48}</MATNR>
            <IVQTY>{49}</IVQTY>
            <ZAIVAMT>{50}</ZAIVAMT>
            <ZANETPR>{51}</ZANETPR>
            <ZANETWR>{52}</ZANETWR>
            <ZCGST>{53}</ZCGST>
            <ZSGST>{54}</ZSGST>
            <ZIGST>{55}</ZIGST>
            <ZADTC2>{56}</ZADTC2>
            <ZACNMC>{57}</ZACNMC>
            <ZACNPC>{58}</ZACNPC>
            <ZAASVL>{59}</ZAASVL>
            <ZHSNSAC>{60}</ZHSNSAC>
            <ZGSTIN>{61}</ZGSTIN>
            <VEHNO>{62}</VEHNO>
            <RETDC>{63}</RETDC>
<ZATCS>{TCS3}</ZATCS>
<ZATOLC>0</ZATOLC>
<ZLOTCODE>{LOT}</ZLOTCODE>
<EWAYBILL>{EWAY}</EWAYBILL>
<EINVNO>{EINVNO}</EINVNO>
<ZMFGDT>{2}</ZMFGDT>
<ZNUM1>{3N1}</ZNUM1>
<ZNUM2>{3N2}</ZNUM2>
<ZNUM3>{3N3}</ZNUM3>
<ZNUM4>{3N4}</ZNUM4>
<ZNUM5>{3N5}</ZNUM5>
<ZNUM6>{3N6}</ZNUM6>
<ZNUM7>{3N7}</ZNUM7>
<ZCHAR1>{3C1}</ZCHAR1>
<ZCHAR2>{3C2}</ZCHAR2>
<ZCHAR3>{3C3}</ZCHAR3>
<ZCHAR4>{3C4}</ZCHAR4>
<ZCHAR5>{3C5}</ZCHAR5>
<ZDATE1>{2}</ZDATE1>
<ZDATE2>{2}</ZDATE2>
<ZDATE3>{2}</ZDATE3>
<ZDATE4>{2}</ZDATE4>
<ZDATE5>{2}</ZDATE5>
          </TAB_DATA_HEADER>
          <TAB_DATA_HEADER>
            <IVNUM>{64}</IVNUM>
            <IVDAT>{65}</IVDAT>
            <LIFNR>{66}</LIFNR>
            <ZSHOP>{67}</ZSHOP>
            <EBELN>{68}</EBELN>
            <MATNR>{69}</MATNR>
            <IVQTY>{70}</IVQTY>
            <ZAIVAMT>{71}</ZAIVAMT>
            <ZANETPR>{72}</ZANETPR>
            <ZANETWR>{73}</ZANETWR>
            <ZCGST>{74}</ZCGST>
            <ZSGST>{75}</ZSGST>
            <ZIGST>{76}</ZIGST>
            <ZADTC2>{77}</ZADTC2>
            <ZACNMC>{78}</ZACNMC>
            <ZACNPC>{79}</ZACNPC>
            <ZAASVL>{80}</ZAASVL>
            <ZHSNSAC>{81}</ZHSNSAC>
            <ZGSTIN>{82}</ZGSTIN>
            <VEHNO>{83}</VEHNO>
            <RETDC>{84}</RETDC>
<ZATCS>{TCS4}</ZATCS>
<ZATOLC>0</ZATOLC>
<ZLOTCODE>{LOT}</ZLOTCODE>
<EWAYBILL>{EWAY}</EWAYBILL>
<EINVNO>{EINVNO}</EINVNO>
<ZMFGDT>{2}</ZMFGDT>
<ZNUM1>{4N1}</ZNUM1>
<ZNUM2>{4N2}</ZNUM2>
<ZNUM3>{4N3}</ZNUM3>
<ZNUM4>{4N4}</ZNUM4>
<ZNUM5>{4N5}</ZNUM5>
<ZNUM6>{4N6}</ZNUM6>
<ZNUM7>{4N7}</ZNUM7>
<ZCHAR1>{4C1}</ZCHAR1>
<ZCHAR2>{4C2}</ZCHAR2>
<ZCHAR3>{4C3}</ZCHAR3>
<ZCHAR4>{4C4}</ZCHAR4>
<ZCHAR5>{4C5}</ZCHAR5>
<ZDATE1>{2}</ZDATE1>
<ZDATE2>{2}</ZDATE2>
<ZDATE3>{2}</ZDATE3>
<ZDATE4>{2}</ZDATE4>
<ZDATE5>{2}</ZDATE5>     
          </TAB_DATA_HEADER>
          <TAB_DATA_HEADER>
            <IVNUM>{85}</IVNUM>
            <IVDAT>{86}</IVDAT>
            <LIFNR>{87}</LIFNR>
            <ZSHOP>{88}</ZSHOP>
            <EBELN>{89}</EBELN>
            <MATNR>{90}</MATNR>
            <IVQTY>{91}</IVQTY>
            <ZAIVAMT>{92}</ZAIVAMT>
            <ZANETPR>{93}</ZANETPR>
            <ZANETWR>{94}</ZANETWR>
            <ZCGST>{95}</ZCGST>
            <ZSGST>{96}</ZSGST>
            <ZIGST>{97}</ZIGST>
            <ZADTC2>{98}</ZADTC2>
            <ZACNMC>{99}</ZACNMC>
            <ZACNPC>{100}</ZACNPC>
            <ZAASVL>{101}</ZAASVL>
            <ZHSNSAC>{102}</ZHSNSAC>
            <ZGSTIN>{103}</ZGSTIN>
            <VEHNO>{104}</VEHNO>
            <RETDC>{105}</RETDC>
<ZATCS>{TCS5}</ZATCS>
<ZATOLC>0</ZATOLC>
<ZLOTCODE>{LOT}</ZLOTCODE>
<EWAYBILL>{EWAY}</EWAYBILL>
<EINVNO>{EINVNO}</EINVNO>
<ZMFGDT>{2}</ZMFGDT>
<ZNUM1>{5N1}</ZNUM1>
<ZNUM2>{5N2}</ZNUM2>
<ZNUM3>{5N3}</ZNUM3>
<ZNUM4>{5N4}</ZNUM4>
<ZNUM5>{5N5}</ZNUM5>
<ZNUM6>{5N6}</ZNUM6>
<ZNUM7>{5N7}</ZNUM7>
<ZCHAR1>{5C1}</ZCHAR1>
<ZCHAR2>{5C2}</ZCHAR2>
<ZCHAR3>{5C3}</ZCHAR3>
<ZCHAR4>{5C4}</ZCHAR4>
<ZCHAR5>{5C5}</ZCHAR5>
<ZDATE1>{2}</ZDATE1>
<ZDATE2>{2}</ZDATE2>
<ZDATE3>{2}</ZDATE3>
<ZDATE4>{2}</ZDATE4>
<ZDATE5>{2}</ZDATE5>    
          </TAB_DATA_HEADER>
          <TAB_DATA_HEADER>
            <IVNUM>{106}</IVNUM>
            <IVDAT>{107}</IVDAT>
            <LIFNR>{108}</LIFNR>
            <ZSHOP>{109}</ZSHOP>
            <EBELN>{110}</EBELN>
            <MATNR>{111}</MATNR>
            <IVQTY>{112}</IVQTY>
            <ZAIVAMT>{113}</ZAIVAMT>
            <ZANETPR>{114}</ZANETPR>
            <ZANETWR>{115}</ZANETWR>
            <ZCGST>{116}</ZCGST>
            <ZSGST>{117}</ZSGST>
            <ZIGST>{118}</ZIGST>
            <ZADTC2>{119}</ZADTC2>
            <ZACNMC>{120}</ZACNMC>
            <ZACNPC>{121}</ZACNPC>
            <ZAASVL>{122}</ZAASVL>
            <ZHSNSAC>{123}</ZHSNSAC>
            <ZGSTIN>{124}</ZGSTIN>
            <VEHNO>{125}</VEHNO>
            <RETDC>{126}</RETDC>
<ZATCS>{TCS6}</ZATCS>
<ZATOLC>0</ZATOLC>
<ZLOTCODE>{LOT}</ZLOTCODE>
<EWAYBILL>{EWAY}</EWAYBILL>
<EINVNO>{EINVNO}</EINVNO>
<ZMFGDT>{2}</ZMFGDT>
<ZNUM1>{6N1}</ZNUM1>
<ZNUM2>{6N2}</ZNUM2>
<ZNUM3>{6N3}</ZNUM3>
<ZNUM4>{6N4}</ZNUM4>
<ZNUM5>{6N5}</ZNUM5>
<ZNUM6>{6N6}</ZNUM6>
<ZNUM7>{6N7}</ZNUM7>
<ZCHAR1>{6C1}</ZCHAR1>
<ZCHAR2>{6C2}</ZCHAR2>
<ZCHAR3>{6C3}</ZCHAR3>
<ZCHAR4>{6C4}</ZCHAR4>
<ZCHAR5>{6C5}</ZCHAR5>
<ZDATE1>{2}</ZDATE1>
<ZDATE2>{2}</ZDATE2>
<ZDATE3>{2}</ZDATE3>
<ZDATE4>{2}</ZDATE4>
<ZDATE5>{2}</ZDATE5> 
          </TAB_DATA_HEADER>
          <TAB_DATA_HEADER>
            <IVNUM>{127}</IVNUM>
            <IVDAT>{128}</IVDAT>
            <LIFNR>{129}</LIFNR>
            <ZSHOP>{130}</ZSHOP>
            <EBELN>{131}</EBELN>
            <MATNR>{132}</MATNR>
            <IVQTY>{133}</IVQTY>
            <ZAIVAMT>{134}</ZAIVAMT>
            <ZANETPR>{135}</ZANETPR>
            <ZANETWR>{136}</ZANETWR>
            <ZCGST>{137}</ZCGST>
            <ZSGST>{138}</ZSGST>
            <ZIGST>{139}</ZIGST>
            <ZADTC2>{140}</ZADTC2>
            <ZACNMC>{141}</ZACNMC>
            <ZACNPC>{142}</ZACNPC>
            <ZAASVL>{143}</ZAASVL>
            <ZHSNSAC>{144}</ZHSNSAC>
            <ZGSTIN>{145}</ZGSTIN>
            <VEHNO>{146}</VEHNO>
            <RETDC>{147}</RETDC>
<ZATCS>{TCS7}</ZATCS>
<ZATOLC>0</ZATOLC>
<ZLOTCODE>{LOT}</ZLOTCODE>
<EWAYBILL>{EWAY}</EWAYBILL>
<EINVNO>{EINVNO}</EINVNO>
<ZMFGDT>{2}</ZMFGDT>
<ZNUM1>{7N1}</ZNUM1>
<ZNUM2>{7N2}</ZNUM2>
<ZNUM3>{7N3}</ZNUM3>
<ZNUM4>{7N4}</ZNUM4>
<ZNUM5>{7N5}</ZNUM5>
<ZNUM6>{7N6}</ZNUM6>
<ZNUM7>{7N7}</ZNUM7>
<ZCHAR1>{7C1}</ZCHAR1>
<ZCHAR2>{7C2}</ZCHAR2>
<ZCHAR3>{7C3}</ZCHAR3>
<ZCHAR4>{7C4}</ZCHAR4>
<ZCHAR5>{7C5}</ZCHAR5>
<ZDATE1>{2}</ZDATE1>
<ZDATE2>{2}</ZDATE2>
<ZDATE3>{2}</ZDATE3>
<ZDATE4>{2}</ZDATE4>
<ZDATE5>{2}</ZDATE5> 
          </TAB_DATA_HEADER>
          <TAB_DATA_HEADER>
            <IVNUM>{148}</IVNUM>
            <IVDAT>{149}</IVDAT>
            <LIFNR>{150}</LIFNR>
            <ZSHOP>{151}</ZSHOP>
            <EBELN>{152}</EBELN>
            <MATNR>{153}</MATNR>
            <IVQTY>{154}</IVQTY>
            <ZAIVAMT>{155}</ZAIVAMT>
            <ZANETPR>{156}</ZANETPR>
            <ZANETWR>{157}</ZANETWR>
            <ZCGST>{158}</ZCGST>
            <ZSGST>{159}</ZSGST>
            <ZIGST>{160}</ZIGST>
            <ZADTC2>{161}</ZADTC2>
            <ZACNMC>{162}</ZACNMC>
            <ZACNPC>{163}</ZACNPC>
            <ZAASVL>{164}</ZAASVL>
            <ZHSNSAC>{165}</ZHSNSAC>
            <ZGSTIN>{166}</ZGSTIN>
            <VEHNO>{167}</VEHNO>
            <RETDC>{168}</RETDC>
<ZATCS>{TCS8}</ZATCS>
<ZATOLC>0</ZATOLC>
<ZLOTCODE>{LOT}</ZLOTCODE>
<EWAYBILL>{EWAY}</EWAYBILL>
<EINVNO>{EINVNO}</EINVNO>
<ZMFGDT>{2}</ZMFGDT>
<ZNUM1>{8N1}</ZNUM1>
<ZNUM2>{8N2}</ZNUM2>
<ZNUM3>{8N3}</ZNUM3>
<ZNUM4>{8N4}</ZNUM4>
<ZNUM5>{8N5}</ZNUM5>
<ZNUM6>{8N6}</ZNUM6>
<ZNUM7>{8N7}</ZNUM7>
<ZCHAR1>{8C1}</ZCHAR1>
<ZCHAR2>{8C2}</ZCHAR2>
<ZCHAR3>{8C3}</ZCHAR3>
<ZCHAR4>{8C4}</ZCHAR4>
<ZCHAR5>{8C5}</ZCHAR5>
<ZDATE1>{2}</ZDATE1>
<ZDATE2>{2}</ZDATE2>
<ZDATE3>{2}</ZDATE3>
<ZDATE4>{2}</ZDATE4>
<ZDATE5>{2}</ZDATE5> 
          </TAB_DATA_HEADER>
         </GET_DATA>
      </data01>
    </getData>
                              </soap:Body>
                            </soap:Envelope>";
                    }

                }
                if (count_rec > 0)
                {
                    soapStr = soapStr.Replace("{0}", hexvalue.ToString().Trim());
                    soapStr = soapStr.Replace("{APISOAP}", Global.APIPosturl);
                    objDSMModelData = new DSMModelData();
                    var invData = objDSMModelData.GetInvoiceDetailsByInvoiceNumber(Global.APIInvoiceNumber);
                    bool valid = Validate(invData);
                    if (valid)
                    {
                        int w_counter = 0;
                        for (int i = 0; i < invData.Count; i++)
                        {
                            //MessageBox.Show("Before Soap :" + Global.APIInvoiceNumber);
                            //MessageBox.Show(SInvoiceNo.VendorCode.ToString().Trim());
                            w_counter++;
                            if (w_counter == 1)
                            {
                                double sgsttaxamt = (Convert.ToDouble(invData[i].AssessableValue.ToString().Trim()) * 14) / 100;

                                double Totaltaxamt = sgsttaxamt * 2;

                                double NetValue = Totaltaxamt + Convert.ToDouble(invData[i].AssessableValue.ToString().Trim());

                                double cgsttaxamt = Math.Round(sgsttaxamt, 2);

                                soapStr = soapStr.Replace("{1}", invData[i].InvNumber.ToString().Trim());
                                soapStr = soapStr.Replace("{2}", invData[i].InvDate.Value.ToString("yyyyMMdd").Trim());
                                soapStr = soapStr.Replace("{3}", invData[i].VendorCode == null ? "" : invData[i].VendorCode);
                                soapStr = soapStr.Replace("{4}", invData[i].ShopCode.ToString().Trim());
                                soapStr = soapStr.Replace("{5}", invData[i].PONumber.ToString().Trim());
                                soapStr = soapStr.Replace("{6}", invData[i].PartNumber.ToString().Replace("-", "").Trim());
                                soapStr = soapStr.Replace("{7}", invData[i].InvQuantity.ToString().Trim());
                                soapStr = soapStr.Replace("{8}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].InvValue.ToString().Trim()), 3)));
                                soapStr = soapStr.Replace("{9}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].UnitPrice.ToString().Trim()), 3)));
                                //soapStr = soapStr.Replace("{10}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(NetValue.ToString().Trim()), 3)));
                                soapStr = soapStr.Replace("{10}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].AssessableValue.ToString().Trim()), 3)));
                                //soapStr = soapStr.Replace("{11}", String.Format("{0:0.00}", cgsttaxamt.ToString().Trim()));
                                //soapStr = soapStr.Replace("{12}", String.Format("{0:0.00}", cgsttaxamt.ToString().Trim()));
                                soapStr = soapStr.Replace("{11}", String.Format("{0:0.00}", invData[i].CGST.ToString().Trim()));
                                soapStr = soapStr.Replace("{12}", String.Format("{0:0.00}", invData[i].SGST.ToString().Trim()));
                                soapStr = soapStr.Replace("{13}", "0");
                                soapStr = soapStr.Replace("{14}", "0");
                                soapStr = soapStr.Replace("{15}", "0");
                                soapStr = soapStr.Replace("{16}", "0");
                                soapStr = soapStr.Replace("{17}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].AssessableValue.ToString().Trim()), 3)));
                                soapStr = soapStr.Replace("{18}", invData[i].TarrifNumber.ToString().Trim());
                                soapStr = soapStr.Replace("{19}", invData[i].GSTN.ToString().Trim());
                                soapStr = soapStr.Replace("{20}", invData[i].VehicleNumber == null ? "" : invData[i].VehicleNumber.ToString().Trim());
                                soapStr = soapStr.Replace("{21}", "0000000000000000");
                                soapStr = soapStr.Replace("{TCS1}", invData[i].TCS.ToString().Trim());
                                soapStr = soapStr.Replace("{LOT}", invData[i].Lot_Code.ToString().Trim());
                                soapStr = soapStr.Replace("{EWAY}", invData[i].Ewaybill_no.ToString().Trim());
                                soapStr = soapStr.Replace("{EINVNO}", invData[i].E_InvNo.ToString().Trim());
                                soapStr = soapStr.Replace("{1N1}", invData[i].EXTRA_NUM_1.ToString().Trim());
                                soapStr = soapStr.Replace("{1N2}", invData[i].EXTRA_NUM_2.ToString().Trim());
                                soapStr = soapStr.Replace("{1N3}", invData[i].EXTRA_NUM_3.ToString().Trim());
                                soapStr = soapStr.Replace("{1N4}", invData[i].EXTRA_NUM_4.ToString().Trim());
                                soapStr = soapStr.Replace("{1N5}", invData[i].EXTRA_NUM_5.ToString().Trim());
                                soapStr = soapStr.Replace("{1N6}", invData[i].EXTRA_NUM_6.ToString().Trim());
                                soapStr = soapStr.Replace("{1N7}", invData[i].EXTRA_NUM_7.ToString().Trim());
                                soapStr = soapStr.Replace("{1C1}", invData[i].EXTRA_CHAR_1.ToString().Trim());
                                soapStr = soapStr.Replace("{1C2}", invData[i].EXTRA_CHAR_2.ToString().Trim());
                                soapStr = soapStr.Replace("{1C3}", invData[i].EXTRA_CHAR_3.ToString().Trim());
                                soapStr = soapStr.Replace("{1C4}", invData[i].EXTRA_CHAR_4.ToString().Trim());
                                soapStr = soapStr.Replace("{1C5}", invData[i].EXTRA_CHAR_5.ToString().Trim());
                            }

                            if (w_counter == 2)
                            {
                                double sgsttaxamt = (Convert.ToDouble(invData[i].AssessableValue.ToString().Trim()) * 14) / 100;

                                double Totaltaxamt = sgsttaxamt * 2;

                                double NetValue = Totaltaxamt + Convert.ToDouble(invData[i].AssessableValue.ToString().Trim());

                                double cgsttaxamt = Math.Round(sgsttaxamt, 2);

                                soapStr = soapStr.Replace("{22}", invData[i].InvNumber.ToString().Trim());
                                soapStr = soapStr.Replace("{23}", invData[i].InvDate.Value.ToString("yyyyMMdd").Trim());
                                soapStr = soapStr.Replace("{24}", invData[i].VendorCode == null ? "" : invData[i].VendorCode);
                                soapStr = soapStr.Replace("{25}", invData[i].ShopCode.ToString().Trim());
                                soapStr = soapStr.Replace("{26}", invData[i].PONumber.ToString().Trim());
                                soapStr = soapStr.Replace("{27}", invData[i].PartNumber.ToString().Replace("-", "").Trim());
                                soapStr = soapStr.Replace("{28}", invData[i].InvQuantity.ToString().Trim());
                                soapStr = soapStr.Replace("{29}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].InvValue.ToString().Trim()), 3)));
                                soapStr = soapStr.Replace("{30}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].UnitPrice.ToString().Trim()), 3)));
                                soapStr = soapStr.Replace("{31}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].AssessableValue.ToString().Trim()), 3)));
                                //soapStr = soapStr.Replace("{32}", String.Format("{0:0.00}", cgsttaxamt.ToString().Trim()));
                                //soapStr = soapStr.Replace("{33}", String.Format("{0:0.00}", cgsttaxamt.ToString().Trim()));
                                soapStr = soapStr.Replace("{32}", String.Format("{0:0.00}", invData[i].CGST.ToString().Trim()));
                                soapStr = soapStr.Replace("{33}", String.Format("{0:0.00}", invData[i].SGST.ToString().Trim()));
                                soapStr = soapStr.Replace("{34}", "0");
                                soapStr = soapStr.Replace("{35}", "0");
                                soapStr = soapStr.Replace("{36}", "0");
                                soapStr = soapStr.Replace("{37}", "0");
                                soapStr = soapStr.Replace("{38}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].AssessableValue.ToString().Trim()), 3)));
                                soapStr = soapStr.Replace("{39}", invData[i].TarrifNumber.ToString().Trim());
                                soapStr = soapStr.Replace("{40}", invData[i].GSTN.ToString().Trim());
                                soapStr = soapStr.Replace("{41}", invData[i].VehicleNumber == null ? "" : invData[i].VehicleNumber.ToString().Trim());
                                soapStr = soapStr.Replace("{42}", "0000000000000000");
                                soapStr = soapStr.Replace("{TCS2}", invData[i].TCS.ToString().Trim());
                                soapStr = soapStr.Replace("{LOT}", invData[i].Lot_Code.ToString().Trim());
                                soapStr = soapStr.Replace("{EWAY}", invData[i].Ewaybill_no.ToString().Trim());
                                soapStr = soapStr.Replace("{EINVNO}", invData[i].E_InvNo.ToString().Trim());
                                soapStr = soapStr.Replace("{2N1}", invData[i].EXTRA_NUM_1.ToString().Trim());
                                soapStr = soapStr.Replace("{2N2}", invData[i].EXTRA_NUM_2.ToString().Trim());
                                soapStr = soapStr.Replace("{2N3}", invData[i].EXTRA_NUM_3.ToString().Trim());
                                soapStr = soapStr.Replace("{2N4}", invData[i].EXTRA_NUM_4.ToString().Trim());
                                soapStr = soapStr.Replace("{2N5}", invData[i].EXTRA_NUM_5.ToString().Trim());
                                soapStr = soapStr.Replace("{2N6}", invData[i].EXTRA_NUM_6.ToString().Trim());
                                soapStr = soapStr.Replace("{2N7}", invData[i].EXTRA_NUM_7.ToString().Trim());
                                soapStr = soapStr.Replace("{2C1}", invData[i].EXTRA_CHAR_1.ToString().Trim());
                                soapStr = soapStr.Replace("{2C2}", invData[i].EXTRA_CHAR_2.ToString().Trim());
                                soapStr = soapStr.Replace("{2C3}", invData[i].EXTRA_CHAR_3.ToString().Trim());
                                soapStr = soapStr.Replace("{2C4}", invData[i].EXTRA_CHAR_4.ToString().Trim());
                                soapStr = soapStr.Replace("{2C5}", invData[i].EXTRA_CHAR_5.ToString().Trim());
                            }

                            if (w_counter == 3)
                            {
                                double sgsttaxamt = (Convert.ToDouble(invData[i].AssessableValue.ToString().Trim()) * 14) / 100;

                                double Totaltaxamt = sgsttaxamt * 2;

                                double NetValue = Totaltaxamt + Convert.ToDouble(invData[i].AssessableValue.ToString().Trim());

                                double cgsttaxamt = Math.Round(sgsttaxamt, 2);


                                soapStr = soapStr.Replace("{43}", invData[i].InvNumber.ToString().Trim());
                                soapStr = soapStr.Replace("{44}", invData[i].InvDate.Value.ToString("yyyyMMdd").Trim());
                                soapStr = soapStr.Replace("{45}", invData[i].VendorCode == null ? "" : invData[i].VendorCode);
                                soapStr = soapStr.Replace("{46}", invData[i].ShopCode.ToString().Trim());
                                soapStr = soapStr.Replace("{47}", invData[i].PONumber.ToString().Trim());
                                soapStr = soapStr.Replace("{48}", invData[i].PartNumber.ToString().Replace("-", "").Trim());
                                soapStr = soapStr.Replace("{49}", invData[i].InvQuantity.ToString().Trim());
                                soapStr = soapStr.Replace("{50}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].InvValue.ToString().Trim()), 3)));
                                soapStr = soapStr.Replace("{51}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].UnitPrice.ToString().Trim()), 3)));
                                soapStr = soapStr.Replace("{52}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].AssessableValue.ToString().Trim()), 3)));
                                soapStr = soapStr.Replace("{53}", String.Format("{0:0.00}", invData[i].CGST.ToString().Trim()));
                                soapStr = soapStr.Replace("{54}", String.Format("{0:0.00}", invData[i].SGST.ToString().Trim()));
                                soapStr = soapStr.Replace("{55}", "0");
                                soapStr = soapStr.Replace("{56}", "0");
                                soapStr = soapStr.Replace("{57}", "0");
                                soapStr = soapStr.Replace("{58}", "0");
                                soapStr = soapStr.Replace("{59}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].AssessableValue.ToString().Trim()), 3)));
                                soapStr = soapStr.Replace("{60}", invData[i].TarrifNumber.ToString().Trim());
                                soapStr = soapStr.Replace("{61}", invData[i].GSTN.ToString().Trim());
                                soapStr = soapStr.Replace("{62}", invData[i].VehicleNumber == null ? "" : invData[i].VehicleNumber.ToString().Trim());
                                soapStr = soapStr.Replace("{63}", "0000000000000000");
                                soapStr = soapStr.Replace("{TCS3}", invData[i].TCS.ToString().Trim());
                                soapStr = soapStr.Replace("{LOT}", invData[i].Lot_Code.ToString().Trim());
                                soapStr = soapStr.Replace("{EWAY}", invData[i].Ewaybill_no.ToString().Trim());
                                soapStr = soapStr.Replace("{EINVNO}", invData[i].E_InvNo.ToString().Trim());
                                soapStr = soapStr.Replace("{3N1}", invData[i].EXTRA_NUM_1.ToString().Trim());
                                soapStr = soapStr.Replace("{3N2}", invData[i].EXTRA_NUM_2.ToString().Trim());
                                soapStr = soapStr.Replace("{3N3}", invData[i].EXTRA_NUM_3.ToString().Trim());
                                soapStr = soapStr.Replace("{3N4}", invData[i].EXTRA_NUM_4.ToString().Trim());
                                soapStr = soapStr.Replace("{3N5}", invData[i].EXTRA_NUM_5.ToString().Trim());
                                soapStr = soapStr.Replace("{3N6}", invData[i].EXTRA_NUM_6.ToString().Trim());
                                soapStr = soapStr.Replace("{3N7}", invData[i].EXTRA_NUM_7.ToString().Trim());
                                soapStr = soapStr.Replace("{3C1}", invData[i].EXTRA_CHAR_1.ToString().Trim());
                                soapStr = soapStr.Replace("{3C2}", invData[i].EXTRA_CHAR_2.ToString().Trim());
                                soapStr = soapStr.Replace("{3C3}", invData[i].EXTRA_CHAR_3.ToString().Trim());
                                soapStr = soapStr.Replace("{3C4}", invData[i].EXTRA_CHAR_4.ToString().Trim());
                                soapStr = soapStr.Replace("{3C5}", invData[i].EXTRA_CHAR_5.ToString().Trim());
                            }
                            if (w_counter == 4)
                            {
                                double sgsttaxamt = (Convert.ToDouble(invData[i].AssessableValue.ToString().Trim()) * 14) / 100;

                                double Totaltaxamt = sgsttaxamt * 2;

                                double NetValue = Totaltaxamt + Convert.ToDouble(invData[i].AssessableValue.ToString().Trim());

                                double cgsttaxamt = Math.Round(sgsttaxamt, 2);


                                soapStr = soapStr.Replace("{64}", invData[i].InvNumber.ToString().Trim());
                                soapStr = soapStr.Replace("{65}", invData[i].InvDate.Value.ToString("yyyyMMdd").Trim());
                                soapStr = soapStr.Replace("{66}", invData[i].VendorCode == null ? "" : invData[i].VendorCode);
                                soapStr = soapStr.Replace("{67}", invData[i].ShopCode.ToString().Trim());
                                soapStr = soapStr.Replace("{68}", invData[i].PONumber.ToString().Trim());
                                soapStr = soapStr.Replace("{69}", invData[i].PartNumber.ToString().Replace("-", "").Trim());
                                soapStr = soapStr.Replace("{70}", invData[i].InvQuantity.ToString().Trim());
                                soapStr = soapStr.Replace("{71}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].InvValue.ToString().Trim()), 3)));
                                soapStr = soapStr.Replace("{72}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].UnitPrice.ToString().Trim()), 3)));
                                soapStr = soapStr.Replace("{73}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].AssessableValue.ToString().Trim()), 3)));
                                soapStr = soapStr.Replace("{74}", String.Format("{0:0.00}", invData[i].CGST.ToString().Trim()));
                                soapStr = soapStr.Replace("{75}", String.Format("{0:0.00}", invData[i].SGST.ToString().Trim()));
                                soapStr = soapStr.Replace("{76}", "0");
                                soapStr = soapStr.Replace("{77}", "0");
                                soapStr = soapStr.Replace("{78}", "0");
                                soapStr = soapStr.Replace("{79}", "0");
                                soapStr = soapStr.Replace("{80}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].AssessableValue.ToString().Trim()), 3)));
                                soapStr = soapStr.Replace("{81}", invData[i].TarrifNumber.ToString().Trim());
                                soapStr = soapStr.Replace("{82}", invData[i].GSTN.ToString().Trim());
                                soapStr = soapStr.Replace("{83}", invData[i].VehicleNumber == null ? "" : invData[i].VehicleNumber.ToString().Trim());
                                soapStr = soapStr.Replace("{84}", "0000000000000000");
                                soapStr = soapStr.Replace("{TCS4}", invData[i].TCS.ToString().Trim());
                                soapStr = soapStr.Replace("{LOT}", invData[i].Lot_Code.ToString().Trim());
                                soapStr = soapStr.Replace("{EWAY}", invData[i].Ewaybill_no.ToString().Trim());
                                soapStr = soapStr.Replace("{EINVNO}", invData[i].E_InvNo.ToString().Trim());
                                soapStr = soapStr.Replace("{4N1}", invData[i].EXTRA_NUM_1.ToString().Trim());
                                soapStr = soapStr.Replace("{4N2}", invData[i].EXTRA_NUM_2.ToString().Trim());
                                soapStr = soapStr.Replace("{4N3}", invData[i].EXTRA_NUM_3.ToString().Trim());
                                soapStr = soapStr.Replace("{4N4}", invData[i].EXTRA_NUM_4.ToString().Trim());
                                soapStr = soapStr.Replace("{4N5}", invData[i].EXTRA_NUM_5.ToString().Trim());
                                soapStr = soapStr.Replace("{4N6}", invData[i].EXTRA_NUM_6.ToString().Trim());
                                soapStr = soapStr.Replace("{4N7}", invData[i].EXTRA_NUM_7.ToString().Trim());
                                soapStr = soapStr.Replace("{4C1}", invData[i].EXTRA_CHAR_1.ToString().Trim());
                                soapStr = soapStr.Replace("{4C2}", invData[i].EXTRA_CHAR_2.ToString().Trim());
                                soapStr = soapStr.Replace("{4C3}", invData[i].EXTRA_CHAR_3.ToString().Trim());
                                soapStr = soapStr.Replace("{4C4}", invData[i].EXTRA_CHAR_4.ToString().Trim());
                                soapStr = soapStr.Replace("{4C5}", invData[i].EXTRA_CHAR_5.ToString().Trim());
                            }
                            if (w_counter == 5)
                            {
                                double sgsttaxamt = (Convert.ToDouble(invData[i].AssessableValue.ToString().Trim()) * 14) / 100;

                                double Totaltaxamt = sgsttaxamt * 2;

                                double NetValue = Totaltaxamt + Convert.ToDouble(invData[i].AssessableValue.ToString().Trim());

                                double cgsttaxamt = Math.Round(sgsttaxamt, 2);

                                soapStr = soapStr.Replace("{85}", invData[i].InvNumber.ToString().Trim());
                                soapStr = soapStr.Replace("{86}", invData[i].InvDate.Value.ToString("yyyyMMdd").Trim());
                                soapStr = soapStr.Replace("{87}", invData[i].VendorCode == null ? "" : invData[i].VendorCode);
                                soapStr = soapStr.Replace("{88}", invData[i].ShopCode.ToString().Trim());
                                soapStr = soapStr.Replace("{89}", invData[i].PONumber.ToString().Trim());
                                soapStr = soapStr.Replace("{90}", invData[i].PartNumber.ToString().Replace("-", "").Trim());
                                soapStr = soapStr.Replace("{91}", invData[i].InvQuantity.ToString().Trim());
                                soapStr = soapStr.Replace("{92}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].InvValue.ToString().Trim()), 3)));
                                soapStr = soapStr.Replace("{93}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].UnitPrice.ToString().Trim()), 3)));
                                soapStr = soapStr.Replace("{94}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].AssessableValue.ToString().Trim()), 3)));
                                soapStr = soapStr.Replace("{95}", String.Format("{0:0.00}", invData[i].CGST.ToString().Trim()));
                                soapStr = soapStr.Replace("{96}", String.Format("{0:0.00}", invData[i].SGST.ToString().Trim()));
                                soapStr = soapStr.Replace("{97}", "0");
                                soapStr = soapStr.Replace("{98}", "0");
                                soapStr = soapStr.Replace("{99}", "0");
                                soapStr = soapStr.Replace("{100}", "0");
                                soapStr = soapStr.Replace("{101}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].AssessableValue.ToString().Trim()), 3)));
                                soapStr = soapStr.Replace("{102}", invData[i].TarrifNumber.ToString().Trim());
                                soapStr = soapStr.Replace("{103}", invData[i].GSTN.ToString().Trim());
                                soapStr = soapStr.Replace("{104}", invData[i].VehicleNumber == null ? "" : invData[i].VehicleNumber.ToString().Trim());
                                soapStr = soapStr.Replace("{105}", "0000000000000000");
                                soapStr = soapStr.Replace("{TCS5}", invData[i].TCS.ToString().Trim());
                                soapStr = soapStr.Replace("{LOT}", invData[i].Lot_Code.ToString().Trim());
                                soapStr = soapStr.Replace("{EWAY}", invData[i].Ewaybill_no.ToString().Trim());
                                soapStr = soapStr.Replace("{EINVNO}", invData[i].E_InvNo.ToString().Trim());
                                soapStr = soapStr.Replace("{5N1}", invData[i].EXTRA_NUM_1.ToString().Trim());
                                soapStr = soapStr.Replace("{5N2}", invData[i].EXTRA_NUM_2.ToString().Trim());
                                soapStr = soapStr.Replace("{5N3}", invData[i].EXTRA_NUM_3.ToString().Trim());
                                soapStr = soapStr.Replace("{5N4}", invData[i].EXTRA_NUM_4.ToString().Trim());
                                soapStr = soapStr.Replace("{5N5}", invData[i].EXTRA_NUM_5.ToString().Trim());
                                soapStr = soapStr.Replace("{5N6}", invData[i].EXTRA_NUM_6.ToString().Trim());
                                soapStr = soapStr.Replace("{5N7}", invData[i].EXTRA_NUM_7.ToString().Trim());
                                soapStr = soapStr.Replace("{5C1}", invData[i].EXTRA_CHAR_1.ToString().Trim());
                                soapStr = soapStr.Replace("{5C2}", invData[i].EXTRA_CHAR_2.ToString().Trim());
                                soapStr = soapStr.Replace("{5C3}", invData[i].EXTRA_CHAR_3.ToString().Trim());
                                soapStr = soapStr.Replace("{5C4}", invData[i].EXTRA_CHAR_4.ToString().Trim());
                                soapStr = soapStr.Replace("{5C5}", invData[i].EXTRA_CHAR_5.ToString().Trim());
                            }
                            if (w_counter == 6)
                            {
                                double sgsttaxamt = (Convert.ToDouble(invData[i].AssessableValue.ToString().Trim()) * 14) / 100;

                                double Totaltaxamt = sgsttaxamt * 2;

                                double NetValue = Totaltaxamt + Convert.ToDouble(invData[i].AssessableValue.ToString().Trim());

                                double cgsttaxamt = Math.Round(sgsttaxamt, 2);

                                soapStr = soapStr.Replace("{106}", invData[i].InvNumber.ToString().Trim());
                                soapStr = soapStr.Replace("{107}", invData[i].InvDate.Value.ToString("yyyyMMdd").Trim());
                                soapStr = soapStr.Replace("{108}", invData[i].VendorCode == null ? "" : invData[i].VendorCode);
                                soapStr = soapStr.Replace("{109}", invData[i].ShopCode.ToString().Trim());
                                soapStr = soapStr.Replace("{110}", invData[i].PONumber.ToString().Trim());
                                soapStr = soapStr.Replace("{111}", invData[i].PartNumber.ToString().Replace("-", "").Trim());
                                soapStr = soapStr.Replace("{112}", invData[i].InvQuantity.ToString().Trim());
                                soapStr = soapStr.Replace("{113}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].InvValue.ToString().Trim()), 3)));
                                soapStr = soapStr.Replace("{114}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].UnitPrice.ToString().Trim()), 3)));
                                soapStr = soapStr.Replace("{115}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].AssessableValue.ToString().Trim()), 3)));
                                soapStr = soapStr.Replace("{116}", String.Format("{0:0.00}", invData[i].CGST.ToString().Trim()));
                                soapStr = soapStr.Replace("{117}", String.Format("{0:0.00}", invData[i].SGST.ToString().Trim()));
                                soapStr = soapStr.Replace("{118}", "0");
                                soapStr = soapStr.Replace("{119}", "0");
                                soapStr = soapStr.Replace("{120}", "0");
                                soapStr = soapStr.Replace("{121}", "0");
                                soapStr = soapStr.Replace("{122}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].AssessableValue.ToString().Trim()), 3)));
                                soapStr = soapStr.Replace("{123}", invData[i].TarrifNumber.ToString().Trim());
                                soapStr = soapStr.Replace("{124}", invData[i].GSTN.ToString().Trim());
                                soapStr = soapStr.Replace("{125}", invData[i].VehicleNumber == null ? "" : invData[i].VehicleNumber.ToString().Trim());
                                soapStr = soapStr.Replace("{126}", "0000000000000000");
                                soapStr = soapStr.Replace("{TCS6}", invData[i].TCS.ToString().Trim());
                                soapStr = soapStr.Replace("{LOT}", invData[i].Lot_Code.ToString().Trim());
                                soapStr = soapStr.Replace("{EWAY}", invData[i].Ewaybill_no.ToString().Trim());
                                soapStr = soapStr.Replace("{EINVNO}", invData[i].E_InvNo.ToString().Trim());
                                soapStr = soapStr.Replace("{6N1}", invData[i].EXTRA_NUM_1.ToString().Trim());
                                soapStr = soapStr.Replace("{6N2}", invData[i].EXTRA_NUM_2.ToString().Trim());
                                soapStr = soapStr.Replace("{6N3}", invData[i].EXTRA_NUM_3.ToString().Trim());
                                soapStr = soapStr.Replace("{6N4}", invData[i].EXTRA_NUM_4.ToString().Trim());
                                soapStr = soapStr.Replace("{6N5}", invData[i].EXTRA_NUM_5.ToString().Trim());
                                soapStr = soapStr.Replace("{6N6}", invData[i].EXTRA_NUM_6.ToString().Trim());
                                soapStr = soapStr.Replace("{6N7}", invData[i].EXTRA_NUM_7.ToString().Trim());
                                soapStr = soapStr.Replace("{6C1}", invData[i].EXTRA_CHAR_1.ToString().Trim());
                                soapStr = soapStr.Replace("{6C2}", invData[i].EXTRA_CHAR_2.ToString().Trim());
                                soapStr = soapStr.Replace("{6C3}", invData[i].EXTRA_CHAR_3.ToString().Trim());
                                soapStr = soapStr.Replace("{6C4}", invData[i].EXTRA_CHAR_4.ToString().Trim());
                                soapStr = soapStr.Replace("{6C5}", invData[i].EXTRA_CHAR_5.ToString().Trim());
                            }
                            if (w_counter == 7)
                            {
                                double sgsttaxamt = (Convert.ToDouble(invData[i].AssessableValue.ToString().Trim()) * 14) / 100;

                                double Totaltaxamt = sgsttaxamt * 2;

                                double NetValue = Totaltaxamt + Convert.ToDouble(invData[i].AssessableValue.ToString().Trim());

                                double cgsttaxamt = Math.Round(sgsttaxamt, 2);

                                soapStr = soapStr.Replace("{127}", invData[i].InvNumber.ToString().Trim());
                                soapStr = soapStr.Replace("{128}", invData[i].InvDate.Value.ToString("yyyyMMdd").Trim());
                                soapStr = soapStr.Replace("{129}", invData[i].VendorCode == null ? "" : invData[i].VendorCode);
                                soapStr = soapStr.Replace("{130}", invData[i].ShopCode.ToString().Trim());
                                soapStr = soapStr.Replace("{131}", invData[i].PONumber.ToString().Trim());
                                soapStr = soapStr.Replace("{132}", invData[i].PartNumber.ToString().Replace("-", "").Trim());
                                soapStr = soapStr.Replace("{133}", invData[i].InvQuantity.ToString().Trim());
                                soapStr = soapStr.Replace("{134}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].InvValue.ToString().Trim()), 3)));
                                soapStr = soapStr.Replace("{135}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].UnitPrice.ToString().Trim()), 3)));
                                soapStr = soapStr.Replace("{136}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].AssessableValue.ToString().Trim()), 3)));
                                soapStr = soapStr.Replace("{137}", String.Format("{0:0.00}", invData[i].CGST.ToString().Trim()));
                                soapStr = soapStr.Replace("{138}", String.Format("{0:0.00}", invData[i].SGST.ToString().Trim()));
                                soapStr = soapStr.Replace("{139}", "0");
                                soapStr = soapStr.Replace("{140}", "0");
                                soapStr = soapStr.Replace("{141}", "0");
                                soapStr = soapStr.Replace("{142}", "0");
                                soapStr = soapStr.Replace("{143}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].AssessableValue.ToString().Trim()), 3)));
                                soapStr = soapStr.Replace("{144}", invData[i].TarrifNumber.ToString().Trim());
                                soapStr = soapStr.Replace("{145}", invData[i].GSTN.ToString().Trim());
                                soapStr = soapStr.Replace("{146}", invData[i].VehicleNumber == null ? "" : invData[i].VehicleNumber.ToString().Trim());
                                soapStr = soapStr.Replace("{147}", "0000000000000000");
                                soapStr = soapStr.Replace("{TCS7}", invData[i].TCS.ToString().Trim());
                                soapStr = soapStr.Replace("{LOT}", invData[i].Lot_Code.ToString().Trim());
                                soapStr = soapStr.Replace("{EWAY}", invData[i].Ewaybill_no.ToString().Trim());
                                soapStr = soapStr.Replace("{EINVNO}", invData[i].E_InvNo.ToString().Trim());
                                soapStr = soapStr.Replace("{7N1}", invData[i].EXTRA_NUM_1.ToString().Trim());
                                soapStr = soapStr.Replace("{7N2}", invData[i].EXTRA_NUM_2.ToString().Trim());
                                soapStr = soapStr.Replace("{7N3}", invData[i].EXTRA_NUM_3.ToString().Trim());
                                soapStr = soapStr.Replace("{7N4}", invData[i].EXTRA_NUM_4.ToString().Trim());
                                soapStr = soapStr.Replace("{7N5}", invData[i].EXTRA_NUM_5.ToString().Trim());
                                soapStr = soapStr.Replace("{7N6}", invData[i].EXTRA_NUM_6.ToString().Trim());
                                soapStr = soapStr.Replace("{7N7}", invData[i].EXTRA_NUM_7.ToString().Trim());
                                soapStr = soapStr.Replace("{7C1}", invData[i].EXTRA_CHAR_1.ToString().Trim());
                                soapStr = soapStr.Replace("{7C2}", invData[i].EXTRA_CHAR_2.ToString().Trim());
                                soapStr = soapStr.Replace("{7C3}", invData[i].EXTRA_CHAR_3.ToString().Trim());
                                soapStr = soapStr.Replace("{7C4}", invData[i].EXTRA_CHAR_4.ToString().Trim());
                                soapStr = soapStr.Replace("{7C5}", invData[i].EXTRA_CHAR_5.ToString().Trim());
                            }
                            if (w_counter == 8)
                            {
                                double sgsttaxamt = (Convert.ToDouble(invData[i].AssessableValue.ToString().Trim()) * 14) / 100;

                                double Totaltaxamt = sgsttaxamt * 2;

                                double NetValue = Totaltaxamt + Convert.ToDouble(invData[i].AssessableValue.ToString().Trim());

                                double cgsttaxamt = Math.Round(sgsttaxamt, 2);

                                soapStr = soapStr.Replace("{148}", invData[i].InvNumber.ToString().Trim());
                                soapStr = soapStr.Replace("{149}", invData[i].InvDate.Value.ToString("yyyyMMdd").Trim());
                                soapStr = soapStr.Replace("{150}", invData[i].VendorCode == null ? "" : invData[i].VendorCode);
                                soapStr = soapStr.Replace("{151}", invData[i].ShopCode.ToString().Trim());
                                soapStr = soapStr.Replace("{152}", invData[i].PONumber.ToString().Trim());
                                soapStr = soapStr.Replace("{153}", invData[i].PartNumber.ToString().Replace("-", "").Trim());
                                soapStr = soapStr.Replace("{154}", invData[i].InvQuantity.ToString().Trim());
                                soapStr = soapStr.Replace("{155}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].InvValue.ToString().Trim()), 3)));
                                soapStr = soapStr.Replace("{156}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].UnitPrice.ToString().Trim()), 3)));
                                soapStr = soapStr.Replace("{157}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].AssessableValue.ToString().Trim()), 3)));
                                soapStr = soapStr.Replace("{158}", String.Format("{0:0.00}", invData[i].CGST.ToString().Trim()));
                                soapStr = soapStr.Replace("{159}", String.Format("{0:0.00}", invData[i].SGST.ToString().Trim()));
                                soapStr = soapStr.Replace("{160}", "0");
                                soapStr = soapStr.Replace("{161}", "0");
                                soapStr = soapStr.Replace("{162}", "0");
                                soapStr = soapStr.Replace("{163}", "0");
                                soapStr = soapStr.Replace("{164}", String.Format("{0:0.000}", Math.Round(Convert.ToDouble(invData[i].AssessableValue.ToString().Trim()), 3)));
                                soapStr = soapStr.Replace("{165}", invData[i].TarrifNumber.ToString().Trim());
                                soapStr = soapStr.Replace("{166}", invData[i].GSTN.ToString().Trim());
                                soapStr = soapStr.Replace("{167}", invData[i].VehicleNumber == null ? "" : invData[i].VehicleNumber.ToString().Trim());
                                soapStr = soapStr.Replace("{168}", "0000000000000000");
                                soapStr = soapStr.Replace("{TCS8}", invData[i].TCS.ToString().Trim());
                                soapStr = soapStr.Replace("{LOT}", invData[i].Lot_Code.ToString().Trim());
                                soapStr = soapStr.Replace("{EWAY}", invData[i].Ewaybill_no.ToString().Trim());
                                soapStr = soapStr.Replace("{EINVNO}", invData[i].E_InvNo.ToString().Trim());
                                soapStr = soapStr.Replace("{8N1}", invData[i].EXTRA_NUM_1.ToString().Trim());
                                soapStr = soapStr.Replace("{8N2}", invData[i].EXTRA_NUM_2.ToString().Trim());
                                soapStr = soapStr.Replace("{8N3}", invData[i].EXTRA_NUM_3.ToString().Trim());
                                soapStr = soapStr.Replace("{8N4}", invData[i].EXTRA_NUM_4.ToString().Trim());
                                soapStr = soapStr.Replace("{8N5}", invData[i].EXTRA_NUM_5.ToString().Trim());
                                soapStr = soapStr.Replace("{8N6}", invData[i].EXTRA_NUM_6.ToString().Trim());
                                soapStr = soapStr.Replace("{8N7}", invData[i].EXTRA_NUM_7.ToString().Trim());
                                soapStr = soapStr.Replace("{8C1}", invData[i].EXTRA_CHAR_1.ToString().Trim());
                                soapStr = soapStr.Replace("{8C2}", invData[i].EXTRA_CHAR_2.ToString().Trim());
                                soapStr = soapStr.Replace("{8C3}", invData[i].EXTRA_CHAR_3.ToString().Trim());
                                soapStr = soapStr.Replace("{8C4}", invData[i].EXTRA_CHAR_4.ToString().Trim());
                                soapStr = soapStr.Replace("{8C5}", invData[i].EXTRA_CHAR_5.ToString().Trim());
                            }
                        }

                        soapStr = soapStr.Replace("{0}", hexvalue.ToString().Trim());
                        LogSoapFormat("Coming Soap Generation : ");
                        LogSoapFormat(soapStr);
                        //MessageBox.Show(soapStr);
                        //MessageBox.Show(Url.ToString());
                        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Url);
                        //req.Headers.Add("SOAPAction", "http://tempuri.org/IWebService/GetMessage");
                        req.ProtocolVersion = HttpVersion.Version11;
                        req.ContentType = "text/xml;charset=\"utf-8\"";
                        req.Accept = "text/xml";
                        req.KeepAlive = true;
                        req.Method = "POST";

                        //MessageBox.Show("URL : " + Url.ToString().Trim());
                        //MessageBox.Show("MethodName : " + methodName.ToString().Trim());
                        //LogSoapFormat(soapStr);

                        int hitCount = 0;
                        APIhit:
                        try
                        {
                            hitCount++;
                            using (Stream stm = req.GetRequestStream())
                            {
                                LogSoapFormat("Coming Request : ");
                                using (StreamWriter stmw = new StreamWriter(stm))
                                {
                                    stmw.Write(soapStr);
                                    stmw.Close();
                                    //MessageBox.Show("Site Reached");
                                }
                                //MessageBox.Show("After Getrequest");
                            }
                            using (StreamReader responseReader = new StreamReader(req.GetResponse().GetResponseStream()))
                            {
                                //MessageBox.Show("Coming Response");
                                string result = responseReader.ReadToEnd();
                                ResultXML = XDocument.Parse(result);
                                ResultString = result;
                                LogSoapFormat(ResultString);

                                //MessageBox.Show("Result :" + ResultString.ToString().Trim());

                                XmlDocument xmlDoc = new XmlDocument();
                                //xmlDoc.LoadXml(strReturnStatus);
                                xmlDoc.LoadXml(ResultString);

                                XmlNamespaceManager xmlnsManager = new System.Xml.XmlNamespaceManager(xmlDoc.NameTable);

                                xmlnsManager.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
                                xmlnsManager.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
                                xmlnsManager.AddNamespace("xsd", "http://www.w3.org/2001/XMLSchema");
                                //xmlnsManager.AddNamespace("si", "http://example.com/SystemIntegration");
                                xmlnsManager.AddNamespace("si", Global.APIPosturl);


                                // You'd access the full path like this
                                //XmlNode node = xmlDoc.SelectSingleNode("/soap:Envelope/soap:Body/si:LoginResponse/si:FirstName", xmlnsManager);
                                //XmlNode node = xmlDoc.SelectSingleNode("/soap:Envelope/soap:Body/si:getDataResponse/si:getDataResult/si:GET_DATA_01/si:TAB_DATA/si:IFRESULT", xmlnsManager);

                                //string firstname = node.InnerText;
                                //string Result = node.InnerText;
                                // string Result = "";

                                int successcount = 0;
                                int errorcount = 0;
                                string failedMsg = "";

                                XmlNodeList xNodelst = xmlDoc.DocumentElement.SelectNodes("/soap:Envelope/soap:Body/si:getDataResponse/si:getDataResult/si:GET_DATA_01/si:TAB_DATA", xmlnsManager);
                                foreach (XmlNode xn in xNodelst)
                                {
                                    //string fbc = xn["ns1:FBC"].Attributes["Destination"].Value;
                                    string xmlResult = xn["IFRESULT"].InnerText;
                                    //MessageBox.Show(xmlResult);
                                    failedMsg = xn["IFFFAILMSG"].InnerText;
                                    string InvoiceNo = xn["IVNUM"].InnerText;
                                    LogSoapFormat(xmlResult.ToString().Trim());
                                    if (xmlResult.ToString().Trim() != "E")
                                    {
                                        successcount++;
                                        //MessageBox.Show("XMLRESULT");
                                        objDSMModelData.UpdateInvoiceAPIstatusByInvoiceNoFinYear(InvoiceNo.ToString().Trim(), xmlResult.ToString().Trim(), failedMsg.ToString().Trim(), Finyear);
                                        objDSMModelData.UpdateInvoiceAPIstatusByInvoiceNo(InvoiceNo.ToString().Trim(), xmlResult.ToString().Trim(), failedMsg.ToString().Trim());
                                       // Update(InvoiceNo.ToString().Trim(), xmlResult.ToString().Trim(), failedMsg.ToString().Trim());
                                    }
                                    else
                                    {
                                        objDSMModelData.UpdateInvoiceAPIstatusByInvoiceNoFinYear(InvoiceNo.ToString().Trim(), xmlResult.ToString().Trim(), failedMsg.ToString().Trim(), Finyear);
                                        objDSMModelData.UpdateInvoiceAPIstatusByInvoiceNo(InvoiceNo.ToString().Trim(), xmlResult.ToString().Trim(), failedMsg.ToString().Trim());
                                        //Update(InvoiceNo.ToString().Trim(), xmlResult.ToString().Trim(), failedMsg.ToString().Trim());

                                        if (failedMsg.ToString().Trim() == "I/F Success, ASN Not cerated,Create Manual ASN")
                                        {
                                            successcount++;
                                            //MessageBox.Show("Invoice Successfully Posted");
                                        }
                                        else if (failedMsg.ToString().Trim() == "I/F Success, ASN Not created,Create Manual ASN")
                                        {
                                            successcount++;
                                            // MessageBox.Show("Invoice Successfully Posted");
                                        }
                                        else
                                        {
                                            errorcount++;
                                            //MessageBox.Show("Invoice Not Successfully Posted." + failedMsg.ToString().Trim());
                                        }

                                        break;
                                    }
                                }

                                //MoveFile(Global.InvoicePath + Global.APIInvoiceNumber + ".pdf","PROCESSED");
                                if (Global.APIpostType == "MANUAL" || Global.UserType == "admin")
                                {
                                    if (errorcount > 0)
                                        MessageBox.Show("Invoice - " + Global.APIInvoiceNumber + " Not Successfully Posted." + failedMsg.ToString().Trim());
                                    else if (successcount > 0)
                                        MessageBox.Show("Invoice - " + Global.APIInvoiceNumber + " Successfully Posted");
                                }
                                else
                                {
                                    if (errorcount > 0)
                                        Global.MailAlert("Invoice - " + Global.APIInvoiceNumber + " Not Successfully Posted. " + failedMsg.ToString().Trim());
                                }
                            }
                            //End Correct Code
                        }
                        catch (Exception exs)
                        {
                            //LogSoapFormat("Got exception at :" + exs.ToString());
                            LogAPIErrors(exs);
                            if (hitCount <= 3)
                            {
                                goto APIhit;
                            }
                            else
                            {
                                objDSMModelData.UpdateInvoiceAPIstatusByInvoiceNoFinYear(Global.APIInvoiceNumber, "MANUAL", exs.Message,Finyear);
                                objDSMModelData.UpdateInvoiceAPIstatusByInvoiceNo(Global.APIInvoiceNumber, "MANUAL", exs.Message);
                                //Update(Global.APIInvoiceNumber, "MANUAL", exs.Message);
                                if (Global.APIpostType == "MANUAL" || Global.UserType == "admin")
                                {
                                    MessageBox.Show("API site cannot be reached");
                                    Global.APIInvoiceNumber = "Null";
                                }
                            }

                        }
                        //End Correct Code
                    }
                    else
                    {
                        LogSoapFormat("API skipped due to wrong data: " + Global.APIInvoiceNumber);
                    }
                }
            }
            catch (Exception ex)
            {
                LogAPIErrors(ex);
                objDSMModelData.UpdateInvoiceAPIstatusByInvoiceNo(Global.APIInvoiceNumber, "MANUAL", "Manual Posting");
                //Update(Global.APIInvoiceNumber, "MANUAL", "Manual Posting");
                if (Global.APIpostType == "MANUAL")
                {
                    MessageBox.Show("API EXCEPTION: " + ex.ToString());
                }
            }
        }

        public void Update(string innum, string sts, string msg)
        {

            string querys = "Update Invoice set APIStatus='" + sts + "' , APIMessage= '" + msg + "',UpdatedDate=CONVERT(varchar,GETDATE(),21) where InvNumber = '" + innum + "' ";
            LogSoapFormat(querys);
            bool upd = Utilities.executeNonQuery(querys);
        }

        public bool Validate(List<Invoice> invData)
        {
            bool checkChar = true;
            string specialChar = @"\/|!#$%&/()=?`+»«@£§€{}-;'<>_,*^";
            foreach (var inv in invData)
            {
                foreach (var item in specialChar)
                {
                    //if (inv.VehicleNumber == null)
                    //{
                    //    checkChar = false;
                    //    break;
                    //}
                    //else
                    //{
                    //    if (inv.VehicleNumber.Trim() == "")
                    //    {
                    //        checkChar = false;
                    //        break;
                    //    }
                    //}
                    if (inv.InvNumber != null)
                    {
                        if (inv.InvNumber.Contains(item))
                        {
                            checkChar = false;
                            break;
                        }
                    }
                    if (inv.PONumber != null)
                    {
                        if (inv.PONumber.Contains(item))
                        {
                            checkChar = false;
                            break;
                        }
                    }
                    if (inv.CGST != null)
                    {
                        if (inv.CGST.Contains(item))
                        {
                            checkChar = false;
                            break;
                        }
                    }
                    if (inv.IGST != null)
                    {
                        if (inv.IGST.Contains(item))
                        {
                            checkChar = false;
                            break;
                        }
                    }
                    if (inv.InvQuantity != null)
                    {
                        if (inv.InvQuantity.Contains(item))
                        {
                            checkChar = false;
                            break;
                        }
                    }
                    if (inv.InvValue != null)
                    {
                        if (inv.InvValue.Contains(item))
                        {
                            checkChar = false;
                            break;
                        }
                    }
                    if (inv.SGST != null)
                    {
                        if (inv.SGST.Contains(item))
                        {
                            checkChar = false;
                            break;
                        }
                    }
                    if (inv.GSTN != null)
                    {
                        if (inv.GSTN.Contains(item))
                        {
                            checkChar = false;
                            break;
                        }
                    }
                    if (inv.UnitPrice != null)
                    {
                        if (inv.UnitPrice.Contains(item))
                        {
                            checkChar = false;
                            break;
                        }
                    }
                    if (inv.AssessableValue != null)
                    {
                        if (inv.AssessableValue.Contains(item))
                        {
                            checkChar = false;
                            break;
                        }
                    }
                    if (inv.TarrifNumber != null)
                    {
                        if (inv.TarrifNumber.Contains(item))
                        {
                            checkChar = false;
                            break;
                        }
                    }
                    if (inv.BedAmount != null)
                    {
                        if (inv.BedAmount.Contains(item))
                        {
                            checkChar = false;
                            break;
                        }
                    }
                    if (inv.VatAmount != null)
                    {
                        if (inv.VatAmount.Contains(item))
                        {
                            checkChar = false;
                            break;
                        }
                    }
                    if (inv.MaterialCost != null)
                    {
                        if (inv.MaterialCost.Contains(item))
                        {
                            checkChar = false;
                            break;
                        }
                    }
                    if (inv.ConsigneeMatlCost != null)
                    {
                        if (inv.ConsigneeMatlCost.Contains(item))
                        {
                            checkChar = false;
                            break;
                        }
                    }
                    if (inv.ConsigneePartCost != null)
                    {
                        if (inv.ConsigneePartCost.Contains(item))
                        {
                            checkChar = false;
                            break;
                        }
                    }
                    if (inv.ExciseDutyCost != null)
                    {
                        if (inv.ExciseDutyCost.Contains(item))
                        {
                            checkChar = false;
                            break;
                        }
                    }
                    if (inv.CSTAmount != null)
                    {
                        if (inv.CSTAmount.Contains(item))
                        {
                            checkChar = false;
                            break;
                        }
                    }
                    if (inv.ToolCost != null)
                    {
                        if (inv.ToolCost.Contains(item))
                        {
                            checkChar = false;
                            break;
                        }
                    }
                }
            }
            return checkChar;
        }

        public void LogSoapFormat(string soapStr)
        {
            StringBuilder sb = new StringBuilder();
            string path = AppDomain.CurrentDomain.BaseDirectory + "LogSoapXML.txt";
            // flush every 20 seconds as you do it

            sb.AppendLine();
            sb.Append("Log Time: " + DateTime.Now);
            sb.AppendLine();
            sb.Append("--------------------------------------------------");
            sb.AppendLine();
            if (soapStr != null)
            {
                sb.Append(soapStr);
            }
            sb.AppendLine();
            sb.Append("--------------------------------------------------");

            File.AppendAllText(path, sb.ToString());
            sb.Clear();
        }

        public void LogAPIErrors(Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            string path = AppDomain.CurrentDomain.BaseDirectory + "LogError.txt";

            sb.AppendLine();
            sb.Append("Log Time: " + DateTime.Now);
            sb.AppendLine();
            sb.Append("--------------------------------------------------");
            sb.AppendLine();
            if (ex != null)
            {
                sb.Append("Exception: " + ex.ToString());
                if (ex.InnerException != null)
                {
                    sb.Append("Inner Exception: " + ex.InnerException.ToString());
                }
            }
            sb.AppendLine();
            sb.Append("--------------------------------------------------");

            File.AppendAllText(path, sb.ToString());
            sb.Clear();
        }

        public void MoveFile(string source, string location)
        {

            System.IO.StreamReader fileMove = new System.IO.StreamReader(source);
            string _SourceFile = source;

            //check folder
            if (!System.IO.Directory.Exists(Global.InvoicePath + "\\" + location + "\\"))
            {
                System.IO.Directory.CreateDirectory(Global.InvoicePath + "\\" + location + "\\");
            }
            string _DestFile = Global.InvoicePath + "\\" + location + "\\" + System.IO.Path.GetFileName(source);
            fileMove.Close();
            fileMove.Dispose();

            //Check File
            if (!System.IO.File.Exists(_DestFile))
            {
                File.Move(_SourceFile, _DestFile);
                System.IO.Directory.CreateDirectory(Global.InvoicePath + "\\" + location + "\\");
            }
            else
            {
                System.IO.File.Delete(_DestFile);
                File.Move(_SourceFile, _DestFile);
            }
        }


        /// <summary>
        /// This method should be called before each Invoke().
        /// </summary>
        internal void PreInvoke()
        {
            CleanLastInvoke();
            InitialCursorState = Cursor.Current;
            //Cursor.Current = Cursor.WaitCursor;
            Cursor.Current = Cursors.WaitCursor;
            // feel free to add more instructions to this method
        }

        /// <summary>
        /// This method should be called after each (successful or unsuccessful) Invoke().
        /// </summary>
        internal void PosInvoke()
        {
            Cursor.Current = InitialCursorState;
            // feel free to add more instructions to this method
        }

        #endregion
    }
}
