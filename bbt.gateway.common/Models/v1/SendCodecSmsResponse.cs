using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bbt.gateway.common.Models
{
    public class SendCodecSmsResponse
    {
        public Guid TxnId { get; set; }
        public CodecResponseCodes Status { get; set; }

    }
}

