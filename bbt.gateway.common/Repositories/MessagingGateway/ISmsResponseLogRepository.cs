using bbt.gateway.common.Models;
using System.Linq.Expressions;

namespace bbt.gateway.common.Repositories
{
    public interface ISmsResponseLogRepository : IRepository<SmsResponseLog>
    {
        Task<IEnumerable<SmsResponseLog>> GetSmsResponseLogsAsync(Expression<Func<SmsResponseLog, bool>> predicate);
        Task<IEnumerable<SmsResponseLog>> GetSmsResponseLogsDescAsync(Expression<Func<SmsResponseLog, bool>> predicate);
    }
}
