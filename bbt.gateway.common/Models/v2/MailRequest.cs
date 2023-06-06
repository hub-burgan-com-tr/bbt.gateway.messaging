using bbt.gateway.common.Attributes;
using System.ComponentModel.DataAnnotations;

namespace bbt.gateway.common.Models.v2
{
    public class MailRequest
    {
        [Required]
        public SenderType Sender { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string From { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Subject { get; set; }
        [Required(AllowEmptyStrings =false)]
        public string Content { get; set; }
        public List<Attachment>? Attachments { get; set; }
        public string? Cc { get; set; }
        public string? Bcc { get; set; }
        public long? CustomerNo { get; set; }
        [CitizenshipNo(10, 11)]
        public string? CitizenshipNo { get; set; }
        public string[] Tags { get; set; }
        public bool? CheckIsVerified { get; set; } = false;
        public bool? InstantReminder { get; set; } = false;
        public Process Process { get; set; }
    }
}
