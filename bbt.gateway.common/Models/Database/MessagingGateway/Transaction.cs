using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace bbt.gateway.common.Models
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public Phone Phone { get; set; }
        public string Mail { get; set; }
        public string CitizenshipNo { get; set; }
        public ulong CustomerNo { get; set; }
        public TransactionType TransactionType { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string IpAdress { get; set; }
        public Process CreatedBy { get; set; }

        public Guid SmsRequestLogId{get;set;}
        public Guid MailRequestLogId{get;set;}
        public OtpRequestLog OtpRequestLog { get; set; }
        [ForeignKey("SmsRequestLogId")]
        public SmsRequestLog SmsRequestLog { get; set; }
        [ForeignKey("MailRequestLogId")]
        public MailRequestLog MailRequestLog { get; set; }
        public PushNotificationRequestLog PushNotificationRequestLog { get; set; }


    }
}
