using bbt.gateway.common.Models;


namespace bbt.gateway.common.Repositories
{
    public class OtpTrackingLogRepository : Repository<OtpTrackingLog>, IOtpTrackingLogRepository
    {
        public OtpTrackingLogRepository(DatabaseContext context) : base(context)
        { 
        
        }

    }
}
