using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSMData.Model
{
    public class UserDisplayModel
    {
        private int userId;
        public int UserId
        {
            get { return userId; }
            set { userId = value; }
        }

        private string userName;
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        private string password;
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        private string userType;
        public string UserType
        {
            get { return userType; }
            set { userType = value; }
        }

        private string loginStatus;
        public string LoginStatus
        {
            get { return loginStatus; }
            set { loginStatus = value; }
        }

        private int userIsActive;
        public int UserIsActive
        {
            get { return userIsActive; }
            set { userIsActive = value; }
        }

        
    }
}
