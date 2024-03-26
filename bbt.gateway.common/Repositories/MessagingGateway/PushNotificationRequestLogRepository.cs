using bbt.gateway.common.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace bbt.gateway.common.Repositories
{
    public class PushNotificationRequestLogRepository : Repository<PushNotificationRequestLog>, IPushNotificationRequestLogRepository
    {
        public PushNotificationRequestLogRepository(DatabaseContext context) : base(context)
        { 
        
        }
        public async Task<List<PushNotificationRequestLog>> GetPushNotifications(string customerId)
        {
            var notifications = await Context.PushNotificationRequestLogs.AsNoTracking().Where(t => t.ContactId == customerId && t.CreatedAt >= DateTime.Now.AddDays(-30) && t.SaveInbox && !t.IsDeleted && t.ResponseLogs.Any(r => r.ResponseCode.Equals("0")))
                .ToListAsync();

            return notifications;

        }

    }
}
