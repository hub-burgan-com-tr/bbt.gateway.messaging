using bbt.gateway.messaging.Api.Turkcell.Model;
using bbt.gateway.common.Models;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System;
using bbt.gateway.messaging.Workers;
using Newtonsoft.Json;

namespace bbt.gateway.messaging.Api.Turkcell
{
    public class TurkcellApi:BaseApi,ITurkcellApi
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public TurkcellApi(ITransactionManager transactionManager, IHttpClientFactory httpClientFactory) :base(transactionManager) {
            Type = OperatorType.Turkcell;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<OperatorApiResponse> SendSms(TurkcellSmsRequest turkcellSmsRequest) {
            OperatorApiResponse turkcellSmsResponse = new() { OperatorType = this.Type};
            var requests = getSendSmsXml(turkcellSmsRequest);
            string response = "";
            try
            {
                HttpContent httpRequest = new StringContent(requests.Item1, Encoding.UTF8, "text/xml");

                using var httpClient = _httpClientFactory.CreateClient("default");
                var httpResponse = await httpClient.PostAsync(OperatorConfig.SendService, httpRequest);
                response = httpResponse.Content.ReadAsStringAsync().Result;
                
                
                if (httpResponse.IsSuccessStatusCode)
                {
                    var parsedXml = response.DeserializeXml<Model.SendSms.SuccessXml.Envelope>();
                    var textResponse = parsedXml.Body.sendSMSResponse.result;
                    
                    if (textResponse.Contains("NOK"))
                    {
                        turkcellSmsResponse.ResponseCode = textResponse.Split(",")[1];
                        turkcellSmsResponse.ResponseMessage = textResponse.Split(",")[2];
                        turkcellSmsResponse.MessageId = "";
                        turkcellSmsResponse.RequestBody = requests.Item2;
                        turkcellSmsResponse.ResponseBody = response;
                        TransactionManager.LogCritical("Turkcell Otp Failed | " + JsonConvert.SerializeObject(turkcellSmsResponse));
                    }
                    else
                    {
                        textResponse = textResponse.Replace("&lt;", "<");
                        textResponse = textResponse.Replace("&gt;", ">");
                        
                        var parsedResponse = textResponse.DeserializeXml<Model.SendSms.BodyXml.MSGIDRETURN>();
                        turkcellSmsResponse.ResponseCode = parsedResponse.MSGID_LIST.MSGID > 0 ? "0" : parsedResponse.MSGID_LIST.MSGID.ToString();
                        turkcellSmsResponse.ResponseMessage = "";
                        turkcellSmsResponse.MessageId = parsedResponse.MSGID_LIST.MSGID > 0 ? parsedResponse.MSGID_LIST.MSGID.ToString() : "";
                        turkcellSmsResponse.RequestBody = requests.Item2;
                        turkcellSmsResponse.ResponseBody = response;

                    }

                }
                else
                {
                    var parsedXml = response.DeserializeXml<Model.SendSms.ErrorXml.Envelope>();
                    turkcellSmsResponse.ResponseCode = parsedXml.Body.Fault.faultcode;
                    turkcellSmsResponse.ResponseMessage = parsedXml.Body.Fault.faultstring;
                    turkcellSmsResponse.MessageId = "";
                    turkcellSmsResponse.RequestBody = requests.Item2;
                    turkcellSmsResponse.ResponseBody = response;
                    TransactionManager.LogCritical("Turkcell Otp Failed | " + JsonConvert.SerializeObject(turkcellSmsResponse));
                }
            }
            catch (Exception ex)
            {
                turkcellSmsResponse.ResponseCode = "-99999";
                turkcellSmsResponse.ResponseMessage = ex.ToString();
                turkcellSmsResponse.MessageId = "";
                turkcellSmsResponse.RequestBody = requests.Item2;
                turkcellSmsResponse.ResponseBody = response;
                TransactionManager.LogCritical("Turkcell Otp Failed | "+JsonConvert.SerializeObject(turkcellSmsResponse));
            }

            return turkcellSmsResponse;
        }

        public async Task<OperatorApiAuthResponse> Auth(TurkcellAuthRequest turkcellAuthRequest)
        {
            OperatorApiAuthResponse turkcellAuthResponse = new();
            try
            {
                HttpContent httpRequest = new StringContent(getAuthXml(turkcellAuthRequest), Encoding.UTF8, "text/xml");
                using var httpClient = _httpClientFactory.CreateClient("default");
                var httpResponse = await httpClient.PostAsync(OperatorConfig.AuthanticationService, httpRequest);
                var response = httpResponse.Content.ReadAsStringAsync().Result;
                if (httpResponse.IsSuccessStatusCode)
                {
                    var parsedXml = response.DeserializeXml<Model.Auth.SuccessXml.Envelope>();
                    if (parsedXml.Body.registerResponse.result.Contains("NOK"))
                    {
                        turkcellAuthResponse.ResponseCode = parsedXml.Body.registerResponse.result.Split(",")[1];
                        turkcellAuthResponse.ResponseMessage = parsedXml.Body.registerResponse.result.Split(",")[2];
                        turkcellAuthResponse.AuthToken = "";
                    }
                    else
                    {
                        turkcellAuthResponse.ResponseCode = "0";
                        turkcellAuthResponse.ResponseMessage = "";
                        turkcellAuthResponse.AuthToken = parsedXml.Body.registerResponse.result;                        
                    }
                }
                else
                {
                    var parsedXml = response.DeserializeXml<Model.Auth.ErrorXml.Envelope>();
                    turkcellAuthResponse.ResponseCode = "-99999";
                    turkcellAuthResponse.ResponseMessage = parsedXml.Body.Fault.faultstring;
                    turkcellAuthResponse.AuthToken = "";
                }
            }
            catch (System.Exception ex)
            {
                turkcellAuthResponse.ResponseCode = "-99999";
                turkcellAuthResponse.ResponseMessage = ex.ToString();
                turkcellAuthResponse.AuthToken = "";
            }

            return turkcellAuthResponse;  
        }

        public async Task<OperatorApiTrackingResponse> CheckSmsStatus(TurkcellSmsStatusRequest turkcellSmsStatusRequest)
        {
            OperatorApiTrackingResponse turkcellSmsStatusResponse = new() { OperatorType = this.Type };
            string response = "";
            try
            {
                HttpContent httpRequest = new StringContent(getSmsStatusXml(turkcellSmsStatusRequest), Encoding.UTF8, "text/xml");
                using var httpClient = _httpClientFactory.CreateClient("default");
                var httpResponse = await httpClient.PostAsync(OperatorConfig.QueryService, httpRequest);
                response = httpResponse.Content.ReadAsStringAsync().Result;

                if (httpResponse.IsSuccessStatusCode)
                {
                    
                    var parsedXml = response.DeserializeXml<Model.SmsStatus.SuccessXml.Envelope>();
                    var textResponse = parsedXml.Body.getStatusResponse.@return;
                    if (textResponse.Contains("NOK"))
                    {
                        turkcellSmsStatusResponse.ResponseCode = textResponse.Split(",")[1];
                        turkcellSmsStatusResponse.ResponseMessage = textResponse.Split(",")[2];
                        turkcellSmsStatusResponse.ResponseBody = response;
                    }
                    else
                    {
                        textResponse = textResponse.Replace("&lt;", "<");
                        textResponse = textResponse.Replace("&gt;", ">");
                        var parsedResponse = textResponse.DeserializeXml<Model.SmsStatus.BodyXml.STATUSRETURN>();
                        turkcellSmsStatusResponse.ResponseCode = parsedResponse.STATUS_LIST.STATUS.MSGSTAT.ToString();
                        turkcellSmsStatusResponse.ResponseMessage = parsedResponse.STATUS_LIST.STATUS.REASON.ToString();
                        turkcellSmsStatusResponse.ResponseBody = response;
                    }

                }
                else
                {
                    var parsedXml = response.DeserializeXml<Model.SmsStatus.ErrorXml.Envelope>();
                    turkcellSmsStatusResponse.ResponseCode = parsedXml.Body.Fault.faultcode;
                    turkcellSmsStatusResponse.ResponseMessage = parsedXml.Body.Fault.faultstring;
                    turkcellSmsStatusResponse.ResponseBody = response;
                }
            }
            catch (HttpRequestException ex)
            {
                TransactionManager.LogError($"Critical Error Occured at Turkcell Otp Services | Network Related | ErrorCode:498 | Exception : {ex.ToString()}");
                turkcellSmsStatusResponse.ResponseCode = "-99999";
                turkcellSmsStatusResponse.ResponseMessage = ex.ToString();
                turkcellSmsStatusResponse.ResponseBody = "";
            }
            catch (Exception ex)
            {
                TransactionManager.LogError($"Critical Error Occured at Turkcell Otp Services | ErrorCode:498 | Exception : {ex.ToString()}");
                turkcellSmsStatusResponse.ResponseCode = "-99999";
                turkcellSmsStatusResponse.ResponseMessage = ex.ToString();
                turkcellSmsStatusResponse.ResponseBody = response;
            }

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
            + "<CONTENT_TEXT>"+turkcellSmsRequest.Content.Trim()+"</CONTENT_TEXT>"
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
            + "<CONTENT_TEXT>"+turkcellSmsRequest.Content.Trim().MaskOtpContent()+"</CONTENT_TEXT>"
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
