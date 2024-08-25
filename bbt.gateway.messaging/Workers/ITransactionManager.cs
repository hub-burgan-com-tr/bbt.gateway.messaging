using bbt.gateway.common.Models;
using System;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Workers
{
    public interface ITransactionManager
    {
        public Guid TxnId { get; }
        public Transaction Transaction { get; set; }
        public OtpRequestInfo OtpRequestInfo { get; set; }
        public SmsRequestInfo SmsRequestInfo { get; set; }
        public MailRequestInfo MailRequestInfo { get; set; }
        public PushRequestInfo PushRequestInfo { get; set; }
        public CustomerRequestInfo CustomerRequestInfo { get; set; }
        public HeaderInfo HeaderInfo { get; set; }
        public common.Models.v2.SenderType Sender { get; set; }
        public bool UseFakeSmtp { get; set; }
        public bool? InstantReminder { get; set; }
        public SmsTypes SmsType { get; set; }
        public DateTime OldBlacklistVerifiedAt { get; set; }
        public bool StringSend { get; set; }
        public int PrefixLength { get; set; }
        public int NumberLength { get; set; }
        public common.Models.v2.Phone GetPhoneFromString(common.Models.v2.PhoneString phoneString);
        public Task AddTransactionAsync();
        public Task SaveTransactionAsync();
        public Task GetCustomerInfoByPhone();
        public Task GetCustomerInfoByEmail();
        public Task GetCustomerInfoByCitizenshipNumber();
        public Task GetCustomerInfoByCustomerNo();

        public Task<Operator> GetOperatorAsync(OperatorType type);
        public Task RevokeOperatorsAsync();

        public string? GetTemplateLanguage();
        public void LogState();

        public void LogCritical(string message);
        public void LogWarning(string message);
        public void LogError(string message);
        public void LogDebug(string message);
        public void LogTrace(string message);
        public void LogInformation(string message);

        
    }
}
