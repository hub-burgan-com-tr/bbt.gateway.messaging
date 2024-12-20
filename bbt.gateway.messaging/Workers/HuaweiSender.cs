using bbt.gateway.common.Api.Amorphie;
using bbt.gateway.common.Api.Amorphie.Model;
using bbt.gateway.common.Extensions;
using bbt.gateway.common.Models;
using bbt.gateway.common.Repositories;
using bbt.gateway.messaging.Exceptions;
using bbt.gateway.messaging.Helpers;
using bbt.gateway.messaging.Workers.OperatorGateway;
using Dapr.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Workers
{
    public class HuaweiSender
    {
        private readonly HeaderManager _headerManager;
        private readonly IRepositoryManager _repositoryManager;
        private readonly ITransactionManager _transactionManager;
        private readonly IOperatorHuawei _operatorHuawei;
        private readonly InstantReminder _instantReminder;
        private readonly IUserApi _userApi;
        private readonly IUserApiPrep _userApiPrep;
        private readonly DaprClient _daprClient;


        public HuaweiSender(HeaderManager headerManager,
            IOperatorHuawei operatorHuawei,
            IRepositoryManager repositoryManager,
            ITransactionManager transactionManager,
            InstantReminder instantReminder,
            IUserApi userApi,
            IUserApiPrep userApiPrep,
            DaprClient daprClient
        )
        {
            _headerManager = headerManager;
            _repositoryManager = repositoryManager;
            _transactionManager = transactionManager;
            _operatorHuawei = operatorHuawei;
            _instantReminder = instantReminder;
            _userApi = userApi;
            _userApiPrep = userApiPrep;
            _daprClient = daprClient;
        }

        public async Task<common.Models.v2.NativePushResponse> SendPushNotificationAsync(common.Models.v2.PushRequest data, HuaweiDevice huaweiDevice)
        {
            common.Models.v2.NativePushResponse nativePushResponse = new()
            {
                TxnId = _transactionManager.TxnId,
            };

            await _operatorHuawei.GetOperatorAsync(OperatorType.Huawei);

            var pushRequest = new PushNotificationRequestLog()
            {
                Operator = _operatorHuawei.Type,
                ContactId = data.CitizenshipNo,
                Content = data.Content.MaskFields(),
                TemplateId = "",
                TemplateParams = "",
                CustomParameters = data.CustomParameters?.MaskFields(),
                CreatedBy = data.Process.MapTo<Process>(),
                SaveInbox = data.saveInbox ?? false,
                NotificationType = data.NotificationType ?? string.Empty
            };

            var deviceList = new List<HuaweiDevice>
            {
                huaweiDevice
            };

            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (env.Equals("Development") || env.Equals("Test"))
            {
                try
                {
                    var prepDevice = await _userApiPrep.GetDeviceTokenAsync(data.CitizenshipNo);

                    if (huaweiDevice.token != prepDevice.token)
                    {
                        var huaweiDevicePrep = new HuaweiDevice
                        {
                            token = huaweiDevice.token,
                            app = huaweiDevice.app + "Prep"
                        };

                        deviceList.Add(huaweiDevicePrep);
                    }
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
                        var response = await _operatorHuawei.SendPushNotificationAsync
                                                                    (
                                                                     device.app,
                                                                     device.token,
                                                                     data.Title ?? string.Empty,
                                                                     data.Content,
                                                                     data.CustomParameters
                                                                     );

                        pushRequest.ResponseLogs.Add(response);
                    }
                }
                else
                {
                    throw new WorkflowException("Device Not Found", System.Net.HttpStatusCode.InternalServerError);
                }

                nativePushResponse.Status = pushRequest.ResponseLogs.Any(l => l.ResponseCode.Equals("0")) ? NativePushResponseCodes.Success : NativePushResponseCodes.Failed;

                return nativePushResponse;
            }
            catch
            {
                throw new WorkflowException("An Error Occured", System.Net.HttpStatusCode.InternalServerError);
            }

        }

        public async Task<common.Models.v2.NativePushResponse> SendTemplatedPushNotificationAsync(common.Models.v2.TemplatedPushRequest data, RevampDevice revampDevice, bool isPrep)
        {
            if (string.IsNullOrWhiteSpace(data.CitizenshipNo))
            {
                data.CitizenshipNo = _transactionManager.CustomerRequestInfo.Tckn;
            }

            common.Models.v2.NativePushResponse nativePushResponse = new()
            {
                TxnId = _transactionManager.TxnId,
            };

            var huaweiDevice = new HuaweiDevice
            {
                token = revampDevice.token,
                app = ((_transactionManager.CustomerRequestInfo.BusinessLine == "X" ? "On" : "Burgan") + (isPrep ? "Prep" : "")).TrimEnd()
            };

            await _operatorHuawei.GetOperatorAsync(OperatorType.Huawei);

            var pushRequest = new PushNotificationRequestLog()
            {
                Operator = _operatorHuawei.Type,
                ContactId = data.CitizenshipNo,
                TemplateId = data.Template,
                TemplateParams = data.TemplateParams?.MaskFields(),
                CustomParameters = data.CustomParameters?.MaskFields(),
                CreatedBy = data.Process.MapTo<Process>(),
                SaveInbox = data.saveInbox ?? false,
                NotificationType = data.NotificationType ?? string.Empty
            };

            var templatedPushHelper = new TemplatedPushHelper(_transactionManager, _daprClient);
            var pushTemplateParameter = await templatedPushHelper.SetTemplateParametersAsync(data, pushRequest, revampDevice.os);

            try
            {
                await _repositoryManager.PushNotificationRequestLogs.AddAsync(pushRequest);
                _transactionManager.Transaction.PushNotificationRequestLog = pushRequest;

                var response = await _operatorHuawei.SendPushNotificationAsync
                                                                (
                                                                    huaweiDevice.app, 
                                                                    huaweiDevice.token,
                                                                    pushTemplateParameter.Title, 
                                                                    pushRequest.Content, 
                                                                    data.CustomParameters, 
                                                                    pushTemplateParameter.TargetUrl
                                                                );
                
                pushRequest.ResponseLogs.Add(response);

                nativePushResponse.Status = response.ResponseCode.Equals("0") ? NativePushResponseCodes.Success : NativePushResponseCodes.Failed;

                return nativePushResponse;
            }
            catch
            {
                throw new WorkflowException("Device is not found", System.Net.HttpStatusCode.NotFound);
            }
        }
    }
}