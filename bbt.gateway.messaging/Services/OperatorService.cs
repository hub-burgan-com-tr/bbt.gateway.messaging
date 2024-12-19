using bbt.gateway.common;
using bbt.gateway.common.GlobalConstants;
using bbt.gateway.common.Models;
using bbt.gateway.common.Repositories;
using Dapr.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Services
{
    public class OperatorService : IOperatorService
    {
        private readonly DaprClient _daprClient;
        private readonly IRepositoryManager _repositoryManager;
        private readonly IConfiguration _configuration;
        private DbContextOptions<DatabaseContext> _dbOptions;

        public OperatorService(DaprClient daprClient, IRepositoryManager repositoryManager, IConfiguration configuration)
        {
            _daprClient = daprClient;
            _repositoryManager = repositoryManager;
            _configuration = configuration;
            _dbOptions = new DbContextOptionsBuilder<DatabaseContext>()
                .UseSqlServer(_configuration.GetConnectionString("DefaultConnection"))
                .Options;
        }

        public async Task<Operator> GetOperator(OperatorType type)
        {
            var operators = await _daprClient.GetStateAsync<IEnumerable<Operator>>(GlobalConstants.DAPR_STATE_STORE, GlobalConstants.OPERATORS_CACHE_KEY);
           
            if (operators is not { })
            {
                using var databaseContext = new DatabaseContext(_dbOptions);

                operators = await databaseContext.Operators.AsNoTracking().ToListAsync();

                await _daprClient.SaveStateAsync(GlobalConstants.DAPR_STATE_STORE, GlobalConstants.OPERATORS_CACHE_KEY, operators, metadata: new Dictionary<string, string>() {
                            {
                                "ttlInSeconds", "290"
                            }
                        });
            }

            return operators.FirstOrDefault(o => o.Type.Equals(type));
        }

        public async Task RevokeCache()
        {
            await _daprClient.DeleteStateAsync(GlobalConstants.DAPR_STATE_STORE, GlobalConstants.OPERATORS_CACHE_KEY);
        }
    }
}