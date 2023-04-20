using bbt.gateway.common.Attributes;
using System.ComponentModel.DataAnnotations;

namespace bbt.gateway.common.Models.v2
{
    public class MailMultipleRequest
    {
        [Required]
        public SenderType Sender { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string From { get; set; }
        [Required]
        public List<MultipleMail> MailAdresses { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Subject { get; set; }
        [Required(AllowEmptyStrings =false)]
        public string Content { get; set; }
        public List<Attachment>? Attachments { get; set; }
        [CitizenshipNo(10, 11)]
        public string[] Tags { get; set; }
        public bool? CheckIsVerified { get; set; } = false;
        public Process Process { get; set; }
    }
}
