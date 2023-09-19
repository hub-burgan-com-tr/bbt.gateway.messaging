using System.Collections.Generic;

namespace bbt.gateway.messaging.Api.Infobip.Model.SendSms
{
    public class InfobipSmsResponse
    {
        public List<ResponseMessage> messages { get; set; }
    }

    public class ResponseMessage
    {
        public string messageId { get; set; }
    }
}
