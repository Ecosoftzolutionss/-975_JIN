using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSMData.Model
{
    public class QuarantineDisplayModel
    {
        public int QId { get; set; }
        public string InvNumber { get; set; }
        public bool IsActive { get; set; }
        public string ScanData { get; set; }
    }
}
