namespace bbt.gateway.common.Api.dEngage.Model.Transactional
{
    public class SendMailResponse
    {
        public string transactionId { get; set; }
        public int code { get; set; }
        public string message { get; set; }
        public DataMail data { get; set; }
        public string GroupId { get; set; }
        
    }

    public class DataMail 
    {
        public MailTo to { get; set; }
        public MailTo cc { get; set; }
        public MailTo bcc { get; set; }
    }

    public class MailTo
    {
        public string email { get; set; }
        public string transactionId { get; set; }
    }

}
