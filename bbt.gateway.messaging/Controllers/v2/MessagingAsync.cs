using bbt.gateway.common.Api.MessagingGateway;
using bbt.gateway.common.GlobalConstants;
using bbt.gateway.common.Models.v2;
using bbt.gateway.common.Repositories;
using bbt.gateway.messaging.Workers;
using Dapr;
using Dapr.Client;
using Elastic.Apm.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Controllers.v2
{
    [ApiController]
    public class MessagingAsync : ControllerBase
    {
        private readonly OtpSender _otpSender;
        private readonly ITransactionManager _transactionManager;
        private readonly dEngageSender _dEngageSender;
        private readonly CodecSender _codecSender;
        private readonly InfobipSender _infobipSender;
        private readonly FirebaseSender _firebaseSender;
        private readonly IRepositoryManager _repositoryManager;
        private readonly ITracer _tracer;
        private readonly IConfiguration _configuration;
        private readonly IMessagingGatewayApi _messagingGatewayApi;
        private readonly DaprClient _daprClient;
        public MessagingAsync(OtpSender otpSender, ITransactionManager transactionManager, dEngageSender dEngageSender, FirebaseSender firebaseSender
            , IRepositoryManager repositoryManager, CodecSender codecSender, IConfiguration configuration, IMessagingGatewayApi messagingGatewayApi
            , InfobipSender infobipSender, DaprClient daprClient)
        {
            _transactionManager = transactionManager;
            _otpSender = otpSender;
            _dEngageSender = dEngageSender;
            _codecSender = codecSender;
            _infobipSender = infobipSender;
            _repositoryManager = repositoryManager;
            _configuration = configuration;
            _messagingGatewayApi = messagingGatewayApi;
            _firebaseSender = firebaseSender;
            _tracer = Elastic.Apm.Agent.Tracer;
            _daprClient = daprClient;
        }

        [Topic(GlobalConstants.DAPR_QUEUE_STORE, GlobalConstants.SMS_QUEUE_BULK_NAME)]
        [Topic(GlobalConstants.DAPR_QUEUE_STORE, GlobalConstants.SMS_QUEUE_FAST_NAME)]
        [Topic(GlobalConstants.DAPR_QUEUE_STORE, GlobalConstants.SMS_QUEUE_OTP_NAME)]
        [HttpPost("sms/message/stringAsyncSubscribe")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> SendMessageSmsString(SmsRequestString data)
        {
            try
            {
                Messaging messaging = new Messaging(_otpSender, _transactionManager, _dEngageSender, _firebaseSender, _repositoryManager,
                                                       _codecSender, _configuration, _messagingGatewayApi, _infobipSender, _daprClient);

                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Mock")
                {
                    return Ok(new SmsResponse()
                    {
                        Status = dEngageResponseCodes.Success,
                        TxnId = Guid.NewGuid(),
                    });
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

                var result = await messaging.ProcessSmsRequestAsync(_data);

                return Ok(_data);
            }
            catch (Exception ex)
            {
                _transactionManager.LogError("stringAsyncSubscribe ex:" + ex.ToString() 
                    + " "  + Newtonsoft.Json.JsonConvert.SerializeObject(data));

                return Ok(data);
            }
        }
    }
}