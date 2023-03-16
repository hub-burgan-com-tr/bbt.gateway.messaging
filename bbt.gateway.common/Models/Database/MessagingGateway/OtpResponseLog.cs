using bbt.gateway.common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bbt.gateway.common.Models
{
    public class OtpResponseLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public OperatorType Operator { get; set; }
        public string Topic { get; set; }
        public SendSmsResponseStatus ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public string StatusQueryId { get; set; }
        public SmsTrackingStatus TrackingStatus { get; set; }
        public ICollection<OtpTrackingLog> TrackingLogs { get; set; } = new List<OtpTrackingLog>();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string RequestBody { get; set; }
        public string ResponseBody { get; set; }
    }
}
