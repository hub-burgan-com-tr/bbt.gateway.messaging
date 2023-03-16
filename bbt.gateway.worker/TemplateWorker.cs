using bbt.gateway.common;
using bbt.gateway.common.Api.dEngage;
using bbt.gateway.common.Api.dEngage.Model.Contents;
using bbt.gateway.common.Api.dEngage.Model.Login;
using bbt.gateway.common.GlobalConstants;
using bbt.gateway.common.Helpers;
using bbt.gateway.common.Models;
using Dapr.Client;
using Elastic.Apm.Api;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Refit;
using System.Text;

namespace bbt.gateway.worker
{
    public class TemplateWorker : BackgroundService
    {
        private readonly LogManager _logManager;
        private readonly ITracer _tracer;
        private readonly DatabaseContext _dbContext;
        private Operator _operatorBurgan;
        private Operator _operatorOn;
        private readonly IdEngageClient _dEngageClient;
        private readonly DaprClient _daprClient;
        

        public TemplateWorker(
            LogManager logManager, ITracer tracer, DbContextOptions<DatabaseContext> dbContextOptions,
            IdEngageClient dEngageClient, DaprClient daprClient)
        {
            _logManager = logManager;
            _tracer = tracer;
            _dEngageClient = dEngageClient;
            _dbContext = new DatabaseContext(dbContextOptions);
            _daprClient = daprClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logManager.LogInformation("Set Templates Initiated");
            _operatorBurgan = await _dbContext.Operators.AsNoTracking().FirstAsync(o => o.Type == OperatorType.dEngageBurgan);
            _operatorOn = await _dbContext.Operators.AsNoTracking().FirstAsync(o => o.Type == OperatorType.dEngageOn);
            while (!stoppingToken.IsCancellationRequested)
            {
                _logManager.LogInformation("Set Templates Triggered");
                var taskList = new List<Task>();
                taskList.Add(SetTemplates(_operatorBurgan));
                taskList.Add(SetTemplates(_operatorOn));
                await Task.WhenAll(taskList);

                await Task.Delay(1000 * 60 * 10, stoppingToken);
            }
        }

        private async Task SetTemplates(Operator @operator)
        {
            await _tracer.CaptureTransaction("SetTemplates" + @operator.Type.ToString(), ApiConstants.TypeRequest, async () =>
            {
                try
                {
                    //Auth dEngage
                    await dEngageAuth(@operator);
                    if (!string.IsNullOrWhiteSpace(@operator.AuthToken))
                    {
                        var taskList = new List<Task>();

                        taskList.Add(SetSmsTemplates(@operator));
                        taskList.Add(SetMailTemplates(@operator));
                        taskList.Add(SetPushTemplates(@operator));

                        await Task.WhenAll(taskList);
                    }

                }
                catch (Exception ex)
                {
                    _logManager.LogError(ex.ToString());
                    _tracer.CaptureException(ex);
                }
            });
        }

        private async Task dEngageAuth(Operator @operator)
        {
            try
            {
                @operator.AuthToken = String.Empty;
                var response = await _dEngageClient.Login(new LoginRequest
                {
                    userkey = @operator.User,
                    password = @operator.Password,
                });
                @operator.AuthToken = response.access_token;
            }
            catch (ApiException apiEx)
            {
                _logManager.LogCritical(apiEx.ToString());
            }
            catch (Exception ex)
            {
                _logManager.LogCritical(ex.ToString());
            }
        }

