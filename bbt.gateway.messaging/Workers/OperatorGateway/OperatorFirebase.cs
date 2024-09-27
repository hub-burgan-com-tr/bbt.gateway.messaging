using Azure;
using bbt.gateway.common.Models;
using bbt.gateway.messaging.Api.Codec.Model;
using CodecFastApi;
using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Polly;
using System;
using System.Linq;
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

        public async Task<PushNotificationResponseLog> SendPushNotificationAsync(string deviceToken, string title, string content)
        {
            var pushNotificationResponseLog = new PushNotificationResponseLog()
            {
                CreatedAt = DateTime.Now
            };
            try
            {
                await Policy.Handle<EndpointNotFoundException>().RetryAsync(5,
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
                          Token = deviceToken
                      };

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
