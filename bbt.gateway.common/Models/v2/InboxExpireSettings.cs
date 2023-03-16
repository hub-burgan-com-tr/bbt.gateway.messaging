using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbt.gateway.common.Models.v2
{
    public class InboxExpireSettings
    {
        public string periodType { get; set; }
        public int? period { get; set; }
    }
}
