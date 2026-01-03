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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DSM.ViewModels
{
    public class EDIViewModel : BindableBase, INotifyPropertyChanged
    {
        #region Properties

        public ICommand CheckEDICommand { get; set; }
        public ICommand CheckEmailCommand { get; set; }
        public ICommand EDISaveCommand { get; set; }
        public ICommand EDIEditCommand { get; set; }
        public ICommand EDIDeleteCommand { get; set; }
        public ICommand AddEmailCommand { get; set; }
        public ICommand RemoveEmailCommand { get; set; }
        public ICommand EDIClearCommand { get; set; }

        DSMModelData objDSMModelData;

        public event PropertyChangedEventHandler PropertyChangeds;

        private void NotifyPropertyChanged([CallerMemberName()] string propertyName = null)
        {
            PropertyChangeds?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private ObservableCollection<CustomerDisplayModel> _Customers;
        public ObservableCollection<CustomerDisplayModel> Customers
        {
            get { return _Customers; }
            set
            {
                SetProperty(ref _Customers, value);
                //this.OnPropertyChanged("customer");
            }
        }

        private CustomerDisplayModel _customer;
        public CustomerDisplayModel Customer
        {
            get { return _customer; }
            set
            {
                _customer = value;
                //NotifyPropertyChanged("Customer");
                RaisePropertyChanged("Customer");
            }
        }

        private EdiDisplayModel _edi;
        public EdiDisplayModel EDI
        {
            get { return _edi; }
            set { SetProperty(ref _edi, value); }
        }

        private ObservableCollection<EdiDisplayModel> _lstEDI;
        public ObservableCollection<EdiDisplayModel> ListEDI
        {
            get { return _lstEDI; }
            set { SetProperty(ref _lstEDI, value); }
        }

        private ObservableCollection<EmailDisplayModel> _lstEmail;
        public ObservableCollection<EmailDisplayModel> ListEmail
        {
            get { return _lstEmail; }
            set { SetProperty(ref _lstEmail, value); }
        }

        private EmailDisplayModel _selectedemail;
        public EmailDisplayModel SelectedEmail
        {
            get { return _selectedemail; }
            set { SetProperty(ref _selectedemail, value); }
        }

        private EdiDisplayModel _selectededi;
        public EdiDisplayModel SelectedEDI
        {
            get { return _selectededi; }
            set { SetProperty(ref _selectededi, value); }
        }

        private string _EdiEnable;
        public string EdiEnable
        {
            get { return _EdiEnable; }
            set { SetProperty(ref _EdiEnable, value); }
        }

        private string _EmailEnable;
        public string EmailEnable
        {
            get { return _EmailEnable; }
            set { SetProperty(ref _EmailEnable, value); }
        }

        private string _SelectedCustomer;
        public string SelectedCustomer
        {
            get { return _SelectedCustomer; }
            set
            {
                _SelectedCustomer = value;
                NotifyPropertyChanged("SelectedCustomer");
            }
        }

        #endregion Properties

        public EDIViewModel()
        {
            EdiEnable = "False";
            EmailEnable = "False";
            objDSMModelData = new DSMModelData();
            CheckEDICommand = new DelegateCommand(CheckEDI);
            CheckEmailCommand = new DelegateCommand(CheckEmail);
            //EDISaveCommand = new DelegateCommand(EDISave);
            EDISaveCommand = new DelegateCommand<object>(EDISave);
            EDIEditCommand = new DelegateCommand(EDIEdit);
            EDIClearCommand = new DelegateCommand(EDIClear);
            //EDIEditCommand = new DelegateCommand<object>(EDIEdit);
            EDIDeleteCommand = new DelegateCommand(EDIDelete);
            AddEmailCommand = new DelegateCommand(AddEmail);
            RemoveEmailCommand = new DelegateCommand(RemoveEmail);
            EDI = new EdiDisplayModel();
            Customer = new CustomerDisplayModel();
            Customers = new ObservableCollection<CustomerDisplayModel>(objDSMModelData.GetAllCustomers());
            SelectedEDI = new EdiDisplayModel();
            ListEDI = new ObservableCollection<EdiDisplayModel>(objDSMModelData.GetAllEdi());
            ListEmail = new ObservableCollection<EmailDisplayModel>();
        }

        private void AddEmail()
        {
            if (EDI.VarFromEmail != null)
            {
                if (Regex.IsMatch(EDI.VarFromEmail, @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"))
                {
                    if (EDI.VarFromEmail.Trim() != "")
                    {
                        var checkEmail = ListEmail.Where(x => x.Mail == EDI.VarFromEmail.Trim()).FirstOrDefault();
                        if (checkEmail == null)
                        {
                            EmailDisplayModel objEmail = new EmailDisplayModel();
                            objEmail.Mail = EDI.VarFromEmail.Trim();
                            ListEmail.Add(objEmail);

                        }
                        else
                        {
                            MessageBox.Show("Email already added");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Invalid Email");
                }
            }
            else
            {
                MessageBox.Show("Enter Email");
            }
        }

        private void RemoveEmail()
        {
            if (SelectedEmail != null)
                ListEmail.Remove(SelectedEmail);
        }

        public void CheckEDI()
        {
            if (EDI.IsEdi)
                EdiEnable = "True";
            else
                EdiEnable = "False";
        }

        public void CheckEmail()
        {
            if (EDI.IsEmail)
                EmailEnable = "True";
            else
                EmailEnable = "False";
        }

        public void EDIEdit()
        {

            if (SelectedEDI.IsEmail)
                EmailEnable = "True";
            else
                EmailEnable = "False";

            if (SelectedEDI.IsEdi)
                EdiEnable = "True";
            else
                EdiEnable = "False";

            EDI = SelectedEDI;


            ListEmail = new ObservableCollection<EmailDisplayModel>();
            string[] arrEmail = EDI.Email.Split(',');

            for (int i = 0; i < arrEmail.Length; i++)
            {
                EmailDisplayModel objEmail = new EmailDisplayModel();
                objEmail.Mail = arrEmail[i];
                ListEmail.Add(objEmail);
            }

            //Customer = new CustomerDisplayModel();
            Customer.CustomerId = EDI.CustomerId;
            Customer.ClientLineId = EDI.ClientLineId;
            //Customers.Remove(Customer);
            Customers.Clear();
            //Customers
            Customer = new CustomerDisplayModel();
            Customer.CustomerName = EDI.CustomerName;
            //Customers.Remove(Customer);
            //var cusname = EDI.CustomerName;
            //        Customers = new ObservableCollection<CustomerDisplayModel>()
            //{
            //  new CustomerDisplayModel(){ CustomerName=EDI.CustomerName}

            //};
            //    Customers = new ObservableCollection<CustomerDisplayModel>() {
            //   Customer
            //};
            //Customers.Add(Customers[0].CustomerName);
            //Customer = Customers.FirstOrDefault();
            Customers.Add(Customer);

            //Customers.cont

            //Customer.se
            //Customers.Select(;
            //Customers = new ObservableCollection<CustomerDisplayModel>(objDSMModelData.GetAllCustomers());
        }

        public void EDIDelete()
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Would you like to delete the customer", "EDI", MessageBoxButton.YesNo);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        //objDSMModelData.DeleteEDI(SelectedEDI.EdiId);

                        string del_qry = "DELETE EDIConfiguration WHERE EdiId=" + SelectedEDI.EdiId + "";
                        bool del = Utilities.executeNonQuery(del_qry);

                        ListEDI = new ObservableCollection<EdiDisplayModel>(objDSMModelData.GetAllEdi());
                        break;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.ToString());
            }
        }

        public void EDISave(object sender)
        {
            //if (sender == null) return;
            //var passwordBox = sender as PasswordBox;
            //EDI.FromPwd = passwordBox.Password;
            try
            {
                if (!string.IsNullOrEmpty(EDI.Host) && !string.IsNullOrEmpty(EDI.Port) && !string.IsNullOrEmpty(EDI.FromEmail) && !string.IsNullOrEmpty(EDI.FromPwd))
                {
                    if (Regex.IsMatch(EDI.FromEmail, @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"))
                    {
                        if (ListEmail.Count != 0)
                        {
                            EDI.ClientLineId = Customer.ClientLineId;
                            EDI.CustomerId = Customer.CustomerId;
                            EDI.Email = "";

                            for (int i = 0; i < ListEmail.Count; i++)
                            {
                                EDI.Email += ListEmail[i].Mail.ToString() + ",";
                            }

                            if (EDI.Email != "")
                            {
                                EDI.Email = EDI.Email.Remove(EDI.Email.Length - 1, 1);
                            }

                            objDSMModelData.SaveEDI(EDI);
                            ListEDI = new ObservableCollection<EdiDisplayModel>(objDSMModelData.GetAllEdi());
                            ListEmail = new ObservableCollection<EmailDisplayModel>();
                            SelectedEDI = new EdiDisplayModel();
                            MessageBox.Show("Saved Successfully");
                            EDIClear();


                        }
                        else
                        {
                            MessageBox.Show("Enter To Mail ID");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid Email");
                    }
                }
                else
                {
                    MessageBox.Show("Enter all the values");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.ToString());
            }
        }
        public void EDIClear()
        {
            EDI.Host = "";
            EDI.Port = "";
            EDI.FromEmail = "";
            EDI.FromPwd = "";
            EDI.VarFromEmail = "";
        }
    }
}
