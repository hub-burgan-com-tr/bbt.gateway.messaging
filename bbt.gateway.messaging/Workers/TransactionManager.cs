using bbt.gateway.common.GlobalConstants;
using bbt.gateway.common.Models;
using bbt.gateway.common.Repositories;
using bbt.gateway.messaging.Api.Fora;
using bbt.gateway.messaging.Api.Fora.Model.Permission;
using bbt.gateway.messaging.Api.Pusula;
using bbt.gateway.messaging.Api.Pusula.Model.GetByCitizenshipNumber;
using bbt.gateway.messaging.Api.Pusula.Model.GetByPhone;
using bbt.gateway.messaging.Api.Pusula.Model.GetCustomer;
using bbt.gateway.messaging.Services;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Workers
{
    public class TransactionManager : ITransactionManager
    {
        private readonly Guid _txnId;
        private Serilog.ILogger _logger;
        private readonly PusulaClient _pusulaClient;
        private readonly ForaClient _foraClient;
        private readonly IRepositoryManager _repositoryManager;
        private readonly IOperatorService _operatorService;
        private Operator _activeOperator;
        public Transaction Transaction { get; set; }

        public Guid TxnId { get { return _txnId; } }
        public OtpRequestInfo OtpRequestInfo { get; set; } = new();
        public SmsRequestInfo SmsRequestInfo { get; set; } = new();
        public MailRequestInfo MailRequestInfo { get; set; } = new();
        public PushRequestInfo PushRequestInfo { get; set; } = new();

        public CustomerRequestInfo CustomerRequestInfo { get; set; } = new();
        public HeaderInfo HeaderInfo { get; set; }
        public common.Models.v2.SenderType Sender { get; set; }
        public bool UseFakeSmtp { get; set; }
        public bool? InstantReminder { get; set; }
        public SmsTypes SmsType { get; set; }
        public DateTime OldBlacklistVerifiedAt { get; set; }
        public bool StringSend { get; set; }
        public int PrefixLength { get; set; }
        public int NumberLength { get; set; }
        public Operator ActiveOperator { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public TransactionManager(PusulaClient pusulaClient, IOperatorService operatorService, ForaClient foraClient, IRepositoryManager repositoryManager)
        {
            _txnId = Guid.NewGuid();
            _logger = Log.ForContext<TransactionManager>();
            _pusulaClient = pusulaClient;
            _foraClient = foraClient;
            _repositoryManager = repositoryManager;
            _operatorService = operatorService;

            Transaction = new()
            {
                Id = _txnId
            };
            StringSend = false;
        }

        public async Task AddTransactionAsync()
        {
            await _repositoryManager.Transactions.AddAsync(Transaction);
        }

        public async Task SaveTransactionAsync()
        {
            await _repositoryManager.SaveChangesAsync();
        }

        public common.Models.v2.Phone GetPhoneFromString(common.Models.v2.PhoneString phoneString)
        {
            if (phoneString == null)
                return null;
            PrefixLength = phoneString.Prefix.Length;
            NumberLength = phoneString.Number.Length;

            return new common.Models.v2.Phone()
            {
                CountryCode = Convert.ToInt32(phoneString.CountryCode),
                Prefix = Convert.ToInt32(phoneString.Prefix),
                Number = Convert.ToInt32(phoneString.Number)
            };
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
                CountryCode = Transaction.Phone.CountryCode.ToString(),
                CityCode = Transaction.Phone.Prefix.ToString(),
                TelephoneNumber = StringSend ? Transaction.Phone.Number.ToString().PadLeft(NumberLength, '0') : (Transaction.Phone.CountryCode == 90 ? Transaction.Phone.Number.ToString().PadLeft(7,'0') : Transaction.Phone.Number.ToString())
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
                    await SetCustomerRequestInfo(customerDetail);
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
                    await SetCustomerRequestInfo(customerDetail);
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
                    await SetCustomerRequestInfo(customerDetail);
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
                await SetCustomerRequestInfo(customerDetail);
            }
            else
            {
                ThrowNotFoundError();
            }
        }

        private async Task<string> GetCustomerPermissionInfo(string CitizenshipNo)
        {
            try
            {
                var response = await _foraClient.getPermission(CitizenshipNo);
                if (response.ResponseCode == 0)
                {
                    try
                    {
                        var permissionInfo = JsonConvert.DeserializeObject<PermissionInfo>(response.ResponseMesssage);
                        return permissionInfo.PreferredLanguage.Split('-')[1];
                    }
                    catch (Exception ex)
                    {
                        LogError($"Preferred Language Couldn't Be Retrieved | Detail : {ex.ToString()}");
                        return string.Empty;
                    }
                    
                }
                else
                {
                    return String.Empty;
                }
            }
            catch (Exception ex)
            {
                return String.Empty;
            }
        }

        private async Task SetCustomerRequestInfo(GetCustomerResponse customerDetail)
        {
            CustomerRequestInfo.BusinessLine = customerDetail.BusinessLine;
            CustomerRequestInfo.BranchCode = customerDetail.BranchCode;
            CustomerRequestInfo.MainPhone = customerDetail.MainPhone;
            CustomerRequestInfo.MainEmail = customerDetail.MainEmail;
            CustomerRequestInfo.Tckn = customerDetail.CitizenshipNo;
            CustomerRequestInfo.PreferedLanguage = await GetCustomerPermissionInfo(customerDetail.CitizenshipNo);

            if (Transaction.Phone == null)
            {
                Transaction.Phone = CustomerRequestInfo.MainPhone;
            }

            if (String.IsNullOrWhiteSpace(Transaction.Mail))
            {
                Transaction.Mail = CustomerRequestInfo.MainEmail;
            }

            if (String.IsNullOrWhiteSpace(Transaction.CitizenshipNo))
            {
                Transaction.CitizenshipNo = CustomerRequestInfo.Tckn;
            }

            if(Transaction.CustomerNo == 0)
            {
                Transaction.CustomerNo = CustomerRequestInfo.CustomerNo;
            }

            if (customerDetail.VerifiedMailAdresses.Exists(m => m == Transaction.Mail))
            {
                MailRequestInfo.IsMailVerified = true;
            }
            else
            {
                MailRequestInfo.IsMailVerified = false;
            }
        }

        private void ThrowNotFoundError()
        {
            if (HeaderInfo != null)
            {
                if (HeaderInfo.Sender == SenderType.AutoDetect)
                {
                    CustomerRequestInfo.BusinessLine = "B";
                }
            }
            else
            {
                if (Sender == common.Models.v2.SenderType.AutoDetect)
                    CustomerRequestInfo.BusinessLine = "B";
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

        public string? GetTemplateLanguage()
        {
            if (!string.IsNullOrEmpty(CustomerRequestInfo.PreferedLanguage))
            {
                return GlobalConstants.AVAILABLE_TEMPLATE_LANGUAGES.Contains(CustomerRequestInfo.PreferedLanguage) ? CustomerRequestInfo.PreferedLanguage : null;
            }
            else
            {
                return null;
            }
        }

        public async Task<Operator> GetOperatorAsync(OperatorType type)
        {
            return await _operatorService.GetOperator(type);
        }

        public async Task RevokeOperatorsAsync()
        {
            await _operatorService.RevokeCache();
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
        public string PreferedLanguage { get; set; }
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
        public bool IsMailVerified { get; set; }
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
