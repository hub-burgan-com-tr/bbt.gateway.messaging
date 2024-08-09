using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapr.Client;

namespace bbt.gateway.common.Helpers
{
    public class KafkaHelper(DaprClient daprClient)
    {
        private readonly DaprClient _daprClient = daprClient;

        public async Task SendToQueue<T>(T model,string topicName)
        {
            await _daprClient.PublishEventAsync<T>(GlobalConstants.GlobalConstants.DAPR_QUEUE_STORE,topicName,model);
        }

    }
}