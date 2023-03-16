using bbt.gateway.messaging.Api.Vodafone.Model;
using bbt.gateway.common.Models;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using bbt.gateway.messaging.Workers;
using Newtonsoft.Json;
using System;
using bbt.gateway.messaging.Helpers;

namespace bbt.gateway.messaging.Api.Vodafone
{
    public class VodafoneApiMock:BaseApi,IVodafoneApi
    {
        private IFakeSmtpHelper _fakeSmtpHelper;
        public VodafoneApiMock(IFakeSmtpHelper fakeSmtpHelper,ITransactionManager transactionManager):base(transactionManager) 
        {
            _fakeSmtpHelper = fakeSmtpHelper;
            Type = OperatorType.Vodafone;
        }

        public async Task<OperatorApiResponse> SendSms(VodafoneSmsRequest vodafoneSmsRequest)
        {
            await Task.CompletedTask;
            OperatorApiResponse vodafoneSmsResponse = new(){ OperatorType = this.Type };
            var requests = getSendSmsXml(vodafoneSmsRequest);
            
            vodafoneSmsResponse.ResponseCode = "100";
            vodafoneSmsResponse.ResponseMessage = "";
            vodafoneSmsResponse.MessageId = Guid.NewGuid().ToString();
            vodafoneSmsResponse.ResponseBody = "<Mock>Successfull</Mock>";
            vodafoneSmsResponse.RequestBody = requests.Item2;

            _fakeSmtpHelper.SendFakeMail("Vodafone@Otp.com", "Vodafone", vodafoneSmsRequest.PhoneNo+"@maildev.com", "Vodafone Otp Sms", vodafoneSmsRequest.Message, null);

            return vodafoneSmsResponse;
        }

        public async Task<OperatorApiTrackingResponse> CheckSmsStatus(VodafoneSmsStatusRequest vodafoneSmsStatusRequest)
        {
            await Task.CompletedTask;
            OperatorApiTrackingResponse vodafoneSmsStatusResponse = new(){ OperatorType = this.Type };
            
            vodafoneSmsStatusResponse.ResponseCode = "0";
            vodafoneSmsStatusResponse.ResponseMessage = "";
            vodafoneSmsStatusResponse.ResponseBody = "<Mock>Successfull</Mock>";
                    

            return vodafoneSmsStatusResponse;
        }

        public async Task<OperatorApiAuthResponse> Auth(VodafoneAuthRequest vodafoneAuthRequest)
        {
            await Task.CompletedTask;
            OperatorApiAuthResponse vodafoneAuthResponse = new();
            
            vodafoneAuthResponse.ResponseCode = "0";
            vodafoneAuthResponse.ResponseMessage = "";
            vodafoneAuthResponse.AuthToken = Guid.NewGuid().ToString();

            return vodafoneAuthResponse;
        }

        private string getAuthXml(VodafoneAuthRequest vodafoneAuthRequest)
        {
            string xml = "<soap:Envelope xmlns:soap=\"http://www.w3.org/2003/05/soap-envelope\" xmlns:aut=\"http://authentication.services.system.sdf.oksijen.com\">"
            + "<soap:Header>"
            + "<Authentication:usernamePassword xmlns:Authentication=\"Authentication\" Authentication:user=\""+vodafoneAuthRequest.User+"\" Authentication:password=\""+vodafoneAuthRequest.Password+"\"/>    </soap:Header>"
            + "<soap:Body>"
            + "<aut:authenticate/>"
            + "</soap:Body>"
            + "</soap:Envelope>";

            return xml;
        }

