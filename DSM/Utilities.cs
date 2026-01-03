using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace DSM
{
    public static class Utilities
    {
        //public static string _sConString = ConfigurationSettings.ConnectionStrings["strcon"];
        public static string _sConString = ConfigurationManager.ConnectionStrings["strcon"].ConnectionString;
        public static string _system_name = "-";
        public static string HMI_INITIATE = "";
        //public static string _comp_GSTIN = "33AAACH2804E1ZA";
        //public static string _comp_id = "HSI";
        //public static string _comp_name = "HSI AUTOMOTIVES PVT LTD";
        public static string _sFlat_File_Path = "";


        public static string _ApiUrl = "";

        public static int DSAuthenticationCount = 0;
        public static string _IrpGenerateEwaybill;
        public static string _IrpGetToken;
        public static string _IrCancelEInv;
        public static string _IrpCancelEwayBill;
        public static string _IrpGenerateInv;
        public static string _IrpGetEInvByIrn;
        public static string _IrpGetGSTINDet;

        public static int cnt = 0;
        public static bool executeNonQuery(string _sCommandText)
        {
            bool _bResult = false;
            SqlConnection _objCon = new SqlConnection(_sConString);
            SqlCommand _objCmd = new SqlCommand();
            try
            {
                if (_objCon.State == System.Data.ConnectionState.Closed) _objCon.Open();
                _objCmd.Connection = _objCon;
                _objCmd.CommandText = _sCommandText;
                _bResult = Convert.ToBoolean(_objCmd.ExecuteNonQuery());
            }
            catch (Exception ex) { }
            finally
            {
                if (_objCon.State == System.Data.ConnectionState.Open) _objCon.Close();
            }
            return _bResult;
        }
        public static int executeUNonQuery(string _sCommandText)
        {
            int _bResult = 0;
            SqlConnection _objCon = new SqlConnection(_sConString);
            SqlCommand _objCmd = new SqlCommand();
            try
            {
                if (_objCon.State == System.Data.ConnectionState.Closed) _objCon.Open();
                _objCmd.Connection = _objCon;
                _objCmd.CommandText = _sCommandText;
                _bResult = Convert.ToInt32(_objCmd.ExecuteNonQuery());
            }
            catch { }
            finally
            {
                if (_objCon.State == System.Data.ConnectionState.Open) _objCon.Close();
            }
            return _bResult;
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
            catch (Exception) { }
            finally { }
            return rdr1;
        }

        //--------------------------------------------------------------------CURRENCY CONVERTER-------------------------
        public static string AmountInWords(decimal Num)
        {
            string returnValue;
            //I have created this function for converting amount in indian rupees (INR).
            //You can manipulate as you wish like decimal setting, Doller (any currency) Prefix.


            string strNum;
            string strNumDec;
            string StrWord;
            strNum = Num.ToString();


            if (strNum.IndexOf(".") + 1 != 0)
            {
                strNumDec = strNum.Substring(strNum.IndexOf(".") + 2 - 1);


                if (strNumDec.Length == 1)
                {
                    strNumDec = strNumDec + "0";
                }
                if (strNumDec.Length > 2)
                {
                    strNumDec = strNumDec.Substring(0, 2);
                }


                strNum = strNum.Substring(0, strNum.IndexOf(".") + 0);
                StrWord = (NumToWord((decimal)(double.Parse(strNum)))) + ((double.Parse(strNumDec) > 0) ? (" Rupees and " + cWord3((decimal)(double.Parse(strNumDec))) + " Paise") : "");//(double.Parse(strNum) == 1) ? " Rupee " : " Rupees ") +
            }
            else
            {
                StrWord = NumToWord((decimal)(double.Parse(strNum))) + " Rupees";//((double.Parse(strNum) == 1) ? " Rupee " : " Rupees ") +
            }
            returnValue = StrWord + " Only";
            return returnValue;
        }
        static public string NumToWord(decimal Num)
        {
            string returnValue;


            //I divided this function in two part.
            //1. Three or less digit number.
            //2. more than three digit number.
            string strNum;
            string StrWord;
            strNum = Num.ToString();


            if (strNum.Length <= 3)
            {
                StrWord = cWord3((decimal)(double.Parse(strNum)));
            }
            else
            {
                StrWord = cWordG3((decimal)(double.Parse(strNum.Substring(0, strNum.Length - 3)))) + " " + cWord3((decimal)(double.Parse(strNum.Substring(strNum.Length - 2 - 1))));
            }
            returnValue = StrWord;
            return returnValue;
        }
        static public string cWordG3(decimal Num)
        {
            string returnValue;
            //2. more than three digit number.
            string strNum = "";
            string StrWord = "";
            string readNum = "";
            strNum = Num.ToString();
            if (strNum.Length % 2 != 0)
            {
                readNum = System.Convert.ToString(double.Parse(strNum.Substring(0, 1)));
                if (readNum != "0")
                {
                    StrWord = retWord(decimal.Parse(readNum));
                    readNum = System.Convert.ToString(double.Parse("1" + strReplicate("0", strNum.Length - 1) + "000"));
                    StrWord = StrWord + " " + retWord(decimal.Parse(readNum));
                }
                strNum = strNum.Substring(1);
            }
            while (!System.Convert.ToBoolean(strNum.Length == 0))
            {
                readNum = System.Convert.ToString(double.Parse(strNum.Substring(0, 2)));
                if (readNum != "0")
                {
                    StrWord = StrWord + " " + cWord3(decimal.Parse(readNum));
                    readNum = System.Convert.ToString(double.Parse("1" + strReplicate("0", strNum.Length - 2) + "000"));
                    StrWord = StrWord + " " + retWord(decimal.Parse(readNum));
                }
                strNum = strNum.Substring(2);
            }
            returnValue = StrWord;
            return returnValue;
        }
        static public string cWord3(decimal Num)
        {
            string returnValue;
            //1. Three or less digit number.
            string strNum = "";
            string StrWord = "";
            string readNum = "";
            if (Num < 0)
            {
                Num = Num * -1;
            }
            strNum = Num.ToString();


            if (strNum.Length == 3)
            {
                readNum = System.Convert.ToString(double.Parse(strNum.Substring(0, 1)));
                StrWord = retWord(decimal.Parse(readNum)) + " Hundred";
                strNum = strNum.Substring(1, strNum.Length - 1);
            }


            if (strNum.Length <= 2)
            {
                if (double.Parse(strNum) >= 0 && double.Parse(strNum) <= 20)
                {
                    StrWord = StrWord + " " + retWord((decimal)(double.Parse(strNum)));
                }
                else
                {
                    StrWord = StrWord + " " + retWord((decimal)(System.Convert.ToDouble(strNum.Substring(0, 1) + "0"))) + " " + retWord((decimal)(double.Parse(strNum.Substring(1, 1))));
                }
            }


            strNum = Num.ToString();
            returnValue = StrWord;
            return returnValue;
        }
        static public string retWord(decimal Num)
        {
            string returnValue;
            //This two dimensional array store the primary word convertion of number.
            returnValue = "";
            object[,] ArrWordList = new object[,] { { 0, "" }, { 1, "One" }, { 2, "Two" }, { 3, "Three" }, { 4, "Four" }, { 5, "Five" }, { 6, "Six" }, { 7, "Seven" }, { 8, "Eight" }, { 9, "Nine" }, { 10, "Ten" }, { 11, "Eleven" }, { 12, "Twelve" }, { 13, "Thirteen" }, { 14, "Fourteen" }, { 15, "Fifteen" }, { 16, "Sixteen" }, { 17, "Seventeen" }, { 18, "Eighteen" }, { 19, "Nineteen" }, { 20, "Twenty" }, { 30, "Thirty" }, { 40, "Forty" }, { 50, "Fifty" }, { 60, "Sixty" }, { 70, "Seventy" }, { 80, "Eighty" }, { 90, "Ninety" }, { 100, "Hundred" }, { 1000, "Thousand" }, { 100000, "Lakh" }, { 10000000, "Crore" } };


            int i;
            for (i = 0; i <= (ArrWordList.Length - 1); i++)
            {
                if (Num == System.Convert.ToDecimal(ArrWordList[i, 0]))
                {
                    returnValue = (string)(ArrWordList[i, 1]);
                    break;
                }
            }
            return returnValue;
        }
        static public string strReplicate(string str, int intD)
        {
            string returnValue;
            //This fucntion padded "0" after the number to evaluate hundred, thousand and on....
            //using this function you can replicate any Charactor with given string.
            int i;
            returnValue = "";
            for (i = 1; i <= intD; i++)
            {
                returnValue = returnValue + str;
            }
            return returnValue;
        }

        public static bool EmailAlert(string Message)
        {
            bool _bResult = false;

            //try
            //{
            //    string fromemail = "";
            //    string txtPort = "";
            //    string txtHost = "";
            //    string txtPassword = "";
            //    string txtToMailId = "";
            //    //string txtCCEmail = "";
            //    string _sFetch_det = "SELECT top 1 * FROM AlertEmailMaster ORDER BY CreatedDate DESC";
            //    SqlDataReader _rdr_Fetch_det = Utilities.executeQuery(_sFetch_det);
            //    if (_rdr_Fetch_det.HasRows)
            //    {
            //        if (_rdr_Fetch_det.Read())
            //        {
            //            fromemail = _rdr_Fetch_det["FromEmail"].ToString().Trim();
            //            txtPort = _rdr_Fetch_det["Port"].ToString().Trim();
            //            txtHost = _rdr_Fetch_det["Host"].ToString().Trim();
            //            txtPassword = _rdr_Fetch_det["FromPwd"].ToString().Trim();
            //            txtToMailId = _rdr_Fetch_det["Email"].ToString().Trim();
            //            //txtCCEmail = _rdr_Fetch_det["M_CCMAILID"].ToString().Trim();
            //        }

            //        System.Net.Mail.MailMessage alert = new System.Net.Mail.MailMessage();
            //        System.Net.Mail.SmtpClient smtp = new SmtpClient();
            //        alert.From = new MailAddress(fromemail.ToString().Trim());
            //        alert.To.Add(txtToMailId.ToString().Trim());
            //        //alert.CC.Add(txtCCEmail.ToString().Trim());
            //        alert.Subject = "EnSi 2.0 ALERT";
            //        alert.Body = "Hello there! \n" + Message.ToString().Trim();
            //        alert.IsBodyHtml = true;
            //        //excel attachment
            //        //if (Global.Attachment != "")
            //        //{
            //        //    System.Net.Mail.Attachment attachment;
            //        //    attachment = new System.Net.Mail.Attachment(AppDomain.CurrentDomain.BaseDirectory + "InvoiceDetails " + DateTime.Now.ToString("yyyy-MM-dd") + ".csv");
            //        //    alert.Attachments.Add(attachment);
            //        //    Global.Attachment = "";
            //        //}
            //        smtp.Port = Convert.ToInt32(txtPort.Trim());
            //        smtp.Host = txtHost.ToString().Trim();
            //        smtp.EnableSsl = true;
            //        smtp.UseDefaultCredentials = false;
            //        smtp.Credentials = new NetworkCredential(fromemail.ToString().Trim(), txtPassword.ToString().Trim());
            //        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            //        smtp.Send(alert);
            _bResult = true;
            //    }
            //}
            //catch (Exception)
            //{
            //}
            return _bResult;
        }

        public static void LogIrpResult(string Irpres)
        {
            StringBuilder sb = new StringBuilder();
            string path = AppDomain.CurrentDomain.BaseDirectory + "LogIRPRes.txt";
            // flush every 20 seconds as you do it

            sb.AppendLine();
            sb.Append("Log Time: " + DateTime.Now);
            sb.AppendLine();
            sb.Append("--------------------------------------------------");
            sb.AppendLine();
            if (Irpres != null)
            {
                sb.Append(Irpres);
            }
            sb.AppendLine();
            sb.Append("--------------------------------------------------");

            File.AppendAllText(path, sb.ToString());
            sb.Clear();
        }

        public static void LogQuery(string pdfname)
        {
            StringBuilder sb = new StringBuilder();
            string path = AppDomain.CurrentDomain.BaseDirectory + "LogQuery.txt";
            // flush every 20 seconds as you do it

            sb.AppendLine();
            sb.Append("Log Time: " + DateTime.Now);
            sb.AppendLine();
            sb.Append("--------------------------------------------------");
            sb.AppendLine();
            if (pdfname != null)
            {
                sb.Append(pdfname);
            }
            sb.AppendLine();
            sb.Append("--------------------------------------------------");

            File.AppendAllText(path, sb.ToString());
            sb.Clear();
        }

        public static object executeScalar(string query)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["strcon"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    return cmd.ExecuteScalar();
                }
            }
        }

    }
}
