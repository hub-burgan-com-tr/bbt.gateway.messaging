namespace bbt.gateway.messaging.Api.Infobip.Model
{
    public class InfobipApiSmsResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string MsgId { get; set; }
        public string RequestBody { get; set; }
        public string ResponseBody { get; set; }
    }
}
