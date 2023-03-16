using bbt.gateway.common.Models;

namespace bbt.gateway.common.Repositories
{
    public class SmsRequestLogRepository : Repository<SmsRequestLog>, ISmsRequestLogRepository
    {
        public SmsRequestLogRepository(DatabaseContext context) : base(context)
        { 
        
        }

    }
}
