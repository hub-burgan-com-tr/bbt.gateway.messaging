using bbt.gateway.common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bbt.gateway.common.Models
{
    public class MailResponseLog : IdEngageResponse
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Topic { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public string StatusQueryId { get; set; }
        public string Status { get; set; }
        public ICollection<MailTrackingLog> TrackingLogs { get; set; } = new List<MailTrackingLog>();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public OperatorType Operator { get; set; }

        public  string GetResponseCode()
        {
            return ResponseCode;
        }
    }
}
