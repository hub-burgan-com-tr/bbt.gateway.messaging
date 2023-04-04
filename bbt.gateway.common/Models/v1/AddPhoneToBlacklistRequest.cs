using bbt.gateway.common.Models.v2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bbt.gateway.common.Models
{
    public class AddPhoneToBlacklistRequest
    {
        public PhoneString Phone { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int Days { get; set; }
        public string Reason { get; set; }
        public string Source { get; set; }
        public Process Process { get; set; }
    }
}

