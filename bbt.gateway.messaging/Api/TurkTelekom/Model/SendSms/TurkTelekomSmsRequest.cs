using bbt.gateway.common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace bbt.gateway.messaging.Api.TurkTelekom.Model
{
    [XmlRoot(ElementName = "SMS")]
    public class TurkTelekomSmsRequest : TurkTelekomBaseRequest
    {
        
        [XmlElement(ElementName = "GSMNO")]
        public string GsmNo { get; set; }
        [XmlElement(ElementName = "CHECKDATE")]
        public string CheckDate { get; set; }
        [XmlElement(ElementName = "DURATION")]
        public string Duration { get; set; }
        [XmlElement(ElementName = "HEADER")]
        public string Header { get; set; }
        [XmlElement(ElementName = "ISENCRYPTED")]
        public string IsEncrypted { get; set; }
        [XmlElement(ElementName = "MESSAGE")]
        public string Message { get; set; }
        [XmlElement(ElementName = "ONNETSIMCHANGE")]
        public string OnNetSimChange { get; set; }
        [XmlElement(ElementName = "ONNETPORTINCONTROL")]
        public string OnNetPortInControl { get; set; }
        [XmlElement(ElementName = "PORTINCHECKDATE")]
        public string PortInCheckDate { get; set; }
        [XmlElement(ElementName = "ISNOTIFICATION")]
        public string IsNotification { get; set;}
        [XmlElement(ElementName = "RECIPIENTTYPE")]
        public string RecipientType { get; set; }
        [XmlElement(ElementName = "BRANCHCODE")]
        public string BrandCode { get; set; }

    }
}
