using bbt.gateway.messaging.Api.Turkcell.Model;
using bbt.gateway.common.Models;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using bbt.gateway.messaging.Workers;
using bbt.gateway.messaging.Helpers;

namespace bbt.gateway.messaging.Api.Turkcell
{
    public class TurkcellApiMock:BaseApi,ITurkcellApi
    {
        private IFakeSmtpHelper _fakeSmtpHelper;
        public TurkcellApiMock(IFakeSmtpHelper fakeSmtpHelper,ITransactionManager transactionManager):base(transactionManager) {
            _fakeSmtpHelper = fakeSmtpHelper;
            Type = OperatorType.Turkcell;
        }

        public async Task<OperatorApiResponse> SendSms(TurkcellSmsRequest turkcellSmsRequest) {
            await Task.CompletedTask;
            OperatorApiResponse turkcellSmsResponse = new() { OperatorType = this.Type };
            var requests = getSendSmsXml(turkcellSmsRequest);
            string response = "<Mock>Successfull</Mock>";
            
            turkcellSmsResponse.ResponseCode = "0";
            turkcellSmsResponse.ResponseMessage = "";
            turkcellSmsResponse.MessageId = Guid.NewGuid().ToString();
            turkcellSmsResponse.RequestBody = requests.Item2;
            turkcellSmsResponse.ResponseBody = response;

            _fakeSmtpHelper.SendFakeMail("Turkcell@Otp", "Turkcell",turkcellSmsRequest.PhoneNo+"@maildev", "Turkcell Otp Sms", turkcellSmsRequest.Content, null);

            return turkcellSmsResponse;
        }

        public async Task<OperatorApiAuthResponse> Auth(TurkcellAuthRequest turkcellAuthRequest)
        {
            await Task.CompletedTask;
            OperatorApiAuthResponse turkcellAuthResponse = new();
            
            turkcellAuthResponse.ResponseCode = "0";
            turkcellAuthResponse.ResponseMessage = "";
            turkcellAuthResponse.AuthToken = Guid.NewGuid().ToString();                        
                

            return turkcellAuthResponse;  
        }

        public async Task<OperatorApiTrackingResponse> CheckSmsStatus(TurkcellSmsStatusRequest turkcellSmsStatusRequest)
        {
            await Task.CompletedTask;
            OperatorApiTrackingResponse turkcellSmsStatusResponse = new() { OperatorType = this.Type };
            
            turkcellSmsStatusResponse.ResponseCode = "0";
            turkcellSmsStatusResponse.ResponseMessage = "";
            turkcellSmsStatusResponse.ResponseBody = "<mock>Response</mock>";

            return turkcellSmsStatusResponse;
        }

        private string getAuthXml(TurkcellAuthRequest turkcellAuthRequest)
        {
            string xml = $"<soapenv:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:reg=\"http://www.turkcell.com.tr/sms/webservices/register\">"
            + "<soapenv:Header/>"
            + "<soapenv:Body>"
            + "<reg:register soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">"
            + "<string xsi:type=\"xsd:string\"><![CDATA[<?xml version=\"1.0\" encoding=\"UTF-8\"?>"
            + "<REGISTER>"
            + "<VERSION>1.0</VERSION>"
            + "<REG>"
            + "<USER>"+turkcellAuthRequest.User+"</USER>"
            + "<PASSWORD>"+turkcellAuthRequest.Password+"</PASSWORD>"
            + "</REG>"
            + "</REGISTER>]]>"
            + "</string>"
            + "</reg:register>"
            + "</soapenv:Body>"
            + "</soapenv:Envelope>";

            return xml;
        }

