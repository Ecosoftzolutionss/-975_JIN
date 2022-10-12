using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSMData.Model
{
    public class DigitalSignDetailDisplayModel
    {
        public int DSDetailsId { get; set; }
        public int ClientLineId { get; set; }
        public string ClientId { get; set; }
        public string KeyName { get; set; }
        public string KeyPinNo { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public bool IsActive { get; set; }
    }
}
