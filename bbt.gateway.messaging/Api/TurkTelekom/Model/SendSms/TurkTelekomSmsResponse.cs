using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using bbt.gateway.common.Models;

namespace bbt.gateway.messaging.Api.TurkTelekom.Model
{
    [XmlRoot(ElementName = "VIPSMS")]
    public class TurkTelekomSmsResponse
    {
      

        [XmlElement(ElementName = "SMS")]
        public TurkTelekomResponseSms ResponseSms { get; set; }

        
    }
}
