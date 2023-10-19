using bbt.gateway.common.Models;
using bbt.gateway.messaging.Api.Codec.Model;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Workers.OperatorGateway
{
    public interface IOperatorCodec: IOperatorGatewayBase
    {
        public Task<CodecSmsStatusResponse> CheckSms(string refId);
        public Task<SmsResponseLog> SendSms(Phone phone, string content);
    }
}
