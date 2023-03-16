using bbt.gateway.common.Models;
using bbt.gateway.common.Repositories;
using bbt.gateway.messaging.Workers;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Controllers.v1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class Messaging : ControllerBase
    {
        private readonly OtpSender _otpSender;
        private readonly ITransactionManager _transactionManager;
        private readonly dEngageSender _dEngageSender;
        private readonly CodecSender _codecSender;
        private readonly IRepositoryManager _repositoryManager;
        public Messaging(OtpSender otpSender, ITransactionManager transactionManager, dEngageSender dEngageSender
            , IRepositoryManager repositoryManager, CodecSender codecSender)
        {
            _transactionManager = transactionManager;
            _otpSender = otpSender;
            _dEngageSender = dEngageSender;
            _codecSender = codecSender;
            _repositoryManager = repositoryManager;
        }

        [SwaggerOperation(
            Summary = "Send templated Sms message",
            Description = "Templates are defined in dEngage",
            Tags = new[] { "Sms" }
            )]
        [HttpPost("sms/templated")]
        [SwaggerResponse(200, "Sms was sent successfully", typeof(SendSmsResponse))]
        [SwaggerResponse(460, "Given template is not found on dEngage", typeof(void))]
        [SwaggerResponse(465, "Sim card is changed.", typeof(void))]
        [SwaggerResponse(466, "Operator is changed.", typeof(void))]
        public async Task<IActionResult> SendTemplatedSms([FromBody] SendTemplatedSmsRequest data)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Mock")
            {
                return Ok(new SendSmsResponse()
                {
                    Status = dEngageResponseCodes.Success,
                    TxnId = Guid.NewGuid(),
                });
            }
            if (data.Phone == null)
            {
                data.Phone = _transactionManager.CustomerRequestInfo.MainPhone;
            }
            return Ok(await _dEngageSender.SendTemplatedSms(data));
        }

        [SwaggerOperation(
           Summary = "Send Sms message",
           Description = "Send given content directly.",
           Tags = new[] { "Sms" }
           )]
        [HttpPost("sms/message")]
        [SwaggerResponse(200, "Sms was sent successfully", typeof(SendSmsResponse))]
        [SwaggerResponse(465, "Sim card is changed.", typeof(void))]
        [SwaggerResponse(466, "Operator is changed.", typeof(void))]
        [SwaggerResponse(460, "Has Blacklist Record.", typeof(void))]

        public async Task<IActionResult> SendMessageSms([FromBody] SendMessageSmsRequest data)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Mock")
            {
                return Ok(new SendSmsResponse()
                {
                    Status = dEngageResponseCodes.Success,
                    TxnId = Guid.NewGuid(),
                });
            }

            if (data.Phone == null)
            {
                data.Phone = _transactionManager.CustomerRequestInfo.MainPhone;
            }

            if (ModelState.IsValid)
            {
                if (data.ContentType == MessageContentType.Otp)
                {
                    return Ok(await _otpSender.SendMessage(data));
                }
                else
                {
                    var codecOperator = await _repositoryManager.Operators.GetOperatorAsNoTracking(OperatorType.Codec);
                    if (codecOperator.Status == OperatorStatus.Active)
                    {
                        return Ok(await _codecSender.SendSms(data));
                    }
                    else
                    {
                        return Ok(await _dEngageSender.SendSms(data));
                    }

                }
            }
            else
            {
                _transactionManager.LogError("Model State is Not Valid | " +
                    string.Join("|", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                return BadRequest(ModelState);
            }


        }

        [SwaggerOperation(
           Summary = "Check Sms Message Status",
           Description = "Check Transactional Sms Delivery Status."
           )]
        [HttpGet("sms/check")]

        public async Task<IActionResult> CheckSmsStatus(System.Guid TxnId)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Mock")
            {
                return Ok(new CheckSmsStatusResponse()
                {
                    code = 0,
                    message = "Delivered",
                    status = SmsStatus.Delivered
                });
            }

            CheckSmsStatusRequest request = new();
            request.TxnId = TxnId;
            return Ok(await _dEngageSender.CheckSms(request));
        }

        [SwaggerOperation(
           Summary = "Check Sms Message Status",
           Description = "Check Otp Sms Delivery Status."
           )]
        [HttpPost("sms/check-message")]
        [ApiExplorerSettings(IgnoreApi = true)]

        public async Task<IActionResult> CheckMessageStatus([FromBody] CheckSmsRequest data)
        {

            if (ModelState.IsValid)
            {
                var res = await _otpSender.CheckMessage(data);
                return Ok(res);
            }
            else
            {
                return BadRequest(ModelState);
            }


        }

        [SwaggerOperation(
           Summary = "Send templated Email message",
           Description = "Templates are defined in dEngage"
           )]
        [HttpPost("email/templated")]
        [SwaggerResponse(200, "Email was sent successfully", typeof(SendEmailResponse))]
        [SwaggerResponse(460, "Given template is not found on dEngage", typeof(void))]

        public async Task<IActionResult> SendTemplatedEmail([FromBody] SendTemplatedEmailRequest data)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Mock")
            {
                return Ok(new SendEmailResponse()
                {
                    Status = dEngageResponseCodes.Success,
                    TxnId = Guid.NewGuid(),
                });
            }

            if (data.Email == null)
            {
                data.Email = _transactionManager.CustomerRequestInfo.MainEmail;
            }
            var response = await _dEngageSender.SendTemplatedMail(data);
            return Ok(response);
        }


        [SwaggerOperation(
           Summary = "Send Email message",
           Description = "Send given content directly."
           )]
        [HttpPost("email/message")]
        [SwaggerResponse(200, "Email was sent successfully", typeof(SendEmailResponse))]

        public async Task<IActionResult> SendMessageEmail([FromBody] SendMessageEmailRequest data)
        {
            if (data.Email == null)
            {
                data.Email = _transactionManager.CustomerRequestInfo.MainEmail;
            }
            var response = await _dEngageSender.SendMail(data);
            return Ok(response);
        }

        //[SwaggerOperation(
        //   Summary = "Send Push Notification",
        //   Description = "Send push notification to device."
        //   )]
        //[HttpPost("push-notification")]
        //[SwaggerResponse(200, "Push notification was sent successfully", typeof(SendPushNotificationResponse))]
        //public async Task<IActionResult> SendPushNotification([FromBody] SendMessagePushNotificationRequest data)
        //{
        //    var response = await _dEngageSender.SendPushNotification(data);
        //    return Ok(response);
        //}

        [SwaggerOperation(
           Summary = "Send Templated Push Notification",
           Description = "Send templated push notification to device."
           )]
        [HttpPost("push-notification/templated")]
        [SwaggerResponse(200, "Push notification was sent successfully", typeof(SendPushNotificationResponse))]

        public async Task<IActionResult> SendTemplatedPushNotification([FromBody] SendTemplatedPushNotificationRequest data)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Mock")
            {
                return Ok(new SendPushNotificationResponse()
                {
                    Status = dEngageResponseCodes.Success,
                    TxnId = Guid.NewGuid(),
                });
            }

            var response = await _dEngageSender.SendTemplatedPushNotification(data);
            return Ok(response);
        }

    }
}
