using bbt.gateway.common.Models;
using bbt.gateway.messaging.Api;
using bbt.gateway.common.Api.dEngage;
using bbt.gateway.common.Api.dEngage.Model.Contents;
using bbt.gateway.common.Api.dEngage.Model.Login;
using bbt.gateway.common.Api.dEngage.Model.Transactional;
using bbt.gateway.messaging.Helpers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Refit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Workers.OperatorGateway
{
    public class OperatordEngageMock : OperatorGatewayBase, IOperatordEngage
    {
        private readonly IdEngageClient _dEngageClient;
        private readonly IFakeSmtpHelper _fakeSmtpHelper;
        private readonly ITransactionManager _transactionManager;

        private string _authToken;
        public OperatordEngageMock(IdEngageClient dEngageClient,IFakeSmtpHelper fakeSmtpHelper,
            ITransactionManager transactionManager,IConfiguration configuration):base(configuration,transactionManager)
        {
            _dEngageClient = dEngageClient;
            _fakeSmtpHelper = fakeSmtpHelper;
            _transactionManager = transactionManager;
        }

        private async Task<OperatorApiAuthResponse> Auth()
        {
            TransactionManager.SmsRequestInfo.Operator = Type;

            OperatorApiAuthResponse authResponse = new();
            if (string.IsNullOrEmpty(OperatorConfig.AuthToken) || OperatorConfig.TokenExpiredAt <= DateTime.Now.AddSeconds(-30))
            {
                var tokenCreatedAt = DateTime.Now;

                try
                {
                    var loginResponse = await _dEngageClient.Login(CreateAuthRequest());

                    authResponse.ResponseCode = "0";
                    OperatorConfig.AuthToken = loginResponse.access_token;
                    OperatorConfig.TokenCreatedAt = tokenCreatedAt;
                    OperatorConfig.TokenExpiredAt = DateTime.Now.AddSeconds(loginResponse.expires_in);
                    _authToken = OperatorConfig.AuthToken;
                    await SaveOperator();
                }
                catch (ApiException ex)
                {
                    authResponse.ResponseCode = "99999";
                    authResponse.ResponseMessage = $"dEngage | Http Status Code : {ex.StatusCode} | Auth Failed";
                }

            }
            else
            {
                authResponse.ResponseCode = "0";
                _authToken = OperatorConfig.AuthToken;
            }

            return authResponse;
        }

        public async Task<SmsStatusResponse> CheckSms(string queryId)
        {
            await Task.CompletedTask;

            return new SmsStatusResponse()
            {
                code = 0,
                message = "Mock Succesfull",
                data = null
            };
        }

        public async Task<MailContentsResponse> GetMailContents(int limit, string offset)
        {
            MailContentsResponse mailContentsResponse = null;
            var authResponse = await Auth();
            if (authResponse.ResponseCode == "0")
            {
                mailContentsResponse = await _dEngageClient.GetMailContents(_authToken,limit,offset);
            }
            else
            {
                TransactionManager.LogCritical("dEngage Auth Failed | " + authResponse.ResponseMessage);
            }

            return mailContentsResponse;
        }

        public async Task<SmsContentsResponse> GetSmsContents(int limit, string offset)
        {
            SmsContentsResponse smsContentsResponse = null;
            var authResponse = await Auth();
            if (authResponse.ResponseCode == "0")
            {
                smsContentsResponse = await _dEngageClient.GetSmsContents(_authToken,limit,offset);
            }
            else
            {
                TransactionManager.LogCritical("dEngage Auth Failed | " + authResponse.ResponseMessage);
            }

            return smsContentsResponse;
        }

        public async Task<PushContentsResponse> GetPushContents(int limit, string offset)
        {
            PushContentsResponse pushContentsResponse = null;
            var authResponse = await Auth();
            if (authResponse.ResponseCode == "0")
            {
                try
                {
                    pushContentsResponse = await _dEngageClient.GetPushContents(_authToken,limit,offset);
                }
                catch (Exception ex)
                { 
                    
                }
            }
            else
            {
                TransactionManager.LogCritical("dEngage Auth Failed | " + authResponse.ResponseMessage);
            }

            return pushContentsResponse;
        }

        public async Task<MailResponseLog> SendMail(string to, string from, string subject, string html, string templateId, string templateParams, List<common.Models.Attachment> attachments,string? cc,string? bcc)
        {
            await Auth();

            MailResponseLog mailResponseLog = new() {
                ResponseCode = "0",
                ResponseMessage = "Mock Mail Successfull",
                StatusQueryId = System.Guid.NewGuid().ToString(),
                Topic = "dEngage Mock Mail"
            };

            var attachmentList = new List<System.Net.Mail.Attachment>();
            if (attachments != null)
            {
                foreach (common.Models.Attachment attachment in attachments)
                {
                    attachmentList.Add(new System.Net.Mail.Attachment(new MemoryStream(Convert.FromBase64String(attachment.Data)), attachment.Name));
                }
            }
            else
            {
                attachmentList = null;
            }

            if (templateId != null)
            {
                var response = await _dEngageClient.GetMailContent(_authToken, templateId);
                var mailContentInfo = response.data.contentDetail.contents.Where(c => c.language == "TR").FirstOrDefault();
                if (mailContentInfo != null)
                {
                    var htmlContent = mailContentInfo.content;
                    if (templateParams != null)
                    {
                        var templateParamsJson = JsonConvert.DeserializeObject<JObject>(templateParams);
                        var templateParamsList = htmlContent.GetWithRegexMultiple("({%=)(.*?)(%})", 2);
                        foreach (string templateParam in templateParamsList)
                        {
                            htmlContent = htmlContent.Replace("{%="+templateParam+"%}",(string)templateParamsJson[templateParam.Split(".")[1]]);
                        }
                    }
                    _fakeSmtpHelper.SendFakeMail(mailContentInfo.fromAddress, mailContentInfo.fromName, to, mailContentInfo.subject, htmlContent, attachmentList);
                }
            }
            else
            {
                _fakeSmtpHelper.SendFakeMail(from,from,to,subject,html,attachmentList);
            }

            return mailResponseLog;
        }

        public async Task<PushNotificationResponseLog> SendPush(string contactId, string template, string templateParams, string customParameters, bool? saveInbox, string[] tags)
        {
            await Auth();
            PushNotificationResponseLog pushNotificationResponseLog = new()
            {
                ResponseCode = "0",
                ResponseMessage = "Mock Push Successfull",
                Topic = "dEngage Mock Transactional Push"
            };

            if (template != null)
            {
                var response = await _dEngageClient.GetPushContent(_authToken, template);
                var pushContentInfo = response.data.contentDetail.contents.Where(c => c.language == "TR").FirstOrDefault();
                if (pushContentInfo != null)
                {
                    var htmlContent = pushContentInfo.message;
                    if (templateParams != null)
                    {
                        var templateParamsJson = JsonConvert.DeserializeObject<JObject>(templateParams);
                        var templateParamsList = htmlContent.GetWithRegexMultiple("({%=)(.*?)(%})", 2);
                        foreach (string templateParam in templateParamsList)
                        {
                            htmlContent = htmlContent.Replace("{%=" + templateParam + "%}", (string)templateParamsJson[templateParam.Split(".")[1]]);
                        }
                    }
                    _fakeSmtpHelper.SendFakeMail("MobileApp@mailDev.com","Mobil App",contactId + "@mailDev.com",pushContentInfo.title, htmlContent, null);
                }
            }
            else
            {
                //_fakeSmtpHelper.SendFakeMail("Sender@mailDev.com", "Sender", phone.ToString() + "mailDev.com", "dEngage Transactional Sms", content, null);
            }

            return pushNotificationResponseLog;
        }

        public async Task<SmsResponseLog> SendSms(Phone phone, SmsTypes smsType, string content, string templateId, string templateParams)
        {
            await Auth();
            SmsResponseLog smsResponseLog = new()
            {
                Operator = _transactionManager.CustomerRequestInfo.BusinessLine == "X" ? OperatorType.dEngageOn : OperatorType.dEngageBurgan,
                StatusQueryId = Guid.NewGuid().ToString(),
                OperatorResponseCode = 0,
                OperatorResponseMessage = "Mock Sms Successfull",
                Status = "",
            };

            if (templateId != null)
            {
                var response = await _dEngageClient.GetSmsContent(_authToken, templateId);
                var smsContentInfo = response.data.contentDetail.contents.Where(c => c.language == "TR").FirstOrDefault();
                if (smsContentInfo != null)
                {
                    var htmlContent = smsContentInfo.message;
                    if (templateParams != null)
                    {
                        var templateParamsJson = JsonConvert.DeserializeObject<JObject>(templateParams);
                        var templateParamsList = htmlContent.GetWithRegexMultiple("({%=)(.*?)(%})", 2);
                        foreach (string templateParam in templateParamsList)
                        {
                            htmlContent = htmlContent.Replace("{%=" + templateParam + "%}", (string)templateParamsJson[templateParam.Split(".")[1]]);
                        }
                    }
                    _fakeSmtpHelper.SendFakeMail(smsContentInfo.senderName+"@mailDev.com", smsContentInfo.senderName, phone.ToString()+"@mailDev.com","dEngage Transactional Sms", htmlContent, null);
                }
            }
            else
            {
                _fakeSmtpHelper.SendFakeMail("Sender@mailDev.com","Sender",phone.ToString()+"@mailDev.com", "dEngage Transactional Sms", content, null);
            }

            return smsResponseLog;
        }

        private LoginRequest CreateAuthRequest()
        {
            return new LoginRequest()
            {
                userkey = OperatorConfig.User,
                password = OperatorConfig.Password
            };
        }

        public Task<MailStatusResponse> CheckMail(string queryId)
        {
            throw new NotImplementedException();
        }
    }
}
