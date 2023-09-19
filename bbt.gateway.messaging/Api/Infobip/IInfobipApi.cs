using bbt.gateway.messaging.Api.Infobip.Model;
using bbt.gateway.messaging.Api.Infobip.Model.SendSms;
using bbt.gateway.messaging.Api.Infobip.Model.SmsStatus;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Api.Infobip
{
    public interface IInfobipApi : IBaseApi
    {
        public Task<InfobipApiSmsResponse> SendSms(InfobipSmsRequest infobipSmsRequest);
        public Task<InfobipApiSmsStatusResponse> CheckSmsStatus(InfobipSmsStatusRequest infobipSmsStatusRequest);
    }
}
