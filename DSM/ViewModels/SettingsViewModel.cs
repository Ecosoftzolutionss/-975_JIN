using DSM.Model;
using DSMData;
using DSMData.Model;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace DSM.ViewModels
{
    public class SettingsViewModel: BindableBase
    {

        Printer objPrinter;
        SettingsDisplayModel objSettings;
        DSMModelData objDSMModelData;
        public ICommand BrowseCommand { get; set; }
        public ICommand SettingsSaveCommand { get; set; }

        #region Properties
        private string inputPath;
        public string InputPath
        {
            get { return inputPath; }
            set { SetProperty(ref inputPath, value); }
        }

        private string outputPath;
        public string OutputPath
        {
            get { return outputPath; }
            set { SetProperty(ref outputPath, value); }
        }

        private string invoicePath;
        public string InvoicePath
        {
            get { return invoicePath; }
            set { SetProperty(ref invoicePath, value); }
        }

        private string apiUrl;
        public string APIUrl
        {
            get { return apiUrl; }
            set { SetProperty(ref apiUrl, value); }
        }

        private string btnSettings;
        public string BtnSettings
        {
            get { return btnSettings; }
            set { SetProperty(ref btnSettings, value); }
        }
        
        private ObservableCollection<Printer> _printers;

        public ObservableCollection<Printer> Printers
        {
            get { return _printers; }
            set { _printers = value; }
        }
        private Printer _printer;

        public Printer Printer
        {
            get { return _printer; }
            set { _printer = value; }
        }

        #endregion Properties

        /// <summary>
        /// Initialization 
        /// </summary>
        public SettingsViewModel()
        {
            objPrinter = new Printer();
            objSettings = new SettingsDisplayModel();
            objDSMModelData = new DSMModelData();
            BtnSettings = "Save";

            BrowseCommand = new DelegateCommand<object>(BrowsePath);
            SettingsSaveCommand = new DelegateCommand<object>(SaveUpdateSettings);
            Printers = new ObservableCollection<Printer>();
            AssignSettingsData();
        }

        /// <summary>
        /// Browse path for Input, Output, Invoice
        /// </summary>
        /// <param name="sender"></param>
        public void BrowsePath(object sender)
        {
            string type = sender.ToString();
            using (var folder = new FolderBrowserDialog())
            {
                DialogResult result = folder.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folder.SelectedPath))
                {
                    if (type == "InputPath")
                    {
                        InputPath = folder.SelectedPath;
                    }
                    else if (type == "OutputPath")
                    {
                        OutputPath = folder.SelectedPath;
                    }
                    else
                    {
                        InvoicePath = folder.SelectedPath;
                    }
                }
            }
        }
        
        /// <summary>
        /// Get list of printers installed in system
        /// </summary>
        public void AssignSettingsData()
        {   
            //Printer List
            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                objPrinter = new Printer();
                objPrinter.PrinterName = printer;
                Printers.Add(objPrinter); 
            }

            //Get Settings Data
            string sysName = System.Environment.MachineName;
            objSettings = objDSMModelData.GetSettingsData(sysName);
            if(objSettings.SettingsId!=0)
            {
                Printer = new Printer();
                Printer.PrinterName = objSettings.PrinterName;
                InputPath = objSettings.InputPath;
                OutputPath = objSettings.OutputPath;
                InvoicePath = objSettings.InvoicePath;
                Printers.Add(Printer);
                //APIUrl = objSettings.ApiUrl;
                BtnSettings = "Update";
            }
        }

        /// <summary>
        /// Save/Update settings
        /// </summary>
        /// <param name="sender"></param>
        public void SaveUpdateSettings(object sender)
        {
            try
            {
                if (Printer.PrinterName!="" && !string.IsNullOrEmpty(InputPath) && !string.IsNullOrEmpty(OutputPath) && !string.IsNullOrEmpty(InvoicePath))
                {
                    objSettings = new SettingsDisplayModel();
                    objSettings.SystemName = System.Environment.MachineName;
                    objSettings.PrinterName = Printer.PrinterName;
                    objSettings.InputPath = InputPath + "\\";
                    objSettings.OutputPath = OutputPath + "\\";
                    objSettings.InvoicePath = InvoicePath + "\\";
                    //objSettings.ApiUrl = APIUrl;
                    objDSMModelData.SaveSettings(objSettings);

                    MessageBox.Show("Settings updated successfully");
                  
                }
                else
                {
                    MessageBox.Show("All fields are required");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
