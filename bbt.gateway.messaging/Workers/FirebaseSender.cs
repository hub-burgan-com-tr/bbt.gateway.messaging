using bbt.gateway.common.Api.Amorphie;
using bbt.gateway.common.Api.Amorphie.Model;
using bbt.gateway.common.Extensions;
using bbt.gateway.common.Models;
using bbt.gateway.common.Repositories;
using bbt.gateway.messaging.Exceptions;
using bbt.gateway.messaging.Helpers;
using bbt.gateway.messaging.Workers.OperatorGateway;
using Dapr.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Workers
{
    public class FirebaseSender
    {
        private readonly HeaderManager _headerManager;
        private readonly IRepositoryManager _repositoryManager;
        private readonly ITransactionManager _transactionManager;
        private readonly IOperatorFirebase _operatorFirebase;
        private readonly InstantReminder _instantReminder;
        private readonly IUserApi _userApi;
        private readonly IUserApiPrep _userApiPrep;
        private readonly DaprClient _daprClient;

        public FirebaseSender(HeaderManager headerManager,
            IOperatorFirebase operatorFirebase,
            IRepositoryManager repositoryManager,
            ITransactionManager transactionManager,
            InstantReminder instantReminder,
            IUserApi userApi,
            IUserApiPrep userApiPrep,
            IConfiguration configuration,
            DaprClient daprClient
        )
        {
            _headerManager = headerManager;
            _repositoryManager = repositoryManager;
            _transactionManager = transactionManager;
            _operatorFirebase = operatorFirebase;
            _instantReminder = instantReminder;
            _userApi = userApi;
            _userApiPrep = userApiPrep;
            _daprClient = daprClient;
        }

        public async Task CheckPushNotificationAsync()
        {
            await Task.CompletedTask;
        }

        public async Task<common.Models.v2.NativePushResponse> SendPushNotificationAsync(common.Models.v2.PushRequest data, RevampDevice revampDevice)
        {
            if (string.IsNullOrWhiteSpace(data.CitizenshipNo))
            {
                data.CitizenshipNo = _transactionManager.CustomerRequestInfo.Tckn;
            }

            common.Models.v2.NativePushResponse firebasePushResponse = new()
            {
                TxnId = _transactionManager.TxnId,
            };

            await _operatorFirebase.GetOperatorAsync(OperatorType.Firebase);

            var pushRequest = new PushNotificationRequestLog()
            {
                Operator = _operatorFirebase.Type,
                ContactId = data.CitizenshipNo,
                Content = data.Content.MaskFields(),
                TemplateId = "",
                TemplateParams = "",
                CustomParameters = data.CustomParameters?.MaskFields(),
                CreatedBy = data.Process.MapTo<Process>(),
                SaveInbox = data.saveInbox ?? false,
                NotificationType = data.NotificationType ?? string.Empty
            };

            List<RevampDevice> deviceList = new();
            deviceList.Add(revampDevice);

            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (env.Equals("Development") || env.Equals("Test"))
            {
                try
                {
                    var prepDevice = await _userApiPrep.GetDeviceTokenAsync(data.CitizenshipNo);

                    if (revampDevice.token != prepDevice.token)
                        deviceList.Add(prepDevice);
                }
                catch { }
            }

            try
            {
                await _repositoryManager.PushNotificationRequestLogs.AddAsync(pushRequest);
                _transactionManager.Transaction.PushNotificationRequestLog = pushRequest;

                if (deviceList.Count > 0)
                {
                    foreach (var device in deviceList)
                    {
                        var response = await _operatorFirebase.SendPushNotificationAsync(device.token, data.Title ?? string.Empty, data.Content, data.CustomParameters);
                        pushRequest.ResponseLogs.Add(response);
                    }
                }
                else
                {
                    throw new WorkflowException("Device Not Found", System.Net.HttpStatusCode.InternalServerError);
                }

                firebasePushResponse.Status = pushRequest.ResponseLogs.Any(l => l.ResponseCode.Equals("0")) ? NativePushResponseCodes.Success : NativePushResponseCodes.Failed;

                return firebasePushResponse;
            }
            catch
            {
                throw new WorkflowException("An Error Occured", System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public async Task<common.Models.v2.NativePushResponse> SendTemplatedPushNotificationAsync(common.Models.v2.TemplatedPushRequest data, RevampDevice device)
        {
            if (string.IsNullOrWhiteSpace(data.CitizenshipNo))
            {
                data.CitizenshipNo = _transactionManager.CustomerRequestInfo.Tckn;
            }

            common.Models.v2.NativePushResponse firebasePushResponse = new()
            {
                TxnId = _transactionManager.TxnId,
            };

            await _operatorFirebase.GetOperatorAsync(OperatorType.Firebase);

            var pushRequest = new PushNotificationRequestLog()
            {
                Operator = _operatorFirebase.Type,
                ContactId = data.CitizenshipNo,
                TemplateId = data.Template,
                TemplateParams = data.TemplateParams?.MaskFields(),
                CustomParameters = data.CustomParameters?.MaskFields(),
                CreatedBy = data.Process.MapTo<Process>(),
                SaveInbox = data.saveInbox ?? false,
                NotificationType = data.NotificationType ?? string.Empty
            };

            var templatedPushHelper = new TemplatedPushHelper(_transactionManager, _daprClient);
            var pushTemplateParameter = await templatedPushHelper.SetTemplateParametersAsync(data, pushRequest, device.os);

            try
            {
                await _repositoryManager.PushNotificationRequestLogs.AddAsync(pushRequest);
                _transactionManager.Transaction.PushNotificationRequestLog = pushRequest;

                var response = await _operatorFirebase.SendPushNotificationAsync
                                                                    (
                                                                        device.token,
                                                                        pushTemplateParameter.Title, 
                                                                        pushRequest.Content, 
                                                                        data.CustomParameters,
                                                                        pushTemplateParameter.TargetUrl
                                                                    );
               
                pushRequest.ResponseLogs.Add(response);

                firebasePushResponse.Status = response.ResponseCode.Equals("0") ? NativePushResponseCodes.Success : NativePushResponseCodes.Failed;

                return firebasePushResponse;
            }
            catch
            {
                throw new WorkflowException("Device is not found", System.Net.HttpStatusCode.NotFound);
            }
        }       
    }
}