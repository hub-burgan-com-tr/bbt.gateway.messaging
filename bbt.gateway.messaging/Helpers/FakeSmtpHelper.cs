using bbt.gateway.messaging.Workers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Helpers
{
    public class FakeSmtpHelper : IFakeSmtpHelper
    {
        private IConfiguration _configuration;
        public FakeSmtpHelper(IConfiguration configuration)
        {
            _configuration = configuration; 
        }

        public void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            string token = (string)e.UserState;
            if (e.Cancelled)
            {
                Debug.WriteLine($"{token} Send Canceled");
            }
            if (e.Error != null)
            {
                Debug.WriteLine($"[{token}] {e.Error.ToString()}");
            }
            else 
            {
                Debug.WriteLine("Message sent");
            }
        }

        public void SendFakeMail(string fromMail,string fromName,string to,string subject,string content,List<Attachment>? attachments)
        {
            using var client = new SmtpClient(_configuration["MailDev:Host"],_configuration.GetValue<int>("MailDev:Port"));
            MailAddress fromAddress = new MailAddress(fromMail,fromName,System.Text.Encoding.UTF8);
            MailAddress toAddress = new MailAddress(to);
            using MailMessage message = new MailMessage(fromAddress,toAddress);

            message.Body = content;
            message.IsBodyHtml = true;
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.Subject = subject;
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            if (attachments != null)
            {
                foreach (var attachment in attachments)
                {
                    message.Attachments.Add(attachment);
                }
            }
            client.Send(message);
        }
    }
}
