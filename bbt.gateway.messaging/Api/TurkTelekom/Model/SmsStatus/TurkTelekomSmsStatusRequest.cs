using bbt.gateway.common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace bbt.gateway.messaging.Api.TurkTelekom.Model
{
    [XmlRoot(ElementName = "VIPSMS_REPORT")]
    public class TurkTelekomSmsStatusRequest : TurkTelekomBaseRequest
    {

        [XmlElement(ElementName = "MESSAGEID")]
        public string MessageId { get; set; }
        [XmlElement(ElementName = "LASTMESSAGEID")]
        public string LastMessageId { get; set; }

    }
}
