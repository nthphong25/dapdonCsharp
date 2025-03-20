using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dapdon.Models
{
    public class MoSummaryModel
    {
        public string MoNo { get; set; }
        public int EpcCount { get; set; }
        public string shoestyle_codefactory { get; set; }
        public string mat_code { get; set; }
        public List<string> EpcList { get; set; } = new List<string>();
    }


}
