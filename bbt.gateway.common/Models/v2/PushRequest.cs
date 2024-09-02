using bbt.gateway.common.Attributes;
using System.ComponentModel.DataAnnotations;

namespace bbt.gateway.common.Models.v2
{
    public class PushRequest
    {
        [Required]
        public SenderType Sender { get; set; }
        public string CitizenshipNo { get; set; }
        public bool? IsFirebase { get; set; } = false;
        public string? Title { get; set; }
        [Required]
        public string Content { get; set; }
        public long? CustomerNo { get; set; }
        public string[] Tags { get; set; }
        public bool? saveInbox { get; set; }
        public Process Process { get; set; }
    }
}
