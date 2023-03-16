using bbt.gateway.messaging.Api.Vodafone.Model;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Api.Vodafone
{
    public interface IVodafoneApi:IBaseApi
    {
        public Task<OperatorApiResponse> SendSms(VodafoneSmsRequest vodafoneSmsRequest);
        public Task<OperatorApiTrackingResponse> CheckSmsStatus(VodafoneSmsStatusRequest vodafoneSmsStatusRequest);
        public Task<OperatorApiAuthResponse> Auth(VodafoneAuthRequest vodafoneAuthRequest);
    }
}
