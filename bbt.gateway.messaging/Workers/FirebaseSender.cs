using bbt.gateway.common.Api.Amorphie;
using bbt.gateway.common.Api.dEngage.Model.Contents;
using bbt.gateway.common.Extensions;
using bbt.gateway.common.GlobalConstants;
using bbt.gateway.common.Models;
using bbt.gateway.common.Repositories;
using bbt.gateway.messaging.Exceptions;
using bbt.gateway.messaging.Helpers;
using bbt.gateway.messaging.Workers.OperatorGateway;
using Dapr.Client;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Workers
{
    public class FirebaseSender
    {
        private readonly HeaderManager _headerManager;
        private readonly IRepositoryManager _repositoryManager;
        private readonly ITransactionManager _transactionManager;
        private readonly IOperatorFirebase _operatorFirebase;
        private readonly InstantReminder _instantReminder;
        private readonly IUserApi _userApi;
        private readonly DaprClient _daprClient;

        public FirebaseSender(HeaderManager headerManager,
            IOperatorFirebase operatorFirebase,
            IRepositoryManager repositoryManager,
            ITransactionManager transactionManager,
            InstantReminder instantReminder,
            IUserApi userApi,
            IConfiguration configuration,
            DaprClient daprClient
        )
        {
            _headerManager = headerManager;
            _repositoryManager = repositoryManager;
            _transactionManager = transactionManager;
            _operatorFirebase = operatorFirebase;
            _instantReminder = instantReminder;
            _userApi = userApi;
            _daprClient = daprClient;
        }

        public async Task CheckPushNotificationAsync()
        {
            await Task.CompletedTask;
        }

        public async Task<common.Models.v2.FirebasePushResponse> SendPushNotificationAsync(common.Models.v2.PushRequest data)
        {
            common.Models.v2.FirebasePushResponse firebasePushResponse = new()
            {
                TxnId = _transactionManager.TxnId,
            };

            await _operatorFirebase.GetOperatorAsync(OperatorType.Firebase);

            var pushRequest = new PushNotificationRequestLog()
            {
                Operator = _operatorFirebase.Type,
                ContactId = data.CitizenshipNo,
                Content = data.Content.MaskFields(),
                TemplateId = "",
                TemplateParams = "",
                CustomParameters = data.CustomParameters?.MaskFields(),
                CreatedBy = data.Process.MapTo<Process>(),
                SaveInbox = data.saveInbox ?? false,
                NotificationType = data.NotificationType ?? string.Empty
            };

            try
            {
                await _repositoryManager.PushNotificationRequestLogs.AddAsync(pushRequest);
                _transactionManager.Transaction.PushNotificationRequestLog = pushRequest;

                var deviceToken = await _userApi.GetDeviceTokenAsync(data.CitizenshipNo);
                var response = await _operatorFirebase.SendPushNotificationAsync(deviceToken.token, data.Title ?? string.Empty, data.Content, data.CustomParameters);
                pushRequest.ResponseLogs.Add(response);

                firebasePushResponse.Status = response.ResponseCode.Equals("0") ? FirebasePushResponseCodes.Success : FirebasePushResponseCodes.Failed;

                return firebasePushResponse;
            }
            catch (System.Exception ex)
            {
                throw new WorkflowException("Device is not found", System.Net.HttpStatusCode.NotFound);
            }
        }

        public async Task<common.Models.v2.FirebasePushResponse> SendTemplatedPushNotificationAsync(common.Models.v2.TemplatedPushRequest data)
        {
            common.Models.v2.FirebasePushResponse firebasePushResponse = new()
            {
                TxnId = _transactionManager.TxnId,
            };

            string contentListName = string.Empty;

            if (_transactionManager.CustomerRequestInfo.BusinessLine == "X")
            {
                contentListName = "dEngageOn";
            }
            else
            {
                contentListName = "dEngageBurgan";
            }

            var contentList = await GetContentList<PushContentInfo>(contentListName + "_" + GlobalConstants.PUSH_CONTENTS_SUFFIX);

            var contentInfo = GetContentInfo(contentList, data.Template);

            await _operatorFirebase.GetOperatorAsync(OperatorType.Firebase);

            var pushRequest = new PushNotificationRequestLog()
            {
                Operator = _operatorFirebase.Type,
                ContactId = data.CitizenshipNo,
                TemplateId = data.Template,
                TemplateParams = data.TemplateParams?.MaskFields(),
                CustomParameters = data.CustomParameters?.MaskFields(),
                CreatedBy = data.Process.MapTo<Process>(),
                SaveInbox = data.saveInbox ?? false,
                NotificationType = data.NotificationType ?? string.Empty
            };

            var pushTemplateTitle = "";

            var targetUrls = new List<KeyValuePair<string,string>>();

            var templateDetail = await GetContentDetail<PushContentDetail>(GlobalConstants.PUSH_CONTENTS_SUFFIX + "_" + contentInfo.id);

            if (templateDetail != null)
            {
                var templateContent = templateDetail.contents.FirstOrDefault();
                pushTemplateTitle = templateContent != null ? templateContent.title : "";
                
                if (!string.IsNullOrWhiteSpace(data.TemplateParams))
                {
                    pushRequest.Content = templateContent.message;
                    var templateParamsJson = JsonConvert.DeserializeObject<JObject>(data.TemplateParams.ClearMaskingFields());
                    var templateParamsList = templateContent?.message.GetWithRegexMultiple("({%=)(.*?)(%})", 2);

                    foreach (var element in templateParamsJson)
                    {
                        _transactionManager.LogInformation($"templateParamsJson Key:{element.Key} | Value :{element.Value} ");
                    }
                    _transactionManager.LogInformation("Parameters");
                    templateParamsList.ForEach(e => _transactionManager.LogInformation(e));

                    foreach (string templateParam in templateParamsList)
                    {
                        pushRequest.Content = pushRequest.Content.Replace("{%=" + templateParam + "%}", (string)templateParamsJson[templateParam.Split(".")[1]]);
                    }
                }
                else
                {
                    pushRequest.Content = templateContent.message;
                }

                //dEngage deeplinks
                if(!string.IsNullOrWhiteSpace(templateContent.android?.targetUrl))
                {
                    targetUrls.Add(new KeyValuePair<string,string>("android", templateContent.android?.targetUrl));
                }
                if (!string.IsNullOrWhiteSpace(templateContent.ios?.targetUrl))
                {
                    targetUrls.Add(new KeyValuePair<string, string>("ios", templateContent.android?.targetUrl));
                }
            }            

            try
            {
                await _repositoryManager.PushNotificationRequestLogs.AddAsync(pushRequest);
                _transactionManager.Transaction.PushNotificationRequestLog = pushRequest;

                var device = await _userApi.GetDeviceTokenAsync(data.CitizenshipNo);
                var targetUrl = string.Empty;
                if(targetUrls?.Count > 0)
                {
                    targetUrl = targetUrls.FirstOrDefault(t => t.Key.Equals(device.os)).Value;
                }

                var response = await _operatorFirebase.SendPushNotificationAsync(device.token, pushTemplateTitle, pushRequest.Content, data.CustomParameters, targetUrl);
                pushRequest.ResponseLogs.Add(response);

                firebasePushResponse.Status = response.ResponseCode.Equals("0") ? FirebasePushResponseCodes.Success : FirebasePushResponseCodes.Failed;

                return firebasePushResponse;
            }
            catch (System.Exception ex)
            {
                throw new WorkflowException("Device is not found", System.Net.HttpStatusCode.NotFound);
            }
        }

        private T GetContentInfo<T>(List<T> contentList, string givenTemplate) where T : IContentReadeble
        {
            var templateInfo = contentList.Where(c => c.GetPath(givenTemplate.Trim().StartsWith("/")) == GetTemplateName(givenTemplate)).FirstOrDefault();
            if (templateInfo == null)
                throw new WorkflowException("Template Not Found", System.Net.HttpStatusCode.NotFound);

            return templateInfo;
        }

        private async Task<List<T>> GetContentList<T>(string templateListPath)
        {
            var contentListByteArray = await _daprClient.GetStateAsync<byte[]>(GlobalConstants.DAPR_STATE_STORE, templateListPath);
            return JsonConvert.DeserializeObject<List<T>>(
                        Encoding.UTF8.GetString(contentListByteArray)
                    );
        }
        private string GetTemplateNameWithPreferredLang(string template)
        {
            return $"{template.Trim()}_{_transactionManager.CustomerRequestInfo.PreferedLanguage}";
        }

        private string GetTemplateName(string template)
        {
            return template.Trim();
        }
        private async Task<T> GetContentDetail<T>(string templateSelector)
        {
            try
            {
                var contentDetailByteArray = await _daprClient.GetStateAsync<byte[]>(GlobalConstants.DAPR_STATE_STORE, templateSelector.Trim());
                return JsonConvert.DeserializeObject<T>(
                            Encoding.UTF8.GetString(contentDetailByteArray)
                        );
            }
            catch (Exception ex)
            {
                return default;
            }
        }
    }
}