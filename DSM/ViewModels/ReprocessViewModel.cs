using DSMData;
using DSMData.Model;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using KeepDynamic.BarcodeReader;
using Microsoft.Practices.Unity;
using Microsoft.Win32;
using OnBarcode.Barcode.BarcodeScanner;
using PdfToImage;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Spire.Pdf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;
using ThoughtWorks.QRCode.Codec;
using ThoughtWorks.QRCode.Codec.Data;
using ThoughtWorks.QRCode.ExceptionHandler;

namespace DSM.ViewModels
{
    public class ReprocessViewModel : BindableBase, INotifyPropertyChanged
    {

        //[DllImport("user32")]
        //public static extern int SetCursorPos(int x, int y);

        //[DllImport("gsdll32.dll", EntryPoint = "gsapi_new_instance")]
        //public static void ReadPDF();

        public event PropertyChangedEventHandler PropertyChangeds;
       
        string QRCodeString = "";
     
        private void NotifyPropertyChanged([CallerMemberName()] string propertyName = null)
        {
            PropertyChangeds?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public DSMModelData objDSMModelData;
        public InvoiceDisplayModel objInvoice;
        public UserDisplayModel objUser;
        
        public List<InvoiceDisplayModel> lstInvoice;
        public List<ReprocessViewModel> lstView;

        private IRegionManager regionManager;
        private IUnityContainer container;
        public ICommand GoToAdminCommand { get; set; }
        public ICommand ReprintCommand { get; set; }

        public ICommand SearchCommand { get; set; }
        public ICommand BrowseCommand { get; set; }


        DigitalSign objDigitalSign = new DigitalSign();

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

        //string QRCodeString = "";
        bool isAssignedcoord = true;
        bool iskeycheck = true;
        bool isserverconnected = true;
        public ReprocessViewModel(IRegionManager regionManager, IUnityContainer uContainer)
        {
            //ProcessMsg = "Scanning under progress...";
            this.regionManager = regionManager;
            this.container = uContainer;
            RefreshLists();
            SearchCommand = new DelegateCommand(SearchInvNo);
            BrowseCommand = new DelegateCommand(Browse);

        }

        public void Closing(CancelEventArgs e)
        {
            e.Cancel = true;
        }

        public void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Your code to handle the event
            //App.Current.MainWindow.Close();
            //Application.Current.MainWindow.Close();
            //e.Cancel = true;
            //this.Visibility = Visibility.Hidden;
            //int loginvalue = 1;
            //EthernetConnectivityCheck();
            objDSMModelData.UpdateUserExitDetails(Global.UserName.ToString().Trim());
            foreach (Process proc in Process.GetProcessesByName("DSM"))
            {
                proc.Kill();
            }
        }
        public void Reprint()
        {
            //SettingsDisplayModel objSettings = new SettingsDisplayModel();
            //objSettings = objDSMModelData.GetSettingsData(System.Environment.MachineName);
            //if (objSettings != null)
            //{
            //    Global.SystemName = objSettings.SystemName;
            //    Global.InputPath = objSettings.InputPath;
            //    Global.InvoicePath = objSettings.InvoicePath;
            //    Global.OutputPath = objSettings.OutputPath;
            //    Global.APIInvoiceNumber = SelectedReprint.InvNumber;

            //    CustomerDisplayModel objCustomer = new CustomerDisplayModel();
            //    Global.CustomerName = "HYUNDAI";
            //    objCustomer = objDSMModelData.GetCustomerByName(Global.CustomerName);

            //    if (objCustomer != null)
            //    {
            //        Global.IsAPIpost = objCustomer.IsAPIpost;
            //        Global.APIpostType = "MANUAL";
            //        Global.APIurl = objCustomer.APIUrl;
            //        if (Global.APIurl == "https://hmieai.hmil.net:6004/Service.asmx?WSDL")
            //        {
            //            Global.APIPosturl = "http://hmieai/DI/data";
            //        }
            //        else
            //        {
            //            Global.APIPosturl = "http://hieai/KIMIL1/data";
            //        }
            //        APIPost();
            //    }
            //    else
            //    {
            //        MessageBox.Show("Client Settings not provided");
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("Settings not provided for this pc");
            //}


            //Global.APIInvoiceNumber = SelectedAPI.InvNumber;
            //EthernetConnectivityCheck();
            CustomerDisplayModel objCustomer = new CustomerDisplayModel();


            if (Global.UserType == "User")
            {


                string PrintInvNo = SelectedReprint.InvNumber;
                objCustomer = objDSMModelData.GetCustomerByInvNumber(PrintInvNo);

                Global.CustomerName = objCustomer.CustomerName;
                objCustomer = objDSMModelData.GetCustomerByName(Global.CustomerName);
                Global.PrintCopies = objCustomer.PrintCopies;
                Print objPrint = new Print();
                //var printStatus = objPrint.PrintInvoicePDFs(Global.InvoiceNumber, "invoice");
                var printStatus = objPrint.ReprintInvoicePDFs(PrintInvNo, "invoice");

            }
            else if (Global.UserType == "Api")
            {


                string PrintInvNo = SelectedReprint.InvNumber;

                Print objPrint = new Print();
                //var printStatus = objPrint.PrintInvoicePDFs(Global.InvoiceNumber, "invoice");
                var printStatus = objPrint.ReprintInvoicePDFs(PrintInvNo, "invoice");

            }
        }