        private async Task SetSmsTemplates(Operator @operator)
        {
            var span = _tracer.CurrentTransaction.StartSpan("Messaging Worker Set Sms", ApiConstants.TypeRequest, ApiConstants.SubtypeHttp);

            try
            {
                var smsContentList = new List<SmsContentInfo>();
                int limit = 500;
                int offsetMultiplexer = 0;

                while (true)
                {
                    var smsContents = await _dEngageClient.GetSmsContents(@operator.AuthToken, limit, (limit * offsetMultiplexer).ToString());

                    if (smsContents.data?.result.Count > 0)
                    {
                        smsContentList.AddRange(smsContents.data.result);
                    }

                    if (smsContents.data.queryForNextPage)
                    {
                        offsetMultiplexer++;
                    }
                    else
                    {
                        break;
                    }
                }

                if (smsContentList != null && smsContentList.Count > 0)
                {
                        await _daprClient.SaveStateAsync(GlobalConstants.DAPR_STATE_STORE,
                            @operator.Type.ToString() + "_" + GlobalConstants.SMS_CONTENTS_SUFFIX,
                            Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(smsContentList)));
                        _logManager.LogInformation($"{@operator.Type} sms content count : {smsContentList.Count}");
                        foreach (SmsContentInfo content in smsContentList)
                        {
                            try
                            {
                                var smsContent = await _dEngageClient.GetSmsContent(@operator.AuthToken, content.publicId);
                                await _daprClient.SaveStateAsync(GlobalConstants.DAPR_STATE_STORE,
                                    GlobalConstants.SMS_CONTENTS_SUFFIX + "_" + content.publicId,
                                    Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(smsContent.data.contentDetail)));
                                _logManager.LogInformation(GlobalConstants.SMS_CONTENTS_SUFFIX + "_" + content.publicId + " is Set With Detail");
                            }
                            catch (ApiException ex)
                            {
                                _logManager.LogError($"Api Exception - Status Code:{(int)ex.StatusCode} | An Error Occured While Trying To Caching Sms Contents");
                            }
                        }
                    }                
            }
            catch (ApiException apiEx)
            {
                span.CaptureException(apiEx);
                _logManager.LogError($"Api Exception - Status Code:{(int)apiEx.StatusCode} | An Error Occured While Trying To Caching Sms Contents");
            }
            catch (Exception ex)
            {
                span.CaptureException(ex);
                _logManager.LogError("An Error Occured While Trying To Caching Sms Contents");
            }
            finally
            {
                span.End();
            }
        }

        private async Task SetMailTemplates(Operator @operator)
        {
            var span = _tracer.CurrentTransaction.StartSpan("Messaging Worker Set Mail", ApiConstants.TypeRequest, ApiConstants.SubtypeHttp);

            try
            {
                var mailContentList = new List<ContentInfo>();
                int limit = 500;
                int offsetMultiplexer = 0;

                while (true)
                {
                    var mailContents = await _dEngageClient.GetMailContents(@operator.AuthToken, limit, (limit*offsetMultiplexer).ToString());

                    if (mailContents.data?.result.Count > 0)
                    {
                        mailContentList.AddRange(mailContents.data.result);
                    }

                    if (mailContents.data.queryForNextPage)
                    {
                        offsetMultiplexer++;
                    }
                    else
                    {
                        break;
                    }
                }

                if (mailContentList != null && mailContentList.Count > 0)
                {
                        await _daprClient.SaveStateAsync(GlobalConstants.DAPR_STATE_STORE,
                            @operator.Type.ToString() + "_" + GlobalConstants.MAIL_CONTENTS_SUFFIX,
                            Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(mailContentList)));
                        _logManager.LogInformation($"{@operator.Type} mail content count : {mailContentList.Count}");
                        foreach (ContentInfo content in mailContentList)
                        {
                            try
                            {
                                var mailContent = await _dEngageClient.GetMailContent(@operator.AuthToken, content.publicId);
                                await _daprClient.SaveStateAsync(GlobalConstants.DAPR_STATE_STORE,
                                    GlobalConstants.MAIL_CONTENTS_SUFFIX + "_" + content.publicId,
                                    Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(mailContent.data.contentDetail)));
                            }
                            catch (ApiException ex)
                            {
                                var error = await ex.GetContentAsAsync<MailContentResponse>();
                                _logManager.LogError($"Api Exception - Status Code:{(int)ex.StatusCode}:Message:{error.message} | An Error Occured While Trying To Caching Mail Content");
                            }
                        }
                }
            }
            catch (ApiException apiEx)
            {
                span.CaptureException(apiEx);
                var error = await apiEx.GetContentAsAsync<MailContentsResponse>();

                _logManager.LogError($"Api Exception - Status Code:{(int)apiEx.StatusCode}:Message:{error.message} | An Error Occured While Trying To Caching Mail Contents");
            }
            catch (Exception ex)
            {
                span.CaptureException(ex);
                _logManager.LogError("An Error Occured While Trying To Caching Mail Contents");
            }
            finally
            {
                span.End();
            }
        }

        private async Task SetPushTemplates(Operator @operator)
        {
            var span = _tracer.CurrentTransaction.StartSpan("Messaging Worker Set Push", ApiConstants.TypeRequest, ApiConstants.SubtypeHttp);

            try
            {
                var pushContentList = new List<PushContentInfo>();
                int limit = 500;
                int offsetMultiplexer = 0;

                while (true)
                {
                    var pushContents = await _dEngageClient.GetPushContents(@operator.AuthToken, limit, (limit*offsetMultiplexer).ToString());

                    if (pushContents.data?.result.Count > 0)
                    {
                        pushContentList.AddRange(pushContents.data.result);
                    }

                    if (pushContents.data.queryForNextPage)
                    {
                        offsetMultiplexer++;
                    }
                    else
                    {
                        break;
                    }
                }

                if (pushContentList != null)
                {
                        await _daprClient.SaveStateAsync(GlobalConstants.DAPR_STATE_STORE,
                            @operator.Type.ToString() + "_" + GlobalConstants.PUSH_CONTENTS_SUFFIX,
                            Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(pushContentList)));
                        _logManager.LogInformation($"{@operator.Type} push content count : {pushContentList.Count}");
                        foreach (PushContentInfo content in pushContentList)
                        {
                            try
                            {
                                var pushContent = await _dEngageClient.GetPushContent(@operator.AuthToken, content.id);
                                await _daprClient.SaveStateAsync(GlobalConstants.DAPR_STATE_STORE,
                                    GlobalConstants.PUSH_CONTENTS_SUFFIX + "_" + content.id,
                                    Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(pushContent.data.contentDetail)));
                            }
                            catch (ApiException ex)
                            {
                                _logManager.LogError($"Api Exception - Status Code:{(int)ex.StatusCode} | An Error Occured While Trying To Caching Push Contents");
                            }
                        }
                }
            }
            catch (ApiException apiEx)
            {
                span.CaptureException(apiEx);
                _logManager.LogError($"Api Exception - Status Code:{(int)apiEx.StatusCode} | An Error Occured While Trying To Caching Push Contents");
            }
            catch (Exception ex)
            {
                span.CaptureException(ex);
                _logManager.LogError("An Error Occured While Trying To Caching Push Contents");
            }
            finally
            {
                span.End();
            }
        }
    }
}
