namespace bbt.gateway.common.Models.v2
{
    public class FirebasePushResponse
    {
        public Guid TxnId { get; set; }
        public FirebasePushResponseCodes Status { get; set; }
        public string StatusMessage { get; set; }
    }
}
