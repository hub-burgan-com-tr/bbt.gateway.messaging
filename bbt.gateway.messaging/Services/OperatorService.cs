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
            var operatorInfo = await _daprClient.GetStateAsync<Operator>(GlobalConstants.DAPR_STATE_STORE,GlobalConstants.OPERATOR_CACHE_PREFIX+type.ToString());
            if(operatorInfo is not {})
            {
                await using (var fileLock = await _daprClient.Lock(GlobalConstants.DAPR_LOCK_STORE, GlobalConstants.OPERATOR_LOCK_PREFIX+type,"MessagingGateway",10))
                {
                    if (fileLock.Success)
                    {
                        operatorInfo = await _repositoryManager.Operators.GetOperatorAsNoTracking(type);
                        await _daprClient.SaveStateAsync(GlobalConstants.DAPR_STATE_STORE, GlobalConstants.OPERATOR_CACHE_PREFIX+type.ToString(), operatorInfo,metadata: new Dictionary<string, string>() {
                            {
                                "ttlInSeconds", "3600"
                            }
                        });
                        var response = await _daprClient.Unlock(GlobalConstants.DAPR_LOCK_STORE, GlobalConstants.OPERATOR_LOCK_PREFIX+type,"MessagingGateway");
                    }
                    else
                    {
                        return await GetOperator(type);
                    }
                }
            }
            return operatorInfo;
        }

        public async Task RevokeCache(OperatorType type)
        {
            await _daprClient.DeleteStateAsync(GlobalConstants.DAPR_STATE_STORE,GlobalConstants.OPERATOR_CACHE_PREFIX+type.ToString());
        }
    }
}