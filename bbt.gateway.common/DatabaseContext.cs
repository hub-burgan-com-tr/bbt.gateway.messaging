using bbt.gateway.common.Models;
using Microsoft.EntityFrameworkCore;

namespace bbt.gateway.common
{
    public class DatabaseContext : DbContext
    {
        public DbSet<PhoneConfiguration> PhoneConfigurations { get; set; }
        public DbSet<Header> Headers { get; set; }
        public DbSet<Operator> Operators { get; set; }
        public DbSet<BlackListEntry> BlackListEntries { get; set; }
        public DbSet<OtpRequestLog> OtpRequestLogs { get; set; }
        public DbSet<OtpResponseLog> OtpResponseLog { get; set; }
        public DbSet<OtpTrackingLog> OtpTrackingLog { get; set; }
        public DbSet<SmsRequestLog> SmsRequestLog { get; set; }
        public DbSet<SmsResponseLog> SmsResponseLog { get; set; }
        public DbSet<SmsTrackingLog> SmsTrackingLog { get; set; }
        public DbSet<MailConfiguration> MailConfigurations { get; set; }
        public DbSet<MailRequestLog> MailRequestLog { get; set; }
        public DbSet<MailResponseLog> MailResponseLog { get; set; }
        public DbSet<MailTrackingLog> MailTrackingLog { get; set; }
        public DbSet<PushNotificationRequestLog> PushNotificationRequestLogs { get; set; }
        public DbSet<PushNotificationResponseLog> PushNotificationResponseLogs { get; set; }

        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<WhiteList> WhiteList { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<PhoneConfiguration>().OwnsOne(i => i.Phone);
            builder.Entity<PhoneConfigurationLog>().OwnsOne(i => i.CreatedBy);
            builder.Entity<MailConfigurationLog>().OwnsOne(i => i.CreatedBy);
            builder.Entity<MailRequestLog>().OwnsOne(i => i.CreatedBy);
            builder.Entity<OtpRequestLog>().OwnsOne(i => i.CreatedBy);
            builder.Entity<OtpRequestLog>().OwnsOne(i => i.Phone);
            builder.Entity<SmsRequestLog>().OwnsOne(i => i.CreatedBy);
            builder.Entity<SmsRequestLog>().OwnsOne(i => i.Phone);
            builder.Entity<BlackListEntry>().OwnsOne(i => i.CreatedBy);
            builder.Entity<BlackListEntry>().OwnsOne(i => i.ResolvedBy);
            builder.Entity<BlackListEntryLog>().OwnsOne(i => i.CreatedBy);
            builder.Entity<Transaction>().OwnsOne(i => i.CreatedBy);
            builder.Entity<Transaction>().OwnsOne(i => i.Phone);
            builder.Entity<WhiteList>().OwnsOne(i => i.CreatedBy);
            builder.Entity<WhiteList>().OwnsOne(i => i.Phone);
            builder.Entity<PushNotificationRequestLog>().OwnsOne(i => i.CreatedBy);
            //Non-cluster Guid index sample
            builder.Entity<PhoneConfiguration>()
                .HasIndex(c => c.Id)
                .IsClustered(false);
            builder.Entity<Transaction>()
                .HasIndex(t => t.CustomerNo)
                .IsClustered(false);

            builder.Entity<Operator>().HasData(new Operator { Id = 1, Type = OperatorType.Turkcell, ControlDaysForOtp = 60, Status = OperatorStatus.Active });
            builder.Entity<Operator>().HasData(new Operator { Id = 2, Type = OperatorType.Vodafone, ControlDaysForOtp = 60, Status = OperatorStatus.Active });
            builder.Entity<Operator>().HasData(new Operator { Id = 3, Type = OperatorType.TurkTelekom, ControlDaysForOtp = 60, Status = OperatorStatus.Active });
            builder.Entity<Operator>().HasData(new Operator { Id = 4, Type = OperatorType.MarketingChannel, ControlDaysForOtp = 60, Status = OperatorStatus.Active });
            builder.Entity<Operator>().HasData(new Operator { Id = 5, Type = OperatorType.IVN, ControlDaysForOtp = 60, Status = OperatorStatus.Active });
            builder.Entity<Operator>().HasData(new Operator { Id = 6, Type = OperatorType.dEngageBurgan, ControlDaysForOtp = 0, Status = OperatorStatus.Active });
            builder.Entity<Operator>().HasData(new Operator { Id = 7, Type = OperatorType.dEngageOn, ControlDaysForOtp = 0, Status = OperatorStatus.Active });


            builder.Entity<Header>().HasData(new Header { Id = Guid.NewGuid(), SmsSender = SenderType.Burgan, SmsPrefix = "", SmsSuffix = "", ContentType = MessageContentType.Otp });
            builder.Entity<Header>().HasData(new Header { Id = Guid.NewGuid(), Branch = 2000, SmsSender = SenderType.On, SmsPrefix = "", ContentType = MessageContentType.Private });
        }

    }
}