        private void SearchInvNo()
        {
            objDSMModelData = new DSMModelData();
            objInvoice = new InvoiceDisplayModel();
            lstInvoice = new List<InvoiceDisplayModel>();

            SelectedReprint = new InvoiceDisplayModel();

            CurrentInvoice = "";



            objUser = new UserDisplayModel();

            if (Global.UserType == "admin")
            {
                //EthernetConnectivityCheck();
                AssignSettings();

                if (iskeycheck && isserverconnected && isAssignedcoord)
                {
                    RefreshLists();
                    StartProcess();
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
        }
        private void Browse()
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.ShowDialog();
            if (string.IsNullOrEmpty(openDialog.FileName)) return;

            SearchInvoiceNo = openDialog.FileName;
        }


        public void CheckApiExeRunOrNot()
        {
            //var lstApiInv = objDSMModelData.GetApiUserType("apipost");
            //objUser = null;
            //EthernetConnectivityCheck();
            var lstusers = objDSMModelData.GetApiUserType("apipost");
            for (int k = 0; k < lstusers.Count; k++)
            {
                if (lstusers[k].LoginStatus == "TRUE")
                {
                    HappyImgVisible = "Visible";
                    SadImgVisible = "Hidden";
                    isserverconnected = true;
                }
                else
                {
                    HappyImgVisible = "Hidden";
                    SadImgVisible = "Visible";

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

                if (keyConnected == false)
                {
                    MessageBox.Show("Check whether the key is connected or not!");
                    //App.Current.MainWindow.Close();
                    //for (int intCounter = App.Current.Windows.Count - 1; intCounter > 0; intCounter--)
                    //    App.Current.Windows[intCounter].Close();
                    ////App.Current.Windows[1].Close();
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
                                if (Global.APIurl == "https://hmieai.hmil.net:6004/Service.asmx?WSDL")
                                {
                                    Global.APIPosturl = "http://hmieai/DI/data";
                                }
                                else
                                {
                                    Global.APIPosturl = "http://hieai/KIMIL1/data";
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
                        if (Global.APIurl == "https://hmieai.hmil.net:6004/Service.asmx?WSDL")
                        {
                            Global.APIPosturl = "http://hmieai/DI/data";
                        }
                        else
                        {
                            Global.APIPosturl = "http://hieai/KIMIL1/data";
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

            //EthernetConnectivityCheck();
            var allInvoiceList = objDSMModelData.GetAllInvoiceStatus();
            //var allInvoiceList = objDSMModelData.GetAllSYSInvoiceStatus();

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

        }

        private void RefreshLists()
        {  //EthernetConnectivityCheck();
            //SearchInvoiceNo = "";
            objDSMModelData = new DSMModelData();
            var allInvoiceList = objDSMModelData.GetAllReprocessInvoiceStatus();
           // var allReprocessList= objDSMModelData.GetAllSYSInvoiceStatus();
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
                //allInvoiceList[0].DSStatus = "No Records Found";
                //allInvoiceList[0].PrintStatus = "No Records Found";
                //allInvoiceList[0].ApiStatus = "No Records Found";
                //List<InvoiceDisplayModel> inotfoundInvoiceList = new List<InvoiceDisplayModel>()
                //{
                //new InvoiceDisplayModel()
                //{
                //    DSStatus = "No Records Found",
                //    PrintStatus = "No Records Found",
                //    ApiStatus ="No Records Found"


                //}
                //};


                InvoiceList = new ObservableCollection<InvoiceDisplayModel>(allInvoiceList);
                //DataMsg = "No Records Found";
               
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
            //var allInvoiceList = objDSMModelData.GetAllInvoiceStatus();
            //var allInvoiceList = "1101";
            //QtList = new ObservableCollection<QuarantineDisplayModel>(objDSMModelData.GetQuarantineList());
            //for (int i = 0; i < allInvoiceList.Count; i++)
            //{
            //    if (allInvoiceList[i].DSStatus == "TRUE")
            //        allInvoiceList[i].DSStatus = "SIGNED";
            //    else
            //        allInvoiceList[i].DSStatus = "PENDING";

            //    if (allInvoiceList[i].PrintStatus == "TRUE")
            //        allInvoiceList[i].PrintStatus = "PRINTED";
            //    else
            //        allInvoiceList[i].PrintStatus = "PENDING";

            //    if (allInvoiceList[i].ApiStatus == "FALSE")
            //        allInvoiceList[i].ApiStatus = "PENDING";
            //}

            //InvNumber = x.InvNumber,
            //        InvDate = x.InvDate.Value,
            //        DSStatus = x.DSStatus,
            //        PrintStatus = x.PrintStatus,
            //        ApiStatus = x.APIStatus,
            //        ApiMsg = x.APIMessage,
            //        CreatedDate = x.UpdatedDate == null ? x.CreatedDate.Value : x.UpdatedDate.Value

            //List<InvoiceDisplayModel> CurInvList = new List<InvoiceDisplayModel>()
            //{
            //    //new InvoiceDisplayModel(){InvNumber = "001", DSStatus = "PENDING",PrintStatus="PENDING",ApiStatus="PENDING"}
            //    new InvoiceDisplayModel(){InvNumber = Global.InvoiceNumber,
            //        InvDate = Convert.ToDateTime(Global.InvoiceDate.ToString()),
            //        DSStatus = "PENDING",
            //        PrintStatus ="PENDING",
            //        ApiStatus ="PENDING"}

            //};

            //DateTime invdate= DateTime.ParseExact(Global.InvoiceDate.ToString().Replace(".", "/").Trim(), "d/M/yyyy", CultureInfo.InvariantCulture);
            //timer.Stop();
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
            //List<InvoiceDisplayModel> CurInvList = new List<InvoiceDisplayModel>()
            //{


            //};


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
            //
            DateTime invdate = DateTime.Now;
            //
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
            //

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
            //
            DateTime invdate = DateTime.Now;
            //
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

            //
            DateTime invdate = DateTime.Now;
            //
            if (Global.InvoiceDate != null)
            {
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
            }
            //timer.Start();
        }

        public void StartProcess()
        {
            //timer.Stop();

            CurrentRefreshLists();
            RefreshLists();
            CheckApiExeRunOrNot();
            ReadPDF();
            SignPDF();
            PrintFiles();
            APIPostFiles();
            CurrentRefreshLists();
            //timer.Start();

            //CurrentRefreshLists();
            //RefreshLists();

            //ReadPDF();
            //RefreshLists();
            //SignPDF();

            ////PrintFiles();
            //CurrentRefreshLists();

            //APIPostFiles();
            //RefreshLists();
        }



        private void APIPostFiles(object sender, EventArgs e)
        {
            try
            {
                //EthernetConnectivityCheck();

                //timerAPIPost.Stop();
                RefreshServerLists();
                DSMModelData objDSMModelData = new DSMModelData();

                ReprintCommand = new DelegateCommand(Reprint);

                int Finyear = 0;
                string prefix_invoice_no = "";
                int fin_mon = 0;
                string _sQry_Prefix_invoice_no = "select month(getdate()) as cur_month, FORMAT(getdate(), 'yy') as cur_year";
                SqlDataReader _rdr_Prefix_invoice_no = Global.executeQuery(_sQry_Prefix_invoice_no);
                if (_rdr_Prefix_invoice_no.Read())
                {
                    fin_mon = 3;
                    if (int.Parse(_rdr_Prefix_invoice_no["cur_month"].ToString()) > fin_mon)
                        prefix_invoice_no = _rdr_Prefix_invoice_no["cur_year"].ToString() + (int.Parse(_rdr_Prefix_invoice_no["cur_year"].ToString()) + 1);
                    else
                        prefix_invoice_no = (int.Parse(_rdr_Prefix_invoice_no["cur_year"].ToString()) - 1) + (_rdr_Prefix_invoice_no["cur_year"].ToString());
                }
                //_inv.Finyear = Convert.ToInt32(prefix_invoice_no.ToString().Trim());
                Finyear = Convert.ToInt32(prefix_invoice_no.ToString().Trim());

                Global.APIFinYear = Convert.ToInt32(prefix_invoice_no.ToString().Trim());

                var lstApiInv = objDSMModelData.GetAllAPIInvoiceStatusAll(Finyear).ToList();

                //for (var i = 0; i < lstPrint.Count; i++)
                //{
                for (int i = 0; i < lstApiInv.Count; i++)

                {
                    if (lstApiInv != null)
                    {
                        //Print objPrint = new Print();
                        //var printStatus = objPrint.PrintInvoicePDFs(lstApiInv.InvNumber, "invoice");
                        //objDSMModelData.UpdateInvoicePrintstatusByInvoiceNo(lstApiInv.InvNumber, printStatus == true ? "TRUE" : "FALSE");

                        //var lstapiinvno= objDSMModelData.get
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
                //}
                //timerAPIPost.Start();
                //timer.Start();
            }
            catch (Exception ex)
            {
                LogErrors(ex);

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
                //string _sQry_Prefix_invoice_no = "select month(getdate()) as cur_month, FORMAT(getdate(), 'yy') as cur_year";
                //SqlDataReader _rdr_Prefix_invoice_no = Global.executeQuery(_sQry_Prefix_invoice_no);
                //if (_rdr_Prefix_invoice_no.Read())
                //{
                //    fin_mon = 3;
                //    if (int.Parse(_rdr_Prefix_invoice_no["cur_month"].ToString()) > fin_mon)
                //        prefix_invoice_no = _rdr_Prefix_invoice_no["cur_year"].ToString() + (int.Parse(_rdr_Prefix_invoice_no["cur_year"].ToString()) + 1);
                //    else
                //        prefix_invoice_no = (int.Parse(_rdr_Prefix_invoice_no["cur_year"].ToString()) - 1) + (_rdr_Prefix_invoice_no["cur_year"].ToString());
                //}
                //_inv.Finyear = Convert.ToInt32(prefix_invoice_no.ToString().Trim());
                Finyear = Convert.ToInt32(prefix_invoice_no.ToString().Trim());

                Global.APIFinYear = Convert.ToInt32(prefix_invoice_no.ToString().Trim());

                var lstApiInv = objDSMModelData.GetAllAPIInvoiceStatusAll(Finyear).ToList();

                //for (var i = 0; i < lstPrint.Count; i++)
                //{
                for (int i = 0; i < lstApiInv.Count; i++)

                {
                    if (lstApiInv != null)
                    {
                        //Print objPrint = new Print();
                        //var printStatus = objPrint.PrintInvoicePDFs(lstApiInv.InvNumber, "invoice");
                        //objDSMModelData.UpdateInvoicePrintstatusByInvoiceNo(lstApiInv.InvNumber, printStatus == true ? "TRUE" : "FALSE");

                        //var lstapiinvno= objDSMModelData.get
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
                //}
                //timer.Start();
                //timerAPIPost.Start();
            }
            catch (Exception ex)
            {
                LogErrors(ex);
                //timer.Start();
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
                //if (Global.APIpostType == "MANUAL")
                //{
                //    MessageBox.Show("API will be posted Manually");
                //}
                //else if (Global.APIpostType == "AUTO")
                //{
                //    MessageBox.Show("Invoice Re-Posted Successfully");
                //}
            }
            catch (Exception ex)
            {
                LogErrors(ex);

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

            }
        }


        private void ReadPDF()
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
                            if (Global.CustomerName == "HYUNDAI")
                            {
                                System.IO.FileInfo input = new FileInfo(fileName);
                                string directoryPath = System.IO.Path.GetDirectoryName(fileName) + "\\" + System.IO.Path.GetFileNameWithoutExtension(input.Name);
                                if (!Directory.Exists(directoryPath))
                                    Directory.CreateDirectory(directoryPath);
                                Spire.Pdf.PdfDocument doc = new Spire.Pdf.PdfDocument();
                                doc.LoadFromFile(fileName);
                                System.Drawing.Image bmp = doc.SaveAsImage(0);
                                System.Drawing.Image emf = doc.SaveAsImage(0, Spire.Pdf.Graphics.PdfImageType.Metafile);
                                System.Drawing.Image zoomImg = new Bitmap((int)(emf.Size.Width * 2), (int)(emf.Size.Height * 2));
                                using (Graphics g = Graphics.FromImage(zoomImg))
                                {
                                    g.ScaleTransform(2.0f, 2.0f);
                                    g.DrawImage(emf, new System.Drawing.Rectangle(new System.Drawing.Point(0, 0), emf.Size), new System.Drawing.Rectangle(new System.Drawing.Point(0, 0), emf.Size), GraphicsUnit.Pixel);
                                }
                                zoomImg.Save(directoryPath + "\\" + input.Name + ".png", ImageFormat.Png);

                                string[] datas = OnBarcode.Barcode.BarcodeScanner.BarcodeScanner.Scan(directoryPath + "\\" + input.Name + ".png", BarcodeType.QRCode);
                                if (datas.Length > 0 && datas != null)
                                    QRCodeString = datas[0];

                                if (QRCodeString != "")
                                {
                                    SplitAndSave(invPath[0], Global.InputPath, Global.InvoiceNumber, "INVOICE");
                                }
                                else
                                {
                                    MoveFile(invPath[0], "INVALID_INVOICE");
                                   
                                }
                                if (Directory.Exists(directoryPath))
                                    Directory.Delete(directoryPath, true);


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
                
            }
        }
        //string[] inputPath = Directory.GetFiles(Global.InputPath, "*.pdf");
        //string[] outputPath = Directory.GetFiles(Global.OutputPath, "*.pdf");
        //string[] invPath = Directory.GetFiles(Global.InvoicePath, "*.pdf");
        //for (int j = 0; j < invPath.Length; j++)
        //{

        //        if (SearchInvoiceNo.Length > 0)
        //        {
        //            String fileName = SearchInvoiceNo;
        //            AssignInvoiceNo_TruckNo(SearchInvoiceNo, "INV");

        //            if (!string.IsNullOrEmpty(Global.InvoiceDate))
        //            {
        //                string FinYear = GetFinancialYear(Global.InvoiceDate.ToString().Trim());
        //                Global.FinYear = Convert.ToInt32(FinYear.ToString().Trim());
        //                CurrentRefreshInvoiceLists();
        //                //CurrentInvoiceList.Items.Refresh();
        //                //EthernetConnectivityCheck();
        //                objDSMModelData = new DSMModelData();
        //                int inv_count_rec = objDSMModelData.GetInvoiceDetailsByInvoiceNumberFinYear(Global.InvoiceNumber, Convert.ToInt32(FinYear)).Count;
        //                if (inv_count_rec >= 0)
        //                {
        //                    if (Global.CustomerName == "HYUNDAI")
        //                    {
        //                        //CurrentRefreshLists();
        //                        PDFConvert converter = new PDFConvert();
        //                        //Setup the converter
        //                        converter.RenderingThreads = -1;
        //                        converter.TextAlphaBit = -1;
        //                        converter.TextAlphaBit = -1;
        //                        converter.OutputToMultipleFile = true;
        //                        converter.FirstPageToConvert = -1;
        //                        converter.LastPageToConvert = -1;
        //                        converter.FitPage = false;
        //                        converter.JPEGQuality = 10;
        //                        converter.OutputFormat = "png256";
        //                        converter.ResolutionX = 600;
        //                        converter.ResolutionY = 600;
        //                        System.IO.FileInfo input = new FileInfo(fileName);
        //                        string directoryPath = System.IO.Path.GetDirectoryName(fileName) + "\\" + System.IO.Path.GetFileNameWithoutExtension(input.Name);
        //                        if (!Directory.Exists(directoryPath))
        //                            Directory.CreateDirectory(directoryPath);
        //                        string output = string.Format("{0}\\{1}{2}", directoryPath, System.IO.Path.GetFileNameWithoutExtension(input.Name), ".png");
        //                        converter.Convert(input.FullName, output);


        //                        try
        //                        {
        //                            QRCodeString = "";

        //                            QRCodeString = ScanQRPdf(directoryPath);

        //                            if (!string.IsNullOrEmpty(QRCodeString))
        //                            {
        //                                //CreateInvoice(QRCodeString);
        //                                SplitAndSave(SearchInvoiceNo, Global.InputPath, Global.InvoiceNumber, "INVOICE");
        //                                //Global.InvoiceNumber = null;
        //                                if (Directory.Exists(directoryPath))
        //                                    Directory.Delete(directoryPath, true);
        //                            }
        //                        }
        //                        catch (Exception i)
        //                        {
        //                            LogErrors(i);

        //                            //if (Directory.Exists(directoryPath))
        //                            //    Directory.Delete(directoryPath, true);

        //                            //MoveFile(invPath[0], "INVALID_PDF");
        //                            //objDSMModelData = new DSMModelData();

        //                            //if (Global.InvoiceNumber != null)
        //                            //{
        //                            //    objDSMModelData.SaveQuarantine(Global.InvoiceNumber);
        //                            //    Global.InvoiceNumber = null;
        //                            //}

        //                            //timer.Start();
        //                        }
        //                    }
        //                    else
        //                    {
        //                        try
        //                        {
        //                            if (!string.IsNullOrEmpty(Global.InvoiceNumber))
        //                            {
        //                                SplitAndSave(SearchInvoiceNo, Global.InputPath, Global.InvoiceNumber, "INVOICE");
        //                            }
        //                        }
        //                        catch (Exception i)
        //                        {
        //                            LogErrors(i);
        //                            //timer.Start();
        //                        }
        //                    }
        //                    //}
        //                    //else
        //                    //{
        //                    //    MoveFile(invPath[j], "INVALID_PDF_XML");
        //                    //    if (Directory.Exists(directoryPath))
        //                    //        Directory.Delete(directoryPath, true);
        //                    //}
        //                    //    }
        //                    //    else
        //                    //    {
        //                    //        MoveFile(invPath[j], "OTHER_CUSTOMER");
        //                    //    }
        //                    //}
        //                    //else
        //                    //{
        //                    //    MoveFile(invPath[j], "OTHER_CUSTOMER");
        //                    //}
        //                }
        //                else
        //                {
        //                    MoveFile(SearchInvoiceNo, "AlreadyExist");
        //                    CurrentRefreshAlreadyInvoiceLists();
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogErrors(ex);
        //        //timer.Start();
        //    }
        //}
        private void Log_xmlerr(string ex)
        {
            string path = Global.InvoicePath + "\\INVALID_INVOICE\\" + "XML_LOG_ERR.txt";
            if (!File.Exists(path))
            {
                StreamWriter sw = File.CreateText(path);
                sw.Close();
                sw.Dispose();
            }
            StringBuilder sb = new StringBuilder();
            //string path = AppDomain.CurrentDomain.BaseDirectory + "LogError.txt";
            // flush every 20 seconds as you do it

            sb.AppendLine();
            sb.Append("Log Time: " + DateTime.Now);
            sb.AppendLine();
            sb.Append("--------------------------------------------------");
            sb.AppendLine();
            if (ex != null)
            {
                sb.Append("Err Detail: " + ex.ToString());

            }
            sb.AppendLine();
            sb.Append("--------------------------------------------------");
            sb.AppendLine();

            File.AppendAllText(path, sb.ToString());
            sb.Clear();
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
                            string PdfFilename = Global.OutputPath + Global.InvoiceNumber + "ORIGINALinvoice.pdf";
                            //if (PdfFilename != null && File.Exists(PdfFilename))
                            if (PdfFilename != null && File.Exists(PdfFilename))
                            {

                                if (!string.IsNullOrEmpty(QRCodeString))
                                {


                                    CreateInvoice(QRCodeString);

                                }
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


                        //var lstSign = objDSMModelData.GetAllInvoiceStatusAll().Where(x => x.DSStatus == "TRUE" && x.IsEmailSend == false).FirstOrDefault();

                        //for (int i = 0; i < lstSign.Count; i++)
                        //{

                        //if (lstSign != null)
                        //{
                        //CurrentInvoice = "Invoice - " + lstSign.InvNumber + " is Signing...";
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
                    ////Global.IsMailSend = false;
                    //var lstSign = objDSMModelData.GetAllInvoiceStatusAll().Where(x => x.DSStatus == "TRUE" && x.IsEmailSend == false).FirstOrDefault();

                    //if (lstSign != null)
                    //{
                    //    CurrentInvoice = "Invoice - " + lstSign.InvNumber + " is Signing...";
                    //    objDigitalSign = new DigitalSign();
                    //    bool mailSent = objDigitalSign.SendEmail(lstSign.InvNumber);
                    //    if (mailSent == true)
                    //    {
                    //        objDSMModelData.UpdateEmailstatusByInvoiceNo(lstSign.InvNumber, true);
                    //    }
                    //    //RefreshLists();
                    //}





                    //var lstSign = objDSMModelData.GetAllInvoiceStatusAll().Where(x => x.DSStatus == "TRUE" && x.IsEmailSend == false).FirstOrDefault();

                    //for (int i = 0; i < lstSign.Count; i++)
                    //{

                    //if (lstSign != null)
                    //{
                    //CurrentInvoice = "Invoice - " + lstSign.InvNumber + " is Signing...";
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

                        if (Global.CustomerName == "HYUNDAI")
                        //if (Global.CustomerName != "")
                        {
                            string PdfFilename = Global.OutputPath + "PRINT" + @"\" + Global.InvoiceNumber + "ORIGINALinvoice.pdf";
                            //if (PdfFilename != null && File.Exists(PdfFilename))
                            if (PdfFilename != null && File.Exists(PdfFilename))
                            {

                                if (!string.IsNullOrEmpty(QRCodeString))
                                {


                                    CreateInvoice(QRCodeString);

                                }
                            }
                            //Global.CustomerName = "";
                        }
                        else if (Global.CustomerName != "")
                        //if (Global.CustomerName != "")
                        {

                            string PdfFilename = Global.OutputPath + "PRINT" + @"\" + Global.InvoiceNumber + "ORIGINALinvoice.pdf";
                            //if (PdfFilename != null && File.Exists(PdfFilename))
                            if (PdfFilename != null && File.Exists(PdfFilename))
                            {
                                OthercustInv();

                            }
                            Global.InvoiceNumber = "";
                            //Global.CustomerName = "";
                        }
                        else
                        {
                            return;
                        }
                        //}
                    }
                }
                //}
                //timerPrintPost.Start();
            }
            catch (Exception ex)
            {
                LogErrors(ex);
                //timerPrintPost.Start();
                //timer.Start();
            }
        }

        private string GetQRCodeString(System.Drawing.Image img, string outPutPath, string directory, string pdfPath)
        {
            string scaningResult = string.Empty;

            img.Save(outPutPath, System.Drawing.Imaging.ImageFormat.Jpeg);
            scaningResult = Spire.Barcode.BarcodeScanner.ScanOne(outPutPath);
            img.Dispose();
            return scaningResult;
        }

        private string ScanQRPdf(string imagePath)
        {
            //EthernetConnectivityCheck();
            ArrayList barcodes = new ArrayList();
            DateTime dtStart = DateTime.Now;
            String decodedString = string.Empty;
            var img = Directory.GetFiles(imagePath);
            if (System.IO.Path.GetExtension(img[0]).ToLower() == ".png")
            {
                Bitmap b = new Bitmap(img[0]);
                try
                {
                    //QRCodeDecoder decoder = new QRCodeDecoder();
                    //decodedString = decoder.decode(new QRCodeBitmapImage(b));

                    QRCodeDecoder decoder = new QRCodeDecoder();
                    //QRCodeDecoder.Canvas = new ConsoleCanvas();
                    decodedString = decoder.decode(new QRCodeBitmapImage(new Bitmap(b)));

                }

                catch (Exception ex)
                {
                    LogErrors(ex);
                    //if (Directory.Exists(imagePath))
                    //    Directory.Delete(imagePath, true);

                    //MoveFile(Global.InvoicePath, "INVALID_PDF");
                    //objDSMModelData = new DSMModelData();

                    //if (Global.InvoiceNumber != null)
                    //{
                    //    objDSMModelData.SaveQuarantine(Global.InvoiceNumber);
                    //    Global.InvoiceNumber = null;
                    //}

                    //timer.Start();
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
            bool reprocess = false;
            //int inv_count_rec1 = objDSMModelData.GetInvoiceDetailsToReprocess(Global.InvoiceNumber, po_no1, part_no1, qty1).Count;
          
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
                    objInvoice.IsEmailSend = Global.IsMailSend;
                    objInvoice.OriginalPDF = Global.OriginalPDFFile;
                    objInvoice.PdfGeneration = "Reprocessed";
                    lstInvoice.Add(objInvoice);
                }
                else
                {
                    objInvoice = new InvoiceDisplayModel();
                    objInvoice.ShopCode = arr[i].Substring(0, 2);
                    objInvoice.PONumber = arr[i].Substring(2, 10);
                    objInvoice.PartNumber = arr[i].Substring(12);
                    objInvoice.Finyear = Global.FinYear;
                    objInvoice.SystemName = System.Environment.MachineName;
                    objInvoice.OriginalPDF = Global.OriginalPDFFile;
                }
                count++;
            }
            int inv_count_rec1 = objDSMModelData.GetInvoiceDetailsToReprocess(Global.InvoiceNumber, objInvoice.PONumber, objInvoice.PartNumber, objInvoice.InvQuantity).Count;
           
            if (arr.Length == 2)
            {
                if (inv_count_rec1 == 0)
                {
                    reprocess = true;
                    objDSMModelData.UpdateOldInvoice(Global.InvoiceNumber);
                }
                else
                {
                    MoveFile(Global.InvoiceFilePath, "Already Exist");
                    return;
                }
            }
            if (reprocess)
                objDSMModelData.SaveInvoice(lstInvoice);
            else
                return;
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
                    if (Global.APIurl == "https://hmieai.hmil.net:6004/Service.asmx?WSDL")
                    {
                        Global.APIPosturl = "http://hmieai/DI/data";
                    }
                    else
                    {
                        Global.APIPosturl = "http://hieai/KIMIL1/data";
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
            }
            reader.Close();

        }

        //public string GetFinancialYear(DateTime curDate)
        public string GetFinancialYear(string curDate)
        {
            //int CurrentYear = curDate.Year;
            //int PreviousYear = (curDate.Year - 1);
            //int NextYear = (curDate.Year + 1);
            //string PreYear = PreviousYear.ToString();
            //string NexYear = NextYear.ToString();
            //string CurYear = CurrentYear.ToString();
            //string FinYear = null;



            //int CurrentYear = int.Parse(curDate.Substring(6, 4));
            //int PreviousYear = (int.Parse(curDate.Substring(6, 4)) - 1);
            //int NextYear = (int.Parse(curDate.Substring(6, 4)) + 1);
            int CurrentYear = int.Parse(curDate.Substring(8, 2));
            int PreviousYear = (int.Parse(curDate.Substring(8, 2)) - 1);
            int NextYear = (int.Parse(curDate.Substring(8, 2)) + 1);
            string PreYear = PreviousYear.ToString();
            string NexYear = NextYear.ToString();
            string CurYear = CurrentYear.ToString();
            string FinYear = null;
            string[] month = new string[5];
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

        //public void AssignInvoiceNo_TruckNo(string src, string type)
        //{
        //    string truckNumber = string.Empty;
        //    string pathToPdf = src;
        //    string pathToXml = Path.ChangeExtension(pathToPdf, ".xml");

        //    SautinSoft.PdfFocus f = new SautinSoft.PdfFocus();
        //    f.XmlOptions.ConvertNonTabularDataToSpreadsheet = true;
        //    f.GetPageSize(1);
        //    f.OpenPdf(pathToPdf);

        //    if (f.PageCount > 0)
        //    {
        //        int result = f.ToXml(pathToXml);

        //        //Show HTML document in browser
        //        if (result == 0)
        //        {
        //            //string truckNo = "Mode of Transport / Vehicle No. :";
        //            //string invNo = "No.:";
        //            //string invNo = "INV.NO:";
        //            XmlDocument doc = new XmlDocument();
        //            doc.Load(pathToXml);
        //            foreach (XmlNode xmlPage in doc.DocumentElement.SelectNodes("page"))
        //            {
        //                foreach (XmlNode xmlTable in xmlPage.SelectNodes("table"))
        //                {
        //                    foreach (XmlNode xmlRow in xmlTable.SelectNodes("row"))
        //                    {
        //                        if (type == "INV")
        //                        {
        //                            int countCell = 0;
        //                            bool invnoExist = false;
        //                            foreach (XmlNode xmlCell in xmlRow.SelectNodes("cell"))
        //                            {
        //                                countCell++;
        //                                if (xmlCell.InnerText == "No.:")
        //                                {
        //                                    invnoExist = true;
        //                                }

        //                                if (invnoExist == true)
        //                                {
        //                                    if (xmlRow.SelectNodes("cell").Count == countCell)
        //                                    {
        //                                        Global.InvoiceNumber = xmlCell.InnerText.Trim();
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        if (type == "TRUCK")
        //                        {
        //                            int counttruckCell = 0;
        //                            int counttruckCells = 0;
        //                            bool trucknoExist = false;
        //                            bool trucknoExist_new = false;

        //                            foreach (XmlNode xmlCell in xmlRow.SelectNodes("cell"))
        //                            {
        //                                counttruckCell++;
        //                                if (xmlCell.InnerText == "Vehicle No.")
        //                                {
        //                                    trucknoExist = true;
        //                                }
        //                                if (trucknoExist == true)
        //                                {
        //                                    if (counttruckCell == xmlRow.SelectNodes("cell").Count - 1)
        //                                    {
        //                                        Global.TruckNumber = xmlCell.InnerText;
        //                                    }
        //                                    if (Global.TruckNumber == "")
        //                                    {
        //                                        if (counttruckCell == xmlRow.SelectNodes("cell").Count)
        //                                        {
        //                                            Global.TruckNumber = xmlCell.InnerText;
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                            if (Global.TruckNumber == null)
        //                            {
        //                                foreach (XmlNode xmlCell in xmlRow.SelectNodes("cell"))
        //                                {
        //                                    counttruckCells++;
        //                                    if (xmlCell.InnerText == "Mode of Transport / Vehicle No.")
        //                                    {
        //                                        trucknoExist_new = true;
        //                                    }
        //                                    if (trucknoExist_new == true)
        //                                    {
        //                                        if (counttruckCells == xmlRow.SelectNodes("cell").Count)
        //                                        {
        //                                            Global.TruckNumber = xmlCell.InnerText;
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    f.ClosePdf();
        //    if (System.IO.File.Exists(pathToXml))
        //    {
        //        File.Delete(pathToXml);
        //    }
        //}

        public int SplitAndSave(string invoicePath, string outputPath, string tdocno_from, string invtype)
        {
            string _from = tdocno_from.ToString().Trim();

            FileInfo file = new FileInfo(invoicePath);
            string name = file.Name.Substring(0, file.Name.LastIndexOf("."));
            string[] printCopies = Global.PrintCopies.Split('-');
            int i = 0;
            using (iTextSharp.text.pdf.PdfReader pdfReader = new iTextSharp.text.pdf.PdfReader(invoicePath))
            {

                //for (int pageNumber = 1; pageNumber <= pdfReader.NumberOfPages; pageNumber++)
                //for (int pageNumber = 1; pageNumber <= 1; pageNumber++)
                for (int pageNumber = 1; pageNumber <= printCopies.Length; pageNumber++)
                {
                    //string vinvono = Convert.ToDecimal(_from).ToString("000000");
                    string vinvono = "";

                    if (printCopies[i] == "ORIGINAL")
                        vinvono = _from.ToString().Trim() + "ORIGINAL" + invtype.ToString().Trim();
                    else if (printCopies[i] == "DUPLICATE")
                        vinvono = _from.ToString().Trim() + "DUPLICATE" + invtype.ToString().Trim();
                    else if (printCopies[i] == "EXTRA")
                        vinvono = _from.ToString().Trim() + "EXTRA" + invtype.ToString().Trim();
                    else if (printCopies[i] == "QUADRAPLICATE")
                        vinvono = _from.ToString().Trim() + "QUADRAPLICATE" + invtype.ToString().Trim();
                    else
                        vinvono = _from.ToString().Trim() + "TRIPLICATE" + invtype.ToString().Trim();

                    //using (iTextSharp.text.pdf.PdfReader pdfReader = new iTextSharp.text.pdf.PdfReader(invoicePath))
                    //{
                    //    for (int pageNumber = 1; pageNumber <= pdfReader.NumberOfPages; pageNumber++)
                    //    //for (int pageNumber = 1; pageNumber <= 1; pageNumber++)
                    //    {
                    //        //string vinvono = Convert.ToDecimal(_from).ToString("000000");
                    //        string vinvono = "";

                    //        if (pageNumber == 1)
                    //            vinvono = _from.ToString().Trim() + "ORIGINAL" + invtype.ToString().Trim();
                    //        else if (pageNumber == 2)
                    //            vinvono = _from.ToString().Trim() + "DUPLICATE" + invtype.ToString().Trim();
                    //        else if (pageNumber == 3)
                    //            vinvono = _from.ToString().Trim() + "TRIPLICATE" + invtype.ToString().Trim();
                    //        else if (pageNumber == 4)
                    //            vinvono = _from.ToString().Trim() + "EXTRA" + invtype.ToString().Trim();
                    //        else
                    //            vinvono = _from.ToString().Trim() + "QUADRAPLICATE" + invtype.ToString().Trim();

                    // Get CGHS No.
                    iTextSharp.text.pdf.parser.ITextExtractionStrategy strategy = new iTextSharp.text.pdf.parser.SimpleTextExtractionStrategy();
                    string currentText = iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(pdfReader, 1, strategy);
                    // Get CGHS No from the pdf as per your pdf containt.
                    // string CGHSNo = currentText.Split(new string[] { "DEBIT NOTE" }, StringSplitOptions.None)[1].Split('\n')[0];


                    // Generate New pdf with CGHS No.
                    //string filename = "A"+pageNumber+".pdf";
                    iTextSharp.text.Document document = new iTextSharp.text.Document();
                    iTextSharp.text.pdf.PdfCopy pdfCopy = new iTextSharp.text.pdf.PdfCopy(document, new FileStream(outputPath + vinvono + ".pdf", FileMode.Create));

                    pdfCopy.SetPdfVersion(iTextSharp.text.pdf.PdfWriter.PDF_VERSION_1_2);

                    pdfCopy.CompressionLevel = iTextSharp.text.pdf.PdfStream.BEST_COMPRESSION;

                    //pdfCopy.CompressionLevel = PdfStream.DEFAULT_COMPRESSION;
                    pdfCopy.SetFullCompression();

                    document.Open();


                    pdfCopy.AddPage(pdfCopy.GetImportedPage(pdfReader, 1));

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

