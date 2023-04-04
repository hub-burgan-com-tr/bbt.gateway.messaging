using bbt.gateway.common.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace bbt.gateway.common.Repositories
{
    public class BlacklistEntryRepository : Repository<BlackListEntry>, IBlacklistEntryRepository
    {
        public BlacklistEntryRepository(DatabaseContext context) : base(context)
        { 
        
        }

        public async Task<BlackListEntry> GetLastBlacklistRecord(int countryCode, int prefix, int number)
        {
            return await Context.BlackListEntries.
                Where(b => 
                b.PhoneConfiguration.Phone.CountryCode == countryCode
                && b.PhoneConfiguration.Phone.Prefix == prefix
                && b.PhoneConfiguration.Phone.Number == number)
                .OrderByDescending(b => b.CreatedAt).
                FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<BlackListEntry>> GetWithLogsAsync(int countryCode, int prefix, int number, int page, int pageSize)
        {
            return await Context.BlackListEntries
                
                .Where(b => b.PhoneConfiguration.Phone.CountryCode == countryCode && b.PhoneConfiguration.Phone.Prefix == prefix && b.PhoneConfiguration.Phone.Number == number)
                .Include(b => b.Logs)
                .Skip(page)
                .Take(pageSize)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }
        public async Task<(IEnumerable<BlackListEntry>, int)> GetBlackListByPhoneAsync(int countryCode, int prefix, int number, int page, int pageSize)
        {
            IEnumerable<BlackListEntry> list =await Context.BlackListEntries

                .Where(b => b.PhoneConfiguration.Phone.CountryCode == countryCode && b.PhoneConfiguration.Phone.Prefix == prefix && b.PhoneConfiguration.Phone.Number == number)
              
                .Skip(page)
                .Take(pageSize)
                .ToListAsync();
            int count  = await Context.BlackListEntries
                                .CountAsync(b => b.PhoneConfiguration.Phone.CountryCode == countryCode && b.PhoneConfiguration.Phone.Prefix == prefix && b.PhoneConfiguration.Phone.Number == number);
            return (list, count);
        }
        public async Task<(IEnumerable<BlackListEntry>, int)> GetBlackListByCustomerNoAsync(ulong customerNo, int page, int pageSize)
        {
            IEnumerable<BlackListEntry> list = await Context.BlackListEntries

                 .Where(b => b.PhoneConfiguration.CustomerNo==customerNo)

                 .Skip(page)
                 .Take(pageSize)
                 .ToListAsync();
            int count = await Context.BlackListEntries
                                .CountAsync(b => b.PhoneConfiguration.CustomerNo == customerNo);
            return (list, count);
        }
    }
}
