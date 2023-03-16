using System.ComponentModel.DataAnnotations.Schema;

namespace bbt.gateway.common.Models
{
    public class MailRequestLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public OperatorType Operator { get; set; }
        public MailConfiguration MailConfiguration { get; set; }
        public string FromMail { get; set; }
        public string subject { get; set; }
        public string content { get; set; }
        public string TemplateId { get; set; }
        public string TemplateParams { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Process CreatedBy { get; set; }
        public ICollection<MailResponseLog> ResponseLogs { get; set; } = new List<MailResponseLog>();

    }
}
