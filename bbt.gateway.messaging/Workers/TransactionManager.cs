﻿using bbt.gateway.common.Models;
using bbt.gateway.common.Repositories;
using bbt.gateway.messaging.Api.Pusula;
using bbt.gateway.messaging.Api.Pusula.Model.GetByCitizenshipNumber;
using bbt.gateway.messaging.Api.Pusula.Model.GetByPhone;
using bbt.gateway.messaging.Api.Pusula.Model.GetCustomer;
using bbt.gateway.messaging.Exceptions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Workers
{
    public class TransactionManager : ITransactionManager
    {
        private readonly Guid _txnId;
        private Serilog.ILogger _logger;
        private readonly PusulaClient _pusulaClient;
        private readonly IRepositoryManager _repositoryManager;
        public Transaction Transaction { get; set; }

        public Guid TxnId { get { return _txnId; } }
        public OtpRequestInfo OtpRequestInfo { get; set; } = new();
        public SmsRequestInfo SmsRequestInfo { get; set; } = new();
        public MailRequestInfo MailRequestInfo { get; set; } = new();
        public PushRequestInfo PushRequestInfo { get; set; } = new();

        public CustomerRequestInfo CustomerRequestInfo { get; set; } = new();
        public HeaderInfo HeaderInfo{ get; set; }
        public common.Models.v2.SenderType Sender { get; set; }
        public bool UseFakeSmtp { get; set; }
        public TransactionManager(PusulaClient pusulaClient,IRepositoryManager repositoryManager)
        {
            _txnId = Guid.NewGuid();
            _logger = Log.ForContext<TransactionManager>();
            _pusulaClient = pusulaClient;
            _repositoryManager = repositoryManager;
            
            Transaction = new()
            {
                Id = _txnId
            };
        }

        public async Task AddTransactionAsync()
        {
            await _repositoryManager.Transactions.AddAsync(Transaction);
        }

        public async Task SaveTransactionAsync()
        {
            await _repositoryManager.SaveChangesAsync();
        }

        public void LogState()
        {
            
            switch (Transaction.TransactionType)
            {
                case TransactionType.Otp:
                    LogInformation(JsonConvert.SerializeObject(new { CustomerRequestInfo, OtpRequestInfo }));
                    break;
                case TransactionType.TransactionalSms:
                    LogInformation(JsonConvert.SerializeObject(new { CustomerRequestInfo, SmsRequestInfo }));
                    break;
                case TransactionType.TransactionalTemplatedSms:
                    LogInformation(JsonConvert.SerializeObject(new { CustomerRequestInfo, SmsRequestInfo }));
                    break;
                case TransactionType.TransactionalMail:
                    LogInformation(JsonConvert.SerializeObject(new { CustomerRequestInfo, MailRequestInfo }));
                    break;
                case TransactionType.TransactionalTemplatedMail:
                    LogInformation(JsonConvert.SerializeObject(new { CustomerRequestInfo, MailRequestInfo }));
                    break;
                case TransactionType.TransactionalPush:
                    LogInformation(JsonConvert.SerializeObject(this));
                    break;
                case TransactionType.TransactionalTemplatedPush:
                    LogInformation(JsonConvert.SerializeObject(this));
                    break;
                default:
                    break;
            }

        }

        public async Task GetCustomerInfoByPhone()
        {
            var customer = await _pusulaClient.GetCustomerByPhoneNumber(new GetByPhoneNumberRequest()
            {
                CountryCode = Transaction.Phone.CountryCode,
                CityCode = Transaction.Phone.Prefix,
                TelephoneNumber = Transaction.Phone.Number
            });

            if (customer.IsSuccess)
            {

                CustomerRequestInfo.CustomerNo = customer.CustomerNo;

                var customerDetail = await _pusulaClient.GetCustomer(new GetCustomerRequest()
                {
                    CustomerNo = customer.CustomerNo
                });

                if (customerDetail.IsSuccess)
                {
                    SetCustomerRequestInfo(customerDetail);
                }
                else
                {
                    ThrowNotFoundError();
                }
            }
            else
            {
                ThrowNotFoundError();
            }

        }

        public async Task GetCustomerInfoByEmail()
        {
            var customer = await _pusulaClient.GetCustomerByEmail(new GetByEmailRequest()
            {
                Email = Transaction.Mail
            });

            if (customer.IsSuccess)
            {
                CustomerRequestInfo.CustomerNo = customer.CustomerNo;

                var customerDetail = await _pusulaClient.GetCustomer(new GetCustomerRequest()
                {
                    CustomerNo = customer.CustomerNo
                });

                if (customerDetail.IsSuccess)
                {
                    SetCustomerRequestInfo(customerDetail);
                }
                else
                {
                    ThrowNotFoundError();
                }
            }
            else
            {
                ThrowNotFoundError();
            }
        }

        public async Task GetCustomerInfoByCitizenshipNumber()
        {
            var customer = await _pusulaClient.GetCustomerByCitizenshipNumber(new GetByCitizenshipNumberRequest()
            {
                CitizenshipNumber = Transaction.CitizenshipNo
            });

            if (customer.IsSuccess)
            {
                CustomerRequestInfo.CustomerNo = customer.CustomerNo;

                var customerDetail = await _pusulaClient.GetCustomer(new GetCustomerRequest()
                {
                    CustomerNo = customer.CustomerNo
                });

                if (customerDetail.IsSuccess)
                {
                    SetCustomerRequestInfo(customerDetail);
                }
                else
                {
                    ThrowNotFoundError();
                }
            }
            else
            {
                ThrowNotFoundError();
            }
        }

        public async Task GetCustomerInfoByCustomerNo()
        {
            CustomerRequestInfo.CustomerNo = Transaction.CustomerNo;

            var customerDetail = await _pusulaClient.GetCustomer(new GetCustomerRequest()
            {
                CustomerNo = CustomerRequestInfo.CustomerNo
            });

            if (customerDetail.IsSuccess)
            {
                SetCustomerRequestInfo(customerDetail);
            }
            else
            {
                ThrowNotFoundError();
            }
        }

        private void SetCustomerRequestInfo(GetCustomerResponse customerDetail)
        {
            CustomerRequestInfo.BusinessLine = customerDetail.BusinessLine;
            CustomerRequestInfo.BranchCode = customerDetail.BranchCode;
            CustomerRequestInfo.MainPhone = customerDetail.MainPhone;
            CustomerRequestInfo.MainEmail = customerDetail.MainEmail;
            CustomerRequestInfo.Tckn = customerDetail.CitizenshipNo;

            if (Transaction.Phone == null)
            {
                Transaction.Phone = CustomerRequestInfo.MainPhone;
            }

            if (String.IsNullOrEmpty(Transaction.Mail))
            {
                Transaction.Mail = CustomerRequestInfo.MainEmail;
            }
        }

        private void ThrowNotFoundError()
        {
            if (HeaderInfo != null)
            {
                if (HeaderInfo.Sender == SenderType.AutoDetect)
                {
                    throw new WorkflowException("Couldn't get customer info", System.Net.HttpStatusCode.NotFound);
                }
            }
            else
            {
                if(Sender == common.Models.v2.SenderType.AutoDetect)
                    throw new WorkflowException("Couldn't get customer info", System.Net.HttpStatusCode.NotFound);
            }
        }

        public void LogCritical(string LogMessage)
        {
            _logger.Fatal("TxnId:" + _txnId + " | Type : " + Transaction.TransactionType + " :" + LogMessage);
        }

        public void LogError(string LogMessage)
        {
            _logger.Error("TxnId:" + _txnId + " | Type : " + Transaction.TransactionType + " :" + LogMessage);
        }

        public void LogDebug(string LogMessage)
        {
            _logger.Debug("TxnId:" + _txnId + " | Type : " + Transaction.TransactionType + " :" + LogMessage);
        }

        public void LogTrace(string LogMessage)
        {
            _logger.Verbose("TxnId:" + _txnId + " | Type : " + Transaction.TransactionType + " :" + LogMessage);
        }

        public void LogInformation(string LogMessage)
        {
            _logger.Information("TxnId:" + _txnId + " | Type : " + Transaction.TransactionType + " :" + LogMessage);
        }

        public void LogWarning(string LogMessage)
        {
            _logger.Warning("TxnId:" + _txnId + " | Type : " + Transaction.TransactionType + " :" + LogMessage);
        }
    }

    

    public class CustomerRequestInfo
    {
        public ulong CustomerNo { get; set; }
        public string BusinessLine { get; set; }
        public int BranchCode { get; set; }
        public Phone? MainPhone { get; set; }
        public string MainEmail { get; set; }
        public string Tckn { get; set; }
    }

    public class OtpRequestInfo
    {
        public Phone Phone { get; set; }
        [JsonIgnore]
        public PhoneConfiguration PhoneConfiguration { get; set; }
        public OperatorType Operator { get; set; }
        public bool BlacklistCheck { get; set; }
        public string Content { get; set; }
        public Process Process { get; set; }
    }

    public class SmsRequestInfo
    {
        public Phone Phone { get; set; }
        [JsonIgnore]
        public PhoneConfiguration PhoneConfiguration { get; set; }
        public OperatorType Operator { get; set; }
        public string Content { get; set; }
        public string TemplateId { get; set; }
        public string TemplateParams { get; set; }
        public Process Process { get; set; }
    }

    public class MailRequestInfo
    {
        public string Email { get; set; }
        [JsonIgnore]
        public MailConfiguration MailConfiguration { get; set; }
        public OperatorType Operator { get; set; }
        public string Content { get; set; }
        public string TemplateId { get; set; }
        public string TemplateParams { get; set; }
        public Process Process { get; set; }
    }

    public class PushRequestInfo
    {
        public string ContactId { get; set; }
        public OperatorType Operator { get; set; }
        public string TemplateId { get; set; }
        public string TemplateParams { get; set; }
        public string CustomParameters { get; set; }
        public Process Process { get; set; }
    }

}
