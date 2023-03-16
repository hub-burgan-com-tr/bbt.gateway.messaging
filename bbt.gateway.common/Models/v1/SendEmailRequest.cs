namespace bbt.gateway.common.Models
{
    public abstract class SendEmailRequest
    {
        public Guid Id { get; set; }

        public string Email { get; set; }

        public List<Attachment> Attachments { get; set; }

        public Process Process { get; set; }

    }
}

