using DSMData;
using DSMData.Model;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DSM.ViewModels
{
    public class ManualApiViewModel : BindableBase
    {
        DSMModelData objDSMModelData;
        public ICommand PostAPIKeyCommand { get; set; }


        private ObservableCollection<InvoiceDisplayModel> _invoiceList = new ObservableCollection<InvoiceDisplayModel>();
        public ObservableCollection<InvoiceDisplayModel> InvoiceList
        {
            get { return _invoiceList; }
            set { SetProperty(ref _invoiceList, value); }
        }

        private InvoiceDisplayModel _selectedAPI;
        public InvoiceDisplayModel SelectedAPI
        {
            get { return _selectedAPI; }
            set { SetProperty(ref _selectedAPI, value); }
        }

        private string _DataMsg;
        public string DataMsg
        {
            get { return _DataMsg; }
            set { SetProperty(ref _DataMsg, value); }
        }


        public ManualApiViewModel()
        {
            objDSMModelData = new DSMModelData();
            SelectedAPI = new InvoiceDisplayModel();
            PostAPIKeyCommand = new DelegateCommand(PostAPI);
            InvoiceList = new ObservableCollection<InvoiceDisplayModel>(objDSMModelData.GetAllManualInvoiceStatus().Where(
                    x => ((x.ApiStatus == "MANUAL" || x.ApiStatus == "SKIPPEDAPI") && x.DSStatus == "TRUE") ||
                    (x.DSStatus == "TRUE" && x.ApiStatus == "E" && x.ApiMsg != "I/F Success, ASN Not created,Create Manual ASN" && x.ApiMsg != "I/F Success, ASN Not cerated,Create Manual ASN")));
            //var allInvoiceList = objDSMModelData.GetAllReprocessInvoiceStatus();
            //var allInvoiceList = objDSMModelData.GetAllManualInvoiceStatus();
            //InvoiceList = new ObservableCollection<ManualApiViewModel>(allInvoiceList);
            DataMsg = "No Records Found";
        }

        public void PostAPI()
        {
            SettingsDisplayModel objSettings = new SettingsDisplayModel();
            objSettings = objDSMModelData.GetSettingsData(System.Environment.MachineName);
            if (objSettings != null)
            {
                Global.SystemName = objSettings.SystemName;
                Global.InputPath = objSettings.InputPath;
                Global.InvoicePath = objSettings.InvoicePath;
                Global.OutputPath = objSettings.OutputPath;
                Global.APIInvoiceNumber = SelectedAPI.InvNumber;
                //MessageBox.Show(SelectedAPI.InvNumber);

                CustomerDisplayModel objCustomer = new CustomerDisplayModel();
                Global.CustomerName = "HYUNDAI";
                objCustomer = objDSMModelData.GetCustomerByName(Global.CustomerName);

                if (objCustomer != null)
                {
                    Global.IsAPIpost = objCustomer.IsAPIpost;
                    Global.APIpostType = "MANUAL";
                    Global.APIurl = objCustomer.APIUrl;
                    if (Global.APIurl == "https://hmieai.hmil.net:5004/Service.asmx?WSDL")
                    {
                        Global.APIPosturl = "http://hmieai/DI/data";

                    }
                    else
                    {
                        Global.APIPosturl = "http://hmieai/DI/data";

                    }
                    // InvoiceList = new ObservableCollection<InvoiceDisplayModel>(objDSMModelData.GetAllManualInvoiceStatus().Where(
                    //x => (x.DSStatus == "TRUE") ||
                    //(x.DSStatus == "TRUE" && x.ApiStatus == "E" && x.ApiMsg != "I/F Success, ASN Not created,Create Manual ASN")));
                    APIPost();
                    InvoiceList = new ObservableCollection<InvoiceDisplayModel>(objDSMModelData.GetAllManualInvoiceStatus().Where(
                    x => ((x.ApiStatus == "MANUAL" || x.ApiStatus == "SKIPPEDAPI") && x.DSStatus == "TRUE") ||
                    (x.DSStatus == "TRUE" && x.ApiStatus == "E" && x.ApiMsg != "I/F Success, ASN Not created,Create Manual ASN")));
                }
                else
                {
                    MessageBox.Show("Client Settings not provided");
                }
            }
            else
            {
                MessageBox.Show("Settings not provided for this pc");
            }
        }

        public void APIPost()
        {
            try
            {
                ExampleAPIProxy exProxy = new ExampleAPIProxy();
                //WebService webService = new WebService("https://hmieai.hmil.net:94/KML.asmx?WSDL", "getData");
                //WebService webService = new WebService("https://hmieai.hmil.net:6004/Service.asmx?WSDL", "getData");
                WebService webService = new WebService(Global.APIurl, "getData");
                webService.Invoke();
                //objDSMModelData.UpdateInvoiceAPIstatusByInvoiceNoFinYear(Global.APIInvoiceNumber, "SKIPPEDAPI", "", 1920);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invoice - " + Global.APIInvoiceNumber + " Error Posting - " + ex.ToString());
            }
        }

        public void BULKAPIPost()
        {
            try
            {

                SettingsDisplayModel objSettings = new SettingsDisplayModel();
                objSettings = objDSMModelData.GetSettingsData(System.Environment.MachineName);
                if (objSettings != null)
                {
                    Global.SystemName = objSettings.SystemName;
                    Global.InputPath = objSettings.InputPath;
                    Global.InvoicePath = objSettings.InvoicePath;
                    Global.OutputPath = objSettings.OutputPath;


                    CustomerDisplayModel objCustomer = new CustomerDisplayModel();
                    Global.CustomerName = "HYUNDAI";
                    objCustomer = objDSMModelData.GetCustomerByName(Global.CustomerName);

                    if (objCustomer != null)
                    {
                        Global.IsAPIpost = objCustomer.IsAPIpost;
                        Global.APIpostType = "MANUAL";
                        Global.APIurl = objCustomer.APIUrl;
                        if (Global.APIurl == "https://hmieai.hmil.net:5004/Service.asmx?WSDL")
                        {
                            Global.APIPosturl = "http://hmieai/DI/data";
                        }
                        else
                        {
                            Global.APIPosturl = "http://hmieai/DI/data";
                        }
                        foreach (var Inv in InvoiceList)
                        {
                            Global.APIInvoiceNumber = Inv.InvNumber;
                            ExampleAPIProxy exProxy = new ExampleAPIProxy();
                            //WebService webService = new WebService("https://hmieai.hmil.net:94/KML.asmx?WSDL", "getData");
                            //WebService webService = new WebService("https://hmieai.hmil.net:6004/Service.asmx?WSDL", "getData");
                            WebService webService = new WebService(Global.APIurl, "getData");
                            webService.Invoke();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Client Settings not provided");
                    }
                }
                else
                {
                    MessageBox.Show("Settings not provided for this pc");
                }



            }
            catch (Exception ex)
            {
                MessageBox.Show("Invoice - " + Global.APIInvoiceNumber + " Error Posting - " + ex.ToString());
            }
        }
    }
}
