using bbt.gateway.messaging.Workers;
using bbt.gateway.messaging.Workers.OperatorGateway;
using Dapr.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Helpers
{
    public class InstantReminder
    {
        private readonly IOperatordEngage _operatordEngage;
        private readonly IConfiguration _configuration;
        private readonly ITransactionManager _transactionManager;
        public InstantReminder(DaprClient daprClient,dEngageFactory dEngageFactory,
            ITransactionManager transactionManager,IConfiguration configuration)
        {
            _operatordEngage = dEngageFactory(false);
            _configuration = configuration;
            _transactionManager = transactionManager;
        }

        public async Task RemindAsync(string subject, string content, List<common.Models.Attachment> attachments)
        {
            if (System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Test" && (_transactionManager.InstantReminder ?? false))
            {
                try
                {
                    _transactionManager.LogInformation("Reminder initialized");
                    _operatordEngage.Type = common.Models.OperatorType.dEngageBurgan;
      
                    var rt = await _operatordEngage.SendMail(_configuration["InstantReminder:To"], "noreply@m.burgan.com.tr", subject, content, null, null, attachments, null, null, null, checkIsVerified: false);
                    _transactionManager.LogInformation("Reminder Response Message:" + rt.ResponseMessage);
                    _transactionManager.LogInformation("Reminder Response Status:" + rt.ResponseCode);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception Reminder:" + ex.Message);
                }
                
            }
        }
    }
}
