using bbt.gateway.messaging.Workers.OperatorGateway;
using Dapr.Client;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Helpers
{
    public class InstantReminder
    {
        private readonly IOperatordEngage _operatordEngage;
        private readonly IConfiguration _configuration;
        public InstantReminder(DaprClient daprClient,dEngageFactory dEngageFactory,IConfiguration configuration)
        {
            _operatordEngage = dEngageFactory(false);
            _configuration = configuration;
        }

        public async Task RemindAsync(string subject, string content, List<common.Models.Attachment> attachments)
        {
            if (System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Test")
            {
                await _operatordEngage.SendMail(_configuration["InstantReminder:To"], "no_replay", subject, content, null, null, attachments: attachments, null, null, null, checkIsVerified: false);
            }
        }
    }
}
