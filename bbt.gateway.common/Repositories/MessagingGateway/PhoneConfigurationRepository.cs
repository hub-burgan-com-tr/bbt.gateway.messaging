using bbt.gateway.common.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace bbt.gateway.common.Repositories
{
    public class PhoneConfigurationRepository : Repository<PhoneConfiguration>, IPhoneConfigurationRepository
    {
        public PhoneConfigurationRepository(DatabaseContext context) : base(context)
        { 
        
        }

        public async Task DeletePhoneConfiguration(Guid id)
        {
            await Context.Database.ExecuteSqlRawAsync("deletePhoneConfiguration @p0",parameters: new[] {id.ToString()});
        }

        public async Task<PhoneConfiguration> GetWithBlacklistEntriesAsync(int countryCode, int prefix, int number, DateTime blackListValidDate)
        {
           
            return await Context.PhoneConfigurations.Where(i =>
                i.Phone.CountryCode == countryCode &&
                i.Phone.Prefix == prefix &&
                i.Phone.Number == number
                )
                .Include(c => c.BlacklistEntries.Where(b => b.ValidTo > blackListValidDate))
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PhoneConfiguration>> GetWithRelatedLogsAndBlacklistEntriesAsync(int countryCode, int prefix, int number, int count)
        {
            return await Context.PhoneConfigurations.Where(c => c.Phone.CountryCode == countryCode && c.Phone.Prefix == prefix && c.Phone.Number == number)
                .Include(c => c.BlacklistEntries.Take(count).OrderByDescending(l => l.CreatedAt))
                .Include(c => c.OtpLogs.Take(count).OrderByDescending(l => l.CreatedAt)).ThenInclude(o => o.ResponseLogs)
                .Include(c => c.Logs.Take(count).OrderByDescending(l => l.CreatedAt))
                .Include(c => c.SmsLogs.Take(count).OrderByDescending(l => l.CreatedAt))
                .ToListAsync();
        }

       
    }
}
