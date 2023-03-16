using bbt.gateway.common.Models;
using System.Collections.Generic;

namespace bbt.gateway.common.Repositories
{
    public interface IOtpRequestLogRepository : IRepository<OtpRequestLog>
    {
        Task<IEnumerable<OtpRequestLog>> GetWithResponseLogsAsync(int countryCode, int prefix, int number, int page, int pageSize);
    }
}
