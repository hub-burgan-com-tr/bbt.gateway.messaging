using bbt.gateway.common.Models;
using System;
using System.Collections.Generic;

namespace bbt.gateway.common.Repositories
{
    public interface IPhoneConfigurationRepository : IRepository<PhoneConfiguration>
    {
        Task<IEnumerable<PhoneConfiguration>> GetWithRelatedLogsAndBlacklistEntriesAsync(int countryCode, int prefix, int number, int count);
        Task<PhoneConfiguration> GetWithBlacklistEntriesAsync(int countryCode, int prefix, int number,DateTime blackListValidDate);

        Task DeletePhoneConfiguration(Guid id);
    }
}
