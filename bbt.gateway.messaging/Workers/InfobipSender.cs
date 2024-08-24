using bbt.gateway.common.Extensions;
using bbt.gateway.common.Models;
using bbt.gateway.common.Models.v2;
using bbt.gateway.common.Repositories;
using bbt.gateway.messaging.Helpers;
using bbt.gateway.messaging.Workers.OperatorGateway;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Workers
{
    public class InfobipSender
    {
        private readonly HeaderManager _headerManager;
        private readonly IRepositoryManager _repositoryManager;
        private readonly ITransactionManager _transactionManager;
        private readonly IOperatorInfobip _operatorInfobip;
        private readonly InstantReminder _instantReminder;
        public InfobipSender(HeaderManager headerManager,
            IRepositoryManager repositoryManager,
            ITransactionManager transactionManager,
            InfobipFactory infobipFactory,
            InstantReminder instantReminder
        )
        {
            _headerManager = headerManager;
            _repositoryManager = repositoryManager;
            _transactionManager = transactionManager;
            _operatorInfobip = infobipFactory(_transactionManager.UseFakeSmtp);
            _instantReminder = instantReminder;
        }

        public async Task<SmsTrackingLog> CheckSms(common.Models.v2.CheckFastSmsRequest checkFastSmsRequest)
        {
            //Operator Cache Fix
            //_operatorInfobip.Type = OperatorType.Infobip;
            await _operatorInfobip.GetOperatorAsync(OperatorType.Infobip);
            var response = await _operatorInfobip.CheckSms(checkFastSmsRequest.StatusQueryId);

            return response.BuildInfobipTrackingResponse(checkFastSmsRequest);
        }

        public async Task<OtpTrackingLog> CheckSms(common.Models.CheckSmsRequest checkSmsRequest)
        {
            //Operator Cache Fix
            //_operatorInfobip.Type = OperatorType.Infobip;
            await _operatorInfobip.GetOperatorAsync(OperatorType.Infobip);
            var response = await _operatorInfobip.CheckSms(checkSmsRequest.StatusQueryId);

            return response.BuildInfobipTrackingResponse(checkSmsRequest);
        }

        public async Task<InfobipSmsResponse> SendSms(common.Models.v2.SmsRequest sendSmsRequest)
        {
            InfobipSmsResponse sendSmsResponse = new InfobipSmsResponse()
            {
                TxnId = _transactionManager.TxnId,
            };

            var header =  _headerManager.Get(sendSmsRequest.SmsType);

            if (sendSmsRequest.Sender != common.Models.v2.SenderType.AutoDetect)
                _transactionManager.CustomerRequestInfo.BusinessLine = sendSmsRequest.Sender == common.Models.v2.SenderType.On ? "X" : "B";

            //Operator Cache Fix
            //_operatorInfobip.Type = OperatorType.Infobip;
            await _operatorInfobip.GetOperatorAsync(OperatorType.Infobip);

            var smsRequest = new SmsRequestLog()
            {
                Operator = _operatorInfobip.Type,
                content = header.SmsPrefix + " " + sendSmsRequest.Content.MaskFields() + " " + header.SmsSuffix,
                TemplateId = "",
                TemplateParams = "",
                SmsType = (common.Models.SmsTypes)sendSmsRequest.SmsType,
                Phone = new() { CountryCode = sendSmsRequest.Phone.CountryCode, Prefix = sendSmsRequest.Phone.Prefix, Number = sendSmsRequest.Phone.Number },
                CreatedBy = sendSmsRequest.Process.MapTo<common.Models.Process>()
            };

            var otpRequest = new OtpRequestLog()
            {
                Content = header.SmsPrefix + " " + sendSmsRequest.Content.MaskOtpContent() + " " + header.SmsSuffix,
                CreatedBy = sendSmsRequest.Process.MapTo<common.Models.Process>(),
                Phone = new() { CountryCode = sendSmsRequest.Phone.CountryCode, Prefix = sendSmsRequest.Phone.Prefix, Number = sendSmsRequest.Phone.Number },
                PhoneConfiguration = _transactionManager.OtpRequestInfo.PhoneConfiguration
            };

            if (_transactionManager.Transaction.TransactionType == TransactionType.Otp)
            {
                await _repositoryManager.OtpRequestLogs.AddAsync(otpRequest);
                _transactionManager.Transaction.OtpRequestLog = otpRequest;
            }
            else
            {   
                smsRequest.PhoneConfiguration = _transactionManager.SmsRequestInfo.PhoneConfiguration;
                _transactionManager.Transaction.SmsRequestLog = smsRequest;

                await _repositoryManager.SmsRequestLogs.AddAsync(smsRequest);
            }
            
            
            var response = await _operatorInfobip.SendSms(sendSmsRequest.Phone.MapTo<common.Models.Phone>(), header.BuildContentForSms(sendSmsRequest.Content));

            if (_transactionManager.Transaction.TransactionType == TransactionType.Otp)
            {
                otpRequest.ResponseLogs.Add(response.Item2);
                sendSmsResponse.Status = response.Item2.ResponseCode == SendSmsResponseStatus.Success ? InfobipResponseCodes.Success : InfobipResponseCodes.Error;
                if (response.Item2 != null && response.Item2.ResponseCode == SendSmsResponseStatus.Success)
                {
                    await _instantReminder.RemindAsync($"{_operatorInfobip.Type} Otp Sms | {sendSmsRequest.Phone.MapTo<common.Models.Phone>().Concatenate()}", sendSmsRequest.Content, null);
                }
            }
            else
            {
                smsRequest.ResponseLogs.Add(response.Item1);
                sendSmsResponse.Status = response.Item1.OperatorResponseCode ==  0 ? InfobipResponseCodes.Success : InfobipResponseCodes.Error;
                if (response.Item1 != null && response.Item1.OperatorResponseCode == 0)
                {
                    await _instantReminder.RemindAsync($"{_operatorInfobip.Type} Fast Sms | {sendSmsRequest.Phone.MapTo<common.Models.Phone>().Concatenate()}", sendSmsRequest.Content, null);
                }
            }

            
            return sendSmsResponse;
        }


       

    }



}
