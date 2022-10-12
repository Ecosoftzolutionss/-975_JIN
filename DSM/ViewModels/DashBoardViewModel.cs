using DataMatrix.net;
using DSMData;
using DSMData.Model;
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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
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
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;
using ThoughtWorks.QRCode.Codec;
using ThoughtWorks.QRCode.Codec.Data;
using ZXing;

namespace DSM.ViewModels
{
    public class DashBoardViewModel : BindableBase, INotifyPropertyChanged
    {

        //[DllImport("user32")]
        //public static extern int SetCursorPos(int x, int y);

        //[DllImport("gsdll32.dll", EntryPoint = "gsapi_new_instance")]
        //public static void ReadPDF();

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

        string  QRCodeString = "";

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
                    Application.Current.MainWindow.Closing += new CancelEventHandler(MainWindow_Closing);
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
                Application.Current.MainWindow.Closing += new CancelEventHandler(MainWindow_Closing);
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
            //var lstApiInv = objDSMModelData.GetApiUserType("apipost");
            //objUser = null;
            //EthernetConnectivityCheck();
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
                    //MessageBox.Show("Server Disconnected");

                }
            }

        }
        public void AssignSettings()
        {
            try
            {
                //EthernetConnectivityCheck();
                SplitStatus = true;
                SettingsDisplayModel objSettings = new SettingsDisplayModel();
                CustomerDisplayModel objCustomer = new CustomerDisplayModel();

                Global.CustomerName = "HYUNDAI";

                objDigitalSign = new DigitalSign();
                var keyConnected = objDigitalSign.CheckKeyConnected(Global.CustomerName);
                keyConnected = true;
                if (keyConnected == false)
                {
                    MessageBox.Show("Check whether the key is connected or not!");
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

        public void EthernetConnectivityCheck()
        {
            try
            {
                // IPHostEntry obj = Dns.GetHostByName("www.home.live.com");
                TcpClient clnt = new TcpClient("www.google.com", 80);
                clnt.Close();
                //MessageBox.Show("Connected");
                // Console.WriteLine("Ethernet Connected");
                //Console.ReadLine();
            }
            catch
            {
                MessageBox.Show("Server Disconnected");
                //for (int intCounter = App.Current.Windows.Count - 1; intCounter > 0; intCounter--)
                //    App.Current.Windows[intCounter].Close();
                //App.Current.MainWindow.Close();
                foreach (Process proc in Process.GetProcessesByName("DSM"))
                {
                    proc.Kill();
                }
                timer.Stop();
                //timerAPIPost.Stop();
                //for (int intCounter = App.Current.Windows.Count - 1; intCounter > 0; intCounter--)
                //    App.Current.Windows[intCounter].Close();

                //Console.WriteLine("Ehternet Not Connected");
                //Console.ReadLine();
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
            ReadPDF1();
            SignPDF();
            PrintFiles();
            APIPostFiles();
            CurrentRefreshLists();
            timer.Start();
        }

        private string DecodeText(string sFileName)
        {
            DmtxImageDecoder decoder = new DmtxImageDecoder();
            System.Drawing.Bitmap oBitmap = new System.Drawing.Bitmap(sFileName);
            List<string> oList = decoder.DecodeImage(oBitmap);

            StringBuilder sb = new StringBuilder();
            sb.Length = 0;
            foreach (string s in oList)
            {
                sb.Append(s);
            }
            return sb.ToString();
        }

        private void APIPostFiles(object sender, EventArgs e)
        {
            try
            {
                //EthernetConnectivityCheck();
                timer.Stop();
                //timerAPIPost.Stop();
                RefreshServerLists();
                DSMModelData objDSMModelData = new DSMModelData();
                //var lstApiInv = objDSMModelData.GetAllInvoiceStatusAll().Where(x => x.DSStatus == "TRUE" && x.PrintStatus == "FALSE").FirstOrDefault();
                //var lstApiInv = objDSMModelData.GetAllAPIInvoiceStatusAll().Where(x => x.DSStatus == "TRUE" && x.PrintStatus == "FALSE").FirstOrDefault();
                //var lstApiInv = objDSMModelData.GetAllAPIInvoiceStatusAll().ToList();

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
                //EthernetConnectivityCheck();
                //timerAPIPost.Stop();
                //efreshServerLists();
                DSMModelData objDSMModelData = new DSMModelData();
                //var lstApiInv = objDSMModelData.GetAllInvoiceStatusAll().Where(x => x.DSStatus == "TRUE" && x.PrintStatus == "FALSE").FirstOrDefault();
                //var lstApiInv = objDSMModelData.GetAllAPIInvoiceStatusAll().Where(x => x.DSStatus == "TRUE" && x.PrintStatus == "FALSE").FirstOrDefault();
                //var lstApiInv = objDSMModelData.GetAllAPIInvoiceStatusAll().ToList();

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
                //timerAPIPost.Start();
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

        private void PickPDF(object sender, EventArgs e)
        {
            try
            {
                //timerPick.Stop();
                //RefreshLists();
                string[] invoicePathList = Directory.GetFiles(Global.InvoicePath, "*.pdf");
                if (invoicePathList.Count() == 0)
                {
                    ProcessMsg = "";
                }
                PendingInvList = new ObservableCollection<InvoiceDisplayModel>();
                for (int i = 0; i < invoicePathList.Length; i++)
                {
                    //AssignInvoiceNo_TruckNo(invoicePathList[i], "INV");
                    SplitAndSave(invoicePathList[i], Global.InputPath, Global.InvoiceNumber, "INVOICE");
                }
                //timerPick.Start();
            }
            catch (Exception ex)
            {
                LogErrors(ex);
                timerPick.Start();
            }
        }

        private static void GetImages(string filename)
        {
            int pageNum = 1;

            PdfReader pdf = new PdfReader(filename);
            PdfDictionary pg = pdf.GetPageN(pageNum);
            PdfDictionary res = (PdfDictionary)PdfReader.GetPdfObject(pg.Get(PdfName.RESOURCES));
            PdfDictionary xobj = (PdfDictionary)PdfReader.GetPdfObject(res.Get(PdfName.XOBJECT));
            if (xobj == null) { return; }
            foreach (PdfName name in xobj.Keys)
            {
                PdfObject obj = xobj.Get(name);
                if (!obj.IsIndirect()) { continue; }
                PdfDictionary tg = (PdfDictionary)PdfReader.GetPdfObject(obj);
                PdfName type = (PdfName)PdfReader.GetPdfObject(tg.Get(PdfName.SUBTYPE));
                if (!type.Equals(PdfName.IMAGE)) { continue; }
                int XrefIndex = Convert.ToInt32(((PRIndirectReference)obj).Number.ToString(System.Globalization.CultureInfo.InvariantCulture));
                PdfObject pdfObj = pdf.GetPdfObject(XrefIndex);
                PdfStream pdfStrem = (PdfStream)pdfObj;
                byte[] bytes = PdfReader.GetStreamBytesRaw((PRStream)pdfStrem);
                if (bytes == null) { continue; }
                using (System.IO.MemoryStream memStream = new System.IO.MemoryStream(bytes))
                {
                    memStream.Position = 0;
                    System.Drawing.Image img = System.Drawing.Image.FromStream(memStream);

                    string path = System.IO.Path.Combine(String.Format(@"result-{0}.jpg", pageNum));
                    System.Drawing.Imaging.EncoderParameters parms = new System.Drawing.Imaging.EncoderParameters(1);
                    parms.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Compression, 0);
                    var jpegEncoder = ImageCodecInfo.GetImageEncoders().ToList().Find(x => x.FormatID == ImageFormat.Jpeg.Guid);
                    img.Save(path, jpegEncoder, parms);
                }
            }
        }
        private void Qrcode(string img)

        {
            //const string imageUrl = @"D:\SHRUTHI\OTHERS\FILES\INVOICE\T220459\QRCode PDF 465.jpg";
            //const string imageUrl = img;

            // Install-Package ZXing.Net -Version 0.16.5
            var client = new WebClient();
            var stream = client.OpenRead(img);
            if (stream == null) return;
            var bitmap = new Bitmap(stream);
            IBarcodeReader reader = new ZXing.BarcodeReader();
            var result = reader.Decode(bitmap);
            MessageBox.Show(result.Text);
            //https://blog.dotnetframework.org/2020/02/26/read-a-qr-code-in-c-using-the-zxing-net-library/
            //foreach (BarcodeResult Result in Results)
            //{

            //    string Value = Result.Value;
            //    Bitmap Img = Result.BarcodeImage;
            //    BarcodeEncoding BarcodeType = Result.BarcodeType;
            //    byte[] Binary = Result.BinaryValue;
            //    Console.WriteLine(Result.Value);
            //}
        }
        private void ReadPDFfinal()
        {
            try
            {

                //string[] inputPath = Directory.GetFiles(Global.InputPath, "*.pdf");
                //string[] outputPath = Directory.GetFiles(Global.OutputPath, "*.pdf");
                string[] invPath = Directory.GetFiles(Global.InvoicePath, "*.pdf");
                //for (int j = 0; j < invPath.Length; j++)
                //{

                if (invPath.Length > 0)
                {
                    String fileName = invPath[0];

                    AssignInvoiceNo_TruckNo(invPath[0], "INV");
                    //if (Global.CustomerCheck != null)
                    //{
                    //    if (Global.CustomerCheck.ToUpper().Contains(Global.CustomerName.ToUpper()))
                    //    {
                    //if (Global.InvoiceNumber != null)
                    //{
                    //AssignInvoiceNo_TruckNo(invPath[0], "TRUCK");

                    //DateTime invdate = Convert.ToDateTime(Global.InvoiceDate.ToString().Trim());
                    //DateTime invdate = DateTime.ParseExact(Global.InvoiceDate.ToString().Trim(), "d/M/yyyy", CultureInfo.InvariantCulture);
                    string FinYear = GetFinancialYear(Global.InvoiceDate.ToString().Trim());
                    Global.FinYear = Convert.ToInt32(FinYear.ToString().Trim());
                    CurrentRefreshInvoiceLists();
                    //CurrentInvoiceList.Items.Refresh();
                    //EthernetConnectivityCheck();

                    objDSMModelData = new DSMModelData();
                    int inv_count_rec = objDSMModelData.GetInvoiceDetailsByInvoiceNumberFinYear(Global.InvoiceNumber, Convert.ToInt32(FinYear)).Count;
                    if (inv_count_rec == 0)
                    {
                        if (Global.CustomerName == "HYUNDAI")
                        {
                            PDFConvert converter = new PDFConvert();
                            //Setup the converter
                            converter.RenderingThreads = -1;
                            converter.TextAlphaBit = -1;
                            converter.TextAlphaBit = -1;
                            converter.OutputToMultipleFile = true;
                            converter.FirstPageToConvert = -1;
                            converter.LastPageToConvert = -1;
                            converter.FitPage = false;
                            converter.JPEGQuality = 10;
                            converter.OutputFormat = "png256";
                            converter.ResolutionX = 500;
                            converter.ResolutionY = 500;
                            System.IO.FileInfo input = new FileInfo(fileName);
                            string directoryPath = System.IO.Path.GetDirectoryName(fileName) + "\\" + System.IO.Path.GetFileNameWithoutExtension(input.Name);
                            if (!Directory.Exists(directoryPath))
                                Directory.CreateDirectory(directoryPath);
                            Spire.Pdf.PdfDocument doc = new Spire.Pdf.PdfDocument();
                            doc.LoadFromFile(invPath[0]);
                            Spire.Pdf.PdfPageBase page = doc.Pages[0];
                            System.Drawing.Image[] images = page.ExtractImages();
                            string output = string.Format("{0}\\{1}{2}", directoryPath, System.IO.Path.GetFileNameWithoutExtension(input.Name), ".png");
                            converter.Convert(input.FullName, output);
                            
                            int count = 0;
                            foreach (System.Drawing.Image image in images)
                            {
                                count++;
                                try
                                {
                                    QRCodeString = "";
                                    QRCodeString = ScanQRPdf(image, output, directoryPath, invPath[0]);
                                    CreateInvoiceChk(QRCodeString.ToString());
                                    if (Global.InvoiceNumberchk != null)
                                    {
                                        if (Global.InvoiceNumber != null)
                                        {
                                            objDSMModelData.SaveQuarantine(Global.InvoiceNumber);
                                            Global.InvoiceNumber = null;
                                            if (Global.InvoiceNumberchk.Length <= 10)
                                            {
                                                CreateInvoice(QRCodeString.ToString());

                                            }
                                            else
                                            {
                                                continue;
                                            }
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }

                                }
                                catch (Exception i)
                                {
                                    LogErrors(i);

                                    if (Directory.Exists(directoryPath))
                                        Directory.Delete(directoryPath, true);

                                    MoveFile(invPath[0], "INVALID_INVOICE");
                                    objDSMModelData = new DSMModelData();

                                    
                                   

                                }
                            }


                        }
                        else
                        {
                            MoveFile(invPath[0], "AlreadyExist");
                            CurrentRefreshAlreadyInvoiceLists();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogErrors(ex);
                timer.Start();
            }
        }
        private void ReadPDF()
        {
            try
            {
                Global.VendorCode = "";
                //string[] inputPath = Directory.GetFiles(Global.InputPath, "*.pdf");
                //string[] outputPath = Directory.GetFiles(Global.OutputPath, "*.pdf");
                string[] invPath = Directory.GetFiles(Global.InvoicePath, "*.pdf");
                //for (int j = 0; j < invPath.Length; j++)
                //{

                if (invPath.Length > 0)
                {
                    String fileName = invPath[0];
                  
                    AssignInvoiceNo_TruckNo(invPath[0], "INV");
                    //if (Global.CustomerCheck != null)
                    //{
                    //    if (Global.CustomerCheck.ToUpper().Contains(Global.CustomerName.ToUpper()))
                    //    {
                    //if (Global.InvoiceNumber != null)
                    //{
                    //AssignInvoiceNo_TruckNo(invPath[0], "TRUCK");

                    //DateTime invdate = Convert.ToDateTime(Global.InvoiceDate.ToString().Trim());
                    //DateTime invdate = DateTime.ParseExact(Global.InvoiceDate.ToString().Trim(), "d/M/yyyy", CultureInfo.InvariantCulture);
                    string FinYear = GetFinancialYear(Global.InvoiceDate.ToString().Trim());
                    Global.FinYear = Convert.ToInt32(FinYear.ToString().Trim());
                    CurrentRefreshInvoiceLists();
                    //CurrentInvoiceList.Items.Refresh();
                    //EthernetConnectivityCheck();
                  
                    objDSMModelData = new DSMModelData();
                    int inv_count_rec = objDSMModelData.GetInvoiceDetailsByInvoiceNumberFinYear(Global.InvoiceNumber, Convert.ToInt32(FinYear)).Count;
                    if (inv_count_rec == 0)
                    {
                        if (Global.CustomerName == "HYUNDAI")
                        {

                            PDFConvert converter = new PDFConvert();
                            //Setup the converter
                            converter.RenderingThreads = -1;
                            converter.TextAlphaBit = -1;
                            converter.TextAlphaBit = -1;
                            converter.OutputToMultipleFile = true;
                            converter.FirstPageToConvert = -1;
                            converter.LastPageToConvert = -1;
                            converter.FitPage = false;
                            converter.JPEGQuality = 10;
                            converter.OutputFormat = "png256";
                            converter.ResolutionX = 500;
                            converter.ResolutionY = 500;
                            System.IO.FileInfo input = new FileInfo(fileName);
                            string directoryPath = System.IO.Path.GetDirectoryName(fileName) + "\\" + System.IO.Path.GetFileNameWithoutExtension(input.Name);
                            if (!Directory.Exists(directoryPath))
                                Directory.CreateDirectory(directoryPath);
                            Spire.Pdf.PdfDocument doc = new Spire.Pdf.PdfDocument();
                            doc.LoadFromFile(invPath[0]);
                            Spire.Pdf.PdfPageBase page = doc.Pages[0];
                            System.Drawing.Image[] images = page.ExtractImages();
                            string output = string.Format("{0}\\{1}{2}", directoryPath, System.IO.Path.GetFileNameWithoutExtension(input.Name), ".png");
                            converter.Convert(input.FullName, output);




                            int count = 0;

                            foreach (System.Drawing.Image image in images)
                            {
                                count++;
                                try
                                {
                                    QRCodeString = "";
                                    Global.InvoiceNumberchk = "";

                                    try
                                    {
                                        QRCodeString = ScanQRPdf(image, output, directoryPath, invPath[0]);



                                        CreateInvoiceChk(QRCodeString.ToString());

                                        if (Global.InvoiceNumberchk != null)
                                        {
                                            if (Global.InvoiceNumberchk.Length <= 10)
                                            {
                                                CreateInvoice(QRCodeString.ToString());

                                            }
                                            else
                                            {
                                                continue;
                                            }

                                        }
                                        else
                                        {
                                            continue;
                                        }

                                    }
                                    catch (Exception ex)
                                    {

                                    }


                                    Directory.Delete(directoryPath, true);

                                    if (!string.IsNullOrEmpty(QRCodeString))
                                    {
                                        //CreateInvoice(QRCodeString);
                                        SplitAndSave(invPath[0], Global.InputPath, Global.InvoiceNumber, "INVOICE");
                                        //Global.InvoiceNumber = null;
                                        if (Directory.Exists(directoryPath))
                                            Directory.Delete(directoryPath, true);
                                    }
                                
                                    else
                                    {
                                        MoveFile(invPath[0], "INVALID_INVOICE");
                                        objDSMModelData = new DSMModelData();

                                        if (Global.InvoiceNumber != null)
                                        {
                                            objDSMModelData.SaveQuarantine(Global.InvoiceNumber);
                                            Global.InvoiceNumber = null;
                                        }

                                        timer.Start();
                                    
                                    }

                                }
                                catch (Exception i)
                                {
                                    LogErrors(i);

                                    if (Directory.Exists(directoryPath))
                                        Directory.Delete(directoryPath, true);

                                    MoveFile(invPath[0], "INVALID_INVOICE");
                                    objDSMModelData = new DSMModelData();

                                    if (Global.InvoiceNumber != null)
                                    {
                                        objDSMModelData.SaveQuarantine(Global.InvoiceNumber);
                                        Global.InvoiceNumber = null;
                                    }

                                    timer.Start();
                                }
                            }
                        }

                        else
                        {
                            try
                            {
                                SplitAndSave(invPath[0], Global.InputPath, Global.InvoiceNumber, "INVOICE");
                            }
                            catch (Exception i)
                            {
                                LogErrors(i);
                                timer.Start();
                            }
                        }
                       
                    }
                    else
                    {
                        MoveFile(invPath[0], "AlreadyExist");
                        CurrentRefreshAlreadyInvoiceLists();
                    }
                }
            }
            catch (Exception ex)
            {
                LogErrors(ex);
                timer.Start();
            }
        }
        private void ReadPDF1()
        {
            try
            {
                
                string[] invPath = Directory.GetFiles(Global.InvoicePath, "*.pdf");
                if (invPath.Length > 0)
                {
                    String fileName = invPath[0];

                    AssignInvoiceNo_TruckNo(invPath[0], "INV");

                    if (!string.IsNullOrEmpty(Global.InvoiceDate))
                    {
                        string FinYear = GetFinancialYear(Global.InvoiceDate.ToString().Trim());
                        Global.FinYear = Convert.ToInt32(FinYear.ToString().Trim());
                        CurrentRefreshInvoiceLists();
                        objDSMModelData = new DSMModelData();
                        int inv_count_rec = objDSMModelData.GetInvoiceDetailsByInvoiceNumberFinYear(Global.InvoiceNumber, Convert.ToInt32(FinYear)).Count;
                        if (inv_count_rec == 0)
                        {
                           

                            //spire
                            if (Global.CustomerName == "HYUNDAI")
                            {

                                PDFConvert converter = new PDFConvert();
                                //Setup the converter
                                converter.RenderingThreads = -1;
                                converter.TextAlphaBit = -1;
                                converter.TextAlphaBit = -1;
                                converter.OutputToMultipleFile = true;
                                converter.FirstPageToConvert = -1;
                                converter.LastPageToConvert = -1;
                                converter.FitPage = false;
                                converter.JPEGQuality = 10;
                                converter.OutputFormat = "png256";
                                converter.ResolutionX = 500;
                                converter.ResolutionY = 500;
                                System.IO.FileInfo input = new FileInfo(fileName);
                                string directoryPath = System.IO.Path.GetDirectoryName(fileName) + "\\" + System.IO.Path.GetFileNameWithoutExtension(input.Name);
                                if (!Directory.Exists(directoryPath))
                                    Directory.CreateDirectory(directoryPath);
                                Spire.Pdf.PdfDocument doc = new Spire.Pdf.PdfDocument();
                                doc.LoadFromFile(invPath[0]);
                                Spire.Pdf.PdfPageBase page = doc.Pages[0];
                                System.Drawing.Image[] images = page.ExtractImages();
                                string output = string.Format("{0}\\{1}{2}", directoryPath, System.IO.Path.GetFileNameWithoutExtension(input.Name), ".png");
                                converter.Convert(input.FullName, output);




                                int count = 0;
                                
                                foreach (System.Drawing.Image image in images)
                                {
                                    count++;
                                    try
                                    {
                                        

                                            try
                                            {
                                         
                                                QRCodeString = ScanQRPdf(image, output, directoryPath, invPath[0]);

                                            //MessageBox.Show(count.ToString());
                                            //SplitAndSave(invPath[0], Global.InputPath, Global.InvoiceNumber, "INVOICE");
                                            CreateInvoiceChk(QRCodeString.ToString());
                                            
                                            if (Global.InvoiceNumberchk != null)
                                                {
                                                  if(Global.InvoiceNumberchk.Length<=13)
                                                   {
                                                    CreateInvoice(QRCodeString.ToString());
                                                    Global.InvoiceNumberchk =null;
                                                    Directory.Delete(directoryPath, true);

                                                    if (!string.IsNullOrEmpty(QRCodeString))
                                                    {
                                                        //CreateInvoice(QRCodeString);
                                                        SplitAndSave(invPath[0], Global.InputPath, Global.InvoiceNumber, "INVOICE");
                                                        //Global.InvoiceNumber = null;
                                                        if (Directory.Exists(directoryPath))
                                                            Directory.Delete(directoryPath, true);
                                                    }
                                                    else
                                                    {
                                                        MoveFile(invPath[0], "INVALID_INVOICE");
                                                        objDSMModelData = new DSMModelData();

                                                        if (Global.InvoiceNumber != null)
                                                        {
                                                            objDSMModelData.SaveQuarantine(Global.InvoiceNumber);
                                                            Global.InvoiceNumber = null;
                                                        }


                                                        timer.Start();
                                                        
                                                    }
                                                    break;

                                                }
                                                    else
                                                         {
                                                             continue;
                                                        }
                                                
                                                }
                                            else
                                            {
                                                continue;
                                            }

                                            }
                                            catch(Exception ex)
                                            {

                                            }

                                       
                                        //Directory.Delete(directoryPath, true);

                                        //if (!string.IsNullOrEmpty(QRCodeString))
                                        //{
                                        //    //CreateInvoice(QRCodeString);
                                        //    SplitAndSave(invPath[0], Global.InputPath, Global.InvoiceNumber, "INVOICE");
                                        //    //Global.InvoiceNumber = null;
                                        //    if (Directory.Exists(directoryPath))
                                        //        Directory.Delete(directoryPath, true);
                                        //}
                                        //else
                                        //{
                                        //    MoveFile(invPath[0], "INVALID_INVOICE");
                                        //    objDSMModelData = new DSMModelData();

                                        //    if (Global.InvoiceNumber != null)
                                        //    {
                                        //        objDSMModelData.SaveQuarantine(Global.InvoiceNumber);
                                        //        Global.InvoiceNumber = null;
                                        //    }


                                        //    timer.Start();
                                        //}
                                    }
                                    catch (Exception i)
                                    {
                                        LogErrors(i);

                                        if (Directory.Exists(directoryPath))
                                            Directory.Delete(directoryPath, true);

                                        MoveFile(invPath[0], "INVALID_INVOICE");
                                        objDSMModelData = new DSMModelData();

                                        if (Global.InvoiceNumber != null)
                                        {
                                            objDSMModelData.SaveQuarantine(Global.InvoiceNumber);
                                            Global.InvoiceNumber = null;
                                        }

                                        timer.Start();
                                    }

                                }
                                
                            }

                            else
                            {
                                try
                                {
                                    if (!string.IsNullOrEmpty(Global.InvoiceNumber))
                                    {
                                        SplitAndSave(invPath[0], Global.InputPath, Global.InvoiceNumber, "INVOICE");
                                    }
                                }
                                catch (Exception i)
                                {
                                    LogErrors(i);
                                    timer.Start();
                                }
                            }

                        }
                        else
                        {
                            MoveFile(invPath[0], "AlreadyExist");
                            CurrentRefreshAlreadyInvoiceLists();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogErrors(ex);
                timer.Start();
            }
        }
        private string  ScanQRPdf(System.Drawing.Image img, string outPutPath, string directory, string pdfPath)
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

        private void SignPDF()
        {
            try
            {
                //timerSign.Stop();
                //var lstSign = objDSMModelData.GetAllInvoiceStatusAll().Where(x => x.DSStatus == "FALSE").FirstOrDefault();
                //var lstSign = objDSMModelData.GetAllInvoiceStatusAll().Where(x => x.DSStatus == "FALSE").FirstOrDefault();
                CurrentRefreshSignedLists();
                string[] inputPathList = Directory.GetFiles(Global.InputPath, "*.pdf");
                if (inputPathList.Count() == 0)
                {
                    ProcessMsg = "";
                }


                //PendingInvList = new ObservableCollection<InvoiceDisplayModel>();
                for (int i = 0; i < inputPathList.Length; i++)
                {
                    //AssignInvoiceNo_TruckNo(invoicePathList[i], "INV");
                    //SplitAndSave(inputPathList[i], Global.InputPath, Global.InvoiceNumber, "INVOICE");
                    CurrentInvoice = "Invoice - " + Global.InvoiceNumber + " is Signing...";
                    //EthernetConnectivityCheck();
                    objDigitalSign = new DigitalSign();
                    bool sign = objDigitalSign.FlatDigitalSignatureInitialize("Invoice", Global.InvoiceNumber, Global.CustomerName, Global.InvoiceNumber);
                    if (sign)
                    {
                        if (Global.CustomerName.ToString().Trim() == "HYUNDAI")
                        {
                            string PdfFilename = Global.OutputPath + Global.InvoiceNumber + "ORIGINALINVOICE.pdf";
                            //if (PdfFilename != null && File.Exists(PdfFilename))
                            if (PdfFilename != null && File.Exists(PdfFilename))
                            {

                                //if (!string.IsNullOrEmpty(QRCodeString))
                                //{


                                //    CreateInvoice(QRCodeString);

                                //}
                            }

                        }
                        else
                        {
                            string PdfFilename = Global.OutputPath + Global.InvoiceNumber + "ORIGINALinvoice.pdf";
                            //if (PdfFilename != null && File.Exists(PdfFilename))
                            if (PdfFilename != null && File.Exists(PdfFilename))
                            {
                                OthercustInv();
                            }

                        }
                    }
                    CurrentRefreshSignedLists();
                }
                //timerSign.Start();
            }
            catch (Exception ex)
            {
                LogErrors(ex);
                //RefreshLists();
                //timerSign.Start();
                timer.Start();
            }
        }

        private void MailPDF()
        {
            try
            {

                string Fordpdffilename = Global.OutputPath + @"\" + Global.InvoiceNumber + "ORIGINALInvoice.pdf";
                if (Global.CustomerName == "FORD")
                {
                    string vendorcode = "FA9NA";
                    string newfilename = "E6_1_" + vendorcode.ToString().Trim() + "_" + Global.InvoiceNumber.ToString().Trim() + ".pdf";
                    if (System.IO.File.Exists(Fordpdffilename))
                    {
                        if (Fordpdffilename.ToString().Trim().Contains("ORIGINAL"))
                        {
                            if (Fordpdffilename.ToString().Trim().Contains("ORIGINAL"))
                            {
                                if ((System.IO.File.Exists(Global.OutputPath + @"\\" + newfilename.ToString().Trim())))
                                    File.Delete(Global.OutputPath + @"\\" + newfilename.ToString().Trim());
                                //System.IO.File.Move(pdffilename, SalesOrderManagementSystem.Utilities._sDSOutput_File_Path + @"\\" + newfilename.ToString().Trim());
                                System.IO.File.Move(Global.OutputPath + @"\\" + System.IO.Path.GetFileName(Fordpdffilename.ToString().Trim()), Global.OutputPath + @"\\" + newfilename.ToString().Trim());

                            }

                        }



                        objDigitalSign = new DigitalSign();
                        //bool mailSent = objDigitalSign.SendEmail(lstSign.InvNumber);
                        bool mailSent = objDigitalSign.FordSendEmail(Global.OutputPath + @"\\" + newfilename.ToString().Trim());
                        if (mailSent == true)
                        {
                            //objDSMModelData.UpdateEmailstatusByInvoiceNo(lstSign.InvNumber, true);
                            Global.IsMailSend = true;
                        }
                        else
                        {
                            Global.IsMailSend = false;
                        }

                        //RefreshLists();
                        //}
                    }
                }
                else if (Global.CustomerName == "HYUNDAI")
                {

                    if (System.IO.File.Exists(Global.OutputPath + @"\\" + Global.InvoiceNumber + "ORIGINALInvoice.pdf"))
                    {
                        objDigitalSign = new DigitalSign();
                        //bool mailSent = objDigitalSign.SendEmail(lstSign.InvNumber);
                        bool mailSent = objDigitalSign.FordSendEmail(Global.OutputPath + @"\\" + Global.InvoiceNumber + "ORIGINALInvoice.pdf");

                        if (mailSent == true)
                        {
                            //objDSMModelData.UpdateEmailstatusByInvoiceNo(lstSign.InvNumber, true);
                            Global.IsMailSend = true;
                        }
                        else
                        {
                            Global.IsMailSend = false;
                        }
                    }

                    //RefreshLists();
                    //}


                }
                //}
                //timerMail.Start();
            }
            catch (Exception ex)
            {

                LogErrors(ex);
                //RefreshLists();
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
        private string ScanQRPdf(string imagePath)
        {
            //EthernetConnectivityCheck();
            ArrayList barcodes = new ArrayList();
            DateTime dtStart = DateTime.Now;
            String decodedString = string.Empty;
            var img = (imagePath);
            if (System.IO.Path.GetExtension(img).ToLower() == ".bmp")
            {
                Bitmap b = new Bitmap(img);
                try
                {
                    QRCodeDecoder decoder = new QRCodeDecoder();
                    decodedString = decoder.decode(new QRCodeBitmapImage(b));
                }
                catch (Exception ex)
                {
                    LogErrors(ex);
                }
                b.Dispose();
            }
            return decodedString;
        }
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
                    Global.InvoiceNumberchk =arr[i].Substring(12);
                    objInvoice.Finyear = Global.FinYear;
                    objInvoice.SystemName = System.Environment.MachineName;
                    objInvoice.OriginalPDF = Global.OriginalPDFFile;
                }
                count++;
            }

           objDSMModelData.SaveInvoice(lstInvoice);
        }

        public void AssignInvoiceNo_TruckNo(string src, string type)
        {
            string truckNumber = string.Empty;
            string pathToPdf = src;

            try
            {

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


                    }
                    if (custcount == 0)
                    {
                        MoveFile(pathToPdf, "OTHER INVOICE");
                        othercustomer("Other Customer Invoice");
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
                    string shopcdcoord = "";
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
                        shopcdcoord = coordinates[i].Shopcdcoord.ToString().Trim();
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
                            Global.InvoiceNumber = invresult;
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
                    if (shopcdcoord.Length > 0 && shopcdcoord.ToString().Trim() != "")
                    {
                        RenderFilter[] filters = new RenderFilter[1];
                        LocationTextExtractionStrategy regionFilter = new LocationTextExtractionStrategy();
                        // filters[0] = new RegionTextRenderFilter(new iTextSharp.text.Rectangle(160,70,220,82));
                        string[] arrvendorcodecoord = shopcdcoord.Split(',');
                        filters[0] = new RegionTextRenderFilter(new iTextSharp.text.Rectangle(int.Parse(arrvendorcodecoord[0]), int.Parse(arrvendorcodecoord[1]), int.Parse(arrvendorcodecoord[2]), int.Parse(arrvendorcodecoord[3])));
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
                        //cond.
                        String scd = Global.Shopcode;
                        if (!string.IsNullOrEmpty(scd))
                        {
                            string auth = scd.Substring(0, 2);
                            Global.Shopcode = auth;
                        }
                    }

                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogErrors(ex);
                MoveFile(pathToPdf, "INVALID_INVOICE");
                timer.Start();
            }
        }

        //public string GetFinancialYear(DateTime curDate)
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
        //            else
        //                vinvono = _from.ToString().Trim() + "ACKNOWLEDGEMENT" + invtype.ToString().Trim();
        //            Spire.Pdf.PdfDocument doc = new Spire.Pdf.PdfDocument(invoicePath);
        //            doc.FileInfo.IncrementalUpdate = false;
        //            doc.CompressionLevel = PdfCompressionLevel.Best;

        //            foreach (PdfPageBase page in doc.Pages)
        //            {
        //                if (page != null)
        //                {
        //                    if (page.ImagesInfo != null)
        //                    {
        //                        foreach (PdfImageInfo info in page.ImagesInfo)
        //                        {
        //                            page.TryCompressImage(info.Index);
        //                        }
        //                    }
        //                }

        //            }
        //            doc.SaveToFile(outputPath + vinvono + ".pdf");

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
        public void LogSignedDetails(String Data)
        {
            StringBuilder sb = new StringBuilder();
            string path = AppDomain.CurrentDomain.BaseDirectory + "OtherInvoiceDetails.txt";
            // flush every 20 seconds as you do it

            sb.AppendLine();
            sb.Append("Log Time: " + DateTime.Now);
            sb.AppendLine();
            sb.Append("--------------------------------------------------");
            sb.AppendLine();
            //if (ex != null)
            //{
            //    sb.Append("Exception: " + ex.ToString());
            //    if (ex.InnerException != null)
            //    {
            //        sb.Append("Inner Exception: " + ex.InnerException.ToString());
            //    }
            //}
            if (Data.ToString().Trim() != "" && Data.ToString().Trim() != null)
            {
                sb.Append(Global.CustomerName + " Customer Invoice No : " + Data.ToString().Trim() + " was Signed Successfully.");
            }
            sb.AppendLine();
            sb.Append("--------------------------------------------------");
            sb.AppendLine();

            File.AppendAllText(path, sb.ToString());
            sb.Clear();
        }

    }
}
