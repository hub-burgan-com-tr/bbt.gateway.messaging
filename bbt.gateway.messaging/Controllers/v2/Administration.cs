using bbt.gateway.common.Extensions;
using bbt.gateway.common.Models;
using bbt.gateway.common.Models.v2;
using bbt.gateway.common.Repositories;
using bbt.gateway.messaging.Authorization;
using bbt.gateway.messaging.Workers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Controllers.v2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public class Administration : ControllerBase
    {
        private readonly HeaderManager _headerManager;
        private readonly OperatorManager _operatorManager;
        private readonly IRepositoryManager _repositoryManager;
        private readonly CodecSender _codecSender;
        private readonly dEngageSender _dEngageSender;
        private readonly OtpSender _otpSender;
        public Administration(HeaderManager headerManager, OperatorManager operatorManager,
            IRepositoryManager repositoryManager,
            CodecSender codecSender, dEngageSender dEngageSender, OtpSender otpSender)
        {
            _headerManager = headerManager;
            _operatorManager = operatorManager;
            _repositoryManager = repositoryManager;
            _codecSender = codecSender;
            _dEngageSender = dEngageSender;
            _otpSender = otpSender;
        }

        [SwaggerOperation(
           Summary = "Returns Sms Counts And Success Rate",
           Description = "Returns Sms Counts And Success Rate."
           )]
        [HttpGet("Report/Sms")]
        [ApiExplorerSettings(IgnoreApi = true)]

        public async Task<IActionResult> SmsReportAsync([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var smsReportResponse = new List<OperatorReport>();

            smsReportResponse.Add( await GetOperatorInfo(startDate, endDate, OperatorType.Turkcell, true, false));
            smsReportResponse.Add(await GetOperatorInfo(startDate, endDate, OperatorType.Vodafone, true, false));
            smsReportResponse.Add(await GetOperatorInfo(startDate, endDate, OperatorType.TurkTelekom, true, false));
            smsReportResponse.Add((await GetOperatorInfo(startDate, endDate, OperatorType.dEngageBurgan, false, true) + await GetOperatorInfo(startDate, endDate, OperatorType.dEngageOn, false, true)));
            smsReportResponse.Add(await GetOperatorInfo(startDate, endDate, OperatorType.Codec, false, true));

            return Ok(smsReportResponse);
        }

        [SwaggerOperation(
           Summary = "Check Fast Sms Message Status",
           Description = "Check Fast Sms Delivery Status."
           )]
        [HttpPost("sms/check-message")]
        [ApiExplorerSettings(IgnoreApi = true)]

        public async Task<IActionResult> CheckMessageStatus([FromBody] CheckFastSmsRequest data)
        {

            if (ModelState.IsValid)
            {
                if (data.Operator == common.Models.OperatorType.Codec)
                {
                    var res = await _codecSender.CheckSms(data);
                    return Ok(res);
                }
                if (data.Operator == common.Models.OperatorType.dEngageOn ||
                    data.Operator == common.Models.OperatorType.dEngageBurgan)
                {
                    var res = await _dEngageSender.CheckSms(data);
                    return Ok(res);
                }
                return BadRequest("Unknown Operator");
            }
            else
            {
                return BadRequest(ModelState);
            }


        }

        [SwaggerOperation(
           Summary = "Check Fast Sms Message Status",
           Description = "Check Fast Sms Delivery Status."
           )]
        [HttpPost("otp/check-message")]
        [ApiExplorerSettings(IgnoreApi = true)]

        public async Task<IActionResult> CheckOtpMessageStatus([FromBody] common.Models.v2.CheckSmsRequest data)
        {

            if (ModelState.IsValid)
            {
                var res = await _otpSender.CheckMessage(data.MapTo<common.Models.CheckSmsRequest>());
                return Ok(res);

            }
            else
            {
                return BadRequest(ModelState);
            }


        }

        [SwaggerOperation(
           Summary = "Check Mail Message Status",
           Description = "Check Mail Delivery Status."
           )]
        [HttpPost("email/check-message")]
        //[ApiExplorerSettings(IgnoreApi = true)]

        public async Task<IActionResult> CheckMessageStatus([FromBody] CheckMailStatusRequest data)
        {

            if (ModelState.IsValid)
            {
                if (data.Operator == OperatorType.dEngageOn ||
                    data.Operator == OperatorType.dEngageBurgan)
                {
                    var res = await _dEngageSender.CheckMail(data);
                    return Ok(res);
                }
                return BadRequest("Unknown Operator");
            }
            else
            {
                return BadRequest(ModelState);
            }

        }

        [SwaggerOperation(
          Summary = "Check Message Status",
          Description = "Check Message Status."
          )]
        [HttpGet("transaction/check")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> CheckTransactionStatus([FromQuery] Guid transactionId)
        {
            var transaction = await _repositoryManager.Transactions.GetWithIdAsNoTrackingAsync(transactionId);
            if (transaction == null)
                return NotFound("Transaction Not Found");

            if (transaction.TransactionType == TransactionType.TransactionalSms
                || transaction.TransactionType == TransactionType.TransactionalTemplatedSms)
            { 
            
            }

            if (transaction.TransactionType == TransactionType.Otp)
            {

            }

            if (transaction.TransactionType == TransactionType.TransactionalMail
                || transaction.TransactionType == TransactionType.TransactionalTemplatedMail)
            {
                if (transaction.MailRequestLog == null)
                {
                    return Ok(new CheckTransactionStatusResponse { Status = TransactionStatus.NotDelivered , Detail = "MailRequestLog Not Found"});
                }

                if (transaction.MailRequestLog?.ResponseLogs == null)
                {
                    return Ok(new CheckTransactionStatusResponse { Status = TransactionStatus.NotDelivered, Detail = "MailResponseLog Not Found" });
                }

                var responseLog = transaction.MailRequestLog.ResponseLogs.FirstOrDefault();
                if (responseLog == null)
                {
                    return Ok(new CheckTransactionStatusResponse { Status = TransactionStatus.NotDelivered, Detail = "MailResponseLog Not Found" });
                }

                if (responseLog.Status == "Delivered")
                {
                    return Ok(new CheckTransactionStatusResponse { Status = TransactionStatus.Delivered, Detail = "E-Mail Delivered Successfully" });
                }

                if (responseLog.Status == "NotDelivered")
                {
                    return Ok(new CheckTransactionStatusResponse { Status = TransactionStatus.NotDelivered, Detail = "E-Mail Not Delivered" });
                }

                if (responseLog.Status == "Waiting" || String.IsNullOrWhiteSpace(responseLog.Status))
                {
                    return Ok(new CheckTransactionStatusResponse { Status = TransactionStatus.Waiting, Detail = "Report is Not Ready" });
                }
            }

            return BadRequest();
        }

        [Authorize]
        [SwaggerOperation(Summary = "Returns content headers configuration",
            Tags = new[] { "Header Management" })]
        [HttpGet("headers")]
        [SwaggerResponse(200, "Headers is returned successfully", typeof(Header[]))]
        public async Task<IActionResult> GetHeaders([FromQuery][Range(0, 100)] int page = 0, [FromQuery][Range(1, 100)] int pageSize = 20)
        {
            return Ok(await _headerManager.Get(page, pageSize));
        }

        [SwaggerOperation(Summary = "Save or update header configuration",
            Tags = new[] { "Header Management" })]
        [HttpPost("headers")]
        [SwaggerRequestExample(typeof(HeaderRequest), typeof(AddHeaderRequestExampleFilter))]
        [SwaggerResponse(200, "Header is saved successfully", typeof(Header[]))]
        public async Task<IActionResult> SaveHeader([FromBody] Header data)
        {
            await _headerManager.Save(data);
            return Ok();
        }

        [SwaggerOperation(Summary = "Deletes header configuration",
            Tags = new[] { "Header Management" })]
        [HttpDelete("headers/{id}")]
        [SwaggerResponse(200, "Header is deleted successfully", typeof(void))]
        public async Task<IActionResult> DeleteHeader(Guid id)
        {
            await _headerManager.Delete(id);
            return Ok();
        }

        [SwaggerOperation(Summary = "Returns operator configurations",
            Tags = new[] { "Operator Management" })]
        [HttpGet("operators")]
        [SwaggerResponse(200, "Operators was returned successfully", typeof(Operator[]))]
        public async Task<IActionResult> GetOperators()
        {
            return Ok(await _operatorManager.Get());
        }

        [SwaggerOperation(Summary = "Updated operator configuration",
            Tags = new[] { "Operator Management" })]
        [HttpPost("operators")]
        [SwaggerResponse(200, "operator has saved successfully", typeof(void))]
        public async Task<IActionResult> SaveOperator([FromBody] Operator data)
        {
            await _operatorManager.Save(data);
            return Ok();
        }

        [SwaggerOperation(Summary = "Resolve Blacklist Entry",
            Tags = new[] { "Phone Management" })]
        [HttpPost("blacklist/resolve")]
        [SwaggerResponse(200, "Record was updated successfully", typeof(void))]

        public async Task<IActionResult> ResolveBlacklistRecordAsync(ResolveBlacklistEntryFromPhoneRequest resolveBlacklistEntryFromPhoneRequest)
        {
            var blacklistRecord = await _repositoryManager.BlackListEntries.GetLastBlacklistEntry(
             Convert.ToInt32(resolveBlacklistEntryFromPhoneRequest.Phone.CountryCode),
             Convert.ToInt32(resolveBlacklistEntryFromPhoneRequest.Phone.Prefix),
             Convert.ToInt32(resolveBlacklistEntryFromPhoneRequest.Phone.Number));

            if (blacklistRecord != null)
            {
                if (blacklistRecord.Status == BlacklistStatus.NotResolved)
                {
                    blacklistRecord.Status = BlacklistStatus.Resolved;
                    blacklistRecord.Source = "BbtGatewayMessaging";
                    blacklistRecord.Reason = resolveBlacklistEntryFromPhoneRequest.Reason;
                    blacklistRecord.ResolvedAt = DateTime.Now;
                    blacklistRecord.ResolvedBy = resolveBlacklistEntryFromPhoneRequest.ResolvedBy;

                    await _repositoryManager.SaveChangesAsync();

                    if (blacklistRecord.SmsId > 0)
                    {
                        var oldBlacklistRecord = await _repositoryManager.DirectBlacklists.FirstOrDefaultAsync(b => b.SmsId == blacklistRecord.SmsId);
                        oldBlacklistRecord.VerifyDate = DateTime.Now;
                        oldBlacklistRecord.VerifiedBy = resolveBlacklistEntryFromPhoneRequest.ResolvedBy.Name;
                        oldBlacklistRecord.Explanation = resolveBlacklistEntryFromPhoneRequest.Reason;
                        oldBlacklistRecord.IsVerified = true;
                        await _repositoryManager.SaveSmsBankingChangesAsync();
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                var oldBlacklistRecord = await _repositoryManager.DirectBlacklists.GetLastBlacklistEntry(resolveBlacklistEntryFromPhoneRequest.Phone.ToOldBlacklistNumber());
                if (oldBlacklistRecord != null)
                {
                    oldBlacklistRecord.VerifyDate = DateTime.Now;
                    oldBlacklistRecord.VerifiedBy = resolveBlacklistEntryFromPhoneRequest.ResolvedBy.Name;
                    oldBlacklistRecord.Explanation = resolveBlacklistEntryFromPhoneRequest.Reason;
                    oldBlacklistRecord.IsVerified = true;
                    await _repositoryManager.SaveSmsBankingChangesAsync();
                    return Ok();
                }
                else
                {
                    return NotFound();
                }
            }
            return BadRequest();
        }

        [SwaggerOperation(Summary = "Returns phone activities",
            Tags = new[] { "Phone Management" })]
        [HttpGet("phone-monitor/{countryCode}/{prefix}/{number}")]
        [SwaggerResponse(200, "Records was returned successfully", typeof(PhoneConfiguration))]

        public async Task<IActionResult> GetPhoneMonitorRecords(int countryCode, int prefix, int number, int count)
        {
            return Ok(await _repositoryManager.PhoneConfigurations.GetWithRelatedLogsAndBlacklistEntriesAsync(countryCode, prefix, number, count));
        }

        [SwaggerOperation(Summary = "Returns phone has active blacklist or not",
            Tags = new[] { "Phone Management" })]
        [HttpGet("check-blacklist/{countryCode}/{prefix}/{number}")]
        [SwaggerResponse(200, "Phone has active blacklist", typeof(void))]
        [SwaggerResponse(404, "Phone has not active blacklist", typeof(void))]
        public async Task<IActionResult> CheckPhoneBlacklistStatus(string countryCode, string prefix, string number)
        {
            var blacklistRecord = await _repositoryManager.BlackListEntries.GetLastBlacklistRecord(
                Convert.ToInt32(countryCode),
                Convert.ToInt32(prefix),
                Convert.ToInt32(number)
            );

            if (blacklistRecord == null)
                return NotFound();
            

            if (blacklistRecord.Status == BlacklistStatus.NotResolved)
                return Ok();

            return NotFound();
        }

        [SwaggerOperation(Summary = "Returns phone blacklist records",
            Tags = new[] { "Phone Management" })]
        [HttpGet("blacklists/{countryCode}/{prefix}/{number}")]
        [SwaggerResponse(200, "Records was returned successfully", typeof(BlackListEntry))]

        public async Task<IActionResult> GetPhoneBlacklistRecords(string countryCode, string prefix, string number, [Range(0, 100)] int page = 0, [Range(1, 100)] int pageSize = 20)
        {
            return Ok(await _repositoryManager.BlackListEntries
                .GetWithLogsAsync(Convert.ToInt32(countryCode), Convert.ToInt32(prefix), Convert.ToInt32(number), page, pageSize));
        }

        [SwaggerOperation(Summary = "Adds phone to blacklist records",
            Tags = new[] { "Phone Management" })]
        [HttpPost("blacklists")]
        [SwaggerResponse(201, "Record was created successfully", typeof(void))]
        public async Task<IActionResult> AddPhoneToBlacklist([FromBody] AddPhoneToBlacklistRequest data)
        {

            Guid newOtpBlackListEntryId = Guid.NewGuid();

            var config = (await _repositoryManager.PhoneConfigurations
                .FindAsync(c => c.Phone.CountryCode == Convert.ToInt32(data.Phone.CountryCode) && 
                c.Phone.Prefix == Convert.ToInt32(data.Phone.Prefix) &&
                c.Phone.Number == Convert.ToInt32(data.Phone.Number)))
                .FirstOrDefault();


            if (config == null)
            {
                config = new PhoneConfiguration
                {
                    Phone = new common.Models.Phone { CountryCode = Convert.ToInt32(data.Phone.CountryCode) , Prefix = Convert.ToInt32(data.Phone.Prefix),
                    Number = Convert.ToInt32(data.Phone.Number)},
                    Logs = new List<PhoneConfigurationLog>(),
                    BlacklistEntries = new List<BlackListEntry>()
                };

                config.Logs.Add(new PhoneConfigurationLog
                {
                    Type = "Initialization",
                    Action = "Blacklist Entry",
                    CreatedBy = data.Process,
                    RelatedId = newOtpBlackListEntryId
                });

                await _repositoryManager.PhoneConfigurations.AddAsync(config);
            }

            var now = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Prod" ? DateTime.Now : (data.CreatedAt ?? DateTime.Now);

            var oldOtpBlacklistEntry = new SmsDirectBlacklist
            {
                GsmNumber = "+" + data.Phone.CountryCode + data.Phone.Prefix + data.Phone.Number,
                Explanation = "Reason",
                IsVerified = false,
                RecordDate = now,
                TryCount = 0,
                VerifiedBy = null,
                VerifyDate = null
            };

            await _repositoryManager.DirectBlacklists.AddAsync(oldOtpBlacklistEntry);
            await _repositoryManager.SaveSmsBankingChangesAsync();

            var newOtpBlackListEntry = new BlackListEntry
            {
                Id = newOtpBlackListEntryId,
                PhoneConfiguration = config,
                PhoneConfigurationId = config.Id,
                Reason = data.Reason,
                Source = data.Source,
                ValidTo = DateTime.UtcNow.AddDays(data.Days),
                CreatedBy = data.Process,
                CreatedAt = now,
                SmsId = oldOtpBlacklistEntry.SmsId
            };

            await _repositoryManager.BlackListEntries.AddAsync(newOtpBlackListEntry);

            await _repositoryManager.SaveChangesAsync();

            return Created("", newOtpBlackListEntryId);
        }

        [SwaggerOperation(Summary = "Resolve blacklist item",
            Tags = new[] { "Phone Management" })]
        [HttpPatch("blacklists/{blacklist-entry-id}/resolve")]
        [SwaggerResponse(201, "Record was updated successfully", typeof(void))]
        [SwaggerResponse(404, "Record not found", typeof(void))]
        public async Task<IActionResult> ResolveBlacklistItem([FromRoute(Name = "blacklist-entry-id")] Guid entryId, [FromBody] ResolveBlacklistEntryRequest data)
        {
            var config = await _repositoryManager.BlackListEntries.FirstOrDefaultAsync(b => b.Id == entryId);
            if (config == null)
                return NotFound(entryId);
            var resolvedAt = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Prod" ? DateTime.Now : (data.ResolvedAt ?? DateTime.Now);
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
                oldBlacklistEntry.Explanation = data.Reason;
                await _repositoryManager.SaveSmsBankingChangesAsync();
            }

            await _repositoryManager.SaveChangesAsync();

            return StatusCode(201);
        }

        [SwaggerOperation(Summary = "Deletes phone activities",
            Tags = new[] { "Phone Management" })]
        [HttpDelete("phone-monitor/{countryCode}/{prefix}/{number}")]
        [SwaggerResponse(204, "Records was deleted successfully", typeof(void))]

        public async Task<IActionResult> GetPhoneMonitorRecords(int countryCode, int prefix, int number)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Prod" &&
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Production")
            {
                var phoneConfigurations = await _repositoryManager.PhoneConfigurations.FindAsync(p =>
                p.Phone.CountryCode == countryCode &&
                p.Phone.Prefix == prefix &&
                p.Phone.Number == number
            );

                foreach (var phoneConfiguration in phoneConfigurations)
                {
                    await _repositoryManager.PhoneConfigurations.DeletePhoneConfiguration(phoneConfiguration.Id);
                }


                return NoContent();
            }
            return Forbid();
        }

        [SwaggerOperation(Summary = "Adds phone to whitelist",
            Description = "<div>Phone Number Has To Be Added To Whitelist To Receives Sms On Test Environment</div>",
            Tags = new[] { "Whitelist Management" })]
        [HttpPost("whitelist/phone")]
        [SwaggerResponse(201, "Record was created successfully", typeof(void))]
        [SwaggerResponse(400, "Phone Number Field Is Mandatory", typeof(void))]
        [SwaggerResponse(409, "Phone Number Is Already Exists In Whitelist", typeof(void))]
        public async Task<IActionResult> AddPhoneToWhitelist([FromBody] AddPhoneToWhitelistRequest data)
        {
            if (data.Phone == null)
            {
                return BadRequest("Phone Number Field Is Mandatory");
            }

            if ((await _repositoryManager.Whitelist.FindAsync(w => (w.Phone.CountryCode == Convert.ToInt32(data.Phone.CountryCode))
             && (w.Phone.Prefix == Convert.ToInt32(data.Phone.Prefix))
             && (w.Phone.Number == Convert.ToInt32(data.Phone.Number)))).FirstOrDefault() != null)
                return BadRequest("Phone Number Is Already Exist");

            var whitelistRecord = new WhiteList()
            {
                CreatedBy = data.CreatedBy.MapTo<common.Models.Process>(),
                IpAddress = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                    ?? HttpContext.Connection.RemoteIpAddress.ToString(),
                Phone = new common.Models.Phone()
                {
                    CountryCode = Convert.ToInt32(data.Phone.CountryCode),
                    Prefix = Convert.ToInt32(data.Phone.Prefix),
                    Number = Convert.ToInt32(data.Phone.Number)
                }
            };

            await _repositoryManager.Whitelist.AddAsync(whitelistRecord);
            await _repositoryManager.SaveChangesAsync();

            return Created("", whitelistRecord.Id);
        }

        [SwaggerOperation(Summary = "Adds mail to whitelist",
            Description = "<div>Mail Address Has To Be Added To Whitelist To Receives E-Mail On Test Environment</div>",
            Tags = new[] { "Whitelist Management" })]
        [HttpPost("whitelist/email")]
        [SwaggerResponse(201, "Record was created successfully", typeof(void))]
        [SwaggerResponse(400, "Phone Number Field Is Mandatory", typeof(void))]
        [SwaggerResponse(409, "Phone Number Is Already Exists In Whitelist", typeof(void))]
        public async Task<IActionResult> AddMailToWhitelist([FromBody] AddMailToWhitelistRequest data)
        {
            if (data.Email == null)
            {
                return BadRequest("Email Field Is Mandatory");
            }

            if ((await _repositoryManager.Whitelist.FindAsync(w => w.Mail == data.Email)).FirstOrDefault() != null)
                return BadRequest("Email Is Already Exist");

            var whitelistRecord = new WhiteList()
            {
                CreatedBy = data.CreatedBy.MapTo<common.Models.Process>(),
                IpAddress = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                    ?? HttpContext.Connection.RemoteIpAddress.ToString(),
                Mail = data.Email
            };

            await _repositoryManager.Whitelist.AddAsync(whitelistRecord);
            await _repositoryManager.SaveChangesAsync();

            return Created("", whitelistRecord.Id);
        }

        [SwaggerOperation(Summary = "Adds Citizenshipno to whitelist",
            Description = "<div>Citizenshipno Has To Be Added To Whitelist To Receives Pushs On Test Environment</div>",
            Tags = new[] { "Whitelist Management" })]
        [HttpPost("whitelist/push")]
        [SwaggerResponse(201, "Record was created successfully", typeof(void))]
        [SwaggerResponse(400, "Citizenshipno Field Is Mandatory", typeof(void))]
        [SwaggerResponse(409, "Citizenshipno Is Already Exists In Whitelist", typeof(void))]
        public async Task<IActionResult> AddCitizenshipnoToWhitelist([FromBody] AddCitizenshipnoToWhitelistRequest data)
        {
            if (data.CitizenshipNo == null)
            {
                return BadRequest("Citizenshipno Field Is Mandatory");
            }

            if ((await _repositoryManager.Whitelist.FindAsync(w => w.ContactId == data.CitizenshipNo)).FirstOrDefault() != null)
                return BadRequest("Citizenshipno Is Already Exist");

            var whitelistRecord = new WhiteList()
            {
                CreatedBy = data.CreatedBy.MapTo<common.Models.Process>(),
                IpAddress = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                    ?? HttpContext.Connection.RemoteIpAddress.ToString(),
                ContactId = data.CitizenshipNo
            };

            await _repositoryManager.Whitelist.AddAsync(whitelistRecord);
            await _repositoryManager.SaveChangesAsync();

            return Created("", whitelistRecord.Id);
        }

        [SwaggerOperation(Summary = "Deletes Phone Number From Whitelist configuration",
            Tags = new[] { "Whitelist Management" })]
        [HttpDelete("whitelist/phone")]
        [SwaggerResponse(200, "Whitelist record is deleted successfully", typeof(void))]
        public async Task<IActionResult> DeletePhoneFromWhitelist(PhoneString phone)
        {
            var recordsToDelete = await _repositoryManager.Whitelist.FindAsync(w => (w.Phone.CountryCode == Convert.ToInt32(phone.CountryCode))
              && (w.Phone.Prefix == Convert.ToInt32(phone.Prefix))
              && (w.Phone.Number == Convert.ToInt32(phone.Number)));

            if (!recordsToDelete.Any())
                return BadRequest("There is no record for given phone number");

            foreach (WhiteList whitelist in recordsToDelete)
            {
                _repositoryManager.Whitelist.Remove(whitelist);
            }

            await _repositoryManager.SaveChangesAsync();
            return Ok();
        }


        [SwaggerOperation(Summary = "Deletes Mail Address From Whitelist configuration",
            Tags = new[] { "Whitelist Management" })]
        [HttpDelete("whitelist/mail")]
        [SwaggerResponse(200, "Whitelist record is deleted successfully", typeof(void))]
        public async Task<IActionResult> DeleteMailFromWhitelist(string Mail)
        {
            var recordsToDelete = await _repositoryManager.Whitelist.FindAsync(w => w.Mail == Mail);

            if (!recordsToDelete.Any())
                return BadRequest("There is no record for given mail address");

            foreach (WhiteList whitelist in recordsToDelete)
            {
                _repositoryManager.Whitelist.Remove(whitelist);
            }

            await _repositoryManager.SaveChangesAsync();
            return Ok();
        }

        [SwaggerOperation(Summary = "Deletes Citizenshipno From Whitelist configuration",
            Tags = new[] { "Whitelist Management" })]
        [HttpDelete("whitelist/push")]
        [SwaggerResponse(200, "Whitelist record is deleted successfully", typeof(void))]
        public async Task<IActionResult> DeletePushFromWhitelist(string CitizenshipNo)
        {
            var recordsToDelete = await _repositoryManager.Whitelist.FindAsync(w => w.ContactId == CitizenshipNo);

            if (!recordsToDelete.Any())
                return BadRequest("There is no record for given citizenshipNo");

            foreach (WhiteList whitelist in recordsToDelete)
            {
                _repositoryManager.Whitelist.Remove(whitelist);
            }

            await _repositoryManager.SaveChangesAsync();
            return Ok();
        }

        [SwaggerOperation(Summary = "Returns phone's whitelist status",
            Tags = new[] { "Whitelist Management" })]
        [HttpGet("whitelist/check/phone")]
        [SwaggerResponse(200, "Phone is in whitelist", typeof(void))]
        [SwaggerResponse(404, "Phone is not in whitelist", typeof(void))]
        public async Task<IActionResult> CheckPhone(string CountryCode, string Prefix, string Number)
        {
            if ((await _repositoryManager.Whitelist.FindAsync(w => (w.Phone.CountryCode == Convert.ToInt32(CountryCode))
               && (w.Phone.Prefix == Convert.ToInt32(Prefix))
               && (w.Phone.Number == Convert.ToInt32(Number)))).FirstOrDefault() != null)
                return Ok();
            else
                return NotFound();
        }

        [SwaggerOperation(Summary = "Returns E-mail's whitelist status",
            Tags = new[] { "Whitelist Management" })]
        [HttpGet("whitelist/check/email")]
        [SwaggerResponse(200, "E-Mail is in whitelist", typeof(void))]
        [SwaggerResponse(404, "E-Mail is not in whitelist", typeof(void))]
        public async Task<IActionResult> CheckMail(string email)
        {
            if ((await _repositoryManager.Whitelist.FindAsync(w => w.Mail == email)).FirstOrDefault() != null)
                return Ok();
            else
                return NotFound();
        }

        [SwaggerOperation(Summary = "Returns CitizenshipNo's whitelist status",
            Tags = new[] { "Whitelist Management" })]
        [HttpGet("whitelist/check/push")]
        [SwaggerResponse(200, "CitizensipNo is in whitelist", typeof(void))]
        [SwaggerResponse(404, "CitizensipNo is not in whitelist", typeof(void))]
        public async Task<IActionResult> CheckPush(string CitizenshipNo)
        {
            if ((await _repositoryManager.Whitelist.FindAsync(w => w.ContactId == CitizenshipNo)).FirstOrDefault() != null)
                return Ok();
            else
                return NotFound();
        }

        

        [SwaggerOperation(Summary = "Returns phones otp sending logs",
            Tags = new[] { "Phone Management" })]
        [HttpGet("otp-log/{countryCode}/{prefix}/{number}")]
        [SwaggerResponse(200, "Records was returned successfully", typeof(OtpRequestLog[]))]
        public async Task<IActionResult> GetOtpLog(int countryCode, int prefix, int number, [Range(0, 100)] int page = 0, [Range(1, 100)] int pageSize = 20)
        {
            return Ok(await _repositoryManager.OtpRequestLogs
                .GetWithResponseLogsAsync(countryCode, prefix, number, page, pageSize));
        }

        [SwaggerOperation(Summary = "Returns phones sms sending logs",
            Tags = new[] { "Phone Management" })]
        [HttpGet("sms-log/{countryCode}/{prefix}/{number}")]
        [SwaggerResponse(200, "Records was returned successfully", typeof(SmsResponseLog[]))]
        public async Task<IActionResult> GetSmsLog(int countryCode, int prefix, int number, [Range(0, 100)] int page = 0, [Range(1, 100)] int pageSize = 20)
        {

            return Ok((await _repositoryManager.SmsRequestLogs
                .FindAsync(c => c.PhoneConfiguration.Phone.CountryCode == countryCode && c.PhoneConfiguration.Phone.Prefix == prefix && c.PhoneConfiguration.Phone.Number == number))
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToArray());
        }

        [SwaggerOperation(Summary = "Returns Generated Template Message Associated With Transaction",
            Tags = new[] { "Transaction Management" })]
        [HttpGet("transaction/{txnId}/generatedMessage")]
        [SwaggerResponse(200, "Records was returned successfully", typeof(GeneratedMessage))]

        public async Task<IActionResult> GetPhoneBlacklistRecords(Guid txnId)
        {
            var transaction = await _repositoryManager.Transactions.GetWithIdAsNoTrackingAsync(txnId);
            if (transaction == null)
                return NotFound();

            if (transaction.TransactionType != TransactionType.TransactionalTemplatedSms &&
               transaction.TransactionType != TransactionType.TransactionalTemplatedMail &&
               transaction.TransactionType != TransactionType.TransactionalTemplatedPush)
                return BadRequest("Transaction Type is Not Templated");

            if (transaction.TransactionType == TransactionType.TransactionalTemplatedSms)
                return Ok(new GeneratedMessage { Content = transaction.SmsRequestLog.content });

            if (transaction.TransactionType == TransactionType.TransactionalTemplatedMail)
                return Ok(new GeneratedMessage { Content = transaction.MailRequestLog.content });

            if (transaction.TransactionType == TransactionType.TransactionalTemplatedPush)
                return Ok(new GeneratedMessage { Content = transaction.PushNotificationRequestLog.Content });

            return BadRequest();
        }

        [SwaggerOperation(Summary = "Returns transactions info",
            Tags = new[] { "Transaction Management" })]
        [HttpGet("transactions/phone/{countryCode}/{prefix}/{number}")]
        [SwaggerResponse(200, "Records was returned successfully", typeof(Transaction[]))]
        public async Task<IActionResult> GetTransactionsWithPhone(int countryCode, int prefix, int number, string createdName, int smsType, DateTime startDate, DateTime endDate, [Range(0, 100)] int page = 0, [Range(1, 100)] int pageSize = 20)
        {
            if (createdName == null)
                createdName = string.Empty;
            if (smsType == 1)
            {
                var res = await _repositoryManager.Transactions.GetOtpMessagesWithPhoneByCreatedNameAsync(createdName, countryCode, prefix, number, startDate, endDate, page, pageSize);
                return Ok(new TransactionsDto { Transactions = res.Item1, Count = res.Item2 });
            }
            if (smsType == 2)
            {
                var res = await _repositoryManager.Transactions.GetTransactionalSmsMessagesWithPhoneByCreatedNameAsync(createdName, countryCode, prefix, number, startDate, endDate, page, pageSize);
                return Ok(new TransactionsDto { Transactions = res.Item1, Count = res.Item2 });
            }

            return Ok(new TransactionsDto());
        }

        [SwaggerOperation(Summary = "Returns transactions info by createdName",
            Tags = new[] { "Transaction Management" })]
        [HttpGet("transactions/createdName/phone/{createdName}/{countryCode}/{prefix}/{number}")]
        [SwaggerResponse(200, "Records was returned successfully", typeof(Transaction[]))]
        public async Task<IActionResult> GetTransactionsWithPhoneByCreatedName(string createdName, int countryCode, int prefix, int number, int smsType, DateTime startDate, DateTime endDate, [Range(0, 100)] int page = 0, [Range(1, 100)] int pageSize = 20)
        {
            if (smsType == 1)
            {
                var res = await _repositoryManager.Transactions.GetOtpMessagesWithPhoneByCreatedNameAsync(createdName, countryCode, prefix, number, startDate, endDate, page, pageSize);
                return Ok(new TransactionsDto { Transactions = res.Item1, Count = res.Item2 });
            }
            if (smsType == 2)
            {
                var res = await _repositoryManager.Transactions.GetTransactionalSmsMessagesWithPhoneByCreatedNameAsync(createdName, countryCode, prefix, number, startDate, endDate, page, pageSize);
                return Ok(new TransactionsDto { Transactions = res.Item1, Count = res.Item2 });
            }

            return Ok(new TransactionsDto());
        }

        [SwaggerOperation(Summary = "Returns transactions info",
            Tags = new[] { "Transaction Management" })]
        [HttpGet("transactions/mail/{email}")]
        [SwaggerResponse(200, "Records was returned successfully", typeof(Transaction[]))]
        public async Task<IActionResult> GetTransactionsWithEmail(string mail, string createdName, DateTime startDate, DateTime endDate, [Range(0, 100)] int page = 0, [Range(1, 100)] int pageSize = 20)
        {
            if (createdName == null)
                createdName = string.Empty;
            var res = await _repositoryManager.Transactions.GetMailMessagesWithMailAsync(createdName, mail, startDate, endDate, page, pageSize);
            return Ok(new TransactionsDto { Transactions = res.Item1, Count = res.Item2 });
        }

        [SwaggerOperation(Summary = "Returns transactions info",
            Tags = new[] { "Transaction Management" })]
        [HttpGet("transactions/customer/{customerNo}/{messageType}")]
        [SwaggerResponse(200, "Records was returned successfully", typeof(Transaction[]))]
        public async Task<IActionResult> GetTransactionsWithCustomerNo(ulong customerNo, string createdName, int messageType, int smsType, DateTime startDate, DateTime endDate, [Range(0, 100)] int page = 0, [Range(1, 100)] int pageSize = 20)
        {
            if (createdName == null)
                createdName = string.Empty;
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
                var res = await _repositoryManager.Transactions.GetMailMessagesWithCustomerNoAsync(createdName, customerNo, startDate, endDate, page, pageSize);
                return Ok(new TransactionsDto { Transactions = res.Item1, Count = res.Item2 });
            }
            if (messageType == 3)
            {
                var res = await _repositoryManager.Transactions.GetPushMessagesWithCustomerNoAsync(createdName, customerNo, startDate, endDate, page, pageSize);
                return Ok(new TransactionsDto { Transactions = res.Item1, Count = res.Item2 });
            }

            return Ok(new TransactionsDto());
        }
        [SwaggerOperation(Summary = "Returns transactions info by created name",
      Tags = new[] { "Transaction Management" })]
        [HttpGet("transactions/createdName/customer/{createdName}/{customerNo}/{messageType}")]
        [SwaggerResponse(200, "Records was returned successfully", typeof(Transaction[]))]
        public async Task<IActionResult> GetTransactionsWithCustomerNoByCreatedName(string createdName, ulong customerNo, int messageType, int smsType, DateTime startDate, DateTime endDate, [Range(0, 100)] int page = 0, [Range(1, 100)] int pageSize = 20)
        {
            if (messageType == 1)
            {
                if (smsType == 1)
                {
                    var res = await _repositoryManager.Transactions.GetOtpMessagesWithCustomerNoByCreatedNameAsync(createdName, customerNo, startDate, endDate, page, pageSize);
                    return Ok(new TransactionsDto { Transactions = res.Item1, Count = res.Item2 });
                }
                if (smsType == 2)
                {
                    var res = await _repositoryManager.Transactions.GetTransactionalSmsMessagesWithCustomerNoByCreatedNameAsync(createdName, customerNo, startDate, endDate, page, pageSize);
                    return Ok(new TransactionsDto { Transactions = res.Item1, Count = res.Item2 });
                }

                return Ok(new TransactionsDto());
            }
            return Ok(new TransactionsDto());
        }
        [SwaggerOperation(Summary = "Returns transactions info",
            Tags = new[] { "Transaction Management" })]
        [HttpGet("transactions/citizen/{citizenshipNo}/{messageType}")]
        [SwaggerResponse(200, "Records was returned successfully", typeof(Transaction[]))]
        public async Task<IActionResult> GetTransactionsWithCitizenshipNo(string citizenshipNo, string createdName, int messageType, int smsType, DateTime startDate, DateTime endDate, [Range(0, 100)] int page = 0, [Range(1, 100)] int pageSize = 20)
        {
            if (createdName == null)
                createdName = string.Empty;
            if (messageType == 1)
            {
                if (smsType == 1)
                {
                    var res = await _repositoryManager.Transactions.GetOtpMessagesWithCitizenshipNoByCreatedNameAsync(createdName, citizenshipNo, startDate, endDate, page, pageSize);
                    return Ok(new TransactionsDto { Transactions = res.Item1, Count = res.Item2 });
                }

                if (smsType == 2)
                {
                    var res = await _repositoryManager.Transactions.GetTransactionalSmsMessagesWithCitizenshipNoByCreatedNameAsync(createdName, citizenshipNo, startDate, endDate, page, pageSize);
                    return Ok(new TransactionsDto { Transactions = res.Item1, Count = res.Item2 });
                }
                return Ok(new TransactionsDto());
            }
            if (messageType == 2)
            {
                var res = await _repositoryManager.Transactions.GetMailMessagesWithCitizenshipNoAsync(createdName, citizenshipNo, startDate, endDate, page, pageSize);
                return Ok(new TransactionsDto { Transactions = res.Item1, Count = res.Item2 });
            }
            if (messageType == 3)
            {
                var res = await _repositoryManager.Transactions.GetPushMessagesWithCitizenshipNoAsync(createdName, citizenshipNo, startDate, endDate, page, pageSize);
                return Ok(new TransactionsDto { Transactions = res.Item1, Count = res.Item2 });
            }

            return Ok(new List<Transaction>());
        }

        [SwaggerOperation(Summary = "Returns transactions info by created name",
    Tags = new[] { "Transaction Management" })]
        [HttpGet("transactions/createdName/citizen/{createdName}/{citizenshipNo}/{messageType}")]
        [SwaggerResponse(200, "Records was returned successfully", typeof(Transaction[]))]
        public async Task<IActionResult> GetTransactionsWithCitizenshipNoByCreatedName(string createdName, string citizenshipNo, int messageType, int smsType, DateTime startDate, DateTime endDate, [Range(0, 100)] int page = 0, [Range(1, 100)] int pageSize = 20)
        {

            if (smsType == 1)
            {
                var res = await _repositoryManager.Transactions.GetOtpMessagesWithCitizenshipNoByCreatedNameAsync(createdName, citizenshipNo, startDate, endDate, page, pageSize);
                return Ok(new TransactionsDto { Transactions = res.Item1, Count = res.Item2 });
            }

            if (smsType == 2)
            {
                var res = await _repositoryManager.Transactions.GetTransactionalSmsMessagesWithCitizenshipNoByCreatedNameAsync(createdName, citizenshipNo, startDate, endDate, page, pageSize);
                return Ok(new TransactionsDto { Transactions = res.Item1, Count = res.Item2 });
            }
            return Ok(new TransactionsDto());


        }

        [SwaggerOperation(Summary = "Returns report for CreditReport",
            Tags = new[] { "Report Management" })]
        [HttpPost("CreditReport")]
        [SwaggerResponse(200, "Report is returned successfully", typeof(FileContentResult))]
        public async Task<FileContentResult> GetReport(IFormFile file)
        {
            using var reader = new StreamReader(file.OpenReadStream());
            string fileContent = await reader.ReadToEndAsync();
            using var stringReader = new StringReader(fileContent);
            using var stringReader2 = new StringReader(fileContent);
            var reportDate = await stringReader.ReadLineAsync();
            reportDate = reportDate.Trim();

            Dictionary<string, int> repeatedLinesOrder = new();

            string resultContent = String.Empty;
            resultContent += reportDate + "\r\n";

            string? line = await stringReader.ReadLineAsync();
            while (line != null)
            {
                var lineArray = line.Trim().Split("|");
                if (repeatedLinesOrder.ContainsKey(GetKeyName(lineArray)))
                {
                    repeatedLinesOrder[GetKeyName(lineArray)]++;
                }
                else
                {
                    repeatedLinesOrder.Add(
                            GetKeyName(lineArray),
                            0);
                }
                line = await stringReader.ReadLineAsync();
            }

            await stringReader2.ReadLineAsync();
            line = await stringReader2.ReadLineAsync();
            while (line != null)
            {
                var lineArray = line.Trim().Split("|");

                DateTime dt = DateTime.Now;
                var transactions = await _repositoryManager.Transactions.GetReportTransaction(
                    Convert.ToInt32(lineArray[1].Substring(5)),
                    reportDate,
                    lineArray[2]
                );

                if (transactions == null || transactions?.Count() == 0)
                {
                    resultContent += line + "|Rapor Bulunamadı\r\n";
                }
                if (transactions?.Count() == 1)
                {
                    var smsResponseLog = GetSmsResponseLog(transactions.FirstOrDefault());

                    resultContent += line +
                        await GetReportLine(
                            GetSmsResponseLog(transactions.FirstOrDefault()),
                            transactions.FirstOrDefault().SmsRequestLog);
                }
                if (transactions?.Count() > 1)
                {
                    resultContent += line +
                        await GetReportLine(
                            GetSmsResponseLog(
                                transactions.OrderByDescending(t => t.CreatedAt).ElementAt(repeatedLinesOrder[GetKeyName(lineArray)]--)),
                                transactions.FirstOrDefault().SmsRequestLog);
                }

                line = await stringReader2.ReadLineAsync();
            }


            return File(Encoding.UTF8.GetBytes(resultContent), "application/octet-stream", "rapor.csv");
        }

        private SmsResponseLog? GetSmsResponseLog(Transaction transaction)
        {
            if (transaction.SmsRequestLog == null)
                return null;
            if (transaction.SmsRequestLog.ResponseLogs == null)
                return null;
            if (transaction.SmsRequestLog.ResponseLogs.Count() == 0)
                return null;
            return transaction.SmsRequestLog.ResponseLogs.FirstOrDefault();
        }

        private string GetKeyName(string[] array)
        {
            return $"{array[1].Trim()}_{array[2].Trim()}".Trim();
        }

        private async Task<string> GetReportLine(SmsResponseLog smsResponseLog, SmsRequestLog smsRequestLog)
        {
            if (smsResponseLog != null)
            {
                SmsTrackingLog? trackingLog = null;
                if (smsResponseLog.Operator == OperatorType.Codec)
                {
                    trackingLog = await _codecSender.CheckSms(new CheckFastSmsRequest
                    {
                        Operator = smsResponseLog.Operator,
                        SmsRequestLogId = smsRequestLog.Id,
                        StatusQueryId = smsResponseLog.StatusQueryId
                    });
                }
                if (smsResponseLog.Operator == OperatorType.dEngageOn ||
                    smsResponseLog.Operator == OperatorType.dEngageBurgan)
                {
                    trackingLog = await _dEngageSender.CheckSms(new CheckFastSmsRequest
                    {
                        Operator = smsResponseLog.Operator,
                        SmsRequestLogId = smsRequestLog.Id,
                        StatusQueryId = smsResponseLog.StatusQueryId
                    });
                }

                if (trackingLog != null)
                {
                    return $"|{smsResponseLog?.CreatedAt.ToString(new System.Globalization.CultureInfo("tr-TR"))}|{trackingLog.Status}|{trackingLog.StatusReason}\r\n";

                }

                return $"|{smsResponseLog?.CreatedAt.ToString(new System.Globalization.CultureInfo("tr-TR"))}|Rapor Bulunamadı\r\n";
            }

            return $"|{smsResponseLog?.CreatedAt.ToString(new System.Globalization.CultureInfo("tr-TR"))}|Rapor Bulunamadı\r\n";

        }

        [SwaggerOperation(Summary = "Returns report for SmsExcelRapor with Phone",
           Tags = new[] { "Report Management" })]
        [HttpGet("report/phone/{countryCode}/{prefix}/{number}")]
        [SwaggerResponse(200, "Report is returned successfully", typeof(string))]
        [SwaggerResponse(400, "Excel oluşturulamadı", typeof(string))]
        public async Task<IActionResult> GetTransactionsExcelReportWithPhone(string createdName, int countryCode, int prefix, int number, int smsType, DateTime startDate, DateTime endDate, int pageSize)
        {
            if (createdName == null)
                createdName = string.Empty;

            string response = string.Empty;
            try
            {
                TransactionsDto dto = new TransactionsDto();
                if (smsType == 1)
                {
                    var res = await _repositoryManager.Transactions.GetOtpMessagesWithPhoneByCreatedNameAsync(createdName, countryCode, prefix, number, startDate, endDate, 0, pageSize);
                    dto = new TransactionsDto { Transactions = res.Item1, Count = res.Item2 };
                }
                if (smsType == 2)
                {
                    var res = await _repositoryManager.Transactions.GetTransactionalSmsMessagesWithPhoneByCreatedNameAsync(createdName, countryCode, prefix, number, startDate, endDate, 0, pageSize);
                    dto = new TransactionsDto { Transactions = res.Item1, Count = res.Item2 };
                }
                response = ExcelCreate(dto.Transactions, 1);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.ToString());
            }


            return Ok(response);
        }
        [SwaggerOperation(Summary = "Returns report for SmsExcelRapor with Phone",
        Tags = new[] { "Report Management" })]
        [HttpGet("report/mail/{mail}")]
        [SwaggerResponse(200, "Report is returned successfully", typeof(byte[]))]
        public async Task<IActionResult> GetTransactionsExcelReportWithMail(string mail, string createdName, DateTime startDate, DateTime endDate, int pageSize)
        {

            if (createdName == null)
                createdName = string.Empty;
            string response = string.Empty;
            try
            {
                var res = await _repositoryManager.Transactions.GetMailMessagesWithMailAsync(createdName, mail, startDate, endDate, 0, pageSize);
                TransactionsDto dto = new TransactionsDto { Transactions = res.Item1, Count = res.Item2 };


                response = ExcelCreate(dto.Transactions, 2);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }

            return Ok(response);
        }

        [SwaggerOperation(Summary = "Returns report for SmsExcelRapor with CustomerNo",
      Tags = new[] { "Report Management" })]
        [HttpGet("report/customer/{customerNo}/{messageType}")]
        [SwaggerResponse(200, "Report is returned successfully", typeof(byte[]))]
        public async Task<IActionResult> GetTransactionsExcelReportWithCustomer(string createdName, ulong customerNo, int messageType, int smsType, DateTime startDate, DateTime endDate, int pageSize)
        {


            string response = string.Empty;
            TransactionsDto dto = new TransactionsDto();
            if (createdName == null)
                createdName = string.Empty;
            try
            {
                if (messageType == 1)
                {
                    if (smsType == 1)
                    {
                        var res = await _repositoryManager.Transactions.GetOtpMessagesWithCustomerNoByCreatedNameAsync(createdName, customerNo, startDate, endDate, 0, pageSize);
                        dto = new TransactionsDto { Transactions = res.Item1, Count = res.Item2 };
                    }
                    if (smsType == 2)
                    {
                        var res = await _repositoryManager.Transactions.GetTransactionalSmsMessagesWithCustomerNoByCreatedNameAsync(createdName, customerNo, startDate, endDate, 0, pageSize);
                        dto = new TransactionsDto { Transactions = res.Item1, Count = res.Item2 };
                    }


                }
                if (messageType == 2)
                {
                    var res = await _repositoryManager.Transactions.GetMailMessagesWithCustomerNoAsync(createdName, customerNo, startDate, endDate, 0, pageSize);
                    dto = new TransactionsDto { Transactions = res.Item1, Count = res.Item2 };
                }
                if (messageType == 3)
                {
                    var res = await _repositoryManager.Transactions.GetPushMessagesWithCustomerNoAsync(createdName, customerNo, startDate, endDate, 0, pageSize);
                    dto = new TransactionsDto { Transactions = res.Item1, Count = res.Item2 };
                }


                response = ExcelCreate(dto.Transactions, messageType);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }

            return Ok(response);
        }


        [SwaggerOperation(Summary = "Returns report for SmsExcelRapor with CitizenshipNo",
      Tags = new[] { "Report Management" })]
        [HttpGet("report/citizen/{citizenshipNo}/{messageType}")]
        [SwaggerResponse(200, "Report is returned successfully", typeof(byte[]))]
        public async Task<IActionResult> GetTransactionsExcelReportWithCitizenshipNo(string citizenshipNo, string createdName, int messageType, int smsType, DateTime startDate, DateTime endDate, int pageSize = 20)
        {

            TransactionsDto dto = new TransactionsDto();
            string response = string.Empty;
            if (createdName == null)
                createdName = string.Empty;
            try
            {
                if (messageType == 1)
                {
                    if (smsType == 1)
                    {
                        var res = await _repositoryManager.Transactions.GetOtpMessagesWithCitizenshipNoByCreatedNameAsync(createdName, citizenshipNo, startDate, endDate, 0, pageSize);
                        dto = new TransactionsDto { Transactions = res.Item1, Count = res.Item2 };
                    }

                    if (smsType == 2)
                    {
                        var res = await _repositoryManager.Transactions.GetTransactionalSmsMessagesWithCitizenshipNoByCreatedNameAsync(createdName, citizenshipNo, startDate, endDate, 0, pageSize);
                        dto = new TransactionsDto { Transactions = res.Item1, Count = res.Item2 };
                    }

                }
                if (messageType == 2)
                {
                    var res = await _repositoryManager.Transactions.GetMailMessagesWithCitizenshipNoAsync(createdName, citizenshipNo, startDate, endDate, 0, pageSize);
                    dto = new TransactionsDto { Transactions = res.Item1, Count = res.Item2 };
                }
                if (messageType == 3)
                {
                    var res = await _repositoryManager.Transactions.GetPushMessagesWithCitizenshipNoAsync(createdName, citizenshipNo, startDate, endDate, 0, pageSize);
                    dto = new TransactionsDto { Transactions = res.Item1, Count = res.Item2 };
                }



                response = ExcelCreate(dto.Transactions, messageType);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }

            return Ok(response);
        }

        private string ExcelCreate(IEnumerable<Transaction> transactions, int messageType)
        {
            var stream = new System.IO.MemoryStream();
            string response = string.Empty;
            using (ExcelPackage package = new ExcelPackage(stream))
            {
                try
                {

                    var worksheet = package.Workbook.Worksheets.Add("SmsExcelRapor");
                    worksheet.Protection.AllowAutoFilter = false;
                    worksheet.Protection.AllowDeleteColumns = false;
                    worksheet.Protection.AllowInsertRows = false;
                    worksheet.Protection.AllowEditObject = true;
                    worksheet.Protection.AllowEditScenarios = false;
                    worksheet.Protection.AllowFormatCells = false;
                    worksheet.Protection.AllowFormatColumns = false;
                    worksheet.Protection.AllowDeleteRows = false;
                    worksheet.Protection.AllowInsertColumns = false;
                    worksheet.Protection.AllowInsertHyperlinks = false;

                    worksheet.Cells[1, 2].Value = "Müşteri Numarası";
                    worksheet.Cells[1, 3].Value = "Kimlik No";
                    worksheet.Cells[1, 4].Value = "Gönderim Zamanı";
                    worksheet.Cells[1, 5].Value = "İşlem Türü";
                    if (messageType == 1 || messageType == 3)
                        worksheet.Cells[1, 6].Value = "Telefon";
                    else if (messageType == 2)
                        worksheet.Cells[1, 6].Value = "Mail";
                    worksheet.Cells[1, 7].Value = "Durum";
                    worksheet.Cells[1, 8].Value = "Gönderen Sistem Bilgisi";
                    worksheet.Cells[1, 9].Value = "Response Message";
                    Color BaslikRengi = ColorTranslator.FromHtml("#ed7c31");
                    for (int i = 1; i < 10; i++)
                    {

                        worksheet.Cells[1, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[1, i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(BaslikRengi);
                        worksheet.Cells[1, i].Style.Font.Color.SetColor(Color.White);
                        worksheet.Column(i).AutoFit();
                        worksheet.Cells[1, i].Style.Font.Bold = true;
                    }
                    int Index = 2;
                    foreach (Transaction transaction in transactions)
                    {

                        worksheet.Cells[Index, 2].Value = transaction.CustomerNo;
                        worksheet.Cells[Index, 3].Value = transaction.CitizenshipNo;
                        worksheet.Cells[Index, 4].Value = transaction.CreatedAt.ToString();
                        worksheet.Cells[Index, 5].Value = transaction.TransactionType.ToString();
                        string Basari = CheckSmsStatus(transaction);
                        if (messageType == 1)
                        {
                            if (transaction.Phone != null)
                                worksheet.Cells[Index, 6].Value = transaction.Phone.ToString();
                            if (Basari != "Basarili" && transaction.OtpRequestLog != null && transaction.OtpRequestLog.ResponseLogs != null && transaction.OtpRequestLog.ResponseLogs.Count() > 0)
                            {



                                worksheet.Cells[Index, 10].Value = transaction.OtpRequestLog.ResponseLogs.FirstOrDefault().StatusQueryId;


                                if (transaction.OtpRequestLog.ResponseLogs.FirstOrDefault().TrackingLogs != null && transaction.OtpRequestLog.ResponseLogs.FirstOrDefault().TrackingLogs.Count() > 0)
                                {
                                    worksheet.Cells[Index, 9].Value = transaction.OtpRequestLog.ResponseLogs.FirstOrDefault().TrackingLogs.FirstOrDefault().ResponseMessage;
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(transaction.OtpRequestLog.ResponseLogs.FirstOrDefault().ResponseMessage))
                                        worksheet.Cells[Index, 9].Value = transaction.OtpRequestLog.ResponseLogs.FirstOrDefault().ResponseMessage;
                                    else
                                    {
                                        worksheet.Cells[Index, 9].Value = transaction.OtpRequestLog.ResponseLogs.FirstOrDefault().ResponseCode.ToString();
                                    }
                                }
                            }
                            else if (Basari != "Basarili")
                            {
                                worksheet.Cells[Index, 9].Value = "Beklenmedik bir hata oluştu:TransactionId=>" + transaction.Id;
                            }

                        }
                        else if (messageType == 2)
                        {
                            worksheet.Cells[Index, 6].Value = transaction.Mail.ToString();
                            if (Basari != "Basarili" && transaction.MailRequestLog != null && transaction.MailRequestLog.ResponseLogs != null && transaction.MailRequestLog.ResponseLogs.Count() > 0)
                            {
                                if (transaction.MailRequestLog.ResponseLogs.FirstOrDefault().ResponseMessage.Length < 50)
                                    worksheet.Cells[Index, 9].Value = transaction.MailRequestLog.ResponseLogs.FirstOrDefault().ResponseMessage;
                                else
                                {
                                    worksheet.Cells[Index, 9].Value = "Beklenmedik bir hata oluştu:TransactionId=>" + transaction.Id;
                                }
                                worksheet.Cells[Index, 10].Value = transaction.MailRequestLog.ResponseLogs.FirstOrDefault().StatusQueryId;
                            }
                        }
                        else if (messageType == 3)
                        {
                            if (Basari != "Basarili" && transaction.PushNotificationRequestLog != null && transaction.PushNotificationRequestLog.ResponseLogs != null && transaction.PushNotificationRequestLog.ResponseLogs.Count() > 0)
                            {



                                worksheet.Cells[Index, 10].Value = transaction.PushNotificationRequestLog.ResponseLogs.FirstOrDefault().StatusQueryId;



                                worksheet.Cells[Index, 9].Value = transaction.PushNotificationRequestLog.ResponseLogs.FirstOrDefault().ResponseMessage;

                            }
                        }
                        worksheet.Cells[Index, 7].Value = Basari;
                        worksheet.Cells[Index, 8].Value = transaction.CreatedBy.Name;


                        Index++;

                    }




                    package.Save();

                }
                catch (Exception ex)
                {
                    throw new Exceptions.WorkflowException(ex.ToString(), System.Net.HttpStatusCode.BadRequest);
                }
            }
            try
            {
                response = Convert.ToBase64String(stream.ToArray());
            }
            catch (Exception ex)
            {

            }
            return response;

        }
        private string CheckSmsStatus(Transaction txn)
        {
            if (txn.TransactionType == TransactionType.Otp)
            {
                if (txn.OtpRequestLog != null)
                {
                    if (txn.OtpRequestLog.ResponseLogs != null)
                    {
                        if (txn.OtpRequestLog.ResponseLogs.Any(l => l.TrackingStatus == SmsTrackingStatus.Delivered))
                            return "Basarili";
                        else if (txn.OtpRequestLog.ResponseLogs.Any(l => l.TrackingStatus == SmsTrackingStatus.Pending))
                        {
                            return "Sms Kontrol Gerekli";
                        }
                        else
                        {
                            return "Başarısız";
                        }
                    }
                }
            }
            if (txn.TransactionType == TransactionType.TransactionalMail || txn.TransactionType == TransactionType.TransactionalTemplatedMail)
            {
                if (txn.MailRequestLog != null)
                {
                    if (txn.MailRequestLog.ResponseLogs != null)
                    {
                        if (txn.MailRequestLog.ResponseLogs.Any(l => l.ResponseCode == "0"))
                            return "Basarili";
                        else
                        {
                            return "Başarısız";
                        }
                    }
                }
            }
            if (txn.TransactionType == TransactionType.TransactionalSms || txn.TransactionType == TransactionType.TransactionalTemplatedSms)
            {
                if (txn.SmsRequestLog != null)
                {
                    if (txn.SmsRequestLog.ResponseLogs != null)
                    {
                        if (txn.SmsRequestLog.ResponseLogs.Any(l => l.OperatorResponseCode == 0))
                            return "Basarili";
                        else
                        {
                            return "Başarısız";
                        }
                    }
                }
            }
            if (txn.TransactionType == TransactionType.TransactionalPush || txn.TransactionType == TransactionType.TransactionalTemplatedPush)
            {
                if (txn.PushNotificationRequestLog != null)
                {
                    if (txn.PushNotificationRequestLog.ResponseLogs != null)
                    {
                        if (txn.PushNotificationRequestLog.ResponseLogs.Any(l => l.ResponseCode == "0"))
                            return "Basarili";
                        else
                        {
                            return "Başarısız";
                        }
                    }
                }
            }

            return "Başarısız";
        }

        private async Task<OperatorReport> GetOperatorInfo(DateTime startDate,DateTime endDate,OperatorType @operator, bool isOtp, bool isFast)
        { 
            var operatorReport = new OperatorReport();
            operatorReport.Operator = @operator;
            if (isOtp)
            {

                try
                {
                    operatorReport.OtpCount = await _repositoryManager.Transactions.GetSuccessfullOtpCount(startDate,endDate,@operator);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                try
                {
                    operatorReport.ForeignOtpCount = await _repositoryManager.Transactions.GetSuccessfullForeignOtpCount(startDate, endDate, @operator);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                try
                {
                    operatorReport.SuccessfullOtpRequestCount = await _repositoryManager.Transactions.GetOtpRequestCount(startDate, endDate, @operator,true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                try
                {
                    operatorReport.UnsuccessfullOtpRequestCount = await _repositoryManager.Transactions.GetOtpRequestCount(startDate, endDate, @operator, false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                try
                {
                    operatorReport.SuccessfullForeignOtpRequestCount = await _repositoryManager.Transactions.GetForeignOtpRequestCount(startDate, endDate, @operator, true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                try
                {
                    operatorReport.UnsuccessfullForeignOtpRequestCount = await _repositoryManager.Transactions.GetForeignOtpRequestCount(startDate, endDate, @operator, false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            if (isFast)
            {
                try
                {
                    operatorReport.FastCount = await _repositoryManager.Transactions.GetSuccessfullSmsCount(startDate, endDate, @operator);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                try
                {
                    operatorReport.ForeignFastCount = await _repositoryManager.Transactions.GetSuccessfullForeignSmsCount(startDate, endDate, @operator);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                try
                {
                    operatorReport.SuccessfullFastRequestCount = await _repositoryManager.Transactions.GetSmsRequestCount(startDate, endDate, @operator, true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                try
                {
                    operatorReport.UnsuccessfullFastRequestCount = await _repositoryManager.Transactions.GetSmsRequestCount(startDate, endDate, @operator, false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                try
                {
                    operatorReport.SuccessfullForeignFastRequestCount = await _repositoryManager.Transactions.GetForeignSmsRequestCount(startDate, endDate, @operator, true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                try
                {
                    operatorReport.UnsuccessfullForeignFastRequestCount = await _repositoryManager.Transactions.GetForeignSmsRequestCount(startDate, endDate, @operator, false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return operatorReport;
        }

        


    }
}
