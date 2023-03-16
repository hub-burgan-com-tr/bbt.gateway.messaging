using bbt.gateway.common.Models;
using Microsoft.EntityFrameworkCore;

namespace bbt.gateway.common.Repositories
{
    public class OperatorRepository : Repository<Operator>, IOperatorRepository
    {
        public OperatorRepository(DatabaseContext context) : base(context)
        { 
        
        }

        public async Task<Operator> GetOperatorAsNoTracking(OperatorType type)
        {
            return await Context.Operators.AsNoTracking().FirstOrDefaultAsync(o => o.Type == type);
        }
    }
}
