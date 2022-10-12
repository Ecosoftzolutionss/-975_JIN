using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DSMData.Model
{
    public class InvoiceDisplayModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName()] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private string shopCode;
        public string ShopCode
        {
            get { return shopCode; }
            set { shopCode = value; NotifyPropertyChanged(); }
        }

        private string poNumber;
        public string PONumber
        {
            get { return poNumber; }
            set { poNumber = value; NotifyPropertyChanged(); }
        }

        private string partNumber;
        public string PartNumber
        {
            get { return partNumber; }
            set { partNumber = value; NotifyPropertyChanged(); }
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

        private string invQuantity;
        public string InvQuantity
        {
            get { return invQuantity; }
            set { invQuantity = value; NotifyPropertyChanged(); }
        }

        private string invValue;
        public string InvValue
        {
            get { return invValue; }
            set { invValue = value; NotifyPropertyChanged(); }
        }
                
        private string tarrifNumber;
        public string TarrifNumber
        {
            get { return tarrifNumber; }
            set { tarrifNumber = value; NotifyPropertyChanged(); }
        }

        private string bedAmount;
        public string BedAmount
        {
            get { return bedAmount; }
            set { bedAmount = value; NotifyPropertyChanged(); }
        }

        private string sgst;
        public string SGST
        {
            get { return sgst; }
            set { sgst = value; NotifyPropertyChanged(); }
        }

        private string igst;
        public string IGST
        {
            get { return igst; }
            set { igst = value; NotifyPropertyChanged(); }
        }

        private string vatAmount;
        public string VatAmount
        {
            get { return vatAmount; }
            set { vatAmount = value; NotifyPropertyChanged(); }
        }
       
        private string unitPrice;
        public string UnitPrice
        {
            get { return unitPrice; }
            set { unitPrice = value; NotifyPropertyChanged(); }
        }

        private string materialCost;
        public string MaterialCost
        {
            get { return materialCost; }
            set { materialCost = value; NotifyPropertyChanged(); }
        }

        private string cgst;
        public string CGST
        {
            get { return cgst; }
            set { cgst = value; NotifyPropertyChanged(); }
        }

        private string consigneePartCost;
        public string ConsigneePartCost
        {
            get { return consigneePartCost; }
            set { consigneePartCost = value; NotifyPropertyChanged(); }
        }

        private string exciseDutyCost;
        public string ExciseDutyCost
        {
            get { return exciseDutyCost; }
            set { exciseDutyCost = value; NotifyPropertyChanged(); }
        }

        private string assessableValue;
        public string AssessableValue
        {
            get { return assessableValue; }
            set { assessableValue = value; NotifyPropertyChanged(); }
        }

        private string cstAmount;
        public string CSTAmount
        {
            get { return cstAmount; }
            set { cstAmount = value; NotifyPropertyChanged(); }
        }

        private string toolCost;
        public string ToolCost
        {
            get { return toolCost; }
            set { toolCost = value; NotifyPropertyChanged(); }
        }

        private string consigneeMatlCost;
        public string ConsigneeMatlCost
        {
            get { return consigneeMatlCost; }
            set { consigneeMatlCost = value; NotifyPropertyChanged(); }
        }
               
        private string gstn;
        public string GSTN
        {
            get { return gstn; }
            set { gstn = value; NotifyPropertyChanged(); }
        }

        private string batchNumber;
        public string BatchNumber
        {
            get { return batchNumber; }
            set { batchNumber = value; NotifyPropertyChanged(); }
        }

        private string vehicleNumber;
        public string VehicleNumber
        {
            get { return vehicleNumber; }
            set { vehicleNumber = value; NotifyPropertyChanged(); }
        }
        
        private string vendorCode;
        public string VendorCode
        {
            get { return vendorCode; }
            set { vendorCode = value; NotifyPropertyChanged(); }
        }

        private string customerName;
        public string CustomerName
        {
            get { return customerName; }
            set { customerName = value; NotifyPropertyChanged(); }
        }

        private string truckNumber;
        public string TruckNumber
        {
            get { return truckNumber; }
            set { truckNumber = value; NotifyPropertyChanged(); }
        }

        public bool isEmailSend; 
        public bool IsEmailSend
        {
            get { return isEmailSend; }
            set { isEmailSend = value; NotifyPropertyChanged(); }
        }

        private int createdBy;
        public int CreatedBy
        {
            get { return createdBy; }
            set { createdBy = value; }
        }

        private string dsStatus;
        public string DSStatus
        {
            get { return dsStatus; }
            set { dsStatus = value; NotifyPropertyChanged(); }
        }

        private string printStatus;
        public string PrintStatus
        {
            get { return printStatus; }
            set { printStatus = value; NotifyPropertyChanged(); }
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

        private DateTime createdDate;
        public DateTime CreatedDate
        {
            get { return createdDate; }
            set { createdDate = value; NotifyPropertyChanged(); }
        }

        private int finyear;
        public int Finyear
        {
            get { return finyear; }
            set { finyear = value; NotifyPropertyChanged(); }
        }

        private string systemName;
        public string SystemName
        {
            get { return systemName; }
            set { systemName = value; NotifyPropertyChanged(); }
        }
        private string pdfGeneration;
        public string PdfGeneration
        {
            get { return pdfGeneration; }
            set { pdfGeneration = value; NotifyPropertyChanged(); }
        }
        private byte[] originalPDF;
        public byte[] OriginalPDF
        {
            get { return originalPDF; }
            set { originalPDF = value; NotifyPropertyChanged(); }
        }
        private string lot_Code;
        public string Lot_Code
        {
            get { return lot_Code; }
            set { lot_Code = value; NotifyPropertyChanged(); }
        }
        private string tcs;
        public string TCS
        {
            get { return tcs; }
            set { tcs = value; NotifyPropertyChanged(); }
        }
        private string e_InvNo;
        public string E_InvNo
        {
            get { return e_InvNo; }
            set { e_InvNo = value; NotifyPropertyChanged(); }
        }
        private DateTime manfac_Date;
        public DateTime Manfac_Date
        {
            get { return manfac_Date; }
            set { manfac_Date = value; NotifyPropertyChanged(); }
        }
        private string ewaybill_no;
        public string Ewaybill_no
        {
            get { return ewaybill_no; }
            set { ewaybill_no = value; NotifyPropertyChanged(); }
        }
        private string eXTRA_NUM_1;
        public string EXTRA_NUM_1
        {
            get { return eXTRA_NUM_1; }
            set { eXTRA_NUM_1 = value; NotifyPropertyChanged(); }
        }
        private string eXTRA_NUM_2;
        public string EXTRA_NUM_2
        {
            get { return eXTRA_NUM_2; }
            set { eXTRA_NUM_2 = value; NotifyPropertyChanged(); }
        }
        private string eXTRA_NUM_3;
        public string EXTRA_NUM_3
        {
            get { return eXTRA_NUM_3; }
            set { eXTRA_NUM_3 = value; NotifyPropertyChanged(); }
        }
        private string eXTRA_NUM_4;
        public string EXTRA_NUM_4
        {
            get { return eXTRA_NUM_4; }
            set { eXTRA_NUM_4 = value; NotifyPropertyChanged(); }
        }
        private string eXTRA_NUM_5;
        public string EXTRA_NUM_5
        {
            get { return eXTRA_NUM_5; }
            set { eXTRA_NUM_5 = value; NotifyPropertyChanged(); }
        }
        private string eXTRA_NUM_6;
        public string EXTRA_NUM_6
        {
            get { return eXTRA_NUM_6; }
            set { eXTRA_NUM_6 = value; NotifyPropertyChanged(); }
        }
        private string eXTRA_NUM_7;
        public string EXTRA_NUM_7
        {
            get { return eXTRA_NUM_7; }
            set { eXTRA_NUM_7 = value; NotifyPropertyChanged(); }
        }

        private string eXTRA_CHAR_1;
        public string EXTRA_CHAR_1
        {
            get { return eXTRA_CHAR_1; }
            set { eXTRA_CHAR_1 = value; NotifyPropertyChanged(); }
        }
        private string eXTRA_CHAR_2;
        public string EXTRA_CHAR_2
        {
            get { return eXTRA_CHAR_2; }
            set { eXTRA_CHAR_2 = value; NotifyPropertyChanged(); }
        }
        private string eXTRA_CHAR_3;
        public string EXTRA_CHAR_3
        {
            get { return eXTRA_CHAR_3; }
            set { eXTRA_CHAR_3 = value; NotifyPropertyChanged(); }
        }

        private string eXTRA_CHAR_4;
        public string EXTRA_CHAR_4
        {
            get { return eXTRA_CHAR_4; }
            set { eXTRA_CHAR_4 = value; NotifyPropertyChanged(); }
        }
        private string eXTRA_CHAR_5;
        public string EXTRA_CHAR_5
        {
            get { return eXTRA_CHAR_5; }
            set { eXTRA_CHAR_5 = value; NotifyPropertyChanged(); }
        }

        private DateTime date1;
        public DateTime Date1
        {
            get { return date1; }
            set { date1 = value; NotifyPropertyChanged(); }
        }

        private DateTime date2;
        public DateTime Date2
        {
            get { return date2; }
            set { date2 = value; NotifyPropertyChanged(); }
        }

        private DateTime date3;
        public DateTime Date3
        {
            get { return date3; }
            set { date3 = value; NotifyPropertyChanged(); }
        }

        private DateTime date4;
        public DateTime Date4
        {
            get { return date4; }
            set { date4 = value; NotifyPropertyChanged(); }
        }

        private DateTime date5;
        public DateTime Date5
        {
            get { return date5; }
            set { date5 = value; NotifyPropertyChanged(); }
        }
    }
}
