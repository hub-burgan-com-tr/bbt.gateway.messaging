using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bbt.gateway.common.Models
{
    public class SendTemplatedSmsRequest : SendSmsRequest
    {
        public HeaderInfo HeaderInfo { get; set; }
        public string TemplateParams { get; set; }
        public string Template { get; set; }
        public long? CustomerNo { get; set; }
        public string ContactId { get; set; }
    }
}
