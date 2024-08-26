using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using bbt.gateway.common.GlobalConstants;
using bbt.gateway.common.Models;
using bbt.gateway.common.Repositories;
using Dapr.Client;
using Elastic.Apm.Config;

namespace bbt.gateway.messaging.Services
{
    public class OperatorService : IOperatorService
    {
        private readonly DaprClient _daprClient;
        private readonly IRepositoryManager _repositoryManager;
        private readonly IConfiguration _configuration;

        public OperatorService(DaprClient daprClient, IRepositoryManager repositoryManager)
        {
            _daprClient = daprClient;
            _repositoryManager = repositoryManager;
        }

        public async Task<Operator> GetOperator(OperatorType type)
        {
            var operators = await _daprClient.GetStateAsync<IEnumerable<Operator>>(GlobalConstants.DAPR_STATE_STORE,GlobalConstants.OPERATORS_CACHE_KEY);
            if(operators is not {})
            {
                //await using (var fileLock = await _daprClient.Lock(GlobalConstants.DAPR_LOCK_STORE, GlobalConstants.OPERATORS_LOCK_KEY,"MessagingGateway",10))
                //{
                //    if (fileLock.Success)
                //    {
                        operators = await _repositoryManager.Operators.GetAllAsNoTrackingAsync();
                        await _daprClient.SaveStateAsync(GlobalConstants.DAPR_STATE_STORE, GlobalConstants.OPERATORS_CACHE_KEY, operators,metadata: new Dictionary<string, string>() {
                            {
                                "ttlInSeconds", "290"
                            }
                        });
                        var response = await _daprClient.Unlock(GlobalConstants.DAPR_LOCK_STORE, GlobalConstants.OPERATORS_LOCK_KEY,"MessagingGateway");
                //    }
                //    else
                //    {
                //        return await GetOperator(type);
                //    }
                //}
            }
            return operators.FirstOrDefault(o => o.Type.Equals(type));
        }

        public async Task RevokeCache()
        {
            await _daprClient.DeleteStateAsync(GlobalConstants.DAPR_STATE_STORE,GlobalConstants.OPERATORS_CACHE_KEY);
        }
    }
}