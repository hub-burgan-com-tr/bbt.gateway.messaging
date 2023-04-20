namespace bbt.gateway.common.Models.v2
{
    public class MailMultipleResponse
    {
        public List<TemplatedMailMultipleResponseData> Response { get; set; } = new();
    }
}
