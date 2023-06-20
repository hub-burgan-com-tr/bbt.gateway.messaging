using bbt.gateway.common.Models;
using bbt.gateway.messaging.Api.Codec.Model;
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
    public class OperatorCodec : OperatorGatewayBase
    {

        private SoapSoapClient _codecClient;
        public OperatorCodec(IConfiguration configuration,
            ITransactionManager transactionManager) : base(configuration, transactionManager)
        {
            _codecClient = new SoapSoapClient(SoapSoapClient.EndpointConfiguration.SoapSoap12);
        }



        public async Task<CodecSmsStatusResponse> CheckSms(string refId)
        {
            var serializeSettings = new JsonSerializerSettings();
            serializeSettings.Converters.Add(new IsoDateTimeConverter() { DateTimeFormat = "ddMMyyHHmmss" });
            try
            {
                var res = await _codecClient.GetStatusAsync(OperatorConfig.User, OperatorConfig.Password, refId, 3, String.Empty);
                return JsonConvert.DeserializeObject<CodecSmsStatusResponse>(res, serializeSettings);
            }
            catch (Exception ex)
            {
                TransactionManager.LogError($"Couldn't get Codec Sms Status  | Exception : {ex}");
                return null;
            }

        }


        public async Task<SmsResponseLog> SendSms(Phone phone, string content)
        {
            var smsLog = new SmsResponseLog()
            {
                Operator = Type,
                Content = String.IsNullOrEmpty(content) ? "" : content.ClearMaskingFields(),
                CreatedAt = DateTime.Now,
            };

            try
            {
                await Policy.Handle<EndpointNotFoundException>().RetryAsync(5,
                  (e, r) =>
                  {
                      TransactionManager.LogError($"Codec Retry : {r}");
                  }).ExecuteAsync(async () =>
                  {
                      var response = await _codecClient.SendSmsAsync(OperatorConfig.User, OperatorConfig.Password, GetSender(),
                    phone.Concatenate(), content, string.Empty, false, Configuration["Api:Codec:HeaderCode"], 3, string.Empty, string.Empty,
                    "BIREYSEL", "BILGILENDIRME");

                      var parsedResponse = JsonConvert.DeserializeObject<CodecSmsResponse>(response);

                      smsLog.OperatorResponseCode = parsedResponse.ResultSet.Code;
                      smsLog.OperatorResponseMessage = parsedResponse.ResultSet.Description;
                      if (parsedResponse.ResultList != null && parsedResponse.ResultList.Count() > 0)
                      {
                          smsLog.StatusQueryId = parsedResponse.ResultList.FirstOrDefault()?.SmsRefId ?? String.Empty;
                      }
                      else
                      {
                          smsLog.StatusQueryId = String.Empty;
                      }
                      smsLog.Status = String.Empty;
                  });

            }
            catch (Exception ex)
            {
                TransactionManager.LogError($"Critical Error Occured at Codec Services | ErrorCode:499 | Exception : {ex}");
                smsLog.OperatorResponseCode = -99999;
                smsLog.OperatorResponseMessage = ex.ToString();
            }

            return smsLog;
        }

        private string GetSender()
        {
            return TransactionManager.CustomerRequestInfo.BusinessLine == "X" ? "On." : "BURGAN";
        }
    }
}
