using bbt.gateway.common.Attributes;
using System.ComponentModel.DataAnnotations;

namespace bbt.gateway.common.Models.v2
{
    public class PushRequest
    {
        [Required]
        public SenderType Sender { get; set; }
        public string CitizenshipNo { get; set; }
        [Required]
        public string Content { get; set; }
        public long? CustomerNo { get; set; }
        public string[] Tags { get; set; }
        public bool? saveInbox { get; set; }
        public Process Process { get; set; }
    }
}
