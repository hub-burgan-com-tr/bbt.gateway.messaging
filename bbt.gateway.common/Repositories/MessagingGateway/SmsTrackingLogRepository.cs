using bbt.gateway.common.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace bbt.gateway.common.Repositories
{
    public class SmsTrackingLogRepository : Repository<SmsTrackingLog>, ISmsTrackingLogRepository
    {
        public SmsTrackingLogRepository(DatabaseContext context) : base(context)
        { 
        
        }


    }
}
