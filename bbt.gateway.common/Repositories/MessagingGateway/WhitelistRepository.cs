using bbt.gateway.common.Models;

namespace bbt.gateway.common.Repositories
{
    public class WhitelistRepository : Repository<WhiteList>, IWhitelistRepository
    {
        public WhitelistRepository(DatabaseContext context) : base(context)
        { 
            
        }

    }
}
