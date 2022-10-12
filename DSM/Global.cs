using DSMData;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DSM
{
    public static class Global
    {
        public static string _sConString = ConfigurationManager.ConnectionStrings["strcon"].ConnectionString;
        public static DSMModelData objDSMModelData;
        private static string _userName;
        public static string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        private static int _userId;
        public static int UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }

        private static bool _UserIsActive;
        public static bool UserIsActive
        {
            get { return _UserIsActive; }
            set { _UserIsActive = value; }
        }

        private static int _clientLineId;
        public static int ClientLineId
        {
            get { return _clientLineId; }
            set { _clientLineId = value; }
        }

        private static string _CustomerId;
        public static string CustomerId
        {
            get { return _CustomerId; }
            set { _CustomerId = value; }
        }

        private static string _systemName;
        public static string SystemName
        {
            get { return _systemName; }
            set { _systemName = value; }
        }

        private static string _userType;
        public static string UserType
        {
            get { return _userType; }
            set { _userType = value; }
        }

        private static string _invoicePath;
        public static string InvoicePath
        {
            get { return _invoicePath; }
            set { _invoicePath = value; }
        }

        private static string _ManualInvoicePath;
        public static string ManualInvoicePath
        {
            get { return _ManualInvoicePath; }
            set { _ManualInvoicePath = value; }
        }

        private static string _inputPath;
        public static string InputPath
        {
            get { return _inputPath; }
            set { _inputPath = value; }
        }

        private static string _outputPath;
        public static string OutputPath
        {
            get { return _outputPath; }
            set { _outputPath = value; }
        }

        private static string _invoiceFilePath;
        public static string InvoiceFilePath
        {
            get { return _invoiceFilePath; }
            set { _invoiceFilePath = value; }
        }

        private static string _truckNumber;
        public static string TruckNumber
        {
            get { return _truckNumber; }
            set { _truckNumber = value; }
        }

        private static string _customerName;
        public static string CustomerName
        {
            get { return _customerName; }
            set { _customerName = value; }
        }

        private static string _vendorCode;
        public static string VendorCode
        {
            get { return _vendorCode; }
            set { _vendorCode = value; }
        }

        private static string _invoiceNumber;
        public static string InvoiceNumber
        {
            get { return _invoiceNumber; }
            set { _invoiceNumber = value; }
        }
        private static string _invoiceNumberchk;
        public static string InvoiceNumberchk
        {
            get { return _invoiceNumberchk; }
            set { _invoiceNumberchk = value; }
        }
        private static string _Templatename;
        public static string Templatename
        {
            get { return _Templatename; }
            set { _Templatename = value; }
        }
        private static string _apiInvoiceNumber;
        public static string APIInvoiceNumber
        {
            get { return _apiInvoiceNumber; }
            set { _apiInvoiceNumber = value; }
        }

        private static string _apiurl;
        public static string APIurl
        {
            get { return _apiurl; }
            set { _apiurl = value; }
        }

        private static string _apiPosturl;
        public static string APIPosturl
        {
            get { return _apiPosturl; }
            set { _apiPosturl = value; }
        }

        private static string _PDFtype;
        public static string PDFtype
        {
            get { return _PDFtype; }
            set { _PDFtype = value; }
        }

        private static string _PDFCopies;
        public static string PDFCopies
        {
            get { return _PDFCopies; }
            set { _PDFCopies = value; }
        }

        private static string _PrintType;
        public static string PrintType
        {
            get { return _PrintType; }
            set { _PrintType = value; }
        }

        private static string _PrintCopies;
        public static string PrintCopies
        {
            get { return _PrintCopies; }
            set { _PrintCopies = value; }
        }

        private static bool _IsAPIpost;
        public static bool IsAPIpost
        {
            get { return _IsAPIpost; }
            set { _IsAPIpost = value; }
        }

        private static string _APIpostType;
        public static string APIpostType
        {
            get { return _APIpostType; }
            set { _APIpostType = value; }
        }
        private static string _Shopcode;
        public static string Shopcode
        {
            get { return _Shopcode; }
            set { _Shopcode = value; }
        }

        private static bool _IsDS;
        public static bool IsDSign
        {
            get { return _IsDS; }
            set { _IsDS = value; }
        }

        private static string _SelectedFile;
        public static string SelectedFile
        {
            get { return _SelectedFile; }
            set { _SelectedFile = value; }
        }

        private static string _CustomerCheck;
        public static string CustomerCheck
        {
            get { return _CustomerCheck; }
            set { _CustomerCheck = value; }
        }

        private static string _InvoiceDate;
        public static string InvoiceDate
        {
            get { return _InvoiceDate; }
            set { _InvoiceDate = value; }
        }

        private static bool _IsMailSend;
        public static bool IsMailSend
        {
            get { return _IsMailSend; }
            set { _IsMailSend = value; }
        }

        private static int _FinYear;
        public static int FinYear
        {
            get { return _FinYear; }
            set { _FinYear = value; }
        }

        private static byte[] _OriginalPDFFile;
        public static byte[] OriginalPDFFile
        {
            get { return _OriginalPDFFile; }
            set { _OriginalPDFFile = value; }
        }

        public static SqlDataReader executeQuery(string _sCommandText)
        {
            SqlDataReader rdr1 = null;
            SqlConnection _objCon = new SqlConnection(_sConString);
            SqlCommand _objCmd = new SqlCommand();
            try
            {
                if (_objCon.State == System.Data.ConnectionState.Closed) _objCon.Open();
                _objCmd.Connection = _objCon;
                _objCmd.CommandText = _sCommandText;
                rdr1 = _objCmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
            }
            catch { }
            finally { }
            return rdr1;
        }
        public static void MailAlert(string Message)
        {
            //*************************************************MAIL ALERT********************************
            objDSMModelData = new DSMModelData();
            var edi = objDSMModelData.GetEdiByClientName(Global.CustomerName);

            if (edi.FromEmail != null)
            {

                try
                {

                    System.Net.Mail.MailMessage DSmessage = new System.Net.Mail.MailMessage();
                    System.Net.Mail.SmtpClient DSsmtp = new SmtpClient();

                    DSmessage.From = new MailAddress(edi.FromEmail);

                    string[] arrToMail = edi.Email.Split(',');
                    for (int i = 0; i < arrToMail.Length; i++)
                    {
                        DSmessage.To.Add(arrToMail[i]);
                    }
                    DSmessage.Subject = "EnSi Mail Alert !";
                    DSmessage.Body = Message;
                    DSmessage.IsBodyHtml = true;
                    DSsmtp.Port = int.Parse(edi.Port);
                    DSsmtp.Host = edi.Host;
                    DSsmtp.EnableSsl = true;
                    DSsmtp.UseDefaultCredentials = false;
                    DSsmtp.Credentials = new NetworkCredential(edi.FromEmail, edi.FromPwd);
                    DSsmtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    DSsmtp.Send(DSmessage);
                    //attachPath.
                    DSmessage.Dispose();
                }
                catch (Exception ex)
                {
                    LogErrors(ex);
                }

            }
            else
            {

            }
        }
        public static void LogErrors(Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            string path = AppDomain.CurrentDomain.BaseDirectory + "LogError.txt";
            // flush every 20 seconds as you do it

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
            sb.AppendLine();

            File.AppendAllText(path, sb.ToString());
            sb.Clear();
        }
        private static int _APIFinYear;
        public static int APIFinYear
        {
            get { return _APIFinYear; }
            set { _APIFinYear = value; }
        }

        private static string _printerName;
        public static string PrinterName
        {
            get { return _printerName; }
            set { _printerName = value; }
        }

    }
}
