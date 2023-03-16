using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bbt.gateway.common.Models.v2
{
    public class CheckFastSmsRequest
    {
        public OperatorType Operator { get; set; }
        public Guid SmsRequestLogId { get; set; }
        public string StatusQueryId { get; set; }

    }
}

