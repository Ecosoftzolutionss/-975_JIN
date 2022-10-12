using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSMData.Model
{
    public class EdiDisplayModel
    {
        private int ediId;
        public int EdiId
        {
            get { return ediId; }
            set { ediId = value; }
        }

        private int clientLineId;
        public int ClientLineId
        {
            get { return clientLineId; }
            set { clientLineId = value; }
        }

        private string customerId;
        public string CustomerId
        {
            get { return customerId; }
            set { customerId = value; }
        }

        private string customerName;
        public string CustomerName
        {
            get { return customerName; }
            set { customerName = value; }
        }

        private bool isEmail;
        public bool IsEmail
        {
            get { return isEmail; }
            set { isEmail = value; }
        }

        private string email;
        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        private string fromPwd;
        public string FromPwd
        {
            get {
                return (fromPwd);
            }
           
            set {
                fromPwd = value;
                //fromPwd = "*";
            }
        }

        private bool isEdi;
        public bool IsEdi
        {
            get { return isEdi; }
            set { isEdi = value; }
        }

        private string ediLink;
        public string EdiLink
        {
            get { return ediLink; }
            set { ediLink = value; }
        }

        private string port;
        public string Port
        {
            get { return port; }
            set { port = value; }
        }

        private string host;
        public string Host
        {
            get { return host; }
            set { host = value; }
        }

        private string fromEmail;
        public string FromEmail
        {
            get { return fromEmail; }
            set { fromEmail = value; }
        }

        private string varFromEmail;
        public string VarFromEmail
        {
            get { return varFromEmail; }
            set { varFromEmail = value; }
        }

        private bool isActive;
        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }
    }
}
