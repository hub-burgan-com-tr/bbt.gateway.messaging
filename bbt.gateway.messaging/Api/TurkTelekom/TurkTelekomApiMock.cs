using bbt.gateway.messaging.Api.TurkTelekom.Model;
using bbt.gateway.common.Models;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using bbt.gateway.messaging.Workers;
using System;
using bbt.gateway.messaging.Helpers;

namespace bbt.gateway.messaging.Api.TurkTelekom
{
    public class TurkTelekomApiMock:BaseApi,ITurkTelekomApi
    {
        private IFakeSmtpHelper _fakeSmtpHelper;
        public TurkTelekomApiMock(IFakeSmtpHelper fakeSmtpHelper,ITransactionManager transactionManager):base(transactionManager) {

            _fakeSmtpHelper = fakeSmtpHelper;
            Type = OperatorType.TurkTelekom;
        }

        public async Task<OperatorApiResponse> SendSms(TurkTelekomSmsRequest turkTelekomSmsRequest) 
        {
            await Task.CompletedTask;
            OperatorApiResponse operatorApiResponse = new() { OperatorType = this.Type };
            
            operatorApiResponse.ResponseCode = "0";
            operatorApiResponse.ResponseMessage = "";
            operatorApiResponse.MessageId = Guid.NewGuid().ToString();
            operatorApiResponse.RequestBody = turkTelekomSmsRequest.SerializeXml();
            operatorApiResponse.ResponseBody = "<Mock>Successfull</Mock>";

            _fakeSmtpHelper.SendFakeMail("TurkTelekom@Otp", "Turk Telekom", turkTelekomSmsRequest.GsmNo+"@maildev", "Turk Telekom Otp Sms", turkTelekomSmsRequest.Message, null);

            return operatorApiResponse;
        }

        public async Task<OperatorApiTrackingResponse> CheckSmsStatus(TurkTelekomSmsStatusRequest turkTelekomSmsStatusRequest)
        {
            await Task.CompletedTask;
            OperatorApiTrackingResponse operatorApiTrackingResponse = new() { OperatorType = this.Type };
           
            var requestBody = turkTelekomSmsStatusRequest.SerializeXml();
            var httpRequest = new StringContent(requestBody, Encoding.UTF8, "text/xml");
                
            operatorApiTrackingResponse.ResponseCode = "0";
            operatorApiTrackingResponse.ResponseMessage = "";
            operatorApiTrackingResponse.ResponseBody = "<Mock>Successfull</Mock>";
                

            return operatorApiTrackingResponse;
        }
    }
}
