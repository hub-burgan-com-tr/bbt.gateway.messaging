using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bbt.gateway.common.Models
{
    public class SendPushNotificationResponse
    {
        public Guid TxnId { get; set; }

        public dEngageResponseCodes Status { get; set; }

    }
}

