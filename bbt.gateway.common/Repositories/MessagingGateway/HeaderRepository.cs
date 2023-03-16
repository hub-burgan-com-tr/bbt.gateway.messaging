using bbt.gateway.common.Models;


namespace bbt.gateway.common.Repositories
{
    public class HeaderRepository : Repository<Header>, IHeaderRepository
    {
        public HeaderRepository(DatabaseContext context) : base(context)
        { 
        
        }

    }
}
