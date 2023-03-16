using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bbt.gateway.common.Models
{
    public class SendMessagePushNotificationRequest : SendPushNotificationRequest
    {
        public string Template { get; set; }
        public string CustomerNo { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
