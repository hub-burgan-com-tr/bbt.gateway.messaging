using bbt.gateway.common.Models;
using bbt.gateway.common.Repositories;
using bbt.gateway.messaging.Exceptions;
using bbt.gateway.messaging.Workers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.IO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Middlewares
{
    public class CustomerInfoMiddleware
    {
        private readonly RequestDelegate _next;
        
        public CustomerInfoMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context,ITransactionManager _transactionManager,IRepositoryManager _repositoryManager)
        {
            
            await GetCustomerDetail(_transactionManager,_repositoryManager);

            // Call the next delegate/middleware in the pipeline.
            await _next(context);   
        }
       
        private async Task GetCustomerDetail(ITransactionManager _transactionManager,IRepositoryManager _repositoryManager)
        {
            if (_transactionManager.Transaction.CustomerNo > 0)
            {
                await GetCustomerInfo(_transactionManager);
            }
            else
            {
                if (!string.IsNullOrEmpty(_transactionManager.Transaction.CitizenshipNo))
                {
                    await GetCustomerInfoByCitizenshipNumber(_transactionManager);
                }
            }
            
            if (_transactionManager.Transaction.Phone != null)
            {
                await GetCustomerInfoByPhone(_transactionManager,_repositoryManager);
            }
            else
            {
                if (!string.IsNullOrEmpty(_transactionManager.Transaction.Mail))
                {
                    await GetCustomerInfoByEmail(_transactionManager,_repositoryManager);
                }
                else
                {
                    if(_transactionManager.Transaction.TransactionType != TransactionType.TransactionalTemplatedMailMultiple &&
                        _transactionManager.Transaction.TransactionType != TransactionType.TransactionalMailMultiple &&
                        _transactionManager.Transaction.CustomerNo <= 0 && string.IsNullOrEmpty(_transactionManager.Transaction.CitizenshipNo))
                        throw new WorkflowException("Request should have at least one of those : (CustomerNo,Phone,Email,ContactId)",System.Net.HttpStatusCode.NotFound);
                }
            }
        }

        private async Task GetCustomerInfoByCitizenshipNumber(ITransactionManager _transactionManager)
        {
            await _transactionManager.GetCustomerInfoByCitizenshipNumber();
            await _transactionManager.GetCustomerInfoByCustomerNo();
        }

        private async Task GetCustomerInfoByPhone(ITransactionManager _transactionManager,IRepositoryManager _repositoryManager)
        {
            var phoneConfiguration = await _repositoryManager.PhoneConfigurations.GetWithBlacklistEntriesAsync(
                _transactionManager.Transaction.Phone.CountryCode, _transactionManager.Transaction.Phone.Prefix, _transactionManager.Transaction.Phone.Number, DateTime.Now);

            if (phoneConfiguration == null)
            {
                if (_transactionManager.Transaction.CustomerNo == 0)
                    await _transactionManager.GetCustomerInfoByPhone();

                phoneConfiguration = new PhoneConfiguration()
                {
                    CustomerNo = _transactionManager.Transaction.CustomerNo,
                    Phone = _transactionManager.Transaction.Phone,
                };
                phoneConfiguration.BlacklistEntries = new List<BlackListEntry>();
                phoneConfiguration.Logs = new List<PhoneConfigurationLog>() {
                                    new PhoneConfigurationLog()
                                    {
                                        Action = "Initialize",
                                        Type = "Add",
                                        Phone = phoneConfiguration,
                                        CreatedBy = _transactionManager.Transaction.CreatedBy,
                                    },
                                };

                await _repositoryManager.PhoneConfigurations.AddAsync(phoneConfiguration);
            }
            else
            {
                if (_transactionManager.Transaction.CustomerNo > 0)
                {
                    phoneConfiguration.CustomerNo = _transactionManager.Transaction.CustomerNo;
                }
                else
                {
                    if (phoneConfiguration.CustomerNo != null && phoneConfiguration.CustomerNo > 0)
                    {
                        _transactionManager.Transaction.CustomerNo = phoneConfiguration.CustomerNo.GetValueOrDefault();
                        await _transactionManager.GetCustomerInfoByCustomerNo();
                    }
                    else
                    {
                        await _transactionManager.GetCustomerInfoByPhone();
                        phoneConfiguration.CustomerNo = _transactionManager.CustomerRequestInfo.CustomerNo;
                    }
                }
            }

            if (_transactionManager.Transaction.TransactionType == TransactionType.Otp)
                _transactionManager.OtpRequestInfo.PhoneConfiguration = phoneConfiguration;
            else
                _transactionManager.SmsRequestInfo.PhoneConfiguration = phoneConfiguration;
        }

        private async Task GetCustomerInfoByEmail(ITransactionManager _transactionManager,IRepositoryManager _repositoryManager)
        {
            
            var mailConfiguration = await _repositoryManager.MailConfigurations.FirstOrDefaultAsync(m => m.Email == _transactionManager.Transaction.Mail);
            if (mailConfiguration == null)
            {
                if(_transactionManager.Transaction.CustomerNo == 0)
                    await _transactionManager.GetCustomerInfoByEmail();
                mailConfiguration = new MailConfiguration()
                {
                    CustomerNo = _transactionManager.Transaction.CustomerNo,
                    Email = _transactionManager.Transaction.Mail,

                };

                mailConfiguration.Logs = new List<MailConfigurationLog>();
                var mailConfigurationLog = new MailConfigurationLog()
                {
                    Action = "Initialize",
                    Type = "Add",
                    Mail = mailConfiguration,
                    CreatedBy = _transactionManager.Transaction.CreatedBy,
                };

                mailConfiguration.Logs.Add(mailConfigurationLog);

                await _repositoryManager.MailConfigurations.AddAsync(mailConfiguration);
            }
            else
            {
                if (_transactionManager.Transaction.CustomerNo > 0)
                {
                    mailConfiguration.CustomerNo = _transactionManager.Transaction.CustomerNo;
                }
                else
                {
                    if (mailConfiguration.CustomerNo != null && mailConfiguration.CustomerNo > 0)
                    {
                        _transactionManager.Transaction.CustomerNo = mailConfiguration.CustomerNo.GetValueOrDefault();
                        await _transactionManager.GetCustomerInfoByCustomerNo();
                    }
                    else
                    {
                        await _transactionManager.GetCustomerInfoByEmail();
                        mailConfiguration.CustomerNo = _transactionManager.CustomerRequestInfo.CustomerNo;
                    }
                }
                
            }
            _transactionManager.MailRequestInfo.MailConfiguration = mailConfiguration;
        }

        private async Task GetCustomerInfo(ITransactionManager _transactionManager)
        {
            await _transactionManager.GetCustomerInfoByCustomerNo();
        }

    }

    public static class CustomerInfoMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomerInfoMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseWhen(context => (context.Request.Path.Value.IndexOf("/Messaging") != -1
            && context.Request.Path.Value.IndexOf("/sms/check") == -1
            ), builder =>
            {
                builder.UseMiddleware<CustomerInfoMiddleware>();
            });
        }
    }
}
