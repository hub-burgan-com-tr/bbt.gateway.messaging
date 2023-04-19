using bbt.gateway.common.Attributes;
using System.ComponentModel.DataAnnotations;

namespace bbt.gateway.common.Models.v2
{
    public class TemplatedMailMultipleRequest
    {
        [Required]
        public SenderType Sender { get; set; }
        [Required]
        public List<MultipleMail> MailAdresses { get; set; }
        [Required]
        public string Template { get; set; }
        public string? TemplateParams { get; set; }
        public List<Attachment>? Attachments { get; set; }
        public string[] Tags { get; set; }
        public bool? CheckIsVerified { get; set; } = false;
        public Process Process { get; set; }
    }

    public class MultipleMail
    {
        [EmailAddress]
        public string Email { get; set; }
        public string? Cc { get; set; }
        public string? Bcc { get; set; }
    }
}
