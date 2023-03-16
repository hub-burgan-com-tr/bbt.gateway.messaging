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

    }
}
