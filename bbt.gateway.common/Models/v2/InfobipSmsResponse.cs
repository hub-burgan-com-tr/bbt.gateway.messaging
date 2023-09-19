namespace bbt.gateway.common.Models.v2
{
    public class InfobipSmsResponse
    {
        public Guid TxnId { get; set; }
        public InfobipResponseCodes Status { get; set; }
    }
}
