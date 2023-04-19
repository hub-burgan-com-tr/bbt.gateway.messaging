using bbt.gateway.common.Models.v2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bbt.gateway.common.Models
{
    public class ResolveBlacklistEntryFromPhoneRequest
    {
        public PhoneString Phone { get; set; }
        public string Reason { get; set; }
        public Process ResolvedBy { get; set; }
    }
}

