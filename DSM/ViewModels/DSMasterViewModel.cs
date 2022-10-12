using DSM.Model;
using DSMData;
using iTextSharp.text.log;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DSMData.Model;
using System.Collections.ObjectModel;
using System.Windows;

namespace DSM.ViewModels
{
    public class DSMasterViewModel : BindableBase
    {
        Printer objPrinter;
        DSMModelData objDSMModelData;
        DSMasterDisplayModel objDSdata;
        public ICommand BrowseCommand { get; set; }
        public ICommand AddKeyCommand { get; set; }
        public ICommand EditDSKeyCommand { get; set; }
        public ICommand RemoveDSKeyCommand { get; set; }
        public ICommand ResetDSKeyCommand { get; set; }

        #region Properties

        private string errorNickName;
        public string ErrorNickName
        {
            get { return errorNickName; }
            set { SetProperty(ref errorNickName, value); }
        }

        private string errorKey;
        public string ErrorKey
        {
            get { return errorKey; }
            set { SetProperty(ref errorKey, value); }
        }

        private string errorDSpin;
        public string ErrorDSpin
        {
            get { return errorDSpin; }
            set { SetProperty(ref errorDSpin, value); }
        }

        public string errorDept;
        public string ErrorDept
        {
            get { return errorDept; }
            set { SetProperty(ref errorDept, value); }
        }

        private string errorDesgn;
        public string ErrorDesgn
        {
            get { return errorDesgn; }
            set { SetProperty(ref errorDesgn, value); }
        }

        private ObservableCollection<DSMasterDisplayModel> lstDS;
        public ObservableCollection<DSMasterDisplayModel> ListDS
        {
            get { return lstDS; }
            set { SetProperty(ref lstDS, value); }
        }

        private DSMasterDisplayModel _SelectedDS;
        public DSMasterDisplayModel SelectedDS
        {
            get { return _SelectedDS; }
            set { SetProperty(ref _SelectedDS, value); }
        }

        private DSMasterDisplayModel _DS;
        public DSMasterDisplayModel DS
        {
            get { return _DS; }
            set { SetProperty(ref _DS, value); } 
        }

        private string _EnableKeySelection;
        public string EnableKeySelection
        {
            get { return _EnableKeySelection; }
            set { SetProperty(ref _EnableKeySelection, value); }
        }
        
        X509Certificate2 pk = null;
        IList<X509Certificate> chain = new List<X509Certificate>();
        string signName;
        string issuername;
        string notaftervalidity;
        string notbeforevalidity;
        string privatekey;
        string publickey;
        string serialno;
        string subjectname;
        string thumbprint;
        DateTime aftval;
        DateTime befval;
        string containerName;
        string ProviderName;

        #endregion Properties

        /// <summary>
        /// Initialization 
        /// </summary>
        public DSMasterViewModel()
        {
            EnableKeySelection = "True";
            objPrinter = new Printer();
            objDSMModelData = new DSMModelData();
            objDSdata = new DSMasterDisplayModel();
            DS = new DSMasterDisplayModel();
            ListDS = new ObservableCollection<DSMasterDisplayModel>(objDSMModelData.GetAllDSKeys());
            SelectedDS = new DSMasterDisplayModel();
            BrowseCommand = new DelegateCommand(BrowseKey);
            AddKeyCommand = new DelegateCommand(SaveKey);
            EditDSKeyCommand = new DelegateCommand(EditDSKey);
            RemoveDSKeyCommand = new DelegateCommand(RemoveDSKey);
            ResetDSKeyCommand = new DelegateCommand(ResetDS);
        }

        private void EditDSKey()
        {
            EnableKeySelection = "False";
            DS = SelectedDS;
        }

        private void RemoveDSKey()
        {
            if (SelectedDS != null)
            {
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure, Do you want to Delete it?", "Delete Confirmation", System.Windows.MessageBoxButton.OKCancel);
                if (messageBoxResult == MessageBoxResult.OK)
                {
                    objDSMModelData.DeleteDSKey(SelectedDS.DSMasterId);
                    ListDS = new ObservableCollection<DSMasterDisplayModel>(objDSMModelData.GetAllDSKeys());
                    ResetDS();
                }
            }
        }

        private void ResetDS()
        {
            DS = new DSMasterDisplayModel();
            ErrorNickName = "";
            ErrorKey = "";
            ErrorDSpin = "";
            ErrorDept = "";
            ErrorDesgn = "";
        }

