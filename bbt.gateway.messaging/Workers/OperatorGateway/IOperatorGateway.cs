using bbt.gateway.common.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Workers.OperatorGateway
{
    public interface IOperatorGateway
    {
        Task<bool> SendOtp(Phone phone, string content, ConcurrentBag<OtpResponseLog> responses, Header header);
        Task<OtpResponseLog> SendOtp(Phone phone, string content, Header header);
        Task<OtpResponseLog> SendOtpForeign(Phone phone, string content, Header header);
        Task<OtpTrackingLog> CheckMessageStatus(CheckSmsRequest checkSmsRequest);

    }
}
