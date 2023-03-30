using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bbt.gateway.common.Models
{
    public class ResolveBlacklistEntryRequest
    {
        public DateTime ResolvedAt { get; set; }
        public Process ResolvedBy { get; set; }
    }
}