        public void BrowseKey()
        {
            try
            {
                if (DS.SignName.Trim() != "" && DS.SignName.Trim().Length > 0)
                {
                    ErrorNickName = "";
                    signName = DS.SignName;
                    X509Store x509Store = new X509Store("My");
                    LoggerFactory.GetInstance().SetLogger(new SysoLogger());
                    x509Store.Open(OpenFlags.ReadOnly);
                    //
                    X509Certificate2Collection collection = (X509Certificate2Collection)x509Store.Certificates;
                    X509Certificate2Collection fcollection = (X509Certificate2Collection)collection.Find(X509FindType.FindByKeyUsage, X509KeyUsageFlags.DigitalSignature, false);
                    X509Certificate2Collection scollection = X509Certificate2UI.SelectFromCollection(fcollection, "Test Certificate Select", "Select a certificate from the following list to get information on that certificate", X509SelectionFlag.MultiSelection);

                    if (scollection.Count > 0)
                    {

                        pk = scollection[0];
                        //lblCertificte.Text = pk.SubjectName.Name;
                        subjectname = pk.SubjectName.Name;
                        issuername = pk.IssuerName.Name;
                        notaftervalidity = pk.NotAfter.ToString();
                        notbeforevalidity = pk.NotBefore.ToString();
                        privatekey = pk.PrivateKey.ToString();
                        publickey = pk.PublicKey.ToString();
                        serialno = pk.SerialNumber;
                        //subjectname = pk.SubjectName;
                        thumbprint = pk.Thumbprint;

                        aftval = Convert.ToDateTime(notaftervalidity);
                        befval = Convert.ToDateTime(notbeforevalidity.ToString());
                        DateTime currentdatetime = DateTime.Now;

                        X509Chain x509chain = new X509Chain();
                        x509chain.Build(pk);
                        foreach (X509ChainElement x509ChainElement in x509chain.ChainElements)
                        {
                            chain.Add(Org.BouncyCastle.Security.DotNetUtilities.FromX509Certificate(x509ChainElement.Certificate));
                            break;
                        }
                        //
                        x509Store.Close();

                        RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)pk.PrivateKey;

                        CspParameters cspp = new CspParameters();
                        cspp.KeyContainerName = rsa.CspKeyContainerInfo.KeyContainerName;
                        cspp.ProviderName = rsa.CspKeyContainerInfo.ProviderName;
                        // cspp.ProviderName = "Microsoft Smart Card Key Storage Provider";

                        cspp.ProviderType = rsa.CspKeyContainerInfo.ProviderType;

                        cspp.Flags = CspProviderFlags.NoPrompt;

                        RSACryptoServiceProvider rsa2 = new RSACryptoServiceProvider(cspp);

                        rsa.PersistKeyInCsp = true;

                        containerName = cspp.KeyContainerName;
                        ProviderName = cspp.ProviderName;


                        if (subjectname.ToString().Trim() != "" && subjectname.ToString().Length > 0 && subjectname.ToString().Trim().Contains("CN="))
                        {
                            //txtDepartment.Text = notbeforevalidity.ToString().Trim();
                            //txtDesignation.Text = notaftervalidity.ToString().Trim();
                            string[] a = subjectname.ToString().Trim().Split(',');
                            //string[] noofrows = pdfcode.ToString().Trim().Split('~');


                            int count = a.Length;
                            if (count > 0)
                            {
                                for (int i = 0; i < a.Length; i++)
                                {
                                    string getName = "";
                                    int index = a[i].IndexOf("CN=");
                                    if (index >= 0)
                                    {
                                        //Console.WriteLine("'{0} begins at character position {1}", s2, index + 1);
                                        getName = a[i].ToString().Trim().Replace("CN=", "");
                                        DS.KeyName = getName.ToString().Trim();
                                        return;
                                    }
                                    //subjectname.
                                }
                            }



                        }
                    }
                    else
                    {
                        //MessageBox.Show("Please Select the Certificate !!", "SOMS Alert");
                        return;
                    }
                }
                else
                {
                    ErrorNickName = "Enter Nickname to browse key!";
                    return;
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        public void SaveKey()
        {

            bool valid = ValidateDS();

            if (valid)
            {   
                DS.IssuerName = issuername;
                DS.SerialNo = serialno;
                DS.ThumbPrint = thumbprint;
                DS.CertificateName = subjectname;
                DS.ContainerName = containerName;
                DS.ProviderName = ProviderName;
                DS.ValidityBefore = befval;
                DS.ValidityAfter = aftval;
                DS.IsActive = true;
                objDSMModelData.SaveDigitalSignature(DS);
                ResetDS();
                ListDS = new ObservableCollection<DSMasterDisplayModel>(objDSMModelData.GetAllDSKeys());
            }
        }

        public bool ValidateDS()
        {
            bool val = false;
            if (string.IsNullOrEmpty(DS.SignName))
            {
                ErrorNickName = "Enter NickName";
            }
            else
            {
                if (DS.SignName.Trim() == "")
                    ErrorNickName = "Enter NickName";
                else
                    ErrorNickName = "";
            }

            if (string.IsNullOrEmpty(DS.KeyName))
            {
                ErrorKey = "Browse Key";
            }
            else
            {
                if (EnableKeySelection == "True")
                {
                    var checkDB = objDSMModelData.ValidateDSKey(subjectname, serialno);
                    if (checkDB)
                    {
                        ErrorKey = "";
                    }
                    else
                    {
                        ErrorKey = "Key Already Exist";
                    }
                }
                else
                {
                    ErrorKey = "";
                }
            }

            if (string.IsNullOrEmpty(DS.KeyPinNo))
                ErrorDSpin = "Enter Key Pin No";
            else
            {
                if (DS.KeyPinNo.Trim() == "")
                    ErrorDSpin = "Enter Key Pin No";
                else
                    ErrorDSpin = "";
            }

            if (string.IsNullOrEmpty(DS.Department))
                ErrorDept = "Enter Department";
            else
            {
                if (DS.Department.Trim() == "")
                    ErrorDept = "Enter Department";
                else
                    ErrorDept = "";
            }

            if (string.IsNullOrEmpty(DS.Designation))
                ErrorDesgn = "Enter Designation";
            else
            {
                if (DS.Designation.Trim() == "")
                    ErrorDesgn = "Enter Designation";
                else
                    ErrorDesgn = "";
            }
            
            if(ErrorNickName=="" && ErrorKey == "" && ErrorDSpin == "" && ErrorDept == "" && ErrorDesgn == "")
                val = true;
            
            return val;
        }
    }
}
