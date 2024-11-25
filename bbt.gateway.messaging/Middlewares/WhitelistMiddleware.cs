using bbt.gateway.common.Models;
using bbt.gateway.common.Repositories;
using bbt.gateway.messaging.Exceptions;
using bbt.gateway.messaging.Workers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace bbt.gateway.messaging.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class WhitelistMiddleware
    {
        private readonly RequestDelegate _next;
        
        public WhitelistMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context,ITransactionManager _transactionManager,IRepositoryManager _repositoryManager)
        {
            
            _transactionManager.UseFakeSmtp = false;
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (environment != "Prod" && environment != "Drc")
            {
                if (environment == "Mock")
                {
                    _transactionManager.UseFakeSmtp = true;
                }
                else
                {
                    await CheckWhitelist(_transactionManager,_repositoryManager);
                }
            }
            await _next(context);
        }

        private async Task CheckWhitelist(ITransactionManager _transactionManager,IRepositoryManager _repositoryManager)
        {
            if (_transactionManager.Transaction.TransactionType == TransactionType.Otp ||
                _transactionManager.Transaction.TransactionType == TransactionType.TransactionalSms ||
                _transactionManager.Transaction.TransactionType == TransactionType.TransactionalTemplatedSms)
            {
                Phone phone = _transactionManager.Transaction.Phone;

                if (phone == null)
                    throw new WorkflowException("Phone number couldn't be resolved",System.Net.HttpStatusCode.NotFound);

                
                if ((await _repositoryManager.Whitelist.FindAsync(w =>
                (w.Phone.CountryCode == phone.CountryCode
                && w.Phone.Prefix == phone.Prefix
                && w.Phone.Number == phone.Number
                ))).FirstOrDefault() == null)
                {
                    _transactionManager.UseFakeSmtp = true;
                }
                                      
            }

            if (_transactionManager.Transaction.TransactionType == TransactionType.TransactionalMail ||
                _transactionManager.Transaction.TransactionType == TransactionType.TransactionalTemplatedMail)
            {
                string email = _transactionManager.Transaction.Mail;
                if (string.IsNullOrEmpty(email))
                    throw new WorkflowException("Mail address couldn't be resolved", System.Net.HttpStatusCode.NotFound);

                if ((await _repositoryManager.Whitelist.FindAsync(w => w.Mail == email)).FirstOrDefault()
                    == null)
                {
                    _transactionManager.UseFakeSmtp = true;
                }
            }

            if (_transactionManager.Transaction.TransactionType == TransactionType.TransactionalPush ||
                _transactionManager.Transaction.TransactionType == TransactionType.TransactionalTemplatedPush)
            {
                if (string.IsNullOrEmpty(_transactionManager.Transaction.CitizenshipNo))
                    throw new WorkflowException("CitizenshipNumber couldn't be resolved", System.Net.HttpStatusCode.NotFound);
                if ((await _repositoryManager.Whitelist.FindAsync(w => w.ContactId == _transactionManager.Transaction.CitizenshipNo)).FirstOrDefault()
                    == null)
                {
                    _transactionManager.UseFakeSmtp = true;
                }
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class WhitelistMiddlewareExtensions
    {
        public static IApplicationBuilder UseWhitelistMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseWhen(context => (context.Request.Path.Value.IndexOf("/Messaging") != -1
            && context.Request.Path.Value.IndexOf("/sms/check") == -1 && context.Request.Path.Value.IndexOf("/sms/message/stringAsync") == -1
            ), builder =>
            {
                builder.UseMiddleware<WhitelistMiddleware>();
            });
        }
    }
}
