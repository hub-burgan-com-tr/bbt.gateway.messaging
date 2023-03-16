using bbt.gateway.common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bbt.gateway.common.Models
{
    public class PushNotificationResponseLog : IdEngageResponse
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Topic { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public ICollection<PushTrackingLog> TrackingLogs { get; set; } = new List<PushTrackingLog>();
        public string StatusQueryId { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string GetResponseCode()
        {
            return ResponseCode;
        }
    }
}
