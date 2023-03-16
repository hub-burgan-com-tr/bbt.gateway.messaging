using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbt.gateway.common.Models
{
    public class MiddlewareRequest
    {
        public ulong? CustomerNo { get; set; }
        public string ContactId { get; set; }
        public string CitizenshipNo { get; set; }
        public HeaderInfo? HeaderInfo { get; set; }
        public Phone Phone { get; set; }
        public string Email { get; set; }
        public string Template { get; set; }
        public string TemplateParams { get; set; }
        public string CustomParameters { get; set; }
        public string Content { get; set; }
        public MessageContentType ContentType { get; set; }
        public Process Process { get; set; }
        public v2.SenderType Sender { get; set; }
        public SmsTypes SmsType { get; set; }
    }
}
