namespace bbt.gateway.common.Models
{
    public class PushTrackingLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public PushTrackingStatus Status { get; set; }
        public string Detail { get; set; }
        public DateTime QueriedAt { get; set; } = DateTime.Now;
        public PushNotificationResponseLog PushResponseLog { get; set; }
    }
}
