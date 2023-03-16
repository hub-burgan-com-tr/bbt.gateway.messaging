using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bbt.gateway.common.Models
{
    public class MailConfigurationLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public MailConfiguration Mail { get; set; }
        public string Type { get; set; }
        public string Action { get; set; }
        public Guid RelatedId { get; set; }
        public Process CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

    }
}
