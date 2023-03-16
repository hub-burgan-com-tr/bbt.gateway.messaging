using bbt.gateway.common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace bbt.gateway.common
{
    public class SmsBankingDatabaseContext : DbContext
    {
        public DbSet<SmsDirectBlacklist> SmsDirectBlackList { get; set; }
        public SmsBankingDatabaseContext(DbContextOptions<SmsBankingDatabaseContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<SmsDirectBlacklist>()
                .ToTable("SmsDirectBlackList","OTP");
        }

    }
}
