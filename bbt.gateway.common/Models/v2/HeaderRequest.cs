using System.ComponentModel.DataAnnotations;

namespace bbt.gateway.common.Models.v2
{
    public class HeaderRequest
    {
        [Required]
        public SmsTypes SmsType { get; set; }
        [Required(AllowEmptyStrings = false)]
        [StringLength(1)]
        [RegularExpression("[XB]")]
        public string BusinessLine { get; set; }
        public int? Branch { get; set; }
        [Required]
        public SenderType Sender { get; set; }
        public string SmsPrefix { get; set; }
        public string SmsSuffix { get; set; }
    }
}
