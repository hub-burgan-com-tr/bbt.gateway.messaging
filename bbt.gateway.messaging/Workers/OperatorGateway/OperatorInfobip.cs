using bbt.gateway.common.Models;
using bbt.gateway.messaging.Api.Infobip;
using bbt.gateway.messaging.Api.Infobip.Model;
using bbt.gateway.messaging.Api.Infobip.Model.SendSms;
using bbt.gateway.messaging.Api.Infobip.Model.SmsStatus;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Workers.OperatorGateway
{
    public class OperatorInfobip : OperatorGatewayBase
    {
        private readonly IInfobipApi _infobipApi;
        private string _authToken;

        public OperatorInfobip(IInfobipApi infobipApi, IConfiguration configuration,
            ITransactionManager transactionManager) : base(configuration, transactionManager)
        {
            _infobipApi = infobipApi;
            Type = OperatorType.Infobip;
            _infobipApi.SetOperatorType(OperatorConfig);
        }

        public async Task<InfobipApiSmsStatusResponse> CheckSms(string refId)
        {

            try
            {
                var res = await _infobipApi.CheckSmsStatus(new InfobipSmsStatusRequest { MessageId = refId });
                return res;
            }
            catch (Exception ex)
            {
                TransactionManager.LogError($"Couldn't get Infobip Sms Status  | Exception : {ex}");
                return null;
            }

        }


        public async Task<(SmsResponseLog, OtpResponseLog)> SendSms(Phone phone, string content)
        {
            SmsResponseLog smsResponseLog = new()
            {
                CreatedAt = DateTime.Now,
                Operator = Type,
                Content = String.IsNullOrEmpty(content) ? "" : content.ClearMaskingFields()
            };
            OtpResponseLog otpResponseLog = new()
            {
                CreatedAt = DateTime.Now,
                Operator = Type
            };

            InfobipApiSmsResponse response = new();
            try
            {
                response = await _infobipApi.SendSms(CreateSmsRequest(content, phone));
                if (response.IsSuccess)
                {
                    smsResponseLog.Status = String.Empty;
                    smsResponseLog.StatusQueryId = response.MsgId;
                    smsResponseLog.OperatorResponseCode = 0;
                    smsResponseLog.OperatorResponseMessage = "Successfull";

                    otpResponseLog.StatusQueryId = response.MsgId;
                    otpResponseLog.ResponseCode = SendSmsResponseStatus.Success;
                    otpResponseLog.ResponseMessage = "Successfull";
                    otpResponseLog.RequestBody = response.RequestBody;
                    otpResponseLog.ResponseBody = response.ResponseBody;
                    otpResponseLog.TrackingStatus = SmsTrackingStatus.Pending;
                }
                else
                {
                    smsResponseLog.Status = String.Empty;
                    smsResponseLog.StatusQueryId = response.MsgId;
                    smsResponseLog.OperatorResponseCode = -9999;
                    smsResponseLog.OperatorResponseMessage = "Failed";

                    otpResponseLog.StatusQueryId = response.MsgId;
                    otpResponseLog.ResponseCode = SendSmsResponseStatus.ClientError;
                    otpResponseLog.ResponseMessage = "Failed";
                    otpResponseLog.RequestBody = response.RequestBody;
                    otpResponseLog.ResponseBody = response.ResponseBody;
                    otpResponseLog.TrackingStatus = SmsTrackingStatus.Pending;
                }
            }
            catch (Exception ex)
            {
                smsResponseLog.OperatorResponseCode = -99999;
                smsResponseLog.OperatorResponseMessage = ex.ToString();

                otpResponseLog.ResponseCode = SendSmsResponseStatus.ClientError;
                otpResponseLog.ResponseMessage = ex.ToString();
                otpResponseLog.RequestBody = response.RequestBody;
                otpResponseLog.ResponseBody = response.ResponseBody;
            }

            return (smsResponseLog, otpResponseLog);
        }

        private string GetSender()
        {
            return TransactionManager.CustomerRequestInfo.BusinessLine == "X" ? "On." : "BURGAN";
        }

        private InfobipSmsRequest CreateSmsRequest(string message, Phone phone)
        {
            string phoneNumber;
            InfobipSmsRequest infoBipSmsRequest = new InfobipSmsRequest();

            List<Destination> destinations = new List<Destination>()
            {
                new Destination() { to = TransactionManager.StringSend ? phone.ConcatenateStringSend(0,TransactionManager.PrefixLength,TransactionManager.NumberLength) : phone.Concatenate()}
            };
            infoBipSmsRequest.messages = new List<Message>()
            {
                new Message() { destinations = destinations, from = GetSender(), text = message }
            };

            return infoBipSmsRequest;
        }
    }
}
