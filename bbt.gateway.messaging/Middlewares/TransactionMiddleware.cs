using bbt.gateway.common;
using bbt.gateway.common.Models;
using bbt.gateway.messaging.Exceptions;
using bbt.gateway.messaging.Workers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Middlewares
{
    public class TransactionMiddleware
    {
        private readonly RequestDelegate _next;
        private RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
        private MiddlewareRequest _middlewareRequest;
        public TransactionMiddleware(RequestDelegate next)
        {
            _next = next;
            _recyclableMemoryStreamManager = new();
        }

        public async Task InvokeAsync(HttpContext context, ITransactionManager _transactionManager,IConfiguration configuration)
        {
            
            try
            {
                context.Request.EnableBuffering();

                //Get IpAddress
                var ipAdress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                    ?? context.Connection.RemoteIpAddress.ToString();

                //Get Request Body
                await using var requestStream = _recyclableMemoryStreamManager.GetStream();

                await context.Request.Body.CopyToAsync(requestStream);
                var body = ReadStreamInChunks(requestStream);

                // Reset the request body stream position so the next middleware can read it
                context.Request.Body.Position = 0;

                _transactionManager.Transaction.Request = body;
                _transactionManager.Transaction.IpAdress = ipAdress;

                //Deserialize Request Body 
                _middlewareRequest = JsonConvert.DeserializeObject<MiddlewareRequest>(body);

                _transactionManager.Transaction.CreatedBy = _middlewareRequest.Process;
                _transactionManager.Transaction.Mail = _middlewareRequest.Email;
                _transactionManager.Transaction.Phone = _middlewareRequest.Phone;
                _transactionManager.Transaction.CustomerNo = _middlewareRequest.CustomerNo.GetValueOrDefault();
                _transactionManager.Transaction.CitizenshipNo = String.IsNullOrWhiteSpace(_middlewareRequest.ContactId) ? 
                    _middlewareRequest.CitizenshipNo : _middlewareRequest.ContactId;
                _transactionManager.HeaderInfo = _middlewareRequest.HeaderInfo;
                _transactionManager.Sender = _middlewareRequest.Sender;
                _transactionManager.SmsType = _middlewareRequest.SmsType;

                SetTransaction(context,_transactionManager);

                //Save Original Stream
                var originalStream = context.Response.Body;
                await using var responseBody = _recyclableMemoryStreamManager.GetStream();
                context.Response.Body = responseBody;

                try
                {
                    await _next(context);
                    _transactionManager.LogState();

                    //Get Response Body
                    context.Response.Body.Seek(0, SeekOrigin.Begin);
                    var response = await new StreamReader(context.Response.Body).ReadToEndAsync();
                    context.Response.Body.Seek(0, SeekOrigin.Begin);
                    await responseBody.CopyToAsync(originalStream);


                    _transactionManager.Transaction.Response = response;
                    

                }
                catch (WorkflowException ex)
                {
                    //Get Response Body
                    context.Response.Body.Seek(0, SeekOrigin.Begin);
                    using var stream = new MemoryStream();
                    using var writer = new StreamWriter(stream);
                    writer.Write(ex.Message);
                    writer.Flush();
                    stream.Position = 0;
                    context.Response.Body.Seek(0, SeekOrigin.Begin);
                    context.Response.ContentType = "text/plain";
                    context.Response.StatusCode = (int)ex.StatusCode;
                    await stream.CopyToAsync(originalStream);

                    _transactionManager.Transaction.Response = "An Error Occured | Detail :" + ex.ToString();

                    _transactionManager.LogState();
                    _transactionManager.LogError("An Error Occured | Detail :" + ex.ToString());


                }
                catch (Exception ex)
                {
                    _transactionManager.Transaction.Response = "An Error Occured | Detail :" + ex.ToString();

                    _transactionManager.LogState();
                    _transactionManager.LogError("An Error Occured | Detail :" + ex.ToString());

                    context.Response.StatusCode = 500;
                }
            }
            catch (Exception ex)
            {
                _transactionManager.Transaction.Response = "An Error Occured | Detail :" + ex.ToString();
                

                _transactionManager.LogState();
                _transactionManager.LogError("An Error Occured | Detail :" + ex.ToString());

                context.Response.StatusCode = 500;
            }
            finally
            {
                try
                {
                    await _transactionManager.AddTransactionAsync();
                    await _transactionManager.SaveTransactionAsync();
                }
                catch (Exception ex)
                {

                    _transactionManager.LogError("An Error Occured | Detail :" + ex.ToString());
                }
                
            }
        }

        private void SetTransaction(HttpContext context,ITransactionManager _transactionManager)
        {
            var path = context.Request.Path.ToString();
            if (path.Contains("sms") && !path.Contains("check"))
            {
                if (_middlewareRequest.ContentType == MessageContentType.Otp
                    || _middlewareRequest.SmsType == SmsTypes.Otp)
                {
                    SetTransactionAsOtp(_transactionManager);
                }
                else
                {
                    if (path.Contains("templated"))
                    {
                        SetTransactionAsTemplatedSms(_transactionManager);
                    }
                    else
                    {
                        SetTransactionAsSms(_transactionManager);
                    }
                }
            }

            if (path.Contains("email"))
            {

                if (path.Contains("templated"))
                {
                    if (path.Contains("multiple"))
                    {
                        SetTransactionAsTemplatedMultipleMail(_transactionManager);
                    }
                    else
                    {
                        SetTransactionAsTemplatedMail(_transactionManager);
                    }
                }
                else
                {
                    if (path.Contains("multiple"))
                    {
                        SetTransactionAsMultipleMail(_transactionManager);
                    }
                    else
                    {
                        SetTransactionAsMail(_transactionManager);
                    }
                }

            }

            if (path.Contains("push"))
            {

                if (path.Contains("templated"))
                {
                    SetTransactionAsTemplatedPushNotification(_transactionManager);
                }
                else
                {
                    SetTransactionAsPushNotification(_transactionManager);
                }

            }
        }

        private void SetTransactionAsOtp(ITransactionManager _transactionManager)
        {
            _transactionManager.Transaction.TransactionType = TransactionType.Otp;
            _transactionManager.OtpRequestInfo.Process = _middlewareRequest.Process;
            _transactionManager.OtpRequestInfo.Content = _middlewareRequest.Content?.MaskOtpContent();
            _transactionManager.OtpRequestInfo.Phone = _middlewareRequest.Phone;
        }

        private void SetTransactionAsSms(ITransactionManager _transactionManager)
        {
            _transactionManager.Transaction.TransactionType = TransactionType.TransactionalSms;
            _transactionManager.SmsRequestInfo.Process = _middlewareRequest.Process;
            _transactionManager.SmsRequestInfo.Content = _middlewareRequest.Content?.MaskFields();
            _transactionManager.SmsRequestInfo.Phone = _middlewareRequest.Phone;
        }

        private void SetTransactionAsTemplatedSms(ITransactionManager _transactionManager)
        {
            _transactionManager.Transaction.TransactionType = TransactionType.TransactionalTemplatedSms;
            _transactionManager.SmsRequestInfo.Process = _middlewareRequest.Process;
            _transactionManager.SmsRequestInfo.TemplateId = _middlewareRequest.Template;
            _transactionManager.SmsRequestInfo.TemplateParams = _middlewareRequest.TemplateParams?.MaskFields();
            _transactionManager.SmsRequestInfo.Phone = _middlewareRequest.Phone;
        }

        private void SetTransactionAsMail(ITransactionManager _transactionManager)
        {
            _transactionManager.Transaction.TransactionType = TransactionType.TransactionalMail;
            _transactionManager.MailRequestInfo.Process = _middlewareRequest.Process;
            _transactionManager.MailRequestInfo.Content = _middlewareRequest.Content?.MaskFields();
            _transactionManager.MailRequestInfo.Email = _middlewareRequest.Email;
        }

        private void SetTransactionAsTemplatedMail(ITransactionManager _transactionManager)
        {
            _transactionManager.Transaction.TransactionType = TransactionType.TransactionalTemplatedMail;
            _transactionManager.MailRequestInfo.Process = _middlewareRequest.Process;
            _transactionManager.MailRequestInfo.TemplateId = _middlewareRequest.Template;
            _transactionManager.MailRequestInfo.TemplateParams = _middlewareRequest.TemplateParams?.MaskFields();
            _transactionManager.MailRequestInfo.Email = _middlewareRequest.Email;
        }

        private void SetTransactionAsTemplatedMultipleMail(ITransactionManager _transactionManager)
        {
            _transactionManager.Transaction.TransactionType = TransactionType.TransactionalTemplatedMailMultiple;
            _transactionManager.MailRequestInfo.Process = _middlewareRequest.Process;
            _transactionManager.MailRequestInfo.TemplateId = _middlewareRequest.Template;
            _transactionManager.MailRequestInfo.TemplateParams = _middlewareRequest.TemplateParams?.MaskFields();
            _transactionManager.MailRequestInfo.Email = "";
        }

        private void SetTransactionAsMultipleMail(ITransactionManager _transactionManager)
        {
            _transactionManager.Transaction.TransactionType = TransactionType.TransactionalMailMultiple;
            _transactionManager.MailRequestInfo.Process = _middlewareRequest.Process;
            _transactionManager.MailRequestInfo.TemplateId = _middlewareRequest.Template;
            _transactionManager.MailRequestInfo.TemplateParams = _middlewareRequest.TemplateParams?.MaskFields();
            _transactionManager.MailRequestInfo.Email = "";
        }

        private void SetTransactionAsPushNotification(ITransactionManager _transactionManager)
        {
            _transactionManager.Transaction.TransactionType = TransactionType.TransactionalPush;
            _transactionManager.PushRequestInfo.Process = _middlewareRequest.Process;
            _transactionManager.PushRequestInfo.ContactId = _middlewareRequest.ContactId;
            _transactionManager.PushRequestInfo.TemplateId = _middlewareRequest.Template;
            _transactionManager.PushRequestInfo.TemplateParams = _middlewareRequest.TemplateParams?.MaskFields();
            _transactionManager.PushRequestInfo.CustomParameters = _middlewareRequest.CustomParameters?.MaskFields();
        }

        private void SetTransactionAsTemplatedPushNotification(ITransactionManager _transactionManager)
        {
            _transactionManager.Transaction.TransactionType = TransactionType.TransactionalTemplatedPush;
            _transactionManager.PushRequestInfo.Process = _middlewareRequest.Process;
            _transactionManager.PushRequestInfo.ContactId = _middlewareRequest.ContactId;
            _transactionManager.PushRequestInfo.TemplateId = _middlewareRequest.Template;
            _transactionManager.PushRequestInfo.TemplateParams = _middlewareRequest.TemplateParams?.MaskFields();
            _transactionManager.PushRequestInfo.CustomParameters = _middlewareRequest.CustomParameters?.MaskFields();
        }

        private string ReadStreamInChunks(Stream stream)
        {
            const int readChunkBufferLength = 4096;
            stream.Seek(0, SeekOrigin.Begin);
            using var textWriter = new StringWriter();
            using var reader = new StreamReader(stream);
            var readChunk = new char[readChunkBufferLength];
            int readChunkLength;
            do
            {
                readChunkLength = reader.ReadBlock(readChunk,
                                                   0,
                                                   readChunkBufferLength);
                textWriter.Write(readChunk, 0, readChunkLength);
            } while (readChunkLength > 0);
            return textWriter.ToString();
        }
    }

    public static class TransactionMiddlewareExtensions
    {
        public static IApplicationBuilder UseTransactionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseWhen(context => (context.Request.Path.Value.IndexOf("/Messaging") != -1
            && context.Request.Path.Value.IndexOf("/sms/check") == -1
            ), builder =>
            {
                builder.UseMiddleware<TransactionMiddleware>();
            });
        }
    }
}
