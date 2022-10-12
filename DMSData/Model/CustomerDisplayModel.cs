using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace DSMData.Model
{
    public class CustomerDisplayModel
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName()] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private bool _IsDatalbl;
        public bool IsDatalbl
        {
            get { return _IsDatalbl; }
            set { _IsDatalbl = value; }
        }
        private int clientLineId;
        public int ClientLineId
        {
            get { return clientLineId; }
            set { clientLineId = value; NotifyPropertyChanged(); }
        }

        private string customerId;
        public string CustomerId
        {
            get { return customerId; }
            set { customerId = value; NotifyPropertyChanged(); }
        }

        private string customerName;
        public string CustomerName
        {
            get { return customerName; }
            set { customerName = value; NotifyPropertyChanged(); }
        }

        private bool isActive;
        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; NotifyPropertyChanged(); }
        }

        private bool isDigitalSign;
        public bool IsDigitalSign
        {
            get { return isDigitalSign; }
            set { isDigitalSign = value; NotifyPropertyChanged(); }
        }

        private bool isTemplateCheck;
        public bool IsTemplateCheck
        {
            get { return isTemplateCheck; }
            set { isTemplateCheck = value; NotifyPropertyChanged(); }
        }

        private bool isAPIpost;
        public bool IsAPIpost
        {
            get { return isAPIpost; }
            set { isAPIpost = value; NotifyPropertyChanged(); }
        }

        private string apiUrl;
        public string APIUrl
        {
            get { return apiUrl; }
            set { apiUrl = value; NotifyPropertyChanged(); }
        }

        private bool apitype;
        public bool APItype
        {
            get { return apitype; }
            set { apitype = value; NotifyPropertyChanged(); }
        }

        private bool _APIAuto;
        public bool APIAuto
        {
            get { return _APIAuto; }
            set { _APIAuto = value; NotifyPropertyChanged(); }
        }

        private bool _APIManual;
        public bool APIManual
        {
            get { return _APIManual; }
            set { _APIManual = value; NotifyPropertyChanged(); }
        }

        private bool pdftype;
        public bool PDFtype
        {
            get { return pdftype; }
            set { pdftype = value; NotifyPropertyChanged(); }
        }

        private bool _IsOriginalCopy;
        public bool IsOriginalCopy
        {
            get { return _IsOriginalCopy; }
            set { _IsOriginalCopy = value; NotifyPropertyChanged(); }
        }

        private bool _IsAllCopy;
        public bool IsAllCopy
        {
            get { return _IsAllCopy; }
            set { _IsAllCopy = value; NotifyPropertyChanged(); }
        }

        private bool _IsSingle;
        public bool IsSingle
        {
            get { return _IsSingle; }
            set { _IsSingle = value; NotifyPropertyChanged(); }
        }

        private bool _IsMultiple;
        public bool IsMultiple
        {
            get { return _IsMultiple; }
            set { _IsMultiple = value; NotifyPropertyChanged(); }
        }

        private bool printType;
        public bool PrintType
        {
            get { return printType; }
            set { printType = value; NotifyPropertyChanged(); }
        }

        private bool _IsOriginal;
        public bool IsOriginal
        {
            get { return _IsOriginal; }
            set { _IsOriginal = value; NotifyPropertyChanged(); }
        }

        private bool _IsDuplicate;
        public bool IsDuplicate
        {
            get { return _IsDuplicate; }
            set { _IsDuplicate = value; NotifyPropertyChanged(); }
        }

        private bool _IsTrplicate;
        public bool IsTrplicate
        {
            get { return _IsTrplicate; }
            set { _IsTrplicate = value; NotifyPropertyChanged(); }
        }

        private bool _IsExtra;
        public bool IsExtra
        {
            get { return _IsExtra; }
            set { _IsExtra = value; NotifyPropertyChanged(); }
        }

        private string printCopies;
        public string PrintCopies
        {
            get { return printCopies; }
            set { printCopies = value; NotifyPropertyChanged(); }
        }

        private string vendorCode;
        public string VendorCode
        {
            get { return vendorCode; }
            set { vendorCode = value; NotifyPropertyChanged(); }
        }

        private string invnocoord;
        public string Invnocoord
        {
            get { return invnocoord; }
            set { invnocoord = value; NotifyPropertyChanged(); }
        }

        private string invdatecoord;
        public string Invdatecoord
        {
            get { return invdatecoord; }
            set { invdatecoord = value; NotifyPropertyChanged(); }
        }

        private string vendorcoord;
        public string Vendorcoord
        {
            get { return vendorcoord; }
            set { vendorcoord = value; NotifyPropertyChanged(); }
        }
        private string shopcdcoord;
        public string Shopcdcoord
        {
            get { return shopcdcoord; }
            set { shopcdcoord = value; NotifyPropertyChanged(); }
        }


        private string trucknocoord;
        public string Trucknocoord
        {
            get { return trucknocoord; }
            set { trucknocoord = value; NotifyPropertyChanged(); }
        }

        private string clientnamecoord;
        public string Clientnamecoord
        {
            get { return clientnamecoord; }
            set { clientnamecoord = value; NotifyPropertyChanged(); }
        }

        private string dscoord;
        public string Dscoord
        {
            get { return dscoord; }
            set { dscoord = value; NotifyPropertyChanged(); }
        }
        private string tempname;
        public string Tempname
        {
            get { return tempname; }
            set { tempname = value; NotifyPropertyChanged(); }
        }
    }
}
