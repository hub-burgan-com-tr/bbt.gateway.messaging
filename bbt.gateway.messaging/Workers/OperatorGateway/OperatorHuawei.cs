using AGConnectAdmin;
using AGConnectAdmin.Messaging;
using bbt.gateway.common.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Workers.OperatorGateway
{
    public class OperatorHuawei : OperatorGatewayBase, IOperatorHuawei
    {

        public OperatorHuawei(IConfiguration configuration,
            ITransactionManager transactionManager) : base(configuration, transactionManager)
        {

        }

        public async Task<PushNotificationResponseLog> SendPushNotificationAsync
                                                                   (
                                                                    string app,
                                                                    string deviceToken,
                                                                    string title,
                                                                    string content,
                                                                    string customParams,
                                                                    string targetUrl = ""
                                                                   )
        {
            var pushNotificationResponseLog = new PushNotificationResponseLog()
            {
                CreatedAt = DateTime.Now
            };
            try
            {
                await Policy.Handle<HttpRequestException>().RetryAsync(3,
                  (e, r) =>
                  {
                      TransactionManager.LogError($"Huawei Retry : {r}");
                  }).ExecuteAsync(async () =>
                  {

                      var message = new Message()
                      {
                          Android = new AndroidConfig()
                          {
                              Notification = new AndroidNotification()
                              {
                                  Title = title,
                                  Body = content,
                                  ClickAction = ClickAction.OpenApp()
                              }
                          },
                          Token = new List<string>() { deviceToken }
                      };

                      if (!string.IsNullOrWhiteSpace(customParams))
                      {
                          var data = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(customParams?.ClearMaskingFields()).ToDictionary(x => x.Keys.First(), x => x.Values.First());
                          if (!string.IsNullOrWhiteSpace(targetUrl))
                          {
                              if (data.ContainsKey("deeplink"))
                              {
                                  data.Remove("deeplink");
                                  data.Add("deeplink", targetUrl);
                              }
                          }
                          message.Data = JsonConvert.SerializeObject(data);
                      }
                      else
                      {
                          if (!string.IsNullOrWhiteSpace(targetUrl))
                          {
                              var data = new Dictionary<string, string>();
                              data.Add("deeplink", targetUrl);
                              message.Data = JsonConvert.SerializeObject(data);
                          }
                      }

                      // Send the message
                      try
                      {
                          TransactionManager.LogInformation("Huawei Push Request : " + JsonConvert.SerializeObject(message));

                          var response = await AGConnectMessaging.GetMessaging(AGConnectApp.GetInstance(app)).SendAsync(message);

                          pushNotificationResponseLog.ResponseCode = "0";
                          pushNotificationResponseLog.ResponseMessage = "Successfuly sended to Huawei";
                          pushNotificationResponseLog.StatusQueryId = response;
                      }
                      catch (Exception ex)
                      {
                          pushNotificationResponseLog.ResponseMessage = ex.Message;
                          pushNotificationResponseLog.ResponseCode = "-9999";
                      }
                  });
            }
            catch (Exception ex)
            {
                pushNotificationResponseLog.ResponseMessage = ex.Message;
                pushNotificationResponseLog.ResponseCode = "-9999";
            }

            return pushNotificationResponseLog;
        }
    }
}