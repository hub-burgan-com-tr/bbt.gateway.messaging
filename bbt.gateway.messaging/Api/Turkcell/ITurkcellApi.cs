using bbt.gateway.messaging.Api.Turkcell.Model;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Api.Turkcell
{
    public interface ITurkcellApi:IBaseApi
    {
        public Task<OperatorApiResponse> SendSms(TurkcellSmsRequest turkcellSmsRequest);
        public Task<OperatorApiAuthResponse> Auth(TurkcellAuthRequest turkcellAuthRequest);
        public Task<OperatorApiTrackingResponse> CheckSmsStatus(TurkcellSmsStatusRequest turkcellSmsStatusRequest);
    }
}
