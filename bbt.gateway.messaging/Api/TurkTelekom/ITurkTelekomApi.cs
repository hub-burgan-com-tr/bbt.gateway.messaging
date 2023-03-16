using bbt.gateway.messaging.Api.TurkTelekom.Model;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Api.TurkTelekom
{
    public interface ITurkTelekomApi:IBaseApi
    {
        public Task<OperatorApiResponse> SendSms(TurkTelekomSmsRequest turkTelekomSmsRequest);
        public Task<OperatorApiTrackingResponse> CheckSmsStatus(TurkTelekomSmsStatusRequest turkTelekomSmsStatusRequest);
    }
}
