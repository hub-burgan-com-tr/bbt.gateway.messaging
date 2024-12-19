using bbt.gateway.common.Api.dEngage.Model.Contents;
using bbt.gateway.common.GlobalConstants;
using bbt.gateway.common.Models;
using bbt.gateway.common.Models.v2;
using bbt.gateway.messaging.Exceptions;
using bbt.gateway.messaging.Workers;
using Dapr.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Helpers
{
    public class TemplatedPushHelper
    {
        private readonly ITransactionManager _transactionManager;
        private readonly DaprClient _daprClient;

        public TemplatedPushHelper(
            ITransactionManager transactionManager,
            DaprClient daprClient
        )
        {
            _transactionManager = transactionManager;
            _daprClient = daprClient;
        }

        public async Task<PushTemplateParameter> SetTemplateParametersAsync(
                                                TemplatedPushRequest data,
                                                PushNotificationRequestLog pushRequest,
                                                string os
                                            )
        {
            var pushTemplateParameter = new PushTemplateParameter();

            string contentListName = string.Empty;

            if (_transactionManager.CustomerRequestInfo.BusinessLine == "X")
            {
                contentListName = "dEngageOn";
            }
            else
            {
                contentListName = "dEngageBurgan";
            }

            var contentList = await GetContentListAsync<PushContentInfo>(contentListName + "_" + GlobalConstants.PUSH_CONTENTS_SUFFIX);

            var contentInfo = GetContentInfo(contentList, data.Template);

            var pushTemplateTitle = "";
            var targetUrl = string.Empty;

            var targetUrls = new List<KeyValuePair<string, string>>();

            var templateDetail = await GetContentDetailAsync<PushContentDetail>(GlobalConstants.PUSH_CONTENTS_SUFFIX + "_" + contentInfo.id);

            _transactionManager.LogInformation($"Template Detail : {JsonConvert.SerializeObject(templateDetail)}");

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

                    if (!string.IsNullOrWhiteSpace(templateContent.android?.targetUrl))
                    {
                        var tUrl = templateContent.android?.targetUrl;
                        var templateParamsListAndroid = templateContent?.android?.targetUrl.GetWithRegexMultiple("({%=)(.*?)(%})", 2);

                        foreach (string templateParam in templateParamsListAndroid)
                        {
                            tUrl = tUrl.Replace("{%=" + templateParam + "%}", (string)templateParamsJson[templateParam.Split(".")[1]]);
                        }

                        targetUrls.Add(new KeyValuePair<string, string>("android", tUrl));
                    }

                    if (!string.IsNullOrWhiteSpace(templateContent.ios?.targetUrl))
                    {
                        var tUrl = templateContent.ios?.targetUrl;
                        var templateParamsListIos = templateContent?.android?.targetUrl.GetWithRegexMultiple("({%=)(.*?)(%})", 2);

                        foreach (string templateParam in templateParamsListIos)
                        {
                            tUrl = tUrl.Replace("{%=" + templateParam + "%}", (string)templateParamsJson[templateParam.Split(".")[1]]);
                        }

                        targetUrls.Add(new KeyValuePair<string, string>("ios", tUrl));
                    }
                }
                else
                {
                    pushRequest.Content = templateContent.message;

                    if (!string.IsNullOrWhiteSpace(templateContent.android?.targetUrl))
                    {
                        targetUrls.Add(new KeyValuePair<string, string>("android", templateContent.android?.targetUrl));
                    }
                    if (!string.IsNullOrWhiteSpace(templateContent.ios?.targetUrl))
                    {
                        targetUrls.Add(new KeyValuePair<string, string>("ios", templateContent.ios?.targetUrl));
                    }
                }

                _transactionManager.LogInformation($"Target Url's : {JsonConvert.SerializeObject(targetUrls)}");

                if (targetUrls?.Count > 0)
                {
                    targetUrl = targetUrls.FirstOrDefault(t => t.Key.Equals(os.ToLower())).Value;
                }

                _transactionManager.LogInformation($"Selected Target Url : {targetUrl}");
            }

            pushTemplateParameter.Title = pushTemplateTitle;
            pushTemplateParameter.TargetUrl = targetUrl;

            return pushTemplateParameter;
        }

        private T GetContentInfo<T>(List<T> contentList, string givenTemplate) where T : IContentReadeble
        {
            var templateInfo = contentList.Where(c => c.GetPath(givenTemplate.Trim().StartsWith("/")) == GetTemplateName(givenTemplate)).FirstOrDefault();
            if (templateInfo == null)
                throw new WorkflowException("Template Not Found", System.Net.HttpStatusCode.NotFound);

            return templateInfo;
        }

        private async Task<List<T>> GetContentListAsync<T>(string templateListPath)
        {
            var contentListByteArray = await _daprClient.GetStateAsync<byte[]>(GlobalConstants.DAPR_STATE_STORE, templateListPath);
            return JsonConvert.DeserializeObject<List<T>>(
                        Encoding.UTF8.GetString(contentListByteArray)
                    );
        }       

        private string GetTemplateName(string template)
        {
            return template.Trim();
        }
        private async Task<T> GetContentDetailAsync<T>(string templateSelector)
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