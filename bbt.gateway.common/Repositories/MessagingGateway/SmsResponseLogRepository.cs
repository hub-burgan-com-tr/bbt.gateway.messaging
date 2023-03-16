using bbt.gateway.common.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace bbt.gateway.common.Repositories
{
    public class SmsResponseLogRepository : Repository<SmsResponseLog>, ISmsResponseLogRepository
    {
        public SmsResponseLogRepository(DatabaseContext context) : base(context)
        { 
        
        }

        public async Task<IEnumerable<SmsResponseLog>> GetSmsResponseLogsAsync(Expression<Func<SmsResponseLog, bool>> predicate)
        {
            return await Context.SmsResponseLog
                .Include(s => s.TrackingLogs)
                .Where(predicate)
                .Take(5)
                .OrderBy(s => s.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<SmsResponseLog>> GetSmsResponseLogsDescAsync(Expression<Func<SmsResponseLog, bool>> predicate)
        {
            return await Context.SmsResponseLog
                .Include(s => s.TrackingLogs)
                .Where(predicate)
                .Take(5)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

    }
}
