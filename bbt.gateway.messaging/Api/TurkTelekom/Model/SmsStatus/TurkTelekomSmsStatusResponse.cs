using bbt.gateway.common.Models;
using System.Xml.Serialization;

namespace bbt.gateway.messaging.Api.TurkTelekom.Model
{
    [XmlRoot(ElementName = "VIPSMS_REPORT")]
    public class TurkTelekomSmsStatusResponse
    {


        [XmlElement(ElementName = "SMS")]
        public TurkTelekomResponseSmsStatus ResponseSmsStatus { get; set; }

        
    }
}
