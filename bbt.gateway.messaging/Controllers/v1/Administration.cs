using Asp.Versioning;
using bbt.gateway.common.Api.dEngage.Model.Contents;
using bbt.gateway.common.GlobalConstants;
using bbt.gateway.common.Models;
using bbt.gateway.common.Models.v1;
using bbt.gateway.common.Repositories;
using bbt.gateway.messaging.Workers;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Controllers.v1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class Administration : ControllerBase
    {
        private readonly HeaderManager _headerManager;
        private readonly OperatorManager _operatorManager;
        private readonly IRepositoryManager _repositoryManager;
        private readonly ITransactionManager _transactionManager;
        private readonly dEngageSender _dEngageSender;
        private readonly IDistributedCache _distributedCache;
        private readonly UserSettings _userSettings;
        private readonly DaprClient _daprClient;
        private readonly IConfiguration _configuration;
        public Administration(HeaderManager headerManager, OperatorManager operatorManager,
            IRepositoryManager repositoryManager, ITransactionManager transactionManager,
            dEngageSender dEngageSender, IDistributedCache distributedCache, IOptions<UserSettings> userSettings,
            DaprClient daprClient, IConfiguration configuration)
        {
            _headerManager = headerManager;
            _operatorManager = operatorManager;
            _repositoryManager = repositoryManager;
            _transactionManager = transactionManager;
            _dEngageSender = dEngageSender;
            _distributedCache = distributedCache;
            _userSettings = userSettings.Value;
            _daprClient = daprClient;
            _configuration = configuration;
        }

        [SwaggerOperation(Summary = "Write Templates To Cache")]
        [HttpPost("templates/sms")]
        [SwaggerResponse(200, "Templates stored successfully")]
        public async Task<IActionResult> SetSmsTemplates()
        {
            await _dEngageSender.SetSmsContents(OperatorType.dEngageBurgan);
            await _dEngageSender.SetSmsContents(OperatorType.dEngageOn);
            return Ok();
        }

        [SwaggerOperation(Summary = "Write Templates To Cache")]
        [HttpPost("templates/mail")]
        [SwaggerResponse(200, "Templates stored successfully")]
        public async Task<IActionResult> SetMailTemplates()
        {
            await _dEngageSender.SetMailContents(OperatorType.dEngageBurgan);
            await _dEngageSender.SetMailContents(OperatorType.dEngageOn);
            return Ok();
        }

        [SwaggerOperation(Summary = "Write Templates To Cache")]
        [HttpPost("templates/push")]
        [SwaggerResponse(200, "Templates stored successfully")]
        public async Task<IActionResult> SetPushTemplates()
        {
            await _dEngageSender.SetPushContents(OperatorType.dEngageBurgan);
            await _dEngageSender.SetPushContents(OperatorType.dEngageOn);
            return Ok();
        }

        [SwaggerOperation(Summary = "Get Templates From Cache")]
        [HttpGet("templates/sms/burgan")]
        [SwaggerResponse(200, "Templates returned successfully")]
        public async Task<IActionResult> GetBurganSmsTemplates()
        {
            var data = await _daprClient.GetStateAsync<byte[]>(GlobalConstants.DAPR_STATE_STORE, "dEngageBurgan_SmsContents");
            return Ok(JsonConvert.DeserializeObject<List<SmsContentInfo>>(
                        Encoding.UTF8.GetString(data)
                    ));
        }

        [SwaggerOperation(Summary = "Get Templates From Cache")]
        [HttpGet("templates/sms/on")]
        [SwaggerResponse(200, "Templates returned successfully")]
        public async Task<IActionResult> GetOnSmsTemplates()
        {
            var data = await _daprClient.GetStateAsync<byte[]>(GlobalConstants.DAPR_STATE_STORE, "dEngageOn_SmsContents");
            return Ok(JsonConvert.DeserializeObject<List<SmsContentInfo>>(
                        Encoding.UTF8.GetString(data)
                    ));
        }

        [SwaggerOperation(Summary = "Get Templates From Cache")]
        [HttpGet("templates/mail/burgan")]
        [SwaggerResponse(200, "Templates returned successfully")]
        public async Task<IActionResult> GetBurganMailTemplates()
        {
            var data = await _daprClient.GetStateAsync<byte[]>(GlobalConstants.DAPR_STATE_STORE, "dEngageBurgan_MailContents");
            return Ok(JsonConvert.DeserializeObject<List<ContentInfo>>(
                        Encoding.UTF8.GetString(data)
                    ));
        }

        [SwaggerOperation(Summary = "Get Templates From Cache")]
        [HttpGet("templates/mail/on")]
        [SwaggerResponse(200, "Templates returned successfully")]
        public async Task<IActionResult> GetOnMailTemplates()
        {
            var data = await _daprClient.GetStateAsync<byte[]>(GlobalConstants.DAPR_STATE_STORE, "dEngageOn_MailContents");
            return Ok(JsonConvert.DeserializeObject<List<ContentInfo>>(
                        Encoding.UTF8.GetString(data)
                    ));
        }

        [SwaggerOperation(Summary = "Get Templates From Cache")]
        [HttpGet("templates/push/burgan")]
        [SwaggerResponse(200, "Templates returned successfully")]
        public async Task<IActionResult> GetBurganPushTemplates()
        {
            var data = await _daprClient.GetStateAsync<byte[]>(GlobalConstants.DAPR_STATE_STORE, "dEngageBurgan_PushContents");
            return Ok(JsonConvert.DeserializeObject<List<PushContentInfo>>(
                        Encoding.UTF8.GetString(data)
                    ));
        }

        [SwaggerOperation(Summary = "Get Templates From Cache")]
        [HttpGet("templates/push/on")]
        [SwaggerResponse(200, "Templates returned successfully")]
        public async Task<IActionResult> GetOnPushTemplates()
        {
            var data = await _daprClient.GetStateAsync<byte[]>(GlobalConstants.DAPR_STATE_STORE, "dEngageOn_PushContents");
            return Ok(JsonConvert.DeserializeObject<List<PushContentInfo>>(
                        Encoding.UTF8.GetString(data)
                    ));
        }

        [SwaggerOperation(Summary = "Get All Key/Value Pairs From Cache")]
        [HttpGet("caches")]
        [SwaggerResponse(200, "Pairs returned successfully")]
        public async Task<IActionResult> GetAllPairs()
        {
            List<string> listKeys = new List<string>();
            using (ConnectionMultiplexer redis = ConnectionMultiplexer.Connect($"{_configuration["Redis:Host"]}:{_configuration["Redis:Port"]},password={_configuration["Redis:Password"]}"))
            {
                var keys = redis.GetServer(_configuration["Redis:Host"]+":"+_configuration["Redis:Port"]).Keys();
                listKeys.AddRange(keys.Select(key => (string)key).ToList());

            }

            return Ok(listKeys);
        }


        [SwaggerOperation(Summary = "Returns content headers configuration")]
        [HttpGet("headers")]
        [SwaggerResponse(200, "Headers is returned successfully", typeof(Header[]))]
        public async Task<IActionResult> GetHeaders([FromQuery][Range(0, 100)] int page = 0, [FromQuery][Range(1, 100)] int pageSize = 20)
        {
            return Ok(await _headerManager.Get(page, pageSize));
        }

        [SwaggerOperation(Summary = "Save or update header configuration")]
        [HttpPost("headers")]
        [SwaggerResponse(200, "Header is saved successfully", typeof(Header[]))]
        public async Task<IActionResult> SaveHeader([FromBody] Header data)
        {
            await _headerManager.Save(data);
            return Ok();
        }

        [SwaggerOperation(Summary = "Deletes header configuration")]
        [HttpDelete("headers/{id}")]
        [SwaggerResponse(200, "Header is deleted successfully", typeof(Header[]))]
        public async Task<IActionResult> DeleteHeader([FromQuery] Guid id)
        {
            await _headerManager.Delete(id);
            return Ok();
        }

        [SwaggerOperation(Summary = "Returns operator configurations")]
        [HttpGet("operators")]
        [SwaggerResponse(200, "Operators was returned successfully", typeof(Operator[]))]
        public async Task<IActionResult> GetOperators()
        {
            return Ok(await _operatorManager.Get());
        }

        [SwaggerOperation(Summary = "Updated operator configuration")]
        [HttpPost("operators")]
        [SwaggerResponse(200, "operator has saved successfully", typeof(void))]
        public async Task<IActionResult> SaveOperator([FromBody] Operator data)
        {
            await _operatorManager.Save(data);
            return Ok();
        }


        [SwaggerOperation(Summary = "Returns phone activities")]
        [HttpGet("phone-monitor/{countryCode}/{prefix}/{number}")]
        [SwaggerResponse(200, "Records was returned successfully", typeof(PhoneConfiguration))]

        public async Task<IActionResult> GetPhoneMonitorRecords(int countryCode, int prefix, int number, int count)
        {
            return Ok(await _repositoryManager.PhoneConfigurations.GetWithRelatedLogsAndBlacklistEntriesAsync(countryCode, prefix, number, count));
        }


        [SwaggerOperation(Summary = "Returns phone blacklist records")]
        [HttpGet("blacklists/{countryCode}/{prefix}/{number}")]
        [SwaggerResponse(200, "Records was returned successfully", typeof(BlackListEntry))]

        public async Task<IActionResult> GetPhoneBlacklistRecords(int countryCode, int prefix, int number, [Range(0, 100)] int page = 0, [Range(1, 100)] int pageSize = 20)
        {
            return Ok(await _repositoryManager.BlackListEntries
                .GetWithLogsAsync(countryCode, prefix, number, page, pageSize));
        }

        [SwaggerOperation(Summary = "Adds phone to blacklist records")]
        [HttpPost("blacklists")]
        [SwaggerResponse(201, "Record was created successfully", typeof(void))]
        public async Task<IActionResult> AddPhoneToBlacklist([FromBody] AddPhoneToBlacklistRequest data)
        {
            return Ok();
        }

        [SwaggerOperation(Summary = "Resolve blacklist item")]
        [HttpPatch("blacklists/{blacklist-entry-id}/resolve")]
        [SwaggerResponse(201, "Record was updated successfully", typeof(void))]
        [SwaggerResponse(404, "Record not found", typeof(void))]
        public async Task<IActionResult> ResolveBlacklistItem([FromRoute(Name = "blacklist-entry-id")] Guid entryId, [FromBody] ResolveBlacklistEntryRequest data)
        {
            var config = await _repositoryManager.BlackListEntries.FirstOrDefaultAsync(b => b.Id == entryId);
            if (config == null)
                return NotFound(entryId);
            var resolvedAt = DateTime.Now;
            config.ResolvedBy = data.ResolvedBy;
            config.Status = BlacklistStatus.Resolved;
            config.ResolvedAt = resolvedAt;

            //Update Old System
            var oldBlacklistEntry = await _repositoryManager.DirectBlacklists.FirstOrDefaultAsync(b => b.SmsId == config.SmsId);
            if (oldBlacklistEntry != null)
            {
                oldBlacklistEntry.VerifyDate = resolvedAt;
                oldBlacklistEntry.IsVerified = true;
                oldBlacklistEntry.VerifiedBy = data.ResolvedBy.Identity;
                oldBlacklistEntry.Explanation = "Messaging Gateway Tarafından Onaylandı.";
                await _repositoryManager.SaveSmsBankingChangesAsync();
            }

            await _repositoryManager.SaveChangesAsync();

            return StatusCode(201);
        }

        [SwaggerOperation(Summary = "Adds phone or mail to whitelist records")]
        [HttpPost("whitelist")]
        [SwaggerResponse(201, "Record was created successfully", typeof(void))]
        public async Task<IActionResult> AddPhoneToWhitelist([FromBody] AddWhitelistRequest data)
        {
            if (string.IsNullOrEmpty(data.Email) && data.Phone == null)
            {
                return NotFound();
            }

            var whitelistRecord = new WhiteList()
            {
                CreatedBy = data.CreatedBy,
                IpAddress = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                    ?? HttpContext.Connection.RemoteIpAddress.ToString()
            };

            if (data.Phone != null)
            {
                whitelistRecord.Phone = data.Phone;
            }
            if (!string.IsNullOrEmpty(data.Email))
            {
                whitelistRecord.Mail = data.Email;
            }

            await _repositoryManager.Whitelist.AddAsync(whitelistRecord);

            return Created("", whitelistRecord.Id);
        }

        [SwaggerOperation(Summary = "Returns phones otp sending logs")]
        [HttpGet("otp-log/{countryCode}/{prefix}/{number}")]
        [SwaggerResponse(200, "Records was returned successfully", typeof(OtpRequestLog[]))]
        public async Task<IActionResult> GetOtpLog(int countryCode, int prefix, int number, [Range(0, 100)] int page = 0, [Range(1, 100)] int pageSize = 20)
        {
            return Ok(await _repositoryManager.OtpRequestLogs
                .GetWithResponseLogsAsync(countryCode, prefix, number, page, pageSize));
        }

        [SwaggerOperation(Summary = "Returns phones sms sending logs")]
        [HttpGet("sms-log/{countryCode}/{prefix}/{number}")]
        [SwaggerResponse(200, "Records was returned successfully", typeof(SmsResponseLog[]))]
        public async Task<IActionResult> GetSmsLog(int countryCode, int prefix, int number, [Range(0, 100)] int page = 0, [Range(1, 100)] int pageSize = 20)
        {

            return Ok((await _repositoryManager.SmsRequestLogs
                .FindAsync(c => c.PhoneConfiguration.Phone.CountryCode == countryCode && c.PhoneConfiguration.Phone.Prefix == prefix && c.PhoneConfiguration.Phone.Number == number))
                .Skip(page * pageSize)
                .Take(pageSize));
        }

        [SwaggerOperation(Summary = "Returns transactions info")]
        [HttpGet("transactions/phone/{countryCode}/{prefix}/{number}")]
        [SwaggerResponse(200, "Records was returned successfully", typeof(Transaction[]))]
        public async Task<IActionResult> GetTransactionsWithPhone(int countryCode, int prefix, int number, int smsType, DateTime startDate, DateTime endDate, [Range(0, 100)] int page = 0, [Range(1, 100)] int pageSize = 20)
        {
            if (smsType == 1)
            {
                var res = await _repositoryManager.Transactions.GetOtpMessagesWithPhoneAsync(countryCode, prefix, number, startDate, endDate, page, pageSize);
                return Ok(new TransactionsDto { Transactions = res.Item1, Count = res.Item2 });
            }
            if (smsType == 2)
            {
                var res = await _repositoryManager.Transactions.GetTransactionalSmsMessagesWithPhoneAsync(countryCode, prefix, number, startDate, endDate, page, pageSize);
                return Ok(new TransactionsDto { Transactions = res.Item1, Count = res.Item2 });
            }

            return Ok(new TransactionsDto());
        }

        [SwaggerOperation(Summary = "Returns transactions info")]
        [HttpGet("transactions/mail/{email}")]
        [SwaggerResponse(200, "Records was returned successfully", typeof(Transaction[]))]
        public async Task<IActionResult> GetTransactionsWithEmail(string mail, DateTime startDate, DateTime endDate, [Range(0, 100)] int page = 0, [Range(1, 100)] int pageSize = 20)
        {
            var res = await _repositoryManager.Transactions.GetMailMessagesWithMailAsync(string.Empty, mail, startDate, endDate, page, pageSize);
            return Ok(new TransactionsDto { Transactions = res.Item1, Count = res.Item2 });
        }

        [SwaggerOperation(Summary = "Returns transactions info")]
        [HttpGet("transactions/customer/{customerNo}/{messageType}")]
        [SwaggerResponse(200, "Records was returned successfully", typeof(Transaction[]))]
        public async Task<IActionResult> GetTransactionsWithCustomerNo(ulong customerNo, int messageType, int smsType, DateTime startDate, DateTime endDate, [Range(0, 100)] int page = 0, [Range(1, 100)] int pageSize = 20)
        {
            if (messageType == 1)
            {
                if (smsType == 1)
                {
                    var res = await _repositoryManager.Transactions.GetOtpMessagesWithCustomerNoAsync(customerNo, startDate, endDate, page, pageSize);
                    return Ok(new TransactionsDto { Transactions = res.Item1, Count = res.Item2 });
                }
                if (smsType == 2)
                {
                    var res = await _repositoryManager.Transactions.GetTransactionalSmsMessagesWithCustomerNoAsync(customerNo, startDate, endDate, page, pageSize);
                    return Ok(new TransactionsDto { Transactions = res.Item1, Count = res.Item2 });
                }

                return Ok(new TransactionsDto());
            }
            if (messageType == 2)
            {
                var res = await _repositoryManager.Transactions.GetMailMessagesWithCustomerNoAsync(string.Empty, customerNo, startDate, endDate, page, pageSize);
                return Ok(new TransactionsDto { Transactions = res.Item1, Count = res.Item2 });
            }
            if (messageType == 3)
            {
                var res = await _repositoryManager.Transactions.GetPushMessagesWithCustomerNoAsync(string.Empty, customerNo, startDate, endDate, page, pageSize);
                return Ok(new TransactionsDto { Transactions = res.Item1, Count = res.Item2 });
            }

            return Ok(new TransactionsDto());
        }

        [SwaggerOperation(Summary = "Returns transactions info")]
        [HttpGet("transactions/citizen/{citizenshipNo}/{messageType}")]
        [SwaggerResponse(200, "Records was returned successfully", typeof(Transaction[]))]
        public async Task<IActionResult> GetTransactionsWithCitizenshipNo(string citizenshipNo, int messageType, int smsType, DateTime startDate, DateTime endDate, [Range(0, 100)] int page = 0, [Range(1, 100)] int pageSize = 20)
        {
            if (messageType == 1)
            {
                if (smsType == 1)
                {
                    var res = await _repositoryManager.Transactions.GetOtpMessagesWithCitizenshipNoAsync(citizenshipNo, startDate, endDate, page, pageSize);
                    return Ok(new TransactionsDto { Transactions = res.Item1, Count = res.Item2 });
                }

                if (smsType == 2)
                {
                    var res = await _repositoryManager.Transactions.GetTransactionalSmsMessagesWithCitizenshipNoAsync(citizenshipNo, startDate, endDate, page, pageSize);
                    return Ok(new TransactionsDto { Transactions = res.Item1, Count = res.Item2 });
                }
                return Ok(new TransactionsDto());
            }
            if (messageType == 2)
            {
                var res = await _repositoryManager.Transactions.GetMailMessagesWithCitizenshipNoAsync(string.Empty,citizenshipNo, startDate, endDate, page, pageSize);
                return Ok(new TransactionsDto { Transactions = res.Item1, Count = res.Item2 });
            }
            if (messageType == 3)
            {
                var res = await _repositoryManager.Transactions.GetPushMessagesWithCitizenshipNoAsync(string.Empty, citizenshipNo, startDate, endDate, page, pageSize);
                return Ok(new TransactionsDto { Transactions = res.Item1, Count = res.Item2 });
            }

            return Ok(new List<Transaction>());
        }



        [SwaggerOperation(Summary = "Returns blacklist info")]
        [HttpGet("blacklists/customer/{customerNo}")]
        [SwaggerResponse(200, "Records was returned successfully", typeof(BlackListEntriesDto[]))]
        public async Task<IActionResult> GetBlackListEntriesWithCustomerNo(ulong customerNo, [Range(0, 100)] int page = 0, [Range(1, 100)] int pageSize = 20)
        {
            var res = await _repositoryManager.BlackListEntries.GetBlackListByCustomerNoAsync(customerNo, page, pageSize);
            return Ok(new BlackListEntriesDto { BlackListEntries = res.Item1, Count = res.Item2 });
        }
        [SwaggerOperation(Summary = "Returns blacklist info")]
        [HttpGet("blacklists/phone/{countryCode}/{prefix}/{number}")]
        [SwaggerResponse(200, "Records was returned successfully", typeof(BlackListEntriesDto[]))]
        public async Task<IActionResult> GetBlackListEntriesWithPhone(int countryCode, int prefix, int number, [Range(0, 100)] int page = 0, [Range(1, 100)] int pageSize = 20)
        {
            var res = await _repositoryManager.BlackListEntries.GetBlackListByPhoneAsync(countryCode, prefix, number, page, pageSize);
            return Ok(new BlackListEntriesDto { BlackListEntries = res.Item1, Count = res.Item2 });
        }

        [SwaggerOperation(Summary = "User Control")]
        [HttpGet("user/control/{userName}")]
        [SwaggerResponse(200, "Records was returned successfully", typeof(bool))]
        public async Task<IActionResult> GetUserControl(string userName)
        {

            UserSettingsModel user = _userSettings.Users.Where(a => a.UserName == userName).FirstOrDefault();
            if (user == null)
                return Ok(null);
            else
            {

                return Ok(user.Pages);
            }
        }
        
    }
}
