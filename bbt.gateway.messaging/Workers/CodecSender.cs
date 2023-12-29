using bbt.gateway.common.Extensions;
using bbt.gateway.common.Models;
using bbt.gateway.common.Repositories;
using bbt.gateway.messaging.Helpers;
using bbt.gateway.messaging.Workers.OperatorGateway;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Workers
{
    public class CodecSender
    {
        private readonly HeaderManager _headerManager;
        private readonly IRepositoryManager _repositoryManager;
        private readonly ITransactionManager _transactionManager;
        private readonly IOperatorCodec _operatorCodec;
        private readonly InstantReminder _instantReminder;

        public CodecSender(HeaderManager headerManager,
            CodecFactory codecFactory,
            IRepositoryManager repositoryManager,
            ITransactionManager transactionManager,
            OperatorCodec operatorCodec,
            InstantReminder instantReminder
        )
        {
            _headerManager = headerManager;
            _repositoryManager = repositoryManager;
            _transactionManager = transactionManager;
            _operatorCodec = codecFactory(_transactionManager.UseFakeSmtp);
        }

        public async Task<SmsTrackingLog> CheckSms(common.Models.v2.CheckFastSmsRequest checkFastSmsRequest)
        {
            _operatorCodec.Type = OperatorType.Codec;
            var response = await _operatorCodec.CheckSms(checkFastSmsRequest.StatusQueryId);

            if (response != null)
            {
                if (response.ResultSet.Code == 0)
                {
                    return response.BuildCodecTrackingResponse(checkFastSmsRequest);
                }
                else
                {
                    return new SmsTrackingLog()
                    {
                        Id = System.Guid.NewGuid(),
                        LogId = checkFastSmsRequest.SmsRequestLogId,
                        Status = SmsTrackingStatus.SystemError,
                        Detail = "",
                        StatusReason = $"Codec operatöründen bilgi alınamadı. Response Code : {response.ResultSet.Code} | Response Message : {response.ResultSet.Description}",

                    };
                }
            }
            else
            {
                return new SmsTrackingLog()
                {
                    Id = System.Guid.NewGuid(),
                    LogId = checkFastSmsRequest.SmsRequestLogId,
                    Status = SmsTrackingStatus.SystemError,
                    Detail = "",
                    StatusReason = "Codec operatöründen bilgi alınamadı.",

                };
            }

        }

        public async Task<SendCodecSmsResponse> SendSms(SendMessageSmsRequest sendMessageSmsRequest)
        {
            SendCodecSmsResponse sendSmsResponse = new SendCodecSmsResponse()
            {
                TxnId = _transactionManager.TxnId,
            };

            var header = await _headerManager.Get(sendMessageSmsRequest.ContentType, sendMessageSmsRequest.HeaderInfo);

            _operatorCodec.Type = OperatorType.Codec;

            var smsRequest = new SmsRequestLog()
            {
                Operator = _operatorCodec.Type,
                Phone = sendMessageSmsRequest.Phone,
                content = header.SmsPrefix + " " + sendMessageSmsRequest.Content.MaskFields() + " " + header.SmsSuffix,
                TemplateId = "",
                TemplateParams = "",
                SmsType = sendMessageSmsRequest.SmsType,
                CreatedBy = sendMessageSmsRequest.Process
            };
            smsRequest.PhoneConfiguration = _transactionManager.SmsRequestInfo.PhoneConfiguration;

            await _repositoryManager.SmsRequestLogs.AddAsync(smsRequest);

            _transactionManager.Transaction.SmsRequestLog = smsRequest;

            var response = await _operatorCodec.SendSms(sendMessageSmsRequest.Phone, header.BuildContentForSms(sendMessageSmsRequest.Content));

            smsRequest.ResponseLogs.Add(response);

            sendSmsResponse.Status = response.GetCodecStatus();

            

            return sendSmsResponse;
        }


        public async Task<SendCodecSmsResponse> SendSmsV2(common.Models.v2.SmsRequest sendSmsRequest)
        {
            SendCodecSmsResponse sendSmsResponse = new()
            {
                TxnId = _transactionManager.TxnId,
            };

            var header = _headerManager.Get(sendSmsRequest.SmsType);

            if (sendSmsRequest.Sender != common.Models.v2.SenderType.AutoDetect)
                _transactionManager.CustomerRequestInfo.BusinessLine = sendSmsRequest.Sender == common.Models.v2.SenderType.On ? "X" : "B";

            _operatorCodec.Type = OperatorType.Codec;

            var smsRequest = new SmsRequestLog()
            {
                Operator = _operatorCodec.Type,
                Phone = new() { CountryCode = sendSmsRequest.Phone.CountryCode, Prefix = sendSmsRequest.Phone.Prefix, Number = sendSmsRequest.Phone.Number },
                content = sendSmsRequest.Content.MaskFields(),
                TemplateId = "",
                TemplateParams = "",
                CreatedBy = sendSmsRequest.Process.MapTo<Process>()
            };

            await _repositoryManager.SmsRequestLogs.AddAsync(smsRequest);
            smsRequest.PhoneConfiguration = _transactionManager.SmsRequestInfo.PhoneConfiguration;
            _transactionManager.Transaction.SmsRequestLog = smsRequest;

            var response = await _operatorCodec.SendSms(sendSmsRequest.Phone.MapTo<Phone>(), header.BuildContentForSms(sendSmsRequest.Content));

            smsRequest.ResponseLogs.Add(response);


            sendSmsResponse.Status = response.GetCodecStatus();

            if (response != null && response.OperatorResponseCode == 0)
            {
                await _instantReminder.RemindAsync($"{_operatorCodec.Type} Fast Sms | {sendSmsRequest.Phone.MapTo<Phone>().Concatenate()}", sendSmsRequest.Content, null);
            }

            return sendSmsResponse;
        }

    }



}
