using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbt.gateway.common.Models.v2
{
    public partial class InboxParams
    {
        public bool enabled { get; set; }
        public Expire? expire { get; set; }

    }
    public partial class Expire
    {
        public string type { get; set; }
        public int? period { get; set; }
        public string periodType { get; set; }
        public string date { get; set; }

    }
}
