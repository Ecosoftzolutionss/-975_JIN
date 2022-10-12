using DSMData;
using DSMData.Model;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DSM.ViewModels
{
    public class CustomerMasterViewModel : BindableBase
    {

        public ICommand CreateClientCommand { get; set; }
        public ICommand CheckPDF { get; set; }
        public ICommand CheckPrint { get; set; }
        public ICommand CheckAPI { get; set; }
        public ICommand CheckDS { get; set; }
        public ICommand AddDSKeyCommand { get; set; }
        public ICommand RemoveDSKeyCommand { get; set; }
        public ICommand DeleteCustomerKeyCommand { get; set; }
        public ICommand EditCustomerKeyCommand { get; set; }
        public ICommand ChkTemp { get; set; }
        
        DSMModelData objDSMModelData;

        private string _isAPI;
        public string IsAPI
        {
            get { return _isAPI; }
            set { SetProperty(ref _isAPI, value); }
        }

        private string _isPrintMultiple;
        public string IsPrintMultiple
        {
            get { return _isPrintMultiple; }
            set { SetProperty(ref _isPrintMultiple, value); }
        }

        private string _isDSKeyAdd;
        public string IsDSKeyAdd
        {
            get { return _isDSKeyAdd; }
            set { SetProperty(ref _isDSKeyAdd, value); }
        }

        private CustomerDisplayModel objCustomer;
        public CustomerDisplayModel Customer
        {
            get { return objCustomer; }
            set { SetProperty(ref objCustomer, value); }
        }

        private DigitalSignDetailDisplayModel dSDet;
        public DigitalSignDetailDisplayModel DSDet
        {
            get { return dSDet; }
            set { SetProperty(ref dSDet, value); }
        }

        private CustomerDisplayModel objPDF;
        public CustomerDisplayModel PDF
        {
            get { return objPDF; }
            set { SetProperty(ref objPDF, value); }
        }

        private ObservableCollection<CustomerDisplayModel> lstpdfType;
        public ObservableCollection<CustomerDisplayModel> ListPDFType
        {
            get { return lstpdfType; }
            set { SetProperty(ref lstpdfType, value); }
        }

        private ObservableCollection<CustomerDisplayModel> listCustomer;
        public ObservableCollection<CustomerDisplayModel> ListCustomer
        {
            get { return listCustomer; }
            set { SetProperty(ref listCustomer, value); }
        }

        private CustomerDisplayModel selectedCustomer;
        public CustomerDisplayModel SelectedCustomer
        {
            get { return selectedCustomer; }
            set { SetProperty(ref selectedCustomer, value); }
        }

        private ObservableCollection<CustomerDisplayModel> ListTemplate;
        public ObservableCollection<CustomerDisplayModel> listTemplate
        {
            get { return ListTemplate; }
            set { SetProperty(ref ListTemplate, value); }
        }

        private CustomerDisplayModel selectedTemplate;
        public CustomerDisplayModel SelectedTemplate
        {
            get { return selectedTemplate; }
            set { SetProperty(ref selectedTemplate, value); }
        }
        private ObservableCollection<CustomerDisplayModel> _Templates;
        public ObservableCollection<CustomerDisplayModel> Templates
        {
            get { return _Templates; }
            set
            {
                SetProperty(ref _Templates, value);
                //this.OnPropertyChanged("customer");
            }
        }

        private CustomerDisplayModel _Template;
        public CustomerDisplayModel Template
        {
            get { return _Template; }
            set
            {
                _Template = value;
                //NotifyPropertyChanged("Customer");
                RaisePropertyChanged("Template");
            }
        }
        private ObservableCollection<DSMasterDisplayModel> lstDS;
        public ObservableCollection<DSMasterDisplayModel> ListDS
        {
            get { return lstDS; }
            set { SetProperty(ref lstDS, value); }
        }
        //private ObservableCollection<DigitalSignDetailDisplayModel> lstDS;
        //public ObservableCollection<DigitalSignDetailDisplayModel> ListDS
        //{
        //    get { return lstDS; }
        //    set { SetProperty(ref lstDS, value); }
        //}

        private DSMasterDisplayModel _SelectedDS;
        public DSMasterDisplayModel SelectedDS
        {
            get { return _SelectedDS; }
            set { _SelectedDS = value; }
        }

        //private DigitalSignDetailDisplayModel _SelectedDS;
        //public DigitalSignDetailDisplayModel SelectedDS
        //{
        //    get { return _SelectedDS; }
        //    set { _SelectedDS = value; }
        //}


        private ObservableCollection<DSMasterDisplayModel> _DSKeys;
        public ObservableCollection<DSMasterDisplayModel> DSKeys
        {
            get { return _DSKeys; }
            set { _DSKeys = value; }
        }

        private DSMasterDisplayModel _DSKey;
        public DSMasterDisplayModel DSKey
        {
            get { return _DSKey; }
            set { _DSKey = value; }
        }

        //private DigitalSignDetailDisplayModel _DSKey;
        //public DigitalSignDetailDisplayModel DSKey
        //{
        //    get { return _DSKey; }
        //    set { _DSKey = value; }
        //}

        private string errorApiUrl;
        public string ErrorApiUrl
        {
            get { return errorApiUrl; }
            set { SetProperty(ref errorApiUrl, value); }
        }

        //private string errorCustomerId;
        //public string ErrorCustomerId
        //{
        //    get { return errorCustomerId; }
        //    set { SetProperty(ref errorCustomerId, value); }
        //}

        public CustomerMasterViewModel()
        {
            Customer = new CustomerDisplayModel();
            CreateClientCommand = new DelegateCommand(CreateClient);
            CheckPDF = new DelegateCommand(CheckPDFtype);
            CheckPrint = new DelegateCommand(CheckPrintCopies);
            CheckAPI = new DelegateCommand(CheckAPIPost);
            CheckDS = new DelegateCommand(CheckDSKey);
            AddDSKeyCommand = new DelegateCommand(AddDSKey);
            RemoveDSKeyCommand = new DelegateCommand(RemoveDSKey);
            DeleteCustomerKeyCommand = new DelegateCommand(DeleteCustomer);
            EditCustomerKeyCommand = new DelegateCommand(EditCustomer);
            objDSMModelData = new DSMModelData();
            ListDS = new ObservableCollection<DSMasterDisplayModel>();
            //ListDS = new ObservableCollection<DigitalSignDetailDisplayModel>();
            ListCustomer = new ObservableCollection<CustomerDisplayModel>(objDSMModelData.GetAllCustomers());
            listTemplate = new ObservableCollection<CustomerDisplayModel>(objDSMModelData.GetInputTemplates());
            Template = new CustomerDisplayModel();
            Templates = new ObservableCollection<CustomerDisplayModel>(objDSMModelData.GetInputTemplates());
            AssignClientSettings();
        }

        private void AddDSKey()
        {
            var checkDS = ListDS.Where(x => x.KeyName == DSKey.KeyName).FirstOrDefault();
            if (checkDS == null)
            {
                //ListDS.Add(DSKey);
                //ListDS.Add(checkDS.KeyName);
                //objDSMModelData.DeleteCustomerDSDet(SelectedDS.ClientId);
                //ListDS.Remove(SelectedDS);
                //objDSMModelData.DeleteDSKey(SelectedDS.DSMasterId);
                //ListDS = new ObservableCollection<DigitalSignDetailDisplayModel>(objDSMModelData.GetAllDSKeys());
                //ListDS = new ObservableCollection<DigitalSignDetailDisplayModel>(objDSMModelData.GetDSdetailbyClientLineId(SelectedCustomer.ClientLineId));
                ListDS.Add(DSKey);
                //ListDS.Add()
            }
            else
            {
                MessageBox.Show("Key already added");
            }
        }

        private void RemoveDSKey()
        {
            if (SelectedDS != null)
            {
                //dSDet.DSDetailsId = 15;
                //objDSMModelData.DeleteCustomerDSDet(Customer.CustomerId);
                //objDSMModelData.DeleteCustomerDSDet(Customer.CustomerId, DSDet.DSDetailsId);
                objDSMModelData.DeleteCustomerDSDet(Customer.CustomerId, SelectedDS.KeyName);
                //ListDS.Remove(SelectedDS);
                //objDSMModelData.DeleteDSKey(SelectedDS.DSMasterId);
                //ListDS = new ObservableCollection<DigitalSignDetailDisplayModel>(objDSMModelData.GetAllDSKeys());
                ListDS = new ObservableCollection<DSMasterDisplayModel>(objDSMModelData.GetDSdetailbyClientLineId(SelectedCustomer.ClientLineId));
            }
        }

        private void EditCustomer()
        {

            Templates.Clear();
            //Customers
            Template = new CustomerDisplayModel();
            Templates = new ObservableCollection<CustomerDisplayModel>(objDSMModelData.GetInputTemplates());
            Template.Tempname = SelectedCustomer.Tempname;
            Templates.Add(Template);
            ////if(selectedCustomer.Tempname == null)
            ////Templates = new ObservableCollection<CustomerDisplayModel>(objDSMModelData.GetInputTemplates());
            if (SelectedCustomer.PDFtype == true)
                SelectedCustomer.IsSingle = true;
            else
                SelectedCustomer.IsMultiple = true;

            if (SelectedCustomer.PrintType == true)
            {
                SelectedCustomer.IsOriginalCopy = true;
                IsPrintMultiple = "Hidden";
            }
            else
            {
                SelectedCustomer.IsAllCopy = true;
                if (!string.IsNullOrEmpty(SelectedCustomer.PrintCopies))
                {
                    if (SelectedCustomer.PrintCopies.Contains("ORIGINAL"))
                        SelectedCustomer.IsOriginal = true;
                    if (SelectedCustomer.PrintCopies.Contains("DUPLICATE"))
                        SelectedCustomer.IsDuplicate = true;
                    if (SelectedCustomer.PrintCopies.Contains("TRIPLICATE") || SelectedCustomer.PrintCopies.Contains("QUADRAPLICATE"))
                        SelectedCustomer.IsTrplicate = true;
                    if (SelectedCustomer.PrintCopies.Contains("EXTRA"))
                        SelectedCustomer.IsExtra = true;
                }
                IsPrintMultiple = "Visible";
            }

            if (SelectedCustomer.IsDigitalSign == true)
            {
                IsDSKeyAdd = "Visible";
                ListDS = new ObservableCollection<DSMasterDisplayModel>(objDSMModelData.GetDSdetailbyClientLineId(SelectedCustomer.ClientLineId));
                //ListDS = new ObservableCollection<DigitalSignDetailDisplayModel>(objDSMModelData.GetDSdetailbyClientLineId(SelectedCustomer.ClientLineId));
            }
            else
            {
                IsDSKeyAdd = "Hidden";
            }

            if (SelectedCustomer.IsAPIpost == true)
            {
                IsAPI = "Visible";

                if (SelectedCustomer.APItype == true)
                {
                    SelectedCustomer.APIAuto = true;
                }
                else
                {
                    SelectedCustomer.APIManual = true;
                }
            }
            else
            {
                IsAPI = "Hidden";
            }

            Customer = new CustomerDisplayModel();
            Customer = SelectedCustomer;
        }

        //private void ResetCustomer()
        //{
        //    //DS = new DSMasterDisplayModel();
        //    ErrorCustomerId = "";
        //    //ErrorKey = "";
        //    //ErrorDSpin = "";
        //    //ErrorDept = "";
        //    //ErrorDesgn = "";
        //}

        private void DeleteCustomer()
        {
            if (SelectedCustomer != null)
            {
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure, Do you want to Delete it?", "Delete Confirmation", System.Windows.MessageBoxButton.OKCancel);
                if (messageBoxResult == MessageBoxResult.OK)
                {
                    objDSMModelData.DeleteCustomer(SelectedCustomer.CustomerId);
                    ListCustomer.Remove(SelectedCustomer);
                }
            }
        }

        private void CreateClient()
        {
            //if (Customer.CustomerId == "" || Customer.CustomerId.Length == 0 || Customer.CustomerId.ToString().Trim() == null)
            //{
            //    ErrorCustomerId = "Enter Nickname to browse key!";
            //}
            //else
            //{

            //}
            if (Customer.CustomerId != null && Customer.CustomerName != null && (Customer.IsOriginalCopy == true || Customer.IsAllCopy == true) && Template.Tempname != null)
            {
                if (Customer.CustomerId != "" && Customer.CustomerId.Length > 0)
                {
                    Customer.Tempname = Template.Tempname;
                    if (Customer.IsOriginalCopy == true)
                    {
                        Customer.PrintCopies = "ORIGINAL";
                    }
                    if (Customer.IsAllCopy == true)
                    {
                        Customer.PrintCopies = string.Empty;
                        Customer.PrintCopies += Customer.IsOriginal == true ? "ORIGINAL-" : "";
                        Customer.PrintCopies += Customer.IsDuplicate == true ? "DUPLICATE-" : "";
                        Customer.PrintCopies += Customer.IsTrplicate == true ? "TRIPLICATE-" : "";
                        Customer.PrintCopies += Customer.IsExtra == true ? "EXTRA-" : "";
                        Customer.PrintCopies = Customer.PrintCopies.Remove(Customer.PrintCopies.Length - 1, 1);
                    }
                    if (Customer.IsAPIpost)
                    {
                        Customer.APItype = Customer.APIAuto == true ? true : false;
                    }

                    if (Customer.IsAPIpost == true)
                    {
                        if (Customer.APIUrl != null && Customer.APIUrl != "" && Customer.APIUrl.ToString().Length > 0)
                        {
                            objDSMModelData.SaveCustomer(Customer, ListDS.ToList());
                        }
                    }
                    else
                    {
                        objDSMModelData.SaveCustomer(Customer, ListDS.ToList());
                    }
                    Customer = new CustomerDisplayModel();
                    ListCustomer = new ObservableCollection<CustomerDisplayModel>(objDSMModelData.GetAllCustomers());
                    ListDS = new ObservableCollection<DSMasterDisplayModel>();
                    Template = new CustomerDisplayModel();
                    Templates = new ObservableCollection<CustomerDisplayModel>(objDSMModelData.GetInputTemplates());

                    //Template.Tempname = "";
                    //ListDS = new ObservableCollection<DigitalSignDetailDisplayModel>();
                }
                else
                {
                    //ErrorCustomerId = "Enter Nickname to browse key!";
                    ListCustomer = new ObservableCollection<CustomerDisplayModel>(objDSMModelData.GetAllCustomers());
                }
            }
            else
            {
                ListCustomer = new ObservableCollection<CustomerDisplayModel>(objDSMModelData.GetAllCustomers());
                MessageBox.Show("Please Enter Mandatory Fields");
            }
        }

        private void CheckPDFtype()
        {
            if (Customer.IsSingle == true)
            {
                Customer.PDFtype = true;
            }
            if (Customer.IsMultiple == true)
            {
                Customer.PDFtype = false;
            }
        }

        private void CheckPrintCopies()
        {
            if (Customer.IsOriginalCopy == true)
            {
                IsPrintMultiple = "Hidden";
            }
            if (Customer.IsAllCopy == true)
            {
                IsPrintMultiple = "Visible";
            }
        }

        private void CheckAPIPost()
        {
            if (Customer.IsAPIpost == true)
            {
                IsAPI = "Visible";
                ErrorApiUrl = "*";
            }
            else
            {
                IsAPI = "Hidden";
                ErrorApiUrl = "";
            }
        }

        private void CheckDSKey()
        {
            if (Customer.IsDigitalSign == true)
            {
                IsDSKeyAdd = "Visible";
            }
            else
            {
                IsDSKeyAdd = "Hidden";
            }
        }

        private void AssignClientSettings()
        {
            IsAPI = "Hidden";
            IsPrintMultiple = "Hidden";
            IsDSKeyAdd = "Hidden";
            DSKeys = new ObservableCollection<DSMasterDisplayModel>(objDSMModelData.GetAllDSKeys());
        }
    }
}
