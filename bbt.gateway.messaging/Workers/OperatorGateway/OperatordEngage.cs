﻿using bbt.gateway.messaging.Api;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using bbt.gateway.messaging.Api.dEngage;
using bbt.gateway.messaging.Api.dEngage.Model.Login;
using bbt.gateway.messaging.Api.dEngage.Model.Settings;
using bbt.gateway.messaging.Api.dEngage.Model.Transactional;
using bbt.gateway.common.Models;
using Refit;
using System.Collections.Generic;
using bbt.gateway.messaging.Api.dEngage.Model.Contents;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace bbt.gateway.messaging.Workers.OperatorGateway
{
    public class OperatordEngage : OperatorGatewayBase,IOperatordEngage
    {
        private GetSmsFromsResponse _smsIds;
        private GetMailFromsResponse _mailIds;

        private int _authTryCount;
        private string _authToken;
        private readonly IdEngageClient _dEngageClient;
        private IDistributedCache _distrubitedCache;
        public OperatordEngage(IdEngageClient dEngageClient, IConfiguration configuration,
            ITransactionManager transactionManager,IDistributedCache distributedCache) : base(configuration,transactionManager)
        {
            _authTryCount = 0;
            _dEngageClient = dEngageClient;
            _distrubitedCache = distributedCache;
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

        private async Task<OperatorApiAuthResponse> RefreshToken()
        {
            OperatorApiAuthResponse authResponse = new();

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

            return authResponse;
        }

        public async Task<SmsStatusResponse> CheckSms(string queryId)
        {
            var authResponse = await Auth();
            if (authResponse.ResponseCode == "0")
            {
                try
                {
                    SmsStatusRequest smsStatusRequest = new()
                    {
                        trackingId = queryId,
                    };
                    var response = await _dEngageClient.GetSmsStatus(_authToken, smsStatusRequest);
                    return response;
                }
                catch (ApiException ex)
                {

                }
            }
            else
            { 
            
            }
            return null;
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

        public async Task<SmsContentsResponse> GetSmsContents(int limit,string offset)
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
                pushContentsResponse = await _dEngageClient.GetPushContents(_authToken,limit,offset);
            }
            else
            {
                TransactionManager.LogCritical("dEngage Auth Failed | " + authResponse.ResponseMessage);
            }

            return pushContentsResponse;
        }

        public async Task<(string, string)> SetMailFromsToCache()
        {
            try
            {                
                var res = await _dEngageClient.GetMailFroms(_authToken);
                await _distrubitedCache.SetAsync(OperatorConfig.Type.ToString() + "_mailFroms",
                    System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(res)),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(1)
                    }
                    );
                _mailIds = res;   
            }
            catch (ApiException ex)
            {
                return ("99999", $"dEngage | Http Status Code : {(int)ex.StatusCode} | Cannot Retrieve Sms Froms");
            }

            return ("0", "");
        }

        public async Task<MailResponseLog> SendMail(string to, string? from, string? subject, string? html, string? templateId, string? templateParams,List<common.Models.Attachment> attachments,string? cc,string? bcc)
        {
            var mailResponseLog = new MailResponseLog() { 
                Topic = "dEngage Mail Sending",
            };

            MailFrom mailFrom = new MailFrom();
            var authResponse = await Auth();
            if (authResponse.ResponseCode == "0")
            {
                if (html != null)
                {
                    var mailFromsByteArray = await _distrubitedCache.GetAsync(OperatorConfig.Type.ToString() + "_mailFroms");
                    if (mailFromsByteArray != null)
                    {
                        _mailIds = JsonConvert.DeserializeObject<GetMailFromsResponse>(System.Text.Encoding.UTF8.GetString(mailFromsByteArray));
                    }
                    else
                    {
                        var res = await SetMailFromsToCache();
                        if (res.Item1 != "0")
                        {
                            mailResponseLog.ResponseCode = res.Item1;
                            mailResponseLog.ResponseMessage = res.Item2;
                        }
                    }

                    mailFrom = _mailIds.data.emailFroms.Where(m => m.fromAddress == from).FirstOrDefault();
                    if (mailFrom == null) 
                    {
                        var res = await SetMailFromsToCache();
                        if (res.Item1 != "0")
                        {
                            mailResponseLog.ResponseCode = res.Item1;
                            mailResponseLog.ResponseMessage = res.Item2;
                        }

                        mailFrom = _mailIds.data.emailFroms.Where(m => m.fromAddress == from).FirstOrDefault();
                        if (mailFrom == null)
                        {
                            mailResponseLog.ResponseCode = "99999";
                            mailResponseLog.ResponseMessage = "Mail From is Not Found";
                        }
                    }
                    
                }
               
                try
                {
                    var req = CreateMailRequest(to,mailFrom.fromName,from, subject, html, templateId, templateParams,attachments,cc,bcc);
                    try
                    {
                        
                        var sendMailResponse = await _dEngageClient.SendMail(req, _authToken);
                        mailResponseLog.ResponseCode = sendMailResponse.code.ToString();
                        mailResponseLog.ResponseMessage = sendMailResponse.message;
                        mailResponseLog.StatusQueryId = sendMailResponse.data.to.trackingId;

                    }
                    catch (ApiException ex)
                    {
                        if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            authResponse = await RefreshToken();
                            if (authResponse.ResponseCode == "0")
                            {
                                _authTryCount++;
                                if (_authTryCount < 3)
                                {
                                    return await SendMail(to,from,subject,html,templateId,templateParams,attachments,cc,bcc);
                                }
                                else
                                {
                                    mailResponseLog.ResponseCode = "99999";
                                    mailResponseLog.ResponseMessage = "dEngage Auth Failed For 3 Times";
                                    return mailResponseLog;
                                }
                            }
                            else
                            {
                                mailResponseLog.ResponseCode = authResponse.ResponseCode;
                                mailResponseLog.ResponseMessage = authResponse.ResponseMessage;
                                return mailResponseLog;
                            }
                        }
                        if ((int)ex.StatusCode >= 400 && (int)ex.StatusCode < 500)
                        {
                            var error = await ex.GetContentAsAsync<Api.dEngage.Model.Transactional.SendSmsResponse>();
                            mailResponseLog.ResponseCode = error.code.ToString();
                            mailResponseLog.ResponseMessage = error.message;
                        }
                        if ((int)ex.StatusCode >= 500)
                        {
                            mailResponseLog.ResponseCode = "-999";
                            mailResponseLog.ResponseMessage = "dEngage Internal Server Error";
                        }
                    }                    


                    return mailResponseLog;
                }
                catch (Exception ex)
                {
                    mailResponseLog.ResponseCode = "-99999";
                    mailResponseLog.ResponseMessage = ex.ToString();

                    //logging
                    return mailResponseLog;
                }

            }
            else
            {
                mailResponseLog.ResponseCode = authResponse.ResponseCode;
                mailResponseLog.ResponseMessage = authResponse.ResponseMessage;

                return mailResponseLog;
            }

        }

        public async Task<PushNotificationResponseLog> SendPush(string contactId, string template, string templateParams, string customParameters)
        {
            var pushNotificationResponseLog = new PushNotificationResponseLog()
            {
                Topic = "dEngage Push Notification Sending",
            };

            var authResponse = await Auth();
            if (authResponse.ResponseCode == "0")
            {
                try
                {
                    var req = CreatePushRequest(contactId,template,templateParams,customParameters);
                    try
                    {
                        var sendPushResponse = await _dEngageClient.SendPush(req, _authToken);
                        pushNotificationResponseLog.ResponseCode = sendPushResponse.code.ToString();
                        pushNotificationResponseLog.ResponseMessage = sendPushResponse.message;
                    }
                    catch (ApiException ex)
                    {
                        if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            authResponse = await RefreshToken();
                            if (authResponse.ResponseCode == "0")
                            {
                                _authTryCount++;
                                if (_authTryCount < 3)
                                {
                                    return await SendPush(contactId, template, templateParams, customParameters);
                                }
                                else
                                {
                                    pushNotificationResponseLog.ResponseCode = "99999";
                                    pushNotificationResponseLog.ResponseMessage = "dEngage Auth Failed For 3 Times";
                                    return pushNotificationResponseLog;
                                }   
                            }
                            else
                            {
                                pushNotificationResponseLog.ResponseCode = authResponse.ResponseCode;
                                pushNotificationResponseLog.ResponseMessage = authResponse.ResponseMessage;
                                return pushNotificationResponseLog;
                            }
                        }
                        if ((int)ex.StatusCode >= 400 && (int)ex.StatusCode < 500)
                        {
                            var error = await ex.GetContentAsAsync<Api.dEngage.Model.Transactional.SendSmsResponse>();
                            pushNotificationResponseLog.ResponseCode = error.code.ToString();
                            pushNotificationResponseLog.ResponseMessage = error.message;
                        }
                        if ((int)ex.StatusCode >= 500)
                        {
                            pushNotificationResponseLog.ResponseCode = "-999";
                            pushNotificationResponseLog.ResponseMessage = "dEngage Internal Server Error";
                        }
                    }


                    return pushNotificationResponseLog;
                }
                catch (Exception ex)
                {
                    pushNotificationResponseLog.ResponseCode = "-99999";
                    pushNotificationResponseLog.ResponseMessage = ex.ToString();

                    //logging
                    return pushNotificationResponseLog;
                }

            }
            else
            {
                pushNotificationResponseLog.ResponseCode = authResponse.ResponseCode;
                pushNotificationResponseLog.ResponseMessage = authResponse.ResponseMessage;

                return pushNotificationResponseLog;
            }

        }

        public async Task<SmsResponseLog> SendSms(Phone phone,SmsTypes smsType, string? content,string? templateId,string? templateParams)
        {
            var smsLog = new SmsResponseLog()
            {
                Operator = Type,
                Content = String.IsNullOrEmpty(content) ? "" : content.ClearMaskingFields(),
                CreatedAt = DateTime.Now,
            };

            var authResponse = await Auth();
            if (authResponse.ResponseCode == "0")
            {
                if (content != null)
                {
                    try
                    {
                        var smsFromsByteArray = await _distrubitedCache.GetAsync(OperatorConfig.Type.ToString() + "_smsFroms");
                        if (smsFromsByteArray != null)
                        {
                            _smsIds = JsonConvert.DeserializeObject<GetSmsFromsResponse>(System.Text.Encoding.UTF8.GetString(smsFromsByteArray));
                        }
                        else
                        {
                            var res = await _dEngageClient.GetSmsFroms(_authToken);
                            await _distrubitedCache.SetAsync(OperatorConfig.Type.ToString() + "_smsFroms",
                                System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(res)),
                                new DistributedCacheEntryOptions
                                {
                                    AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(1)
                                }
                                );
                            _smsIds = res;
                        }
                        
                    }
                    catch (ApiException ex)
                    {
                        smsLog.OperatorResponseCode = 99999;
                        smsLog.OperatorResponseMessage = $"dEngage | Http Status Code : {(int)ex.StatusCode} | Cannot Retrieve Sms Froms";
                        return smsLog;
                    }
                }
                try
                {
                    var req = CreateSmsRequest(phone, smsType, content, templateId, templateParams);
                    try
                    {
                        var sendSmsResponse = await _dEngageClient.SendSms(req, _authToken);
                        smsLog.OperatorResponseCode = sendSmsResponse.code;
                        smsLog.OperatorResponseMessage = sendSmsResponse.message;
                        smsLog.StatusQueryId = sendSmsResponse.data.to.trackingId;
                        smsLog.Status = "";

                    }
                    catch (ApiException ex)
                    {
                        if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            authResponse = await RefreshToken();
                            if (authResponse.ResponseCode == "0")
                            {
                                _authTryCount++;
                                if (_authTryCount < 3)
                                {
                                    return await SendSms(phone, smsType, content, templateId, templateParams);
                                }
                                else
                                {
                                    smsLog.OperatorResponseCode = 99999;
                                    smsLog.OperatorResponseMessage = "dEngage Auth Failed For 3 Times";
                                    return smsLog;
                                }
                            }
                            else
                            {
                                smsLog.OperatorResponseCode = Convert.ToInt32(authResponse.ResponseCode);
                                smsLog.OperatorResponseMessage = authResponse.ResponseMessage;
                                return smsLog;
                            }
                        }
                        if ((int)ex.StatusCode >= 400 && (int)ex.StatusCode < 500)
                        {
                            var error = await ex.GetContentAsAsync<Api.dEngage.Model.Transactional.SendSmsResponse>();
                            smsLog.OperatorResponseCode = error.code;
                            smsLog.OperatorResponseMessage = error.message;
                        }
                        if ((int)ex.StatusCode >= 500)
                        {
                            smsLog.OperatorResponseCode = -999;
                            smsLog.OperatorResponseMessage = "dEngage Internal Server Error";
                        }
                    }
                    
                    return smsLog;
                }
                catch (Exception ex)
                {
                    //logging
                    smsLog.OperatorResponseCode = -99999;
                    smsLog.OperatorResponseMessage = ex.ToString();
                    return smsLog;
                }                 
                
            }
            else
            {
                smsLog.OperatorResponseCode = Convert.ToInt32(authResponse.ResponseCode);
                smsLog.OperatorResponseMessage = authResponse.ResponseMessage;              

                return smsLog;
            }
        }

        private SendMailRequest CreateMailRequest(string to,string fromName,string from,string subject, string html, string templateId, string templateParams,List<common.Models.Attachment> attachments,string cc,string bcc)
        {
            SendMailRequest sendMailRequest = new();
            sendMailRequest.send.to = to;

            sendMailRequest.send.cc = string.IsNullOrEmpty(cc) ? null : cc;
            sendMailRequest.send.bcc = string.IsNullOrEmpty(bcc) ? null : bcc;

            if (attachments != null)
            {
                sendMailRequest.attachments = new();
                foreach (common.Models.Attachment attachment in attachments)
                {
                    sendMailRequest.attachments.Add(new()
                    {
                        fileName = attachment.Name,
                        fileContent = attachment.Data 
                    });
                }
            }
            if (!string.IsNullOrEmpty(templateId))
            {
                sendMailRequest.content.templateId = templateId;
                if (!string.IsNullOrEmpty(templateParams))
                {
                    sendMailRequest.current = templateParams.ClearMaskingFields();
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(html))
                {
                    sendMailRequest.content.FromAddress = from;
                    sendMailRequest.content.FromName = fromName;
                    sendMailRequest.content.html = html.ClearMaskingFields();
                    sendMailRequest.content.subject = subject.ClearMaskingFields();
                }
                else
                {
                    //Critical Error
                }

            }
            return sendMailRequest;
        }

        private SendPushRequest CreatePushRequest(string contactId, string template, string templateParams,string customParameters)
        {
            SendPushRequest sendPushRequest = new();
            sendPushRequest.contactKey = contactId;
            sendPushRequest.contentId = template;
            if (string.IsNullOrEmpty(templateParams))
            {
                sendPushRequest.current = null;
            }
            else
            {
                sendPushRequest.current = templateParams?.ClearMaskingFields();
            }
            if (string.IsNullOrEmpty(customParameters))
            {
                sendPushRequest.customParameters = null;
            }
            else
            {
                sendPushRequest.customParameters = customParameters?.ClearMaskingFields();
            }
            return sendPushRequest;
        }

        private Api.dEngage.Model.Transactional.SendSmsRequest CreateSmsRequest(Phone phone, SmsTypes smsType, string content = null,string templateId = null,string templateParams = null)
        {
            Api.dEngage.Model.Transactional.SendSmsRequest sendSmsRequest = new();
            sendSmsRequest.send.to = phone.Concatenate();
            if (!string.IsNullOrEmpty(templateId))
            {
                sendSmsRequest.content.templateId = templateId;
                if (!string.IsNullOrEmpty(templateParams))
                {
                    sendSmsRequest.current = templateParams.ClearMaskingFields();
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(content))
                {
                    sendSmsRequest.content.smsFromId = _smsIds.data.smsFroms.Where(i => i.partnerName == Constant.smsTypes[smsType]).FirstOrDefault().id;
                    sendSmsRequest.content.message = content.ClearMaskingFields();
                }
                else
                { 
                    //Critical Error
                }

            }
            return sendSmsRequest;
        }

        private LoginRequest CreateAuthRequest()
        {
            return new LoginRequest() { 
                userkey = OperatorConfig.User,
                password = OperatorConfig.Password
            };
        }
    }
}
