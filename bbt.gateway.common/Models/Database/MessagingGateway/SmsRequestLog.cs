using System.ComponentModel.DataAnnotations.Schema;

namespace bbt.gateway.common.Models
{
    public class SmsRequestLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public OperatorType Operator { get; set; }
        public PhoneConfiguration PhoneConfiguration { get; set; }
        public Phone Phone { get; set; }
        public string content { get; set; }
        public string TemplateId { get; set; }
        public string TemplateParams { get; set; }
        public SmsTypes SmsType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Process CreatedBy { get; set; }
        public ICollection<SmsResponseLog> ResponseLogs { get; set; } = new List<SmsResponseLog>();

    }
}
