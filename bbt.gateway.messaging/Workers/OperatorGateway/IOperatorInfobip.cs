using bbt.gateway.common.Models;
using bbt.gateway.messaging.Api.Infobip.Model;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Workers.OperatorGateway
{
    public interface IOperatorInfobip: IOperatorGatewayBase
    {
        public Task<InfobipApiSmsStatusResponse> CheckSms(string refId);
        public Task<(SmsResponseLog, OtpResponseLog)> SendSms(Phone phone, string content);
    }
}
