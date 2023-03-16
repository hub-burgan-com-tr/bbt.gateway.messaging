using System.ComponentModel.DataAnnotations.Schema;

namespace bbt.gateway.common.Models
{
    public class OtpRequestLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public PhoneConfiguration PhoneConfiguration { get; set; }
        public Phone Phone { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Process CreatedBy { get; set; }
        public ICollection<OtpResponseLog> ResponseLogs { get; set; } = new List<OtpResponseLog>();

    }
}
