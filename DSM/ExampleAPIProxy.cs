using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSM
{
    //class ExampleAPIProxy
    internal class ExampleAPIProxy
    {
        //private static WebService ExampleAPI = new WebService("http://.../example.asmx");    // DEFAULT location of the WebService, containing the WebMethods
        //private static WebService ExampleAPI = new WebService("https://hmieai.hmil.net:94/KML.asmx","getData");    // DEFAULT location of the WebService, containing the WebMethods

        ////Test API Url
        //private static WebService ExampleAPI = new WebService("https://hmieai.hmil.net:94/KML.asmx?WSDL", "getData");    // DEFAULT location of the WebService, containing the WebMethods
        ////End Test API Url

        //Live API Url
        //private static WebService ExampleAPI = new WebService("https://hmieai.hmil.net:6004/Service.asmx?WSDL", "getData");    // DEFAULT location of the WebService, containing the WebMethods
        private static WebService ExampleAPI = new WebService(Global.APIurl, "getData");  // DEFAULT location of the WebService, containing the WebMethods
        //End Live API Url

        public static void ChangeUrl(string webserviceEndpoint)
        {
            ExampleAPI = new WebService(webserviceEndpoint);
        }

        //public static string ExampleWebMethod(string name, int number)
        public static string getData(string name, int number)
        {
            ExampleAPI.PreInvoke();

            ExampleAPI.AddParameter("HEXADECIMAL", name);                    // Case Sensitive! To avoid typos, just copy the WebMethod's signature and paste it

            //ExampleAPI.AddParameter("IVNUM", "2211900005");     // all parameters are passed as strings
            //ExampleAPI.AddParameter("IVDAT", number.ToString());     // all parameters are passed as strings
            //ExampleAPI.AddParameter("LIFNR", number.ToString());     // all parameters are passed as strings
            //ExampleAPI.AddParameter("MATNR", number.ToString());     // all parameters are passed as strings
            //ExampleAPI.AddParameter("ZSHOP", number.ToString());     // all parameters are passed as strings
            //ExampleAPI.AddParameter("EBELN", number.ToString());     // all parameters are passed as strings
            //ExampleAPI.AddParameter("IVQTY", number.ToString());     // all parameters are passed as strings
            //ExampleAPI.AddParameter("ZAIVAMT", number.ToString());     // all parameters are passed as strings
            //ExampleAPI.AddParameter("ZANETPR", number.ToString());     // all parameters are passed as strings
            //ExampleAPI.AddParameter("ZANETWR", number.ToString());     // all parameters are passed as strings
            //ExampleAPI.AddParameter("ZCGST", number.ToString());     // all parameters are passed as strings

            //ExampleAPI.AddParameter("ZSGST", number.ToString());     // all parameters are passed as strings
            //ExampleAPI.AddParameter("ZIGST", number.ToString());     // all parameters are passed as strings
            //ExampleAPI.AddParameter("ZUGST", number.ToString());     // all parameters are passed as strings
            //ExampleAPI.AddParameter("COMPCESS", number.ToString());     // all parameters are passed as strings
            //ExampleAPI.AddParameter("ZATOLC", number.ToString());     // all parameters are passed as strings
            //ExampleAPI.AddParameter("ZADTC2", number.ToString());     // all parameters are passed as strings
            //ExampleAPI.AddParameter("ZACNMC", number.ToString());     // all parameters are passed as strings
            //ExampleAPI.AddParameter("ZACNPC", number.ToString());     // all parameters are passed as strings
            //ExampleAPI.AddParameter("ZAASVL", number.ToString());     // all parameters are passed as strings
            //ExampleAPI.AddParameter("ZHSNSAC", number.ToString());     // all parameters are passed as strings
            //ExampleAPI.AddParameter("ZGSTIN", number.ToString());     // all parameters are passed as strings
            //ExampleAPI.AddParameter("VEHNO", number.ToString());     // all parameters are passed as strings

            ExampleAPI.AddParameter("IVNUM", "2211900005");     // all parameters are passed as strings
            ExampleAPI.AddParameter("IVDAT", "21032019");     // all parameters are passed as strings
            ExampleAPI.AddParameter("LIFNR", "T5ES");     // all parameters are passed as strings
            ExampleAPI.AddParameter("MATNR", " ");     // all parameters are passed as strings
            ExampleAPI.AddParameter("ZSHOP", "XX");     // all parameters are passed as strings
            ExampleAPI.AddParameter("EBELN", "4200521142");     // all parameters are passed as strings
            ExampleAPI.AddParameter("IVQTY", "2");     // all parameters are passed as strings
            ExampleAPI.AddParameter("ZAIVAMT", "654.88");     // all parameters are passed as strings
            ExampleAPI.AddParameter("ZANETPR", "327.44");     // all parameters are passed as strings
            ExampleAPI.AddParameter("ZANETWR", "746.563");     // all parameters are passed as strings
            ExampleAPI.AddParameter("ZCGST", "91.68");     // all parameters are passed as strings

            ExampleAPI.AddParameter("ZSGST", "91.68");     // all parameters are passed as strings
            ExampleAPI.AddParameter("ZIGST", "0.00");     // all parameters are passed as strings
            ExampleAPI.AddParameter("ZUGST", "0.00");     // all parameters are passed as strings
            ExampleAPI.AddParameter("COMPCESS", " ");     // all parameters are passed as strings
            ExampleAPI.AddParameter("ZATOLC", " ");     // all parameters are passed as strings
            ExampleAPI.AddParameter("ZADTC2", "0.00");     // all parameters are passed as strings
            ExampleAPI.AddParameter("ZACNMC", "0.00");     // all parameters are passed as strings
            ExampleAPI.AddParameter("ZACNPC", "0.00");     // all parameters are passed as strings
            ExampleAPI.AddParameter("ZAASVL", "654.88");     // all parameters are passed as strings
            ExampleAPI.AddParameter("ZHSNSAC", "8708.99.00");     // all parameters are passed as strings
            ExampleAPI.AddParameter("ZGSTIN", "33AAECM3018M1ZK");     // all parameters are passed as strings
            ExampleAPI.AddParameter("VEHNO", "TN22BK9096");     // all parameters are passed as strings


            try
            {
                //ExampleAPI.Invoke("ExampleWebMethod");                // name of the WebMethod to call (Case Sentitive again!)
                ExampleAPI.Invoke("getData");
            }
            finally { ExampleAPI.PosInvoke(); }

            return ExampleAPI.ResultString;                           // you can either return a string or an XML, your choice
        }
    }
}
