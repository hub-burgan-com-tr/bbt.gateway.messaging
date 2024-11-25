using bbt.gateway.common.GlobalConstants;
using bbt.gateway.common.Models.v2;
using bbt.gateway.messaging.Helpers;
using bbt.gateway.messaging.Workers;
using Dapr;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Controllers.v2
{
    [ApiController]
    public class MessagingAsync : ControllerBase
    {
        private readonly ITransactionManager _transactionManager;
        private readonly SmsStringHelper _smsStringHelper;

        public MessagingAsync(ITransactionManager transactionManager, SmsStringHelper smsStringHelper)
        {
            _transactionManager = transactionManager;
            _smsStringHelper = smsStringHelper;
        }

        [Topic(GlobalConstants.DAPR_QUEUE_STORE, GlobalConstants.SMS_QUEUE_BULK_NAME, GlobalConstants.SMS_QUEUE_DEAD_LETTER_NAME, false)]
        [Topic(GlobalConstants.DAPR_QUEUE_STORE, GlobalConstants.SMS_QUEUE_FAST_NAME, GlobalConstants.SMS_QUEUE_DEAD_LETTER_NAME, false)]
        [Topic(GlobalConstants.DAPR_QUEUE_STORE, GlobalConstants.SMS_QUEUE_OTP_NAME, GlobalConstants.SMS_QUEUE_DEAD_LETTER_NAME, false)]
        [HttpPost("/sms/Messaging/stringAsyncSubscribe")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> SendMessageSmsString(SmsRequestString data)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Mock")
            {
                return Ok();
            }

            _transactionManager.StringSend = true;
            SmsRequest _data = new SmsRequest();
            _data.SmsType = data.SmsType;
            _data.Sender = data.Sender;
            _data.CustomerNo = data.CustomerNo;
            _data.Process = data.Process;
            _data.Content = data.Content;
            _data.Tags = data.Tags;
            _data.CitizenshipNo = data.CitizenshipNo;
            _data.Phone = _transactionManager.GetPhoneFromString(data.Phone);

            if (_data.Phone == null)
            {
                _data.Phone = new Phone
                {
                    CountryCode = _transactionManager.CustomerRequestInfo.MainPhone.CountryCode,
                    Prefix = _transactionManager.CustomerRequestInfo.MainPhone.Prefix,
                    Number = _transactionManager.CustomerRequestInfo.MainPhone.Number,
                };
            }

            var actionResult = await _smsStringHelper.ProcessSmsRequestAsync(_data);

            if ((actionResult as Microsoft.AspNetCore.Mvc.ObjectResult)?.StatusCode == 200)
            {
                return Ok();
            }

            return Problem();
        }
    }
}