using bbt.gateway.common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace bbt.gateway.messaging.Api.Turkcell.Model
{
    
    public class TurkcellSmsStatusRequest 
    {
        public string AuthToken { get; set; }
        public string MsgId { get; set; }
    }

}
