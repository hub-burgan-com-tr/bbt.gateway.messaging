using Azure;
using bbt.gateway.common.Api.dEngage.Model.Transactional;
using bbt.gateway.common.Models;
using bbt.gateway.messaging.Api.Codec.Model;
using CodecFastApi;
using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Workers.OperatorGateway
{
    public class OperatorFirebase : OperatorGatewayBase,IOperatorFirebase
    {

        public OperatorFirebase(IConfiguration configuration,
            ITransactionManager transactionManager) : base(configuration, transactionManager)
        {
            
        }

        public Task CheckPushNotificationAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<PushNotificationResponseLog> SendPushNotificationAsync(string deviceToken, string title, string content, string customParams)
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
                      TransactionManager.LogError($"Firebase Retry : {r}");
                  }).ExecuteAsync(async () =>
                  {
                      var message = new Message()
                      {
                          Notification = new Notification
                          {
                              Title = title,
                              Body = content
                          },
                          Token = deviceToken,
                          
                      };

                      if (!string.IsNullOrWhiteSpace(customParams))
                      {
                          message.Data = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(customParams?.ClearMaskingFields()).ToDictionary(x => x.Keys.First(), x => x.Values.First());
                      }

                      // Send the message
                      try
                      {
                          var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                          pushNotificationResponseLog.ResponseCode = "0";
                          pushNotificationResponseLog.ResponseMessage = "Successfuly sended to Firebase";
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
