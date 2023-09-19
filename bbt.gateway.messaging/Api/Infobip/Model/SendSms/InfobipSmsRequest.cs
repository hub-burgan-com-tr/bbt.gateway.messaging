using System.Collections.Generic;

namespace bbt.gateway.messaging.Api.Infobip.Model.SendSms
{
    public class InfobipSmsRequest
    {
        public List<Message> messages { get; set; }
    }

    public class Message
    {
        public List<Destination> destinations { get; set; }
        public string from { get; set; }
        public string text { get; set; }
    }

    public class Destination
    {
        public string to { get; set; }
    }
}
