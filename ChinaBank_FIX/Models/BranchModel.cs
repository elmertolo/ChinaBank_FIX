using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChinaBank_FIX.Models
{
    class BranchModel
    {
        public string BRSTN { get; set; }
        
        public string BranchName { get; set; }
        public Int64? LastNo_PA { get; set; }
        public Int64? LastNo_CA { get; set; }

    }
}
