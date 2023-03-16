using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bbt.gateway.common.Models
{
    public class SendTemplatedPushNotificationRequest : SendPushNotificationRequest
    {
        public string TemplateParams { get; set; }
        public string Template { get; set; }
        public string CustomerNo { get; set; }
        public string ContactId { get; set; }
        public string CustomParameters { get; set; }
    }
}
