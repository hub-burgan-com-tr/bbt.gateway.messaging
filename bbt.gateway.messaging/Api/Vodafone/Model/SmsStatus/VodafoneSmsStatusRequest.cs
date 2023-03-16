using bbt.gateway.common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace bbt.gateway.messaging.Api.Vodafone.Model
{
    public class VodafoneSmsStatusRequest
    {
       public string MessageId { get; set; }
       public string AuthToken { get; set; }
       public string User { get; set; }

    }
}
