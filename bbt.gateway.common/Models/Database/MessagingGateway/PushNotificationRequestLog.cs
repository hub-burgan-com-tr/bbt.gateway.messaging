using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bbt.gateway.common.Models
{
    [Index(nameof(ContactId))]
    public class PushNotificationRequestLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public OperatorType Operator { get; set; }
        [MaxLength(11)]
        public string ContactId { get; set; }
        public string TemplateId { get; set; }
        public string TemplateParams { get; set; }
        public string Content { get; set; }
        public string CustomParameters { get; set; }
        public bool SaveInbox { get; set; }
        public bool IsRead { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        [MaxLength(50)]
        public string NotificationType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Process CreatedBy { get; set; }
        public ICollection<PushNotificationResponseLog> ResponseLogs { get; set; } = new List<PushNotificationResponseLog>();

    }
}
