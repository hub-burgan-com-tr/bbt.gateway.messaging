using bbt.gateway.common.Models;
using System.Collections.Generic;

namespace bbt.gateway.common.Repositories
{
    public interface IDirectBlacklistRepository : IRepository<SmsDirectBlacklist>
    {
        Task<SmsDirectBlacklist> GetLastBlacklistEntry(string number);
    }
}
