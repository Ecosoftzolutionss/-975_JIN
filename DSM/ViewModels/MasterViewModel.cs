using DSMData;
using DSMData.Model;
using PdfToImage;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;
using ThoughtWorks.QRCode.Codec;
using ThoughtWorks.QRCode.Codec.Data;

namespace DSM.ViewModels
{
    public class MasterViewModel : BindableBase
    {
        //[DllImport("user32")]
        //public static extern int SetCursorPos(int x, int y);

        //[DllImport("gsdll32.dll", EntryPoint = "gsapi_new_instance")]
        //public static void ReadPDF();


        private IRegionManager regionManager;
        public ICommand NavigationCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        List<object> views = new List<object>();
        List<object> removeViews = new List<object>();

        public DSMModelData objDSMModelData;
        public InvoiceDisplayModel objInvoice;
        public List<InvoiceDisplayModel> lstInvoice;
        DigitalSign objDigitalSign = new DigitalSign();
        DispatcherTimer timer = new DispatcherTimer();

        #region Properties

        private double isrowHeight;
        public double IsrowHight
        {
            get { return isrowHeight; }
            set { SetProperty(ref isrowHeight, value); }
        }

        private string _showMenu;
        public string ShowMenu
        {
            get { return _showMenu; }
            set { SetProperty(ref _showMenu, value); }
        }
        private string _ShowMaster;
        public string ShowMaster
        {
            get { return _ShowMaster; }
            set { SetProperty(ref _ShowMaster, value); }
        }
        private string _ShowSettings;
        public string ShowSettings
        {
            get { return _ShowSettings; }
            set { SetProperty(ref _ShowSettings, value); }
        }
        private string _ShowReprocess;
        public string ShowReprocess
        {
            get { return _ShowReprocess; }
            set { SetProperty(ref _ShowReprocess, value); }
        }
        private string _ShowManual;
        public string ShowManual
        {
            get { return _ShowManual; }
            set { SetProperty(ref _ShowManual, value); }
        }
        private string _ShowQuarantine;
        public string ShowQuarantine
        {
            get { return _ShowQuarantine; }
            set { SetProperty(ref _ShowQuarantine, value); }
        }
        private string _ShowReports;
        public string ShowReports
        {
            get { return _ShowReports; }
            set { SetProperty(ref _ShowReports, value); }
        }
        #endregion

        public MasterViewModel(IRegionManager regionManager)
        {
            if (Global.UserType.ToUpper() == "ADMIN")
            {
                ShowMenu = "Visible";
                ShowMaster = "True";
                ShowSettings = "True";
                ShowReprocess = "True";
                ShowManual = "True";
                ShowQuarantine = "True";
                ShowReports = "True";

                    
            }
            else if(Global.UserType.ToUpper()=="REPORT")
            {
                ShowMenu = "Visible";
                ShowMaster = "False";
                ShowSettings = "False";
                ShowReprocess = "False";
                ShowManual = "False";
                ShowQuarantine = "False";
                ShowReports = "True";
            }
            else
            {
                ShowMenu = "Hidden";
            }
            this.regionManager = regionManager;
            IsrowHight = SystemParameters.PrimaryScreenHeight - 100;
            NavigationCommand = new DelegateCommand<string>(PageNavigation);
            CloseCommand = new DelegateCommand(CloseAction);
            objDSMModelData = new DSMModelData();
            lstInvoice = new List<InvoiceDisplayModel>();
            objDSMModelData.UpdateUserEntryDetails(Global.UserName.ToString().Trim());
            Application.Current.MainWindow.Closing += new CancelEventHandler(MainWindow_Closing);
        }

        public void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Your code to handle the event
            //App.Current.MainWindow.Close();
            //Application.Current.MainWindow.Close();
            //e.Cancel = true;
            //this.Visibility = Visibility.Hidden;
            //int loginvalue = 1;
            objDSMModelData.UpdateUserExitDetails(Global.UserName.ToString().Trim());
            foreach (Process proc in Process.GetProcessesByName("DSM"))
            {
                proc.Kill();
            }
        }
        public void PageNavigation(string sender)
        {
            RemoveView();

            if (sender == null)
                return;

            switch (sender)
            {
                case "DashBoardView":
                    regionManager.RegisterViewWithRegion("ChildRegion", typeof(Views.DashBoardView));
                    break;
                case "SettingsView":
                    regionManager.RegisterViewWithRegion("ChildRegion", typeof(Views.SettingsView));
                    break;
                case "EDIView":
                    regionManager.RegisterViewWithRegion("ChildRegion", typeof(Views.EDIView));
                    break;
                case "DSMasterView":
                    regionManager.RegisterViewWithRegion("ChildRegion", typeof(Views.DSMasterView));
                    break;
                case "CustomerMasterView":
                    regionManager.RegisterViewWithRegion("ChildRegion", typeof(Views.CustomerMasterView));
                    break;
                case "Input_Layout":
                    
                    regionManager.RegisterViewWithRegion("ChildRegion", typeof(Views.Input_Layout));
                    break;
                case "ReprocessView":

                    regionManager.RegisterViewWithRegion("ChildRegion", typeof(Views.ReprocessView));
                    break;


                case "ManualApiView":
                    regionManager.RegisterViewWithRegion("ChildRegion", typeof(Views.ManualApiView));
                    break;
                case "ManualScanView":
                    regionManager.RegisterViewWithRegion("ChildRegion", typeof(Views.ManualScanView));
                    break;
                case "ReportView":
                    regionManager.RegisterViewWithRegion("ChildRegion", typeof(Views.ReportView));
                    break;
                case "ReportReprintView":
                    regionManager.RegisterViewWithRegion("ChildRegion", typeof(Views.ReportReprintView));
                    break;
                default:
                    return;
            }
        }

        private void RemoveView()
        {
            views = new List<object>();
            removeViews = new List<object>();
            if (regionManager.Regions["ChildRegion"] != null)
            {
                foreach (object view in regionManager.Regions["ChildRegion"].ActiveViews)
                {
                    views.Add(view);
                }
                for (int i = 0; i < views.Count; i++)
                {

                    removeViews.Add(views[i]);
                }

                if (removeViews.Count > 0)
                    for (int i = 0; i < removeViews.Count; i++)
                    {
                        regionManager.Regions["ChildRegion"].Remove(views[i]);
                    }
            }
        }

        public void CloseAction()
        {
            App.Current.Windows[0].Close();
            objDSMModelData.UpdateUserExitDetails(Global.UserName.ToString().Trim());
            foreach (Process proc in Process.GetProcessesByName("DSM"))
            {
                proc.Kill(); 
            }
        }
    }
}
