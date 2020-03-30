using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChinaBank_FIX.Models
{
    class ErrorModel
    {
        public string BRSTN { get; set; }
        public string BranchName { get; set; }
        public string CheckType { get; set; }
        public Int64? HistorySerial { get; set; }
        public Int64? CurrentSerial { get; set; }
    }
}
