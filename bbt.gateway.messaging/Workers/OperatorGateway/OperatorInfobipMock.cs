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
    public class OperatorInfobipMock : OperatorGatewayBase,IOperatorInfobip
    {
        private readonly IInfobipApi _infobipApi;
        private string _authToken;

        public OperatorInfobipMock(IInfobipApi infobipApi, IConfiguration configuration,
            ITransactionManager transactionManager) : base(configuration, transactionManager)
        {
            _infobipApi = infobipApi;
            Type = OperatorType.Infobip;
            _infobipApi.SetOperatorType(OperatorConfig);
        }

        public async Task<InfobipApiSmsStatusResponse> CheckSms(string refId)
        {
            await Task.CompletedTask;
            return new InfobipApiSmsStatusResponse();

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

            smsResponseLog.Status = "Mock";
            smsResponseLog.StatusQueryId = "";
            smsResponseLog.OperatorResponseCode = 0;
            smsResponseLog.OperatorResponseMessage = "Mock Successfull";

            otpResponseLog.StatusQueryId = "";
            otpResponseLog.ResponseCode = SendSmsResponseStatus.Success;
            otpResponseLog.ResponseMessage = "Mock Successfull";
            otpResponseLog.RequestBody = "";
            otpResponseLog.ResponseBody = "";
            otpResponseLog.TrackingStatus = SmsTrackingStatus.Pending;

            return (smsResponseLog, otpResponseLog);
        }

        private string GetSender()
        {
            return TransactionManager.CustomerRequestInfo.BusinessLine == "X" ? "ON Dijital" : "BURGAN BANK";
        }
    }
}
