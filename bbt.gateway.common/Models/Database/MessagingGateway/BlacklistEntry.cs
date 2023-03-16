using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bbt.gateway.common.Models
{
    public class BlackListEntry
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public long SmsId { get; set; }
        public Guid PhoneConfigurationId { get; set; }
        public PhoneConfiguration PhoneConfiguration { get; set; }
        public string Reason { get; set; }
        public string Source { get; set; }
        public BlacklistStatus Status { get; set; } = BlacklistStatus.NotResolved;
        public DateTime ValidTo { get; set; }
        public Process CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Process ResolvedBy { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public ICollection<BlackListEntryLog> Logs { get; set; }
    }
}
