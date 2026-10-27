using DataMatrix.net;
using DSMData;
using DSMData.Model;
using ExcelDataReader;
using IronBarCode;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using KeepDynamic.BarcodeReader;
using Microsoft.Practices.Unity;
using OnBarcode.Barcode.BarcodeScanner;
using PdfToImage;
using PQScan.BarcodeScanner;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Spire.Barcode;
using Spire.Pdf;
using Spire.Pdf.Exporting;
using Spire.Pdf.OPC;
using System;
using ClosedXML.Excel;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;
using ThoughtWorks.QRCode.Codec;
using ThoughtWorks.QRCode.Codec.Data;
using ZXing;
using ZXing.Common;
using Tesseract;
using PdfiumViewer;
using DSM.Views;
using Microsoft.Reporting.WinForms;
using OnBarcode.Barcode;
using DocumentFormat.OpenXml.InkML;
using System.Windows.Forms;


namespace DSM.ViewModels
{
    public class DashBoardViewModel : BindableBase, INotifyPropertyChanged
    {

        //[DllImport("user32")]
        //public static extern int SetCursorPos(int x, int y);

        //[DllImport("gsdll32.dll", EntryPoint = "gsapi_new_instance")]
        //public static void ReadPDF();
        private DataSet DsReport;
        private DataSet DsReport1;
        //-----------------------------------Barcode-----------------------
        public Bitmap barcodeInBitmap;
        public Bitmap barcodeInBitmap1;

        public byte[] barcodeInBytes;

        public event PropertyChangedEventHandler PropertyChangeds;

        int mailcount = 0;
        string[] invPath = new string[1000];
        private void NotifyPropertyChanged([CallerMemberName()] string propertyName = null)
        {
            PropertyChangeds?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public DSMModelData objDSMModelData;
        public InvoiceDisplayModel objInvoice;
        public UserDisplayModel objUser;
        public List<InvoiceDisplayModel> lstInvoice;
        private IRegionManager regionManager;
        private IUnityContainer container;
        public ICommand GoToAdminCommand { get; set; }
        public ICommand ReprintCommand { get; set; }

        public ICommand SearchCommand { get; set; }

        DigitalSign objDigitalSign = new DigitalSign();
        DispatcherTimer timer = new DispatcherTimer();
        DispatcherTimer timerPick = new DispatcherTimer();
        DispatcherTimer timerSign = new DispatcherTimer();
        DispatcherTimer timerPrintPost = new DispatcherTimer();
        DispatcherTimer timerAPIPost = new DispatcherTimer();
        DispatcherTimer timerRefresh = new DispatcherTimer();
        DispatcherTimer timerMail = new DispatcherTimer();

        private ObservableCollection<InvoiceDisplayModel> _invoiceList = new ObservableCollection<InvoiceDisplayModel>();
        public ObservableCollection<InvoiceDisplayModel> InvoiceList
        {
            get { return _invoiceList; }
            set { SetProperty(ref _invoiceList, value); }
        }

        private ObservableCollection<InvoiceDisplayModel> _currentinvoiceList = new ObservableCollection<InvoiceDisplayModel>();
        public ObservableCollection<InvoiceDisplayModel> CurrentInvoiceList
        {
            get { return _currentinvoiceList; }
            set { SetProperty(ref _currentinvoiceList, value); }
            //get { return _currentinvoiceList; }
            //set { _currentinvoiceList = value; NotifyPropertyChanged("CurrentInvoiceList"); }
        }

        private string _isData;
        public string IsData
        {
            get { return _isData; }
            set { SetProperty(ref _isData, value); }
        }

        private InvoiceDisplayModel _selectedReprint;
        public InvoiceDisplayModel SelectedReprint
        {
            get { return _selectedReprint; }
            set { SetProperty(ref _selectedReprint, value); }
        }

        private ObservableCollection<QuarantineDisplayModel> _QList = new ObservableCollection<QuarantineDisplayModel>();
        public ObservableCollection<QuarantineDisplayModel> QtList
        {
            get { return _QList; }
            set { SetProperty(ref _QList, value); }
        }

        private ObservableCollection<InvoiceDisplayModel> _SignedInvoiceList = new ObservableCollection<InvoiceDisplayModel>();
        public ObservableCollection<InvoiceDisplayModel> SignedInvoiceList
        {
            get { return _SignedInvoiceList; }
            set { SetProperty(ref _SignedInvoiceList, value); }
        }

        private ObservableCollection<InvoiceDisplayModel> _PrintedInvoiceList = new ObservableCollection<InvoiceDisplayModel>();
        public ObservableCollection<InvoiceDisplayModel> PrintedInvoiceList
        {
            get { return _PrintedInvoiceList; }
            set { SetProperty(ref _PrintedInvoiceList, value); }
        }

        private ObservableCollection<InvoiceDisplayModel> _APIInvoiceList = new ObservableCollection<InvoiceDisplayModel>();
        public ObservableCollection<InvoiceDisplayModel> APIInvoiceList
        {
            get { return _APIInvoiceList; }
            set { SetProperty(ref _APIInvoiceList, value); }
        }

        private ObservableCollection<InvoiceDisplayModel> _pendingInv = new ObservableCollection<InvoiceDisplayModel>();
        public ObservableCollection<InvoiceDisplayModel> PendingInvList
        {
            get { return _pendingInv; }
            set { SetProperty(ref _pendingInv, value); }
        }

        private string _currentInvoice;
        public string CurrentInvoice
        {
            get { return _currentInvoice; }
            set { _currentInvoice = value; NotifyPropertyChanged("CurrentInvoice"); }
        }

        private bool _splitStatus;
        public bool SplitStatus
        {
            get { return _splitStatus; }
            set { SetProperty(ref _splitStatus, value); }
        }

        private string _DataMsg;
        public string DataMsg
        {
            get { return _DataMsg; }
            set { SetProperty(ref _DataMsg, value); }
        }

        private string _ProcessMsg;
        public string ProcessMsg
        {
            get { return _ProcessMsg; }
            set { SetProperty(ref _ProcessMsg, value); }
        }

        private double _ScreenHeight;
        public double ScreenHeight
        {
            get { return _ScreenHeight; }
            set { SetProperty(ref _ScreenHeight, value); }
        }

        private double _ScreenWidth;
        public double ScreenWidth
        {
            get { return _ScreenWidth; }
            set { SetProperty(ref _ScreenWidth, value); }
        }

        private double _DashboardWidth;
        public double DashboardWidth
        {
            get { return _DashboardWidth; }
            set { SetProperty(ref _DashboardWidth, value); }
        }

        private double _DashboardHeight;
        public double DashboardHeight
        {
            get { return _DashboardHeight; }
            set { SetProperty(ref _DashboardHeight, value); }
        }

        private double _CurDashboardWidth;
        public double CurDashboardWidth
        {
            get { return _CurDashboardWidth; }
            set { SetProperty(ref _CurDashboardWidth, value); }
        }

        private double _CurDashboardHeight;
        public double CurDashboardHeight
        {
            get { return _CurDashboardHeight; }
            set { SetProperty(ref _CurDashboardHeight, value); }
        }

        private ObservableCollection<CustomerDisplayModel> _Customers;
        public ObservableCollection<CustomerDisplayModel> Customers
        {
            get { return _Customers; }
            set { SetProperty(ref _Customers, value); }
        }

        private double _QDashboardHeight;
        public double QDashboardHeight
        {
            get { return _QDashboardHeight; }
            set { SetProperty(ref _QDashboardHeight, value); }
        }

        private string _CurrentInvDetEnable;
        public string CurrentInvDetEnable
        {
            get { return _CurrentInvDetEnable; }
            set { SetProperty(ref _CurrentInvDetEnable, value); }
        }

        private string _HappyImgVisible;
        public string HappyImgVisible
        {
            get { return _HappyImgVisible; }
            set { SetProperty(ref _HappyImgVisible, value); }
        }

        private string _SadImgVisible;
        public string SadImgVisible
        {
            get { return _SadImgVisible; }
            set { SetProperty(ref _SadImgVisible, value); }
        }

        private string _SearchInvoiceNo;
        public string SearchInvoiceNo
        {
            get { return _SearchInvoiceNo; }
            set { SetProperty(ref _SearchInvoiceNo, value); }
        }

        string QRCodeString = "";

        bool isAssignedcoord = true;
        bool iskeycheck = true;
        bool isserverconnected = true;
        public DashBoardViewModel(IRegionManager regionManager, IUnityContainer uContainer)
        {
            //ProcessMsg = "Scanning under progress...";
            this.regionManager = regionManager;
            this.container = uContainer;

            ScreenWidth = SystemParameters.PrimaryScreenWidth;
            ScreenHeight = SystemParameters.PrimaryScreenHeight;
            DashboardWidth = SystemParameters.PrimaryScreenWidth - 200;

            DashboardHeight = SystemParameters.PrimaryScreenHeight - 470;


            CurDashboardHeight = (SystemParameters.PrimaryScreenHeight - 900);
            CurDashboardWidth = SystemParameters.PrimaryScreenWidth - 200;

            QDashboardHeight = SystemParameters.PrimaryScreenHeight - 300;
            GoToAdminCommand = new DelegateCommand(GoToAdmin);

            objDSMModelData = new DSMModelData();
            objInvoice = new InvoiceDisplayModel();
            lstInvoice = new List<InvoiceDisplayModel>();

            SelectedReprint = new InvoiceDisplayModel();

            CurrentInvoice = "";

            SearchCommand = new DelegateCommand(SearchInvNo);

            objUser = new UserDisplayModel();


            CheckApiExeRunOrNot();

            if (Global.UserType == "User")
            {
                //EthernetConnectivityCheck();
                AssignSettings();

                if (iskeycheck && isserverconnected && isAssignedcoord)
                {
                    RefreshLists();
                    CheckApiExeRunOrNot();
                    CurrentInvDetEnable = "Visible";

                    timer = new DispatcherTimer();
                    //timer.Interval = new TimeSpan(0, 0, int.Parse(ConfigurationManager.AppSettings["timerSeconds"].ToString()));
                    timer.Interval = TimeSpan.FromSeconds(5);
                    timer.Tick += new EventHandler(StartProcess);
                    timer.Start();

                    ReprintCommand = new DelegateCommand(Reprint);
                    //EthernetConnectivityCheck();
                    objDSMModelData.UpdateUserEntryDetails(Global.UserName.ToString().Trim());
                    System.Windows.Application.Current.MainWindow.Closing += new CancelEventHandler(MainWindow_Closing);
                }
                else
                {
                    //EthernetConnectivityCheck();
                    objDSMModelData.UpdateUserExitDetails(Global.UserName.ToString().Trim());
                }
            }
            else if (Global.UserType == "Api")
            {
                //EthernetConnectivityCheck();
                AssignServerSettings();
                //RefreshLists();
                RefreshServerLists();
                //CurrentRefreshLists();
                //dgCurrentInv.
                CurrentInvDetEnable = "Hidden";
                HappyImgVisible = "Hidden";
                SadImgVisible = "Hidden";
                timerAPIPost = new DispatcherTimer();
                timerAPIPost.Interval = TimeSpan.FromSeconds(5);
                timerAPIPost.Tick += new EventHandler(APIPostFiles);
                timerAPIPost.Start();
                //EthernetConnectivityCheck();
                objDSMModelData.UpdateUserEntryDetails(Global.UserName.ToString().Trim());
                System.Windows.Application.Current.MainWindow.Closing += new CancelEventHandler(MainWindow_Closing);
            }
        }

        public void Closing(CancelEventArgs e)
        {
            e.Cancel = true;
        }

        public void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            objDSMModelData.UpdateUserExitDetails(Global.UserName.ToString().Trim());
            foreach (Process proc in Process.GetProcessesByName("DSM"))
            {
                proc.Kill();
            }
        }

        public void Reprint()
        {

            CustomerDisplayModel objCustomer = new CustomerDisplayModel();


            if (Global.UserType == "User")
            {
                timer.Stop();

                string PrintInvNo = SelectedReprint.InvNumber;
                objCustomer = objDSMModelData.GetCustomerByInvNumber(PrintInvNo);

                Global.CustomerName = objCustomer.CustomerName;
                objCustomer = objDSMModelData.GetCustomerByName(Global.CustomerName);
                Global.PrintCopies = objCustomer.PrintCopies;
                Print objPrint = new Print();
                //var printStatus = objPrint.PrintInvoicePDFs(Global.InvoiceNumber, "invoice");
                var printStatus = objPrint.ReprintInvoicePDFs(PrintInvNo, "invoice");
                timer.Start();
            }
            else if (Global.UserType == "Api")
            {
                timerAPIPost.Stop();

                string PrintInvNo = SelectedReprint.InvNumber;

                Print objPrint = new Print();
                //var printStatus = objPrint.PrintInvoicePDFs(Global.InvoiceNumber, "invoice");
                var printStatus = objPrint.ReprintInvoicePDFs(PrintInvNo, "invoice");
                timerAPIPost.Start();
            }
        }

        private void SearchInvNo()
        {

            if (Global.UserType == "User")
            {
                timer.Stop();
                RefreshSearchLists();
                timer.Start();
            }
            else if (Global.UserType == "Api")
            {
                timerAPIPost.Stop();
                RefreshSearchLists();
                timerAPIPost.Start();
            }
        }

        public void GoToAdmin()
        {
            //App.Current.MainWindow.Close();
        }

        public void CheckApiExeRunOrNot()
        {
            var lstusers = objDSMModelData.GetApiUserType("Api");
            for (int k = 0; k < lstusers.Count; k++)
            {
                if (lstusers[k].LoginStatus == "TRUE")
                {
                    HappyImgVisible = "Visible";
                    SadImgVisible = "Hidden";
                    isserverconnected = true;
                    mailcount = 0;
                }
                else
                {
                    HappyImgVisible = "Hidden";
                    SadImgVisible = "Visible";

                    //Mail alert
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

                    int lstApiInv = objDSMModelData.GetAllAPIInvoiceStatusAll(Finyear).Count();
                    if (lstApiInv >= 1)
                    {
                        if (mailcount == 0)
                        {
                            Global.MailAlert("API Application was not running. Please connect the Server");
                            mailcount++;
                        }
                    }
                }
            }

        }

