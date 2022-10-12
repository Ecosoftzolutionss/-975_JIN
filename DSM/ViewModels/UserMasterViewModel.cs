using DSMData;
using DSMData.Model;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DSM.ViewModels
{
    public class UserMasterViewModel : BindableBase
    {
        public ICommand CreateUserCommand { get; set; }
        public DSMModelData objDSMModelData;

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
            set { SetProperty(ref password, value); }
        }

        private string userType;
        public string UserType
        {
            get { return userType; }
            set { SetProperty(ref userType, value); }
        }

        #endregion

        public UserMasterViewModel()
        {
            CreateUserCommand = new DelegateCommand(CreateUser);
            objDSMModelData = new DSMModelData();
        }

        public void CreateUser()
        {
            UserDisplayModel objUser = new UserDisplayModel();
            objUser.UserName = UserName;
            objUser.Password = Password;
            objUser.UserType = "admin";
            objDSMModelData.CreateUser(objUser);
        }

    }
}