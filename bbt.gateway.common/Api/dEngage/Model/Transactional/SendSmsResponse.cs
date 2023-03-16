namespace bbt.gateway.common.Api.dEngage.Model.Transactional
{
    public class SendSmsResponse
    {
        public string transactionId { get; set; }
        public int code { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
        
    }

    public class Data 
    {
        public To to { get; set; }
    }

    public class To
    {
        public string gsm { get; set; }
        public string trackingId { get; set; }
    }
}
