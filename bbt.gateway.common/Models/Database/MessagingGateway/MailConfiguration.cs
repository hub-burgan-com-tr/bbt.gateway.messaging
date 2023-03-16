using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bbt.gateway.common.Models
{
    public class MailConfiguration
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Email { get; set; } 
        public ulong? CustomerNo { get; set; } 
        public ICollection<MailConfigurationLog> Logs { get; set; }
        public ICollection<MailRequestLog> MailLogs {get;set;}
    }
}