        private (string,string) getSendSmsXml(TurkcellSmsRequest turkcellSmsRequest)
        {
            string xml = "<soapenv:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:sen=\"http://www.turkcell.com.tr/sms/webservices/sendsms\">"
            + "<soapenv:Header/>"
            + "<soapenv:Body>"
            + "<sen:sendSMS soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">"
            + "<string xsi:type=\"xsd:string\">"
            + "<![CDATA[<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>"
            + "<SENDSMS>"
            + "<VERSION>1.0</VERSION>"
            + "<SESSION_ID>"+turkcellSmsRequest.SessionId+"</SESSION_ID>"
            + "<MSG_CODE>"+turkcellSmsRequest.MsgCode+"</MSG_CODE>"
            + "<VARIANT_ID>"+turkcellSmsRequest.VariantId+"</VARIANT_ID>"
            + "<VP>3</VP>"
            + "<SRC_MSISDN>"+(turkcellSmsRequest.IsAbroad ? turkcellSmsRequest.Header : "")+ "</SRC_MSISDN>"
            + "<SENDER>"+ (turkcellSmsRequest.IsAbroad ? "" : turkcellSmsRequest.Header) + "</SENDER>"
            + "<NOTIFICATION>T</NOTIFICATION>"
            + "<COMMERCIAL>N</COMMERCIAL>"
            + "<BRAND_CODE></BRAND_CODE>"
            + "<RECIPIENT_TYPE>BIREYSEL</RECIPIENT_TYPE>"
            + "<TM_LIST>"
            + "<TM>"
            + "<TRUSTED_DATE_LIST>"
            + "<TRUSTED_DATE>"+turkcellSmsRequest.TrustedDate+"</TRUSTED_DATE>"
            + "<TRUSTED_DATE_ALT>"+ turkcellSmsRequest.TrustedDate + "</TRUSTED_DATE_ALT>"
            + "</TRUSTED_DATE_LIST>"
            + "<DST_MSISDN_LIST>"
            + "<DST_MSISDN>"+ turkcellSmsRequest.PhoneNo+ "</DST_MSISDN>"
            + "</DST_MSISDN_LIST>"
            + "<CONTENT_LIST>"
            + "<CONTENT>"
            + "<CONTENT_TEXT>"+ turkcellSmsRequest.Content+ "</CONTENT_TEXT>"
            + "</CONTENT>"
            + "</CONTENT_LIST>"
            + "</TM>"
            + "</TM_LIST>"
            + "</SENDSMS>"
            + "]]>"
            + "</string>"
            + "</sen:sendSMS>"
            + "</soapenv:Body>"
            + "</soapenv:Envelope>";

            string maskedXml = "<soapenv:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:sen=\"http://www.turkcell.com.tr/sms/webservices/sendsms\">"
            + "<soapenv:Header/>"
            + "<soapenv:Body>"
            + "<sen:sendSMS soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">"
            + "<string xsi:type=\"xsd:string\">"
            + "<![CDATA[<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>"
            + "<SENDSMS>"
            + "<VERSION>1.0</VERSION>"
            + "<SESSION_ID>XXXX</SESSION_ID>"
            + "<MSG_CODE>XXXX</MSG_CODE>"
            + "<VARIANT_ID>XXXXX</VARIANT_ID>"
            + "<VP></VP>"
            + "<SRC_MSISDN>" + (turkcellSmsRequest.IsAbroad ? turkcellSmsRequest.Header : "") + "</SRC_MSISDN>"
            + "<SENDER>" + (turkcellSmsRequest.IsAbroad ? "" : turkcellSmsRequest.Header) + "</SENDER>"
            + "<NOTIFICATION>T</NOTIFICATION>"
            + "<COMMERCIAL>N</COMMERCIAL>"
            + "<BRAND_CODE></BRAND_CODE>"
            + "<RECIPIENT_TYPE>BIREYSEL</RECIPIENT_TYPE>"
            + "<TM_LIST>"
            + "<TM>"
            + "<TRUSTED_DATE_LIST>"
            + "<TRUSTED_DATE>" + turkcellSmsRequest.TrustedDate + "</TRUSTED_DATE>"
            + "<TRUSTED_DATE_ALT>" + turkcellSmsRequest.TrustedDate + "</TRUSTED_DATE_ALT>"
            + "</TRUSTED_DATE_LIST>"
            + "<DST_MSISDN_LIST>"
            + "<DST_MSISDN>" + turkcellSmsRequest.PhoneNo + "</DST_MSISDN>"
            + "</DST_MSISDN_LIST>"
            + "<CONTENT_LIST>"
            + "<CONTENT>"
            + "<CONTENT_TEXT>" + turkcellSmsRequest.Content.MaskOtpContent() + "</CONTENT_TEXT>"
            + "</CONTENT>"
            + "</CONTENT_LIST>"
            + "</TM>"
            + "</TM_LIST>"
            + "</SENDSMS>"
            + "]]>"
            + "</string>"
            + "</sen:sendSMS>"
            + "</soapenv:Body>"
            + "</soapenv:Envelope>";
            return (xml,maskedXml);
        }

        private string getSmsStatusXml(TurkcellSmsStatusRequest turkcellSmsStatusRequest)
        {
            string xml = "<soapenv:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:get=\"http://www.turkcell.com.tr/sms/webservices/getstatus\">"
            + "<soapenv:Header/>"
            + "<soapenv:Body>"
            + "<get:getStatus soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">"
            + "<string xsi:type=\"xsd:string\"><![CDATA[<GETSTATUS>"
            + "<VERSION>1.0</VERSION>"
            + "<SESSION_ID>"+turkcellSmsStatusRequest.AuthToken+"</SESSION_ID>"
            + "<MSGID_LIST>"
            + "<MSGID>"+turkcellSmsStatusRequest.MsgId+"</MSGID>"
            + "</MSGID_LIST>"
            + "</GETSTATUS>"
            + "]]>"
            + "</string>"
            + "</get:getStatus>"
            + "</soapenv:Body>"
            + "</soapenv:Envelope>";            

            return xml;
        }

    }
}
