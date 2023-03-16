using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace bbt.gateway.messaging.Api.TurkTelekom.Model
{
    public class TurkTelekomResponseSmsStatus
    {
        [XmlElement(ElementName = "MESSAGEID")]
        public string MessageId { get; set; }
        [XmlElement(ElementName = "SENDPHONENO")]
        public string SendPhoneNo { get; set; }
        [XmlElement(ElementName = "PHONENO")]
        public string PhoneNo { get; set; }
        [XmlElement(ElementName = "SENDDATE")]
        public string SendDate { get; set; }
        [XmlElement(ElementName = "STATUS")]
        public string Status { get; set; }
        [XmlElement(ElementName = "STATUSDESC")]
        public string StatusDesc { get; set; }
        [XmlElement(ElementName = "DELIVERSTATUS")]
        public string DeliverStatus { get; set; }
        [XmlElement(ElementName = "DELIVEREDDATE")]
        public string DeliveredDate { get; set; }
        [XmlElement(ElementName = "DCODE")]
        public string DCode { get; set; }
        [XmlElement(ElementName = "DCODEDESC")]
        public string DCodeDesc { get; set; }
        [XmlElement(ElementName = "CHANNELSTATUS")]
        public string ChannelStatus { get; set; }
        
    }
}
