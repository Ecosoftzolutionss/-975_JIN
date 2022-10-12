using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DSMData.Model
{
    public class InvoiceReportModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName()] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        private string invNumber;
        public string InvNumber
        {
            get { return invNumber; }
            set { invNumber = value; NotifyPropertyChanged(); }
        }

        private DateTime dateTime;
        public DateTime InvDate
        {
            get { return dateTime; }
            set { dateTime = value; NotifyPropertyChanged(); }
        }


        private string apiStatus;
        public string ApiStatus
        {
            get { return apiStatus; }
            set { apiStatus = value; NotifyPropertyChanged(); }
        }

        private string apiMsg;
        public string ApiMsg
        {
            get { return apiMsg; }
            set { apiMsg = value; NotifyPropertyChanged(); }
        }
    }
}
