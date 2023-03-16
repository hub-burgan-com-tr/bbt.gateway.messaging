using bbt.gateway.common.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace bbt.gateway.common.Repositories
{
    public class PushNotificationResponseLogRepository : Repository<PushNotificationResponseLog>, IPushNotificationResponseLogRepository
    {
        public PushNotificationResponseLogRepository(DatabaseContext context) : base(context)
        { 
        
        }

    }
}
