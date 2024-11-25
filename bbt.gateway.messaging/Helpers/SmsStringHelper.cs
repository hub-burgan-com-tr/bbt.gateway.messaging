using bbt.gateway.common.Models.v2;
using bbt.gateway.messaging.Workers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Helpers
{
    public class SmsStringHelper
    {
        private readonly OtpSender _otpSender;
        private readonly ITransactionManager _transactionManager;
        private readonly dEngageSender _dEngageSender;
        private readonly CodecSender _codecSender;
        private readonly InfobipSender _infobipSender;
        public SmsStringHelper(OtpSender otpSender,
                                ITransactionManager transactionManager,
                                dEngageSender dEngageSender,
                                CodecSender codecSender,
                                InfobipSender infobipSender)
        {
            _transactionManager = transactionManager;
            _otpSender = otpSender;
            _dEngageSender = dEngageSender;
            _codecSender = codecSender;
            _infobipSender = infobipSender;
        }

        public async Task<IActionResult> ProcessSmsRequestAsync(SmsRequest data)
        {
            var codecOperator = await _transactionManager.GetOperatorAsync(common.Models.OperatorType.Codec);
            var infobipOperator = await _transactionManager.GetOperatorAsync(common.Models.OperatorType.Infobip);
            if (data.SmsType == SmsTypes.Otp)
            {
                if (data.Phone.CountryCode != 90)
                {
                    if (infobipOperator?.Status == common.Models.OperatorStatus.Active)
                    {
                        return new OkObjectResult(await _infobipSender.SendSms(data));
                    }

                    return new OkObjectResult(await _otpSender.SendMessageV2(data));
                }
                else
                {
                    return new OkObjectResult(await _otpSender.SendMessageV2(data));
                }
            }
            else
            {
                if ((data.Phone.CountryCode != 90 || _transactionManager.SmsRequestInfo?.PhoneConfiguration?.Operator == common.Models.OperatorType.Foreign) && infobipOperator?.Status == common.Models.OperatorStatus.Active)
                {
                    return new OkObjectResult(await _infobipSender.SendSms(data));
                }

                if (codecOperator.Status == common.Models.OperatorStatus.Active)
                {
                    return new OkObjectResult(await _codecSender.SendSmsV2(data));
                }
                else
                {
                    return new OkObjectResult(await _dEngageSender.SendSmsV2(data));
                }
            }
        }
    }
}