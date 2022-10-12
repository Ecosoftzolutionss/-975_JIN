using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSMData.Model
{
    public class SettingsDisplayModel
    {

        private int settingsId;
        public int SettingsId
        {
            get { return settingsId; }
            set { settingsId = value; }
        }

        private string inputPath;
        public string InputPath
        {
            get { return inputPath; }
            set { inputPath = value; }
        }

        private string outputPath;
        public string OutputPath
        {
            get { return outputPath; }
            set { outputPath = value; }
        }

        private string invoicePath;
        public string InvoicePath
        {
            get { return invoicePath; }
            set { invoicePath = value; }
        }

        private string printerName;
        public string PrinterName
        {
            get { return printerName; }
            set { printerName = value; }
        }

        private string systemName;
        public string SystemName
        {
            get { return systemName; }
            set { systemName = value; }
        }

        private string apiUrl;
        public string ApiUrl
        {
            get { return apiUrl; }
            set { apiUrl = value; }
        }
    }
}
