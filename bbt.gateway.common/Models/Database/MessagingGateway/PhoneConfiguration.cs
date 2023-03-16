using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bbt.gateway.common.Models
{
    public class PhoneConfiguration
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Phone Phone { get; set; } 
        public ulong? CustomerNo { get; set; } 
        public OperatorType? Operator { get; set; }
        public ICollection<PhoneConfigurationLog> Logs { get; set; }
        public ICollection<OtpRequestLog> OtpLogs {get;set;}
        public ICollection<SmsRequestLog> SmsLogs { get; set; }
        public ICollection<BlackListEntry> BlacklistEntries { get; set; }
    }
}
