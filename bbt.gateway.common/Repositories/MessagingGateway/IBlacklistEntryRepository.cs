using bbt.gateway.common.Models;
using System.Collections.Generic;

namespace bbt.gateway.common.Repositories
{
    public interface IBlacklistEntryRepository : IRepository<BlackListEntry>
    {
        Task<IEnumerable<BlackListEntry>> GetWithLogsAsync(int countryCode, int prefix, int number, int page, int pageSize);

        Task<(IEnumerable<BlackListEntry>, int)> GetBlackListByPhoneAsync(int countryCode, int prefix, int number, int page, int pageSize);

        Task<(IEnumerable<BlackListEntry>, int)> GetBlackListByCustomerNoAsync(ulong customerNo, int page, int pageSiz);

        Task<BlackListEntry> GetLastBlacklistRecord(int countryCode, int prefix, int number);
    }
}
