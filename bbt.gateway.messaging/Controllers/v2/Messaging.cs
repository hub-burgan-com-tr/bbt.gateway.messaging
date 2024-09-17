using bbt.gateway.common.Api.MessagingGateway;
using bbt.gateway.common.Models.v2;
using bbt.gateway.common.Repositories;
using bbt.gateway.messaging.Workers;
using Elastic.Apm.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Refit;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Controllers.v2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public class Messaging : ControllerBase
    {
        private readonly OtpSender _otpSender;
        private readonly ITransactionManager _transactionManager;
        private readonly dEngageSender _dEngageSender;
        private readonly CodecSender _codecSender;
        private readonly InfobipSender _infobipSender;
        private readonly FirebaseSender _firebaseSender;
        private readonly IRepositoryManager _repositoryManager;
        private readonly ITracer _tracer;
        private readonly IConfiguration _configuration;
        private readonly IMessagingGatewayApi _messagingGatewayApi;
        public Messaging(OtpSender otpSender, ITransactionManager transactionManager, dEngageSender dEngageSender, FirebaseSender firebaseSender
            , IRepositoryManager repositoryManager, CodecSender codecSender, IConfiguration configuration, IMessagingGatewayApi messagingGatewayApi
            , InfobipSender infobipSender)
        {
            _transactionManager = transactionManager;
            _otpSender = otpSender;
            _dEngageSender = dEngageSender;
            _codecSender = codecSender;
            _infobipSender = infobipSender;
            _repositoryManager = repositoryManager;
            _configuration = configuration;
            _messagingGatewayApi = messagingGatewayApi;
            _firebaseSender = firebaseSender;
            _tracer = Elastic.Apm.Agent.Tracer;
        }

        private async Task<IActionResult> ProcessSmsRequestAsync(SmsRequest data)
        {
            if (ModelState.IsValid)
            {
                var codecOperator = await _transactionManager.GetOperatorAsync(common.Models.OperatorType.Codec);
                var infobipOperator = await _transactionManager.GetOperatorAsync(common.Models.OperatorType.Infobip);
                if (data.SmsType == SmsTypes.Otp)
                {

                    if (data.Phone.CountryCode != 90)
                    {
                        if (infobipOperator?.Status == common.Models.OperatorStatus.Active)
                        {
                            return Ok(await _infobipSender.SendSms(data));
                        }

                        return Ok(await _otpSender.SendMessageV2(data));
                    }
                    else
                    {
                        return Ok(await _otpSender.SendMessageV2(data));
                    }
                }
                else
                {

                    if ((data.Phone.CountryCode != 90 || _transactionManager.SmsRequestInfo?.PhoneConfiguration?.Operator == common.Models.OperatorType.Foreign) && infobipOperator?.Status == common.Models.OperatorStatus.Active)
                    {

                        return Ok(await _infobipSender.SendSms(data));

                    }

                    if (codecOperator.Status == common.Models.OperatorStatus.Active)
                    {

                        return Ok(await _codecSender.SendSmsV2(data));
                    }
                    else
                    {

                        return Ok(await _dEngageSender.SendSmsV2(data));

                    }
                }
            }
            else
            {
                _transactionManager.LogError("Model State is Not Valid | " +
                    string.Join("|", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                return BadRequest(ModelState);
            }
        }

        [SwaggerOperation(
            Summary = "Send Templated Sms Message",
            Description = ""
            + "<div>To Send Sms With Template Which Defined On dEngage Use This Method</div>"
            + "<div>Sender,Template,Phone,Process Fields are Mandatory</div>"
            + "<div>When Customer Type(Burgan/On) is Not Known, Sender Field Must Be Set To AutoDetect"
            + " <br />Otherwise This Field Must Be Set Burgan or On</div>"
            + "<div>Given Template Must Be Defined On Both Of dEngage Tenants(On And Burgan) With Same Content Name</div>"
            + "<div>Template Params Must Be Set As JsonString Which Serialized From Dynamic Fields That Given Template Contains"
            + "<br />Example : Let's Assume template content is 'Welcome {%=$Current.Name%} {%=$Current.Surname%}.' "
            + "<br />In This Case Template Params Must Be Set To {\"Name\":\"Actual Name\",\"Surname\":\"Actual Surname\"}</div>"
            + "<div>TemplateParams Field Will Be Logged After This Method Called. If You Need To Masking Critical Information You Should Surround"
            + " Critical Information with &lt;Mask&gt;&lt;/Mask&gt; . "
            + "<br />Example : {\"Password\",\"&lt;Mask&gt;123456&lt;/Mask&gt;\"}</div>"
            + "",
            Tags = new[] { "Sms" }
            )]
        [HttpPost("sms/templated")]
        [SwaggerRequestExample(typeof(TemplatedSmsRequest), typeof(TemplatedSmsRequestExampleFilter))]
        [SwaggerResponse(200, "Sms was sent successfully", typeof(TemplatedSmsResponse))]
        [SwaggerResponse(400, "Bad Request", typeof(TemplatedSmsResponse))]
        [SwaggerResponse(401, "Unauthorized", typeof(TemplatedSmsResponse))]
        [SwaggerResponse(403, "Not Allowed", typeof(TemplatedSmsResponse))]
        [SwaggerResponse(404, "Not Found", typeof(TemplatedSmsResponse))]
        [SwaggerResponse(429, "Too Many Request", typeof(TemplatedSmsResponse))]
        [SwaggerResponse(450, "Given template is not found on dEngage", typeof(void))]
        [SwaggerResponse(451, "Customer Not Found.", typeof(void))]
        [SwaggerResponse(500, "Internal Server Error. Get Contact With Integration", typeof(void))]
        public async Task<IActionResult> SendTemplatedSms([FromBody] TemplatedSmsRequest data)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Mock")
            {
                return Ok(new TemplatedSmsResponse()
                {
                    Status = dEngageResponseCodes.Success,
                    TxnId = Guid.NewGuid(),
                });
            }
            if (data.Phone == null)
            {
                data.Phone = new Phone
                {
                    CountryCode = _transactionManager.CustomerRequestInfo.MainPhone.CountryCode,
                    Prefix = _transactionManager.CustomerRequestInfo.MainPhone.Prefix,
                    Number = _transactionManager.CustomerRequestInfo.MainPhone.Number,
                };
            }

            return Ok(await _dEngageSender.SendTemplatedSmsV2(data));
        }

        [SwaggerOperation(
           Summary = "Send Sms message",
           Description = ""
            + "<div>To Send Sms With Plain Text Use This Method</div>"
            + "<div>Sender,SmsType,Content,Phone,Process Fields are Mandatory</div>"
            + "<div>When Customer Type(Burgan/On) is Not Known, Sender Field Should Be Set To AutoDetect"
            + " <br />Otherwise This Field Must Be Set Burgan or On</div>"
            + "<div>You can use advantages of headers by creating header from Header Management Services"
            + "<br /> When message services are called, we get customer info(BusinessLine,BranchCode)"
            + "<br /> Then try to find header matches with (BusinessLine[BL],BranchCode[BC],SmsType[ST])"
            + "<br /> Match Order by priority is (BL-BC-ST)-(BL-ST)-(BL-SC)-(BL)"
            + "<br /> If any header is matches, we add matched header prefix and suffix to beginning of message and end of the message</div>"
            + "<div>Content Field Will Be Logged After This Method Called. If You Need To Masking Critical Information You Should Surround"
            + " Critical Information with &lt;Mask&gt;&lt;/Mask&gt; . "
            + "<br />Example : \"Content\" : \"Your password is &lt;Mask&gt;1000&lt;/Mask&gt;.\"</div>"
            + "",
           Tags = new[] { "Sms" }
           )]
        [HttpPost("sms/message")]
        [SwaggerRequestExample(typeof(SmsRequest), typeof(SmsRequestExampleFilter))]
        [SwaggerResponse(200, "Sms was sent successfully", typeof(SmsResponse))]
        [SwaggerResponse(200, "Sms was sent successfully", typeof(OtpResponse))]
        [SwaggerResponse(400, "Bad Request", typeof(SmsResponse))]
        [SwaggerResponse(401, "Unauthorized", typeof(SmsResponse))]
        [SwaggerResponse(403, "Not Allowed", typeof(SmsResponse))]
        [SwaggerResponse(404, "Not Found", typeof(SmsResponse))]
        [SwaggerResponse(429, "Too Many Request", typeof(SmsResponse))]
        [SwaggerResponse(450, "Given template is not found on dEngage", typeof(void))]
        [SwaggerResponse(451, "Customer Not Found.", typeof(void))]
        [SwaggerResponse(460, "Has Blacklist Record", typeof(OtpResponse))]
        [SwaggerResponse(461, "Sim Change", typeof(OtpResponse))]
        [SwaggerResponse(462, "Operator Change", typeof(OtpResponse))]
        [SwaggerResponse(463, "Rejected By Operator", typeof(OtpResponse))]
        [SwaggerResponse(464, "Not Subscriber", typeof(OtpResponse))]
        [SwaggerResponse(465, "Client Error", typeof(OtpResponse))]
        [SwaggerResponse(466, "Server Error", typeof(OtpResponse))]
        [SwaggerResponse(467, "Maximum Characters Count Exceed", typeof(OtpResponse))]
        [SwaggerResponse(500, "Internal Server Error. Get Contact With Integration", typeof(void))]
        public async Task<IActionResult> SendMessageSms([FromBody] SmsRequest data)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Mock")
            {
                return Ok(new SmsResponse()
                {
                    Status = dEngageResponseCodes.Success,
                    TxnId = Guid.NewGuid(),
                });
            }

            if (data.Phone == null)
            {
                data.Phone = new Phone
                {
                    CountryCode = _transactionManager.CustomerRequestInfo.MainPhone.CountryCode,
                    Prefix = _transactionManager.CustomerRequestInfo.MainPhone.Prefix,
                    Number = _transactionManager.CustomerRequestInfo.MainPhone.Number,
                };
            }

            return await ProcessSmsRequestAsync(data);

        }

        [SwaggerOperation(
           Summary = "Send Sms message",
           Description = ""
            + "<div>To Send Sms With Plain Text Use This Method</div>"
            + "<div>Sender,SmsType,Content,Phone,Process Fields are Mandatory</div>"
            + "<div>When Customer Type(Burgan/On) is Not Known, Sender Field Should Be Set To AutoDetect"
            + " <br />Otherwise This Field Must Be Set Burgan or On</div>"
            + "<div>You can use advantages of headers by creating header from Header Management Services"
            + "<br /> When message services are called, we get customer info(BusinessLine,BranchCode)"
            + "<br /> Then try to find header matches with (BusinessLine[BL],BranchCode[BC],SmsType[ST])"
            + "<br /> Match Order by priority is (BL-BC-ST)-(BL-ST)-(BL-SC)-(BL)"
            + "<br /> If any header is matches, we add matched header prefix and suffix to beginning of message and end of the message</div>"
            + "<div>Content Field Will Be Logged After This Method Called. If You Need To Masking Critical Information You Should Surround"
            + " Critical Information with &lt;Mask&gt;&lt;/Mask&gt; . "
            + "<br />Example : \"Content\" : \"Your password is &lt;Mask&gt;1000&lt;/Mask&gt;.\"</div>"
            + "",
           Tags = new[] { "Sms" }
           )]
        [HttpPost("sms/message/codec")]
        [SwaggerRequestExample(typeof(SmsRequest), typeof(SmsRequestExampleFilter))]
        [SwaggerResponse(200, "Sms was sent successfully", typeof(SmsResponse))]
        [SwaggerResponse(200, "Sms was sent successfully", typeof(OtpResponse))]
        [SwaggerResponse(400, "Bad Request", typeof(SmsResponse))]
        [SwaggerResponse(401, "Unauthorized", typeof(SmsResponse))]
        [SwaggerResponse(403, "Not Allowed", typeof(SmsResponse))]
        [SwaggerResponse(404, "Not Found", typeof(SmsResponse))]
        [SwaggerResponse(429, "Too Many Request", typeof(SmsResponse))]
        [SwaggerResponse(450, "Given template is not found on dEngage", typeof(void))]
        [SwaggerResponse(451, "Customer Not Found.", typeof(void))]
        [SwaggerResponse(460, "Has Blacklist Record", typeof(OtpResponse))]
        [SwaggerResponse(461, "Sim Change", typeof(OtpResponse))]
        [SwaggerResponse(462, "Operator Change", typeof(OtpResponse))]
        [SwaggerResponse(463, "Rejected By Operator", typeof(OtpResponse))]
        [SwaggerResponse(464, "Not Subscriber", typeof(OtpResponse))]
        [SwaggerResponse(465, "Client Error", typeof(OtpResponse))]
        [SwaggerResponse(466, "Server Error", typeof(OtpResponse))]
        [SwaggerResponse(467, "Maximum Characters Count Exceed", typeof(OtpResponse))]
        [SwaggerResponse(500, "Internal Server Error. Get Contact With Integration", typeof(void))]
        public async Task<IActionResult> SendMessageSmsCodec([FromBody] SmsRequest data)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Mock")
            {
                return Ok(new SmsResponse()
                {
                    Status = dEngageResponseCodes.Success,
                    TxnId = Guid.NewGuid(),
                });
            }

            if (data.Phone == null)
            {
                data.Phone = new Phone
                {
                    CountryCode = _transactionManager.CustomerRequestInfo.MainPhone.CountryCode,
                    Prefix = _transactionManager.CustomerRequestInfo.MainPhone.Prefix,
                    Number = _transactionManager.CustomerRequestInfo.MainPhone.Number,
                };
            }

            if (ModelState.IsValid)
            {
                if (data.SmsType == SmsTypes.Otp)
                {
                    return Ok(await _otpSender.SendMessageV2(data));
                }
                else
                {

                    return Ok(await _codecSender.SendSmsV2(data));

                }
            }
            else
            {
                _transactionManager.LogError("Model State is Not Valid | " +
                    string.Join("|", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                return BadRequest(ModelState);
            }

        }

        [SwaggerOperation(
            Summary = "Send Templated Sms Message",
            Description = ""
            + "<div>To Send Sms With Template Which Defined On dEngage Use This Method</div>"
            + "<div>Sender,Template,Phone,Process Fields are Mandatory</div>"
            + "<div>When Customer Type(Burgan/On) is Not Known, Sender Field Must Be Set To AutoDetect"
            + " <br />Otherwise This Field Must Be Set Burgan or On</div>"
            + "<div>Given Template Must Be Defined On Both Of dEngage Tenants(On And Burgan) With Same Content Name</div>"
            + "<div>Template Params Must Be Set As JsonString Which Serialized From Dynamic Fields That Given Template Contains"
            + "<br />Example : Let's Assume template content is 'Welcome {%=$Current.Name%} {%=$Current.Surname%}.' "
            + "<br />In This Case Template Params Must Be Set To {\"Name\":\"Actual Name\",\"Surname\":\"Actual Surname\"}</div>"
            + "<div>TemplateParams Field Will Be Logged After This Method Called. If You Need To Masking Critical Information You Should Surround"
            + " Critical Information with &lt;Mask&gt;&lt;/Mask&gt; . "
            + "<br />Example : {\"Password\",\"&lt;Mask&gt;123456&lt;/Mask&gt;\"}</div>"
            + "",
            Tags = new[] { "Sms" }
            )]
        [HttpPost("sms/templated/string")]
        [SwaggerRequestExample(typeof(TemplatedSmsRequestString), typeof(TemplatedSmsRequestStringExampleFilter))]
        [SwaggerResponse(200, "Sms was sent successfully", typeof(TemplatedSmsResponse))]
        [SwaggerResponse(400, "Bad Request", typeof(TemplatedSmsResponse))]
        [SwaggerResponse(401, "Unauthorized", typeof(TemplatedSmsResponse))]
        [SwaggerResponse(403, "Not Allowed", typeof(TemplatedSmsResponse))]
        [SwaggerResponse(404, "Not Found", typeof(TemplatedSmsResponse))]
        [SwaggerResponse(429, "Too Many Request", typeof(TemplatedSmsResponse))]
        [SwaggerResponse(450, "Given template is not found on dEngage", typeof(void))]
        [SwaggerResponse(451, "Customer Not Found.", typeof(void))]
        [SwaggerResponse(500, "Internal Server Error. Get Contact With Integration", typeof(void))]
        public async Task<IActionResult> SendTemplatedSmsString([FromBody] TemplatedSmsRequestString data)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Mock")
            {
                return Ok(new TemplatedSmsResponse()
                {
                    Status = dEngageResponseCodes.Success,
                    TxnId = Guid.NewGuid(),
                });
            }

            _transactionManager.StringSend = true;
            TemplatedSmsRequest _data = new TemplatedSmsRequest();
            _data.Sender = data.Sender;
            _data.CustomerNo = data.CustomerNo;
            _data.Process = data.Process;
            _data.TemplateParams = data.TemplateParams;
            _data.Tags = data.Tags;
            _data.CitizenshipNo = data.CitizenshipNo;
            _data.Template = data.Template;
            _data.Phone = _transactionManager.GetPhoneFromString(data.Phone);

            if (_data.Phone == null)
            {
                _data.Phone.CountryCode = _transactionManager.CustomerRequestInfo.MainPhone.CountryCode;
                _data.Phone.Prefix = _transactionManager.CustomerRequestInfo.MainPhone.Prefix;
                _data.Phone.Number = _transactionManager.CustomerRequestInfo.MainPhone.Number;
            }

            return Ok(await _dEngageSender.SendTemplatedSmsV2(_data));
        }

        [SwaggerOperation(
           Summary = "Send Sms message",
           Description = ""
            + "<div>To Send Sms With Plain Text Use This Method</div>"
            + "<div>Sender,SmsType,Content,Phone,Process Fields are Mandatory</div>"
            + "<div>When Customer Type(Burgan/On) is Not Known, Sender Field Should Be Set To AutoDetect"
            + " <br />Otherwise This Field Must Be Set Burgan or On</div>"
            + "<div>You can use advantages of headers by creating header from Header Management Services"
            + "<br /> When message services are called, we get customer info(BusinessLine,BranchCode)"
            + "<br /> Then try to find header matches with (BusinessLine[BL],BranchCode[BC],SmsType[ST])"
            + "<br /> Match Order by priority is (BL-BC-ST)-(BL-ST)-(BL-SC)-(BL)"
            + "<br /> If any header is matches, we add matched header prefix and suffix to beginning of message and end of the message</div>"
            + "<div>Content Field Will Be Logged After This Method Called. If You Need To Masking Critical Information You Should Surround"
            + " Critical Information with &lt;Mask&gt;&lt;/Mask&gt; . "
            + "<br />Example : \"Content\" : \"Your password is &lt;Mask&gt;1000&lt;/Mask&gt;.\"</div>"
            + "",
           Tags = new[] { "Sms" }
           )]
        [HttpPost("sms/message/string")]
        [SwaggerRequestExample(typeof(SmsRequestString), typeof(SmsRequestExampleFilter))]
        [SwaggerResponse(200, "Sms was sent successfully", typeof(SmsResponse))]
        [SwaggerResponse(200, "Sms was sent successfully", typeof(OtpResponse))]
        [SwaggerResponse(400, "Bad Request", typeof(SmsResponse))]
        [SwaggerResponse(401, "Unauthorized", typeof(SmsResponse))]
        [SwaggerResponse(403, "Not Allowed", typeof(SmsResponse))]
        [SwaggerResponse(404, "Not Found", typeof(SmsResponse))]
        [SwaggerResponse(429, "Too Many Request", typeof(SmsResponse))]
        [SwaggerResponse(450, "Given template is not found on dEngage", typeof(void))]
        [SwaggerResponse(451, "Customer Not Found.", typeof(void))]
        [SwaggerResponse(460, "Has Blacklist Record", typeof(OtpResponse))]
        [SwaggerResponse(461, "Sim Change", typeof(OtpResponse))]
        [SwaggerResponse(462, "Operator Change", typeof(OtpResponse))]
        [SwaggerResponse(463, "Rejected By Operator", typeof(OtpResponse))]
        [SwaggerResponse(464, "Not Subscriber", typeof(OtpResponse))]
        [SwaggerResponse(465, "Client Error", typeof(OtpResponse))]
        [SwaggerResponse(466, "Server Error", typeof(OtpResponse))]
        [SwaggerResponse(467, "Maximum Characters Count Exceed", typeof(OtpResponse))]
        [SwaggerResponse(500, "Internal Server Error. Get Contact With Integration", typeof(void))]
        public async Task<IActionResult> SendMessageSmsString([FromBody] SmsRequestString data)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Mock")
            {
                return Ok(new SmsResponse()
                {
                    Status = dEngageResponseCodes.Success,
                    TxnId = Guid.NewGuid(),
                });
            }

            _transactionManager.StringSend = true;
            SmsRequest _data = new SmsRequest();
            _data.SmsType = data.SmsType;
            _data.Sender = data.Sender;
            _data.CustomerNo = data.CustomerNo;
            _data.Process = data.Process;
            _data.Content = data.Content;
            _data.Tags = data.Tags;
            _data.CitizenshipNo = data.CitizenshipNo;
            _data.Phone = _transactionManager.GetPhoneFromString(data.Phone);

            if (_data.Phone == null)
            {
                _data.Phone = new Phone
                {
                    CountryCode = _transactionManager.CustomerRequestInfo.MainPhone.CountryCode,
                    Prefix = _transactionManager.CustomerRequestInfo.MainPhone.Prefix,
                    Number = _transactionManager.CustomerRequestInfo.MainPhone.Number,
                };
            }

            return await ProcessSmsRequestAsync(_data);

        }



        [SwaggerOperation(
           Summary = "Send Multiple templated Email message",
           Description = ""
            + "<div>To Send Multiple E-Mail With Template Which Defined On dEngage Use This Method</div>"
            + "<div>Sender,Template,E-Mail,Process Fields are Mandatory</div>"
            + "<div>When Customer Type(Burgan/On) is Not Known, Sender Field Must Be Set To AutoDetect"
            + " <br />Otherwise This Field Must Be Set Burgan or On</div>"
            + "<div>Given Template Must Be Defined On Both Of dEngage Tenants(On And Burgan) With Same Content Name</div>"
            + "<div>Template Params Must Be Set As JsonString Which Serialized From Dynamic Fields That Given Template Contains"
            + "<br />Example : Let's Assume template content is 'Welcome {%=$Current.Name%} {%=$Current.Surname%}.' "
            + "<br />In This Case Template Params Must Be Set To {\"Name\":\"Actual Name\",\"Surname\":\"Actual Surname\"}</div>"
            + "<div>TemplateParams Field Will Be Logged After This Method Called. If You Need To Masking Critical Information You Should Surround"
            + " Critical Information with &lt;Mask&gt;&lt;/Mask&gt; . "
            + "<br />Example : {\"Password\",\"&lt;Mask&gt;123456&lt;/Mask&gt;\"}</div>"
            + "<div>If You Want Send Attachments With Mail You Can Use Attachments Field"
            + "<br />Attachments.Name must be set Filename and Attachment.Data must be set to Base64 String Encoded From File Byte Array</div>"
            + "",
           Tags = new[] { "E-Mail" }
           )]
        [HttpPost("email/templated/multiple")]
        [SwaggerResponse(200, "Email was sent successfully", typeof(TemplatedMailResponse))]
        [SwaggerResponse(400, "Bad Request", typeof(TemplatedMailResponse))]
        [SwaggerResponse(401, "Unauthorized", typeof(TemplatedMailResponse))]
        [SwaggerResponse(403, "Not Allowed", typeof(TemplatedMailResponse))]
        [SwaggerResponse(404, "Not Found", typeof(TemplatedMailResponse))]
        [SwaggerResponse(429, "Too Many Request", typeof(TemplatedMailResponse))]
        [SwaggerResponse(450, "Given template is not found on dEngage", typeof(void))]
        [SwaggerResponse(451, "Customer Not Found.", typeof(void))]
        [SwaggerResponse(500, "Internal Server Error. Get Contact With Integration", typeof(void))]
        public async Task<IActionResult> SendTemplatedEmailMultiple([FromBody] TemplatedMailMultipleRequest data)
        {
            var response = new TemplatedMailMultipleResponse();

            List<Task> taskList = new List<Task>();
            ConcurrentBag<(string, TemplatedMailResponse)> bag = new ConcurrentBag<(string, TemplatedMailResponse)>();

            foreach (var address in data.MailAdresses)
            {
                var templatedMailRequest = new TemplatedMailRequest()
                {
                    Attachments = data.Attachments,
                    Bcc = address.Bcc,
                    Cc = address.Cc,
                    CheckIsVerified = data.CheckIsVerified,
                    CitizenshipNo = "",
                    CustomerNo = 0,
                    Email = address.Email,
                    Process = data.Process,
                    Sender = data.Sender,
                    Tags = data.Tags,
                    Template = data.Template,
                    TemplateParams = data.TemplateParams
                };

                taskList.Add(ProcessTemplatedMailRequest(bag, templatedMailRequest));
            }

            await Task.WhenAll(taskList);

            foreach (var bagData in bag)
            {
                response.Response.Add(new TemplatedMailMultipleResponseData
                {
                    MailAddress = bagData.Item1,
                    Status = bagData.Item2.Status,
                    StatusMessage = bagData.Item2.StatusMessage,
                    TxnId = bagData.Item2.TxnId
                });
            }

            return Ok(response);
        }

        [SwaggerOperation(
           Summary = "Send templated Email message",
           Description = ""
            + "<div>To Send E-Mail With Template Which Defined On dEngage Use This Method</div>"
            + "<div>Sender,Template,E-Mail,Process Fields are Mandatory</div>"
            + "<div>When Customer Type(Burgan/On) is Not Known, Sender Field Must Be Set To AutoDetect"
            + " <br />Otherwise This Field Must Be Set Burgan or On</div>"
            + "<div>Given Template Must Be Defined On Both Of dEngage Tenants(On And Burgan) With Same Content Name</div>"
            + "<div>Template Params Must Be Set As JsonString Which Serialized From Dynamic Fields That Given Template Contains"
            + "<br />Example : Let's Assume template content is 'Welcome {%=$Current.Name%} {%=$Current.Surname%}.' "
            + "<br />In This Case Template Params Must Be Set To {\"Name\":\"Actual Name\",\"Surname\":\"Actual Surname\"}</div>"
            + "<div>TemplateParams Field Will Be Logged After This Method Called. If You Need To Masking Critical Information You Should Surround"
            + " Critical Information with &lt;Mask&gt;&lt;/Mask&gt; . "
            + "<br />Example : {\"Password\",\"&lt;Mask&gt;123456&lt;/Mask&gt;\"}</div>"
            + "<div>If You Want Send Attachments With Mail You Can Use Attachments Field"
            + "<br />Attachments.Name must be set Filename and Attachment.Data must be set to Base64 String Encoded From File Byte Array</div>"
            + "",
           Tags = new[] { "E-Mail" }
           )]
        [HttpPost("email/templated")]
        [SwaggerResponse(200, "Email was sent successfully", typeof(TemplatedMailResponse))]
        [SwaggerResponse(400, "Bad Request", typeof(TemplatedMailResponse))]
        [SwaggerResponse(401, "Unauthorized", typeof(TemplatedMailResponse))]
        [SwaggerResponse(403, "Not Allowed", typeof(TemplatedMailResponse))]
        [SwaggerResponse(404, "Not Found", typeof(TemplatedMailResponse))]
        [SwaggerResponse(429, "Too Many Request", typeof(TemplatedMailResponse))]
        [SwaggerResponse(450, "Given template is not found on dEngage", typeof(void))]
        [SwaggerResponse(451, "Customer Not Found.", typeof(void))]
        [SwaggerResponse(500, "Internal Server Error. Get Contact With Integration", typeof(void))]
        public async Task<IActionResult> SendTemplatedEmail([FromBody] TemplatedMailRequest data)
        {
            if (data.Email == null)
            {
                data.Email = _transactionManager.CustomerRequestInfo.MainEmail;
            }

            var response = await _dEngageSender.SendTemplatedMailV2(data);
            return Ok(response);
        }

        [SwaggerOperation(
           Summary = "Send Multiple Email message",
           Description = ""
            + "<div>To Send E-Mail With Plain Text or Html Use This Method</div>"
            + "<div>Sender,From,Subject,Content,Email,Process Fields are Mandatory</div>"
            + "<div>When Customer Type(Burgan/On) is Not Known, Sender Field Should Be Set To AutoDetect"
            + " <br />Otherwise This Field Must Be Set Burgan or On</div>"
            + "<div>From Field Must Be Contains Just FromName Not Domain."
            + "<br/> System Will Decide Domain Part Depends On Customer Type"
            + "Example : (noreply | Correct Usage) - (noreply@burgan.com.tr | Wrong Usage) </div>"
            + "<div>Content and Subject Fields Will Be Logged After This Method Called. If You Need To Masking Critical Information You Should Surround"
            + " Critical Information with &lt;Mask&gt;&lt;/Mask&gt; . "
            + "<br />Example : \"Content\" : \"Your password is &lt;Mask&gt;1000&lt;/Mask&gt;.\"</div>"
            + "",
           Tags = new[] { "E-Mail" }
           )]
        [HttpPost("email/message/multiple")]
        [SwaggerResponse(200, "Email was sent successfully", typeof(MailResponse))]
        [SwaggerResponse(400, "Bad Request", typeof(MailResponse))]
        [SwaggerResponse(401, "Unauthorized", typeof(MailResponse))]
        [SwaggerResponse(403, "Not Allowed", typeof(MailResponse))]
        [SwaggerResponse(404, "Not Found", typeof(MailResponse))]
        [SwaggerResponse(429, "Too Many Request", typeof(MailResponse))]
        [SwaggerResponse(451, "Customer Not Found.", typeof(void))]
        [SwaggerResponse(500, "Internal Server Error. Get Contact With Integration", typeof(void))]
        public async Task<IActionResult> SendMessageEmailMultiple([FromBody] MailMultipleRequest data)
        {
            var response = new MailMultipleResponse();

            List<Task> taskList = new List<Task>();
            ConcurrentBag<(string, MailResponse)> bag = new ConcurrentBag<(string, MailResponse)>();

            foreach (var address in data.MailAdresses)
            {
                var mailRequest = new MailRequest()
                {
                    Attachments = data.Attachments,
                    Bcc = address.Bcc,
                    Cc = address.Cc,
                    CheckIsVerified = data.CheckIsVerified,
                    CitizenshipNo = "",
                    CustomerNo = 0,
                    Email = address.Email,
                    Process = data.Process,
                    Sender = data.Sender,
                    Tags = data.Tags,
                    Content = data.Content,
                    From = data.From,
                    Subject = data.Subject
                };

                taskList.Add(ProcessMailRequest(bag, mailRequest));
            }

            await Task.WhenAll(taskList);

            foreach (var bagData in bag)
            {
                response.Response.Add(new TemplatedMailMultipleResponseData
                {
                    MailAddress = bagData.Item1,
                    Status = bagData.Item2.Status,
                    StatusMessage = bagData.Item2.StatusMessage,
                    TxnId = bagData.Item2.TxnId
                });
            }

            return Ok(response);
        }

        [SwaggerOperation(
           Summary = "Send Email message",
           Description = ""
            + "<div>To Send E-Mail With Plain Text or Html Use This Method</div>"
            + "<div>Sender,From,Subject,Content,Email,Process Fields are Mandatory</div>"
            + "<div>When Customer Type(Burgan/On) is Not Known, Sender Field Should Be Set To AutoDetect"
            + " <br />Otherwise This Field Must Be Set Burgan or On</div>"
            + "<div>From Field Must Be Contains Just FromName Not Domain."
            + "<br/> System Will Decide Domain Part Depends On Customer Type"
            + "Example : (noreply | Correct Usage) - (noreply@burgan.com.tr | Wrong Usage) </div>"
            + "<div>Content and Subject Fields Will Be Logged After This Method Called. If You Need To Masking Critical Information You Should Surround"
            + " Critical Information with &lt;Mask&gt;&lt;/Mask&gt; . "
            + "<br />Example : \"Content\" : \"Your password is &lt;Mask&gt;1000&lt;/Mask&gt;.\"</div>"
            + "",
           Tags = new[] { "E-Mail" }
           )]
        [HttpPost("email/message")]
        [SwaggerResponse(200, "Email was sent successfully", typeof(MailResponse))]
        [SwaggerResponse(400, "Bad Request", typeof(MailResponse))]
        [SwaggerResponse(401, "Unauthorized", typeof(MailResponse))]
        [SwaggerResponse(403, "Not Allowed", typeof(MailResponse))]
        [SwaggerResponse(404, "Not Found", typeof(MailResponse))]
        [SwaggerResponse(429, "Too Many Request", typeof(MailResponse))]
        [SwaggerResponse(451, "Customer Not Found.", typeof(void))]
        [SwaggerResponse(500, "Internal Server Error. Get Contact With Integration", typeof(void))]
        public async Task<IActionResult> SendMessageEmail([FromBody] MailRequest data)
        {

            if (data.Email == null)
            {
                data.Email = _transactionManager.CustomerRequestInfo.MainEmail;
            }
            var response = await _dEngageSender.SendMailV2(data);
            return Ok(response);
        }

        [SwaggerOperation(
           Summary = "Send Push Notification",
           Description = ""
            + "<div>To Send Push Notification With Plain Text Use This Method</div>"
            + "<div>Sender,Content,CitizenshipNo,Process Fields are Mandatory</div>"
            + "<div>When Customer Type(Burgan/On) is Not Known, Sender Field Should Be Set To AutoDetect"
            + " <br />Otherwise This Field Must Be Set Burgan or On</div>"
            + "<div>Content Field Will Be Logged After This Method Called. If You Need To Masking Critical Information You Should Surround"
            + " Critical Information with &lt;Mask&gt;&lt;/Mask&gt; . "
            + "<br />Example : \"Content\" : \"Your password is &lt;Mask&gt;1000&lt;/Mask&gt;.\"</div>"
            + "<br />"
             + "<div>saveInbox parameter is used to save to inbox</div>"
             + "",
            Tags = new[] { "Push Notification" }
           )]
        [HttpPost("push-notification/message")]
        [SwaggerResponse(200, "Push notification was sent successfully", typeof(PushResponse))]
        [SwaggerResponse(400, "Bad Request", typeof(PushResponse))]
        [SwaggerResponse(401, "Unauthorized", typeof(PushResponse))]
        [SwaggerResponse(403, "Not Allowed", typeof(PushResponse))]
        [SwaggerResponse(404, "Not Found", typeof(PushResponse))]
        [SwaggerResponse(429, "Too Many Request", typeof(PushResponse))]
        [SwaggerResponse(451, "Customer Not Found.", typeof(void))]
        [SwaggerResponse(500, "Internal Server Error. Get Contact With Integration", typeof(void))]
        public async Task<IActionResult> SendPushNotification([FromBody] PushRequest data)
        {
            if (data?.IsFirebase ?? false)
            {
                var response = await _firebaseSender.SendPushNotificationAsync(data);
                return Ok(response);
            }
            else
            {
                var response = await _dEngageSender.SendPushNotificationV2(data);
                return Ok(response);
            }
        }

        [SwaggerOperation(
           Summary = "Send Templated Push Notification",
           Description = ""
            + "<div>To Send Push Notification With Template Which Defined On dEngage Use This Method</div>"
            + "<div>Sender,Template,CitizenshipNo,Process Fields are Mandatory</div>"
            + "<div>When Customer Type(Burgan/On) is Not Known, Sender Field Must Be Set To AutoDetect"
            + " <br />Otherwise This Field Must Be Set Burgan or On</div>"
            + "<div>Given Template Must Be Defined On Both Of dEngage Tenants(On And Burgan) With Same Content Name</div>"
            + "<div>Template Params Must Be Set As JsonString Which Serialized From Dynamic Fields That Given Template Contains"
            + "<br />Example : Let's Assume template content is 'Welcome {%=$Current.Name%} {%=$Current.Surname%}.' "
            + "<br />In This Case Template Params Must Be Set To {\"Name\":\"Actual Name\",\"Surname\":\"Actual Surname\"}</div>"
            + "<div>TemplateParams Field Will Be Logged After This Method Called. If You Need To Masking Critical Information You Should Surround"
            + " Critical Information with &lt;Mask&gt;&lt;/Mask&gt; . "
            + "<br />Example : {\"Password\",\"&lt;Mask&gt;123456&lt;/Mask&gt;\"}</div>"
            + "<br />"
             + "<div>saveInbox parameter is used to save to inbox</div>"
             + "",
           Tags = new[] { "Push Notification" }
           )]
        [HttpPost("push-notification/templated")]
        [SwaggerResponse(200, "Push notication was sent successfully", typeof(TemplatedPushResponse))]
        [SwaggerResponse(400, "Bad Request", typeof(TemplatedPushResponse))]
        [SwaggerResponse(401, "Unauthorized", typeof(TemplatedPushResponse))]
        [SwaggerResponse(403, "Not Allowed", typeof(TemplatedPushResponse))]
        [SwaggerResponse(404, "Not Found", typeof(TemplatedPushResponse))]
        [SwaggerResponse(429, "Too Many Request", typeof(TemplatedPushResponse))]
        [SwaggerResponse(450, "Given template is not found on dEngage", typeof(void))]
        [SwaggerResponse(451, "Customer Not Found.", typeof(void))]
        [SwaggerResponse(500, "Internal Server Error. Get Contact With Integration", typeof(void))]

        public async Task<IActionResult> SendTemplatedPushNotification([FromBody] TemplatedPushRequest data)
        {
            var response = await _dEngageSender.SendTemplatedPushNotificationV2(data);
            return Ok(response);
        }



        private async Task ProcessTemplatedMailRequest(ConcurrentBag<(string, TemplatedMailResponse)> bag, TemplatedMailRequest request)
        {
            try
            {
                var response = await _messagingGatewayApi.SendTemplatedMail(request);
                bag.Add((request.Email, response));
            }
            catch (ApiException ex)
            {
                var templatedMailResponse = new TemplatedMailResponse();
                templatedMailResponse.Status = dEngageResponseCodes.BadRequest;
                templatedMailResponse.StatusMessage = await ex.GetContentAsAsync<string>();
                bag.Add((request.Email, templatedMailResponse));
            }
            catch (Exception ex)
            {
                var templatedMailResponse = new TemplatedMailResponse();
                templatedMailResponse.Status = dEngageResponseCodes.BadRequest;
                templatedMailResponse.StatusMessage = "Internal Server Error";
                bag.Add((request.Email, templatedMailResponse));
            }

        }

        private async Task ProcessMailRequest(ConcurrentBag<(string, MailResponse)> bag, MailRequest request)
        {
            try
            {
                var response = await _messagingGatewayApi.SendMail(request);
                bag.Add((request.Email, response));
            }
            catch (ApiException ex)
            {
                var mailResponse = new MailResponse();
                mailResponse.Status = dEngageResponseCodes.BadRequest;
                mailResponse.StatusMessage = await ex.GetContentAsAsync<string>();
                bag.Add((request.Email, mailResponse));
            }
            catch (Exception ex)
            {
                var mailResponse = new MailResponse();
                mailResponse.Status = dEngageResponseCodes.BadRequest;
                mailResponse.StatusMessage = "Internal Server Error";
                bag.Add((request.Email, mailResponse));
            }

        }
    }
}
