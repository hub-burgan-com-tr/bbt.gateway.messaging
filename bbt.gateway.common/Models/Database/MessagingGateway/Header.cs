using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bbt.gateway.common.Models
{
    public class Header
    {
        public Guid Id { get; set; }
        public MessageContentType ContentType { get; set; }
        public string BusinessLine { get; set; }
        public int? Branch { get; set; }
        public SenderType SmsSender { get; set; }
        public string SmsPrefix { get; set; }
        public string SmsSuffix { get; set; }
        public string EmailTemplatePrefix { get; set; }
        public string EmailTemplateSuffix { get; set; }
        public string SmsTemplatePrefix { get; set; }
        public string SmsTemplateSuffix { get; set; }
    }
}
