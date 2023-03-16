using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bbt.gateway.common.Models
{
    public class SendMessageEmailRequest : SendEmailRequest
    {
        public string From { get; set; }    
        public long? CustomerNo { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string ContactId { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
    }
}
