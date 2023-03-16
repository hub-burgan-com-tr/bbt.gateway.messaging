using bbt.gateway.common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace bbt.gateway.messaging.Api.Turkcell.Model
{

    public class TurkcellSmsRequest
    {
        public bool IsAbroad { get; set; }
        public string PhoneNo{get;set;}
        public string Header { get; set; }
        public string Content { get; set; }
        public string TrustedDate { get; set; }
        public string SessionId { get; set; }
        public string VariantId { get; set; }
        public string MsgCode { get; set; }
    }

    
}

