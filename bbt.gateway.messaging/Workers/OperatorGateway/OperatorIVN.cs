using bbt.gateway.common.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Workers.OperatorGateway
{
    public class OperatorIVN : OperatorGatewayBase, IOperatorGateway
    {
        public OperatorIVN(IConfiguration configuration,ITransactionManager transactionManager) : base(configuration,transactionManager)
        {
            Type = OperatorType.IVN;
        }


        public async Task<bool> SendOtp(Phone phone, string content, ConcurrentBag<OtpResponseLog> responses, Header header)
        {
            await Task.CompletedTask;
            var response = new OtpResponseLog { 
                Operator = OperatorType.Turkcell,
                Topic = "IVN otp sending",
                TrackingStatus = SmsTrackingStatus.Delivered
            };

            System.Diagnostics.Debug.WriteLine("IVN otp is send");
            response.ResponseCode = SendSmsResponseStatus.NotSubscriber;
            responses.Add(response);
            return true;
        }

      

        public async Task<OtpResponseLog> SendOtp(Phone phone, string content, Header header)
        {
           await Task.CompletedTask;
           var response = new OtpResponseLog { 
                Operator = OperatorType.IVN,
                Topic = "IVN otp sending",
                TrackingStatus = SmsTrackingStatus.Delivered
            };

            System.Diagnostics.Debug.WriteLine("IVN otp is send");
            response.ResponseCode = SendSmsResponseStatus.NotSubscriber;

            return response;
        }

        public async Task<OtpResponseLog> SendOtpForeign(Phone phone, string content, Header header)
        {
            await Task.CompletedTask;
            var response = new OtpResponseLog
            {
                Operator = OperatorType.IVN,
                Topic = "IVN otp sending",
                TrackingStatus = SmsTrackingStatus.Delivered
            };

            System.Diagnostics.Debug.WriteLine("IVN otp is send");
            response.ResponseCode = SendSmsResponseStatus.NotSubscriber;

            return response;
        }

        public async Task<OtpTrackingLog> CheckMessageStatus(CheckSmsRequest checkSmsRequest)
        {
           await Task.CompletedTask;
           throw new NotSupportedException();
        }

    }
}
