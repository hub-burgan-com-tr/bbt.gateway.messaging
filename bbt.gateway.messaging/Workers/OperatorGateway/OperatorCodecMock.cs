using bbt.gateway.common.Models;
using bbt.gateway.messaging.Api.Codec.Model;
using bbt.gateway.messaging.Helpers;
using CodecFastApi;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Polly;
using System;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Workers.OperatorGateway
{
    public class OperatorCodecMock : OperatorGatewayBase,IOperatorCodec
    {

        private SoapSoapClient _codecClient;
        private readonly IFakeSmtpHelper _fakeSmtpHelper;
        public OperatorCodecMock(IConfiguration configuration,IFakeSmtpHelper fakeSmtpHelper,
            ITransactionManager transactionManager) : base(configuration, transactionManager)
        {
            _codecClient = new SoapSoapClient(SoapSoapClient.EndpointConfiguration.SoapSoap12);
            _fakeSmtpHelper = fakeSmtpHelper;
        }



        public async Task<CodecSmsStatusResponse> CheckSms(string refId)
        {
            await Task.CompletedTask;

            return new CodecSmsStatusResponse()
            {
                
                
            };

        }


        public async Task<SmsResponseLog> SendSms(Phone phone, string content)
        {
            var smsLog = new SmsResponseLog()
            {
                Operator = Type,
                Content = String.IsNullOrEmpty(content) ? "" : content.ClearMaskingFields(),
                CreatedAt = DateTime.Now,
                OperatorResponseCode = 0,
                OperatorResponseMessage = "Mock",
                StatusQueryId = "",
                Status = "Mock"
            };

            

            return smsLog;
        }

        private string GetSender()
        {
            return TransactionManager.CustomerRequestInfo.BusinessLine == "X" ? "On." : "BURGAN";
        }
    }
}
