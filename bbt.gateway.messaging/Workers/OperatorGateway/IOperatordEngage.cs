﻿using bbt.gateway.common.Api.dEngage.Model.Contents;
using bbt.gateway.common.Api.dEngage.Model.Transactional;
using bbt.gateway.common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Workers.OperatorGateway
{
    public interface IOperatordEngage : IOperatorGatewayBase
    {
        public Task<MailResponseLog> SendMail(string to, string? from, string? subject, string? html, string? templateId, string? templateParams, List<common.Models.Attachment> attachments, string? cc, string? bcc, string?[] tags,bool? checkIsVerified);
        public Task<List<MailResponseLog>> SendBulkMail(string to, string? from, string? subject, string? html, string? templateId, string? templateParams, List<common.Models.Attachment> attachments, string? cc, string? bcc, string?[] tags);
        public Task<SmsResponseLog> SendSms(Phone phone, SmsTypes smsType, string? content, string? templateId, string? templateParams, string?[] tags);
        public Task<PushNotificationResponseLog> SendPush(string contactId, string template, string templateParams, string customParameters, bool? saveInbox, string[] tags);
        public Task<SmsStatusResponse> CheckSms(string queryId);
        public Task<MailStatusResponse> CheckMail(string queryId);
        public Task<MailContentsResponse> GetMailContents(int limit, string offset);
        public Task<SmsContentsResponse> GetSmsContents(int limit, string offset);
        public Task<PushContentsResponse> GetPushContents(int limit, string offset);

    }
}
