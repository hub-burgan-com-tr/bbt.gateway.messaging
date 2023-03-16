using bbt.gateway.common.Models;
using System.Collections.Generic;

namespace bbt.gateway.common.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        public IEnumerable<User> GetWithDevices(string Username);
        
    }
}
