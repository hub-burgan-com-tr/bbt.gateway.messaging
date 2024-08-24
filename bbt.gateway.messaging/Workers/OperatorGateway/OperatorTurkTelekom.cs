using bbt.gateway.messaging.Api.TurkTelekom.Model;
using bbt.gateway.common.Models;
using bbt.gateway.messaging.Api.TurkTelekom;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace bbt.gateway.messaging.Workers.OperatorGateway
{

    public class OperatorTurkTelekom : OperatorGatewayBase, IOperatorGateway
    {
        private readonly ITurkTelekomApi _turkTelekomApi;
        public OperatorTurkTelekom(TurkTelekomApiFactory turkTelekomApiFactory, IConfiguration configuration,
            ITransactionManager transactionManager) : base(configuration, transactionManager)
        {
            _turkTelekomApi = turkTelekomApiFactory(TransactionManager.UseFakeSmtp);
        }

        private async Task SetOperatorAsync()
        {
            await GetOperatorAsync(OperatorType.TurkTelekom);
            _turkTelekomApi.SetOperatorType(OperatorConfig);
        }

        public async Task<bool> SendOtp(Phone phone, string content, ConcurrentBag<OtpResponseLog> responses, Header header)
        {
            await SetOperatorAsync();
            var turkTelekomResponse = await _turkTelekomApi.SendSms(CreateSmsRequest(phone,content,header));

            var response =  turkTelekomResponse.BuildOperatorApiResponse();
            responses.Add(response);

            return true;
        }
        public async Task<OtpResponseLog> SendOtp(Phone phone, string content, Header header)
        {
            await SetOperatorAsync();
            var turkTelekomResponse = await _turkTelekomApi.SendSms(CreateSmsRequest(phone, content, header));

            var response = turkTelekomResponse.BuildOperatorApiResponse();

            return response;
        }

        public async Task<OtpResponseLog> SendOtpForeign(Phone phone, string content, Header header)
        {
            await Task.CompletedTask;
            return new OtpResponseLog();
        }

        public async Task<OtpTrackingLog> CheckMessageStatus(CheckSmsRequest checkSmsRequest)
        {
            await SetOperatorAsync();
            var turkTelekomResponse = await _turkTelekomApi.CheckSmsStatus(CreateSmsStatusRequest(checkSmsRequest.StatusQueryId));
            return turkTelekomResponse.BuildOperatorApiTrackingResponse(checkSmsRequest);
        }

        private TurkTelekomSmsRequest CreateSmsRequest(Phone phone, string content, Header header)
        {
            DateTime checkDate = DateTime.Now.AddDays(-1 * OperatorConfig.ControlDaysForOtp);
            
            var phoneConfiguration = TransactionManager.OtpRequestInfo.PhoneConfiguration;

            if (phoneConfiguration != null)
            {
                if (phoneConfiguration.BlacklistEntries != null &&
                    phoneConfiguration.BlacklistEntries.Count > 0)
                {
                    var blackListEntry = phoneConfiguration.BlacklistEntries
                    .Where(b => b.Status == BlacklistStatus.Resolved).OrderByDescending(b => b.CreatedAt)
                    .FirstOrDefault();

                    if (blackListEntry != null)
                    {
                        if (blackListEntry.ResolvedAt != null)
                        {
                            DateTime resolvedDate = blackListEntry.ResolvedAt.Value;
                            checkDate = checkDate > resolvedDate ? checkDate : resolvedDate;
                        }
                    }
                    else
                    {
                        checkDate = checkDate > TransactionManager.OldBlacklistVerifiedAt ? checkDate : TransactionManager.OldBlacklistVerifiedAt;
                    }
                }
                else
                {
                    checkDate = checkDate > TransactionManager.OldBlacklistVerifiedAt ? checkDate : TransactionManager.OldBlacklistVerifiedAt;
                }
            }
            else
            {
                checkDate = checkDate > TransactionManager.OldBlacklistVerifiedAt ? checkDate : TransactionManager.OldBlacklistVerifiedAt;
            }
            

            string gsmNo;

            if (TransactionManager.StringSend)
            {
                gsmNo = phone.CountryCode.ToString() + phone.Prefix.ToString().PadLeft(TransactionManager.PrefixLength, '0') + phone.Number.ToString().PadLeft(TransactionManager.NumberLength, '0');
            }
            else
            {
                gsmNo = phone.CountryCode.ToString() + phone.Prefix.ToString() + (phone.CountryCode == 90 ? phone.Number.ToString().PadLeft(7, '0') : phone.Number);
            }

            return new TurkTelekomSmsRequest()
            {
                UserCode = OperatorConfig.User,
                Password = OperatorConfig.Password,
                CheckDate = checkDate.ToString("ddMMyyyyHHmmss"),
                Duration = "300",
                GsmNo = gsmNo,
                IsEncrypted = "False",
                IsNotification = "True",
                Header = Constant.OperatorSenders[header.SmsSender][OperatorType.TurkTelekom],
                Message = content.Trim(),
                OnNetPortInControl = "True",
                OnNetSimChange = "True",
                PortInCheckDate = checkDate.ToString("ddMMyyyyHHmmss")

            };
        }

        private TurkTelekomSmsStatusRequest CreateSmsStatusRequest(string MessageId, string LastMessageId = "")
        {
            return new TurkTelekomSmsStatusRequest() {
                UserCode = OperatorConfig.User,
                Password = OperatorConfig.Password,
                MessageId = MessageId,
                LastMessageId = LastMessageId
            };
        }
    }
}
