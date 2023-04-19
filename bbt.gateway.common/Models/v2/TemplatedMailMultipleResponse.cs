namespace bbt.gateway.common.Models.v2
{
    public class TemplatedMailMultipleResponse
    {
        public List<TemplatedMailMultipleResponseData> TemplatedMailResponse { get; set; } = new();
    }

    public class TemplatedMailMultipleResponseData
    {
        public string MailAddress { get; set; }
        public Guid TxnId { get; set; }
        public dEngageResponseCodes Status { get; set; }
        public string StatusMessage { get; set; }
    }
}
