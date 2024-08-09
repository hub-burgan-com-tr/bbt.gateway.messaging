using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace bbt.gateway.common.Models
{
    public class SmsResponseLog : IdEngageResponse,ICodecResponse
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public OperatorType Operator { get; set; }
        public int OperatorResponseCode { get; set; }
        public string OperatorResponseMessage { get; set; }
        public string StatusQueryId { get; set; }
        public string Status { get; set; }
        [ForeignKey("SmsRequestLogId")]
        public Guid SmsRequestLogId{get;set;}
        public ICollection<SmsTrackingLog> TrackingLogs { get; set; } = new List<SmsTrackingLog>();
        public DateTime CreatedAt { get; set; }

        public string GetResponseCode()
        {
            return OperatorResponseCode.ToString();
        }

        public int GetCodecResponseCode()
        {
            return OperatorResponseCode;
        }
    }
}
