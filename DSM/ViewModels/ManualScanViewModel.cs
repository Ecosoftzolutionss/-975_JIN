using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using DSM;
using DSMData;
using DSMData.Model;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Prism.Commands;
using Prism.Mvvm;
using Spire.Pdf;
using Spire.Pdf.Exporting;

namespace DSM.ViewModels
{
    public class ManualScanViewModel : BindableBase
    {

        DSMModelData objDSMModelData;
        public ICommand PostInvoiceCommand { get; set; }

        private ObservableCollection<QuarantineDisplayModel> _QList = new ObservableCollection<QuarantineDisplayModel>();
        public ObservableCollection<QuarantineDisplayModel> QtList
        {
            get { return _QList; }
            set { SetProperty(ref _QList, value); }
        }
        private ObservableCollection<CustomerDisplayModel> _Customers;
        public ObservableCollection<CustomerDisplayModel> Customers
        {
            get { return _Customers; }
            set { SetProperty(ref _Customers, value); }
        }
        private QuarantineDisplayModel _SelectedInv;
        public QuarantineDisplayModel SelectedInv
        {
            get { return _SelectedInv; }
            set { SetProperty(ref _SelectedInv, value); }
        }

        bool valid = true;
        bool iskeycheck = true;

        public ManualScanViewModel()
        {
            objDSMModelData = new DSMModelData();
            SelectedInv = new QuarantineDisplayModel();
            PostInvoiceCommand = new DelegateCommand(PostInvoice);
            QtList = new ObservableCollection<QuarantineDisplayModel>(objDSMModelData.GetQuarantineList());
            AssignSettings();
        }

