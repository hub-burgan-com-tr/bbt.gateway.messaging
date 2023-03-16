using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace bbt.gateway.common.Models
{
    public class OtpTrackingLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [ForeignKey("OtpResponseLog")]
        public Guid LogId { get; set; }
        public SmsTrackingStatus Status { get; set; }

        public string ResponseMessage { get; set; }
        public string Detail { get; set; }
        public DateTime QueriedAt { get; set; } = DateTime.Now;
        public OtpResponseLog OtpResponseLog { get; set; }
    }
}
