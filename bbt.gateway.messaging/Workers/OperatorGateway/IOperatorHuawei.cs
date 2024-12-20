using bbt.gateway.common.Models;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Workers.OperatorGateway
{
    public interface IOperatorHuawei : IOperatorGatewayBase
    {
        public Task<PushNotificationResponseLog> SendPushNotificationAsync(
                                                                           string app, 
                                                                           string deviceToken, 
                                                                           string title, 
                                                                           string content, 
                                                                           string customParams, 
                                                                           string targetUrl = ""
                                                                          );
    }
}