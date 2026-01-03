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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

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
        private string _ShowDSMaster;
        public string ShowDSMaster
        {
            get { return _ShowDSMaster; }
            set { SetProperty(ref _ShowDSMaster, value); }
        }
        private string _ShowCustomer;
        public string ShowCustomer
        {
            get { return _ShowCustomer; }
            set { SetProperty(ref _ShowCustomer, value); }
        }
        private string _ShowInputLayout;
        public string ShowInputLayout
        {
            get { return _ShowInputLayout; }
            set { SetProperty(ref _ShowInputLayout, value); }
        }
        private string _ShowManualAPI;
        public string ShowManualAPI
        {
            get { return _ShowManualAPI; }
            set { SetProperty(ref _ShowManualAPI, value); }
        }
        private string _ShowQuarantine;
        public string ShowQuarantine
        {
            get { return _ShowQuarantine; }
            set { SetProperty(ref _ShowQuarantine, value); }
        }
        private string _ShowSignReports;
        public string ShowSignReports
        {
            get { return _ShowSignReports; }
            set { SetProperty(ref _ShowSignReports, value); }
        }

        private string _ShowReports;
        public string ShowReports
        {
            get { return _ShowReports; }
            set { SetProperty(ref _ShowReports, value); }
        }

        private string _ShowEDI;
        public string ShowEDI
        {
            get { return _ShowEDI; }
            set { SetProperty(ref _ShowEDI, value); }
        }

        private string _ShowSettings;
        public string ShowSettings
        {
            get { return _ShowSettings; }
            set { SetProperty(ref _ShowSettings, value); }
        }



        #endregion

        public MasterViewModel(IRegionManager regionManager)
        {
            if (Global.UserType.ToUpper() == "ADMIN")
            {
                ShowMenu = "Visible";

                ShowDSMaster = "True";
                ShowCustomer = "True";
                ShowInputLayout = "True";
                ShowManualAPI = "True";
                ShowQuarantine = "True";
                ShowSignReports = "True";
                ShowReports = "True";
                ShowEDI = "True";
                ShowSettings = "True";

            }
            else if(Global.UserType.ToUpper()=="REPORT")
            {
                ShowMenu = "Visible";

                ShowDSMaster = "False";
                ShowCustomer = "False";
                ShowInputLayout = "False";
                ShowManualAPI = "True";
                ShowQuarantine = "True";
                ShowSignReports = "True";
                ShowReports = "True";
                ShowEDI = "False";
                ShowSettings = "False";
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
        private string lastView = ""; // Default starting view

        public void PageNavigation(string sender  )
        {
            RemoveView();

            if (string.IsNullOrEmpty(sender))
            {
                // If sender is null or empty, revert to the last valid view
                regionManager.RegisterViewWithRegion("ChildRegion", GetViewType(lastView));
                return;
            }

            bool viewRegistered = false;

            switch (sender)
            {
                case "DashBoardView":
                    regionManager.RegisterViewWithRegion("ChildRegion", typeof(Views.DashBoardView));
                    viewRegistered = true;
                    break;
                case "SettingsView":
                    if (ShowSettings == "True")
                    {
                        regionManager.RegisterViewWithRegion("ChildRegion", typeof(Views.SettingsView));
                        viewRegistered = true;
                    }
                    break;
                case "EDIView":
                    if (ShowEDI == "True")
                    {
                        regionManager.RegisterViewWithRegion("ChildRegion", typeof(Views.EDIView));
                        viewRegistered = true;
                    }
                    break;
                case "DSMasterView":
                    if (ShowDSMaster == "True")
                    {
                        regionManager.RegisterViewWithRegion("ChildRegion", typeof(Views.DSMasterView));
                        viewRegistered = true;
                    }
                    break;
                case "CustomerMasterView":
                    if (ShowCustomer == "True")
                    {
                        regionManager.RegisterViewWithRegion("ChildRegion", typeof(Views.CustomerMasterView));
                        viewRegistered = true;
                    }
                    break;
                case "Input_Layout":
                    if (ShowInputLayout == "True")
                    {
                        regionManager.RegisterViewWithRegion("ChildRegion", typeof(Views.Input_Layout));
                        viewRegistered = true;
                    }
                    break;
                case "ReprocessView":
                    regionManager.RegisterViewWithRegion("ChildRegion", typeof(Views.ReprocessView));
                    viewRegistered = true;
                    break;
                case "ManualApiView":
                    if (ShowManualAPI == "True")
                    {
                        regionManager.RegisterViewWithRegion("ChildRegion", typeof(Views.ManualApiView));
                        viewRegistered = true;
                    }
                    break;
                case "ManualScanView":
                    if (ShowQuarantine == "True")
                    {
                        regionManager.RegisterViewWithRegion("ChildRegion", typeof(Views.ManualScanView));
                        viewRegistered = true;
                    }
                    break;
                case "ReportView":
                    if (ShowSignReports == "True")
                    {
                        regionManager.RegisterViewWithRegion("ChildRegion", typeof(Views.ReportView));
                        viewRegistered = true;
                    }
                    break;
                case "ReportReprintView":
                    if (ShowReports == "True")
                    {
                        regionManager.RegisterViewWithRegion("ChildRegion", typeof(Views.ReportReprintView));
                        viewRegistered = true;
                    }
                    break;
                default:
                    break;
            }

            // If the view was not successfully registered, revert to the last valid view
            if (!viewRegistered)
            {
                regionManager.RegisterViewWithRegion("ChildRegion", GetViewType(lastView));
            }
            else
            {
                // Update the last valid view
                lastView = sender;
            }
        }

        // Helper method to get the type of the view from its name
        private Type GetViewType(string viewName)
        {
            switch (viewName)
            {
                case "DashBoardView": return typeof(Views.DashBoardView);
                case "SettingsView": return typeof(Views.SettingsView);
                case "EDIView": return typeof(Views.EDIView);
                case "DSMasterView": return typeof(Views.DSMasterView);
                case "CustomerMasterView": return typeof(Views.CustomerMasterView);
                case "Input_Layout": return typeof(Views.Input_Layout);
                case "ReprocessView": return typeof(Views.ReprocessView);
                case "ManualApiView": return typeof(Views.ManualApiView);
                case "ManualScanView": return typeof(Views.ManualScanView);
                case "ReportView": return typeof(Views.ReportView);
                case "ReportReprintView": return typeof(Views.ReportReprintView);
                default: return typeof(Views.DashBoardView);
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