        public void AssignSettings()
        {
            try
            {

                SettingsDisplayModel objSettings = new SettingsDisplayModel();
                CustomerDisplayModel objCustomer = new CustomerDisplayModel();

                Global.CustomerName = "HYUNDAI";

                DigitalSign objDigitalSign = new DigitalSign();
                var keyConnected = objDigitalSign.CheckKeyConnected(Global.CustomerName);
                keyConnected = true;
                if (keyConnected == false)
                {
                    MessageBox.Show("Check whether the key is connected or not!");
                    App.Current.MainWindow.Close();
                    for (int intCounter = App.Current.Windows.Count - 1; intCounter > 0; intCounter--)
                        App.Current.Windows[intCounter].Close();

                    iskeycheck = false;
                }
                else
                {
                    objSettings = objDSMModelData.GetSettingsData(System.Environment.MachineName);
                    if (objSettings != null)
                    {
                        Global.SystemName = objSettings.SystemName;
                        Global.InputPath = objSettings.InputPath;
                        Global.ManualInvoicePath = objSettings.InvoicePath + "INVALID_INVOICE";
                        Global.InvoicePath = objSettings.InvoicePath;
                        Global.OutputPath = objSettings.OutputPath;
                    }

                    objCustomer = objDSMModelData.GetCustomerByName(Global.CustomerName);

                    if (objCustomer != null)
                    {
                        Global.VendorCode = objCustomer.VendorCode;
                        Global.CustomerId = objCustomer.CustomerId;
                        Global.PDFtype = objCustomer.PDFtype == true ? "SINGLE" : "MULTIPLE";
                        Global.PrintType = objCustomer.PrintType == true ? "ORIGINAL" : "SELECTED";
                        Global.PrintCopies = objCustomer.PrintCopies;
                        if (objCustomer.IsAPIpost) 
                        {
                            Global.IsAPIpost = true;
                            Global.APIpostType = objCustomer.APItype == true ? "AUTO" : "MANUAL";
                            Global.APIurl = objCustomer.APIUrl;
                            if (Global.APIurl == "https://hmieai.hmil.net:5004/Service.asmx?WSDL")
                            {
                                Global.APIPosturl = "http://hmieai/DI/data";
                            }
                            else
                            {
                                Global.APIPosturl = "http://hmieai/DI/data";
                            }
                        }
                        else
                        {
                            Global.IsAPIpost = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("SETTINGS ERROR: " + ex.ToString());
            }
        }

        public void PostInvoice()
        {
            try
            {
                if (string.IsNullOrEmpty(SelectedInv.ScanData))
                {
                    MessageBox.Show("Scan Invoice!");
                }
                else
                {
                    bool chkFile = false;
                    string[] invoicePathList = Directory.GetFiles(Global.ManualInvoicePath, "*.pdf");

                    for (int i = 0; i < invoicePathList.Length; i++)
                    {
                        AssignInvoiceNo_TruckNo(invoicePathList[i].ToString(), "INV");
                        //string FinYear = GetFinancialYear(Global.InvoiceDate.ToString().Trim());
                       // Global.FinYear = Convert.ToInt32(FinYear.ToString().Trim());
                        if (Global.InvoiceNumber == SelectedInv.InvNumber)
                        {
                            AssignInvoiceNo_TruckNo(invoicePathList[i].ToString(), "TRUCK");
                            Global.SelectedFile = invoicePathList[i].ToString();
                            chkFile = true;
                            break;
                        }
                    }

                    if (chkFile == false)
                    {
                        MessageBox.Show("Selected Invoice not found");
                    }
                    else
                    {
                        CreateInvoice(SelectedInv.ScanData);
                        if (valid == true && Global.APIInvoiceNumber != "Null")
                        {
                            MessageBox.Show("Success");
                            QtList = new ObservableCollection<QuarantineDisplayModel>(objDSMModelData.GetQuarantineList());
                        }
                        else
                        {
                            MessageBox.Show("Api not posted");
                            QtList = new ObservableCollection<QuarantineDisplayModel>(objDSMModelData.GetQuarantineList());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.ToString());
                valid = false;
            }
        }
        public string GetFinancialYear(string curDate)
        {

            int CurrentYear = int.Parse(curDate.Substring(8, 2));
            int PreviousYear = (int.Parse(curDate.Substring(8, 2)) - 1);
            int NextYear = (int.Parse(curDate.Substring(8, 2)) + 1);
            string PreYear = PreviousYear.ToString();
            string NexYear = NextYear.ToString();
            string CurYear = CurrentYear.ToString();
            string FinYear = null;
            string[] month = new string[4];
            int CurrentMonth = 0;
            if (curDate.Contains("/"))
            {
                month = curDate.Split('/');

                if (month[1].Length == 2)
                {
                    CurrentMonth = int.Parse(curDate.Substring(3, 2));
                }
                else if (month[1].Length == 1)
                {
                    CurrentMonth = int.Parse(curDate.Substring(3, 1));
                }
            }
            else if (curDate.Contains("-"))
            {
                month = curDate.Split('-');

                if (month[1].Length == 2)
                {
                    CurrentMonth = int.Parse(curDate.Substring(3, 2));
                }
                else if (month[1].Length == 1)
                {
                    CurrentMonth = int.Parse(curDate.Substring(3, 1));
                }
            }
            else
            {
                month = curDate.Split('.');

                if (month[1].Length == 2)
                {
                    CurrentMonth = int.Parse(curDate.Substring(3, 2));
                }
                else if (month[1].Length == 1)
                {
                    CurrentMonth = int.Parse(curDate.Substring(3, 1));
                }
            }
            //int CurrentMonth= int.Parse(curDate.Split('/').ToString());
            //if (curDate.Month > 3)
            if (CurrentMonth > 3)
            {
                FinYear = CurYear + NexYear;
            }
            else
            {
                FinYear = PreYear + CurYear;
            }
            return FinYear;
        }
        public void CreateInvoice(string decodedString)
        {
            try
            {
                bool chkInv = true;
                int count = 1;
                char[] a = { '\n', '\r' };
                string[] arr = decodedString.Split(a);
                arr = arr.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                List<InvoiceDisplayModel> lstInvoice = new List<InvoiceDisplayModel>();
                InvoiceDisplayModel objInvoice = new InvoiceDisplayModel();
                objDSMModelData = new DSMModelData();

                for (int i = 0; i < arr.Length; i++)
                {
                    if (count % 2 == 0)
                    {
                        string[] arr1 = arr[i].Split(' ');

                        string dateString = arr1[1].Substring(2, 2) + '/' + arr1[1].Substring(0, 2) + '/' + arr1[1].Substring(4, 4);
                        CultureInfo provider = CultureInfo.InvariantCulture;
                        DateTime dateInv = DateTime.ParseExact(dateString, new string[] { "MM.dd.yyyy", "MM-dd-yyyy", "MM/dd/yyyy" }, provider, DateTimeStyles.None);

                        if (arr1[0] != SelectedInv.InvNumber)
                        {
                            MessageBox.Show("Scanned Invoice not matched with selcted invoice");
                            chkInv = false;
                            break;
                        }
                        else
                        {
                            objInvoice.InvNumber = arr1[0];
                            objInvoice.InvDate = dateInv;
                            objInvoice.InvQuantity = arr1[1].Substring(8);
                            objInvoice.InvValue = arr1[2];
                            objInvoice.TarrifNumber = arr1[3].Substring(0, 10);
                            objInvoice.BedAmount = arr1[3].Substring(10);
                            objInvoice.SGST = arr1[4];
                            objInvoice.IGST = arr1[5];
                            objInvoice.VatAmount = "0";
                            objInvoice.UnitPrice = arr1[7];
                            objInvoice.MaterialCost = arr1[8];
                            objInvoice.CGST = arr1[9];
                            objInvoice.ConsigneePartCost = arr1[10];
                            objInvoice.ExciseDutyCost = arr1[11];
                            objInvoice.AssessableValue = arr1[12];
                            objInvoice.CSTAmount = arr1[13];
                            objInvoice.ToolCost = arr1[14];
                            objInvoice.ConsigneeMatlCost = arr1[15];
                            objInvoice.GSTN = arr1[16];
                            objInvoice.CreatedBy = Global.UserId;
                            objInvoice.CustomerName = Global.CustomerName;
                            objInvoice.VehicleNumber = Global.TruckNumber;
                            objInvoice.VendorCode = Global.VendorCode;
                            objInvoice.TCS = arr1[6];
                            objInvoice.E_InvNo = arr1[17];
                            objInvoice.Ewaybill_no = "0";
                            objInvoice.Lot_Code = "0";
                            objInvoice.EXTRA_NUM_1 = "0";
                            objInvoice.EXTRA_NUM_2 = "0";
                            objInvoice.EXTRA_NUM_3 = "0";
                            objInvoice.EXTRA_NUM_4 = "0";
                            objInvoice.EXTRA_NUM_5 = "0";
                            objInvoice.EXTRA_NUM_6 = "0";
                            objInvoice.EXTRA_NUM_7 = "0";
                            objInvoice.EXTRA_CHAR_1 = "0";
                            objInvoice.EXTRA_CHAR_2 = "0";
                            objInvoice.EXTRA_CHAR_3 = "0";
                            objInvoice.EXTRA_CHAR_4 = "0";
                            objInvoice.EXTRA_CHAR_5 = "0";
                            objInvoice.Date1 = dateInv;
                            objInvoice.Date2 = dateInv;
                            objInvoice.Date3 = dateInv;
                            objInvoice.Date4 = dateInv;
                            objInvoice.Date5 = dateInv;
                            objInvoice.Manfac_Date = dateInv;
                            lstInvoice.Add(objInvoice);
                        }
                    }
                    else
                    {
                        objInvoice = new InvoiceDisplayModel();
                        objInvoice.ShopCode = arr[i].Substring(0, 2);
                        objInvoice.PONumber = arr[i].Substring(2, 10);
                        objInvoice.PartNumber = arr[i].Substring(12);
                       // objInvoice.Finyear = Global.FinYear;
                    }
                    count++;
                }
                if (chkInv == true)
                {
                    objDSMModelData.SaveQuarantineInvoice(lstInvoice);
                    objDSMModelData.DeleteQuarantine(Global.InvoiceNumber);
                    PickPDF();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("INVALID DATA: " + ex.ToString());
                valid = false;
            }
        }

        private void PickPDF()
        {
            try
            {

                //timerPick.Stop();
                //RefreshLists();
                Global.InvoiceNumber = SelectedInv.InvNumber;
                Global.APIInvoiceNumber = SelectedInv.InvNumber;
                SplitAndSave(Global.SelectedFile, Global.InputPath, SelectedInv.InvNumber, "INVOICE");
                SignPDF();
            }
            catch (Exception ex)
            {
                MessageBox.Show("PICK/SPLIT PDF ERROR: " + ex.ToString());
                valid = false;
            }
        }

        private void SignPDF()
        {
            try
            {
                var lstDS = objDSMModelData.GetSelectedInvoice(SelectedInv.InvNumber).Where(x => x.DSStatus == "FALSE" && x.PrintStatus == "FALSE").FirstOrDefault();
                if (lstDS != null)
                {
                    DigitalSign objDigitalSign = new DigitalSign();
                    bool sign = objDigitalSign.FlatDigitalSignatureInitialize("Invoice", SelectedInv.InvNumber, Global.CustomerName, SelectedInv.InvNumber,Global.Dscoord);
                    if (sign)
                    {
                        PrintFiles();
                    }
                    else
                    {
                        MessageBox.Show("Invoice Saved");
                        valid = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("SIGN ERROR: " + ex.ToString());
                valid = false;
            }
        }

        private void PrintFiles()
        {
            try
            {
                int Finyear = 0;
                string prefix_invoice_no = "";
                int fin_mon = 0;
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

                Finyear = Convert.ToInt32(prefix_invoice_no.ToString().Trim());

                var lstPrint = objDSMModelData.GetSelectedInvoice(SelectedInv.InvNumber).Where(x => x.DSStatus == "TRUE" && x.PrintStatus == "FALSE").FirstOrDefault();

                if (lstPrint != null)
                {
                    Print objPrint = new Print();
                    var printStatus = objPrint.PrintInvoicePDFs(lstPrint.InvNumber, "invoice");
                    objDSMModelData.UpdateInvoicePrintstatusByInvoiceNo(lstPrint.InvNumber, printStatus == true ? "TRUE" : "FALSE");
                }
                if (Global.IsAPIpost)
                {
                    if (Global.APIpostType == "AUTO")
                    {
                        APIPost();
                    }
                    else
                    {
                        //objDSMModelData.UpdateInvoiceAPIstatusByInvoiceNo(SelectedInv.InvNumber, Global.APIpostType, "Manual Posting");
                        objDSMModelData.UpdateInvoiceAPIstatusByInvoiceNoFinYear(SelectedInv.InvNumber, Global.APIpostType, "Manual Posting", Finyear);

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("PRINT ERROR: " + ex.ToString());
                valid = false;
            }
        }

        public void APIPost()
        {
            Global.APIInvoiceNumber = SelectedInv.InvNumber;
            try
            {
                ExampleAPIProxy exProxy = new ExampleAPIProxy();
                WebService webService = new WebService(Global.APIurl, "getData");
                webService.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show("API ERROR: " + ex.ToString());
                valid = false;
            }
        }

        public int SplitAndSave(string invoicePath, string outputPath, string tdocno_from, string invtype)
        {
            string _from = tdocno_from.ToString().Trim();

            FileInfo file = new FileInfo(invoicePath);
            string name = file.Name.Substring(0, file.Name.LastIndexOf("."));

            using (iTextSharp.text.pdf.PdfReader pdfReader = new iTextSharp.text.pdf.PdfReader(invoicePath))
            {
                for (int pageNumber = 1; pageNumber <= pdfReader.NumberOfPages; pageNumber++)
                //for (int pageNumber = 1; pageNumber <= 1; pageNumber++)
                {
                    //string vinvono = Convert.ToDecimal(_from).ToString("000000");
                    string vinvono = "";

                    if (pageNumber == 1)
                        vinvono = _from.ToString().Trim() + "ORIGINAL" + invtype.ToString().Trim();
                    else if (pageNumber == 2)
                        vinvono = _from.ToString().Trim() + "DUPLICATE" + invtype.ToString().Trim();
                    else if (pageNumber == 3)
                        vinvono = _from.ToString().Trim() + "TRIPLICATE" + invtype.ToString().Trim();
                    else if (pageNumber == 4)
                        vinvono = _from.ToString().Trim() + "EXTRA" + invtype.ToString().Trim();
                    else if (pageNumber == 5)

                        vinvono = _from.ToString().Trim() + "EXTRA1" + invtype.ToString().Trim();
                    else
                        vinvono = _from.ToString().Trim() + "QUADRAPLICATE" + invtype.ToString().Trim();

                    Global.PrintCopies = "ORIGINAL-DUPLICATE-TRIPLICATE-QUADRAPLICATE-EXTRA-EXTRA1";

                    // Get CGHS No.
                    iTextSharp.text.pdf.parser.ITextExtractionStrategy strategy = new iTextSharp.text.pdf.parser.SimpleTextExtractionStrategy();
                    string currentText = iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(pdfReader, pageNumber, strategy);
                    // Get CGHS No from the pdf as per your pdf containt.
                    // string CGHSNo = currentText.Split(new string[] { "DEBIT NOTE" }, StringSplitOptions.None)[1].Split('\n')[0];


                    // Generate New pdf with CGHS No.
                    //string filename = "A"+pageNumber+".pdf";
                    iTextSharp.text.Document document = new iTextSharp.text.Document();
                    iTextSharp.text.pdf.PdfCopy pdfCopy = new iTextSharp.text.pdf.PdfCopy(document, new FileStream(outputPath + vinvono + ".pdf", FileMode.Create));

                    pdfCopy.SetPdfVersion(iTextSharp.text.pdf.PdfWriter.PDF_VERSION_1_2);

                    pdfCopy.CompressionLevel = iTextSharp.text.pdf.PdfStream.BEST_SPEED;

                    //pdfCopy.CompressionLevel = PdfStream.DEFAULT_COMPRESSION;
                    pdfCopy.SetFullCompression();

                    document.Open();


                    pdfCopy.AddPage(pdfCopy.GetImportedPage(pdfReader, pageNumber));

                    document.Close();

                    //_from++;
                }
                pdfReader.Close();
                MoveFile(invoicePath, "PROCESSED");
                return pdfReader.NumberOfPages;
            }
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

        public void AssignInvoiceNo_TruckNo(string src, string type)
        {
            string truckNumber = string.Empty;
            string pathToPdf = src;

            Customers = new ObservableCollection<CustomerDisplayModel>(objDSMModelData.GetAllCustomers());

            using (PdfReader Custreader = new PdfReader(pathToPdf))
            {

                var custparser = new PdfReaderContentParser(Custreader);

                var custstrategy = custparser.ProcessContent(1, new LocationTextExtractionStrategyWithPosition());

                var res = custstrategy.GetLocations();

                Custreader.Close();

                //var custid = Customers;
                Global.CustomerName = "";
                Global.CustomerId = "";
                Global.Templatename = "";
                int custcount = 0;
                for (int k = 0; k < Customers.Count; k++)
                {

                    var custsearchResult = res.Where(p => p.Text.ToUpper().Contains(Customers[k].CustomerName)).OrderBy(p => p.Y).Reverse().ToList();
                    //var custsearchResult = res.Where(p => p.Text.Contains("Mobis")).OrderBy(p => p.Y).Reverse().ToList();
                    if (custsearchResult.Count > 0)
                    {
                        custcount++;
                        Global.CustomerName = Customers[k].CustomerName.ToString().Trim();
                        Global.CustomerId = Customers[k].CustomerId.ToString().Trim();
                        break;
                    }
                    //else
                    //{
                    //    MoveFile(pathToPdf, "OTHER INVOICE");
                    //    othercustomer("Other Customer Invoice");
                    //    return;
                    //}

                }
                if (custcount == 0)
                {
                    MoveFile(pathToPdf, "OTHER INVOICE");

                    return;
                }
            }
            SettingsDisplayModel objSettings = new SettingsDisplayModel();
            CustomerDisplayModel objCustomer = new CustomerDisplayModel();
            objSettings = objDSMModelData.GetSettingsData(System.Environment.MachineName);
            if (objSettings != null)
            {
                Global.SystemName = objSettings.SystemName;
                Global.InputPath = objSettings.InputPath;
                Global.InvoicePath = objSettings.InvoicePath;
                Global.OutputPath = objSettings.OutputPath;
                Global.PrinterName = objSettings.PrinterName;

            }

            objCustomer = objDSMModelData.GetCustomerByName(Global.CustomerName);

            if (objCustomer != null)
            {
                Global.VendorCode = objCustomer.VendorCode;
                Global.CustomerId = objCustomer.CustomerId;
                Global.PDFtype = objCustomer.PDFtype == true ? "SINGLE" : "MULTIPLE";
                Global.PrintType = objCustomer.PrintType == true ? "ORIGINAL" : "SELECTED";
                Global.PrintCopies = objCustomer.PrintCopies;
                if (objCustomer.IsAPIpost)
                {
                    Global.IsAPIpost = true;
                    Global.APIpostType = objCustomer.APItype == true ? "AUTO" : "MANUAL";
                    Global.APIurl = objCustomer.APIUrl;
                    if (Global.APIurl == "https://hmieai.hmil.net:5004/Service.asmx?WSDL")
                    {
                        Global.APIPosturl = "http://hmieai/DI/data";
                    }
                    else
                    {
                        Global.APIPosturl = "http://hmieai/DI/data";
                    }
                }
                else
                {
                    Global.IsAPIpost = false;
                }
            }
            PdfReader reader = new PdfReader(pathToPdf);
            //if(reader.)
            if (Global.CustomerId != "" && Global.CustomerId.Length > 0)
            {
                //var dsDataDetail = objDSMModelData.GetDSdetailbyClientId("HMI1");
                var dsDataDetail = objDSMModelData.GetDSdetailbyClientId(Global.CustomerId);

                string invnocoord = "";
                string invdatecoord = "";
                string vendorcodecoord = "";
                string trucknocoord = "";
                //string clientnamecoord = "";

                //for (var i = 0; i < dsDataDetail.Count; i++)
                //{
                //    invnocoord= dsDataDetail[i].i
                //}

                //var coordinates = objDSMModelData.GetInputTemplates(Global.CustomerName);
                var template = objDSMModelData.GetTemplates(Global.CustomerId);
                Global.Templatename = template[0].Tempname.ToString();
                var coordinates = objDSMModelData.GetTemplatesCord(Global.Templatename, Global.CustomerId);
                //string coord = "";
                for (int i = 0; i < coordinates.Count; i++)
                {
                    //string DSSignName = dsMasterData[i].KeyName;
                    invnocoord = coordinates[i].Invnocoord.ToString().Trim();
                    invdatecoord = coordinates[i].Invdatecoord.ToString().Trim();
                    vendorcodecoord = coordinates[i].Vendorcoord.ToString().Trim();
                    trucknocoord = coordinates[i].Trucknocoord.ToString().Trim();
                    //clientnamecoord = coordinates[i].Clientnamecoord.ToString().Trim();
                }
                if (invnocoord.Length > 0 && invnocoord.ToString().Trim() != "")
                {
                    RenderFilter[] filters = new RenderFilter[1];
                    LocationTextExtractionStrategy regionFilter = new LocationTextExtractionStrategy();
                    string[] arrinvcoord = invnocoord.Split(',');
                    filters[0] = new RegionTextRenderFilter(new iTextSharp.text.Rectangle(int.Parse(arrinvcoord[0]), int.Parse(arrinvcoord[1]), int.Parse(arrinvcoord[2]), int.Parse(arrinvcoord[3])));
                    FilteredTextRenderListener strategy = new FilteredTextRenderListener(regionFilter, filters);

                    String invresult = PdfTextExtractor.GetTextFromPage(reader, 1, strategy);
                    string[] invno = new string[3];
                    if (invresult.Contains(':'))
                    {
                        invno = invresult.Split(':');
                        int invind = invno.GetLength(0);
                        Global.InvoiceNumber = invno[invind - 1].ToString().Trim();
                    }
                    else if (invresult.Contains('\t'))
                    {
                        invno = invresult.Split('\t');
                        int invind = invno.GetLength(0);
                        Global.InvoiceNumber = invno[invind - 1].ToString().Trim();
                    }
                    else if (invresult.Contains('-'))
                    {
                        invno = invresult.Split('-');
                        int invind = invno.GetLength(0);
                        Global.InvoiceNumber = invno[invind - 1].ToString().Trim();
                    }
                    //int invind = invno.GetLength(0);
                    //Global.InvoiceNumber = invno[invind-1].ToString().Trim();
                    else
                    {
                        invno = invresult.Split(' ');
                        int invind = invno.GetLength(0);
                        Global.InvoiceNumber = invno[invind - 1].ToString().Trim();
                    }
                }


                if (invdatecoord.Length > 0 && invdatecoord.ToString().Trim() != "")
                {
                    RenderFilter[] filters = new RenderFilter[1];
                    LocationTextExtractionStrategy regionFilter = new LocationTextExtractionStrategy();
                    string[] arrinvdatecoord = invdatecoord.Split(',');
                    filters[0] = new RegionTextRenderFilter(new iTextSharp.text.Rectangle(int.Parse(arrinvdatecoord[0]), int.Parse(arrinvdatecoord[1]), int.Parse(arrinvdatecoord[2]), int.Parse(arrinvdatecoord[3])));
                    FilteredTextRenderListener strategy = new FilteredTextRenderListener(regionFilter, filters);

                    String invdateresult = PdfTextExtractor.GetTextFromPage(reader, 1, strategy);
                    string[] invdate = new string[3];
                    if (invdateresult.Contains(':'))
                    {
                        invdate = invdateresult.Split(':');
                        int invind = invdate.GetLength(0);
                        Global.InvoiceDate = invdate[invind - 1].ToString().Trim();
                    }
                    else if (invdateresult.Contains('\t'))
                    {
                        invdate = invdateresult.Split('\t');
                        int invind = invdate.GetLength(0);
                        Global.InvoiceDate = invdate[invind - 1].ToString().Trim();
                    }
                    //else if (invdateresult.Contains('-'))
                    //{
                    //    invdate = invdateresult.Split('-');
                    //    int invind = invdate.GetLength(0);
                    //    Global.InvoiceDate = invdate[invind - 1].ToString().Trim();
                    //}
                    else
                    {
                        Global.InvoiceDate = invdateresult;
                    }
                    //Global.InvoiceDate = "11/11/2019";
                    //int invind = invdate.GetLength(0);
                    //Global.InvoiceDate = invdate[invind-1].ToString().Trim();
                }

                if (trucknocoord.Length > 0 && trucknocoord.ToString().Trim() != "")
                {
                    RenderFilter[] filters = new RenderFilter[1];
                    LocationTextExtractionStrategy regionFilter = new LocationTextExtractionStrategy();
                    string[] arrtrucknocoord = trucknocoord.Split(',');
                    filters[0] = new RegionTextRenderFilter(new iTextSharp.text.Rectangle(int.Parse(arrtrucknocoord[0]), int.Parse(arrtrucknocoord[1]), int.Parse(arrtrucknocoord[2]), int.Parse(arrtrucknocoord[3])));
                    FilteredTextRenderListener strategy = new FilteredTextRenderListener(regionFilter, filters);

                    String truckresult = PdfTextExtractor.GetTextFromPage(reader, 1, strategy);
                    string[] truck = new string[3];
                    if (truckresult.Contains(':'))
                    {
                        truck = truckresult.Split(':');
                        int invind = truck.GetLength(0);
                        Global.TruckNumber = truck[invind - 1].ToString().Trim();
                    }
                    else if (truckresult.Contains('\t'))
                    {
                        truck = truckresult.Split('\t');
                        int invind = truck.GetLength(0);
                        Global.TruckNumber = truck[invind - 1].ToString().Trim();
                    }
                    else if (truckresult.Contains('-'))
                    {
                        truck = truckresult.Split('-');
                        int invind = truck.GetLength(0);
                        Global.TruckNumber = truck[invind - 1].ToString().Trim();
                    }
                    else
                    {
                        Global.TruckNumber = truckresult;
                    }

                }

                if (vendorcodecoord.Length > 0 && vendorcodecoord.ToString().Trim() != "")
                {
                    RenderFilter[] filters = new RenderFilter[1];
                    LocationTextExtractionStrategy regionFilter = new LocationTextExtractionStrategy();
                    string[] arrvendorcodecoord = vendorcodecoord.Split(',');
                    filters[0] = new RegionTextRenderFilter(new iTextSharp.text.Rectangle(int.Parse(arrvendorcodecoord[0]), int.Parse(arrvendorcodecoord[1]), int.Parse(arrvendorcodecoord[2]), int.Parse(arrvendorcodecoord[3])));
                    FilteredTextRenderListener strategy = new FilteredTextRenderListener(regionFilter, filters);

                    String vendorresult = PdfTextExtractor.GetTextFromPage(reader, 1, strategy);
                    string[] vendor = new string[3];
                    if (vendorresult.Contains(':'))
                    {
                        vendor = vendorresult.Split(':');
                        int invind = vendor.GetLength(0);
                        Global.VendorCode = vendor[invind - 1].ToString().Trim();
                    }
                    else if (vendorresult.Contains('\t'))
                    {
                        vendor = vendorresult.Split('\t');
                        int invind = vendor.GetLength(0);
                        Global.VendorCode = vendor[invind - 1].ToString().Trim();
                    }
                    else if (vendorresult.Contains('-'))
                    {
                        vendor = vendorresult.Split('-');
                        int invind = vendor.GetLength(0);
                        Global.VendorCode = vendor[invind - 1].ToString().Trim();
                    }
                    else
                    {
                        Global.VendorCode = vendorresult;
                    }

                }
                if (invnocoord.Length > 0 && invnocoord.ToString().Trim() != "")
                {
                    RenderFilter[] filters = new RenderFilter[1];
                    LocationTextExtractionStrategy regionFilter = new LocationTextExtractionStrategy();
                    filters[0] = new RegionTextRenderFilter(new iTextSharp.text.Rectangle(160, 70, 220, 82));
                    FilteredTextRenderListener strategy = new FilteredTextRenderListener(regionFilter, filters);

                    String invresult = PdfTextExtractor.GetTextFromPage(reader, 1, strategy);
                    string[] invno = new string[3];
                    if (invresult.Contains(':'))
                    {
                        invno = invresult.Split(':');
                        int invind = invno.GetLength(0);
                        Global.Shopcode = invno[invind - 1].ToString().Trim();
                    }
                    else if (invresult.Contains('\t'))
                    {
                        invno = invresult.Split('\t');
                        int invind = invno.GetLength(0);
                        Global.Shopcode = invno[invind - 1].ToString().Trim();
                    }
                    else if (invresult.Contains('-'))
                    {
                        invno = invresult.Split('-');
                        int invind = invno.GetLength(0);
                        Global.Shopcode = invno[invind - 1].ToString().Trim();
                    }
                    //int invind = invno.GetLength(0);
                    //Global.InvoiceNumber = invno[invind-1].ToString().Trim();
                    else
                    {
                        Global.Shopcode = invresult;
                    }
                }

            }
            reader.Close();
        }
    }
}
