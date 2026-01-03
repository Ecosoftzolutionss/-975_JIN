using iTextSharp.text.log;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Security;
//
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;
using DSMData;
using DSM;
using System.Net.Mail;
using System.Net;
using Spire.Pdf.General.Find;
using Spire.Pdf.OPC;
using Spire.Pdf;
using System.Drawing;
using System.Linq;
using System.Text;

namespace DSM
{
    public class DigitalSign
    {
        IList<X509Certificate> chain = new List<X509Certificate>();
        X509Certificate2 pk = null;
        X509Certificate2 pkval = null;
        DSMModelData objDSMModelData = new DSMModelData();
        Print objPrint = new Print();


        public bool FlatDigitalSignatureInitialize(string invtype, string invNo, string DS_comp_id, string t_inv_no, string DScoord)
        {
            try
            {
                int certfound = 0;

                string PinCode = "";
                string KeyContainerName = "";
                string ProviderName = "";

                int errcount = 0;

                int rescount = 0;
                var dsDataDetail = objDSMModelData.GetDSdetailbyClientId(DS_comp_id);
                for (var i = 0; i < dsDataDetail.Count; i++)
                {
                    string DSSignName = dsDataDetail[0].KeyName.ToString().Trim();

                    errcount = 0;
                    certfound = 0;

                    IOcspClient ocspClient = new OcspClientBouncyCastle();
                    ITSAClient tsaClient = null;
                    chain.Clear();
                    pk = null;
                    IList<ICrlClient> crlList = new List<ICrlClient>();
                    crlList.Add(new CrlClientOnline(chain));
                    var dsMasterData = objDSMModelData.GetDSmasterByKeyName(DSSignName);

                    PinCode = dsMasterData.KeyPinNo;
                    KeyContainerName = dsMasterData.ContainerName;
                    ProviderName = dsMasterData.ProviderName;

                    if (PinCode != "" && KeyContainerName != "" && ProviderName != "" && PinCode.ToString().Trim().Length > 0 && KeyContainerName.ToString().Trim().Length > 0 && ProviderName.ToString().Trim().Length > 0)
                    {
                        if (PinCode != "")
                        {
                            //if pin code is set then no windows form will popup to ask it
                            SecureString pwd = GetSecurePin(PinCode);
                            CspParameters csp = new CspParameters(1,
                                                                    ProviderName,
                                                                    KeyContainerName,
                                                                    new System.Security.AccessControl.CryptoKeySecurity(),
                                                                    pwd);
                            try
                            {
                                RSACryptoServiceProvider rsaCsp = new RSACryptoServiceProvider(csp);
                                // the pin code will be cached for next access to the smart card
                            }
                            catch (Exception ex)
                            {
                                //MessageBox.Show("Crypto error: " + ex.Message);
                                //string _sQuery_Quarantine = "UPDATE INVOICE SET SYSTEM_PRINT_STATUS='FALSE QUARANTINE' WHERE SYSTEM_PRINT_STATUS='FALSE' AND H_INVOICE_NO='" + invNo.ToString().Trim() + "'";
                                // db_upd_tmr.Start();
                                //bool _bQuarantineResult = DBCL.executeNonQuery(_sQuery_Quarantine);
                                //return false;
                                errcount++;
                            }
                        }
                        if (errcount == 0)
                        {
                            X509Store store = new X509Store(StoreLocation.CurrentUser);
                            store.Open(OpenFlags.ReadOnly);
                            X509Certificate2 cert = null;
                            if ((ProviderName == "") || (KeyContainerName == ""))
                            {
                                MessageBox.Show("You must set Provider Name and Key Container Name");
                                return false;
                            }
                            foreach (X509Certificate2 cert2 in store.Certificates)
                            {
                                if (cert2.HasPrivateKey)
                                {
                                    RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)cert2.PrivateKey;
                                    if (rsa == null) continue; // not smart card cert again
                                    if (rsa.CspKeyContainerInfo.HardwareDevice) // sure - smartcard
                                    {
                                        if ((rsa.CspKeyContainerInfo.KeyContainerName == KeyContainerName) && (rsa.CspKeyContainerInfo.ProviderName == ProviderName))
                                        {
                                            //we find it
                                            cert = cert2;
                                            certfound = 1;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (cert == null)
                            {
                                //MessageBox.Show("Certificate not found");
                                //return false;
                            }
                            if (certfound == 1)
                            {
                                bool _resSign = SignWithThisCert(cert, DSSignName.ToString().Trim().ToUpper(), invtype.ToString().Trim(), invNo.ToString().Trim(), t_inv_no.ToString().Trim(), DScoord);
                                if (_resSign)
                                {
                                   rescount++;
                                    //rescount = '1';
                                    break;
                                }
                            }
                        }

                    }
                }
                if (rescount > 0)
                {
                    return true;
                }
                else
                {
                    int dssigncount = 0;
                    var dsMasterData = objDSMModelData.GetAllKeysDSmaster();
                    for (int i = 0; i < dsMasterData.Count; i++)
                    {
                        errcount = 0;
                        certfound = 0;

                        string DSSignName = dsMasterData[i].KeyName;

                        PinCode = dsMasterData[i].KeyPinNo;
                        KeyContainerName = dsMasterData[i].ContainerName; ;
                        ProviderName = dsMasterData[i].ProviderName;

                        if (PinCode != "" && KeyContainerName != "" && ProviderName != "" && PinCode.ToString().Trim().Length > 0 && KeyContainerName.ToString().Trim().Length > 0 && ProviderName.ToString().Trim().Length > 0)
                        {
                            if (PinCode != "")
                            {
                                //if pin code is set then no windows form will popup to ask it
                                SecureString pwd = GetSecurePin(PinCode);
                                CspParameters csp = new CspParameters(1,
                                                                        ProviderName,
                                                                        KeyContainerName,
                                                                        new System.Security.AccessControl.CryptoKeySecurity(),
                                                                        pwd);
                                try
                                {
                                    RSACryptoServiceProvider rsaCsp = new RSACryptoServiceProvider(csp);
                                    // the pin code will be cached for next access to the smart card
                                }
                                catch (Exception ex)
                                {
                                    //MessageBox.Show("Crypto error: " + ex.Message);
                                    ////string _sQuery_Quarantine = "UPDATE INVOICE SET SYSTEM_PRINT_STATUS='FALSE QUARANTINE' WHERE SYSTEM_PRINT_STATUS='FALSE' AND H_INVOICE_NO='" + invNo.ToString().Trim() + "'";
                                    //// db_upd_tmr.Start();
                                    ////bool _bQuarantineResult = DBCL.executeNonQuery(_sQuery_Quarantine);
                                    //return false;
                                    errcount++;
                                }
                            }

                            if (errcount == 0)
                            {

                                IOcspClient ocspClient = new OcspClientBouncyCastle();
                                ITSAClient tsaClient = null;
                                //chain = "";
                                // chain.Remove();
                                chain.Clear();
                                pk = null;
                                IList<ICrlClient> crlList = new List<ICrlClient>();
                                crlList.Add(new CrlClientOnline(chain));
                                X509Store store = new X509Store(StoreLocation.CurrentUser);
                                store.Open(OpenFlags.ReadOnly);
                                X509Certificate2 cert = null;
                                if ((ProviderName == "") || (KeyContainerName == ""))
                                {
                                    MessageBox.Show("You must set Provider Name and Key Container Name");
                                    return false;
                                }
                                foreach (X509Certificate2 cert2 in store.Certificates)
                                {
                                    if (cert2.HasPrivateKey)
                                    {
                                        RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)cert2.PrivateKey;
                                        if (rsa == null) continue; // not smart card cert again
                                        if (rsa.CspKeyContainerInfo.HardwareDevice) // sure - smartcard
                                        {
                                            if ((rsa.CspKeyContainerInfo.KeyContainerName == KeyContainerName) && (rsa.CspKeyContainerInfo.ProviderName == ProviderName))
                                            {
                                                //we find it
                                                cert = cert2;
                                                certfound = 1;
                                                break;
                                            }
                                        }
                                    }
                                }
                                if (cert == null)
                                {
                                    //MessageBox.Show("Certificate not found");
                                    //return false;
                                }

                                if (certfound == 1)
                                {
                                    bool _resSign = SignWithThisCert(cert, DSSignName.ToString().Trim().ToUpper(), invtype.ToString().Trim(), invNo.ToString().Trim(), t_inv_no.ToString().Trim(), DScoord);
                                    if (_resSign)
                                    {
                                        dssigncount++;
                                        //dssigncount='1';
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    if (dssigncount > 0)
                        return true;
                    else
                    {
                        MessageBox.Show("Please Insert the Digital Signature Master");
                        return false;
                    }
                }
                //return true;
            }
            catch
            {

                return false;
            }
        }

        public bool CheckKeyConnected(string DS_comp_id)
        {
            try
            {
                int certfound = 0;
                int errcount = 0;

                bool keycheck = false;
                string PinCode = "";
                string KeyContainerName = "";
                string ProviderName = "";

                int rescount = 0;
                var dsDataDetail = objDSMModelData.GetDSdetailbyClientId(DS_comp_id);
                for (var i = 0; i < dsDataDetail.Count; i++)
                {
                    errcount = 0;
                    certfound = 0;

                    string DSSignName = dsDataDetail[0].KeyName.ToString().Trim();

                    IOcspClient ocspClient = new OcspClientBouncyCastle();
                    ITSAClient tsaClient = null;
                    chain.Clear();
                    pk = null;
                    IList<ICrlClient> crlList = new List<ICrlClient>();
                    crlList.Add(new CrlClientOnline(chain));
                    var dsMasterData = objDSMModelData.GetDSmasterByKeyName(DSSignName);

                    PinCode = dsMasterData.KeyPinNo;
                    KeyContainerName = dsMasterData.ContainerName;
                    ProviderName = dsMasterData.ProviderName;

                    if (PinCode != "" && KeyContainerName != "" && ProviderName != "" && PinCode.ToString().Trim().Length > 0 && KeyContainerName.ToString().Trim().Length > 0 && ProviderName.ToString().Trim().Length > 0)
                    {
                        if (PinCode != "")
                        {
                            //if pin code is set then no windows form will popup to ask it
                            SecureString pwd = GetSecurePin(PinCode);
                            CspParameters csp = new CspParameters(1,
                                                                    ProviderName,
                                                                    KeyContainerName,
                                                                    new System.Security.AccessControl.CryptoKeySecurity(),
                                                                    pwd);
                            try
                            {
                                RSACryptoServiceProvider rsaCsp = new RSACryptoServiceProvider(csp);
                                // the pin code will be cached for next access to the smart card
                            }
                            catch (Exception ex)
                            {
                                //MessageBox.Show("Crypto error: " + ex.Message);
                                //return false;
                                errcount++;
                            }
                        }
                        if (errcount == 0)

                        {
                            X509Store store = new X509Store(StoreLocation.CurrentUser);
                            store.Open(OpenFlags.ReadOnly);
                            X509Certificate2 cert = null;
                            if ((ProviderName == "") || (KeyContainerName == ""))
                            {
                                MessageBox.Show("You must set Provider Name and Key Container Name");
                                return false;
                            }
                            foreach (X509Certificate2 cert2 in store.Certificates)
                            {
                                if (cert2.HasPrivateKey)
                                {
                                    RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)cert2.PrivateKey;
                                    if (rsa == null) continue; // not smart card cert again
                                    if (rsa.CspKeyContainerInfo.HardwareDevice) // sure - smartcard
                                    {
                                        if ((rsa.CspKeyContainerInfo.KeyContainerName == KeyContainerName) && (rsa.CspKeyContainerInfo.ProviderName == ProviderName))
                                        {
                                            //we find it
                                            cert = cert2;
                                            certfound = 1;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (cert == null)
                            {
                                //MessageBox.Show("Certificate not found");
                                //return false;
                            }
                            if (certfound == 1)
                            {
                                keycheck = true;
                            }
                        }
                    }
                }
                if (keycheck)
                {
                    return true;
                }
                else
                {
                    var dsMasterData = objDSMModelData.GetAllKeysDSmaster();
                    for (int i = 0; i < dsMasterData.Count; i++)
                    {
                        errcount = 0;
                        certfound = 0;
                        string DSSignName = dsMasterData[i].KeyName;

                        PinCode = dsMasterData[i].KeyPinNo;
                        KeyContainerName = dsMasterData[i].ContainerName; ;
                        ProviderName = dsMasterData[i].ProviderName;

                        if (PinCode != "" && KeyContainerName != "" && ProviderName != "" && PinCode.ToString().Trim().Length > 0 && KeyContainerName.ToString().Trim().Length > 0 && ProviderName.ToString().Trim().Length > 0)
                        {
                            if (PinCode != "")
                            {
                                //if pin code is set then no windows form will popup to ask it
                                SecureString pwd = GetSecurePin(PinCode);
                                CspParameters csp = new CspParameters(1,
                                                                        ProviderName,
                                                                        KeyContainerName,
                                                                        new System.Security.AccessControl.CryptoKeySecurity(),
                                                                        pwd);
                                try
                                {
                                    RSACryptoServiceProvider rsaCsp = new RSACryptoServiceProvider(csp);
                                    // the pin code will be cached for next access to the smart card
                                }
                                catch (Exception ex)
                                {
                                    //MessageBox.Show("Crypto error: " + ex.Message);
                                    ////string _sQuery_Quarantine = "UPDATE INVOICE SET SYSTEM_PRINT_STATUS='FALSE QUARANTINE' WHERE SYSTEM_PRINT_STATUS='FALSE' AND H_INVOICE_NO='" + invNo.ToString().Trim() + "'";
                                    //// db_upd_tmr.Start();
                                    ////bool _bQuarantineResult = DBCL.executeNonQuery(_sQuery_Quarantine);
                                    //return false;
                                    errcount++;
                                }
                            }

                            if (errcount == 0)
                            {
                                IOcspClient ocspClient = new OcspClientBouncyCastle();
                                ITSAClient tsaClient = null;
                                //chain = "";
                                // chain.Remove();
                                chain.Clear();
                                pk = null;
                                IList<ICrlClient> crlList = new List<ICrlClient>();
                                crlList.Add(new CrlClientOnline(chain));
                                X509Store store = new X509Store(StoreLocation.CurrentUser);
                                store.Open(OpenFlags.ReadOnly);
                                X509Certificate2 cert = null;
                                if ((ProviderName == "") || (KeyContainerName == ""))
                                {
                                    MessageBox.Show("You must set Provider Name and Key Container Name");
                                    return false;
                                }
                                foreach (X509Certificate2 cert2 in store.Certificates)
                                {
                                    if (cert2.HasPrivateKey)
                                    {
                                        RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)cert2.PrivateKey;
                                        if (rsa == null) continue; // not smart card cert again
                                        if (rsa.CspKeyContainerInfo.HardwareDevice) // sure - smartcard
                                        {
                                            if ((rsa.CspKeyContainerInfo.KeyContainerName == KeyContainerName) && (rsa.CspKeyContainerInfo.ProviderName == ProviderName))
                                            {
                                                //we find it
                                                cert = cert2;
                                                certfound = 1;
                                                break;
                                            }
                                        }
                                    }
                                }
                                if (cert == null)
                                {
                                    //MessageBox.Show("Certificate not found");
                                    //return false;
                                }
                                //else
                                //{
                                //    keycheck = true;
                                //}
                                if (certfound == 1)
                                {
                                    keycheck = true;
                                }

                            }
                        }
                    }
                }
                //return true;
                return keycheck;
            }
            catch
            {

                return false;
            }
        }

        private SecureString GetSecurePin(string PinCode)
        {
            SecureString pwd = new SecureString();
            foreach (var c in PinCode.ToCharArray()) pwd.AppendChar(c);
            return pwd;
        }

        private Boolean SignWithThisCert(X509Certificate2 cert, string DSSignName, string InvoiceType, string InvoiceNo, string t_inv_no, string DScoord)
        {
            string outputFilepath;
            int nooffiles = 0;
            try
            {
                string[] srcfilePaths = Directory.GetFiles(Global.InputPath, "*.pdf");

                foreach (var src in srcfilePaths)
                {
                    // Get FileName
                    if (src.Contains(InvoiceNo))
                    {
                        Org.BouncyCastle.X509.X509CertificateParser cp = new Org.BouncyCastle.X509.X509CertificateParser();
                        Org.BouncyCastle.X509.X509Certificate[] chain = new Org.BouncyCastle.X509.X509Certificate[] { cp.ReadCertificate(cert.RawData) };
                        IExternalSignature externalSignature = new X509Certificate2Signature(cert, "SHA-1");

                        outputFilepath = Global.OutputPath + @"\" + Path.GetFileName(src);

                        string outputfileName = Global.OutputPath + @"\" + Path.GetFileName(src);
                        if (outputfileName != null || outputfileName != string.Empty)
                        {
                            if ((System.IO.File.Exists(outputfileName)))
                                System.IO.File.Delete(outputfileName);
                        }

                        FileStream os = new FileStream(outputFilepath, FileMode.Create);
                        PdfReader reader = new PdfReader(src);
                        PdfStamper stamper = PdfStamper.CreateSignature(reader, os, '\0');
                        PdfSignatureAppearance appearance = stamper.SignatureAppearance;
                        appearance.Contact = "Document Certified By P SANTHARAJ <Santharaj@hnsinfotech.com>";
                        appearance.Acro6Layers = false;
                        appearance.SignatureGraphic = iTextSharp.text.Image.GetInstance(Application.StartupPath + @"\login_top.png");
                        //appearance.SetVisibleSignature(
                        //   new iTextSharp.text.Rectangle(450, 215, 580, 265),
                        //   reader.NumberOfPages,
                        //   "Signature");

                        var template = objDSMModelData.GetTemplates(Global.CustomerId);
                        if(template.Count>0)
                        {
                            Global.Templatename = template[0].Tempname.ToString();
                        }
                        var dscoordinate = objDSMModelData.GetTemplatesCord(Global.Templatename, Global.CustomerId);
                        string coord = "";

                        for (int i = 0; i < dscoordinate.Count; i++)
                        {
                            coord = dscoordinate[i].Dscoord.ToString().Trim();
                        }

                        if (coord.Length > 0 && coord.ToString().Trim() != "")
                        {
                            string[] arrcoord1 = coord.Split(',');
                            appearance.SetVisibleSignature(
                              new iTextSharp.text.Rectangle(int.Parse(arrcoord1[0]), int.Parse(arrcoord1[1]), int.Parse(arrcoord1[2]), int.Parse(arrcoord1[3])),
                              reader.NumberOfPages,
                              "Signature");

                            MakeSignature.SignDetached(appearance, externalSignature, chain, null, null, null, 0, CryptoStandard.CMS);

                            // MakeSignature.si
                            nooffiles++;

                            string fileName = Global.InputPath + @"\" + Path.GetFileName(src);
                            if (fileName != null || fileName != string.Empty)
                            {
                                if ((System.IO.File.Exists(fileName)))
                                    System.IO.File.Delete(fileName);
                            }

                            byte[] file;
                            string OutputfileName = outputFilepath;
                            if (OutputfileName != null || OutputfileName != string.Empty)
                            {
                                if ((System.IO.File.Exists(OutputfileName)))
                                {
                                    using (var stream = new FileStream(OutputfileName, FileMode.Open, FileAccess.Read))
                                    {
                                        using (var DSreader = new BinaryReader(stream))
                                        {
                                            file = DSreader.ReadBytes((int)stream.Length);
                                        }
                                    }
                                    string pdffilename = Path.GetFileName(OutputfileName);
                                    if (pdffilename.ToString().Trim().Contains("ORIGINAL"))
                                    {
                                        Global.OriginalPDFFile = file;
                                        objDSMModelData.UpdateHXValueByInvoiceNo("ORIGINAL", file, t_inv_no.ToString().Trim());
                                    }
                                    else if (pdffilename.ToString().Trim().Contains("DUPLICATE"))
                                    {
                                        // objDSMModelData.UpdateHXValueByInvoiceNo("DUPLICATE", file, t_inv_no.ToString().Trim());
                                    }
                                    else if (pdffilename.ToString().Trim().Contains("QUADRAPLICATE") || pdffilename.ToString().Trim().Contains("TRIPLICATE"))
                                    {
                                        //objDSMModelData.UpdateHXValueByInvoiceNo("TRIPLICATE", file, t_inv_no.ToString().Trim());
                                    }
                                    else if (pdffilename.ToString().Trim().Contains("EXTRA"))
                                    {
                                        // objDSMModelData.UpdateHXValueByInvoiceNo("EXTRA", file, t_inv_no.ToString().Trim());
                                    }
                                }
                            }
                        }
                        else
                        {

                            string[] invPath = Directory.GetFiles(Global.InputPath, "*.pdf");
                            Check_coor(invPath[0], DScoord);

                            string[] arrcoord1 = coord1.Split(',');
                            appearance.SetVisibleSignature(
                              new iTextSharp.text.Rectangle(int.Parse(arrcoord1[0]), int.Parse(arrcoord1[1]), int.Parse(arrcoord1[2]), int.Parse(arrcoord1[3])),
                              reader.NumberOfPages,
                              "Signature");

                            MakeSignature.SignDetached(appearance, externalSignature, chain, null, null, null, 0, CryptoStandard.CMS);

                            // MakeSignature.si
                            nooffiles++;

                            string fileName = Global.InputPath + @"\" + Path.GetFileName(src);
                            if (fileName != null || fileName != string.Empty)
                            {
                                if ((System.IO.File.Exists(fileName)))
                                    System.IO.File.Delete(fileName);
                            }

                            byte[] file;
                            string OutputfileName = outputFilepath;
                            if (OutputfileName != null || OutputfileName != string.Empty)
                            {
                                if ((System.IO.File.Exists(OutputfileName)))
                                {
                                    using (var stream = new FileStream(OutputfileName, FileMode.Open, FileAccess.Read))
                                    {
                                        using (var DSreader = new BinaryReader(stream))
                                        {
                                            file = DSreader.ReadBytes((int)stream.Length);
                                        }
                                    }
                                    string pdffilename = Path.GetFileName(OutputfileName);
                                    if (pdffilename.ToString().Trim().Contains("ORIGINAL"))
                                    {
                                        Global.OriginalPDFFile = file;
                                        objDSMModelData.UpdateHXValueByInvoiceNo("ORIGINAL", file, t_inv_no.ToString().Trim());
                                    }
                                    else if (pdffilename.ToString().Trim().Contains("DUPLICATE"))
                                    {
                                        // objDSMModelData.UpdateHXValueByInvoiceNo("DUPLICATE", file, t_inv_no.ToString().Trim());
                                    }
                                    else if (pdffilename.ToString().Trim().Contains("QUADRAPLICATE") || pdffilename.ToString().Trim().Contains("TRIPLICATE"))
                                    {
                                        //objDSMModelData.UpdateHXValueByInvoiceNo("TRIPLICATE", file, t_inv_no.ToString().Trim());
                                    }
                                    else if (pdffilename.ToString().Trim().Contains("EXTRA"))
                                    {
                                        // objDSMModelData.UpdateHXValueByInvoiceNo("EXTRA", file, t_inv_no.ToString().Trim());
                                    }
                                }
                            }
                        }

                    }
                }
                if (nooffiles > 0)
                {
                    objPrint = new Print();
                    objDSMModelData.UpdateInvoiceDSstatusByInvoiceNo(t_inv_no.ToString().Trim());
                    //var printStatus = objPrint.PrintInvoicePDFs(Global.InvoiceNumber, "invoice");
                    //objDSMModelData.UpdateInvoicePrintstatusByInvoiceNo(t_inv_no.ToString().Trim(),printStatus == true ? "TRUE" : "FALSE");

                    //string Fordpdffilename = Global.OutputPath + @"\" + t_inv_no + "ORIGINALInvoice.pdf";
                    //if (Global.CustomerName == "FORD")
                    //{
                    //    string vendorcode = "FA9NA";
                    //    string newfilename = "E6_1_" + vendorcode.ToString().Trim() + "_" + t_inv_no.ToString().Trim() + ".pdf";
                    //    if (Fordpdffilename.ToString().Trim().Contains("ORIGINAL"))
                    //    {
                    //        if (Fordpdffilename.ToString().Trim().Contains("ORIGINAL"))
                    //        {
                    //            if ((System.IO.File.Exists(Global.OutputPath + @"\\" + newfilename.ToString().Trim())))
                    //                File.Delete(Global.OutputPath + @"\\" + newfilename.ToString().Trim());
                    //            //System.IO.File.Move(pdffilename, SalesOrderManagementSystem.Utilities._sDSOutput_File_Path + @"\\" + newfilename.ToString().Trim());
                    //            System.IO.File.Move(Global.OutputPath + @"\\" + System.IO.Path.GetFileName(Fordpdffilename.ToString().Trim()), Global.OutputPath + @"\\" + newfilename.ToString().Trim());

                    //        }

                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                LogErrors(ex);
                MessageBox.Show("Please Insert Digital Signature Token !");
                return false;
            }
            return true;
        }
        private void Check_coor(string path, string DScoord)
        {
            Spire.Pdf.PdfDocument doc = new Spire.Pdf.PdfDocument();
            doc.LoadFromFile(path);
            PdfTextFind[] results = null;
            int i = 0;

            foreach (PdfPageBase page in doc.Pages)
            {
                string[] do1 = DScoord.Split(',');
                //results = page.FindText("AUTHORISED SIGNATORY").Finds;
                for (int x = 0; x < do1.Count(); x++)
                {

                    results = page.FindText(do1[x]).Finds;
                    int e = results.Count();
                    if (e == 1)
                    {
                        results = page.FindText(do1[x]).Finds;
                        break;
                    }
                    //string txt = results.ToString();
                }
                int chk = 0;

                foreach (PdfTextFind text in results)
                {

                    i++;
                    chk++;
                    PointF p = text.Position;
                    string A = p.ToString().Trim();
                    string[] A1 = p.ToString().Split(',');
                    string pno = "Page No " + i.ToString();
                    //LogSignedDetails(Global.InvoiceNumber + pno + A.ToString());
                    string x = A1[0].ToString().Substring(3, 3);
                    string y = A1[1].ToString().Substring(3, 3);

                    double L, B, R, T, R1, T1;
                    double x1 = Convert.ToDouble(x);
                    double y1 = Convert.ToDouble(y);

                    L = x1;
                    R = x1 + 90;
                    B = 792 - y1 + 50;
                    T = B + 45;
                    string s = "" + L + "," + B + "," + R + "," + T + "";
                    coord1 = s;

                    //L = x1;
                    //R = x1 + 100;
                    //B = 792 - y1 + 30;
                    //T = B + 45;
                    //string s = "" + L + "," + B + "," + R + "," + T + "";
                    //coord1 = s;

                    //L = x1;
                    //R = Convert.ToInt32(x1 * 0.2);
                    //R1 = R + x1;
                    //B = y1;
                    //T = Convert.ToInt32(0.2* y1);
                    //T1 = y1 + T;


                    Console.WriteLine(coord1);
                }
                if (chk == 0)
                {
                    coord1 = "350,90,500,160";
                    Console.WriteLine(coord1);
                }
            }
        }

        string coord1 = "";
        //public bool SendEmail(string printInvNo)
        //{
        //    try
        //    {
        //        Utilities.LogQuery(printInvNo);
        //        //MessageBox.Show("inside Mail");
        //        //*************************************************MAIL ALERT********************************
        //        objDSMModelData = new DSMModelData();
        //        var edi = objDSMModelData.GetEdiByClientName(Global.CustomerName);

        //        if (edi.FromEmail != null)
        //        {
        //            string DSmailpdf = Global.OutputPath + @"\\" + printInvNo + "ORIGINALInvoice.pdf";
        //            string DSmailprintpdf = Global.OutputPath + @"\\PRINT\\" + printInvNo + "ORIGINALInvoice.pdf";
        //            if ((System.IO.File.Exists(DSmailpdf)) || (System.IO.File.Exists(DSmailprintpdf)))
        //            {
        //                try
        //                {
        //                    //MessageBox.Show("settings Mail");
        //                    string attachPath = string.Empty;
        //                    if ((System.IO.File.Exists(DSmailpdf)))
        //                    {
        //                        attachPath = DSmailpdf;
        //                    }
        //                    else
        //                    {
        //                        attachPath = DSmailprintpdf;
        //                    }

        //                    System.Net.Mail.MailMessage DSmessage = new System.Net.Mail.MailMessage();
        //                    System.Net.Mail.SmtpClient DSsmtp = new SmtpClient();
        //                    DSmessage.From = new MailAddress(edi.FromEmail);
        //                    Utilities.LogQuery(edi.FromEmail);

        //                    string[] arrToMail = edi.Email.Split(',');
        //                    for (int i = 0; i < arrToMail.Length; i++)
        //                    {
        //                        DSmessage.To.Add(arrToMail[i]);
        //                        Utilities.LogQuery(arrToMail[i]);
        //                    }

        //                    DSmessage.Subject = Global.CustomerName + "- DS INVOICE";
        //                    DSmessage.Body = "Kindly, Find the DS Invoice for your reference";
        //                    System.Net.Mail.Attachment DSattachment;
        //                    DSattachment = new System.Net.Mail.Attachment(attachPath);
        //                    DSmessage.Attachments.Add(DSattachment);
        //                    DSmessage.IsBodyHtml = true;
        //                    DSsmtp.Port = int.Parse(edi.Port);
        //                    DSsmtp.Host = edi.Host;
        //                    DSsmtp.EnableSsl = true;  
        //                   // DSsmtp.EnableSsl = false;
        //                    DSsmtp.UseDefaultCredentials = false;
        //                    DSsmtp.Credentials = new NetworkCredential(edi.FromEmail, edi.FromPwd);
        //                    DSsmtp.DeliveryMethod = SmtpDeliveryMethod.Network;
        //                    //MessageBox.Show("complete Mail");
        //                    DSsmtp.Send(DSmessage);

        //                    //MessageBox.Show("sent Mail");
        //                }
        //                catch (Exception ex)
        //                {
        //                    //MessageBox.Show("error Mail");
        //                    Global.LogErrors(ex);
        //                    return false;
        //                }
        //            }
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        Global.LogErrors(ex);
        //        return false;
        //    }
        //}

        public bool SendEmail(string printInvNo)
        {
            try
            {
                Utilities.LogQuery("Checking if already sent: " + printInvNo);

                objDSMModelData = new DSMModelData();

                // ✅ STEP 1: Check if already sent
                if (objDSMModelData.IsEmailAlreadySent(printInvNo))
                {
                    Utilities.LogQuery("Already sent: " + printInvNo);
                    return true;
                }

                // ✅ STEP 2: Get EDI config
                var edi = objDSMModelData.GetEdiByClientName(Global.CustomerName);

                if (edi.FromEmail != null)
                {
                    string DSmailpdf = Global.OutputPath + @"\\" + printInvNo + "ORIGINALInvoice.pdf";
                    string DSmailprintpdf = Global.OutputPath + @"\\PRINT\\" + printInvNo + "ORIGINALInvoice.pdf";
                    if ((File.Exists(DSmailpdf)) || (File.Exists(DSmailprintpdf)))
                    {
                        try
                        {
                            string attachPath = File.Exists(DSmailpdf) ? DSmailpdf : DSmailprintpdf;

                            MailMessage DSmessage = new MailMessage();
                            SmtpClient DSsmtp = new SmtpClient();

                            DSmessage.From = new MailAddress(edi.FromEmail);
                            string[] arrToMail = edi.Email.Split(',');
                            foreach (string to in arrToMail)
                                DSmessage.To.Add(to);

                            DSmessage.Subject = Global.CustomerName + "- DS INVOICE";
                            DSmessage.Body = "Kindly, Find the DS Invoice for your reference";
                            DSmessage.Attachments.Add(new Attachment(attachPath));
                            DSmessage.IsBodyHtml = true;

                            DSsmtp.Port = int.Parse(edi.Port);
                            DSsmtp.Host = edi.Host;
                            DSsmtp.EnableSsl = true;
                            DSsmtp.UseDefaultCredentials = false;
                            DSsmtp.Credentials = new NetworkCredential(edi.FromEmail, edi.FromPwd);
                            DSsmtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                            DSsmtp.Send(DSmessage);

                            // ✅ STEP 3: Mark as sent
                            if (!Global.CustomerName.ToUpper().Contains("HYUNDAI"))
                            { 
                                objDSMModelData.MarkEmailAsSent(printInvNo);
                            }
                                
                        }
                        catch (Exception ex)
                        {
                            Global.LogErrors(ex);
                            return false;
                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Global.LogErrors(ex);
                return false;
            }
        }

        public bool FordSendEmail(string FordPdf)
        {

            //*************************************************MAIL ALERT********************************
            objDSMModelData = new DSMModelData();
            var edi = objDSMModelData.GetEdiByClientName(Global.CustomerName);

            if (edi.FromEmail != null)
            {
                //string DSmailpdf = Global.OutputPath + @"\\" + FordprintInvNo + "ORIGINALINVOICE.pdf";
                //string DSmailprintpdf = Global.OutputPath + @"\\PRINT\\" + FordprintInvNo + "ORIGINALINVOICE.pdf";
                string DSFordpdf = FordPdf;
                //if ((System.IO.File.Exists(DSmailpdf)) || (System.IO.File.Exists(DSmailprintpdf)))
                if ((System.IO.File.Exists(DSFordpdf)))
                {
                    try
                    {
                        string attachPath = string.Empty;
                        if ((System.IO.File.Exists(DSFordpdf)))
                        {
                            attachPath = DSFordpdf;
                        }
                        //else
                        //{
                        //    attachPath = DSmailprintpdf;
                        //}

                        System.Net.Mail.MailMessage DSmessage = new System.Net.Mail.MailMessage();
                        System.Net.Mail.SmtpClient DSsmtp = new SmtpClient();

                        DSmessage.From = new MailAddress(edi.FromEmail);

                        string[] arrToMail = edi.Email.Split(',');
                        for (int i = 0; i < arrToMail.Length; i++)
                        {
                            DSmessage.To.Add(arrToMail[i]);
                        }

                        DSmessage.Subject = "DS INVOICE";


                        DSmessage.Body = "Kindly Find the DS Invoice for your reference";

                        System.Net.Mail.Attachment DSattachment;
                        DSattachment = new System.Net.Mail.Attachment(attachPath);

                        DSmessage.Attachments.Add(DSattachment);

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
                        return false;
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }
        public void LogErrors(Exception ex)
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
    }
}
