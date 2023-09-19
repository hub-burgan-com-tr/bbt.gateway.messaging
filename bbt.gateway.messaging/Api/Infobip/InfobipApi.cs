using bbt.gateway.common.Models;
using bbt.gateway.messaging.Api.Infobip.Model;
using bbt.gateway.messaging.Api.Infobip.Model.SendSms;
using bbt.gateway.messaging.Api.Infobip.Model.SmsStatus;
using bbt.gateway.messaging.Workers;
using Newtonsoft.Json;
using Polly;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Api.Infobip
{
    public class InfobipApi : BaseApi,IInfobipApi
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public InfobipApi(ITransactionManager transactionManager, IHttpClientFactory httpClientFactory) : base(transactionManager)
        {
            Type = OperatorType.Infobip;
            _httpClientFactory = httpClientFactory;
        }
        public Task<InfobipApiSmsStatusResponse> CheckSmsStatus(InfobipSmsStatusRequest infobipSmsStatusRequest)
        {
            throw new System.NotImplementedException();
        }

        public async Task<InfobipApiSmsResponse> SendSms(InfobipSmsRequest infobipSmsRequest)
        {
            InfobipApiSmsResponse infobipApiSmsResponse = new InfobipApiSmsResponse();
            try
            {
                using var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("App", OperatorConfig.AuthToken);
                var httpRequest = new StringContent(JsonConvert.SerializeObject(infobipSmsRequest), Encoding.UTF8, "application/json");

                HttpResponseMessage httpResponse;
                await Policy.Handle<HttpRequestException>().RetryAsync(5,
                (e, r) =>
                {
                    TransactionManager.LogError($"Infobip Retry : {r}");
                    if(r == 5)
                    {
                        TransactionManager.LogError($"Critical Error Occured at Infobip Services | ErrorCode:499");
                        infobipApiSmsResponse.IsSuccess = false;
                        infobipApiSmsResponse.Message = "Couldn't Access To Infobip";
                        infobipApiSmsResponse.MsgId = "";
                        infobipApiSmsResponse.RequestBody = JsonConvert.SerializeObject(infobipSmsRequest);
                    }
                }).ExecuteAsync(async () =>
                {
                    httpResponse = await client.PostAsync(OperatorConfig.SendService, httpRequest);
                    if (httpResponse.IsSuccessStatusCode)
                    {
                        var responseText = await httpResponse.Content.ReadAsStringAsync();
                        var response = JsonConvert.DeserializeObject<InfobipSmsResponse>(responseText);
                        var msgId = response.messages.FirstOrDefault().messageId;
                        infobipApiSmsResponse.IsSuccess = true;
                        infobipApiSmsResponse.Message = "";
                        infobipApiSmsResponse.MsgId = msgId;
                        infobipApiSmsResponse.RequestBody = JsonConvert.SerializeObject(infobipSmsRequest);
                        infobipApiSmsResponse.ResponseBody = responseText;
                    }
                    else
                    {
                        var errorResponse = await httpResponse.Content.ReadAsStringAsync();
                        TransactionManager.LogError($"Critical Error Occured at Infobip Services | ErrorCode:498 | " + errorResponse);
                        var response = JsonConvert.DeserializeObject<InfobipErrorResponse>(errorResponse);
                        var errorMessage = response.requestError.serviceException.text;
                        infobipApiSmsResponse.IsSuccess = false;
                        infobipApiSmsResponse.Message = errorMessage;
                        infobipApiSmsResponse.MsgId = "";
                        infobipApiSmsResponse.RequestBody = JsonConvert.SerializeObject(infobipSmsRequest);
                        infobipApiSmsResponse.ResponseBody = errorResponse;
                    }
                });

                
            }
            catch (System.Exception ex)
            {
                TransactionManager.LogError($"Critical Error Occured at Infobip Services | ErrorCode:499 | ex : "+ex.ToString());
                infobipApiSmsResponse.IsSuccess = false;
                infobipApiSmsResponse.Message = ex.ToString();
                infobipApiSmsResponse.MsgId = "";
                infobipApiSmsResponse.RequestBody = JsonConvert.SerializeObject(infobipSmsRequest);
            }
            return infobipApiSmsResponse;
        }

    }
}
