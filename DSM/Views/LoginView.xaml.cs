using DSMData;
using DSMData.Model;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Reflection;
using System.IO;
using System.Data.SqlClient;

namespace DSM.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        public ICommand LoginCommand { get; set; }

        public DSMModelData objDSMModelData;
        public UserDisplayModel objUser;

        private string errorMsg;
        public string ErrorMsg
        {
            get { return errorMsg; }
            set { errorMsg = value; }
        }
        public LoginView()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Assembly thisAssem = typeof(LoginView).Assembly;
            AssemblyName thisAssemName = thisAssem.GetName();
            DateTime buildDate = System.IO.File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location);
            //string displayableVersion = $" version{version} Updated on {buildDate}";
            Version ver = thisAssemName.Version;
            lblversion.Text = ver.ToString();
            lblbuild.Text = buildDate.ToString();
            SqlDataReader _rdr_space = null;
            string _sqry_space = "";
            decimal Space = 0;
            _sqry_space = "SELECT TOP 1 DB_NAME() AS DbName, name AS FileName, type_desc, size / 128.0 AS CurrentSizeMB, size/ 128.0 - CAST(FILEPROPERTY(name, 'SpaceUsed') AS INT) / 128.0 AS FreeSpaceMB FROM sys.database_files WHERE type IN(0,1); ";

            _rdr_space = Utilities.executeQuery(_sqry_space);
            if (_rdr_space.Read())
            {
                Space = Convert.ToDecimal(_rdr_space["FreeSpaceMB"].ToString());
                if (Space != null)
                {
                    _rdr_space.Close();
                    if (Utilities.cnt == 0)
                    {
                        Utilities.cnt++;

                        string list = "";
                        string[] drive = new string[] { @"C:\", @"D:\", @"E:\" };
                        for (int d = 0; d < 3; d++)
                        {
                            list += drive[d].ToString().Replace(@":\", " Drive : ");
                            string drive_letter = drive[d];
                            DriveInfo di = new DriveInfo(drive_letter);

                            if (di.IsReady)
                            {

                                list += "AvailableFreeSpace : " + Math.Round(di.AvailableFreeSpace / (double)(1024 * 1024 * 1024)) + "GB  ";
                                list += "TotalSize : " + Math.Round(di.TotalSize / (double)(1024 * 1024 * 1024)) + "GB  ";
                                list += "VolumeLabel : " + di.VolumeLabel;
                            }
                            list += "\n";
                        }
                        LogSignedDetails(list);
                        // MessageBox.Show(list, "SOMS Info");
                        string data = "";
                        data = Space.ToString();
                        LogSignedDetails1(data);

                        // MessageBox.Show( "Available Space " + Space + " MB");
                        //this.Close();
                    }
                }
            }

        }
        public void LogSignedDetails1(String Data)
        {
            StringBuilder sb = new StringBuilder();
            string path = AppDomain.CurrentDomain.BaseDirectory + "AvailableDatabaseSpace.txt";
            // flush every 20 seconds as you do it

            sb.AppendLine();
            sb.Append("Log Time: " + DateTime.Now);
            sb.AppendLine();
            sb.Append("--------------------------------------------------");
            sb.AppendLine();

            if (Data.ToString().Trim() != "" && Data.ToString().Trim() != null)
            {
                sb.Append(" Available Database Space : " + Data.ToString().Trim() + " MB ");
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
            string path = AppDomain.CurrentDomain.BaseDirectory + "Memorydetails.txt";
            // flush every 20 seconds as you do it

            sb.AppendLine();
            sb.Append("Log Time: " + DateTime.Now);
            sb.AppendLine();
            sb.Append("--------------------------------------------------");
            sb.AppendLine();

            if (Data.ToString().Trim() != "" && Data.ToString().Trim() != null)
            {
                sb.Append(" Memory Details : " + Data.ToString().Trim() + " ");
            }
            sb.AppendLine();
            sb.Append("--------------------------------------------------");
            sb.AppendLine();

            File.AppendAllText(path, sb.ToString());
            sb.Clear();
        }
        public void LoginAction()
        {

            // If no operation specified, return
            //if (sender == null) return;

            //var passwordBox = sender as PasswordBox;
            //Password = passwordBox.Password;

            //if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
            if (string.IsNullOrEmpty(txtUserName.Text) || string.IsNullOrEmpty(txtPassword.ToString()))
            {
                ErrorMsg = "UserName or Password is required.";
                return;
            }
            else
            {
                try
                {
                    objDSMModelData = new DSMModelData();
                    objUser = new UserDisplayModel();
                    objUser = objDSMModelData.GetUserType(txtUserName.Text, txtPassword.Password.ToString());
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
                        return;
                    }
                }
                catch (Exception ex)
                {
                    ErrorMsg = "Invalid credential." + ex.Message;
                }
            }
        }

        private void btnLogin_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
