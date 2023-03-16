using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bbt.gateway.common.Models
{
    public class CheckSmsRequest
    {
        public OperatorType Operator { get; set; }
        public Guid OtpRequestLogId { get; set; }
        public string StatusQueryId { get; set; }

    }
}