        public void AssignSettings()
        {
            try
            {
                SplitStatus = true;
                SettingsDisplayModel objSettings = new SettingsDisplayModel();
                CustomerDisplayModel objCustomer = new CustomerDisplayModel();

                Global.CustomerName = "HYUNDAI";

                objDigitalSign = new DigitalSign();
                var keyConnected = objDigitalSign.CheckKeyConnected(Global.CustomerName);
                keyConnected = true;
                if (keyConnected == false)
                {
                    System.Windows.MessageBox.Show("Check whether the key is connected or not!");
                    App.Current.MainWindow.Close();
                    for (int intCounter = App.Current.Windows.Count - 1; intCounter > 0; intCounter--)
                        App.Current.Windows[intCounter].Close();
                    //App.Current.Windows[1].Close();
                    iskeycheck = false;
                }

                else
                {
                    //EthernetConnectivityCheck();
                    if (isAssignedcoord && iskeycheck)
                    {
                        objSettings = objDSMModelData.GetSettingsData(System.Environment.MachineName);
                        if (objSettings != null)
                        {
                            Global.SystemName = objSettings.SystemName;
                            Global.InputPath = objSettings.InputPath;
                            Global.InvoicePath = objSettings.InvoicePath;
                            Global.OutputPath = objSettings.OutputPath;
                            Global.PrinterName = objSettings.PrinterName;
                            Global.Dscoord = objSettings.FlatFile.ToString();

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
            }
            catch (Exception ex)
            {
                LogErrors(ex);
            }
        }

        public void AssignServerSettings()
        {
            try
            {
                //SplitStatus = true;
                SettingsDisplayModel objSettings = new SettingsDisplayModel();
                CustomerDisplayModel objCustomer = new CustomerDisplayModel();

                Global.CustomerName = "HYUNDAI";
                //EthernetConnectivityCheck();
                objSettings = objDSMModelData.GetSettingsData(System.Environment.MachineName);
                if (objSettings != null)
                {
                    Global.SystemName = objSettings.SystemName;
                    Global.InputPath = objSettings.InputPath;
                    Global.InvoicePath = objSettings.InvoicePath;
                    Global.OutputPath = objSettings.OutputPath;
                    Global.PrinterName = objSettings.PrinterName;

                }
                //EthernetConnectivityCheck();
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
            catch (Exception ex)
            {
                LogErrors(ex);
            }
        }

        private void RefreshList(object sender, EventArgs e)
        {
            timerRefresh.Stop();
            //EthernetConnectivityCheck();
            //var allInvoiceList = objDSMModelData.GetAllInvoiceStatus();
            var allInvoiceList = objDSMModelData.GetAllSYSInvoiceStatus();

            for (int i = 0; i < allInvoiceList.Count; i++)
            {
                if (allInvoiceList[i].DSStatus == "TRUE")
                    allInvoiceList[i].DSStatus = "SIGNED";
                else
                    allInvoiceList[i].DSStatus = "PENDING";

                if (allInvoiceList[i].PrintStatus == "TRUE")
                    allInvoiceList[i].PrintStatus = "PRINTED";
                else
                    allInvoiceList[i].PrintStatus = "PENDING";

                if (allInvoiceList[i].ApiStatus == "FALSE")
                    allInvoiceList[i].ApiStatus = "PENDING";
            }
            if (allInvoiceList.Count() > 0)
            {
                InvoiceList = new ObservableCollection<InvoiceDisplayModel>(allInvoiceList);
            }
            timerRefresh.Start();
        }

        private void RefreshLists()
        {
            //EthernetConnectivityCheck();
            SearchInvoiceNo = "";
            //var allInvoiceList = objDSMModelData.GetAllInvoiceStatus();
            var allInvoiceList = objDSMModelData.GetAllSYSInvoiceStatus();
            QtList = new ObservableCollection<QuarantineDisplayModel>(objDSMModelData.GetQuarantineList());
            for (int i = 0; i < allInvoiceList.Count; i++)
            {
                if (allInvoiceList[i].DSStatus == "TRUE")
                    allInvoiceList[i].DSStatus = "SIGNED";
                else
                    allInvoiceList[i].DSStatus = "PENDING";

                if (allInvoiceList[i].PrintStatus == "TRUE")
                    allInvoiceList[i].PrintStatus = "PRINTED";
                else
                    allInvoiceList[i].PrintStatus = "PENDING";

                if (allInvoiceList[i].ApiStatus == "FALSE")
                    allInvoiceList[i].ApiStatus = "PENDING";


                if (allInvoiceList[i].ApiMsg == null)
                    allInvoiceList[i].ApiMsg = allInvoiceList[i].CustomerName + "- Invoice";
            }
            if (allInvoiceList.Count() > 0)
            {
                InvoiceList = new ObservableCollection<InvoiceDisplayModel>(allInvoiceList);
                ProcessMsg = "";
                DataMsg = "";
            }
            else
            {
                InvoiceList = new ObservableCollection<InvoiceDisplayModel>(allInvoiceList);
                DataMsg = "No Records Found";
                //ProcessMsg = "Scanning under progress...";
            }
        }

        private void RefreshServerLists()
        {
            //EthernetConnectivityCheck();
            var allInvoiceList = objDSMModelData.GetAllInvoiceStatus();
            //var allInvoiceList = objDSMModelData.GetAllSYSInvoiceStatus();
            QtList = new ObservableCollection<QuarantineDisplayModel>(objDSMModelData.GetQuarantineList());
            for (int i = 0; i < allInvoiceList.Count; i++)
            {
                if (allInvoiceList[i].DSStatus == "TRUE")
                    allInvoiceList[i].DSStatus = "SIGNED";
                else
                    allInvoiceList[i].DSStatus = "PENDING";

                if (allInvoiceList[i].PrintStatus == "TRUE")
                    allInvoiceList[i].PrintStatus = "PRINTED";
                else
                    allInvoiceList[i].PrintStatus = "PENDING";

                if (allInvoiceList[i].ApiStatus == "FALSE")
                    allInvoiceList[i].ApiStatus = "PENDING";

            }
            if (allInvoiceList.Count() > 0)
            {
                InvoiceList = new ObservableCollection<InvoiceDisplayModel>(allInvoiceList);
                ProcessMsg = "";
            }
            else
            {
                InvoiceList = new ObservableCollection<InvoiceDisplayModel>(allInvoiceList);
                DataMsg = "No Records Found";
                //ProcessMsg = "Scanning under progress...";
            }
        }

        private void RefreshSearchLists()
        {
            //EthernetConnectivityCheck();
            //var allInvoiceList = objDSMModelData.GetAllInvoiceStatus();
            //string sinvoiceNo = "52173";
            string sinvoiceNo = SearchInvoiceNo;
            //InvoiceList = new ObservableCollection<InvoiceReportModel>(objDSMModelData.FilterInvoiceData(InvNumber == null ? "" : InvNumber, FromDate.ToString(), ToDate.ToString()));
            var allInvoiceList = objDSMModelData.GetAllSearchInvoiceStatus(sinvoiceNo);
            //QtList = new ObservableCollection<QuarantineDisplayModel>(objDSMModelData.GetQuarantineList());
            for (int i = 0; i < allInvoiceList.Count; i++)
            {
                if (allInvoiceList[i].DSStatus == "TRUE")
                    allInvoiceList[i].DSStatus = "SIGNED";
                else
                    allInvoiceList[i].DSStatus = "PENDING";

                if (allInvoiceList[i].PrintStatus == "TRUE")
                    allInvoiceList[i].PrintStatus = "PRINTED";
                else
                    allInvoiceList[i].PrintStatus = "PENDING";

                if (allInvoiceList[i].ApiStatus == "FALSE")
                    allInvoiceList[i].ApiStatus = "PENDING";
            }
            if (allInvoiceList.Count() > 0)
            {
                InvoiceList = new ObservableCollection<InvoiceDisplayModel>(allInvoiceList);
                ProcessMsg = "";
            }
            else
            {
                InvoiceList = new ObservableCollection<InvoiceDisplayModel>(allInvoiceList);
                DataMsg = "No Records Found";
                //ProcessMsg = "Scanning under progress...";
            }
        }

        private void CurrentRefreshLists()
        {

            List<InvoiceDisplayModel> CurInvList = new List<InvoiceDisplayModel>()
            {
                new InvoiceDisplayModel(){
                    CustomerName = "",
                    InvNumber ="",
                    PartNumber ="",
                    DSStatus = "",
                    PrintStatus =""

                }

            };


            var allInvoiceList = CurInvList;

            if (allInvoiceList.Count() > 0)
            {
                CurrentInvoiceList = new ObservableCollection<InvoiceDisplayModel>(allInvoiceList);
                ProcessMsg = "";
            }
            else
            {
                CurrentInvoiceList = new ObservableCollection<InvoiceDisplayModel>(allInvoiceList);
                //ProcessMsg = "Scanning under progress...";
            }
            //timer.Start();

        }

        private void CurrentRefreshInvoiceLists()
        {
            //timer.Stop();
            DateTime invdate = DateTime.Now;
            //timer.Stop();
            if (Global.InvoiceDate.Contains("."))
            {
                invdate = DateTime.ParseExact(Global.InvoiceDate.ToString().Replace(".", "/").Trim(), "d/M/yyyy", CultureInfo.InvariantCulture);
            }
            else if (Global.InvoiceDate.Contains("-"))
            {
                invdate = DateTime.ParseExact(Global.InvoiceDate.ToString().Replace("-", "/").Trim(), "d/M/yyyy", CultureInfo.InvariantCulture);
            }
            //DateTime invdate = DateTime.ParseExact(Global.InvoiceDate.ToString().Replace(".", "/").Trim(), "d/M/yyyy", CultureInfo.InvariantCulture);
            string invoicedate = invdate.ToString().Trim();
            List<InvoiceDisplayModel> CurInvList = new List<InvoiceDisplayModel>()
            {
                new InvoiceDisplayModel(){
                    CustomerName = Global.CustomerName,
                    InvNumber = Global.InvoiceNumber,
                    PartNumber = invoicedate,
                    DSStatus = "PENDING",
                    PrintStatus ="PENDING"

                }

            };


            var allInvoiceList = CurInvList;

            if (allInvoiceList.Count() > 0)
            {
                CurrentInvoiceList = new ObservableCollection<InvoiceDisplayModel>(allInvoiceList);
                //ProcessMsg = "";
                ProcessMsg = "Scanning under progress...";
            }
            else
            {
                CurrentInvoiceList = new ObservableCollection<InvoiceDisplayModel>(allInvoiceList);
                ProcessMsg = "";
            }
            //timer.Start();
        }

        private void CurrentRefreshAlreadyInvoiceLists()
        {
            //timer.Stop();

            DateTime invdate = DateTime.ParseExact(Global.InvoiceDate.ToString().Replace(".", "/").Trim(), "d/M/yyyy", CultureInfo.InvariantCulture);
            string invoicedate = invdate.ToString().Trim();
            List<InvoiceDisplayModel> CurInvList = new List<InvoiceDisplayModel>()
            {
                new InvoiceDisplayModel(){
                    CustomerName = Global.CustomerName,
                    InvNumber = Global.InvoiceNumber,
                    PartNumber = invoicedate,
                    DSStatus = "Already Exist",
                    PrintStatus ="Already Exist"

                }

            };


            var allInvoiceList = CurInvList;

            if (allInvoiceList.Count() > 0)
            {
                CurrentInvoiceList = new ObservableCollection<InvoiceDisplayModel>(allInvoiceList);
                //ProcessMsg = "";
                ProcessMsg = "Scanning under progress...";
            }
            else
            {
                CurrentInvoiceList = new ObservableCollection<InvoiceDisplayModel>(allInvoiceList);
                ProcessMsg = "";
            }
            //timer.Start();
        }

        private void CurrentRefreshSignedLists()
        {
            //timer.Stop();
            DateTime invdate = DateTime.Now;
            //timer.Stop();
            if (!string.IsNullOrEmpty(Global.InvoiceDate))
            {
                if (Global.InvoiceDate.Contains("."))
                {
                    invdate = DateTime.ParseExact(Global.InvoiceDate.ToString().Replace(".", "/").Trim(), "d/M/yyyy", CultureInfo.InvariantCulture);
                }
                else if (Global.InvoiceDate.Contains("-"))
                {
                    invdate = DateTime.ParseExact(Global.InvoiceDate.ToString().Replace("-", "/").Trim(), "d/M/yyyy", CultureInfo.InvariantCulture);
                }
            }
            //DateTime invdate = DateTime.ParseExact(Global.InvoiceDate.ToString().Replace(".", "/").Trim(), "d/M/yyyy", CultureInfo.InvariantCulture);
            string invoicedate = invdate.ToString().Trim();
            List<InvoiceDisplayModel> CurInvList = new List<InvoiceDisplayModel>()
                {
                    new InvoiceDisplayModel(){
                        CustomerName = Global.CustomerName,
                        InvNumber = Global.InvoiceNumber,
                        PartNumber = invoicedate,
                        DSStatus = "SIGNED",
                        PrintStatus ="PENDING"
                        }

                };


            var allInvoiceList = CurInvList;

            if (allInvoiceList.Count() > 0)
            {
                CurrentInvoiceList = new ObservableCollection<InvoiceDisplayModel>(allInvoiceList);
                //ProcessMsg = "";
                ProcessMsg = "Scanning under progress...";
            }
            else
            {
                CurrentInvoiceList = new ObservableCollection<InvoiceDisplayModel>(allInvoiceList);
                ProcessMsg = "";
            }

            //timer.Start();
        }

        private void CurrentRefreshPrintedLists()
        {

            //timer.Stop()
            DateTime invdate = DateTime.Now;
            //timer.Stop();
            if (Global.InvoiceDate.Contains("."))
            {
                invdate = DateTime.ParseExact(Global.InvoiceDate.ToString().Replace(".", "/").Trim(), "d/M/yyyy", CultureInfo.InvariantCulture);
            }
            else if (Global.InvoiceDate.Contains("-"))
            {
                invdate = DateTime.ParseExact(Global.InvoiceDate.ToString().Replace("-", "/").Trim(), "d/M/yyyy", CultureInfo.InvariantCulture);
            }
            //DateTime invdate = DateTime.ParseExact(Global.InvoiceDate.ToString().Replace(".", "/").Trim(), "d/M/yyyy", CultureInfo.InvariantCulture);
            string invoicedate = invdate.ToString().Trim();
            //EthernetConnectivityCheck();
            List<InvoiceDisplayModel> CurInvList = new List<InvoiceDisplayModel>()
            {
                new InvoiceDisplayModel(){
                    CustomerName = Global.CustomerName,
                    InvNumber = Global.InvoiceNumber,
                    PartNumber = invoicedate,
                    DSStatus = "SIGNED",
                    PrintStatus ="PRINTED"
                    }

            };


            var allInvoiceList = CurInvList;

            if (allInvoiceList.Count() > 0)
            {
                CurrentInvoiceList = new ObservableCollection<InvoiceDisplayModel>(allInvoiceList);
                //ProcessMsg = "";
                ProcessMsg = "Scanning under progress...";
            }
            else
            {
                CurrentInvoiceList = new ObservableCollection<InvoiceDisplayModel>(allInvoiceList);
                ProcessMsg = "";
            }
            //timer.Start();
        }

        public void StartProcess(object sender, EventArgs e)
        {
            timer.Stop();

            CurrentRefreshLists();
            RefreshLists();
            CheckApiExeRunOrNot();
            //Template("JG2026270006", 1, "33AAHCH4782N1Z3");
            ReadPDF1();
            SignPDF(Global.Dscoord);
            RefreshLists();
            PrintFiles();
            RefreshLists();
            APIPostFiles();
            CurrentRefreshLists();
            RefreshLists();
            timer.Start();
        }

        private void APIPostFiles(object sender, EventArgs e)
        {
            try
            {
                timer.Stop();
                RefreshServerLists();
                DSMModelData objDSMModelData = new DSMModelData();

                ReprintCommand = new DelegateCommand(Reprint);

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

                Global.APIFinYear = Convert.ToInt32(prefix_invoice_no.ToString().Trim());

                var lstApiInv = objDSMModelData.GetAllAPIInvoiceStatusAll(Finyear).ToList();

                //for (var i = 0; i < lstPrint.Count; i++)
                //{
                for (int i = 0; i < lstApiInv.Count; i++)

                {
                    if (lstApiInv != null)
                    {

                        if (Global.IsAPIpost)
                        {
                            if (Global.APIpostType == "AUTO")
                            {
                                APIPost(lstApiInv[i].InvNumber.ToString().Trim());
                            }
                            else
                            {
                                //objDSMModelData.UpdateInvoiceAPIstatusByInvoiceNo(lstApiInv[i].InvNumber.ToString().Trim(), Global.APIpostType, "Manual Posting");
                                objDSMModelData.UpdateInvoiceAPIstatusByInvoiceNoFinYear(lstApiInv[i].InvNumber.ToString().Trim(), Global.APIpostType, "Manual Posting", Finyear);
                            }
                        }
                        RefreshServerLists();
                    }
                }

            }
            catch (Exception ex)
            {
                LogErrors(ex);
                timer.Start();
                //timerAPIPost.Start();
            }
        }

        private void APIPostFiles()
        {
            try
            {
                DSMModelData objDSMModelData = new DSMModelData();

                ReprintCommand = new DelegateCommand(Reprint);

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

                Global.APIFinYear = Convert.ToInt32(prefix_invoice_no.ToString().Trim());

                var lstApiInv = objDSMModelData.GetAllAPIInvoiceStatusAll(Finyear).ToList();

                //for (var i = 0; i < lstPrint.Count; i++)
                //{
                for (int i = 0; i < lstApiInv.Count; i++)

                {
                    if (lstApiInv != null)
                    {

                        if (Global.IsAPIpost)
                        {
                            if (Global.APIpostType == "AUTO")
                            {
                                APIPost(lstApiInv[i].InvNumber.ToString().Trim());
                            }
                            else
                            {
                                //objDSMModelData.UpdateInvoiceAPIstatusByInvoiceNo(lstApiInv[i].InvNumber.ToString().Trim(), Global.APIpostType, "Manual Posting");
                                objDSMModelData.UpdateInvoiceAPIstatusByInvoiceNoFinYear(lstApiInv[i].InvNumber.ToString().Trim(), Global.APIpostType, "Manual Posting", Finyear);
                            }
                        }
                        RefreshServerLists();
                    }
                }
                CurrentRefreshSignedLists();
            }
            catch (Exception ex)
            {
                LogErrors(ex);
                timer.Start();
            }
        }

        public void APIPost(string InvNumber)
        {
            //EthernetConnectivityCheck();
            Global.APIInvoiceNumber = InvNumber;
            CurrentInvoice = "Invoice - " + Global.APIInvoiceNumber + " is Posting...";
            try
            {
                ExampleAPIProxy exProxy = new ExampleAPIProxy();
                WebService webService = new WebService(Global.APIurl, "getData");
                webService.Invoke();
            }
            catch (Exception ex)
            {
                LogErrors(ex);
                timer.Start();
                //timerAPIPost.Start();
            }
        }

        public bool oth_cus = false;

        //private void ReadPDF1()
        //{
        //    try
        //    {
        //        Global.InvoiceDate = null;
        //        oth_cus = false;
        //        //string[] invPath = Directory.GetFiles(Global.InvoicePath, "*.pdf");

        //        string[] invPath = Directory.GetFiles(Global.InvoicePath, "*.xlsx")
        //                        .Concat(Directory.GetFiles(Global.InvoicePath, "*.xls")).ToArray();
        //        if (invPath.Length > 0)
        //        {
        //            string fileName = invPath[0];

        //            AssignInvoiceNo_TruckNo(invPath[0], "INV");

        //            if (!string.IsNullOrEmpty(Global.InvoiceDate))
        //            {
        //                string FinYear = GetFinancialYear(Global.InvoiceDate.ToString().Trim());
        //                Global.FinYear = Convert.ToInt32(FinYear.ToString().Trim());

        //                CurrentRefreshInvoiceLists();

        //                objDSMModelData = new DSMModelData();
        //                int inv_count_rec = objDSMModelData.GetInvoiceDetailsByInvoiceNumberFinYear(Global.InvoiceNumber, Convert.ToInt32(FinYear)).Count;
        //                if (inv_count_rec == 0)
        //                {
        //                    if (Global.CustomerName == "HYUNDAI")
        //                    {
        //                        PDFConvert converter = new PDFConvert();
        //                        converter.RenderingThreads = -1;
        //                        converter.TextAlphaBit = -1;
        //                        converter.TextAlphaBit = -1;
        //                        converter.OutputToMultipleFile = true;
        //                        converter.FirstPageToConvert = -1;
        //                        converter.LastPageToConvert = -1;
        //                        converter.FitPage = false;
        //                        converter.JPEGQuality = 10;
        //                        converter.OutputFormat = "png256";
        //                        converter.ResolutionX = 500;
        //                        converter.ResolutionY = 500;
        //                        System.IO.FileInfo input = new FileInfo(fileName);
        //                        string directoryPath = System.IO.Path.GetDirectoryName(fileName) + "\\" + System.IO.Path.GetFileNameWithoutExtension(input.Name);
        //                        if (!Directory.Exists(directoryPath))
        //                            Directory.CreateDirectory(directoryPath);
        //                        Spire.Pdf.PdfDocument doc = new Spire.Pdf.PdfDocument();
        //                        doc.LoadFromFile(invPath[0]);
        //                        Spire.Pdf.PdfPageBase page = doc.Pages[0];
        //                        System.Drawing.Image[] images = page.ExtractImages();
        //                        string output = string.Format("{0}\\{1}{2}", directoryPath, System.IO.Path.GetFileNameWithoutExtension(input.Name), ".png");
        //                        converter.Convert(input.FullName, output);

        //                        int count = 0;

        //                        foreach (System.Drawing.Image image in images)
        //                        {
        //                            count++;
        //                            try
        //                            {
        //                                try
        //                                {
        //                                    QRCodeString = ScanQRPdf(image, output, directoryPath, invPath[0]);

        //                                    CreateInvoiceChk(QRCodeString.ToString());

        //                                    if (Global.InvoiceNumberchk != null)
        //                                    {
        //                                        if (Global.InvoiceNumberchk.Length <= 13)
        //                                        {
        //                                            CreateInvoice(QRCodeString.ToString());
        //                                            Global.InvoiceNumberchk = null;
        //                                            Directory.Delete(directoryPath, true);

        //                                            if (!string.IsNullOrEmpty(QRCodeString))
        //                                            {
        //                                                SplitAndSave(invPath[0], Global.InputPath, Global.InvoiceNumber, "INVOICE");
        //                                                if (Directory.Exists(directoryPath))
        //                                                    Directory.Delete(directoryPath, true);
        //                                            }
        //                                            else
        //                                            {
        //                                                MoveFile(invPath[0], "INVALID_INVOICE");
        //                                                objDSMModelData = new DSMModelData();

        //                                                if (Global.InvoiceNumber != null)
        //                                                {
        //                                                    objDSMModelData.SaveQuarantine(Global.InvoiceNumber);
        //                                                    Global.InvoiceNumber = null;
        //                                                }
        //                                                timer.Start();
        //                                            }
        //                                            break;
        //                                        }
        //                                        else
        //                                        {
        //                                            continue;
        //                                        }
        //                                    }
        //                                    else
        //                                    {
        //                                        continue;
        //                                    }
        //                                }
        //                                catch (Exception ex)
        //                                {
        //                                    if (count == 2)
        //                                    {
        //                                        MoveFile(invPath[0], "INVALID_INVOICE");
        //                                        objDSMModelData = new DSMModelData();

        //                                        if (Global.InvoiceNumber != null)
        //                                        {
        //                                            objDSMModelData.SaveQuarantine(Global.InvoiceNumber);
        //                                            Global.InvoiceNumber = null;
        //                                        }

        //                                        timer.Start();
        //                                        break;
        //                                    }
        //                                    LogErrors(ex);
        //                                }
        //                            }
        //                            catch (Exception i)
        //                            {
        //                                LogErrors(i);

        //                                if (Directory.Exists(directoryPath))
        //                                    Directory.Delete(directoryPath, true);

        //                                MoveFile(invPath[0], "INVALID_INVOICE");
        //                                objDSMModelData = new DSMModelData();

        //                                if (Global.InvoiceNumber != null)
        //                                {
        //                                    objDSMModelData.SaveQuarantine(Global.InvoiceNumber);
        //                                    Global.InvoiceNumber = null;
        //                                }

        //                                timer.Start();
        //                            }

        //                        }

        //                    }

        //                    else
        //                    {
        //                        try
        //                        {
        //                            if (!string.IsNullOrEmpty(Global.InvoiceNumber))
        //                            {
        //                                SplitAndSave(invPath[0], Global.InputPath, Global.InvoiceNumber, "INVOICE");
        //                            }
        //                        }
        //                        catch (Exception i)
        //                        {
        //                            LogErrors(i);
        //                            timer.Start();
        //                        }
        //                    }

        //                }
        //                else
        //                {
        //                    MoveFile(invPath[0], "AlreadyExist");
        //                    CurrentRefreshAlreadyInvoiceLists();
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogErrors(ex);
        //        timer.Start();
        //    }
        //}

        private string ScanQRPdf(System.Drawing.Image img, string outPutPath, string directory, string pdfPath)
        {
            string scaningResult = string.Empty;
            img.Save(outPutPath, System.Drawing.Imaging.ImageFormat.Png);
            System.Drawing.Image imgbmp = System.Drawing.Image.FromFile(outPutPath);
            Bitmap bmp = new Bitmap(imgbmp);
            //  Qrcode(outPutPath);
            try
            {
                bmp = ResizeImage(bmp, 120, 120);
                //bmp.Save(@"C:\\SOMS\Maximized.png", System.Drawing.Imaging.ImageFormat.Png);
                scaningResult = Spire.Barcode.BarcodeScanner.ScanOne(bmp);
                bmp.Dispose();
                if (scaningResult.Length < 64)
                {
                    Bitmap bmp0 = new Bitmap(imgbmp);
                    bmp0 = ResizeImage(bmp0, 200, 200);
                    //bmp.Save(@"C:\\SOMS\Maximized.png", System.Drawing.Imaging.ImageFormat.Png);
                    scaningResult = Spire.Barcode.BarcodeScanner.ScanOne(bmp0);
                    bmp0.Dispose();
                    if (scaningResult.Length < 64)
                    {
                        Bitmap bmp3 = new Bitmap(imgbmp);
                        bmp3 = ResizeImage(bmp3, 300, 300);
                        //bmp.Save(@"C:\\SOMS\Maximized.png", System.Drawing.Imaging.ImageFormat.Png);
                        scaningResult = Spire.Barcode.BarcodeScanner.ScanOne(bmp3);
                        bmp3.Dispose();
                        if (scaningResult.Length < 64)
                        {
                            Bitmap bmp4 = new Bitmap(imgbmp);
                            bmp4 = ResizeImage(bmp4, 350, 350);
                            //bmp.Save(@"C:\\SOMS\Maximized.png", System.Drawing.Imaging.ImageFormat.Png);
                            scaningResult = Spire.Barcode.BarcodeScanner.ScanOne(bmp4);
                            bmp4.Dispose();
                            if (scaningResult.Length < 64)
                            {
                                Bitmap bmp5 = new Bitmap(imgbmp);
                                bmp5 = ResizeImage(bmp5, 400, 400);
                                //bmp.Save(@"C:\\SOMS\Maximized.png", System.Drawing.Imaging.ImageFormat.Png);
                                scaningResult = Spire.Barcode.BarcodeScanner.ScanOne(bmp5);
                                bmp4.Dispose();
                            }


                            if (scaningResult.Length < 64)
                            {
                                Bitmap bmp1 = new Bitmap(imgbmp);
                                bmp1 = ResizeImage(bmp1, 450, 450);
                                //bmp.Save(@"C:\\SOMS\Maximized.png", System.Drawing.Imaging.ImageFormat.Png);
                                scaningResult = Spire.Barcode.BarcodeScanner.ScanOne(bmp1);
                                bmp1.Dispose();
                                if (scaningResult.Length < 64)
                                {
                                    Bitmap bmp2 = new Bitmap(imgbmp);
                                    bmp2 = ResizeImage(bmp2, 512, 512);
                                    //bmp.Save(@"C:\\SOMS\Maximized.png", System.Drawing.Imaging.ImageFormat.Png);
                                    scaningResult = Spire.Barcode.BarcodeScanner.ScanOne(bmp2);
                                    bmp2.Dispose();
                                }
                                if (scaningResult.Length < 64)
                                {
                                    int data = scaningResult.Length;
                                    Bitmap bmp2 = new Bitmap(imgbmp);
                                    bmp2 = ResizeImage(bmp2, 550, 550);
                                    //bmp.Save(@"C:\\SOMS\Maximized.png", System.Drawing.Imaging.ImageFormat.Png);
                                    scaningResult = Spire.Barcode.BarcodeScanner.ScanOne(bmp2);
                                    bmp2.Dispose();
                                }
                            }
                            if (scaningResult.Length < 64)
                            {
                                Bitmap bmp1 = new Bitmap(imgbmp);
                                bmp1 = ResizeImage(bmp1, 600, 600);
                                //bmp.Save(@"C:\\SOMS\Maximized.png", System.Drawing.Imaging.ImageFormat.Png);
                                scaningResult = Spire.Barcode.BarcodeScanner.ScanOne(bmp1);
                                bmp1.Dispose();
                                if (scaningResult.Length < 64)
                                {
                                    Bitmap bmp2 = new Bitmap(imgbmp);
                                    bmp2 = ResizeImage(bmp2, 700, 700);
                                    //bmp.Save(@"C:\\SOMS\Maximized.png", System.Drawing.Imaging.ImageFormat.Png);
                                    scaningResult = Spire.Barcode.BarcodeScanner.ScanOne(bmp2);
                                    bmp2.Dispose();
                                }
                                if (scaningResult.Length < 64)
                                {
                                    Bitmap bmp2 = new Bitmap(imgbmp);
                                    bmp2 = ResizeImage(bmp2, 800, 800);
                                    //bmp.Save(@"C:\\SOMS\Maximized.png", System.Drawing.Imaging.ImageFormat.Png);
                                    scaningResult = Spire.Barcode.BarcodeScanner.ScanOne(bmp2);
                                    bmp2.Dispose();
                                }
                                if (scaningResult.Length < 64)
                                {
                                    Bitmap bmp2 = new Bitmap(imgbmp);
                                    bmp2 = ResizeImage(bmp2, 900, 900);
                                    //bmp.Save(@"C:\\SOMS\Maximized.png", System.Drawing.Imaging.ImageFormat.Png);
                                    scaningResult = Spire.Barcode.BarcodeScanner.ScanOne(bmp2);
                                    bmp2.Dispose();
                                }
                            }
                        }
                    }
                }
                imgbmp.Dispose();
                img.Dispose();
            }
            catch (Exception ex)
            {
                LogErrors(ex);
                bmp.Dispose();
                imgbmp.Dispose();
                img.Dispose();

                if (Directory.Exists(directory))
                    Directory.Delete(directory, true);

                MoveFile(pdfPath, "INVALID_INVOICE");
                objDSMModelData = new DSMModelData();
                objDSMModelData.SaveQuarantine(Global.InvoiceNumber);
                timer.Start();
            }
            return scaningResult;
        }

        private void SignPDF(string DScoord)
        {
            string[] inputPathList = new string[0];

            try
            {
                CurrentRefreshSignedLists();

                if (oth_cus)
                {
                    inputPathList = Directory.GetFiles(Global.InvoicePath + "OTHER INVOICE\\", "*.pdf");

                    if (inputPathList.Length == 0)
                        return;

                    string[] file_name = inputPathList[0].ToString().Split('\\');
                    int v = file_name.Length;
                    string[] file = file_name[v - 1].ToString().Split('.');
                    Global.InvoiceNumber = file[0].ToString();

                    using (PdfReader reader = new PdfReader(inputPathList[0]))
                    {
                        string fullText = PdfTextExtractor.GetTextFromPage(reader, 1);
                        string[] lines = fullText.Split('\n');

                        foreach (var line in lines)
                        {
                            if (line.Trim().ToUpper().StartsWith("INV DATE"))
                            {
                                // Try to extract the date from the same line
                                Match match = Regex.Match(line, @"INV\s*DATE\s*[:\-]?\s*(\d{2}[./-]\d{2}[./-]\d{4})", RegexOptions.IgnoreCase);
                                if (match.Success)
                                {
                                    Global.InvoiceDate = match.Groups[1].Value.Trim();
                                    break;
                                }
                            }
                        }

                        if (string.IsNullOrWhiteSpace(Global.InvoiceDate))
                        {
                            Global.InvoiceDate = DateTime.Now.ToString("MM.dd.yyyy");
                        }
                    }

                    SplitAndSave(inputPathList[0], Global.InputPath, Global.InvoiceNumber, "INVOICE");
                    //SplitAndSave(inputPathList[0], Global.InputPath, Global.InvoiceNumber, "INVOICE", System.IO.Path.Combine(Global.InvoicePath, "INVOICE"));

                }

                else
                {
                    inputPathList = Directory.GetFiles(Global.InputPath, "*.pdf");
                }
                if (inputPathList.Count() == 0)
                {
                    ProcessMsg = "";
                }
                for (int i = 0; i < inputPathList.Length; i++)
                {
                    CurrentInvoice = "Invoice - " + Global.InvoiceNumber + " is Signing...";
                    objDigitalSign = new DigitalSign();
                    if (Global.CustomerName == "")
                    {
                        Global.CustomerName = "OTHER CUSTOMER";
                    }

                    bool sign = objDigitalSign.FlatDigitalSignatureInitialize("Invoice", Global.InvoiceNumber, Global.CustomerName, Global.InvoiceNumber, DScoord);
                    if (sign)
                    {
                        if (!Global.CustomerName.ToUpper().Contains("HYUNDAI"))
                        {
                            string checkQry = "SELECT COUNT(*) FROM Invoice WHERE InvNumber = '" + Global.InvoiceNumber + "'";
                            object result = Utilities.executeScalar(checkQry);

                            int count = result != null ? Convert.ToInt32(result) : 0;
                            if (count == 0)
                            {
                                string ins_qry = "INSERT INTO Invoice (CustomerName, InvNumber, InvDate, DSStatus, CreatedDate) " +
                                                 "VALUES ('" + Global.CustomerName + "', '" + Global.InvoiceNumber + "', '" + DateTime.Now.ToString("yyyy-MM-dd 00:00:00.000") + "','TRUE', GETDATE())";
                                bool ins = Utilities.executeNonQuery(ins_qry);
                            }
                        }


                        if (!Global.CustomerName.ToUpper().Contains("HYUNDAI"))
                        {
                            objDigitalSign.SendEmail(Global.InvoiceNumber);
                        }

                        if (Global.CustomerName.ToString().Trim() == "HYUNDAI")
                        {
                            string PdfFilename = Global.OutputPath + Global.InvoiceNumber + "ORIGINALINVOICE.pdf";
                            if (PdfFilename != null && File.Exists(PdfFilename))
                            {
                            }
                        }
                        else
                        {
                            string PdfFilename = Global.OutputPath + Global.InvoiceNumber + "ORIGINALinvoice.pdf";
                            if (PdfFilename != null && File.Exists(PdfFilename))
                            {
                                OthercustInv();
                            }
                        }
                    }
                    CurrentRefreshSignedLists();
                }
            }
            catch (Exception ex)
            {
                LogErrors(ex);
                timer.Start();
            }
        }

        private void PrintFiles()
        {
            try
            {
                string[] printPathList = Directory.GetFiles(Global.OutputPath, "*.pdf");
                if (printPathList.Count() == 0)
                {
                    ProcessMsg = "";
                }
                if (printPathList.ToString().Trim() != null)
                {
                    //PendingInvList = new ObservableCollection<InvoiceDisplayModel>();
                    //for (int i = 0; i < printPathList.Length; i++)
                    for (int i = 0; i < 1; i++)
                    {
                        CurrentInvoice = "Invoice - " + Global.InvoiceNumber + " is Printing...";
                        //EthernetConnectivityCheck();
                        Print objPrint = new Print();
                        var printStatus = objPrint.PrintInvoicePDFs(Global.InvoiceNumber, "invoice");
                        CurrentRefreshPrintedLists();
                    }
                }
                //}
                //timerPrintPost.Start();
            }
            catch (Exception ex)
            {
                LogErrors(ex);
                //timerPrintPost.Start();
                timer.Start();
            }
        }

        public static Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
        {
            var destRect = new System.Drawing.Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                    //wrapMode.Dispose();
                }
                //graphics.Dispose();
            }

            return destImage;
        }
       
        //private string ScanQRPdf(string imagePath)
        //{
        //    //EthernetConnectivityCheck();
        //    ArrayList barcodes = new ArrayList();
        //    DateTime dtStart = DateTime.Now;
        //    String decodedString = string.Empty;
        //    var img = (imagePath);
        //    if (System.IO.Path.GetExtension(img).ToLower() == ".bmp")
        //    {
        //        Bitmap b = new Bitmap(img);
        //        try
        //        {
        //            QRCodeDecoder decoder = new QRCodeDecoder();
        //            decodedString = decoder.decode(new QRCodeBitmapImage(b));
        //        }
        //        catch (Exception ex)
        //        {
        //            LogErrors(ex);
        //        }
        //        b.Dispose();
        //    }
        //    return decodedString;
        //}

        public void OthercustInv()
        {

            lstInvoice = new List<InvoiceDisplayModel>();
            objDSMModelData = new DSMModelData();
            for (int i = 0; i < 1; i++)
            {
                string dateString = Global.InvoiceDate;
                CultureInfo provider = CultureInfo.InvariantCulture;
                DateTime dateInv = DateTime.ParseExact(dateString, new string[] { "MM.dd.yyyy", "MM-dd-yyyy", "MM/dd/yyyy" }, provider, DateTimeStyles.None);

                objInvoice.InvNumber = Global.InvoiceNumber;
                objInvoice.InvDate = dateInv;

                objInvoice.CreatedBy = Global.UserId;
                objInvoice.CustomerName = Global.CustomerName;
                objInvoice.VehicleNumber = Global.TruckNumber;
                objInvoice.VendorCode = Global.VendorCode;
                objInvoice.IsEmailSend = Global.IsMailSend;
                objInvoice.OriginalPDF = Global.OriginalPDFFile;

                lstInvoice.Add(objInvoice);
            }

            objDSMModelData.saveOtherInvoice(lstInvoice);
            return;
        }
        public void CreateInvoice(string decodedString)
        {
            //EthernetConnectivityCheck();
            int count = 1;
            char[] a = { '\n', '\r' };
            string[] arr = decodedString.Split(a);
            arr = arr.Where(x => !string.IsNullOrEmpty(x)).ToArray();
            lstInvoice = new List<InvoiceDisplayModel>();
            objDSMModelData = new DSMModelData();

            for (int i = 0; i < arr.Length; i++)
            {
                if (count % 2 == 0)
                {
                    string[] arr1 = arr[i].Split('\t');

                    string dateString = arr1[1].Substring(2, 2) + '/' + arr1[1].Substring(0, 2) + '/' + arr1[1].Substring(4, 4);
                    CultureInfo provider = CultureInfo.InvariantCulture;
                    DateTime dateInv = DateTime.ParseExact(dateString, new string[] { "MM.dd.yyyy", "MM-dd-yyyy", "MM/dd/yyyy" }, provider, DateTimeStyles.None);

                    objInvoice.InvNumber = arr1[0];

                    Global.InvoiceNumber = Global.InvoiceNumber == null ? objInvoice.InvNumber : Global.InvoiceNumber;

                    objInvoice.InvDate = dateInv;
                    objInvoice.InvQuantity = arr1[1].Substring(8);
                    objInvoice.InvValue = arr1[2];
                    objInvoice.TarrifNumber = arr1[3].Substring(0, 10);
                    objInvoice.BedAmount = arr1[3].Substring(10);
                    objInvoice.SGST = arr1[4];
                    objInvoice.IGST = arr1[5];
                    objInvoice.VatAmount = arr1[6];
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
                    objInvoice.ShopCode = Global.Shopcode;
                    objInvoice.TCS = arr1[6];
                    //objInvoice.E_InvNo = arr1[17];
                    if (arr1[17].Length == 64)
                    {
                        objInvoice.E_InvNo = arr1[17];
                    }
                    else
                    {
                        objInvoice.E_InvNo = arr1[18];
                    }
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
                    objInvoice.PdfGeneration = "";
                    lstInvoice.Add(objInvoice);
                }
                else
                {
                    objInvoice = new InvoiceDisplayModel();
                    objInvoice.ShopCode = Global.Shopcode;
                    objInvoice.PONumber = arr[i].Substring(2, 10);
                    objInvoice.PartNumber = arr[i].Substring(12);
                    Global.InvoiceNumberchk = arr[i].Substring(12);
                    objInvoice.Finyear = Global.FinYear;
                    objInvoice.SystemName = System.Environment.MachineName;
                    objInvoice.OriginalPDF = Global.OriginalPDFFile;
                }
                count++;
            }

            objDSMModelData.SaveInvoice(lstInvoice);
        }
        public void CreateInvoiceChk(string decodedString)
        {
            //EthernetConnectivityCheck();
            int count = 1;
            char[] a = { '\n', '\r' };
            string[] arr = decodedString.Split(a);
            arr = arr.Where(x => !string.IsNullOrEmpty(x)).ToArray();
            lstInvoice = new List<InvoiceDisplayModel>();
            objDSMModelData = new DSMModelData();

            for (int i = 0; i < arr.Length; i++)
            {
                if (count % 2 == 0)
                {
                    string[] arr1 = arr[i].Split('\t');

                    string dateString = arr1[1].Substring(2, 2) + '/' + arr1[1].Substring(0, 2) + '/' + arr1[1].Substring(4, 4);
                    CultureInfo provider = CultureInfo.InvariantCulture;
                    DateTime dateInv = DateTime.ParseExact(dateString, new string[] { "MM.dd.yyyy", "MM-dd-yyyy", "MM/dd/yyyy" }, provider, DateTimeStyles.None);

                    objInvoice.InvNumber = arr1[0];

                    Global.InvoiceNumber = Global.InvoiceNumber == null ? objInvoice.InvNumber : Global.InvoiceNumber;

                    objInvoice.InvDate = dateInv;
                    objInvoice.InvQuantity = arr1[1].Substring(8);
                    objInvoice.InvValue = arr1[2];
                    objInvoice.TarrifNumber = arr1[3].Substring(0, 10);
                    objInvoice.BedAmount = arr1[3].Substring(10);
                    objInvoice.SGST = arr1[4];
                    objInvoice.IGST = arr1[5];
                    objInvoice.VatAmount = arr1[6];
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
                    objInvoice.ShopCode = Global.Shopcode;
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
                    objInvoice.PdfGeneration = "";
                    lstInvoice.Add(objInvoice);
                }
                else
                {
                    objInvoice = new InvoiceDisplayModel();
                    objInvoice.ShopCode = Global.Shopcode;
                    objInvoice.PONumber = arr[i].Substring(2, 10);
                    objInvoice.PartNumber = arr[i].Substring(12);
                    Global.InvoiceNumberchk = arr[i].Substring(12);
                    objInvoice.Finyear = Global.FinYear;
                    objInvoice.SystemName = System.Environment.MachineName;
                    objInvoice.OriginalPDF = Global.OriginalPDFFile;
                }
                count++;
            }

            objDSMModelData.SaveInvoice(lstInvoice);
        }

        //public void AssignInvoiceNo_TruckNo(string src, string type)
        //{
        //    string truckNumber = string.Empty;
        //    string pathToPdf = src;

        //    try
        //    {

        //        Customers = new ObservableCollection<CustomerDisplayModel>(objDSMModelData.GetAllCustomers());

        //        using (PdfReader Custreader = new PdfReader(pathToPdf))
        //        {

        //            var custparser = new PdfReaderContentParser(Custreader);

        //            var custstrategy = custparser.ProcessContent(1, new LocationTextExtractionStrategyWithPosition());

        //            var res = custstrategy.GetLocations();

        //            Custreader.Close();

        //            //var custid = Customers;
        //            Global.CustomerName = "";
        //            Global.CustomerId = "";
        //            Global.Templatename = "";
        //            int custcount = 0;
        //            for (int k = 0; k < Customers.Count; k++)
        //            {

        //                var custsearchResult = res.Where(p => p.Text.ToUpper().Contains(Customers[k].CustomerName.ToUpper())).OrderBy(p => p.Y).Reverse().ToList();
        //                //var custsearchResult = res.Where(p => p.Text.Contains("Mobis")).OrderBy(p => p.Y).Reverse().ToList();
        //                if (custsearchResult.Count > 0)
        //                {
        //                    custcount++;
        //                    Global.CustomerName = Customers[k].CustomerName.ToString().Trim();
        //                    Global.CustomerId = Customers[k].CustomerId.ToString().Trim();

        //                    if (!Global.CustomerName.ToUpper().Contains("HYUNDAI"))
        //                    {
        //                        MoveFile(pathToPdf, "OTHER INVOICE");
        //                        othercustomer("Other Customer Invoice");
        //                        oth_cus = true;
        //                        return;
        //                    }
        //                    break;
        //                }


        //            }
        //            if (custcount == 0)
        //            {
        //                MoveFile(pathToPdf, "OTHER INVOICE");
        //                othercustomer("Other Customer Invoice");
        //                oth_cus = true;
        //                return;
        //            }
        //        }

        //        SettingsDisplayModel objSettings = new SettingsDisplayModel();
        //        CustomerDisplayModel objCustomer = new CustomerDisplayModel();
        //        objSettings = objDSMModelData.GetSettingsData(System.Environment.MachineName);
        //        if (objSettings != null)
        //        {
        //            Global.SystemName = objSettings.SystemName;
        //            Global.InputPath = objSettings.InputPath;
        //            Global.InvoicePath = objSettings.InvoicePath;
        //            Global.OutputPath = objSettings.OutputPath;
        //            Global.PrinterName = objSettings.PrinterName;

        //        }

        //        objCustomer = objDSMModelData.GetCustomerByName(Global.CustomerName);

        //        if (objCustomer != null)
        //        {
        //            Global.VendorCode = objCustomer.VendorCode;
        //            Global.CustomerId = objCustomer.CustomerId;
        //            Global.PDFtype = objCustomer.PDFtype == true ? "SINGLE" : "MULTIPLE";
        //            Global.PrintType = objCustomer.PrintType == true ? "ORIGINAL" : "SELECTED";
        //            Global.PrintCopies = objCustomer.PrintCopies;
        //            if (objCustomer.IsAPIpost)
        //            {
        //                Global.IsAPIpost = true;
        //                Global.APIpostType = objCustomer.APItype == true ? "AUTO" : "MANUAL";
        //                Global.APIurl = objCustomer.APIUrl;
        //                if (Global.APIurl == "https://hmieai.hmil.net:5004/Service.asmx?WSDL")
        //                {
        //                    Global.APIPosturl = "http://hmieai/DI/data";
        //                }
        //                else
        //                {
        //                    Global.APIPosturl = "http://hmieai/DI/data";
        //                }
        //            }
        //            else
        //            {
        //                Global.IsAPIpost = false;
        //            }
        //        }
        //        PdfReader reader = new PdfReader(pathToPdf);
        //        //if(reader.)
        //        if (Global.CustomerId != "" && Global.CustomerId.Length > 0)
        //        {
        //            //var dsDataDetail = objDSMModelData.GetDSdetailbyClientId("HMI1");
        //            var dsDataDetail = objDSMModelData.GetDSdetailbyClientId(Global.CustomerId);

        //            string invnocoord = "";
        //            string invdatecoord = "";
        //            string vendorcodecoord = "";
        //            string trucknocoord = "";
        //            string shopcdcoord = "";

        //            var template = objDSMModelData.GetTemplates(Global.CustomerId);
        //            Global.Templatename = template[0].Tempname.ToString();
        //            var coordinates = objDSMModelData.GetTemplatesCord(Global.Templatename, Global.CustomerId);
        //            //string coord = "";
        //            for (int i = 0; i < coordinates.Count; i++)
        //            {
        //                //string DSSignName = dsMasterData[i].KeyName;
        //                invnocoord = coordinates[i].Invnocoord.ToString().Trim();
        //                invdatecoord = coordinates[i].Invdatecoord.ToString().Trim();
        //                vendorcodecoord = coordinates[i].Vendorcoord.ToString().Trim();
        //                trucknocoord = coordinates[i].Trucknocoord.ToString().Trim();
        //                shopcdcoord = coordinates[i].Shopcdcoord.ToString().Trim();
        //                //clientnamecoord = coordinates[i].Clientnamecoord.ToString().Trim();
        //            }
        //            if (invnocoord.Length > 0 && invnocoord.ToString().Trim() != "")
        //            {
        //                RenderFilter[] filters = new RenderFilter[1];
        //                LocationTextExtractionStrategy regionFilter = new LocationTextExtractionStrategy();
        //                string[] arrinvcoord = invnocoord.Split(',');
        //                filters[0] = new RegionTextRenderFilter(new iTextSharp.text.Rectangle(int.Parse(arrinvcoord[0]), int.Parse(arrinvcoord[1]), int.Parse(arrinvcoord[2]), int.Parse(arrinvcoord[3])));
        //                FilteredTextRenderListener strategy = new FilteredTextRenderListener(regionFilter, filters);

        //                String invresult = PdfTextExtractor.GetTextFromPage(reader, 1, strategy);
        //                string[] invno = new string[3];
        //                if (invresult.Contains(':'))
        //                {
        //                    invno = invresult.Split(':');
        //                    int invind = invno.GetLength(0);
        //                    Global.InvoiceNumber = invno[invind - 1].ToString().Trim();
        //                }
        //                else if (invresult.Contains('\t'))
        //                {
        //                    invno = invresult.Split('\t');
        //                    int invind = invno.GetLength(0);
        //                    Global.InvoiceNumber = invno[invind - 1].ToString().Trim();
        //                }
        //                else if (invresult.Contains('-'))
        //                {
        //                    invno = invresult.Split('-');
        //                    int invind = invno.GetLength(0);
        //                    Global.InvoiceNumber = invno[invind - 1].ToString().Trim();
        //                }
        //                //int invind = invno.GetLength(0);
        //                //Global.InvoiceNumber = invno[invind-1].ToString().Trim();
        //                else
        //                {
        //                    invno = invresult.Split(' ');
        //                    int invind = invno.GetLength(0);
        //                    Global.InvoiceNumber = invno[invind - 1].ToString().Trim();

        //                    //Global.InvoiceNumber = invresult.Trim();
        //                }
        //            }


        //            if (invdatecoord.Length > 0 && invdatecoord.ToString().Trim() != "")
        //            {
        //                RenderFilter[] filters = new RenderFilter[1];
        //                LocationTextExtractionStrategy regionFilter = new LocationTextExtractionStrategy();
        //                string[] arrinvdatecoord = invdatecoord.Split(',');
        //                filters[0] = new RegionTextRenderFilter(new iTextSharp.text.Rectangle(int.Parse(arrinvdatecoord[0]), int.Parse(arrinvdatecoord[1]), int.Parse(arrinvdatecoord[2]), int.Parse(arrinvdatecoord[3])));
        //                FilteredTextRenderListener strategy = new FilteredTextRenderListener(regionFilter, filters);

        //                String invdateresult = PdfTextExtractor.GetTextFromPage(reader, 1, strategy);
        //                string[] invdate = new string[3];
        //                if (invdateresult.Contains(':'))
        //                {
        //                    invdate = invdateresult.Split(':');
        //                    int invind = invdate.GetLength(0);
        //                    Global.InvoiceDate = invdate[invind - 1].ToString().Trim();
        //                }
        //                else if (invdateresult.Contains('\t'))
        //                {
        //                    invdate = invdateresult.Split('\t');
        //                    int invind = invdate.GetLength(0);
        //                    Global.InvoiceDate = invdate[invind - 1].ToString().Trim();
        //                }
        //                //else if (invdateresult.Contains('-'))
        //                //{
        //                //    invdate = invdateresult.Split('-');
        //                //    int invind = invdate.GetLength(0);
        //                //    Global.InvoiceDate = invdate[invind - 1].ToString().Trim();
        //                //}
        //                else
        //                {
        //                    Global.InvoiceDate = invdateresult;
        //                }
        //                //Global.InvoiceDate = "11/11/2019";
        //                //int invind = invdate.GetLength(0);
        //                //Global.InvoiceDate = invdate[invind-1].ToString().Trim();
        //            }

        //            if (trucknocoord.Length > 0 && trucknocoord.ToString().Trim() != "")
        //            {
        //                RenderFilter[] filters = new RenderFilter[1];
        //                LocationTextExtractionStrategy regionFilter = new LocationTextExtractionStrategy();
        //                string[] arrtrucknocoord = trucknocoord.Split(',');
        //                filters[0] = new RegionTextRenderFilter(new iTextSharp.text.Rectangle(int.Parse(arrtrucknocoord[0]), int.Parse(arrtrucknocoord[1]), int.Parse(arrtrucknocoord[2]), int.Parse(arrtrucknocoord[3])));
        //                FilteredTextRenderListener strategy = new FilteredTextRenderListener(regionFilter, filters);

        //                String truckresult = PdfTextExtractor.GetTextFromPage(reader, 1, strategy);
        //                string[] truck = new string[3];
        //                if (truckresult.Contains(':'))
        //                {
        //                    truck = truckresult.Split(':');
        //                    int invind = truck.GetLength(0);
        //                    Global.TruckNumber = truck[invind - 1].ToString().Trim();
        //                }
        //                else if (truckresult.Contains('\t'))
        //                {
        //                    truck = truckresult.Split('\t');
        //                    int invind = truck.GetLength(0);
        //                    Global.TruckNumber = truck[invind - 1].ToString().Trim();
        //                }
        //                else if (truckresult.Contains('-'))
        //                {
        //                    truck = truckresult.Split('-');
        //                    int invind = truck.GetLength(0);
        //                    Global.TruckNumber = truck[invind - 1].ToString().Trim();
        //                }
        //                else
        //                {
        //                    Global.TruckNumber = truckresult;
        //                }

        //            }

        //            if (vendorcodecoord.Length > 0 && vendorcodecoord.ToString().Trim() != "")
        //            {
        //                RenderFilter[] filters = new RenderFilter[1];
        //                LocationTextExtractionStrategy regionFilter = new LocationTextExtractionStrategy();
        //                string[] arrvendorcodecoord = vendorcodecoord.Split(',');
        //                filters[0] = new RegionTextRenderFilter(new iTextSharp.text.Rectangle(int.Parse(arrvendorcodecoord[0]), int.Parse(arrvendorcodecoord[1]), int.Parse(arrvendorcodecoord[2]), int.Parse(arrvendorcodecoord[3])));
        //                FilteredTextRenderListener strategy = new FilteredTextRenderListener(regionFilter, filters);

        //                String vendorresult = PdfTextExtractor.GetTextFromPage(reader, 1, strategy);
        //                string[] vendor = new string[3];
        //                if (vendorresult.Contains(':'))
        //                {
        //                    vendor = vendorresult.Split(':');
        //                    int invind = vendor.GetLength(0);
        //                    Global.VendorCode = vendor[invind - 1].ToString().Trim();
        //                }
        //                else if (vendorresult.Contains('\t'))
        //                {
        //                    vendor = vendorresult.Split('\t');
        //                    int invind = vendor.GetLength(0);
        //                    Global.VendorCode = vendor[invind - 1].ToString().Trim();
        //                }
        //                else if (vendorresult.Contains('-'))
        //                {
        //                    vendor = vendorresult.Split('-');
        //                    int invind = vendor.GetLength(0);
        //                    Global.VendorCode = vendor[invind - 1].ToString().Trim();
        //                }
        //                else
        //                {
        //                    Global.VendorCode = vendorresult;
        //                }

        //            }
        //            if (shopcdcoord.Length > 0 && shopcdcoord.ToString().Trim() != "")
        //            {
        //                RenderFilter[] filters = new RenderFilter[1];
        //                LocationTextExtractionStrategy regionFilter = new LocationTextExtractionStrategy();
        //                // filters[0] = new RegionTextRenderFilter(new iTextSharp.text.Rectangle(160,70,220,82));
        //                string[] arrvendorcodecoord = shopcdcoord.Split(',');
        //                filters[0] = new RegionTextRenderFilter(new iTextSharp.text.Rectangle(int.Parse(arrvendorcodecoord[0]), int.Parse(arrvendorcodecoord[1]), int.Parse(arrvendorcodecoord[2]), int.Parse(arrvendorcodecoord[3])));
        //                FilteredTextRenderListener strategy = new FilteredTextRenderListener(regionFilter, filters);

        //                String invresult = PdfTextExtractor.GetTextFromPage(reader, 1, strategy);
        //                string[] invno = new string[3];
        //                if (invresult.Contains(':'))
        //                {
        //                    invno = invresult.Split(':');
        //                    int invind = invno.GetLength(0);
        //                    Global.Shopcode = invno[invind - 1].ToString().Trim();
        //                }
        //                else if (invresult.Contains('\t'))
        //                {
        //                    invno = invresult.Split('\t');
        //                    int invind = invno.GetLength(0);
        //                    Global.Shopcode = invno[invind - 1].ToString().Trim();
        //                }
        //                else if (invresult.Contains('-'))
        //                {
        //                    invno = invresult.Split('-');
        //                    int invind = invno.GetLength(0);
        //                    Global.Shopcode = invno[invind - 1].ToString().Trim();
        //                }
        //                //int invind = invno.GetLength(0);
        //                //Global.InvoiceNumber = invno[invind-1].ToString().Trim();
        //                else
        //                {
        //                    Global.Shopcode = invresult;

        //                }
        //                //cond.
        //                String scd = Global.Shopcode;
        //                if (!string.IsNullOrEmpty(scd))
        //                {
        //                    string auth = scd.Substring(0, 2);
        //                    Global.Shopcode = auth;
        //                }
        //            }

        //        }
        //        reader.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        LogErrors(ex);
        //        MoveFile(pathToPdf, "INVALID_INVOICE");
        //        timer.Start();
        //    }
        //}

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

        public int SplitAndSave(string invoicePath, string outputPath, string tdocno_from, string invtype)
        {
            using (iTextSharp.text.pdf.PdfReader pdfReader = new iTextSharp.text.pdf.PdfReader(invoicePath))
            {
                return pdfReader.NumberOfPages;
            }
        }

        private void MoveOriginalFileToProcessed(string source)
        {
            if (string.IsNullOrWhiteSpace(source) || !File.Exists(source))
                throw new FileNotFoundException("Source file not found.", source);

            string processedFolder = System.IO.Path.Combine(Global.InvoicePath, "PROCESSED");
            Directory.CreateDirectory(processedFolder);

            string destination = System.IO.Path.Combine(processedFolder, System.IO.Path.GetFileName(source));

            if (File.Exists(destination))
                File.Delete(destination);

            File.Move(source, destination);
        }

        //public int SplitAndSave(string invoicePath, string outputPath, string tdocno_from, string invtype)
        //{
        //    string _from = tdocno_from.ToString().Trim();

        //    FileInfo file = new FileInfo(invoicePath);
        //    string name = file.Name.Substring(0, file.Name.LastIndexOf("."));

        //    using (iTextSharp.text.pdf.PdfReader pdfReader = new iTextSharp.text.pdf.PdfReader(invoicePath))
        //    {
        //        for (int pageNumber = 1; pageNumber <= pdfReader.NumberOfPages; pageNumber++)
        //        //for (int pageNumber = 1; pageNumber <= 1; pageNumber++)
        //        {
        //            //string vinvono = Convert.ToDecimal(_from).ToString("000000");
        //            string vinvono = "";

        //            if (pageNumber == 1)
        //                vinvono = _from.ToString().Trim() + "ORIGINAL" + invtype.ToString().Trim();
        //            else if (pageNumber == 2)
        //                vinvono = _from.ToString().Trim() + "DUPLICATE" + invtype.ToString().Trim();
        //            else if (pageNumber == 3)
        //                vinvono = _from.ToString().Trim() + "TRIPLICATE" + invtype.ToString().Trim();
        //            else if (pageNumber == 4)
        //                vinvono = _from.ToString().Trim() + "EXTRA" + invtype.ToString().Trim();
        //            else if (pageNumber == 5)
        //                vinvono = _from.ToString().Trim() + "EXTRA1" + invtype.ToString().Trim();
        //            else
        //                vinvono = _from.ToString().Trim() + "QUADRAPLICATE" + invtype.ToString().Trim();

        //            Global.PrintCopies = "ORIGINAL-DUPLICATE-TRIPLICATE-QUADRAPLICATE-EXTRA-EXTRA1";

        //            // Get CGHS No.
        //            iTextSharp.text.pdf.parser.ITextExtractionStrategy strategy = new iTextSharp.text.pdf.parser.SimpleTextExtractionStrategy();
        //            string currentText = iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(pdfReader, pageNumber, strategy);
        //            // Get CGHS No from the pdf as per your pdf containt.
        //            // string CGHSNo = currentText.Split(new string[] { "DEBIT NOTE" }, StringSplitOptions.None)[1].Split('\n')[0];


        //            // Generate New pdf with CGHS No.
        //            //string filename = "A"+pageNumber+".pdf";
        //            iTextSharp.text.Document document = new iTextSharp.text.Document();
        //            iTextSharp.text.pdf.PdfCopy pdfCopy = new iTextSharp.text.pdf.PdfCopy(document, new FileStream(outputPath + vinvono + ".pdf", FileMode.Create));

        //            pdfCopy.SetPdfVersion(iTextSharp.text.pdf.PdfWriter.PDF_VERSION_1_2);

        //            pdfCopy.CompressionLevel = iTextSharp.text.pdf.PdfStream.BEST_SPEED;

        //            //pdfCopy.CompressionLevel = PdfStream.DEFAULT_COMPRESSION;
        //            pdfCopy.SetFullCompression();

        //            document.Open();


        //            pdfCopy.AddPage(pdfCopy.GetImportedPage(pdfReader, pageNumber));

        //            document.Close();

        //            //_from++;
        //        }
        //        pdfReader.Close();
        //        MoveFile(invoicePath, "PROCESSED");
        //        return pdfReader.NumberOfPages;
        //    }
        //}

        public void MoveFile(string source, string location)
        {
            System.IO.StreamReader fileMove = new System.IO.StreamReader(source);
            string _SourceFile = source;

            //check folder
            if (!System.IO.Directory.Exists(Global.InvoicePath + "\\" + location + "\\"))
            {
                System.IO.Directory.CreateDirectory(Global.InvoicePath + "\\" + location + "\\");
            }
            if (!System.IO.Directory.Exists(Global.InvoicePath + "\\OTHER INVOICE\\" + location + "\\"))
            {
                System.IO.Directory.CreateDirectory(Global.InvoicePath + "\\OTHER INVOICE\\" + location + "\\");
            }
            string _DestFile = Global.InvoicePath + "\\" + location + "\\" + System.IO.Path.GetFileName(source);
            if (oth_cus)
            {
                _DestFile = Global.InvoicePath + "\\OTHER INVOICE\\" + location + "\\" + System.IO.Path.GetFileName(source);
            }
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

        public void othercustomer(String Data)
        {
            StringBuilder sb = new StringBuilder();
            string path = AppDomain.CurrentDomain.BaseDirectory + "OtherInvoiceDetails.txt";
            // flush every 20 seconds as you do it

            sb.AppendLine();
            sb.Append("Log Time: " + DateTime.Now);
            sb.AppendLine();
            sb.Append("--------------------------------------------------");
            sb.AppendLine();

            if (Data.ToString().Trim() != "" && Data.ToString().Trim() != null)
            {
                sb.Append("Other Customer PDF was found. Please add your customer in Customer Master");
            }
            sb.AppendLine();
            sb.Append("--------------------------------------------------");
            sb.AppendLine();

            File.AppendAllText(path, sb.ToString());
            sb.Clear();
        }

        //private void ReadPDF1()
        //{
        //    try
        //    {
        //        Global.InvoiceDate = null;
        //        Global.InvoiceNumber = null;
        //        oth_cus = false;

        //        string[] excelFiles = Directory.GetFiles(Global.InvoicePath, "*.xlsx");
        //        if (excelFiles.Length == 0)
        //            throw new Exception("Excel invoice file not found.");

        //        string excelPath = excelFiles[0];

        //        string excelInvoiceNo = ReadExcelInvoiceNumber(excelPath);
        //        if (string.IsNullOrWhiteSpace(excelInvoiceNo))
        //            throw new Exception("Invoice number is missing in Excel.");

        //        string[] pdfFiles = Directory.GetFiles(Global.InvoicePath, "*.pdf");
        //        if (pdfFiles.Length == 0)
        //            throw new Exception("PDF invoice not found.");

        //        string pdfPath = pdfFiles[0];

        //        // Read invoice number from PDF using OCR/QR.
        //        string pdfInvoiceNo = ReadInvoiceNumberFromPDF(pdfPath);

        //        if (string.IsNullOrWhiteSpace(pdfInvoiceNo))
        //        {
        //            System.Windows.MessageBox.Show("Invoice number could not be detected from the PDF.");
        //            return;
        //        }

        //        // Compare Excel and PDF invoice numbers before processing.
        //        if (!string.Equals(excelInvoiceNo.Trim(), pdfInvoiceNo.Trim(), StringComparison.OrdinalIgnoreCase))
        //        {
        //            System.Windows.MessageBox.Show("Invoice number mismatch.\nExcel: " + excelInvoiceNo + "\nPDF: " + pdfInvoiceNo);
        //            return;
        //        }

        //        // Read invoice details from Excel after invoice number match.
        //        string invoiceNo, customerName, gstin, poNumber, shopCode, vehicleNo, vendorCode;
        //        DateTime invoiceDate;

        //        string partNo = "", partName = "";

        //        using (ClosedXML.Excel.XLWorkbook workbook = new ClosedXML.Excel.XLWorkbook(excelPath))
        //        {
        //            var sheet = workbook.Worksheet(1);

        //            invoiceNo = sheet.Cell("G4").GetString().Trim();
        //            invoiceDate = ReadExcelDate(sheet.Cell("J4"));
        //            customerName = "HYUNDAI";
        //            gstin = sheet.Cell("D14").GetString().Trim();
        //            poNumber = sheet.Cell("G10").GetString().Trim();
        //            shopCode = sheet.Cell("J14").GetString().Trim();
        //            vehicleNo = sheet.Cell("J16").GetString().Trim();
        //            vendorCode = "TNGV";

        //            List<int> itemRows = FindItemRows(sheet);

        //            if (itemRows.Count == 0)
        //                throw new Exception("No item rows found in Excel.");

        //            int itemRow = itemRows[0];

        //            partName = sheet.Cell(itemRow, 2).GetString().Trim();  // Column B
        //            partNo = sheet.Cell(itemRow, 10).GetString().Trim();   // Column J

        //            if (string.IsNullOrWhiteSpace(partName))
        //                throw new Exception("Part Name is missing in Excel.");

        //            if (string.IsNullOrWhiteSpace(partNo))
        //                throw new Exception("Part Number is missing in Excel.");
        //        }

        //        // Use the verified Excel values.
        //        Global.InvoiceNumber = invoiceNo;
        //        Global.InvoiceDate = invoiceDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

        //        // Pass all Excel invoice details to the existing method.
        //        AssignInvoiceNo_TruckNo(pdfPath, "INV", invoiceNo, invoiceDate, customerName, gstin, poNumber, shopCode, vehicleNo, vendorCode);

        //        if (oth_cus || string.IsNullOrWhiteSpace(Global.InvoiceNumber))
        //            return;

        //        string finYear = GetFinancialYear(Global.InvoiceDate.Trim());
        //        Global.FinYear = Convert.ToInt32(finYear.Trim());

        //        CurrentRefreshInvoiceLists();

        //        objDSMModelData = new DSMModelData();
        //        int invoiceCount = objDSMModelData.GetInvoiceDetailsByInvoiceNumberFinYear(Global.InvoiceNumber, Convert.ToInt32(finYear)).Count;

        //        if (invoiceCount > 0)
        //        {
        //            MoveFile(pdfPath, "AlreadyExist");
        //            MoveFile(excelPath, "AlreadyExist");
        //            CurrentRefreshAlreadyInvoiceLists();
        //            return;
        //        }

        //        CreateInvoiceFromExcel(excelPath, pdfPath);

        //        Template(invoiceNo, 4, gstin);

        //        SplitAndSave(pdfPath, Global.InputPath, Global.InvoiceNumber, "INVOICE");

        //        // Move the original PDF and Excel only after processing is complete.
        //        MoveOriginalFileToProcessed(pdfPath);
        //        MoveOriginalFileToProcessed(excelPath);

        //        //SplitAndSave(pdfPath, Global.InputPath, Global.InvoiceNumber, "INVOICE", System.IO.Path.Combine(Global.InvoicePath, "INVOICE"));
        //    }
        //    catch (Exception ex)
        //    {
        //        LogErrors(ex);
        //        timer.Start();
        //    }
        //}

        private void ReadPDF1()
        {
            try
            {
                Global.InvoiceDate = null;
                Global.InvoiceNumber = null;
                oth_cus = false;

                string[] excelFiles = Directory.GetFiles(Global.InvoicePath, "*.xlsx");
                string[] pdfFiles = Directory.GetFiles(Global.InvoicePath, "*.pdf");

                if (excelFiles.Length == 0)
                    throw new Exception("Excel invoice file not found.");

                if (pdfFiles.Length == 0)
                    throw new Exception("PDF invoice not found.");

                foreach (string excelPath in excelFiles)
                {
                    string excelInvoiceNo = ReadExcelInvoiceNumber(excelPath);

                    if (string.IsNullOrWhiteSpace(excelInvoiceNo))
                    {
                        System.Windows.MessageBox.Show(
                            "Invoice number is missing in Excel:\n" + System.IO.Path.GetFileName(excelPath));
                        continue;
                    }

                    string pdfPath = null;
                    string pdfInvoiceNo = "";

                    // Search all PDFs until the matching invoice number is found.
                    foreach (string file in pdfFiles)
                    {
                        string extractedInvoiceNo = ReadInvoiceNumberFromPDF(file);

                        if (!string.IsNullOrWhiteSpace(extractedInvoiceNo) &&
                            string.Equals(excelInvoiceNo.Trim(), extractedInvoiceNo.Trim(),
                            StringComparison.OrdinalIgnoreCase))
                        {
                            pdfPath = file;
                            pdfInvoiceNo = extractedInvoiceNo;
                            break;
                        }
                    }

                    if (pdfPath == null)
                    {
                        System.Windows.MessageBox.Show(
                            "Matching PDF invoice not found.\nExcel Invoice: " + excelInvoiceNo +
                            "\nExcel File: " + System.IO.Path.GetFileName(excelPath),
                            "Invoice Mismatch",
                            System.Windows.MessageBoxButton.OK,
                            System.Windows.MessageBoxImage.Warning);

                        continue;
                    }

                    // Read invoice details from Excel after invoice number match.
                    string invoiceNo, customerName, gstin, poNumber, shopCode, vehicleNo, vendorCode;
                    DateTime invoiceDate;
                    string partNo = "", partName = "";

                    using (ClosedXML.Excel.XLWorkbook workbook = new ClosedXML.Excel.XLWorkbook(excelPath))
                    {
                        var sheet = workbook.Worksheet(1);

                        invoiceNo = sheet.Cell("G4").GetString().Trim();
                        invoiceDate = ReadExcelDate(sheet.Cell("J4"));
                        customerName = "HYUNDAI";
                        gstin = sheet.Cell("D14").GetString().Trim();
                        poNumber = sheet.Cell("G10").GetString().Trim();
                        shopCode = sheet.Cell("J14").GetString().Trim();
                        vehicleNo = sheet.Cell("J16").GetString().Trim();
                        vendorCode = "TNGV";

                        List<int> itemRows = FindItemRows(sheet);

                        if (itemRows.Count == 0)
                            throw new Exception("No item rows found in Excel: " + System.IO.Path.GetFileName(excelPath));

                        int itemRow = itemRows[0];
                        partName = sheet.Cell(itemRow, 2).GetString().Trim();
                        partNo = sheet.Cell(itemRow, 10).GetString().Trim();

                        if (string.IsNullOrWhiteSpace(partName))
                            throw new Exception("Part Name is missing in Excel: " + System.IO.Path.GetFileName(excelPath));

                        if (string.IsNullOrWhiteSpace(partNo))
                            throw new Exception("Part Number is missing in Excel: " + System.IO.Path.GetFileName(excelPath));
                    }

                    // Use the verified Excel invoice details.
                    Global.InvoiceNumber = invoiceNo;
                    Global.InvoiceDate = invoiceDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                    // Process the matching PDF.
                    AssignInvoiceNo_TruckNo(
                        pdfPath, "INV", invoiceNo, invoiceDate, customerName,
                        gstin, poNumber, shopCode, vehicleNo, vendorCode);

                    if (oth_cus || string.IsNullOrWhiteSpace(Global.InvoiceNumber))
                        continue;

                    string finYear = GetFinancialYear(Global.InvoiceDate.Trim());
                    Global.FinYear = Convert.ToInt32(finYear.Trim());

                    CurrentRefreshInvoiceLists();

                    objDSMModelData = new DSMModelData();

                    int invoiceCount = objDSMModelData
                        .GetInvoiceDetailsByInvoiceNumberFinYear(
                            Global.InvoiceNumber, Convert.ToInt32(finYear)).Count;

                    if (invoiceCount > 0)
                    {
                        MoveFile(pdfPath, "AlreadyExist");
                        MoveFile(excelPath, "AlreadyExist");
                        CurrentRefreshAlreadyInvoiceLists();
                        continue;
                    }

                    CreateInvoiceFromExcel(excelPath, pdfPath);
                    Template(invoiceNo, 4, gstin);
                    SplitAndSave(pdfPath, Global.InputPath, Global.InvoiceNumber, "INVOICE");

                    // Move original files only after processing is complete.
                    MoveOriginalFileToProcessed(pdfPath);
                    MoveOriginalFileToProcessed(excelPath);
                }
            }
            catch (Exception ex)
            {
                LogErrors(ex);
                timer.Start();
            }
        }

        private List<int> FindItemRows(ClosedXML.Excel.IXLWorksheet sheet)
        {
            var rows = new List<int>();

            int headerRow = 0;
            foreach (var cell in sheet.Column(2).CellsUsed())
            {
                if (cell.GetString().Trim().StartsWith("Description of Goods", StringComparison.OrdinalIgnoreCase))
                {
                    headerRow = cell.Address.RowNumber;
                    break;
                }
            }

            if (headerRow == 0)
                throw new Exception("Item table header 'Description of Goods' not found in Excel.");

            int lastRow = sheet.LastRowUsed().RowNumber();

            for (int r = headerRow + 1; r <= lastRow; r++)
            {
                string serialNo = sheet.Cell(r, 1).GetString().Trim();

                if (int.TryParse(serialNo, out int sl) && sl > 0)
                {
                    rows.Add(r);
                    continue;
                }

                // Subtotal/total row: check Amount in column N (14).
                string firstCell = sheet.Cell(r, 1).GetString().Trim();
                if (firstCell.Equals("Total", StringComparison.OrdinalIgnoreCase) ||
                    ReadExcelDecimal(sheet.Cell(r, 14)) > 0)
                    break;
            }

            return rows;
        }

        private string ReadTcsFromExcel(ClosedXML.Excel.IXLWorksheet sheet)
        {
            foreach (var cell in sheet.CellsUsed())
            {
                string text = cell.GetString().Trim();
                if (!text.StartsWith("TCS", StringComparison.OrdinalIgnoreCase)) continue;

                int row = cell.Address.RowNumber;
                int lastColumn = sheet.LastColumnUsed().ColumnNumber();

                // Look to the right on the same row for a percentage such as "1%" or "0.1%"
                for (int c = cell.Address.ColumnNumber + 1; c <= lastColumn; c++)
                {
                    string value = sheet.Cell(row, c).GetString().Trim();
                    if (value.EndsWith("%") &&
                        decimal.TryParse(value.TrimEnd('%'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal rate))
                        return rate.ToString("0.00", CultureInfo.InvariantCulture);
                }
            }

            return "0.00";  // no TCS row in the Excel
        }

        private void ReadTaxFromHeader(ClosedXML.Excel.IXLWorksheet sheet, string[] headerNames, out decimal rate, out decimal amount)
        {
            rate = 0m;
            amount = 0m;

            // 1. Find the header cell with an exact match (e.g. "CGST" or "SGST/UTGST")
            ClosedXML.Excel.IXLCell header = null;
            foreach (var cell in sheet.CellsUsed())
            {
                string text = cell.GetString().Trim();
                if (headerNames.Any(h => text.Equals(h, StringComparison.OrdinalIgnoreCase)))
                {
                    header = cell;
                    break;
                }
            }

            if (header == null) return;   // e.g. IGST on an intra-state invoice

            // 2. In the row below the header, find the "Rate" and "Amount" columns
            int subRow = header.Address.RowNumber + 1;
            int startCol = header.Address.ColumnNumber;
            int rateCol = 0, amountCol = 0;

            for (int c = startCol; c <= startCol + 2; c++)
            {
                string sub = sheet.Cell(subRow, c).GetString().Trim();
                if (rateCol == 0 && sub.Equals("Rate", StringComparison.OrdinalIgnoreCase)) rateCol = c;
                else if (amountCol == 0 && sub.Equals("Amount", StringComparison.OrdinalIgnoreCase)) amountCol = c;
            }

            if (rateCol == 0 || amountCol == 0) return;

            // 3. Read the rows under the sub-header until the "Total" row
            int lastRow = sheet.LastRowUsed().RowNumber();

            for (int r = subRow + 1; r <= lastRow; r++)
            {
                if (sheet.Cell(r, 1).GetString().Trim().Equals("Total", StringComparison.OrdinalIgnoreCase))
                    break;

                decimal rowRate = ReadExcelDecimal(sheet.Cell(r, rateCol));
                if (rowRate > 0m && rowRate < 1m) rowRate *= 100m;   // 0.09 stored as a percent value

                if (rate == 0m && rowRate > 0m) rate = rowRate;
                amount += ReadExcelDecimal(sheet.Cell(r, amountCol));
            }
        }

        private DateTime ReadExcelDate(ClosedXML.Excel.IXLCell cell)
        {
            if (cell.TryGetValue<DateTime>(out DateTime date)) return date;

            string value = cell.GetString().Trim();
            string[] formats = { "dd-MMM-yy", "dd-MMM-yyyy", "dd/MM/yyyy", "dd-MM-yyyy" };

            if (DateTime.TryParseExact(value, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                return date;

            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                return date;

            throw new Exception("Invalid Excel invoice date: " + value);
        }

        private decimal ReadExcelDecimal(ClosedXML.Excel.IXLCell cell)
        {
            if (cell.TryGetValue<decimal>(out decimal number)) return number;

            string value = cell.GetString().Trim().Replace(",", "").Replace("%", "");
            return decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out number) ? number : 0m;
        }

        private string ReadBelowLabel(ClosedXML.Excel.IXLWorksheet sheet, string label)
        {
            foreach (var cell in sheet.CellsUsed())
            {
                if (cell.GetString().Trim().Equals(label, StringComparison.OrdinalIgnoreCase))
                    return sheet.Cell(cell.Address.RowNumber + 1, cell.Address.ColumnNumber).GetString().Trim();
            }
            return "";
        }

        private string ReadExcelInvoiceNumber(string excelPath)
        {
            using (ClosedXML.Excel.XLWorkbook workbook = new ClosedXML.Excel.XLWorkbook(excelPath))
                return workbook.Worksheet(1).Cell("G4").GetString().Trim();
        }

        public string Template(string invoiceno, int copyNo, string gstin)
        {
            try
            {
                string[] pc = new string[5];
                //create a new data set.
                this.DsReport = new DataSet();
                //create a new table with two columns and add the table to the dataset
                System.Data.DataTable InvoiceTable = new System.Data.DataTable("mul_INVOICE_hmi");

                InvoiceTable.Columns.Add(new DataColumn("INV_NO_1", typeof(string)));        //InvNo 
                InvoiceTable.Columns.Add(new DataColumn("INV_NO_2", typeof(string)));       //InvoiceDate

                InvoiceTable.Columns.Add(new DataColumn("bar_code_image1", typeof(System.Byte[]))); // SignedQrCode

                InvoiceTable.Columns.Add(new DataColumn("INV_NO_3", typeof(string)));       // serial no
                InvoiceTable.Columns.Add(new DataColumn("INV_NO_4", typeof(string)));       //shop code 
                InvoiceTable.Columns.Add(new DataColumn("INVOICE_DATE", typeof(string)));    // pono
                InvoiceTable.Columns.Add(new DataColumn("COMP_NAME", typeof(string)));       // podate

                InvoiceTable.Columns.Add(new DataColumn("COMP_ID", typeof(string)));       // part no
                InvoiceTable.Columns.Add(new DataColumn("COMP_ADDR", typeof(string)));     // part name  
                InvoiceTable.Columns.Add(new DataColumn("PO_NO", typeof(string)));         // unit
                InvoiceTable.Columns.Add(new DataColumn("ECC_NO", typeof(string)));        // qty

                InvoiceTable.Columns.Add(new DataColumn("RANGE_OFF", typeof(string)));       // unit price
                InvoiceTable.Columns.Add(new DataColumn("DIVISION_OFF", typeof(string)));     //tool cost
                InvoiceTable.Columns.Add(new DataColumn("TIN_NO", typeof(string)));          //total amount

                InvoiceTable.Columns.Add(new DataColumn("CST_NO", typeof(string)));         // Hsn code
                InvoiceTable.Columns.Add(new DataColumn("TNGST_NO", typeof(string)));       // Mde/terms of payment 

                InvoiceTable.Columns.Add(new DataColumn("PART_NO", typeof(string)));        // Shipping
                InvoiceTable.Columns.Add(new DataColumn("PART_NAME", typeof(string)));
                InvoiceTable.Columns.Add(new DataColumn("QTY", typeof(string)));
                InvoiceTable.Columns.Add(new DataColumn("RATE", typeof(string)));
                InvoiceTable.Columns.Add(new DataColumn("TOTAL_PRICE", typeof(string)));

                InvoiceTable.Columns.Add(new DataColumn("DETAIL_DEDUCTION", typeof(string)));
                InvoiceTable.Columns.Add(new DataColumn("ASSESSABLE_VALUE", typeof(string)));    // Multipart Details
                InvoiceTable.Columns.Add(new DataColumn("CHAPTER", typeof(string)));

                InvoiceTable.Columns.Add(new DataColumn("DESCRIPTION", typeof(string)));
                InvoiceTable.Columns.Add(new DataColumn("TARIFF_HEAD", typeof(string)));
                InvoiceTable.Columns.Add(new DataColumn("TOTAL_ASSESABLE_VALUE", typeof(string)));

                InvoiceTable.Columns.Add(new DataColumn("CESS_EXCISE", typeof(string)));
                InvoiceTable.Columns.Add(new DataColumn("SH_EDU_CESS", typeof(string)));
                InvoiceTable.Columns.Add(new DataColumn("ADD_DUTY", typeof(string)));
                InvoiceTable.Columns.Add(new DataColumn("RAW_MET_COST", typeof(string)));

                InvoiceTable.Columns.Add(new DataColumn("AMORTISATIONCOST10", typeof(string)));   // Discount Details
                InvoiceTable.Columns.Add(new DataColumn("AMORTISATIONCOST9", typeof(string)));

                InvoiceTable.Columns.Add(new DataColumn("AMORTISATIONCOST8", typeof(string)));
                InvoiceTable.Columns.Add(new DataColumn("AMORTISATIONCOST7", typeof(string)));
                InvoiceTable.Columns.Add(new DataColumn("AMORTISATIONCOST6", typeof(string)));
                InvoiceTable.Columns.Add(new DataColumn("AMORTISATIONCOST5", typeof(string)));

                InvoiceTable.Columns.Add(new DataColumn("bar_code_image2", typeof(System.Byte[]))); // HMI QR Code

                this.DsReport.Tables.Add(InvoiceTable);

                string InvoiceNo = "";
                string InvoiceDate = "";
                bool _bBarcode_IRP = true;

                string ShopCode = ""; string PONO = ""; string POMonth = "";
                string PartNo = ""; string PartName = "";
                string Qty = ""; string UnitPrice = ""; string ToolCost = "";

                string TotalAmount = "";

                string HsnCode = ""; string ModeTermsOfPayment = ""; string UOM = "";
                string TermsOfDelivery = ""; string VehicleNo = ""; string TransportName = "";
                decimal TotalAssessableValue = 0;
                decimal AmortCost = 0; decimal SubTotal = 0;

                string SGST_Percen = ""; decimal SGST_Amount = 0;
                string CGST_Percen = ""; decimal CGST_Amount = 0;
                string IGST_Percen = ""; decimal IGST_Amount = 0;
                decimal TCS_Amount = 0; decimal TotalTaxValue = 0; decimal GrandTotal = 0;

                string IRNNo = ""; string EwayBilNo = ""; string AckNo = ""; string AckDate = ""; string barcode_value = "";

                DataRow dr = this.DsReport.Tables["mul_INVOICE_hmi"].NewRow();

                byte[] qrImageBytes = null;

                string qry = "Select [InvDate], [EInvoiceQRCode], [TarrifNumber], [Mode], [Terms], [VehicleNumber]," +
                             " [Dispatched], [IRN], [Ewaybill_no], [AckNo], [AckDate] From [Invoice] Where [InvNumber]='" + invoiceno.Trim() + "' ";

                SqlDataReader rdr = Utilities.executeQuery(qry);
                if (rdr.Read())
                {
                    InvoiceNo = invoiceno.ToString().Trim();
                    InvoiceDate = rdr["InvDate"] != DBNull.Value ? Convert.ToDateTime(rdr["InvDate"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "";

                    qrImageBytes = rdr["EInvoiceQRCode"] != DBNull.Value ? (byte[])rdr["EInvoiceQRCode"] : null;

                    HsnCode = rdr["TarrifNumber"].ToString();
                    ModeTermsOfPayment = rdr["Mode"].ToString();
                    TermsOfDelivery = rdr["Terms"].ToString();
                    VehicleNo = rdr["VehicleNumber"].ToString();
                    TransportName = rdr["Dispatched"].ToString();
                    IRNNo = rdr["IRN"].ToString();
                    EwayBilNo = rdr["Ewaybill_no"].ToString();
                    AckNo = rdr["AckNo"].ToString();
                    AckDate = rdr["AckDate"] != DBNull.Value ? Convert.ToDateTime(rdr["AckDate"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "";
                }
                rdr.Close();

                //// TEST: Save the QR image retrieved from the database
                //if (qrImageBytes != null && qrImageBytes.Length > 0)
                //{
                //    string testPath = @"D:\Temp\InvoiceQRCode.png";
                //    Directory.CreateDirectory(@"D:\Temp");
                //    File.WriteAllBytes(testPath, qrImageBytes);
                //}

                #region IRP QR Code
                if (_bBarcode_IRP && qrImageBytes != null && qrImageBytes.Length > 0)
                {
                    dr["bar_code_image1"] = qrImageBytes;
                }
                else
                {
                    dr["bar_code_image1"] = DBNull.Value;
                }
                #endregion

                string TotalTaxValueInWords = "Zero Rupees Only";
                string GrandTotalInWords = "Zero Rupees Only";

                string calqry = "SELECT SUM(TRY_CONVERT(DECIMAL(18,2), AssessableValue)) AS TotalAssessableValue, " +
                                "SUM(TRY_CONVERT(DECIMAL(18,2), SGST)) AS SGSTAMOUNT, " +
                                "SUM(TRY_CONVERT(DECIMAL(18,2), CGST)) AS CGSTAMOUNT, " +
                                "SUM(TRY_CONVERT(DECIMAL(18,2), IGST)) AS IGSTAMOUNT " +
                                "FROM [Invoice] WHERE [InvNumber] = '" + invoiceno.Trim().Replace("'", "''") + "'";

                SqlDataReader calrdr = Utilities.executeQuery(calqry);
                if (calrdr != null)
                {
                    using (calrdr)
                    {
                        if (calrdr.Read())
                        {
                            TotalAssessableValue = calrdr["TotalAssessableValue"] == DBNull.Value ? 0m : Convert.ToDecimal(calrdr["TotalAssessableValue"]);
                            SubTotal = TotalAssessableValue;
                            SGST_Amount = calrdr["SGSTAMOUNT"] == DBNull.Value ? 0m : Convert.ToDecimal(calrdr["SGSTAMOUNT"]);
                            CGST_Amount = calrdr["CGSTAMOUNT"] == DBNull.Value ? 0m : Convert.ToDecimal(calrdr["CGSTAMOUNT"]);
                            IGST_Amount = calrdr["IGSTAMOUNT"] == DBNull.Value ? 0m : Convert.ToDecimal(calrdr["IGSTAMOUNT"]);

                            TotalTaxValue = SGST_Amount + CGST_Amount + IGST_Amount;
                            GrandTotal = TotalAssessableValue + TotalTaxValue;

                            TotalTaxValueInWords = AmountInWords(TotalTaxValue);
                            GrandTotalInWords = AmountInWords(GrandTotal);
                        }
                    }
                }

                int serialNumber = 1;
                string barcodeFolder = @"C:\SOMS\BARCODES";
                byte[] barcodeQrBytes = null;

                Directory.CreateDirectory(barcodeFolder);
                string getqry = "SELECT [TarrifNumber], [ShopCode], [PONumber], [PODATE], [PartNumber], [PartName], [UOM], [InvQuantity], [UnitPrice], " +
                   "[ToolCost], [MaterialCost], [AssessableValue], [SGSTper], [CGSTper], [IGSTper], [AMORTCOST], [TCSper], " +
                   "[InvDate], [InvValue], [BedAmount], [SGST], [IGST], [TCS], [CGST], [ConsigneePartCost], [ExciseDutyCost], " +
                   "[CSTAmount], [ConsigneeMatlCost], [IRN] FROM [Invoice] WHERE [InvNumber] = '" + invoiceno.Trim().Replace("'", "''") + "'";

                SqlDataReader rdr_mul = Utilities.executeQuery(getqry);
                while (rdr_mul.Read())
                {
                    int serialno = serialNumber++;

                    ShopCode = rdr_mul["ShopCode"].ToString();
                    PONO = rdr_mul["PONumber"].ToString();
                    POMonth = rdr_mul["PODATE"] != DBNull.Value
                              ? Convert.ToDateTime(rdr_mul["PODATE"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "";
                    PartNo = rdr_mul["PartNumber"].ToString();
                    PartName = rdr_mul["PartName"].ToString();
                    UOM = rdr_mul["UOM"].ToString();
                    HsnCode = rdr_mul["TarrifNumber"].ToString();
                    Qty = rdr_mul["InvQuantity"].ToString();
                    UnitPrice = rdr_mul["UnitPrice"].ToString();
                    ToolCost = rdr_mul["ToolCost"].ToString();
                    TotalAmount = rdr_mul["AssessableValue"].ToString();

                    SGST_Percen = rdr_mul["SGSTper"].ToString() + "%";
                    CGST_Percen = rdr_mul["CGSTper"].ToString() + "%";
                    IGST_Percen = rdr_mul["IGSTper"].ToString() + "%";
                    AmortCost = rdr_mul["AMORTCOST"] == DBNull.Value ? 0m : Convert.ToDecimal(rdr_mul["AMORTCOST"]);
                    TCS_Amount = rdr_mul["TCSper"] == DBNull.Value ? 0m : Convert.ToDecimal(rdr_mul["TCSper"]);

                    barcode_value += string.Format("{0,2}", rdr_mul["ShopCode"].ToString()) + string.Format("{0,10}", rdr_mul["PONumber"].ToString()) + rdr_mul["PartNumber"].ToString() + Environment.NewLine
                                  + invoiceno + "\t" + Convert.ToDateTime(rdr_mul["InvDate"]).ToString("ddMMyyyy") + rdr_mul["InvQuantity"].ToString() + "\t" 
                                  + rdr_mul["InvValue"].ToString() + "\t" + string.Format("{0,10}", rdr_mul["TarrifNumber"].ToString()) + rdr_mul["BedAmount"].ToString() + "\t"
                                  + rdr_mul["SGST"].ToString() + "\t" + rdr_mul["IGST"].ToString() + "\t" + rdr_mul["TCS"].ToString() + "\t" 
                                  + Convert.ToDecimal(rdr_mul["UnitPrice"]).ToString("0.00") + "\t" + rdr_mul["MaterialCost"].ToString() + "\t" 
                                  + rdr_mul["CGST"].ToString() + "\t" + Convert.ToDecimal(rdr_mul["ConsigneePartCost"]).ToString("0.00") + "\t" + rdr_mul["ExciseDutyCost"].ToString() + "\t"
                                  + rdr_mul["AssessableValue"].ToString() + "\t" + Convert.ToDecimal(rdr_mul["CSTAmount"]).ToString("0.00") + "\t"
                                  + Convert.ToDecimal(rdr_mul["ToolCost"]).ToString("0.00") + "\t" + Convert.ToDecimal(rdr_mul["ConsigneeMatlCost"]).ToString("0.00") + "\t" + string.Format("{0,15}", gstin) + "\t0\t"
                                 + (serialno == 1 ? rdr_mul["IRN"].ToString().Trim() : "") + Environment.NewLine;

                   
                    
                    InvoiceTable.Rows.Add(InvoiceNo.ToString(), InvoiceDate, qrImageBytes, serialno, ShopCode, PONO, POMonth,
                        PartNo, PartName, UOM, Qty, UnitPrice, ToolCost, TotalAmount, HsnCode, ModeTermsOfPayment, TermsOfDelivery, VehicleNo,
                        TransportName, TotalTaxValueInWords, GrandTotalInWords, TotalAssessableValue, AmortCost, SubTotal, SGST_Percen, CGST_Percen, IGST_Percen,
                        SGST_Amount, CGST_Amount, IGST_Amount, TCS_Amount, TotalTaxValue, GrandTotal,
                        IRNNo, EwayBilNo, AckNo, AckDate , barcodeQrBytes);

                }
                rdr_mul.Close();

                if (!string.IsNullOrWhiteSpace(barcode_value) && InvoiceTable.Rows.Count > 0)
                {
                    string qrPath = System.IO.Path.Combine(barcodeFolder, "qrcode_" + invoiceno + ".bmp");

                    QRCode qrcode = new QRCode();
                    qrcode.Data = barcode_value.TrimEnd('\r', '\n');
                    qrcode.ProcessTilde = false;
                    qrcode.FNC1 = FNC1.FNC1_NONE;
                    qrcode.ECL = OnBarcode.Barcode.QRCodeECL.L;
                    qrcode.DataMode = OnBarcode.Barcode.QRCodeDataMode.Auto;
                    qrcode.UOM = UnitOfMeasure.PIXEL;
                    qrcode.X = 3;
                    qrcode.LeftMargin = 2;
                    qrcode.RightMargin = 2;
                    qrcode.TopMargin = 2;
                    qrcode.BottomMargin = 2;
                    qrcode.Resolution = 72;
                    qrcode.Rotate = Rotate.Rotate0;
                    qrcode.ImageFormat = System.Drawing.Imaging.ImageFormat.Bmp;

                    qrcode.drawBarcode(qrPath);
                    barcodeQrBytes = File.ReadAllBytes(qrPath);

                    InvoiceTable.Rows[0]["bar_code_image2"] = barcodeQrBytes;
                }

                ReportViewer rv = new ReportViewer();
                ReportDataSource datasource = new ReportDataSource("MUL_INV_HMI", this.DsReport.Tables[0].Copy());

                string reportPath = reportPath = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "Account", "Invoice.rdlc");

                rv.LocalReport.ReportPath = reportPath;
                rv.LocalReport.DataSources.Clear();
                rv.LocalReport.DataSources.Add(datasource);
                rv.RefreshReport();

                byte[] bytes = rv.LocalReport.Render("PDF");

                string invno = InvoiceNo.Trim();

                // Date in ddMMyyyy format
                string todayDate = DateTime.Now.ToString("ddMMyyyy");

                string fileName = invno + "_" + todayDate + "(" + copyNo + ").pdf";

                string folderPath = System.IO.Path.Combine(Global.InvoicePath, "INVOICE");
                Directory.CreateDirectory(folderPath);

                string fullPath = System.IO.Path.Combine(folderPath, fileName);
                File.WriteAllBytes(fullPath, bytes);

                string inputFolder = Global.InputPath;
                Directory.CreateDirectory(inputFolder);

                for (int i = 1; i <= 4; i++)
                {
                    string copyType = i == 1 ? "ORIGINAL" : i == 2 ? "DUPLICATE" : i == 3 ? "TRIPLICATE" : "EXTRA";
                    string inputFileName = invno.ToString().Trim() + copyType + "invoice" + ".pdf";
                    string inputFullPath = System.IO.Path.Combine(inputFolder, inputFileName);

                    File.Copy(fullPath, inputFullPath, true);
                }

                string processedFolder = System.IO.Path.Combine(folderPath, "PROCESSED");
                Directory.CreateDirectory(processedFolder);

                string processedFile = System.IO.Path.Combine(processedFolder, fileName);
                if (File.Exists(processedFile)) File.Delete(processedFile);
                File.Move(fullPath, processedFile);

                return processedFile;
            }
            catch (Exception ex)
            {
                LogErrors(ex);
                return "";
            }
        }

        private string AmountInWords(decimal amount)
        {
            if (amount < 0) return "Minus " + AmountInWords(Math.Abs(amount));

            string[] units = { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
            string[] tens = { "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

            string Words(long number)
            {
                if (number < 20) return units[number];
                if (number < 100) return tens[number / 10] + (number % 10 > 0 ? " " + units[number % 10] : "");
                if (number < 1000) return units[number / 100] + " Hundred" + (number % 100 > 0 ? " " + Words(number % 100) : "");
                if (number < 100000) return Words(number / 1000) + " Thousand" + (number % 1000 > 0 ? " " + Words(number % 1000) : "");
                if (number < 10000000) return Words(number / 100000) + " Lakh" + (number % 100000 > 0 ? " " + Words(number % 100000) : "");
                return Words(number / 10000000) + " Crore" + (number % 10000000 > 0 ? " " + Words(number % 10000000) : "");
            }

            long rupees = (long)Math.Floor(amount);
            int paise = (int)Math.Round((amount - rupees) * 100, MidpointRounding.AwayFromZero);

            if (paise == 100)
            {
                rupees++;
                paise = 0;
            }

            string result = Words(rupees) + " Rupees";
            if (paise > 0) result += " and " + Words(paise) + " Paise";

            return result + " Only";
        }

        private string ReadInvoiceNumberFromPDF(string pdfPath)
        {
            try
            {
                var doc = new Spire.Pdf.PdfDocument();
                doc.LoadFromFile(pdfPath);
                try
                {
                    using (System.Drawing.Image img = doc.SaveAsImage(0, Spire.Pdf.Graphics.PdfImageType.Bitmap, 300, 300))
                    using (var page = new System.Drawing.Bitmap(img))
                    {
                        // 1. e-Invoice QR code: exact invoice no, no OCR mistakes
                        string no = InvoiceNoFromQr(page);
                        if (!string.IsNullOrEmpty(no))
                        {
                            LogErrors(new Exception("Invoice no from QR: " + no));
                            return no;
                        }

                        // 2. backup: OCR of the header area
                        no = InvoiceNoFromOcr(page);
                        LogErrors(new Exception("Invoice no from OCR: " + (no ?? "(not found)")));
                        return no;
                    }
                }
                finally { doc.Close(); }
            }
            catch (Exception ex)
            {
                LogErrors(ex);
            }
            return null;
        }

        public static string NumberToWords(long number)  // Added by Meeena
        {
            if (number == 0)
                return "Zero";

            string words = "";

            if ((number / 10000000) > 0)
            {
                words += NumberToWords(number / 10000000) + " Crore ";
                number %= 10000000;
            }

            if ((number / 100000) > 0)
            {
                words += NumberToWords(number / 100000) + " Lakh ";
                number %= 100000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " Thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " Hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                string[] unitsMap = { "Zero","One","Two","Three","Four","Five","Six","Seven","Eight","Nine","Ten",
                              "Eleven","Twelve","Thirteen","Fourteen","Fifteen","Sixteen","Seventeen",
                              "Eighteen","Nineteen" };

                string[] tensMap = { "Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += " " + unitsMap[number % 10];
                }
            }

            return words.Trim();
        }

        private string InvoiceNoFromQr(System.Drawing.Bitmap page)
        {
            // the QR is at the top-right of page 1; try that area first, then the whole page
            var areas = new[]
            {
                new System.Drawing.Rectangle(page.Width / 2, 0, page.Width / 2, page.Height / 3),
                new System.Drawing.Rectangle(0, 0, page.Width, page.Height)
            };

            foreach (var area in areas)
            {
                using (var crop = page.Clone(area, page.PixelFormat))
                {
                    string token = null;

                    try
                    {
                        var reader = new ZXing.BarcodeReader
                        {
                            AutoRotate = true,
                            TryInverted = true,
                            Options = new ZXing.Common.DecodingOptions
                            {
                                TryHarder = true,
                                PossibleFormats = new List<ZXing.BarcodeFormat> { ZXing.BarcodeFormat.QR_CODE }
                            }
                        };
                        ZXing.Result r = reader.Decode(crop);
                        if (r != null) token = r.Text;
                    }
                    catch { }

                    if (string.IsNullOrEmpty(token))
                    {
                        try { token = Spire.Barcode.BarcodeScanner.ScanOne(crop); } catch { }
                    }

                    string no = DocNoFromToken(token);
                    if (no != null) return no;
                }
            }
            return null;
        }

        private string DocNoFromToken(string token)
        {
            if (string.IsNullOrEmpty(token) || !token.StartsWith("eyJ")) return null;

            string[] parts = token.Split('.');
            if (parts.Length != 3) return null;

            string s = parts[1].Replace('-', '+').Replace('_', '/');
            switch (s.Length % 4) { case 2: s += "=="; break; case 3: s += "="; break; }

            string payload = Encoding.UTF8.GetString(Convert.FromBase64String(s));
            Match m = Regex.Match(payload, @"\\?""DocNo\\?""\s*:\s*\\?""([^""\\]+)");
            return m.Success ? m.Groups[1].Value.Trim() : null;
        }

        // ---------- OCR backup ----------
        private string InvoiceNoFromOcr(System.Drawing.Bitmap page)
        {
            string tessDataPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata");

            var rect = new System.Drawing.Rectangle(0, 0, page.Width, (int)(page.Height * 0.40));   // header area
            using (var top = page.Clone(rect, page.PixelFormat))
            using (var ms = new MemoryStream())
            using (var engine = new Tesseract.TesseractEngine(tessDataPath, "eng", Tesseract.EngineMode.Default))
            {
                top.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                byte[] png = ms.ToArray();

                foreach (var mode in new[] { Tesseract.PageSegMode.Auto, Tesseract.PageSegMode.SparseText })
                {
                    string text;
                    using (var pix = Tesseract.Pix.LoadFromMemory(png))
                    using (var result = engine.Process(pix, mode))
                        text = result.GetText() ?? "";

                    Match m = Regex.Match(text, @"(?i)\bJ\s*G[\s\-:]*([0-9OIL](?:[\s\-]*[0-9OIL]){9})");
                    if (m.Success)
                    {
                        string digits = Regex.Replace(m.Groups[1].Value, @"[\s\-]", "")
                                             .ToUpperInvariant()
                                             .Replace('O', '0').Replace('I', '1').Replace('L', '1');
                        return "JG" + digits;
                    }
                }
            }
            return null;
        }

        public void AssignInvoiceNo_TruckNo(string src, string type, string invoiceNo, DateTime invoiceDate, string customerName, string gstin, string poNumber, string shopCode, string vehicleNo, string vendorCode)
        {
            string pathToPdf = src;

            try
            {
                // Use invoice details obtained from Excel.
                Global.InvoiceNumber = invoiceNo;
                Global.InvoiceDate = invoiceDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                Global.CustomerName = customerName.Trim();
                Global.VendorCode = vendorCode.Trim();
                Global.Shopcode = shopCode.Trim();
                Global.TruckNumber = vehicleNo.Trim();

                // Load customers and validate the Excel customer name.
                Customers = new ObservableCollection<CustomerDisplayModel>(objDSMModelData.GetAllCustomers());

                Global.CustomerId = "";
                Global.Templatename = "";

                var matchedCustomer = Customers.FirstOrDefault(c =>
                    !string.IsNullOrWhiteSpace(c.CustomerName) &&
                    c.CustomerName.Trim().Equals(customerName.Trim(), StringComparison.OrdinalIgnoreCase));

                if (matchedCustomer == null)
                {
                    MoveFile(pathToPdf, "OTHER INVOICE");
                    othercustomer("Other Customer Invoice");
                    oth_cus = true;
                    return;
                }

                Global.CustomerName = matchedCustomer.CustomerName.Trim();
                Global.CustomerId = matchedCustomer.CustomerId.ToString().Trim();

                if (!Global.CustomerName.ToUpperInvariant().Contains("HYUNDAI"))
                {
                    MoveFile(pathToPdf, "OTHER INVOICE");
                    othercustomer("Other Customer Invoice");
                    oth_cus = true;
                    return;
                }

                // Load system settings.
                SettingsDisplayModel objSettings = objDSMModelData.GetSettingsData(Environment.MachineName);

                if (objSettings != null)
                {
                    Global.SystemName = objSettings.SystemName;
                    Global.InputPath = objSettings.InputPath;
                    Global.InvoicePath = objSettings.InvoicePath;
                    Global.OutputPath = objSettings.OutputPath;
                    Global.PrinterName = objSettings.PrinterName;
                }

                // Load customer configuration.
                CustomerDisplayModel objCustomer = objDSMModelData.GetCustomerByName(Global.CustomerName);

                if (objCustomer != null)
                {
                    // Keep the vendor code from Excel.
                    Global.PDFtype = objCustomer.PDFtype == true ? "SINGLE" : "MULTIPLE";
                    Global.PrintType = objCustomer.PrintType == true ? "ORIGINAL" : "SELECTED";
                    Global.PrintCopies = objCustomer.PrintCopies;

                    if (objCustomer.IsAPIpost)
                    {
                        Global.IsAPIpost = true;
                        Global.APIpostType = objCustomer.APItype == true ? "AUTO" : "MANUAL";
                        Global.APIurl = objCustomer.APIUrl;
                        Global.APIPosturl = "http://hmieai/DI/data";
                    }
                    else
                    {
                        Global.IsAPIpost = false;
                    }
                }

                // Retain template lookup for other existing functionality.
                if (!string.IsNullOrWhiteSpace(Global.CustomerId))
                {
                    var dsDataDetail = objDSMModelData.GetDSdetailbyClientId(Global.CustomerId);

                    var template = objDSMModelData.GetTemplates(Global.CustomerId);

                    if (template != null && template.Count > 0)
                    {
                        Global.Templatename = template[0].Tempname.ToString().Trim();

                        var coordinates = objDSMModelData.GetTemplatesCord(Global.Templatename, Global.CustomerId);

                        // Coordinates are no longer used to extract invoice number,
                        // invoice date, vendor code, truck number, or shop code.
                        // Those values are taken from Excel.
                    }
                }

                // Excel GSTIN and PO number are available for the remaining
                // invoice-processing logic.
                // Use the values directly where required:
                // gstin, poNumber, Global.InvoiceNumber, Global.InvoiceDate,
                // Global.CustomerName, Global.VendorCode, Global.Shopcode,
                // Global.TruckNumber.
            }
            catch (Exception ex)
            {
                LogErrors(ex);
                MoveFile(pathToPdf, "INVALID_INVOICE");
                timer.Start();
            }
        }

        private void CreateInvoiceFromExcel(string excelPath, string pdfPath)
        {
            try
            {
                _expectedInvoiceNo = ReadExcelInvoiceNumber(excelPath);
                _ewayBillNo = null;

                ExtractEInvoiceText(pdfPath, out string irn, out string ackNo, out DateTime? ackDate);

                _qrIrn = null;
                byte[] qrCodeBytes = ExtractEInvoiceQRCode(pdfPath, irn);

                if (string.IsNullOrWhiteSpace(_qrIrn) || !Regex.IsMatch(_qrIrn, @"^[A-F0-9]{64}$"))
                    throw new Exception("A valid IRN could not be extracted from the QR code.");

                irn = _qrIrn;
                LogErrors(new Exception("Final IRN used for database insertion: " + irn));

                using (ClosedXML.Excel.XLWorkbook workbook = new ClosedXML.Excel.XLWorkbook(excelPath))
                {
                    var sheet = workbook.Worksheet(1);
                    lstInvoice = new List<InvoiceDisplayModel>();
                    objDSMModelData = new DSMModelData();

                    string invoiceNo = sheet.Cell("G4").GetString().Trim();
                    DateTime invoiceDate = ReadExcelDate(sheet.Cell("J4"));
                    string customerName = sheet.Cell("A10").GetString().Trim();
                    string gstin = sheet.Cell("D14").GetString().Trim();
                    string poNumber = sheet.Cell("G10").GetString().Trim();
                    DateTime poDate = ReadExcelDate(sheet.Cell("J10"));
                    string shopCode = sheet.Cell("J14").GetString().Trim();
                    string vehicleNo = sheet.Cell("J16").GetString().Trim();
                    string vendorCode = "TNGV";

                    string mode = ReadBelowLabel(sheet, "Mode/Terms of Payment");
                    string terms = ReadBelowLabel(sheet, "Terms of Delivery");
                    string dispatched = ReadBelowLabel(sheet, "Dispatched through");

                    // e-Way Bill: PDF first, Excel as backup                                          // NEW
                    string excelEway = Regex.Replace(ReadBelowLabel(sheet, "e-Way Bill No."), @"\D", "");   // NEW
                    string ewayBill = !string.IsNullOrEmpty(_ewayBillNo) ? _ewayBillNo : excelEway;          // NEW

                    if (!string.IsNullOrEmpty(_ewayBillNo) && !string.IsNullOrEmpty(excelEway) && _ewayBillNo != excelEway)   // NEW
                        LogErrors(new Exception("e-Way Bill mismatch. PDF: " + _ewayBillNo + " | Excel: " + excelEway));      // NEW

                    if (string.IsNullOrWhiteSpace(invoiceNo))
                        throw new Exception("Invoice number is missing in Excel.");

                    // Tax rate and amount from the tax table (header -> Rate / Amount columns)
                    ReadTaxFromHeader(sheet, new[] { "CGST" }, out decimal cgstRate, out decimal excelCgstTotal);
                    ReadTaxFromHeader(sheet, new[] { "SGST/UTGST", "SGST" }, out decimal sgstRate, out decimal excelSgstTotal);
                    ReadTaxFromHeader(sheet, new[] { "IGST" }, out decimal igstRate, out decimal excelIgstTotal);

                    string tcsValue = ReadTcsFromExcel(sheet);

                    //int[] itemRows = { 25, 27, 29, 31, 33 };

                    List<int> itemRows = FindItemRows(sheet);

                    if (itemRows.Count == 0)
                        throw new Exception("No line items found in the Excel.");

                    if (itemRows.Count > 8)
                        throw new Exception("Excel has " + itemRows.Count + " line items. Maximum allowed is 8.");

                    foreach (int row in itemRows)
                    {
                        string partName = sheet.Cell(row, 2).GetString().Trim();   // B - Description of Goods
                        string partNo = sheet.Cell(row, 10).GetString().Trim();    // J - Part No.
                        decimal quantity = ReadExcelDecimal(sheet.Cell(row, 11));  // K - Quantity
                        decimal unitPrice = ReadExcelDecimal(sheet.Cell(row, 12)); // L - Rate
                        string uom = sheet.Cell(row, 13).GetString().Trim();       // M - per
                        decimal excelAmount = ReadExcelDecimal(sheet.Cell(row, 14)); // N - Amount
                        string hsn = sheet.Cell(row, 9).GetString().Trim();        // I - HSN/SAC

                        if (quantity <= 0 && excelAmount <= 0) continue;

                        if (string.IsNullOrWhiteSpace(partName))
                            throw new Exception("Part Name is missing in Excel at row " + row + ".");

                        if (string.IsNullOrWhiteSpace(partNo))
                            throw new Exception("Part Number is missing in Excel at row " + row + ".");

                        decimal amount = Math.Round(quantity * unitPrice, 2, MidpointRounding.AwayFromZero);

                        if (excelAmount > 0 && Math.Abs(excelAmount - amount) > 0.02m)
                            throw new Exception("Quantity × Rate does not match Excel Amount on row " + row + ".");

                        decimal cgst = Math.Round(amount * cgstRate / 100m, 2, MidpointRounding.AwayFromZero);
                        decimal sgst = Math.Round(amount * sgstRate / 100m, 2, MidpointRounding.AwayFromZero);
                        decimal igst = Math.Round(amount * igstRate / 100m, 2, MidpointRounding.AwayFromZero);
                        decimal invValue = amount + cgst + sgst + igst;

                        InvoiceDisplayModel item = new InvoiceDisplayModel();
                        item.InvNumber = invoiceNo;
                        item.InvDate = invoiceDate;
                        item.CustomerName = string.IsNullOrWhiteSpace(customerName) ? Global.CustomerName : customerName;
                        item.VehicleNumber = string.IsNullOrWhiteSpace(vehicleNo) ? Global.TruckNumber : vehicleNo;
                        item.VendorCode = string.IsNullOrWhiteSpace(vendorCode) ? Global.VendorCode : vendorCode;
                        item.ShopCode = string.IsNullOrWhiteSpace(shopCode) ? Global.Shopcode : shopCode;
                        item.PONumber = poNumber;
                        item.PODATE = poDate;

                        item.PartNumber = partNo;
                        item.PartName = partName; // Add this property to InvoiceDisplayModel if it does not exist.

                        item.TarrifNumber = hsn;
                        item.UOM = uom;
                        item.Mode = mode;
                        item.Terms = terms;
                        item.Dispatched = dispatched;
                        item.InvQuantity = quantity.ToString(CultureInfo.InvariantCulture);
                        item.UnitPrice = unitPrice.ToString("0.00", CultureInfo.InvariantCulture);
                        item.MaterialCost = amount.ToString("0.00", CultureInfo.InvariantCulture);
                        item.AssessableValue = amount.ToString("0.00", CultureInfo.InvariantCulture);
                        item.CGST = cgst.ToString("0.00", CultureInfo.InvariantCulture);
                        item.SGST = sgst.ToString("0.00", CultureInfo.InvariantCulture);
                        item.IGST = igst.ToString("0.00", CultureInfo.InvariantCulture);
                        item.CGSTper = cgstRate.ToString("0.00", CultureInfo.InvariantCulture);
                        item.SGSTper = sgstRate.ToString("0.00", CultureInfo.InvariantCulture);
                        item.IGSTper = igstRate.ToString("0.00", CultureInfo.InvariantCulture);
                        item.TCSper = tcsValue;
                        item.InvValue = invValue.ToString("0.00", CultureInfo.InvariantCulture);
                        item.GSTN = gstin;
                        item.CreatedBy = Global.UserId;
                        item.Finyear = Global.FinYear;
                        item.SystemName = Environment.MachineName;
                        item.OriginalPDF = Global.OriginalPDFFile;
                        item.TCS = tcsValue;
                        item.Ewaybill_no = string.IsNullOrWhiteSpace(ewayBill) ? "0" : ewayBill;
                        item.Lot_Code = "0";
                        item.EXTRA_NUM_1 = "0";
                        item.EXTRA_NUM_2 = "0";
                        item.EXTRA_NUM_3 = "0";
                        item.EXTRA_NUM_4 = "0";
                        item.EXTRA_NUM_5 = "0";
                        item.EXTRA_NUM_6 = "0";
                        item.EXTRA_NUM_7 = "0";
                        item.EXTRA_CHAR_1 = "";
                        item.EXTRA_CHAR_2 = "";
                        item.EXTRA_CHAR_3 = "";
                        item.EXTRA_CHAR_4 = "";
                        item.EXTRA_CHAR_5 = "";
                        item.Date1 = invoiceDate;
                        item.Date2 = invoiceDate;
                        item.Date3 = invoiceDate;
                        item.Date4 = invoiceDate;
                        item.Date5 = invoiceDate;
                        item.Manfac_Date = invoiceDate;
                        item.PdfGeneration = "";
                        item.IRN = irn;
                        item.AckNo = ackNo;
                        item.AckDate = ackDate;
                        item.EInvoiceQRCode = qrCodeBytes;

                        lstInvoice.Add(item);
                    }

                    if (lstInvoice.Count == 0)
                        throw new Exception("No valid invoice item rows were found in Excel.");

                    // Check calculated tax against the Excel tax table
                    decimal sumCgst = lstInvoice.Sum(i => decimal.Parse(i.CGST, CultureInfo.InvariantCulture));
                    decimal sumSgst = lstInvoice.Sum(i => decimal.Parse(i.SGST, CultureInfo.InvariantCulture));

                    if (Math.Abs(sumCgst - excelCgstTotal) > 0.05m)
                        throw new Exception("CGST mismatch. Calculated: " + sumCgst + ", Excel: " + excelCgstTotal);

                    if (Math.Abs(sumSgst - excelSgstTotal) > 0.05m)
                        throw new Exception("SGST mismatch. Calculated: " + sumSgst + ", Excel: " + excelSgstTotal);

                    objDSMModelData.SaveInvoice(lstInvoice);

                    Global.InvoiceNumber = invoiceNo;
                    Global.InvoiceDate = invoiceDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
            }
            catch (Exception ex)
            {
                LogErrors(ex);
                throw;
            }
        }

        private void ExtractEInvoiceText(string pdfPath, out string irn, out string ackNo, out DateTime? ackDate)
        {
            irn = null;
            ackNo = null;
            ackDate = null;
            _ewayBillNo = null;

            string tessDataPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata");
            StringBuilder pdfText = new StringBuilder();

            try
            {
                // Step 1: Extract text directly from the PDF, if available.
                try
                {
                    using (var reader = new iTextSharp.text.pdf.PdfReader(pdfPath))
                    {
                        for (int page = 1; page <= reader.NumberOfPages; page++)
                        {
                            string pageText = iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(reader, page);
                            pdfText.AppendLine(pageText);

                            Match match = Regex.Match(pageText, @"(?i)\bIRN\b[\s:=-]*([A-F0-9](?:[\s\-]*[A-F0-9]){63})");
                            if (match.Success)
                            {
                                irn = Regex.Replace(match.Groups[1].Value, @"[\s\-]", "").ToUpperInvariant();
                                LogErrors(new Exception("IRN extracted from PDF text layer: " + irn));
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogErrors(ex);
                }

                // Step 2: OCR every page, preserving your original extraction flow.
                var doc = new Spire.Pdf.PdfDocument();
                doc.LoadFromFile(pdfPath);

                try
                {
                    for (int i = 0; i < doc.Pages.Count; i++)
                    {
                        using (System.Drawing.Image img = doc.SaveAsImage(i, Spire.Pdf.Graphics.PdfImageType.Bitmap, 300, 300))
                        using (var bitmap = new System.Drawing.Bitmap(img))
                        using (var ms = new MemoryStream())
                        using (var engine = new Tesseract.TesseractEngine(tessDataPath, "eng", Tesseract.EngineMode.Default))
                        {
                            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            byte[] imageBytes = ms.ToArray();

                            using (var pix = Tesseract.Pix.LoadFromMemory(imageBytes))
                            using (var result = engine.Process(pix, Tesseract.PageSegMode.Auto))
                            {
                                pdfText.AppendLine(result.GetText());
                            }
                        }
                    }
                }
                finally
                {
                    doc.Close();
                }

                string text = pdfText.ToString();

                // Step 3: Use OCR for IRN only if the PDF text layer did not provide it.
                if (string.IsNullOrWhiteSpace(irn))
                {
                    Match irnMatch = Regex.Match(text, @"(?i)\bIRN\b[\s:=-]*([A-F0-9](?:[\s\-]*[A-F0-9]){63})");
                    if (irnMatch.Success)
                        irn = Regex.Replace(irnMatch.Groups[1].Value, @"[\s\-]", "").ToUpperInvariant();
                }

                // Acknowledgement Number.
                Match ackMatch = Regex.Match(text, @"(?is)\bAck(?:nowledgement|nowledgment)?\s*No\.?\s*[:\-]?\s*([0-9][0-9\s]*)");
                if (ackMatch.Success)
                    ackNo = Regex.Replace(ackMatch.Groups[1].Value, @"\s+", "").Trim();

                // Acknowledgement Date.
                Match dateMatch = Regex.Match(text, @"(?i)\bAck\s*Date\b[\s:=-]*(\d{1,2}[-/ ](?:[A-Za-z]{3,9}|\d{1,2})[-/ ]\d{2,4}|\d{4}-\d{2}-\d{2})");

                if (dateMatch.Success)
                {
                    string dateText = Regex.Replace(dateMatch.Groups[1].Value.Trim(), @"\s+", "-");
                    string[] formats = { "dd/MM/yyyy", "d/M/yyyy", "dd-MM-yyyy", "d-M-yyyy", "dd-MMM-yyyy", "d-MMM-yyyy", "dd-MMM-yy", "d-MMM-yy", "dd-MMMM-yyyy", "d-MMMM-yyyy", "yyyy-MM-dd" };

                    if (DateTime.TryParseExact(dateText, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate) ||
                        DateTime.TryParse(dateText, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out parsedDate))
                        ackDate = parsedDate;
                }

                // e-Way Bill Number.
                Match ewayMatch = Regex.Match(text, @"(?i)e[\s\-]*way\s*bill\s*(?:no\.?|number)?[\s:.=\-]*((?:\d[\s\-]?){11}\d)(?!\d)");
                if (ewayMatch.Success)
                    _ewayBillNo = Regex.Replace(ewayMatch.Groups[1].Value, @"\D", "");

                if (string.IsNullOrWhiteSpace(ackNo) || ackDate == null)
                {
                    LogErrors(new Exception(
                        "E-Invoice extraction failed.\n" +
                        "IRN: " + (irn ?? "NOT FOUND") + "\n" +
                        "Ack No: " + (ackNo ?? "NOT FOUND") + "\n" +
                        "Ack Date: " + (ackDate?.ToString("dd/MM/yyyy") ?? "NOT FOUND") + "\n" +
                        "OCR/PDF Text:\n" + text));

                    throw new Exception("Could not extract Ack No., or Ack Date. Check the OCR log.");
                }
            }
            catch (Exception ex)
            {
                LogErrors(ex);
                throw;
            }
        }

        private byte[] ExtractEInvoiceQRCode(string pdfPath, string irn)
        {
            if (string.IsNullOrWhiteSpace(pdfPath) || !File.Exists(pdfPath))
                throw new FileNotFoundException("PDF file not found.", pdfPath);

            //if (string.IsNullOrWhiteSpace(irn))
            //    throw new Exception("IRN is empty. Cannot validate the QR code.");

            _qrIrn = null;

            try
            {
                using (PdfReader reader = new PdfReader(pdfPath))
                {
                    var parser = new PdfReaderContentParser(reader);

                    for (int page = 1; page <= reader.NumberOfPages; page++)
                    {
                        var listener = new EInvoiceImageListener(this, irn);
                        parser.ProcessContent(page, listener);

                        if (listener.QRCodeBytes != null)
                            return listener.QRCodeBytes;
                    }
                }
            }
            catch (Exception ex)
            {
                LogErrors(ex);
            }


            try
            {
                using (PdfReader pdfReader = new PdfReader(pdfPath))
                using (var document = new Spire.Pdf.PdfDocument())
                {
                    document.LoadFromFile(pdfPath);

                    for (int p = 0; p < document.Pages.Count; p++)
                    {
                        var pdfSize = pdfReader.GetPageSize(p + 1);

                        using (System.Drawing.Image img = document.SaveAsImage(p, 600, 600))
                        using (Bitmap page = new Bitmap(img))
                        {
                            byte[] cropped = CropQRCodeByCoordinates(page, irn, pdfSize.Width, pdfSize.Height);
                            if (cropped != null)
                                return cropped;

                            byte[] qr = TryExtractQrFromBitmap(page, irn);
                            if (qr != null)
                                return qr;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogErrors(ex);
            }

            throw new Exception("QR code could not be extracted and validated. Check the application log.");
        }

        private byte[] TryExtractQrFromBitmap(Bitmap bitmap, string irn)
        {
            if (bitmap == null)
                return null;

            try
            {
                var reader = new ZXing.BarcodeReader
                {
                    AutoRotate = true,
                    Options = new DecodingOptions
                    {
                        TryHarder = true,
                        PossibleFormats = new List<BarcodeFormat>
                {
                    BarcodeFormat.QR_CODE
                }
                    }
                };

                var result = reader.Decode(bitmap);

                if (result == null)
                    return null;

                LogErrors(new Exception(
                    "QR decoded. Text length: " +
                    (result.Text == null ? 0 : result.Text.Length)));

                if (!QrContainsIRN(result.Text, irn))
                {
                    LogErrors(new Exception(
                        "QR decoded, but the payload does not match the invoice IRN."));
                    return null;
                }

                // Crop the actual QR using the decoded finder-point coordinates.
                if (result.ResultPoints != null && result.ResultPoints.Length >= 3)
                {
                    float minX = result.ResultPoints.Min(pt => pt.X);
                    float maxX = result.ResultPoints.Max(pt => pt.X);
                    float minY = result.ResultPoints.Min(pt => pt.Y);
                    float maxY = result.ResultPoints.Max(pt => pt.Y);

                    int padX = Math.Max(10, (int)((maxX - minX) * 0.20));
                    int padY = Math.Max(10, (int)((maxY - minY) * 0.20));

                    int left = Math.Max(0, (int)Math.Floor(minX) - padX);
                    int top = Math.Max(0, (int)Math.Floor(minY) - padY);
                    int right = Math.Min(bitmap.Width, (int)Math.Ceiling(maxX) + padX);
                    int bottom = Math.Min(bitmap.Height, (int)Math.Ceiling(maxY) + padY);

                    if (right > left && bottom > top)
                    {
                        using (Bitmap crop = bitmap.Clone(
                            new System.Drawing.Rectangle(left, top, right - left, bottom - top),
                            PixelFormat.Format24bppRgb))
                        {
                            var qrBytes = ToPng(crop);
                            if (QrDecodesToIrn(qrBytes, _qrIrn))
                                return qrBytes;
                        }
                    }
                }

                // If the QR was validated but its points are unavailable,
                // preserve the validated source image.
                return null;
            }
            catch (Exception ex)
            {
                LogErrors(ex);
                return null;
            }
        }

        private bool QrContainsIRN(string qrText, string irn)
        {
            string qrIrn = GetIrnFromQr(qrText);
            string actualQrIrn = Regex.Replace(qrIrn ?? "", @"[^A-Fa-f0-9]", "").ToUpperInvariant();

            if (!Regex.IsMatch(actualQrIrn, @"^[A-F0-9]{64}$"))
            {
                LogErrors(new Exception("QR validation failed. Invalid QR IRN: " + actualQrIrn));
                return false;
            }

            _qrIrn = actualQrIrn;
            LogErrors(new Exception("Correct IRN extracted from QR: " + _qrIrn));
            return true;
        }

        private string GetIrnFromQr(string qrText)
        {
            if (string.IsNullOrWhiteSpace(qrText)) return null;

            Match match = Regex.Match(qrText, @"\\?""Irn\\?""\s*:\s*\\?""([A-Fa-f0-9]{64})\\?""", RegexOptions.IgnoreCase);
            if (match.Success) return match.Groups[1].Value.ToUpperInvariant();

            string irn = FindIrnInJson(qrText);
            if (!string.IsNullOrEmpty(irn)) return irn.ToUpperInvariant();

            string[] parts = qrText.Split('.');
            if (parts.Length >= 2)
            {
                try
                {
                    string payload = parts[1].Replace('-', '+').Replace('_', '/');
                    payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
                    string json = Encoding.UTF8.GetString(Convert.FromBase64String(payload));

                    match = Regex.Match(json, @"\\?""Irn\\?""\s*:\s*\\?""([A-Fa-f0-9]{64})\\?""", RegexOptions.IgnoreCase);
                    if (match.Success) return match.Groups[1].Value.ToUpperInvariant();

                    irn = FindIrnInJson(json);
                    if (!string.IsNullOrEmpty(irn)) return irn.ToUpperInvariant();
                }
                catch (Exception ex) { LogErrors(ex); }
            }

            return null;
        }

        private string FindIrnInJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            Match match = Regex.Match(
                json,
                @"""Irn""\s*:\s*""([A-Fa-f0-9]{64})""",
                RegexOptions.IgnoreCase);

            return match.Success
                ? match.Groups[1].Value.ToUpperInvariant()
                : null;
        }

        private byte[] CropQRCodeByCoordinates(Bitmap page, string irn, float pdfWidth, float pdfHeight)
        {
            // Original PDF coordinates: Left, Bottom, Right, Top
            float pdfLeft = 382, pdfBottom = 685, pdfRight = 502, pdfTop = 805;

            double scaleX = page.Width / (double)pdfWidth;
            double scaleY = page.Height / (double)pdfHeight;

            int left = (int)Math.Round(pdfLeft * scaleX);
            int right = (int)Math.Round(pdfRight * scaleX);
            int top = (int)Math.Round((pdfHeight - pdfTop) * scaleY);
            int bottom = (int)Math.Round((pdfHeight - pdfBottom) * scaleY);

            if (left < 0 || top < 0 || right > page.Width || bottom > page.Height || right <= left || bottom <= top)
            {
                LogErrors(new Exception("Invalid QR crop. Page: " + page.Width + " x " + page.Height +
                    "; Crop: L=" + left + ", T=" + top + ", R=" + right + ", B=" + bottom));
                return null;
            }

            using (Bitmap crop = page.Clone(new System.Drawing.Rectangle(left, top, right - left, bottom - top), PixelFormat.Format24bppRgb))
            {
                var reader = new ZXing.BarcodeReader
                {
                    AutoRotate = true,
                    Options = new DecodingOptions { TryHarder = true, PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.QR_CODE } }
                };

                var result = reader.Decode(crop);

                if (result == null || !QrContainsIRN(result.Text, irn))
                {
                    LogErrors(new Exception("QR crop failed validation. Crop dimensions: " + crop.Width + " x " + crop.Height));
                    return null;
                }

                LogErrors(new Exception("QR crop validated successfully. Dimensions: " + crop.Width + " x " + crop.Height));
                return ToPng(crop);
            }
        }

        private byte[] ToPng(Bitmap bmp)
        {
            using (var ms = new MemoryStream())
            {
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }

        private string _qrIrn;
        private string _ewayBillNo;

        private string _expectedInvoiceNo;   // invoice number from the Excel, set in CreateInvoiceFromExcel

        // True when the image decodes to a QR whose IRN equals the given IRN
        private bool QrDecodesToIrn(byte[] png, string expectedIrn)
        {
            if (png == null || string.IsNullOrEmpty(expectedIrn)) return false;

            try
            {
                using (var ms = new MemoryStream(png))
                using (var bmp = new Bitmap(ms))
                {
                    var reader = new ZXing.BarcodeReader
                    {
                        AutoRotate = true,
                        Options = new DecodingOptions
                        {
                            TryHarder = true,
                            PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.QR_CODE }
                        }
                    };

                    var res = reader.Decode(bmp);
                    return res != null && string.Equals(GetIrnFromQr(res.Text), expectedIrn, StringComparison.OrdinalIgnoreCase);
                }
            }
            catch
            {
                return false;
            }
        }

        // Crops exactly to the QR: expands from the corner squares until the first fully white row/column

        private class EInvoiceImageListener : IRenderListener
        {
            private readonly DashBoardViewModel owner;
            private readonly string irn;

            public byte[] QRCodeBytes { get; private set; }

            public EInvoiceImageListener(DashBoardViewModel owner, string invoiceIrn)
            {
                this.owner = owner;
                irn = invoiceIrn;
            }

            public void BeginTextBlock() { }
            public void EndTextBlock() { }
            public void RenderText(TextRenderInfo renderInfo) { }

            public void RenderImage(ImageRenderInfo renderInfo)
            {
                if (QRCodeBytes != null)
                    return;

                try
                {
                    var pdfImage = renderInfo.GetImage();
                    if (pdfImage == null)
                        return;

                    using (System.Drawing.Image image = pdfImage.GetDrawingImage())
                    {
                        if (image == null)
                            return;

                        using (Bitmap bitmap = new Bitmap(image))
                        {
                            byte[] qr = owner.TryExtractQrFromBitmap(bitmap, irn);

                            if (qr != null)
                                QRCodeBytes = qr;
                        }
                    }
                }
                catch (Exception ex)
                {
                    owner.LogErrors(ex);
                }
            }
        }


    }
}
