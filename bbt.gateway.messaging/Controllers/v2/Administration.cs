﻿using Asp.Versioning;
using bbt.gateway.common.Extensions;
using bbt.gateway.common.GlobalConstants;
using bbt.gateway.common.Models;
using bbt.gateway.common.Models.v2;
using bbt.gateway.common.Repositories;
using bbt.gateway.messaging.Api.Pusula;
using bbt.gateway.messaging.Api.Pusula.Model.GetByCitizenshipNumber;
using bbt.gateway.messaging.Api.Pusula.Model.GetByPhone;
using bbt.gateway.messaging.Api.Pusula.Model.GetCustomer;
using bbt.gateway.messaging.Workers;
using Dapr.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Globalization;
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
        private readonly InfobipSender _infobipSender;
        private readonly DaprClient _daprClient;
        private readonly ITransactionManager _transactionManager;
        private readonly PusulaClient _pusulaClient;

        public Administration(HeaderManager headerManager, OperatorManager operatorManager,
            IRepositoryManager repositoryManager,
            CodecSender codecSender, dEngageSender dEngageSender, OtpSender otpSender, InfobipSender infobipSender, DaprClient daprClient,
            ITransactionManager transactionManager, PusulaClient pusulaClient)
        {
            _headerManager = headerManager;
            _operatorManager = operatorManager;
            _repositoryManager = repositoryManager;
            _codecSender = codecSender;
            _dEngageSender = dEngageSender;
            _otpSender = otpSender;
            _daprClient = daprClient;
            _infobipSender = infobipSender;
            _transactionManager = transactionManager;
            _pusulaClient = pusulaClient;
        }

        [SwaggerOperation(Summary = "Update Notification Read Status", Tags = ["Notifications Management"])]
        [HttpPost("notification/{customerId}")]
        [SwaggerResponse(200, "Record was updated successfully")]
        public async Task<IActionResult> SetNotificationReadAsync(
                                                                    string customerId,
                                                                    [FromBody] NotificationSetReadRequest? notificationSetReadRequest,
                                                                    [FromQuery(Name = "notificationId")] string notificationGuid
                                                                 )
        {
            if (notificationSetReadRequest is not { })
            {
                notificationSetReadRequest = new NotificationSetReadRequest
                {
                    notificationId = notificationGuid
                };
            }

            var isGuid = Guid.TryParse(notificationSetReadRequest.notificationId, out Guid notificationId);

            if (isGuid)
            {
                var notification = await _repositoryManager.PushNotificationRequestLogs.FirstOrDefaultAsync(p => p.Id.Equals(notificationId));

                if (notification is { })
                {
                    notification.IsRead = true;
                    notification.ReadAt = DateTime.Now;
                    await _repositoryManager.SaveChangesAsync();
                    await _daprClient.DeleteStateAsync(GlobalConstants.DAPR_STATE_STORE, "mg_" + customerId + "_notifications");

                    return Ok();
                }
            }

            return NotFound();
        }

        [SwaggerOperation(Summary = "Update Notification Delete Status", Tags = ["Notifications Management"])]
        [HttpDelete("notification/{customerId}/{notificationId}")]
        [SwaggerResponse(200, "Record was updated successfully")]
        public async Task<IActionResult> SetNotificationDeletedAsync(string customerId, string notificationId)
        {
            var isGuid = Guid.TryParse(notificationId, out Guid notificationIdGuid);

            if (isGuid)
            {
                var notification = await _repositoryManager.PushNotificationRequestLogs.FirstOrDefaultAsync(p => p.Id.Equals(notificationIdGuid));

                if (notification is { })
                {
                    notification.IsDeleted = true;
                    notification.DeletedAt = DateTime.Now;
                    await _repositoryManager.SaveChangesAsync();
                    await _daprClient.DeleteStateAsync(GlobalConstants.DAPR_STATE_STORE, "mg_" + customerId + "_notifications");

                    return Ok();
                }
            }

            return NotFound();
        }

        [SwaggerOperation(Summary = "Update Notification Delete Status", Tags = ["Notifications Management"])]
        [HttpDelete("notification/{customerId}")]
        [SwaggerResponse(200, "Records was updated successfully")]
        public async Task<IActionResult> SetNotificationsDeletedAsync(string customerId)
        {
            await DeleteNotificationAsync(customerId);

            await _daprClient.DeleteStateAsync(GlobalConstants.DAPR_STATE_STORE, "mg_" + customerId + "_notifications");

            return Ok();
        }

        [SwaggerOperation(Summary = "Returns notifications", Tags = ["Notifications Management"])]
        [HttpGet("notifications/statistics/{customerId}")]
        [SwaggerResponse(200, "Records was returned successfully", typeof(NotificationsCountResponse))]
        public async Task<IActionResult> GetNotificationStaticsAsync(string customerId)
        {
            var stats = await GetNotificationStatisticsAsync(customerId);

            return Ok(new NotificationsCountResponse()
            {
                readCount = stats.readCount,
                unreadCount = stats.unreadCount
            });
        }

        [SwaggerOperation(Summary = "Returns notifications", Tags = ["Notifications Management"])]
        [HttpGet("notifications/{customerId}/{pageIndex}/{pageSize}")]
        [SwaggerResponse(200, "Records was returned successfully", typeof(Notification[]))]
        public async Task<IActionResult> GetNotificationsAsync(string customerId, int pageIndex, int pageSize)
        {
            var notifications = await _daprClient.GetStateAsync<List<Notification>>(GlobalConstants.DAPR_STATE_STORE, "mg_" + customerId + "_notifications");

            if (notifications == null)
            {
                notifications = await GetNotificationAsync(customerId);

                await _daprClient.SaveStateAsync(GlobalConstants.DAPR_STATE_STORE, "mg_" + customerId + "_notifications", notifications, metadata: new Dictionary<string, string>() {
                    {
                        "ttlInSeconds", "60"
                    }
                });

                Response.Headers.TryAdd("X-Cache", "Miss");
            }
            else
            {
                Response.Headers.TryAdd("X-Cache", "Hit");
            }

            return Ok(notifications.Skip((pageIndex - 1) * pageSize).Take(pageSize));
        }

        private async Task<List<Notification>> GetNotificationAsync(string customerId)
        {
            var pushList = await _repositoryManager.PushNotificationRequestLogs.GetPushNotifications(customerId);

            return pushList.Select(n => new Notification()
            {
                contentHtml = n.Content,
                date = n.CreatedAt.ToString("d MMMM yyyy", new CultureInfo("tr-TR")),
                isRead = n.IsRead,
                notificationId = n.Id.ToString(),
                reminderType = n.NotificationType,
                dateTime = n.CreatedAt,
                customParametersString = n.CustomParameters
            }).OrderByDescending(t => t.dateTime).ToList();
        }

        private async Task DeleteNotificationAsync(string customerId)
        {
            var notificationList = await _repositoryManager.PushNotificationRequestLogs.FindAsync(p => p.ContactId.Equals(customerId) && p.IsDeleted != true);

            foreach (var notification in notificationList)
            {
                notification.IsDeleted = true;
                notification.DeletedAt = DateTime.Now;
            }

            await _repositoryManager.SaveChangesAsync();
        }

        private async Task<NotificationsCountResponse> GetNotificationStatisticsAsync(string customerId)
        {
            var pushList = await _repositoryManager.PushNotificationRequestLogs.GetPushNotifications(customerId);
            var readCount = pushList.Count(p => p.IsRead);
            var unreadCount = pushList.Count() - readCount;

            return new NotificationsCountResponse()
            {
                readCount = readCount,
                unreadCount = unreadCount
            };
        }

        [SwaggerOperation(Summary = "Update Old Blacklist Record's CustomerNo or ContactNo", Tags = ["Phone Management"])]
        [HttpPut("blacklist/phone/{countryCode}/{prefix}/{number}/{customerNo}")]
        [SwaggerResponse(200, "Whitelist record is deleted successfully", typeof(void))]
        public async Task<IActionResult> UpdateOldBlacklistCustomerInfoAsync(string countryCode, string prefix, string number, long customerNo)
        {
            var phoneNumber = $"+{countryCode}{prefix}{number}";

            var blacklistRecords = await _repositoryManager.DirectBlacklists.FindAsync(b => b.GsmNumber.Equals(phoneNumber) && b.CustomerNo == 0);

            foreach (var blacklistRecord in blacklistRecords)
            {
                blacklistRecord.CustomerNo = customerNo;
                _transactionManager.LogInformation($"{blacklistRecord.SmsId}-{phoneNumber} blacklist updated with customer No:{customerNo}");
            }

            await _repositoryManager.SaveSmsBankingChangesAsync();

            return Ok();
        }

        [SwaggerOperation(Summary = "Returns Sms Counts And Success Rate", Description = "Returns Sms Counts And Success Rate.")]
        [HttpGet("Report/Sms/{operator}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> SmsReportAsync(int @operator, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var key = GlobalConstants.SMS_DAILY_REPORT + "_" + @operator + "_" + startDate.ToShortDateString() + "_" + endDate.ToShortDateString();
                var res = await _daprClient.GetStateAsync<OperatorReport>(GlobalConstants.DAPR_STATE_STORE, key);

                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [SwaggerOperation(Summary = "Check Fast Sms Message Status", Description = "Check Fast Sms Delivery Status.")]
        [HttpPost("sms/check-message")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> CheckMessageStatusAsync([FromBody] CheckFastSmsRequest data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            switch (data.Operator)
            {
                case OperatorType.Codec:
                    {
                        var res = await _codecSender.CheckSms(data);
                        return Ok(res);
                    }

                case OperatorType.dEngageOn:
                case OperatorType.dEngageBurgan:
                    {
                        var res = await _dEngageSender.CheckSms(data);
                        return Ok(res);
                    }

                case OperatorType.Infobip:
                    {
                        var res = await _infobipSender.CheckSms(data);
                        return Ok(res);
                    }
            }

            return BadRequest("Unknown Operator");
        }

        [SwaggerOperation(Summary = "Check Fast Sms Message Status", Description = "Check Fast Sms Delivery Status.")]
        [HttpPost("otp/check-message")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> CheckOtpMessageStatusAsync([FromBody] common.Models.v2.CheckSmsRequest data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (data.Operator == OperatorType.Infobip)
            {
                var res = await _infobipSender.CheckSms(data.MapTo<common.Models.CheckSmsRequest>());
                return Ok(res);
            }
            else
            {
                var res = await _otpSender.CheckMessage(data.MapTo<common.Models.CheckSmsRequest>());
                return Ok(res);
            }
        }

        [SwaggerOperation(Summary = "Check Mail Message Status", Description = "Check Mail Delivery Status.")]
        [HttpPost("email/check-message")]
        public async Task<IActionResult> CheckMessageStatusAsync([FromBody] CheckMailStatusRequest data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (data.Operator == OperatorType.dEngageOn || data.Operator == OperatorType.dEngageBurgan)
            {
                var res = await _dEngageSender.CheckMail(data);
                return Ok(res);
            }

            return BadRequest("Unknown Operator");
        }

        [SwaggerOperation(Summary = "Check Sms Message Status", Description = "Check Transactional Sms Delivery Status.")]
        [HttpGet("sms/check")]
        public async Task<IActionResult> CheckSmsStatusAsync(Guid TxnId)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Mock")
            {
                return Ok(new CheckSmsStatusResponse()
                {
                    code = 0,
                    message = "Delivered",
                    status = SmsStatus.Delivered
                });
            }

            CheckSmsStatusResponse res = new();

            var transaction = await _repositoryManager.Transactions.GetWithIdAsNoTrackingAsync(TxnId);

            if (transaction == null)
                return NotFound("Transaction Not Found");

            if (transaction.SmsRequestLog == null)
            {
                return NotFound("Sms Log Not Found");
            }

            var smsRequestLog = transaction.SmsRequestLog;

            if (smsRequestLog.ResponseLogs == null || smsRequestLog.ResponseLogs.Count <= 0)
            {
                return NotFound("Sms Log Not Found");
            }

            var smsResponseLog = smsRequestLog.ResponseLogs.FirstOrDefault();

            if (String.IsNullOrWhiteSpace(smsResponseLog.Status))
            {
                res.status = SmsStatus.Pending;
                res.message = "Pending";
                res.code = 0;
                return Ok(res);
            }

            if (smsResponseLog.Status.Equals("Delivered"))
            {
                res.status = SmsStatus.Delivered;
                res.message = "Delivered";
                res.code = 0;
                return Ok(res);
            }
            else
            {
                res.status = SmsStatus.NotDelivered;
                res.message = "NotDelivered";
                res.code = 0;
                return Ok(res);
            }
        }

        [SwaggerOperation(Summary = "Check Message Status", Description = "Check Message Status.")]
        [HttpGet("transaction/check")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> CheckTransactionStatusAsync([FromQuery] Guid transactionId)
        {
            var transaction = await _repositoryManager.Transactions.GetWithIdAsNoTrackingAsync(transactionId);

            if (transaction == null)
                return NotFound("Transaction Not Found");

            if (transaction.TransactionType == TransactionType.TransactionalMail || transaction.TransactionType == TransactionType.TransactionalTemplatedMail)
            {
                if (transaction.MailRequestLog == null)
                {
                    return Ok(new CheckTransactionStatusResponse { Status = TransactionStatus.NotDelivered, Detail = "MailRequestLog Not Found" });
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

        [SwaggerOperation(Summary = "Returns content headers configuration", Tags = ["Header Management"])]
        [HttpGet("headers")]
        [SwaggerResponse(200, "Headers is returned successfully", typeof(Header[]))]
        public async Task<IActionResult> GetHeadersAsync([FromQuery][Range(0, 100)] int page = 0, [FromQuery][Range(1, 100)] int pageSize = 20)
        {
            return Ok(await _headerManager.Get(page, pageSize));
        }

        [SwaggerOperation(Summary = "Save or update header configuration", Tags = ["Header Management"])]
        [HttpPost("headers")]
        [SwaggerRequestExample(typeof(HeaderRequest), typeof(AddHeaderRequestExampleFilter))]
        [SwaggerResponse(200, "Header is saved successfully", typeof(Header[]))]
        public async Task<IActionResult> SaveHeaderAsync([FromBody] Header data)
        {
            await _headerManager.Save(data);
            return Ok();
        }

        [SwaggerOperation(Summary = "Deletes header configuration", Tags = ["Header Management"])]
        [HttpDelete("headers/{id}")]
        [SwaggerResponse(200, "Header is deleted successfully", typeof(void))]
        public async Task<IActionResult> DeleteHeaderAsync(Guid id)
        {
            await _headerManager.Delete(id);
            return Ok();
        }

        [SwaggerOperation(Summary = "Returns operator configurations", Tags = ["Operator Management"])]
        [HttpGet("operators")]
        [SwaggerResponse(200, "Operators was returned successfully", typeof(Operator[]))]
        public async Task<IActionResult> GetOperatorsAsync()
        {
            return Ok(await _operatorManager.Get());
        }

        [SwaggerOperation(Summary = "Updated operator configuration", Tags = ["Operator Management"])]
        [HttpPost("operators")]
        [SwaggerResponse(200, "operator has saved successfully", typeof(void))]
        public async Task<IActionResult> SaveOperatorAsync([FromBody] Operator data)
        {
            await _operatorManager.Save(data);
            return Ok();
        }

        [SwaggerOperation(Summary = "Resolve Blacklist Entry", Tags = ["Phone Management"])]
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

                    return Ok();
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
        }

        [SwaggerOperation(Summary = "Returns phone activities", Tags = ["Phone Management"])]
        [HttpGet("phone-monitor/{countryCode}/{prefix}/{number}")]
        [SwaggerResponse(200, "Records was returned successfully", typeof(PhoneConfiguration))]
        public async Task<IActionResult> GetPhoneMonitorRecordsAsync(int countryCode, int prefix, int number, int count)
        {
            return Ok(await _repositoryManager.PhoneConfigurations.GetWithRelatedLogsAndBlacklistEntriesAsync(countryCode, prefix, number, count));
        }

        [SwaggerOperation(Summary = "Returns phone has active blacklist or not", Tags = ["Phone Management"])]
        [HttpGet("check-blacklist/{countryCode}/{prefix}/{number}")]
        [SwaggerResponse(200, "Phone has active blacklist", typeof(void))]
        [SwaggerResponse(404, "Phone has not active blacklist", typeof(void))]
        public async Task<IActionResult> CheckPhoneBlacklistStatusAsync(string countryCode, string prefix, string number)
        {
            var blacklistRecord = await _repositoryManager.BlackListEntries.GetLastBlacklistRecord(
                Convert.ToInt32(countryCode),
                Convert.ToInt32(prefix),
                Convert.ToInt32(number)
            );

            if (blacklistRecord == null)
                return NotFound();

            if (blacklistRecord.Status == BlacklistStatus.NotResolved)
            {
                if (blacklistRecord.SmsId > 0)
                {
                    var oldBlacklistRecord = await _repositoryManager.DirectBlacklists.FirstOrDefaultAsync(b => b.SmsId == blacklistRecord.SmsId);

                    if (oldBlacklistRecord.IsVerified)
                    {
                        blacklistRecord.Status = BlacklistStatus.Resolved;
                        blacklistRecord.ResolvedAt = oldBlacklistRecord.VerifyDate;
                        if (blacklistRecord.ResolvedBy is null)
                        {
                            blacklistRecord.ResolvedBy = new common.Models.Process();
                        }
                        blacklistRecord.ResolvedBy.Name = oldBlacklistRecord.VerifiedBy;
                        blacklistRecord.Reason = oldBlacklistRecord.Explanation;

                        await _repositoryManager.SaveChangesAsync();

                        return NotFound();
                    }

                    return Ok();
                }

                return Ok();
            }

            return NotFound();
        }

        [SwaggerOperation(Summary = "Returns phone blacklist records", Tags = ["Phone Management"])]
        [HttpGet("blacklists/{countryCode}/{prefix}/{number}")]
        [SwaggerResponse(200, "Records was returned successfully", typeof(BlackListEntry))]
        public async Task<IActionResult> GetPhoneBlacklistRecordsAsync(string countryCode, string prefix, string number, [Range(0, 100)] int page = 0, [Range(1, 100)] int pageSize = 20)
        {
            return Ok(await _repositoryManager.BlackListEntries
                .GetWithLogsAsync(Convert.ToInt32(countryCode), Convert.ToInt32(prefix), Convert.ToInt32(number), page, pageSize));
        }

        [SwaggerOperation(Summary = "Adds phone to blacklist records", Tags = ["Phone Management"])]
        [HttpPost("blacklists")]
        [SwaggerResponse(201, "Record was created successfully", typeof(void))]
        public async Task<IActionResult> AddPhoneToBlacklistAsync([FromBody] AddPhoneToBlacklistRequest data)
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
                    Phone = new common.Models.Phone
                    {
                        CountryCode = Convert.ToInt32(data.Phone.CountryCode),
                        Prefix = Convert.ToInt32(data.Phone.Prefix),
                        Number = Convert.ToInt32(data.Phone.Number)
                    },
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

            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var now = (env == "Prod" || env == "Drc") ? DateTime.Now : (data.CreatedAt ?? DateTime.Now);

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

        [SwaggerOperation(Summary = "Resolve blacklist item", Tags = ["Phone Management"])]
        [HttpPatch("blacklists/{blacklist-entry-id}/resolve")]
        [SwaggerResponse(201, "Record was updated successfully", typeof(void))]
        [SwaggerResponse(404, "Record not found", typeof(void))]
        public async Task<IActionResult> ResolveBlacklistItemAsync([FromRoute(Name = "blacklist-entry-id")] Guid entryId, [FromBody] ResolveBlacklistEntryRequest data)
        {
            var config = await _repositoryManager.BlackListEntries.FirstOrDefaultAsync(b => b.Id == entryId);

            if (config == null)
                return NotFound(entryId);

            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var resolvedAt = (env == "Prod" || env == "Drc") ? DateTime.Now : (data.ResolvedAt ?? DateTime.Now);
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

        [SwaggerOperation(Summary = "Deletes phone activities", Tags = ["Phone Management"])]
        [HttpDelete("phone-monitor/{countryCode}/{prefix}/{number}")]
        [SwaggerResponse(204, "Records was deleted successfully", typeof(void))]
        public async Task<IActionResult> DeletePhoneMonitorRecordsAsync(int countryCode, int prefix, int number)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (env != "Prod" &&
                env != "Production" &&
                env != "Drc")
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
            Tags = ["Whitelist Management"])]
        [HttpPost("whitelist/phone")]
        [SwaggerResponse(201, "Record was created successfully", typeof(void))]
        [SwaggerResponse(400, "Phone Number Field Is Mandatory", typeof(void))]
        [SwaggerResponse(409, "Phone Number Is Already Exists In Whitelist", typeof(void))]
        public async Task<IActionResult> AddPhoneToWhitelistAsync([FromBody] AddPhoneToWhitelistRequest data)
        {
            if (data.Phone == null)
            {
                return BadRequest("Phone Number Field Is Mandatory");
            }

            if ((await _repositoryManager.Whitelist.FindAsync(w => (w.Phone.CountryCode == Convert.ToInt32(data.Phone.CountryCode))
             && (w.Phone.Prefix == Convert.ToInt32(data.Phone.Prefix))
             && (w.Phone.Number == Convert.ToInt32(data.Phone.Number)))).FirstOrDefault() != null)
            {
                return BadRequest("Phone Number Is Already Exist");
            }

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
            Tags = ["Whitelist Management"])]
        [HttpPost("whitelist/email")]
        [SwaggerResponse(201, "Record was created successfully", typeof(void))]
        [SwaggerResponse(400, "Phone Number Field Is Mandatory", typeof(void))]
        [SwaggerResponse(409, "Phone Number Is Already Exists In Whitelist", typeof(void))]
        public async Task<IActionResult> AddMailToWhitelistAsync([FromBody] AddMailToWhitelistRequest data)
        {
            if (data.Email == null)
                return BadRequest("Email Field Is Mandatory");

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
            Tags = ["Whitelist Management"])]
        [HttpPost("whitelist/push")]
        [SwaggerResponse(201, "Record was created successfully", typeof(void))]
        [SwaggerResponse(400, "Citizenshipno Field Is Mandatory", typeof(void))]
        [SwaggerResponse(409, "Citizenshipno Is Already Exists In Whitelist", typeof(void))]
        public async Task<IActionResult> AddCitizenshipnoToWhitelistAsync([FromBody] AddCitizenshipnoToWhitelistRequest data)
        {
            if (data.CitizenshipNo == null)
                return BadRequest("Citizenshipno Field Is Mandatory");

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

        [SwaggerOperation(Summary = "Deletes Phone Number From Whitelist configuration", Tags = ["Whitelist Management"])]
        [HttpDelete("whitelist/phone")]
        [SwaggerResponse(200, "Whitelist record is deleted successfully", typeof(void))]
        public async Task<IActionResult> DeletePhoneFromWhitelistAsync(PhoneString phone)
        {
            var ip = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                    ?? HttpContext.Connection.RemoteIpAddress.ToString();
            _transactionManager.LogInformation($"Delete From Whitelist Called {ip} : request : {JsonConvert.SerializeObject(phone)}");
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

        [SwaggerOperation(Summary = "Deletes Mail Address From Whitelist configuration", Tags = ["Whitelist Management"])]
        [HttpDelete("whitelist/mail")]
        [SwaggerResponse(200, "Whitelist record is deleted successfully", typeof(void))]
        public async Task<IActionResult> DeleteMailFromWhitelistAsync(string Mail)
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

        [SwaggerOperation(Summary = "Deletes Citizenshipno From Whitelist configuration", Tags = ["Whitelist Management"])]
        [HttpDelete("whitelist/push")]
        [SwaggerResponse(200, "Whitelist record is deleted successfully", typeof(void))]
        public async Task<IActionResult> DeletePushFromWhitelistAsync(string CitizenshipNo)
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

        [SwaggerOperation(Summary = "Returns phone's whitelist status", Tags = ["Whitelist Management"])]
        [HttpGet("whitelist/check/phone")]
        [SwaggerResponse(200, "Phone is in whitelist", typeof(void))]
        [SwaggerResponse(404, "Phone is not in whitelist", typeof(void))]
        public async Task<IActionResult> CheckPhoneAsync(string CountryCode, string Prefix, string Number)
        {
            if ((await _repositoryManager.Whitelist.FindAsync(w => (w.Phone.CountryCode == Convert.ToInt32(CountryCode))
               && (w.Phone.Prefix == Convert.ToInt32(Prefix))
               && (w.Phone.Number == Convert.ToInt32(Number)))).FirstOrDefault() != null)
            {
                return Ok();
            }

            return NotFound();
        }

        [SwaggerOperation(Summary = "Returns E-mail's whitelist status", Tags = ["Whitelist Management"])]
        [HttpGet("whitelist/check/email")]
        [SwaggerResponse(200, "E-Mail is in whitelist", typeof(void))]
        [SwaggerResponse(404, "E-Mail is not in whitelist", typeof(void))]
        public async Task<IActionResult> CheckMailAsync(string email)
        {
            if ((await _repositoryManager.Whitelist.FindAsync(w => w.Mail == email)).FirstOrDefault() != null)
                return Ok();

            return NotFound();
        }

        [SwaggerOperation(Summary = "Returns CitizenshipNo's whitelist status", Tags = ["Whitelist Management"])]
        [HttpGet("whitelist/check/push")]
        [SwaggerResponse(200, "CitizensipNo is in whitelist", typeof(void))]
        [SwaggerResponse(404, "CitizensipNo is not in whitelist", typeof(void))]
        public async Task<IActionResult> CheckPushAsync(string CitizenshipNo)
        {
            if ((await _repositoryManager.Whitelist.FindAsync(w => w.ContactId == CitizenshipNo)).FirstOrDefault() != null)
                return Ok();

            return NotFound();
        }

        [SwaggerOperation(Summary = "Returns phones otp sending logs", Tags = ["Phone Management"])]
        [HttpGet("otp-log/{countryCode}/{prefix}/{number}")]
        [SwaggerResponse(200, "Records was returned successfully", typeof(OtpRequestLog[]))]
        public async Task<IActionResult> GetOtpLogAsync(int countryCode, int prefix, int number, [Range(0, 100)] int page = 0, [Range(1, 100)] int pageSize = 20)
        {
            return Ok(await _repositoryManager.OtpRequestLogs
                .GetWithResponseLogsAsync(countryCode, prefix, number, page, pageSize));
        }

        [SwaggerOperation(Summary = "Returns phones sms sending logs", Tags = ["Phone Management"])]
        [HttpGet("sms-log/{countryCode}/{prefix}/{number}")]
        [SwaggerResponse(200, "Records was returned successfully", typeof(SmsResponseLog[]))]
        public async Task<IActionResult> GetSmsLogAsync(int countryCode, int prefix, int number, [Range(0, 100)] int page = 0, [Range(1, 100)] int pageSize = 20)
        {
            return Ok((await _repositoryManager.SmsRequestLogs
                .FindAsync(c => c.PhoneConfiguration.Phone.CountryCode == countryCode && c.PhoneConfiguration.Phone.Prefix == prefix && c.PhoneConfiguration.Phone.Number == number))
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToArray());
        }

        [SwaggerOperation(Summary = "Returns Generated Template Message Associated With Transaction", Tags = ["Transaction Management"])]
        [HttpGet("transaction/{txnId}/generatedMessage")]
        [SwaggerResponse(200, "Records was returned successfully", typeof(GeneratedMessage))]
        public async Task<IActionResult> GetPhoneBlacklistRecordsAsync(Guid txnId)
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

        [SwaggerOperation(Summary = "Returns transactions info", Tags = ["Transaction Management"])]
        [HttpGet("transactions/phone/{countryCode}/{prefix}/{number}")]
        [SwaggerResponse(200, "Records was returned successfully", typeof(Transaction[]))]
        public async Task<IActionResult> GetTransactionsWithPhoneAsync(int countryCode, int prefix, int number, string createdName, int smsType, DateTime startDate, DateTime endDate, [Range(0, 100)] int page = 0, [Range(1, 100)] int pageSize = 20)
        {
            if (createdName == null)
                createdName = string.Empty;

            if (smsType == 1)
            {
                var res = await _repositoryManager.Transactions.GetOtpMessagesWithPhoneByCreatedNameAsync(createdName, countryCode, prefix, number, startDate, endDate, page, pageSize);
                return Ok(new TransactionsDto { Transactions = res.Item1, Count = res.Item2 });
            }
            else if (smsType == 2)
            {
                var res = await _repositoryManager.Transactions.GetTransactionalSmsMessagesWithPhoneByCreatedNameAsync(createdName, countryCode, prefix, number, startDate, endDate, page, pageSize);
                return Ok(new TransactionsDto { Transactions = res.Item1, Count = res.Item2 });
            }

            return Ok(new TransactionsDto());
        }

        [SwaggerOperation(Summary = "Returns transactions info by createdName", Tags = ["Transaction Management"])]
        [HttpGet("transactions/createdName/phone/{createdName}/{countryCode}/{prefix}/{number}")]
        [SwaggerResponse(200, "Records was returned successfully", typeof(Transaction[]))]
        public async Task<IActionResult> GetTransactionsWithPhoneByCreatedNameAsync(string createdName, int countryCode, int prefix, int number, int smsType, DateTime startDate, DateTime endDate, [Range(0, 100)] int page = 0, [Range(1, 100)] int pageSize = 20)
        {
            if (smsType == 1)
            {
                var res = await _repositoryManager.Transactions.GetOtpMessagesWithPhoneByCreatedNameAsync(createdName, countryCode, prefix, number, startDate, endDate, page, pageSize);
                return Ok(new TransactionsDto { Transactions = res.Item1, Count = res.Item2 });
            }
            else if (smsType == 2)
            {
                var res = await _repositoryManager.Transactions.GetTransactionalSmsMessagesWithPhoneByCreatedNameAsync(createdName, countryCode, prefix, number, startDate, endDate, page, pageSize);
                return Ok(new TransactionsDto { Transactions = res.Item1, Count = res.Item2 });
            }

            return Ok(new TransactionsDto());
        }

        [SwaggerOperation(Summary = "Returns transactions info", Tags = ["Transaction Management"])]
        [HttpGet("transactions/mail/{mail}")]
        [SwaggerResponse(200, "Records was returned successfully", typeof(Transaction[]))]
        public async Task<IActionResult> GetTransactionsWithEmailAsync(string mail, string createdName, DateTime startDate, DateTime endDate, [Range(0, 100)] int page = 0, [Range(1, 100)] int pageSize = 20)
        {
            if (createdName == null)
                createdName = string.Empty;

            var res = await _repositoryManager.Transactions.GetMailMessagesWithMailAsync(createdName, mail, startDate, endDate, page, pageSize);

            return Ok(new TransactionsDto { Transactions = res.Item1, Count = res.Item2 });
        }

        [SwaggerOperation(Summary = "Returns transactions info", Tags = ["Transaction Management"])]
        [HttpGet("transactions/customer/{customerNo}/{messageType}")]
        [SwaggerResponse(200, "Records was returned successfully", typeof(Transaction[]))]
        public async Task<IActionResult> GetTransactionsWithCustomerNoAsync(ulong customerNo, string createdName, int messageType, int smsType, DateTime startDate, DateTime endDate, [Range(0, 100)] int page = 0, [Range(1, 100)] int pageSize = 20)
        {
            if (createdName == null)
                createdName = string.Empty;

            switch (messageType)
            {
                case 1:
                    {
                        if (smsType == 1)
                        {
                            var res = await _repositoryManager.Transactions.GetOtpMessagesWithCustomerNoAsync(customerNo, startDate, endDate, page, pageSize);
                            return Ok(new TransactionsDto { Transactions = res.Item1, Count = res.Item2 });
                        }
                        else if (smsType == 2)
                        {
                            var res = await _repositoryManager.Transactions.GetTransactionalSmsMessagesWithCustomerNoAsync(customerNo, startDate, endDate, page, pageSize);
                            return Ok(new TransactionsDto { Transactions = res.Item1, Count = res.Item2 });
                        }

                        return Ok(new TransactionsDto());
                    }

                case 2:
                    {
                        var res = await _repositoryManager.Transactions.GetMailMessagesWithCustomerNoAsync(createdName, customerNo, startDate, endDate, page, pageSize);
                        return Ok(new TransactionsDto { Transactions = res.Item1, Count = res.Item2 });
                    }

                case 3:
                    {
                        var res = await _repositoryManager.Transactions.GetPushMessagesWithCustomerNoAsync(createdName, customerNo, startDate, endDate, page, pageSize);
                        return Ok(new TransactionsDto { Transactions = res.Item1, Count = res.Item2 });
                    }
            }

            return Ok(new TransactionsDto());
        }
        [SwaggerOperation(Summary = "Returns transactions info by created name", Tags = ["Transaction Management"])]
        [HttpGet("transactions/createdName/customer/{createdName}/{customerNo}/{messageType}")]
        [SwaggerResponse(200, "Records was returned successfully", typeof(Transaction[]))]
        public async Task<IActionResult> GetTransactionsWithCustomerNoByCreatedNameAsync(string createdName, ulong customerNo, int messageType, int smsType, DateTime startDate, DateTime endDate, [Range(0, 100)] int page = 0, [Range(1, 100)] int pageSize = 20)
        {
            if (messageType == 1)
            {
                if (smsType == 1)
                {
                    var res = await _repositoryManager.Transactions.GetOtpMessagesWithCustomerNoByCreatedNameAsync(createdName, customerNo, startDate, endDate, page, pageSize);
                    return Ok(new TransactionsDto { Transactions = res.Item1, Count = res.Item2 });
                }
                else if (smsType == 2)
                {
                    var res = await _repositoryManager.Transactions.GetTransactionalSmsMessagesWithCustomerNoByCreatedNameAsync(createdName, customerNo, startDate, endDate, page, pageSize);
                    return Ok(new TransactionsDto { Transactions = res.Item1, Count = res.Item2 });
                }

                return Ok(new TransactionsDto());
            }

            return Ok(new TransactionsDto());
        }

        [SwaggerOperation(Summary = "Returns transactions info", Tags = ["Transaction Management"])]
        [HttpGet("transactions/citizen/{citizenshipNo}/{messageType}")]
        [SwaggerResponse(200, "Records was returned successfully", typeof(Transaction[]))]
        public async Task<IActionResult> GetTransactionsWithCitizenshipNoAsync(string citizenshipNo, string createdName, int messageType, int smsType, DateTime startDate, DateTime endDate, [Range(0, 100)] int page = 0, [Range(1, 100)] int pageSize = 20)
        {
            if (createdName == null)
                createdName = string.Empty;

            switch (messageType)
            {
                case 1:
                    {
                        if (smsType == 1)
                        {
                            var res = await _repositoryManager.Transactions.GetOtpMessagesWithCitizenshipNoByCreatedNameAsync(createdName, citizenshipNo, startDate, endDate, page, pageSize);
                            return Ok(new TransactionsDto { Transactions = res.Item1, Count = res.Item2 });
                        }

                        else if (smsType == 2)
                        {
                            var res = await _repositoryManager.Transactions.GetTransactionalSmsMessagesWithCitizenshipNoByCreatedNameAsync(createdName, citizenshipNo, startDate, endDate, page, pageSize);
                            return Ok(new TransactionsDto { Transactions = res.Item1, Count = res.Item2 });
                        }

                        return Ok(new TransactionsDto());
                    }

                case 2:
                    {
                        var res = await _repositoryManager.Transactions.GetMailMessagesWithCitizenshipNoAsync(createdName, citizenshipNo, startDate, endDate, page, pageSize);
                        return Ok(new TransactionsDto { Transactions = res.Item1, Count = res.Item2 });
                    }

                case 3:
                    {
                        var res = await _repositoryManager.Transactions.GetPushMessagesWithCitizenshipNoAsync(createdName, citizenshipNo, startDate, endDate, page, pageSize);
                        return Ok(new TransactionsDto { Transactions = res.Item1, Count = res.Item2 });
                    }
            }

            return Ok(new List<Transaction>());
        }

        [SwaggerOperation(Summary = "Returns transactions info by created name", Tags = ["Transaction Management"])]
        [HttpGet("transactions/createdName/citizen/{createdName}/{citizenshipNo}/{messageType}")]
        [SwaggerResponse(200, "Records was returned successfully", typeof(Transaction[]))]
        public async Task<IActionResult> GetTransactionsWithCitizenshipNoByCreatedNameAsync(string createdName, string citizenshipNo, int messageType, int smsType, DateTime startDate, DateTime endDate, [Range(0, 100)] int page = 0, [Range(1, 100)] int pageSize = 20)
        {
            if (smsType == 1)
            {
                var res = await _repositoryManager.Transactions.GetOtpMessagesWithCitizenshipNoByCreatedNameAsync(createdName, citizenshipNo, startDate, endDate, page, pageSize);
                return Ok(new TransactionsDto { Transactions = res.Item1, Count = res.Item2 });
            }
            else if (smsType == 2)
            {
                var res = await _repositoryManager.Transactions.GetTransactionalSmsMessagesWithCitizenshipNoByCreatedNameAsync(createdName, citizenshipNo, startDate, endDate, page, pageSize);
                return Ok(new TransactionsDto { Transactions = res.Item1, Count = res.Item2 });
            }

            return Ok(new TransactionsDto());
        }

        [SwaggerOperation(Summary = "Returns report for CreditReport", Tags = ["Report Management"])]
        [HttpPost("CreditReport")]
        [SwaggerResponse(200, "Report is returned successfully", typeof(FileContentResult))]
        public async Task<FileContentResult> GetReportAsync(IFormFile file)
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
                        await GetReportLineAsync(
                            GetSmsResponseLog(transactions.FirstOrDefault()),
                            transactions.FirstOrDefault().SmsRequestLog);
                }
                if (transactions?.Count() > 1)
                {
                    resultContent += line +
                        await GetReportLineAsync(
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

        private async Task<string> GetReportLineAsync(SmsResponseLog smsResponseLog, SmsRequestLog smsRequestLog)
        {
            if (smsResponseLog == null)
            {
                return $"|{smsResponseLog?.CreatedAt.ToString(new CultureInfo("tr-TR"))}|Rapor Bulunamadı\r\n";
            }

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
            else if (smsResponseLog.Operator == OperatorType.dEngageOn || smsResponseLog.Operator == OperatorType.dEngageBurgan)
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
                return $"|{smsResponseLog?.CreatedAt.ToString(new CultureInfo("tr-TR"))}|{trackingLog.Status}|{trackingLog.StatusReason}\r\n";
            }

            return $"|{smsResponseLog?.CreatedAt.ToString(new CultureInfo("tr-TR"))}|Rapor Bulunamadı\r\n";
        }

        [SwaggerOperation(Summary = "Returns report for SmsExcelRapor with Phone", Tags = ["Report Management"])]
        [HttpGet("report/phone/{countryCode}/{prefix}/{number}")]
        [SwaggerResponse(200, "Report is returned successfully", typeof(string))]
        [SwaggerResponse(400, "Excel oluşturulamadı", typeof(string))]
        public async Task<IActionResult> GetTransactionsExcelReportWithPhoneAsync(string createdName, int countryCode, int prefix, int number, int smsType, DateTime startDate, DateTime endDate, int pageSize)
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
                else if (smsType == 2)
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
        [SwaggerOperation(Summary = "Returns report for SmsExcelRapor with Phone", Tags = ["Report Management"])]
        [HttpGet("report/mail/{mail}")]
        [SwaggerResponse(200, "Report is returned successfully", typeof(byte[]))]
        public async Task<IActionResult> GetTransactionsExcelReportWithMailAsync(string mail, string createdName, DateTime startDate, DateTime endDate, int pageSize)
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

        [SwaggerOperation(Summary = "Returns report for SmsExcelRapor with CustomerNo", Tags = ["Report Management"])]
        [HttpGet("report/customer/{customerNo}/{messageType}")]
        [SwaggerResponse(200, "Report is returned successfully", typeof(byte[]))]
        public async Task<IActionResult> GetTransactionsExcelReportWithCustomerAsync(string createdName, ulong customerNo, int messageType, int smsType, DateTime startDate, DateTime endDate, int pageSize)
        {
            string response = string.Empty;
            TransactionsDto dto = new TransactionsDto();
            if (createdName == null)
                createdName = string.Empty;
            try
            {
                switch (messageType)
                {
                    case 1:
                        {
                            if (smsType == 1)
                            {
                                var res = await _repositoryManager.Transactions.GetOtpMessagesWithCustomerNoByCreatedNameAsync(createdName, customerNo, startDate, endDate, 0, pageSize);
                                dto = new TransactionsDto { Transactions = res.Item1, Count = res.Item2 };
                            }
                            else if (smsType == 2)
                            {
                                var res = await _repositoryManager.Transactions.GetTransactionalSmsMessagesWithCustomerNoByCreatedNameAsync(createdName, customerNo, startDate, endDate, 0, pageSize);
                                dto = new TransactionsDto { Transactions = res.Item1, Count = res.Item2 };
                            }

                            break;
                        }

                    case 2:
                        {
                            var res = await _repositoryManager.Transactions.GetMailMessagesWithCustomerNoAsync(createdName, customerNo, startDate, endDate, 0, pageSize);
                            dto = new TransactionsDto { Transactions = res.Item1, Count = res.Item2 };
                            break;
                        }

                    case 3:
                        {
                            var res = await _repositoryManager.Transactions.GetPushMessagesWithCustomerNoAsync(createdName, customerNo, startDate, endDate, 0, pageSize);
                            dto = new TransactionsDto { Transactions = res.Item1, Count = res.Item2 };
                            break;
                        }
                }

                response = ExcelCreate(dto.Transactions, messageType);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }

            return Ok(response);
        }

        [SwaggerOperation(Summary = "Returns report for SmsExcelRapor with CitizenshipNo", Tags = ["Report Management"])]
        [HttpGet("report/citizen/{citizenshipNo}/{messageType}")]
        [SwaggerResponse(200, "Report is returned successfully", typeof(byte[]))]
        public async Task<IActionResult> GetTransactionsExcelReportWithCitizenshipNoAsync(string citizenshipNo, string createdName, int messageType, int smsType, DateTime startDate, DateTime endDate, int pageSize = 20)
        {
            TransactionsDto dto = new TransactionsDto();
            string response = string.Empty;

            if (createdName == null)
                createdName = string.Empty;

            try
            {
                switch (messageType)
                {
                    case 1:
                        {
                            if (smsType == 1)
                            {
                                var res = await _repositoryManager.Transactions.GetOtpMessagesWithCitizenshipNoByCreatedNameAsync(createdName, citizenshipNo, startDate, endDate, 0, pageSize);
                                dto = new TransactionsDto { Transactions = res.Item1, Count = res.Item2 };
                            }
                            else if (smsType == 2)
                            {
                                var res = await _repositoryManager.Transactions.GetTransactionalSmsMessagesWithCitizenshipNoByCreatedNameAsync(createdName, citizenshipNo, startDate, endDate, 0, pageSize);
                                dto = new TransactionsDto { Transactions = res.Item1, Count = res.Item2 };
                            }

                            break;
                        }

                    case 2:
                        {
                            var res = await _repositoryManager.Transactions.GetMailMessagesWithCitizenshipNoAsync(createdName, citizenshipNo, startDate, endDate, 0, pageSize);
                            dto = new TransactionsDto { Transactions = res.Item1, Count = res.Item2 };
                            break;
                        }

                    case 3:
                        {
                            var res = await _repositoryManager.Transactions.GetPushMessagesWithCitizenshipNoAsync(createdName, citizenshipNo, startDate, endDate, 0, pageSize);
                            dto = new TransactionsDto { Transactions = res.Item1, Count = res.Item2 };
                            break;
                        }
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

                        string Basari = CheckMessageStatusByTransaction(transaction);

                        switch (messageType)
                        {
                            case 1:
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
                                break;
                            case 2:
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
                                break;
                            case 3:
                                if (Basari != "Basarili" && transaction.PushNotificationRequestLog != null && transaction.PushNotificationRequestLog.ResponseLogs != null && transaction.PushNotificationRequestLog.ResponseLogs.Count() > 0)
                                {
                                    worksheet.Cells[Index, 10].Value = transaction.PushNotificationRequestLog.ResponseLogs.FirstOrDefault().StatusQueryId;

                                    worksheet.Cells[Index, 9].Value = transaction.PushNotificationRequestLog.ResponseLogs.FirstOrDefault().ResponseMessage;

                                }
                                break;
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
        private string CheckMessageStatusByTransaction(Transaction txn)
        {
            switch (txn.TransactionType)
            {
                case TransactionType.Otp:
                    {
                        if (txn.OtpRequestLog != null && txn.OtpRequestLog.ResponseLogs != null)
                        {
                            if (txn.OtpRequestLog.ResponseLogs.Any(l => l.TrackingStatus == SmsTrackingStatus.Delivered))
                            {
                                return "Basarili";
                            }
                            else if (txn.OtpRequestLog.ResponseLogs.Any(l => l.TrackingStatus == SmsTrackingStatus.Pending))
                            {
                                return "Sms Kontrol Gerekli";
                            }
                            else
                            {
                                return "Başarısız";
                            }
                        }

                        break;
                    }

                case TransactionType.TransactionalMail:
                case TransactionType.TransactionalTemplatedMail:
                    {
                        if (txn.MailRequestLog != null && txn.MailRequestLog.ResponseLogs != null)
                        {
                            if (txn.MailRequestLog.ResponseLogs.Any(l => l.ResponseCode == "0"))
                            {
                                return "Basarili";
                            }
                            else
                            {
                                return "Başarısız";
                            }
                        }

                        break;
                    }

                case TransactionType.TransactionalSms:
                case TransactionType.TransactionalTemplatedSms:
                    {
                        if (txn.SmsRequestLog != null && txn.SmsRequestLog.ResponseLogs != null)
                        {
                            if (txn.SmsRequestLog.ResponseLogs.Any(l => l.OperatorResponseCode == 0))
                            {
                                return "Basarili";
                            }
                            else
                            {
                                return "Başarısız";
                            }
                        }

                        break;
                    }

                case TransactionType.TransactionalPush:
                case TransactionType.TransactionalTemplatedPush:
                    {
                        if (txn.PushNotificationRequestLog != null && txn.PushNotificationRequestLog.ResponseLogs != null)
                        {
                            if (txn.PushNotificationRequestLog.ResponseLogs.Any(l => l.ResponseCode == "0"))
                            {
                                return "Basarili";
                            }
                            else
                            {
                                return "Başarısız";
                            }
                        }

                        break;
                    }
            }

            return "Başarısız";
        }

        [HttpGet("operator/getFastOperator")]
        public async Task<IActionResult> GetFastOperatorAsync()
        {
            return Ok(await _operatorManager.GetFastOperator());
        }

        [HttpPost("operator/changeFastOperator/{status}")]
        public async Task<IActionResult> ChangeFastOperatorAsync(int status)
        {
            await _operatorManager.ChangeFastOperator(status);

            return Ok();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("operators/updateOperatorStatus/{operatorId}/{status}")]
        public async Task<IActionResult> UpdateOperatorStatusAsync(int operatorId, int status)
        {
            await _operatorManager.UpdateOperatorStatusAsync(operatorId, status);

            return Ok();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("customerprofiles/customer/{customerNo}")]
        public async Task<IActionResult> GetCustomerByCustomerNoAsync(ulong customerNo)
        {
            var getCustomerProfileResponse = new GetCustomerProfileResponse();

            await SetCustomerProfileAsync(getCustomerProfileResponse, customerNo);

            return Ok(getCustomerProfileResponse);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("customerprofiles/citizen/{citizenshipNo}")]
        public async Task<IActionResult> GetCustomerByCitizenshipNumberAsync(string citizenshipNo)
        {
            var customer = await _pusulaClient.GetCustomerByCitizenshipNumber(new GetByCitizenshipNumberRequest()
            {
                CitizenshipNumber = citizenshipNo
            });

            var getCustomerProfileResponse = new GetCustomerProfileResponse();

            if (customer.IsSuccess)
            {
                await SetCustomerProfileAsync(getCustomerProfileResponse, customer.CustomerNo);
            }
            else
            {
                getCustomerProfileResponse.IsSuccess = false;
            }

            return Ok(getCustomerProfileResponse);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("customerprofiles/phone/{countryCode}/{prefix}/{number}")]
        public async Task<IActionResult> GetCustomerByPhoneNumberAsync(int countryCode, int prefix, int number)
        {
            var customer = await _pusulaClient.GetCustomerByPhoneNumber(new GetByPhoneNumberRequest()
            {
                CountryCode = countryCode.ToString(),
                CityCode = prefix.ToString(),
                TelephoneNumber = countryCode == 90 ? number.ToString().PadLeft(7, '0') : number.ToString()
            });

            var getCustomerProfileResponse = new GetCustomerProfileResponse();

            if (customer.IsSuccess)
            {
                await SetCustomerProfileAsync(getCustomerProfileResponse, customer.CustomerNo);
            }
            else
            {
                getCustomerProfileResponse.IsSuccess = false;
            }

            return Ok(getCustomerProfileResponse);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("customerprofiles/mail/{mail}")]
        public async Task<IActionResult> GetCustomerByEmailAsync(string mail)
        {
            var customer = await _pusulaClient.GetCustomerByEmail(new GetByEmailRequest()
            {
                Email = mail
            });

            var getCustomerProfileResponse = new GetCustomerProfileResponse();

            if (customer.IsSuccess)
            {
                await SetCustomerProfileAsync(getCustomerProfileResponse, customer.CustomerNo);
            }
            else
            {
                getCustomerProfileResponse.IsSuccess = false;
            }

            return Ok(getCustomerProfileResponse);
        }

        private async Task SetCustomerProfileAsync(GetCustomerProfileResponse getCustomerProfileResponse, ulong customerNo)
        {
            var customerDetail = await _pusulaClient.GetCustomer(new GetCustomerRequest()
            {
                CustomerNo = customerNo
            });

            if (customerDetail.IsSuccess)
            {
                getCustomerProfileResponse.IsSuccess = true;
                getCustomerProfileResponse.IsStaff = customerDetail.CustomerProfile == "GB" || customerDetail.CustomerProfile == "GG";
            }
            else
            {
                getCustomerProfileResponse.IsSuccess = false;
            }
        }
    }
}