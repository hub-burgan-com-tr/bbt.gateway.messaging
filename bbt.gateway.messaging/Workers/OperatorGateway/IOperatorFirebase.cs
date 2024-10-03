using bbt.gateway.common.Models;
using bbt.gateway.messaging.Api.Codec.Model;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Workers.OperatorGateway
{
    public interface IOperatorFirebase: IOperatorGatewayBase
    {
        public Task CheckPushNotificationAsync();
        public Task<PushNotificationResponseLog> SendPushNotificationAsync(string deviceToken, string title, string content, string customParams);
    }
}
