namespace bbt.gateway.messaging.Api.Infobip.Model
{
    public class InfobipApiSmsStatusResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int GroupId { get; set; }
        public int SubCode { get; set; }
    }
}
