using bbt.gateway.common.Models;
using Refit;

namespace bbt.gateway.common.Api.MessagingGateway
{
    public interface IMessagingGatewayApi
    {
        [Post("/api/v2/Messaging/email/templated")]
        Task<Models.v2.TemplatedMailResponse> SendTemplatedMailMultiple(Models.v2.TemplatedMailRequest request);
        [Post("/api/v2/Administration/otp/check-message")]
        Task<OtpTrackingLog> CheckOtpStatus(CheckSmsRequest request);
        [Post("/api/v2/Administration/sms/check-message")]
        Task<SmsTrackingLog> CheckSmsStatus(common.Models.v2.CheckFastSmsRequest request);
        [Post("/api/v2/Administration/email/check-message")]
        Task<MailTrackingLog> CheckMailStatus(common.Models.v2.CheckMailStatusRequest request);
        [Post("/api/v1/Administration/templates/sms")]
        Task SetSmsTemplates();
        [Post("/api/v1/Administration/templates/mail")]
        Task SetMailTemplates();
        [Post("/api/v1/Administration/templates/push")]
        Task SetPushTemplates();
    }
}
