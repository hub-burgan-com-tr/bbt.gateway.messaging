using bbt.gateway.common.Repositories;

namespace bbt.gateway.common
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly DatabaseContext _databaseContext;
        //private readonly DodgeDatabaseContext _dodgeDatabaseContext;
        private readonly SmsBankingDatabaseContext _smsBankingDatabaseContext;
        //private UserRepository _userRepository;
        private DirectBlacklistRepository _directBlacklistRepository;
        private HeaderRepository _headerRepository;
        private OperatorRepository _operatorRepository;
        private BlacklistEntryRepository _blacklistEntryRepository;
        private PhoneConfigurationRepository _phoneConfigurationRepository;
        private MailConfigurationRepository _mailConfigurationRepository;
        private OtpRequestLogRepository _otpRequestLogRepository;
        private MailRequestLogRepository _mailRequestLogRepository;
        private PushNotificationRequestLogRepository _pushNotificationRequestLogRepository;
        private SmsResponseLogRepository _smsResponseLogRepository;
        private SmsRequestLogRepository _smsRequestLogRepository;
        private SmsTrackingLogRepository _smsTrackingLogRepository;
        private OtpResponseLogRepository _otpResponseLogRepository;
        private MailResponseLogRepository _mailResponseLogRepository;
        private PushNotificationResponseLogRepository _pushNotificationResponseLogRepository;
        private OtpTrackingLogRepository _otpTrackingLogRepository;
        private TransactionRepository _transactionRepository;
        private WhitelistRepository _whitelistRepository;

        public RepositoryManager(DatabaseContext databaseContext,
            SmsBankingDatabaseContext smsBankingDatabaseContext)
        {
            _databaseContext = databaseContext;
            //_dodgeDatabaseContext = dodgeDatabaseContext;
            _smsBankingDatabaseContext = smsBankingDatabaseContext;
        }

        public IHeaderRepository Headers => _headerRepository ??= new HeaderRepository(_databaseContext);

        public IOperatorRepository Operators => _operatorRepository ??= new OperatorRepository(_databaseContext);

        public IBlacklistEntryRepository BlackListEntries => _blacklistEntryRepository ??= new BlacklistEntryRepository(_databaseContext);

        public IPhoneConfigurationRepository PhoneConfigurations => _phoneConfigurationRepository ??= new PhoneConfigurationRepository(_databaseContext);

        public IOtpRequestLogRepository OtpRequestLogs => _otpRequestLogRepository ??= new OtpRequestLogRepository(_databaseContext);

        public ISmsResponseLogRepository SmsResponseLogs => _smsResponseLogRepository ??= new SmsResponseLogRepository(_databaseContext);

        public ISmsRequestLogRepository SmsRequestLogs => _smsRequestLogRepository ?? new SmsRequestLogRepository(_databaseContext);

        public ISmsTrackingLogRepository SmsTrackingLogs => _smsTrackingLogRepository ?? new SmsTrackingLogRepository(_databaseContext);


        public IOtpResponseLogRepository OtpResponseLogs => _otpResponseLogRepository ??= new OtpResponseLogRepository(_databaseContext);

        public IOtpTrackingLogRepository OtpTrackingLog => _otpTrackingLogRepository ??= new OtpTrackingLogRepository(_databaseContext);

        //public IUserRepository Users => _userRepository ??= new UserRepository(_dodgeDatabaseContext);

        public IDirectBlacklistRepository DirectBlacklists => _directBlacklistRepository ??= new DirectBlacklistRepository(_smsBankingDatabaseContext);

        public IMailConfigurationRepository MailConfigurations => _mailConfigurationRepository ??= new MailConfigurationRepository(_databaseContext);

        public IMailRequestLogRepository MailRequestLogs => _mailRequestLogRepository ??= new MailRequestLogRepository(_databaseContext);

        public IMailResponseLogRepository MailResponseLogs => _mailResponseLogRepository ??= new MailResponseLogRepository(_databaseContext);
        public IPushNotificationRequestLogRepository PushNotificationRequestLogs => _pushNotificationRequestLogRepository ??= new PushNotificationRequestLogRepository(_databaseContext);

        public IPushNotificationResponseLogRepository PushNotificationResponseLogs => _pushNotificationResponseLogRepository ??= new PushNotificationResponseLogRepository(_databaseContext);

        public ITransactionRepository Transactions => _transactionRepository ??= new TransactionRepository(_databaseContext);

        public IWhitelistRepository Whitelist => _whitelistRepository ??= new WhitelistRepository(_databaseContext);

        public async Task<int> SaveChangesAsync()
        {
            return await _databaseContext.SaveChangesAsync();
        }

        //public int SaveDodgeChanges()
        //{
        //    return _dodgeDatabaseContext.SaveChanges();
        //}

        public async Task<int> SaveSmsBankingChangesAsync()
        {
            return await _smsBankingDatabaseContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            
        }
    }
}
