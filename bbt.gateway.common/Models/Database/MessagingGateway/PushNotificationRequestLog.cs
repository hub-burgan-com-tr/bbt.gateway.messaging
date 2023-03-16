using System.ComponentModel.DataAnnotations.Schema;

namespace bbt.gateway.common.Models
{
    public class PushNotificationRequestLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public OperatorType Operator { get; set; }
        public string ContactId { get; set; }
        public string TemplateId { get; set; }
        public string TemplateParams { get; set; }
        public string Content { get; set; }
        public string CustomParameters { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Process CreatedBy { get; set; }
        public ICollection<PushNotificationResponseLog> ResponseLogs { get; set; } = new List<PushNotificationResponseLog>();

    }
}