        private (string,string) getSendSmsXml(VodafoneSmsRequest vodafoneSmsRequest)
        {
            string xml = "<soap:Envelope xmlns:soap=\"http://www.w3.org/2003/05/soap-envelope\" xmlns:mes=\"http://messaging.packet.services.system.sdf.oksijen.com\">"
            + "<soap:Header>"
            + "<Authentication:usernamePassword Authentication:user=\""+vodafoneSmsRequest.User+"\""
            + " Authentication:sessionid=\""+vodafoneSmsRequest.AuthToken+"\" Authentication:serviceid=\"OTP\" xmlns:Authentication=\"Authentication\"/>"
            + "</soap:Header>"
            + "<soap:Body>"
            + "<mes:sendSMSPacket>"
            + "<smsPacket>"
            + "<sms>"
            + "<destinationList>"
            + "<destination>"
            + "<subscriberId>"+vodafoneSmsRequest.PhoneNo+"</subscriberId>"
            + "</destination>"
            + "</destinationList>"
            + "<message>"+vodafoneSmsRequest.Message+"</message>"
            + "<smsParameters>"
            + "<sender>"+vodafoneSmsRequest.Header+"</sender>"
            + "<shortCode xsi:nil=\"true\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"/>"
            + "<sourceMsisdn xsi:nil=\"true\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"/>                  <startDate xsi:nil=\"true\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"/>               </smsParameters>"
            + "<unifier xsi:nil=\"true\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"/>            </sms>"
            + "</smsPacket>"
            + "<traceId xsi:nil=\"true\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"/>"
            + "<correlator xsi:nil=\"true\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"/>"
            + "<notificationURL xsi:nil=\"true\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"/>         <serviceAttributes>"
            + "<attribute>"
            + "<key>SKIP_ALL_CONTROLS</key>"
            + "<value>false</value>"
            + "</attribute>"
            + "<attribute>"
            + "<key>CHECK_MNP_STATUS</key>"
            + "<value>true</value>"
            + "</attribute>"
            + "<attribute>"
            + "<key>CHECK_SIMCARD_STATUS</key>"
            + "<value>true</value>"
            + "</attribute>"
            + "<attribute>"
            + "<key>NOTIFICATION_SMS_REQUESTED</key>"
            + "<value>true</value>"
            + "</attribute>"
            + "<attribute>"
            + "<key>SENDABLE_TO_OFFNET</key>"
            + "<value>false</value>"
            + "</attribute>"
            + "<attribute>"
            + "<key>IS_ENCRYPTED</key>"
            + "<value>false</value>"
            + "</attribute>"
            + "<attribute>"
            + "<key>SIMCARD_CHANGE_PERIOD</key>"
            + "<value>"+vodafoneSmsRequest.ControlHour+"</value>"
            + "</attribute>"
            + "<attribute>"
            + "<key>OTP_EXPIRY_PERIOD</key>"
            + "<value>"+vodafoneSmsRequest.ExpiryPeriod+"</value>"
            + "</attribute>"
            + "<attribute>"
            + "<key>MNP_PERIOD</key>"
            + "<value>"+vodafoneSmsRequest.ControlHour+"</value>"
            + "</attribute>"
            + "</serviceAttributes>"
            + "</mes:sendSMSPacket>"
            + "</soap:Body>"
            + "</soap:Envelope>";

            string maskedXml = "<soap:Envelope xmlns:soap=\"http://www.w3.org/2003/05/soap-envelope\" xmlns:mes=\"http://messaging.packet.services.system.sdf.oksijen.com\">"
            + "<soap:Header>"
            + "<Authentication:usernamePassword Authentication:user=\"" + vodafoneSmsRequest.User + "\""
            + " Authentication:sessionid=\"XXXX\" Authentication:serviceid=\"OTP\" xmlns:Authentication=\"Authentication\"/>"
            + "</soap:Header>"
            + "<soap:Body>"
            + "<mes:sendSMSPacket>"
            + "<smsPacket>"
            + "<sms>"
            + "<destinationList>"
            + "<destination>"
            + "<subscriberId>" + vodafoneSmsRequest.PhoneNo + "</subscriberId>"
            + "</destination>"
            + "</destinationList>"
            + "<message>" + vodafoneSmsRequest.Message.MaskOtpContent() + "</message>"
            + "<smsParameters>"
            + "<sender>" + vodafoneSmsRequest.Header + "</sender>"
            + "<shortCode xsi:nil=\"true\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"/>"
            + "<sourceMsisdn xsi:nil=\"true\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"/>                  <startDate xsi:nil=\"true\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"/>               </smsParameters>"
            + "<unifier xsi:nil=\"true\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"/>            </sms>"
            + "</smsPacket>"
            + "<traceId xsi:nil=\"true\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"/>"
            + "<correlator xsi:nil=\"true\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"/>"
            + "<notificationURL xsi:nil=\"true\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"/>         <serviceAttributes>"
            + "<attribute>"
            + "<key>SKIP_ALL_CONTROLS</key>"
            + "<value>false</value>"
            + "</attribute>"
            + "<attribute>"
            + "<key>CHECK_MNP_STATUS</key>"
            + "<value>true</value>"
            + "</attribute>"
            + "<attribute>"
            + "<key>CHECK_SIMCARD_STATUS</key>"
            + "<value>true</value>"
            + "</attribute>"
            + "<attribute>"
            + "<key>NOTIFICATION_SMS_REQUESTED</key>"
            + "<value>true</value>"
            + "</attribute>"
            + "<attribute>"
            + "<key>SENDABLE_TO_OFFNET</key>"
            + "<value>false</value>"
            + "</attribute>"
            + "<attribute>"
            + "<key>IS_ENCRYPTED</key>"
            + "<value>false</value>"
            + "</attribute>"
            + "<attribute>"
            + "<key>SIMCARD_CHANGE_PERIOD</key>"
            + "<value>" + vodafoneSmsRequest.ControlHour + "</value>"
            + "</attribute>"
            + "<attribute>"
            + "<key>OTP_EXPIRY_PERIOD</key>"
            + "<value>" + vodafoneSmsRequest.ExpiryPeriod + "</value>"
            + "</attribute>"
            + "<attribute>"
            + "<key>MNP_PERIOD</key>"
            + "<value>" + vodafoneSmsRequest.ControlHour + "</value>"
            + "</attribute>"
            + "</serviceAttributes>"
            + "</mes:sendSMSPacket>"
            + "</soap:Body>"
            + "</soap:Envelope>";

            return (xml,maskedXml);
        }

        private string getSmsStatusXml(VodafoneSmsStatusRequest vodafoneSmsStatusRequest)
        {
            string xml = "<soap:Envelope xmlns:soap=\"http://www.w3.org/2003/05/soap-envelope\" xmlns:mes=\"http://messaging.packet.services.system.sdf.oksijen.com\">"
            + "<soap:Header>"
            + "<Authentication:usernamePassword Authentication:user=\""+ vodafoneSmsStatusRequest.User+ "\""
            + " Authentication:sessionid=\""+vodafoneSmsStatusRequest.AuthToken+"\" Authentication:serviceid=\"OTP\" xmlns:Authentication=\"Authentication\"/>"
            + "</soap:Header>"
            + "<soap:Body>"
            + "<mes:queryPacketStatus>"
            + "<packetId>" + vodafoneSmsStatusRequest.MessageId.Substring(0, vodafoneSmsStatusRequest.MessageId.Length-13) + "</packetId>"
            + "<messageId>" + vodafoneSmsStatusRequest.MessageId + "</messageId>"
            + "<transactionIdList>"
            + "<transactionId>" + vodafoneSmsStatusRequest.MessageId + "</transactionId>"
            + "</transactionIdList>"
            + "</mes:queryPacketStatus>"
            + "</soap:Body>"
            + "</soap:Envelope>";

            return xml;
        }
        
    }
}
