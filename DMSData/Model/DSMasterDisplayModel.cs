using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSMData.Model
{
    public class DSMasterDisplayModel
    {
        public int DSMasterId { get; set; }
        public string KeyName { get; set; }
        public string KeyPinNo { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public string SignName { get; set; }
        public string IssuerName { get; set; }
        public string SerialNo { get; set; }
        public string ThumbPrint { get; set; }
        public string CertificateName { get; set; }
        public string STATUS { get; set; }
        public string ContainerName { get; set; }
        public string ProviderName { get; set; }
        public DateTime ValidityBefore { get; set; }
        public DateTime ValidityAfter { get; set; }
    }
}
