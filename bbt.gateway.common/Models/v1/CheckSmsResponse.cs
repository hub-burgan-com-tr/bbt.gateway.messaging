using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bbt.gateway.common.Models
{
    public abstract class CheckSmsResponse
    {
        public SmsTrackingStatus SmsTrackingStatus { get; set; }

    }
}

