using bbt.gateway.common.Attributes;
using System.ComponentModel.DataAnnotations;

namespace bbt.gateway.common.Models.v2
{
    public class TemplatedSmsRequest
    {
        [Required]
        public SenderType Sender { get; set; }
        public Phone Phone { get; set; }
        [Required]
        public string Template { get; set; }
        public string? TemplateParams { get; set; }
        public long? CustomerNo { get; set; }

        [CitizenshipNo(10, 11)]
        public string? CitizenshipNo { get; set; }
        public string[] Tags { get; set; }
        public bool? InstantReminder { get; set; } = false;
        public Process Process { get; set; }
    }
}
