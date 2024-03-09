using bbt.gateway.common.Models;
using System.Collections.Generic;

namespace bbt.gateway.common.Repositories
{
    public interface IPushNotificationRequestLogRepository : IRepository<PushNotificationRequestLog>
    {
        public Task<List<PushNotificationRequestLog>> GetPushNotifications(string customerId);
    }
}
