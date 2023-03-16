using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace bbt.gateway.messaging.Api.TurkTelekom.Model
{
    public class TurkTelekomBaseRequest
    {
        [XmlElement(ElementName = "USERCODE")]
        public string UserCode { get; set; }
        [XmlElement(ElementName = "PASSWORD")]
        public string Password { get; set; }
    }
}
