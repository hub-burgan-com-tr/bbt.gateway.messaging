using System;

namespace bbt.gateway.common.Repositories
{
    public interface IRepositoryManager : IDisposable
    {
        IHeaderRepository Headers { get; }
        IOperatorRepository Operators { get; }
        IBlacklistEntryRepository BlackListEntries { get; }
        IPhoneConfigurationRepository PhoneConfigurations { get; }
        IMailConfigurationRepository MailConfigurations { get; }
        IOtpRequestLogRepository OtpRequestLogs { get; }
        ISmsResponseLogRepository SmsResponseLogs { get;  }
        ISmsRequestLogRepository SmsRequestLogs { get; }
        ISmsTrackingLogRepository SmsTrackingLogs { get; }
        IMailRequestLogRepository MailRequestLogs { get; }
        IMailResponseLogRepository MailResponseLogs { get; }
        IPushNotificationRequestLogRepository PushNotificationRequestLogs { get; }
        IPushNotificationResponseLogRepository PushNotificationResponseLogs { get; }
        IOtpResponseLogRepository OtpResponseLogs { get; }
        IOtpTrackingLogRepository OtpTrackingLog { get; }
        ITransactionRepository Transactions { get; }
        //IUserRepository Users { get; }
        IDirectBlacklistRepository DirectBlacklists { get; }
        IWhitelistRepository Whitelist { get; }
        Task<int> SaveChangesAsync();
        //int SaveDodgeChanges();
        Task<int> SaveSmsBankingChangesAsync();
    }
}
