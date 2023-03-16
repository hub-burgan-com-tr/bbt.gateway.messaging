namespace bbt.gateway.common.Models
{
    public class MailTrackingLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public MailTrackingStatus Status { get; set; }
        public int BounceCode { get; set; }
        public string BounceText { get; set; }
        public string Detail { get; set; }
        public DateTime QueriedAt { get; set; } = DateTime.Now;
        public MailResponseLog MailResponseLog { get; set; }
    }
}
