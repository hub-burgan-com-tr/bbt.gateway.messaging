namespace bbt.gateway.common.Models.v2
{
    public class NativePushResponse
    {
        public Guid TxnId { get; set; }
        public NativePushResponseCodes Status { get; set; }
    }
}