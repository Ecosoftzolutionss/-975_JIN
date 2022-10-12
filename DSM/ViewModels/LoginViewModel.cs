using DSMData;
using DSMData.Model;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DSM.ViewModels
{
    public class LoginViewModel : BindableBase, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChangeds;

        private void NotifyPropertyChanged([CallerMemberName()] string propertyName = null)
        {
            PropertyChangeds?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Properties

        private string userName;
        public string UserName
        {
            get { return userName; }
            set { SetProperty(ref userName, value); }
        }

        private string password;
        public string Password
        {
            get { return password; }
            set { SetProperty(ref password, value); NotifyPropertyChanged("Password"); }
        }

        private string _txtPassword;
        public string txtPassword
        {
            get { return _txtPassword; }
            set { SetProperty(ref _txtPassword, value); NotifyPropertyChanged("txtPassword"); }
        }
        

        private string errorMsg;
        public string ErrorMsg
        {
            get { return errorMsg; }
            set { SetProperty(ref errorMsg, value); }
        }

        private string enableLogin;
        public string EnableLogin
        {
            get { return enableLogin; }
            set { SetProperty(ref enableLogin, value); }
        }

        private DateTime _expiryDate;
        public DateTime ExpiryDate
        {
            get { return _expiryDate; }
            set { SetProperty(ref _expiryDate, value); }
        }

        public ICommand LoginCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public ICommand CloseCommand { get; set; }
        public DSMModelData objDSMModelData;
        public UserDisplayModel objUser;

        #endregion

        public LoginViewModel()
        {

            Process CurrentProcess = Process.GetCurrentProcess();

            foreach (System.Diagnostics.Process p in System.Diagnostics.Process.GetProcesses())
            {
                if (p.Id != CurrentProcess.Id)
                {
                    if (p.ProcessName == "DSM")
                    {
                        MessageBox.Show("Application already running..!");
                        App.Current.MainWindow.Close();
                    }
                }
            }
            string today = DateTime.Now.ToString("");
            //if (int.Parse(DateTime.Now.ToString("dd")) >= 03 && int.Parse(DateTime.Now.ToString("MM")) >= 09 && int.Parse(DateTime.Now.ToString("yyyy")) >= 2022)
            //{
            //    MessageBox.Show("Application Expired", "ALERT");
            //    App.Current.MainWindow.Close();
            //}
            LoginCommand = new DelegateCommand<object>(LoginAction);
            //CancelCommand = new DelegateCommand(CancelAction);
            CancelCommand = new DelegateCommand<object>(CancelAction);
            CloseCommand = new DelegateCommand(CloseAction);
            objDSMModelData = new DSMModelData();
            objUser = new UserDisplayModel();

            //ErrorMsg = string.Empty;
            //string iString = "12/08/2019";
            //DateTime startDate = DateTime.ParseExact(iString, "dd/MM/yyyy", null);
            //ExpiryDate = startDate.Date.AddDays(7);

            //if (ExpiryDate.Date <= DateTime.Today)
            //{
            //    EnableLogin = "False";
            //    ErrorMsg = "Trial Version Expired.";
            //    MessageBox.Show("Trial Version Expired.");
            //    App.Current.MainWindow.Close();
            //}
            //else
            //{
            //    EnableLogin = "True";
            //    ErrorMsg = "";
            //}
        }
        public void LoginAction(object sender)
        {

            // If no operation specified, return
            if (sender == null) return;

            var passwordBox = sender as PasswordBox;
            Password = passwordBox.Password;

            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
            {
                ErrorMsg = "UserName or Password is required.";
                UserName = "";
                passwordBox.Clear();
                return;
            }
            else
            {
                try
                {
                    objUser = objDSMModelData.GetUserType(UserName, Password);
                    if (!string.IsNullOrEmpty(objUser.UserType))
                    {
                        //Assign value to global variables
                        Global.UserId = objUser.UserId;
                        Global.UserName = objUser.UserName;
                        Global.UserType = objUser.UserType;
                        //Global.UserIsActive = objUser.UserIsActive;

                        var loginWondow = App.Current.MainWindow;
                        Bootstrapper bootstrapper = new Bootstrapper();
                        bootstrapper.Run();
                        loginWondow.Close();
                    }
                    else
                    {
                        ErrorMsg = "Invalid UserName or Password.";
                        passwordBox.Clear();
                        passwordBox.Focus();
                        return;
                    }
                }
                catch (Exception ex)
                {
                    ErrorMsg = "Invalid credential." + ex.Message;
                }
            }
        }

      
            public void CancelAction(object sender)
        {
            //App.Current.MainWindow.Close();
            //DS = new DSMasterDisplayModel();
            UserName = "";
            
            if (sender == null) return;

            var passwordBox = sender as PasswordBox;
            passwordBox.Clear();





            //Password = "";
            //txtPassword = "";
            //ErrorDSpin = "";
            //ErrorDept = "";
            //ErrorDesgn = "";
        }

        public void CloseAction()
        {
            App.Current.MainWindow.Close();
        }
    }
}
