using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DSM
{
    public class Print
    {
        static string Invno;
        public Boolean PrintInvoicePDFs(string InvoiceNo, string InvType)
        {
            try
            {
                Process proc = new Process();
                //proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //proc.StartInfo.Arguments = "\"" + Global.PrinterName + "\"";
                //proc.StartInfo.Verb = "print";
                //proc.StartInfo.Verb = "printto";

                string pdfFileName = "";

                if (Global.PrintType == "ORIGINAL")
                {
                    pdfFileName = Global.OutputPath + @"\" + InvoiceNo.ToString().Trim() + "ORIGINAL" + InvType.ToString().Trim() + ".pdf";
                    if (System.IO.File.Exists(pdfFileName))
                    {
                        proc.StartInfo.FileName = System.IO.Path.GetFullPath(pdfFileName);
                        proc.StartInfo.CreateNoWindow = true;
                        proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        proc.StartInfo.Arguments = "\"" + Global.PrinterName + "\"";
                        proc.StartInfo.Verb = "printto";
                        //Task.Delay(3000);
                        System.Threading.Thread.Sleep(5000);
                        proc.Start();
                        if (proc.HasExited == false)
                        {
                            proc.WaitForExit(12000);
                        }
                        proc.EnableRaisingEvents = true;

                        proc.Close();
                        FindAndKillProcess("AcroRd32");

                        System.IO.StreamReader file = new System.IO.StreamReader(pdfFileName);
                        string _SourceFile = pdfFileName;

                        //check folder
                        if (!System.IO.Directory.Exists(Global.OutputPath + "\\PRINT\\"))
                        {
                            System.IO.Directory.CreateDirectory(Global.OutputPath + "\\PRINT\\");
                        }
                        string _DestFile = Global.OutputPath + "\\PRINT\\" + System.IO.Path.GetFileName(pdfFileName);
                        file.Close();
                        file.Dispose();
                        if (File.Exists(_DestFile))
                            File.Delete(_DestFile);
                        File.Move(_SourceFile, _DestFile);
                    }
                    else
                    {
                        pdfFileName = Global.OutputPath + @"\E6_1_FA9NA_" + InvoiceNo.ToString().Trim() + ".pdf";
                        if (System.IO.File.Exists(pdfFileName))
                        {
                            proc.StartInfo.FileName = System.IO.Path.GetFullPath(pdfFileName);
                            proc.StartInfo.UseShellExecute = true;
                            proc.StartInfo.CreateNoWindow = true;
                            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                            proc.StartInfo.Arguments = "\"" + Global.PrinterName + "\"";
                            //proc.StartInfo.Arguments = @" /h /p " + "\"" + pdfFileName + "\"" + " " + "\"" + Global.PrinterName + "\"";
                            proc.StartInfo.Verb = "printto";
                            //Task.Delay(3000);
                            System.Threading.Thread.Sleep(5000);
                            proc.Start();
                            if (proc.HasExited == false)
                            {
                                proc.WaitForExit(12000);
                                //proc.WaitForExit(3000);
                            }
                            proc.EnableRaisingEvents = true;

                            proc.Close();
                            FindAndKillProcess("AcroRd32");

                            System.IO.StreamReader file = new System.IO.StreamReader(pdfFileName);
                            string _SourceFile = pdfFileName;

                            //check folder
                            if (!System.IO.Directory.Exists(Global.OutputPath + "\\PRINT\\"))
                            {
                                System.IO.Directory.CreateDirectory(Global.OutputPath + "\\PRINT\\");
                            }
                            string _DestFile = Global.OutputPath + "\\PRINT\\" + System.IO.Path.GetFileName(pdfFileName);
                            file.Close();
                            file.Dispose();
                            if (File.Exists(_DestFile))
                                File.Delete(_DestFile);
                            File.Move(_SourceFile, _DestFile);
                        }
                    }
                }
                else
                {
                    string[] printCopies = Global.PrintCopies.Split('-');
                    for (int i = 0; i < printCopies.Length; i++)
                    {
                        //if (InvType == "DCInvoice" && printCopies[i].ToUpper().Trim() == "TRIPLICATE")
                        //    pdfFileName = Global.OutputPath + @"\" + InvoiceNo.ToString().Trim() + "QUADRAPLICATE" + InvType.ToString().Trim() + ".pdf";
                        //else
                        pdfFileName = Global.OutputPath + @"\" + InvoiceNo.ToString().Trim() + printCopies[i].ToUpper().Trim() + InvType.ToString().Trim() + ".pdf";

                        //Get application path will get default application for given file type ("pdf")
                        //This will allow you to not care if its adobe reader 10 or adobe acrobat.
                        if (System.IO.File.Exists(pdfFileName))
                        {
                            
                            proc.StartInfo.FileName = System.IO.Path.GetFullPath(pdfFileName);
                            proc.StartInfo.CreateNoWindow = true;
                            
                            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                            proc.StartInfo.Arguments = "\"" + Global.PrinterName + "\"";
                            proc.StartInfo.Verb = "printto";
                            
                            //proc.StartInfo.UseShellExecute = false;
                            //Task.Delay(2000);
                            //Process.GetProcessById(proc.Id);
                            //System.Threading.Thread.Sleep(5000);
                            proc.Start();
                            //proc.WaitForExit();
                            //System.Threading.Thread.Sleep(3000);
                            if (proc.HasExited == false)
                            {
                                proc.WaitForExit(12000);
                            }

                            proc.EnableRaisingEvents = true;

                            proc.Close();

                            FindAndKillProcess("AcroRd32");

                            System.IO.StreamReader file = new System.IO.StreamReader(pdfFileName);
                            string _SourceFile = pdfFileName;

                            //check folder
                            if (!System.IO.Directory.Exists(Global.OutputPath + "\\PRINT\\"))
                            {
                                System.IO.Directory.CreateDirectory(Global.OutputPath + "\\PRINT\\");
                            }
                            string _DestFile = Global.OutputPath + "\\PRINT\\" + System.IO.Path.GetFileName(pdfFileName);
                            file.Close();
                            file.Dispose();
                            if (File.Exists(_DestFile))
                                File.Delete(_DestFile);
                            File.Move(_SourceFile, _DestFile);

                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Global.LogErrors(ex);
                return false;
            }
        }

        public Boolean ReprintInvoicePDFs(string InvoiceNo, string InvType)
        {
            try
            {
                Process proc = new Process();
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                proc.StartInfo.Arguments = "\"" + Global.PrinterName + "\"";
                //proc.StartInfo.Verb = "print";
                proc.StartInfo.Verb = "printto";

                string pdfFileName = "";

                if (Global.PrintType == "ORIGINAL")
                {
                    pdfFileName = Global.OutputPath + @"\" + InvoiceNo.ToString().Trim() + "ORIGINAL" + InvType.ToString().Trim() + ".pdf";
                    if (System.IO.File.Exists(pdfFileName))
                    {
                        proc.StartInfo.FileName = System.IO.Path.GetFullPath(pdfFileName);
                        proc.StartInfo.CreateNoWindow = true;
                        proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        proc.StartInfo.Arguments = "\"" + Global.PrinterName + "\"";
                        proc.StartInfo.Verb = "printto";
                        proc.Start();
                        if (proc.HasExited == false)
                        {
                            proc.WaitForExit(12000);
                        }
                        proc.EnableRaisingEvents = true;

                        proc.Close();
                        FindAndKillProcess("AcroRd32");

                        System.IO.StreamReader file = new System.IO.StreamReader(pdfFileName);
                        string _SourceFile = pdfFileName;

                        //check folder
                        if (!System.IO.Directory.Exists(Global.OutputPath + "\\REPRINT\\"))
                        {
                            System.IO.Directory.CreateDirectory(Global.OutputPath + "\\REPRINT\\");
                        }
                        string _DestFile = Global.OutputPath + "\\REPRINT\\" + System.IO.Path.GetFileName(pdfFileName);
                        if (File.Exists(_DestFile))
                            File.Delete(_DestFile);
                        file.Close();
                        file.Dispose();
                        File.Move(_SourceFile, _DestFile);
                    }
                    else
                    {
                        pdfFileName = Global.OutputPath + @"\PRINT\" + InvoiceNo.ToString().Trim() + "ORIGINAL" + InvType.ToString().Trim() + ".pdf";
                        if (System.IO.File.Exists(pdfFileName))
                        {
                            proc.StartInfo.FileName = System.IO.Path.GetFullPath(pdfFileName);
                            proc.StartInfo.CreateNoWindow = true;
                            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                            proc.StartInfo.Arguments = "\"" + Global.PrinterName + "\"";
                            proc.StartInfo.Verb = "printto";
                            proc.Start();
                            if (proc.HasExited == false)
                            {
                                proc.WaitForExit(12000);
                            }
                            proc.EnableRaisingEvents = true;

                            proc.Close();
                            FindAndKillProcess("AcroRd32");

                            System.IO.StreamReader file = new System.IO.StreamReader(pdfFileName);
                            string _SourceFile = pdfFileName;

                            //check folder
                            if (!System.IO.Directory.Exists(Global.OutputPath + "\\REPRINT\\"))
                            {
                                System.IO.Directory.CreateDirectory(Global.OutputPath + "\\REPRINT\\");
                            }
                            string _DestFile = Global.OutputPath + "\\REPRINT\\" + System.IO.Path.GetFileName(pdfFileName);
                            if (File.Exists(_DestFile))
                                File.Delete(_DestFile);
                            file.Close();
                            file.Dispose();
                            File.Move(_SourceFile, _DestFile);
                        }
                        else
                        {
                            pdfFileName = Global.OutputPath + @"\E6_1_FA9NA_" + InvoiceNo.ToString().Trim() + ".pdf";
                            if (System.IO.File.Exists(pdfFileName))
                            {
                                proc.StartInfo.FileName = System.IO.Path.GetFullPath(pdfFileName);
                                proc.StartInfo.CreateNoWindow = true;
                                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                                proc.StartInfo.Arguments = "\"" + Global.PrinterName + "\"";
                                proc.StartInfo.Verb = "printto";
                                proc.Start();
                                if (proc.HasExited == false)
                                {
                                    proc.WaitForExit(12000);
                                }
                                proc.EnableRaisingEvents = true;

                                proc.Close();
                                FindAndKillProcess("AcroRd32");

                                System.IO.StreamReader file = new System.IO.StreamReader(pdfFileName);
                                string _SourceFile = pdfFileName;

                                //check folder
                                if (!System.IO.Directory.Exists(Global.OutputPath + "\\REPRINT\\"))
                                {
                                    System.IO.Directory.CreateDirectory(Global.OutputPath + "\\REPRINT\\");
                                }
                                string _DestFile = Global.OutputPath + "\\REPRINT\\" + System.IO.Path.GetFileName(pdfFileName);
                                if (File.Exists(_DestFile))
                                    File.Delete(_DestFile);
                                file.Close();
                                file.Dispose();
                                File.Move(_SourceFile, _DestFile);
                            }
                            else
                            {
                                string _con_Str = ConfigurationManager.ConnectionStrings["strcon"].ConnectionString;

                                //string _con_Str = ConfigurationSettings.AppSettings["strcon"].ToString();
                                using (SqlConnection _objCon = new SqlConnection(_con_Str))
                                {
                                    //if (_objCon.State == System.Data.ConnectionState.Closed) _objCon.Open();
                                    // _objCmd.Connection = _objCon;
                                    _objCon.Open();
                                    //using (var varConnection = Locale.sqlConnectOneTime(Locale.sqlDataConnectionDetails))
                                    //using (var sqlWrite = new SqlCommand("UPDATE invoice SET DS_PDFFILE=@File WHERE DS_STATUS='FALSE' AND INVOICE_NO='" + InvoiceNo.ToString().Trim() + "'", _objCon))
                                    //{
                                    //    sqlWrite.Parameters.Add("@File", SqlDbType.VarBinary, file.Length).Value = file;
                                    //    sqlWrite.ExecuteNonQuery();
                                    //}

                                    //using (var varConnection = Locale.sqlConnectOneTime(Locale.sqlDataConnectionDetails))
                                    //using (var sqlQuery = new SqlCommand(@"select DS_SIGN from invoice where invoice_no = @varID", _objCon))


                                    using (var sqlQuery = new SqlCommand(@"select OriginalPDF from Invoice where InvNumber = @varID", _objCon))
                                    {
                                        sqlQuery.Parameters.AddWithValue("@varID", InvoiceNo.ToString().Trim());
                                        using (var sqlQueryResult = sqlQuery.ExecuteReader())
                                            if (sqlQueryResult != null)
                                            {
                                                sqlQueryResult.Read();
                                                var blob = new Byte[(sqlQueryResult.GetBytes(0, 0, null, 0, int.MaxValue))];
                                                sqlQueryResult.GetBytes(0, 0, blob, 0, blob.Length);
                                                //using (var fs = new FileStream(@"D:\READPDF\INVOICE\Empty.pdf", FileMode.Create, FileAccess.Write))
                                                //    fs.Write(blob, 0, blob.Length);
                                                //pdfFileName = @"D:\READPDF\INVOICE\Empty.pdf";
                                                using (var fs = new FileStream(@"C:\EnSi\Empty.pdf", FileMode.Create, FileAccess.Write))
                                                    fs.Write(blob, 0, blob.Length);
                                                //pdfFileName = @"D:\READPDF\INVOICE\Empty.pdf";
                                                pdfFileName = @"C:\EnSi\Empty.pdf";
                                            }
                                    }

                                    _objCon.Close();
                                }
                                if (System.IO.File.Exists(pdfFileName))
                                {
                                    proc.StartInfo.FileName = System.IO.Path.GetFullPath(pdfFileName);
                                    proc.StartInfo.CreateNoWindow = true;
                                    proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                                    proc.StartInfo.Arguments = "\"" + Global.PrinterName + "\"";
                                    proc.StartInfo.Verb = "printto";
                                    proc.Start();
                                    if (proc.HasExited == false)
                                    {
                                        proc.WaitForExit(12000);
                                    }
                                    proc.EnableRaisingEvents = true;

                                    proc.Close();
                                    FindAndKillProcess("AcroRd32");

                                    System.IO.StreamReader file = new System.IO.StreamReader(pdfFileName);
                                    string _SourceFile = pdfFileName;

                                    //check folder
                                    if (!System.IO.Directory.Exists(@"C:\EnSi" + "\\REPRINT\\"))
                                    {
                                        System.IO.Directory.CreateDirectory(@"C:\EnSi" + "\\REPRINT\\");
                                    }
                                    string _DestFile = @"C:\EnSi" + "\\REPRINT\\" + System.IO.Path.GetFileName(pdfFileName);
                                    if (File.Exists(_DestFile))
                                        File.Delete(_DestFile);
                                    file.Close();
                                    file.Dispose();
                                    File.Move(_SourceFile, _DestFile);
                                }
                            }
                        }
                    }
                }
                else
                {
                    string[] printCopies = Global.PrintCopies.Split('-');
                    for (int i = 0; i < printCopies.Length; i++)
                    {
                        //if (InvType == "DCInvoice" && printCopies[i].ToUpper().Trim() == "TRIPLICATE")
                        //    pdfFileName = Global.OutputPath + @"\" + InvoiceNo.ToString().Trim() + "QUADRAPLICATE" + InvType.ToString().Trim() + ".pdf";
                        //else
                        pdfFileName = Global.OutputPath + @"\" + InvoiceNo.ToString().Trim() + printCopies[i].ToUpper().Trim() + InvType.ToString().Trim() + ".pdf";

                        //Get application path will get default application for given file type ("pdf")
                        //This will allow you to not care if its adobe reader 10 or adobe acrobat.
                        if (System.IO.File.Exists(pdfFileName))
                        {
                            proc.StartInfo.FileName = System.IO.Path.GetFullPath(pdfFileName);
                            proc.StartInfo.CreateNoWindow = true;
                            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                            proc.StartInfo.Arguments = "\"" + Global.PrinterName + "\"";
                            proc.StartInfo.Verb = "printto";
                            proc.Start();
                            if (proc.HasExited == false)
                            {
                                proc.WaitForExit(12000);
                            }

                            proc.EnableRaisingEvents = true;

                            proc.Close();
                            FindAndKillProcess("AcroRd32");

                            System.IO.StreamReader file = new System.IO.StreamReader(pdfFileName);
                            string _SourceFile = pdfFileName;

                            //check folder
                            if (!System.IO.Directory.Exists(Global.OutputPath + "\\REPRINT\\"))
                            {
                                System.IO.Directory.CreateDirectory(Global.OutputPath + "\\REPRINT\\");
                            }
                            string _DestFile = Global.OutputPath + "\\REPRINT\\" + System.IO.Path.GetFileName(pdfFileName);
                            if (File.Exists(_DestFile))
                                File.Delete(_DestFile);
                            file.Close();
                            file.Dispose();
                            File.Move(_SourceFile, _DestFile);
                        }
                        
                        else
                        {
                            string _con_Str = ConfigurationManager.ConnectionStrings["strcon"].ConnectionString;

                            //string _con_Str = ConfigurationSettings.AppSettings["strcon"].ToString();
                            using (SqlConnection _objCon = new SqlConnection(_con_Str))
                            {
                               
                                _objCon.Open();
                                
                                using (var sqlQuery = new SqlCommand(@"select OriginalPDF from Invoice where InvNumber = @varID", _objCon))
                                {
                                    sqlQuery.Parameters.AddWithValue("@varID", InvoiceNo.ToString().Trim());
                                    using (var sqlQueryResult = sqlQuery.ExecuteReader())
                                        if (sqlQueryResult != null)
                                        {
                                            sqlQueryResult.Read();
                                            var blob = new Byte[(sqlQueryResult.GetBytes(0, 0, null, 0, int.MaxValue))];
                                            sqlQueryResult.GetBytes(0, 0, blob, 0, blob.Length); 
                                            using (var fs = new FileStream(@"C:\EnSi\Empty.pdf", FileMode.Create, FileAccess.Write))
                                                fs.Write(blob, 0, blob.Length);
                                            //pdfFileName = @"D:\READPDF\INVOICE\Empty.pdf";
                                            pdfFileName = @"C:\EnSi\Empty.pdf";
                                        }
                                }

                                _objCon.Close();
                            }
                            if (System.IO.File.Exists(pdfFileName))
                            {
                                proc.StartInfo.FileName = System.IO.Path.GetFullPath(pdfFileName);
                                proc.StartInfo.CreateNoWindow = true;
                                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                                proc.StartInfo.Arguments = "\"" + Global.PrinterName + "\"";
                                proc.StartInfo.Verb = "printto";
                                proc.Start();
                                if (proc.HasExited == false)
                                {
                                    proc.WaitForExit(12000);
                                }
                                proc.EnableRaisingEvents = true;

                                proc.Close();
                                FindAndKillProcess("AcroRd32");

                                System.IO.StreamReader file = new System.IO.StreamReader(pdfFileName);
                                string _SourceFile = pdfFileName;

                                //check folder
                                if (!System.IO.Directory.Exists(@"C:\EnSi" + "\\REPRINT\\"))
                                {
                                    System.IO.Directory.CreateDirectory(@"C:\EnSi" + "\\REPRINT\\");
                                }
                                string _DestFile = @"C:\EnSi" + "\\REPRINT\\" + System.IO.Path.GetFileName(pdfFileName);
                                if (File.Exists(_DestFile))
                                    File.Delete(_DestFile);
                                file.Close();
                                file.Dispose();
                                File.Move(_SourceFile, _DestFile);
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Global.LogErrors(ex);
                return false;
            }
        }

        public Boolean ReportReprintInvoicePDFs(string InvoiceNo,string InvType)
        {
            try
            {
                Process proc = new Process();
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                proc.StartInfo.Arguments = "\"" + Global.PrinterName + "\"";
                //proc.StartInfo.Verb = "print";
                proc.StartInfo.Verb = "printto";

                string pdfFileName = "";

                string _con_Str = ConfigurationManager.ConnectionStrings["strcon"].ConnectionString;

                //string _con_Str = ConfigurationSettings.AppSettings["strcon"].ToString();
                using (SqlConnection _objCon = new SqlConnection(_con_Str))
                {
                    //if (_objCon.State == System.Data.ConnectionState.Closed) _objCon.Open();
                    // _objCmd.Connection = _objCon;
                    _objCon.Open();
                    //using (var varConnection = Locale.sqlConnectOneTime(Locale.sqlDataConnectionDetails))
                    //using (var sqlWrite = new SqlCommand("UPDATE invoice SET DS_PDFFILE=@File WHERE DS_STATUS='FALSE' AND INVOICE_NO='" + InvoiceNo.ToString().Trim() + "'", _objCon))
                    //{
                    //    sqlWrite.Parameters.Add("@File", SqlDbType.VarBinary, file.Length).Value = file;
                    //    sqlWrite.ExecuteNonQuery();
                    //}

                    //using (var varConnection = Locale.sqlConnectOneTime(Locale.sqlDataConnectionDetails))
                    //using (var sqlQuery = new SqlCommand(@"select DS_SIGN from invoice where invoice_no = @varID", _objCon))


                    using (var sqlQuery = new SqlCommand(@"select OriginalPDF from Invoice where InvNumber = @varID", _objCon))
                    {
                        sqlQuery.Parameters.AddWithValue("@varID", InvoiceNo.ToString().Trim());
                        using (var sqlQueryResult = sqlQuery.ExecuteReader())
                            if (sqlQueryResult != null)
                            {
                                sqlQueryResult.Read();
                                var blob = new Byte[(sqlQueryResult.GetBytes(0, 0, null, 0, int.MaxValue))];
                                sqlQueryResult.GetBytes(0, 0, blob, 0, blob.Length);
                                //using (var fs = new FileStream(@"D:\READPDF\INVOICE\Empty.pdf", FileMode.Create, FileAccess.Write))
                                //    fs.Write(blob, 0, blob.Length);
                                //pdfFileName = @"D:\READPDF\INVOICE\Empty.pdf";
                                using (var fs = new FileStream(@"C:\EnSi\" + InvoiceNo.ToString().Trim() + ".pdf", FileMode.Create, FileAccess.Write))
                                    fs.Write(blob, 0, blob.Length);
                                //pdfFileName = @"D:\READPDF\INVOICE\Empty.pdf";
                                pdfFileName = @"C:\EnSi\"+ InvoiceNo.ToString().Trim() + ".pdf";
                            }
                    }

                    _objCon.Close();
                }
                if (System.IO.File.Exists(pdfFileName))
                {
                    proc.StartInfo.FileName = System.IO.Path.GetFullPath(pdfFileName);
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    proc.StartInfo.Arguments = "\"" + Global.PrinterName + "\"";
                    proc.StartInfo.Verb = "printto";
                    proc.Start();
                    if (proc.HasExited == false)
                    {
                        proc.WaitForExit(12000);
                    }
                    proc.EnableRaisingEvents = true;

                    proc.Close();
                    FindAndKillProcess("AcroRd32");

                    System.IO.StreamReader file = new System.IO.StreamReader(pdfFileName);
                    string _SourceFile = pdfFileName;

                    //check folder
                    if (!System.IO.Directory.Exists(@"C:\EnSi" + "\\REPRINT\\"))
                    {
                        System.IO.Directory.CreateDirectory(@"C:\EnSi" + "\\REPRINT\\");
                    }
                    string _DestFile = @"C:\EnSi" + "\\REPRINT\\" + System.IO.Path.GetFileName(pdfFileName);
                    if (File.Exists(_DestFile))
                        File.Delete(_DestFile);
                    file.Close();
                    file.Dispose();
                    File.Move(_SourceFile, _DestFile);
                }
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public static bool FindAndKillProcess(string name)
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName.StartsWith(name))
                {
                    clsProcess.Kill();
                    return true;
                }
                if (clsProcess.ProcessName.Contains("Acrobat"))
                {
                    clsProcess.Kill();
                    return true;
                }
            }
            return false;
        }
    }
}
