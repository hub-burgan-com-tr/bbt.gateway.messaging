namespace bbt.gateway.messaging.Api.Infobip.Model.SendSms
{
    public class InfobipErrorResponse
    {
        public RequestError requestError { get; set; }
    }

    public class RequestError
    {
        public ServiceException serviceException { get; set; }
    }

    public class ServiceException
    {
        public string messageId { get; set; }
        public string text { get; set; }
    }
}
