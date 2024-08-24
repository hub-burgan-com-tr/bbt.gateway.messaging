using Azure;
using bbt.gateway.common.Api.dEngage.Model.Transactional;
using bbt.gateway.common.Models;
using Google.Api;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

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
