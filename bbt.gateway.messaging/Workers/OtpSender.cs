using bbt.gateway.common.Extensions;
using bbt.gateway.common.Models;
using bbt.gateway.common.Repositories;
using bbt.gateway.messaging.Helpers;
using bbt.gateway.messaging.Workers.OperatorGateway;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Workers
{
    public class OtpSender
    {
        private readonly HeaderManager _headerManager;
        private readonly Func<OperatorType, IOperatorGateway> _operatorRepository;
        private readonly IRepositoryManager _repositoryManager;
        private readonly ITransactionManager _transactionManager;
        private readonly InstantReminder _instantReminder;

        SendMessageSmsRequest _data;
        common.Models.v2.SmsRequest _dataV2;
        SendSmsResponseStatus returnValue;
        OtpRequestLog _requestLog;
        PhoneConfiguration phoneConfiguration;

        private readonly Dictionary<Type, OperatorType> operators = new Dictionary<Type, OperatorType>()
        {
            { typeof(OperatorTurkcell) , OperatorType.Turkcell},
            { typeof(OperatorVodafone) , OperatorType.Vodafone},
            { typeof(OperatorTurkTelekom) , OperatorType.TurkTelekom}
        };
        public OtpSender(HeaderManager headerManager,
            Func<OperatorType, IOperatorGateway> operatorRepository,
            IRepositoryManager repositoryManager,
            ITransactionManager transactionManager,
            InstantReminder instantReminder,
            InfobipSender infobipSender)
        {
            _headerManager = headerManager;
            _operatorRepository = operatorRepository;
            _repositoryManager = repositoryManager;
            _transactionManager = transactionManager;
            _instantReminder = instantReminder;
        }

        public async Task<SendSmsOtpResponse> SendMessage(SendMessageSmsRequest sendMessageSmsRequest)
        {
            SendSmsOtpResponse sendSmsOtpResponse = new() { 
                TxnId = _transactionManager.TxnId
            };
            //Set returnValue ClientError for Unexpected Errors
            returnValue = SendSmsResponseStatus.ClientError;

            //Set Request Body To Class Variable
            _data = sendMessageSmsRequest;

            //Turkish Character Conversion And Length Validation
            _data.Content = _data.Content.ConvertToTurkish();
            if(_data.Content.Length > 160)
            {
                _transactionManager.LogError("Otp Maximum Characters Count Exceed");
                returnValue = SendSmsResponseStatus.MaximumCharactersCountExceed;
                sendSmsOtpResponse.Status = returnValue;
                return sendSmsOtpResponse;
            }

            _requestLog = new OtpRequestLog
            {
                CreatedBy = _data.Process,
                Phone = _data.Phone
            };

            // Load Phone configuration and related active blacklist entiries.
            phoneConfiguration = await _repositoryManager.PhoneConfigurations.GetWithBlacklistEntriesAsync(
                _data.Phone.CountryCode, _data.Phone.Prefix, _data.Phone.Number, DateTime.Now);

            //Get blacklist records from current Otp System
            var oldBlacklistRecord = (await _repositoryManager.DirectBlacklists.FindAsync(b => b.GsmNumber == _data.Phone.ToString())).OrderByDescending(b => b.RecordDate).FirstOrDefault();

            if (oldBlacklistRecord != null)
            {
                var blackListRecord = phoneConfiguration.BlacklistEntries.FirstOrDefault(b => b.SmsId == oldBlacklistRecord.SmsId);
                if (blackListRecord != null)
                {
                    if (blackListRecord.Status == BlacklistStatus.NotResolved && oldBlacklistRecord.IsVerified)
                    {
                        //Resolve Blacklist entry
                        blackListRecord.Status = BlacklistStatus.Resolved;
                        blackListRecord.ResolvedAt = oldBlacklistRecord.VerifyDate;
                        blackListRecord.ResolvedBy = new Process { Name = oldBlacklistRecord.VerifiedBy };
                    }
                    if (blackListRecord.Status == BlacklistStatus.NotResolved && !oldBlacklistRecord.IsVerified)
                    {
                        oldBlacklistRecord.TryCount++;
                        await _repositoryManager.SaveSmsBankingChangesAsync();
                    }
                }
                else
                {
                    if (!oldBlacklistRecord.IsVerified)
                    {

                        //Increase Try Count
                        oldBlacklistRecord.TryCount++;
                        await _repositoryManager.SaveSmsBankingChangesAsync();

                        //Insert Blacklist entry
                        var blacklistEntry = createBlackListEntry(phoneConfiguration, returnValue.ToString(), "SendMessageToKnownProcess", oldBlacklistRecord.SmsId);
                        phoneConfiguration.BlacklistEntries.Add(blacklistEntry);
                        await _repositoryManager.BlackListEntries.AddAsync(blacklistEntry);
                    }
                }
            }

            // if known number without any blacklist entry 
            if (
            phoneConfiguration != null &&
            phoneConfiguration.Operator != null &&
            !phoneConfiguration.BlacklistEntries.Any(b => b.Status == BlacklistStatus.NotResolved)
            )
            {

                var responseLog = await SendMessageToKnown(phoneConfiguration);
                _requestLog.ResponseLogs.Add(responseLog);

                if (responseLog.ResponseCode == SendSmsResponseStatus.OperatorChange
                    || responseLog.ResponseCode == SendSmsResponseStatus.SimChange)
                {
                    _transactionManager.LogError("OperatorChange or SimChange Has Occured");

                    if (phoneConfiguration.BlacklistEntries != null)
                    {
                        //Add to Blacklist If Not Exists
                        if (!phoneConfiguration.BlacklistEntries.Any(b => b.Status == BlacklistStatus.NotResolved && b.ValidTo > DateTime.Today))
                        {
                            _transactionManager.LogError("Phone has a blacklist record");
                            var oldBlackListEntry = createOldBlackListEntry((long)_transactionManager.CustomerRequestInfo.CustomerNo, phoneConfiguration.Phone.ToString());
                            await _repositoryManager.DirectBlacklists.AddAsync(oldBlackListEntry);
                            await _repositoryManager.SaveSmsBankingChangesAsync();

                            var blackListEntry = createBlackListEntry(phoneConfiguration, returnValue.ToString(), "SendMessageToKnownProcess", oldBlackListEntry.SmsId);
                            await _repositoryManager.BlackListEntries.AddAsync(blackListEntry);
                        }
                    }
                    else
                    {
                            _transactionManager.LogError("Phone has a blacklist record");
                            var oldBlackListEntry = createOldBlackListEntry((long)_transactionManager.CustomerRequestInfo.CustomerNo, phoneConfiguration.Phone.ToString());
                            await _repositoryManager.DirectBlacklists.AddAsync(oldBlackListEntry);
                            await _repositoryManager.SaveSmsBankingChangesAsync();

                            var blackListEntry = createBlackListEntry(phoneConfiguration, returnValue.ToString(), "SendMessageToKnownProcess", oldBlackListEntry.SmsId);
                            await _repositoryManager.BlackListEntries.AddAsync(blackListEntry);
                    }
                }
                returnValue = responseLog.ResponseCode;
                
                //Operator Change | Sim Change | Not Subscriber handle edilmeli
                if (responseLog.ResponseCode == SendSmsResponseStatus.NotSubscriber)
                {
                    _transactionManager.LogError("Known Number Changed Operator");

                    //Known Number Returns Not Subscriber For Related Operator
                    //Try to Send Sms With Another Operators
                    //Should pass true for discarding current operator
                    await SendMessageToUnknownProcess(true);
                }                
            }
            else
            {
                //Known Number With Active Blacklist Entry
                if (phoneConfiguration != null &&
                    phoneConfiguration.BlacklistEntries.Any(b => b.Status == BlacklistStatus.NotResolved && b.ValidTo > DateTime.Today))
                {
                    _transactionManager.LogError("Phone has a blacklist record");
                    returnValue = SendSmsResponseStatus.HasBlacklistRecord;
                    
                }
                else
                {
                    //If configuration is not available then create clean phone configuration to phone number   
                    if (phoneConfiguration == null)
                    {
                        phoneConfiguration = createNewPhoneConfiguration();
                        await _repositoryManager.PhoneConfigurations.AddAsync(phoneConfiguration);
                    }

                    await SendMessageToUnknownProcess(false);
                }

            }

            _requestLog.PhoneConfiguration = phoneConfiguration;

            await _repositoryManager.OtpRequestLogs.AddAsync(_requestLog);
            _transactionManager.Transaction.OtpRequestLog = _requestLog;

            sendSmsOtpResponse.Status = returnValue;
            return sendSmsOtpResponse;
        }

        private async Task SendMessageToUnknownProcess(bool discardCurrentOperator)
        {

            //if discardCurrentOperator is true,phone is known number
            var responseLogs = await SendMessageToUnknown(phoneConfiguration, discardCurrentOperator);

            // Decide which response code will be returned
            returnValue = responseLogs.UnifyResponse();

            //Blackliste eklenmesi gerekiyorsa ekle.
            if (returnValue == SendSmsResponseStatus.OperatorChange || returnValue == SendSmsResponseStatus.SimChange)
            {
                _transactionManager.LogError("OperatorChange or SimChange Has Occured");
                if (phoneConfiguration.BlacklistEntries != null)
                {
                    if (phoneConfiguration.BlacklistEntries.All(b => b.Status == BlacklistStatus.Resolved))
                    {
                        var oldBlackListEntry = createOldBlackListEntry((long)_transactionManager.CustomerRequestInfo.CustomerNo, phoneConfiguration.Phone.ToString());
                        await _repositoryManager.DirectBlacklists.AddAsync(oldBlackListEntry);
                        await _repositoryManager.SaveSmsBankingChangesAsync();

                        var blackListEntry = createBlackListEntry(phoneConfiguration, returnValue.ToString(), "SendMessageToUnknownProcess", oldBlackListEntry.SmsId);
                        await _repositoryManager.BlackListEntries.AddAsync(blackListEntry);
                    }
                }
                else
                {
                        var oldBlackListEntry = createOldBlackListEntry((long)_transactionManager.CustomerRequestInfo.CustomerNo, phoneConfiguration.Phone.ToString());
                        await _repositoryManager.DirectBlacklists.AddAsync(oldBlackListEntry);
                        await _repositoryManager.SaveSmsBankingChangesAsync();

                        var blackListEntry = createBlackListEntry(phoneConfiguration, returnValue.ToString(), "SendMessageToUnknownProcess", oldBlackListEntry.SmsId);
                        await _repositoryManager.BlackListEntries.AddAsync(blackListEntry);
                }
            }


            // Update with valid operator if any otp sending 
            var successAttempt = responseLogs.FirstOrDefault(l => (l.ResponseCode == SendSmsResponseStatus.Success 
            || l.ResponseCode == SendSmsResponseStatus.OperatorChange
            || l.ResponseCode == SendSmsResponseStatus.SimChange ));

            if (successAttempt != null)
            {
                _transactionManager.OtpRequestInfo.Operator = successAttempt.Operator;
                phoneConfiguration.Operator = successAttempt.Operator;
                _transactionManager.OtpRequestInfo.PhoneConfiguration = phoneConfiguration;
            }

            // Add all response logs to request log
            responseLogs.ForEach(l => _requestLog.ResponseLogs.Add(l));

        }

        private async Task<List<OtpResponseLog>> SendMessageToUnknown(PhoneConfiguration phoneConfiguration,bool discardCurrentOperator = false)
        {
            var header =  await _headerManager.Get(_data.ContentType,_data.HeaderInfo);
            _requestLog.Content = header.BuildContentForLog(_data.Content);

            ConcurrentBag<OtpResponseLog> responses = new ConcurrentBag<OtpResponseLog>();
            List<Task> tasks = new List<Task>();
            if (_data.Phone.CountryCode == 90)
            {
                foreach (var currentElement in operators)
                {
                    if (discardCurrentOperator)
                    {
                        if (phoneConfiguration.Operator != currentElement.Value)
                        {
                            IOperatorGateway gateway = _operatorRepository(currentElement.Value);
                            tasks.Add(gateway.SendOtp(_data.Phone, header.BuildContentForSms(_data.Content), responses, header));
                        }
                    }
                    else
                    {
                        IOperatorGateway gateway = _operatorRepository(currentElement.Value);
                        tasks.Add(gateway.SendOtp(_data.Phone, header.BuildContentForSms(_data.Content), responses, header));
                    }
                }
            }
            else 
            {
                IOperatorGateway gateway = _operatorRepository(OperatorType.Turkcell);
                tasks.Add(gateway.SendOtp(_data.Phone, header.BuildContentForSms(_data.Content), responses, header));
            }

            await Task.WhenAll(tasks);
            return responses.ToList();
        }

        private async Task<OtpResponseLog> SendMessageToKnown(PhoneConfiguration phoneConfiguration)
        {
            _transactionManager.OtpRequestInfo.Operator = phoneConfiguration.Operator.Value;

            IOperatorGateway gateway = null;
            var header =  await _headerManager.Get(_data.ContentType, _data.HeaderInfo);
            _requestLog.Content = header.BuildContentForLog(_data.Content);

            switch (phoneConfiguration.Operator)
            {
                case OperatorType.Turkcell:
                    gateway = _operatorRepository(OperatorType.Turkcell);
                    break;
                case OperatorType.Vodafone:
                    gateway = _operatorRepository(OperatorType.Vodafone);
                    break;
                case OperatorType.TurkTelekom:
                    gateway = _operatorRepository(OperatorType.TurkTelekom);
                    break;
                case OperatorType.IVN:
                    gateway = _operatorRepository(OperatorType.IVN);
                    break;
                default:
                    // Serious Exception
                    break;
            }

            var result = await gateway.SendOtp(_data.Phone, header.BuildContentForSms(_data.Content), header);

            return result;
        }

        public async Task<OtpTrackingLog> CheckMessage(CheckSmsRequest checkSmsRequest)
        {
            IOperatorGateway gateway = null;

            switch (checkSmsRequest.Operator)
            {
                case OperatorType.Turkcell:
                    gateway = _operatorRepository(OperatorType.Turkcell);
                    break;
                case OperatorType.Vodafone:
                    gateway = _operatorRepository(OperatorType.Vodafone);
                    break;
                case OperatorType.TurkTelekom:
                    gateway = _operatorRepository(OperatorType.TurkTelekom);
                    break;
                case OperatorType.Foreign:
                    gateway = _operatorRepository(OperatorType.Turkcell);
                    break;
                default:
                    // Serious Exception
                    break;
            }

            var result = await gateway.CheckMessageStatus(checkSmsRequest);

            return result;
        }

        public async Task<SendSmsOtpResponse> SendMessageV2(common.Models.v2.SmsRequest smsRequest)
        {
            SendSmsOtpResponse sendSmsOtpResponse = new()
            {
                TxnId = _transactionManager.TxnId
            };
            //Set returnValue ClientError for Unexpected Errors
            returnValue = SendSmsResponseStatus.ClientError;

            //Set Request Body To Class Variable
            _dataV2 = smsRequest;

            //Turkish Character Conversion And Length Validation
            _dataV2.Content = _dataV2.Content.ConvertToTurkish();
            if (_dataV2.Content.Length > 160)
            {
                _transactionManager.LogError("Otp Maximum Characters Count Exceed");
                returnValue = SendSmsResponseStatus.MaximumCharactersCountExceed;
                sendSmsOtpResponse.Status = returnValue;
                return sendSmsOtpResponse;
            }

            _requestLog = new OtpRequestLog
            {
                CreatedBy = _dataV2.Process.MapTo<Process>(),
                Phone = _dataV2.Phone.MapTo<Phone>()
            };

            // Load Phone configuration and related active blacklist entiries.
            phoneConfiguration = _transactionManager.OtpRequestInfo.PhoneConfiguration;

            //Get blacklist records from current Otp System
            var oldBlacklistRecord = (await _repositoryManager.DirectBlacklists.FindAsync(b => b.GsmNumber == _dataV2.Phone.ToString())).OrderByDescending(b => b.RecordDate).FirstOrDefault();

            if (oldBlacklistRecord != null)
            {
                var blackListRecord = phoneConfiguration.BlacklistEntries.FirstOrDefault(b => b.SmsId == oldBlacklistRecord.SmsId);
                if (blackListRecord != null)
                {
                    if (blackListRecord.Status == BlacklistStatus.NotResolved && oldBlacklistRecord.IsVerified)
                    {
                        //Resolve Blacklist entry
                        blackListRecord.Status = BlacklistStatus.Resolved;
                        blackListRecord.ResolvedAt = oldBlacklistRecord.VerifyDate;
                        blackListRecord.ResolvedBy = new Process { Name = oldBlacklistRecord.VerifiedBy };
                    }
                    if (blackListRecord.Status == BlacklistStatus.NotResolved && !oldBlacklistRecord.IsVerified)
                    {
                        oldBlacklistRecord.TryCount++;
                        await _repositoryManager.SaveSmsBankingChangesAsync();
                    }
                }
                else
                {
                    if (!oldBlacklistRecord.IsVerified)
                    {

                        //Increase Try Count
                        oldBlacklistRecord.TryCount++;
                        await _repositoryManager.SaveSmsBankingChangesAsync();

                        //Insert Blacklist entry
                        var blacklistEntry = createBlackListEntryV2(phoneConfiguration, returnValue.ToString(), "SendMessageToKnownProcess", oldBlacklistRecord.SmsId);
                        phoneConfiguration.BlacklistEntries.Add(blacklistEntry);
                        await _repositoryManager.BlackListEntries.AddAsync(blacklistEntry);
                    }
                    else
                    {
                        _transactionManager.OldBlacklistVerifiedAt = oldBlacklistRecord.VerifyDate ?? DateTime.MinValue;
                        //Insert Blacklist entry
                        var blacklistEntry = createBlackListEntryV2(phoneConfiguration, returnValue.ToString(), "SendMessageToKnownProcess", oldBlacklistRecord.SmsId);
                        blacklistEntry.ResolvedAt = oldBlacklistRecord.VerifyDate;
                        blacklistEntry.Status = BlacklistStatus.Resolved;
                        blacklistEntry.ResolvedBy = new();
                        blacklistEntry.ResolvedBy.Name = "Migrated From Old Db";
                        blacklistEntry.ResolvedBy.Identity = oldBlacklistRecord.VerifiedBy;
                        phoneConfiguration.BlacklistEntries.Add(blacklistEntry);
                        await _repositoryManager.BlackListEntries.AddAsync(blacklistEntry);
                    }
                }
            }

            // if known number without any blacklist entry 
            if (phoneConfiguration.Operator != null)
            {
                //Known Number With Active Blacklist Entry
                var blacklistRecord = phoneConfiguration.BlacklistEntries.Where(b => b.ValidTo > DateTime.Today).OrderByDescending(b => b.CreatedAt).FirstOrDefault();
                if (blacklistRecord?.Status != null && blacklistRecord?.Status == BlacklistStatus.NotResolved)
                {
                    _transactionManager.LogError("Phone has a blacklist record");
                    returnValue = SendSmsResponseStatus.HasBlacklistRecord;
                    _requestLog.ResponseLogs.Add(new OtpResponseLog
                    {
                        Operator = (OperatorType)phoneConfiguration.Operator,
                        ResponseCode = SendSmsResponseStatus.HasBlacklistRecord
                    });
                }
                else
                {
                    var responseLog = await SendMessageToKnownV2(phoneConfiguration);
                    _requestLog.ResponseLogs.Add(responseLog);

                    if (responseLog.ResponseCode == SendSmsResponseStatus.OperatorChange
                        || responseLog.ResponseCode == SendSmsResponseStatus.SimChange)
                    {
                        _transactionManager.LogError("OperatorChange or SimChange Has Occured");

                        _transactionManager.LogError("Phone has a blacklist record");
                        var oldBlackListEntry = createOldBlackListEntry((long)_transactionManager.CustomerRequestInfo.CustomerNo, phoneConfiguration.Phone.ToString());
                        await _repositoryManager.DirectBlacklists.AddAsync(oldBlackListEntry);
                        await _repositoryManager.SaveSmsBankingChangesAsync();

                        var blackListEntry = createBlackListEntryV2(phoneConfiguration, returnValue.ToString(), "SendMessageToKnownProcess", oldBlackListEntry.SmsId);
                        await _repositoryManager.BlackListEntries.AddAsync(blackListEntry);

                    }
                    returnValue = responseLog.ResponseCode;

                    //Operator Change | Sim Change | Not Subscriber handle edilmeli
                    if (responseLog.ResponseCode == SendSmsResponseStatus.NotSubscriber)
                    {
                        _transactionManager.LogError("Known Number Changed Operator");

                        //Known Number Returns Not Subscriber For Related Operator
                        //Try to Send Sms With Another Operators
                        //Should pass true for discarding current operator
                        await SendMessageToUnknownProcessV2(true);
                    }
                }
            }
            else
            {
                //Unknown Phone Number
                await SendMessageToUnknownProcessV2(false);
            }

            _requestLog.PhoneConfiguration = phoneConfiguration;
            
            await _repositoryManager.OtpRequestLogs.AddAsync(_requestLog);
            _transactionManager.Transaction.OtpRequestLog = _requestLog;

            sendSmsOtpResponse.Status = returnValue;

            if (sendSmsOtpResponse.Status == SendSmsResponseStatus.Success)
            {
                await _instantReminder.RemindAsync($"Otp Sms | {phoneConfiguration.Operator} | {_requestLog.Phone.Concatenate()}", smsRequest.Content,null);
            }

            return sendSmsOtpResponse;
        }

        private async Task SendMessageToUnknownProcessV2(bool discardCurrentOperator)
        {

            //if discardCurrentOperator is true,phone is known number
            var responseLogs = await SendMessageToUnknownV2(phoneConfiguration, discardCurrentOperator);

            if (responseLogs.All(c => c.ResponseCode == SendSmsResponseStatus.NotSubscriber))
            {
                var foreignOperator = _operatorRepository(OperatorType.Turkcell);

                var header = _headerManager.Get(_dataV2.SmsType);
                var foreignResponse = await foreignOperator.SendOtpForeign(_dataV2.Phone.MapTo<Phone>(), header.BuildContentForSms(_dataV2.Content), header);
                _requestLog.ResponseLogs.Add(foreignResponse);

                responseLogs.Add(foreignResponse);
            }

            // Decide which response code will be returned
            returnValue = responseLogs.UnifyResponse();

            //Blackliste eklenmesi gerekiyorsa ekle.
            if (returnValue == SendSmsResponseStatus.OperatorChange || returnValue == SendSmsResponseStatus.SimChange)
            {
                _transactionManager.LogError("OperatorChange or SimChange Has Occured");
                if (phoneConfiguration.BlacklistEntries != null)
                {
                    if (phoneConfiguration.BlacklistEntries.All(b => b.Status == BlacklistStatus.Resolved))
                    {
                        var oldBlackListEntry = createOldBlackListEntry((long)_transactionManager.CustomerRequestInfo.CustomerNo, phoneConfiguration.Phone.ToString());
                        await _repositoryManager.DirectBlacklists.AddAsync(oldBlackListEntry);
                        await _repositoryManager.SaveSmsBankingChangesAsync();

                        var blackListEntry = createBlackListEntryV2(phoneConfiguration, returnValue.ToString(), "SendMessageToUnknownProcess", oldBlackListEntry.SmsId);
                        await _repositoryManager.BlackListEntries.AddAsync(blackListEntry);
                    }
                }
                else
                {
                    var oldBlackListEntry = createOldBlackListEntry((long)_transactionManager.CustomerRequestInfo.CustomerNo, phoneConfiguration.Phone.ToString());
                    await _repositoryManager.DirectBlacklists.AddAsync(oldBlackListEntry);
                    await _repositoryManager.SaveSmsBankingChangesAsync();

                    var blackListEntry = createBlackListEntryV2(phoneConfiguration, returnValue.ToString(), "SendMessageToUnknownProcess", oldBlackListEntry.SmsId);
                    await _repositoryManager.BlackListEntries.AddAsync(blackListEntry);
                }
            }


            // Update with valid operator if any otp sending 
            var successAttempt = responseLogs.FirstOrDefault(l => (l.ResponseCode == SendSmsResponseStatus.Success
            || l.ResponseCode == SendSmsResponseStatus.OperatorChange
            || l.ResponseCode == SendSmsResponseStatus.SimChange));

            if (successAttempt != null)
            {
                _transactionManager.OtpRequestInfo.Operator = successAttempt.Operator;
                phoneConfiguration.Operator = successAttempt.Operator;
                _transactionManager.OtpRequestInfo.PhoneConfiguration = phoneConfiguration;
            }

            // Add all response logs to request log
            responseLogs.ForEach(l => _requestLog.ResponseLogs.Add(l));

            

        }

        private async Task<List<OtpResponseLog>> SendMessageToUnknownV2(PhoneConfiguration phoneConfiguration,bool discardCurrentOperator = false)
        {
            var header = _headerManager.Get(_dataV2.SmsType);
            _requestLog.Content = header.BuildContentForLog(_dataV2.Content);

            ConcurrentBag<OtpResponseLog> responses = new ConcurrentBag<OtpResponseLog>();
            List<Task> tasks = new List<Task>();
            if (_dataV2.Phone.CountryCode == 90)
            {
                foreach (var currentElement in operators)
                {
                    if (discardCurrentOperator)
                    {
                        if (phoneConfiguration.Operator != currentElement.Value)
                        {
                            IOperatorGateway gateway = _operatorRepository(currentElement.Value);
                            tasks.Add(gateway.SendOtp(_dataV2.Phone.MapTo<Phone>(), header.BuildContentForSms(_dataV2.Content), responses, header));
                        }
                    }
                    else
                    {
                        IOperatorGateway gateway = _operatorRepository(currentElement.Value);
                        tasks.Add(gateway.SendOtp(_dataV2.Phone.MapTo<Phone>(),header.BuildContentForSms(_dataV2.Content), responses, header));
                    }
                }
            }
            else
            {
                IOperatorGateway gateway = _operatorRepository(OperatorType.Turkcell);
                tasks.Add(gateway.SendOtp(_dataV2.Phone.MapTo<Phone>(), header.BuildContentForSms(_dataV2.Content), responses, header));
            }

            await Task.WhenAll(tasks);
            return responses.ToList();
        }

        private async Task<OtpResponseLog> SendMessageToKnownV2(PhoneConfiguration phoneConfiguration)
        {
            _transactionManager.OtpRequestInfo.Operator = phoneConfiguration.Operator.Value;

            IOperatorGateway gateway = null;
            var header = _headerManager.Get(_dataV2.SmsType);
            _requestLog.Content = header.BuildContentForLog(_dataV2.Content);

            switch (phoneConfiguration.Operator)
            {
                case OperatorType.Turkcell:
                    gateway = _operatorRepository(OperatorType.Turkcell);
                    break;
                case OperatorType.Vodafone:
                    gateway = _operatorRepository(OperatorType.Vodafone);
                    break;
                case OperatorType.TurkTelekom:
                    gateway = _operatorRepository(OperatorType.TurkTelekom);
                    break;
                case OperatorType.IVN:
                    gateway = _operatorRepository(OperatorType.IVN);
                    break;
                case OperatorType.Foreign:
                    gateway = _operatorRepository(OperatorType.Turkcell);
                    break;
                case OperatorType.ForeignVodafone:
                    gateway = _operatorRepository(OperatorType.Vodafone);
                    break;
                case OperatorType.Infobip:
                    gateway = _operatorRepository(OperatorType.Turkcell);
                    break;
                default:
                    // Serious Exception
                    break;
            }

            if (phoneConfiguration.Operator == OperatorType.Foreign
                || phoneConfiguration.Operator == OperatorType.ForeignVodafone || phoneConfiguration.Operator == OperatorType.Infobip)
            { 
                var resultForeign = await gateway.SendOtpForeign(_dataV2.Phone.MapTo<Phone>(), header.BuildContentForSms(_dataV2.Content), header);
                return resultForeign;
            }

            var result = await gateway.SendOtp(_dataV2.Phone.MapTo<Phone>(), header.BuildContentForSms(_dataV2.Content), header);
            return result;
        }

        private PhoneConfiguration createNewPhoneConfiguration()
        {
            var newConfig = new PhoneConfiguration
            {
                Phone = _data.Phone,
                Logs = new List<PhoneConfigurationLog>{
                    new PhoneConfigurationLog
                    {
                        Type = "Initialization",
                        Action = "Send Otp Request",
                        RelatedId = _requestLog.Id,
                        CreatedBy = _data.Process
                    }}
            };

            return newConfig;
        }

        private PhoneConfiguration createNewPhoneConfigurationV2()
        {
            var newConfig = new PhoneConfiguration
            {
                Phone = _dataV2.Phone.MapTo<Phone>(),
                Logs = new List<PhoneConfigurationLog>{
                    new PhoneConfigurationLog
                    {
                        Type = "Initialization",
                        Action = "Send Otp Request",
                        RelatedId = _requestLog.Id,
                        CreatedBy = _dataV2.Process.MapTo<Process>()
                    }}
            };

            return newConfig;
        }

        private BlackListEntry createBlackListEntry(PhoneConfiguration phoneConfiguration,string reason,string source,long smsId)
        {
            var newBlackListEntry = new BlackListEntry();
            newBlackListEntry.PhoneConfiguration = phoneConfiguration;
            newBlackListEntry.PhoneConfigurationId = phoneConfiguration.Id;
            newBlackListEntry.Reason = reason;
            newBlackListEntry.Source = source;
            newBlackListEntry.CreatedBy = _data.Process;
            newBlackListEntry.ValidTo = DateTime.Now.AddMonths(1);
            newBlackListEntry.SmsId = smsId;
            newBlackListEntry.Logs = new List<BlackListEntryLog>
            {
                new BlackListEntryLog
                {
                    Action = "Added To Blacklist",
                    BlackListEntry = newBlackListEntry,
                    CreatedBy = _data.Process,
                    ParameterMaster = "master",
                    ParameterSlave = "slave",
                    Type = "type"
                }
            };

            return newBlackListEntry;
        }

        private BlackListEntry createBlackListEntryV2(PhoneConfiguration phoneConfiguration, string reason, string source, long smsId)
        {
            var newBlackListEntry = new BlackListEntry();
            newBlackListEntry.PhoneConfiguration = phoneConfiguration;
            newBlackListEntry.PhoneConfigurationId = phoneConfiguration.Id;
            newBlackListEntry.Reason = reason;
            newBlackListEntry.Source = source;
            newBlackListEntry.CreatedBy = _dataV2.Process.MapTo<Process>();
            newBlackListEntry.ValidTo = DateTime.Now.AddDays(90);
            newBlackListEntry.SmsId = smsId;
            newBlackListEntry.Logs = new List<BlackListEntryLog>
            {
                new BlackListEntryLog
                {
                    Action = "Added To Blacklist",
                    BlackListEntry = newBlackListEntry,
                    CreatedBy = _dataV2.Process.MapTo<Process>(),
                    ParameterMaster = "master",
                    ParameterSlave = "slave",
                    Type = "type"
                }
            };

            return newBlackListEntry;
        }

        private SmsDirectBlacklist createOldBlackListEntry(long customerNo,string phone)
        {
            var newOldBlackListEntry = new SmsDirectBlacklist
            {
                CustomerNo = customerNo,
                GsmNumber = phone,
                RecordDate = DateTime.Now,
                IsVerified = false,
                TryCount = 0,
                Explanation = "Messaging Gateway tarafından eklendi."
            };

            return newOldBlackListEntry;
        }
    }
}
