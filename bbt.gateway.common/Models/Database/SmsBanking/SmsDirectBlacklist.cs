using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bbt.gateway.common.Models
{
    public class SmsDirectBlacklist
    {
        [Key]
        [Column("sms_id")]
        public long SmsId { get; set; }
        [Column("cust_no")]
        public long CustomerNo { get; set; }
        public string GsmNumber { get; set; }
        public DateTime RecordDate { get; set; }
        public bool IsVerified { get; set; }
        public string? VerifiedBy { get; set; }

        public DateTime? VerifyDate{get;set;}
        public int? TryCount { get; set; }
        public string? Explanation { get; set; }


    }
}
