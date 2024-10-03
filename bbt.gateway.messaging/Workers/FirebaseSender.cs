using bbt.gateway.common.Api.Amorphie;
using bbt.gateway.common.Extensions;
using bbt.gateway.common.Models;
using bbt.gateway.common.Repositories;
using bbt.gateway.messaging.Exceptions;
using bbt.gateway.messaging.Helpers;
using bbt.gateway.messaging.Workers.OperatorGateway;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
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

        public FirebaseSender(HeaderManager headerManager,
            IOperatorFirebase operatorFirebase,
            IRepositoryManager repositoryManager,
            ITransactionManager transactionManager,
            InstantReminder instantReminder,
            IUserApi userApi,
            IConfiguration configuration
        )
        {
            _headerManager = headerManager;
            _repositoryManager = repositoryManager;
            _transactionManager = transactionManager;
            _operatorFirebase = operatorFirebase;
            _instantReminder = instantReminder;
            _userApi = userApi;
            
        }

        public async Task CheckPushNotificationAsync()
        {
            await Task.CompletedTask;
        }

        public async Task<common.Models.v2.FirebasePushResponse> SendPushNotificationAsync(common.Models.v2.PushRequest data)
        {
            common.Models.v2.FirebasePushResponse firebasePushResponse = new()
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

            try
            {
                await _repositoryManager.PushNotificationRequestLogs.AddAsync(pushRequest);
                _transactionManager.Transaction.PushNotificationRequestLog = pushRequest;

                var deviceToken = await _userApi.GetDeviceTokenAsync(data.CitizenshipNo);
                var response = await _operatorFirebase.SendPushNotificationAsync(await deviceToken.Content.ReadAsStringAsync(), data.Title ?? string.Empty, data.Content, data.CustomParameters);
                pushRequest.ResponseLogs.Add(response);

                firebasePushResponse.Status = response.ResponseCode.Equals("0") ? FirebasePushResponseCodes.Success : FirebasePushResponseCodes.Failed;

                return firebasePushResponse;
            }
            catch (System.Exception ex)
            {
                throw new WorkflowException("Device is not found",System.Net.HttpStatusCode.NotFound);
            }
            
        }
    }



}
