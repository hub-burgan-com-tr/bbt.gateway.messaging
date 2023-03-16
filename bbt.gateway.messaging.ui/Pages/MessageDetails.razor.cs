using bbt.gateway.common.Models;
using bbt.gateway.messaging.ui.Pages.Base;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace bbt.gateway.messaging.ui.Pages
{
    public partial class MessageDetails : ComponentBase
    {
        [Parameter] public Transaction Txn { get; set; }

        private ICollection<OtpResponseLog> responseLogs;
        private ICollection<OtpTrackingLog> trackingsLogs;
        private string message { get; set; }
        private string template { get; set; }
        private string templateParams { get; set; }

        protected override void OnInitialized()
        {
            message = Txn.OtpRequestLog == null ? "" : (Txn.OtpRequestLog.Content ?? "");  
            responseLogs = GetOtpResponseLogs();
            trackingsLogs = GetOtpTrackingLogs();
        }

        private string GetTemplate()
        {
            if (Txn.TransactionType == TransactionType.TransactionalTemplatedSms)
            {
                if (Txn.SmsRequestLog == null)
                    return "";
                return Txn.SmsRequestLog.TemplateId ?? "";
            }
            if (Txn.TransactionType == TransactionType.TransactionalTemplatedMail)
            {
                if (Txn.MailRequestLog == null)
                    return "";
                return Txn.MailRequestLog.TemplateId ?? "";
            }
            if (Txn.TransactionType == TransactionType.TransactionalTemplatedPush)
            {
                if (Txn.PushNotificationRequestLog == null)
                    return "";
                return Txn.PushNotificationRequestLog.TemplateId ?? "";
            }
            return "";
        }

        private string GetTemplateParams()
        {
            if (Txn.TransactionType == TransactionType.TransactionalTemplatedSms)
            {
                if (Txn.SmsRequestLog == null)
                    return "";
                return Txn.SmsRequestLog.TemplateParams ?? "";
            }
            if (Txn.TransactionType == TransactionType.TransactionalTemplatedMail)
            {
                if (Txn.MailRequestLog == null)
                    return "";
                return Txn.MailRequestLog.TemplateParams ?? "";
            }
            if (Txn.TransactionType == TransactionType.TransactionalTemplatedPush)
            {
                if (Txn.PushNotificationRequestLog == null)
                    return "";
                return Txn.PushNotificationRequestLog.TemplateParams ?? "";
            }
            return "";
        }

        private ICollection<OtpResponseLog> GetOtpResponseLogs()
        {
            if (Txn.OtpRequestLog == null)
                return new List<OtpResponseLog>();
            return Txn.OtpRequestLog.ResponseLogs ?? new List<OtpResponseLog>();
        }

        private ICollection<OtpTrackingLog> GetOtpTrackingLogs()
        {
            if (Txn.OtpRequestLog == null)
                return new List<OtpTrackingLog>();
            if (Txn.OtpRequestLog.ResponseLogs == null)
                return new List<OtpTrackingLog>();

            if (Txn.OtpRequestLog.ResponseLogs.Count == 1)
            {
                return Txn.OtpRequestLog.ResponseLogs.First().TrackingLogs ?? new List<OtpTrackingLog>();
            }
            else
            {
                if (Txn.OtpRequestLog.ResponseLogs.FirstOrDefault(l => l.ResponseCode == SendSmsResponseStatus.Success) == null)
                { 
                    return new List<OtpTrackingLog>();
                }

                return Txn.OtpRequestLog.ResponseLogs.FirstOrDefault(l => l.ResponseCode == SendSmsResponseStatus.Success).TrackingLogs ?? new List<OtpTrackingLog>();
            }
        }

        private string GetOperator()
        {
            if (Txn.TransactionType == TransactionType.Otp)
            {
                if (Txn.OtpRequestLog != null)
                {
                    return Txn.OtpRequestLog.PhoneConfiguration.Operator.ToString() ?? "unknown";
                }
            }
            if (Txn.TransactionType == TransactionType.TransactionalMail || Txn.TransactionType == TransactionType.TransactionalTemplatedMail)
            {
                if (Txn.MailRequestLog != null)
                {
                    return Txn.MailRequestLog.Operator.ToString();
                }
            }
            if (Txn.TransactionType == TransactionType.TransactionalSms || Txn.TransactionType == TransactionType.TransactionalTemplatedSms)
            {
                if (Txn.SmsRequestLog != null)
                {
                    return Txn.SmsRequestLog.Operator.ToString();
                }
            }
            if (Txn.TransactionType == TransactionType.TransactionalPush || Txn.TransactionType == TransactionType.TransactionalTemplatedPush)
            {
                if (Txn.PushNotificationRequestLog != null)
                {
                    return Txn.PushNotificationRequestLog.Operator.ToString();
                }
            }

            return "unknown";
        }
        async Task CheckSmsStatus(Radzen.LoadDataArgs args = null)
        {
            string MessageSmsStatus = string.Empty;
            try
            {
                OtpResponseLog? responseLog = responseLogs.FirstOrDefault(f =>! string.IsNullOrEmpty(f.StatusQueryId));
              
                if (responseLog != null)
                {
                    if (Txn.TransactionType == TransactionType.Otp)
                    {
                        common.Models.v2.CheckSmsRequest checkSmsRequest = new common.Models.v2.CheckSmsRequest();
                        if (Txn.OtpRequestLog != null && Txn.OtpRequestLog.PhoneConfiguration.Operator != null)
                        {
                            checkSmsRequest.Operator = Txn.OtpRequestLog.PhoneConfiguration.Operator.Value;
                            checkSmsRequest.OtpRequestLogId = Txn.OtpRequestLog.Id;
                        }
                        else
                        {
                            MessageSmsStatus = "Operator bulunamadı";
                        }
                        checkSmsRequest.StatusQueryId = responseLog.StatusQueryId;

                        var res = MessagingGateway.CheckOtpSmsStatus(checkSmsRequest).Result;
                       
                        if(res != null)
                        {
                            if(res.OtpResponseLog==null)
                            {
                                MessageSmsStatus = "Status:" + res.Status.ToString() + " " + Environment.NewLine + "  ResponseMessage:" + res.ResponseMessage;
                            }
                            else
                            {
                                MessageSmsStatus = "Status:" + res.Status.ToString() + " " + Environment.NewLine+ "  ResponseMessage:" + res.OtpResponseLog.ResponseMessage;
                            }
                        }
                    }

                }
                else
                {
                    MessageSmsStatus = "Herhangi bir Sorgulama için gereken Operator Response id değeri bulunamadı ";
                    
                }
            }
            catch(Exception ex)
            {
                MessageSmsStatus = "Sms Sorgulamasında beklenmedik bir hata oluştu";
            }
            if(!string.IsNullOrEmpty(MessageSmsStatus))
            {
                
                dialogService.Open<BaseMessageDialog>("Bilgilendirme",
           new Dictionary<string, object>() { { "Message", MessageSmsStatus } },
           new DialogOptions() { CloseDialogOnOverlayClick = true });
            }
           

        }
    }
}
