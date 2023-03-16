using bbt.gateway.common.Models;

namespace bbt.gateway.common.Repositories
{
    public interface IOperatorRepository : IRepository<Operator>
    {
        Task<Operator> GetOperatorAsNoTracking(OperatorType type);
    }
}
