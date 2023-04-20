using System.ComponentModel.DataAnnotations.Schema;

namespace bbt.gateway.common.Models
{
    public class MailTrackingLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [ForeignKey("MailResponseLog")]
        [Column("MailResponseLogId")]
        public Guid LogId { get; set; }
        public MailTrackingStatus Status { get; set; }
        public MailTrackingType Type { get; set; }
        public int BounceCode { get; set; }
        public string BounceText { get; set; }
        public string Detail { get; set; }
        public DateTime QueriedAt { get; set; } = DateTime.Now;
        public MailResponseLog MailResponseLog { get; set; }
    }

    public enum MailTrackingType
    {
        To,
        Cc,
        Bcc
    }
}
