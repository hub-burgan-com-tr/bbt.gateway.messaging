using bbt.gateway.common.Api.Amorphie;
using bbt.gateway.common.Api.Amorphie.Model;
using bbt.gateway.common.Models;
using bbt.gateway.common.Models.v2;
using bbt.gateway.messaging.Exceptions;
using System;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Workers
{
    public class NativePushSender
    {
        private readonly IUserApi _userApi;
        private readonly IUserApiPrep _userApiPrep;
        private readonly FirebaseSender _firebaseSender;
        private readonly HuaweiSender _huaweiSender;
        private readonly ITransactionManager _transactionManager;


        public NativePushSender(
                                 FirebaseSender firebaseSender,
                                 HuaweiSender huaweiSender,
                                 IUserApi userApi,
                                 IUserApiPrep userApiPrep,
                                 ITransactionManager transactionManager
                               )
        {
            _firebaseSender = firebaseSender;
            _userApi = userApi;
            _userApiPrep = userApiPrep;
            _huaweiSender = huaweiSender;
            _transactionManager = transactionManager;
        }

        public async Task<NativePushResponse> SendPushNotificationAsync(PushRequest data)
        {
            try
            {
                var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                RevampDevice revampDevice;
                bool isPrep = false;

                if (env.Equals("Development") || env.Equals("Test"))
                {
                    try
                    {
                        revampDevice = await _userApi.GetDeviceTokenAsync(data.CitizenshipNo);
                    }
                    catch
                    {
                        revampDevice = await _userApiPrep.GetDeviceTokenAsync(data.CitizenshipNo);
                        isPrep = true;
                    }
                }
                else
                {
                    revampDevice = await _userApi.GetDeviceTokenAsync(data.CitizenshipNo);
                }

                if (revampDevice.IsGoogleServiceAvailable)
                {
                    var responseFirebase = await _firebaseSender.SendPushNotificationAsync(data, revampDevice);
                    return responseFirebase;
                }
                else
                {
                    var huaweiDevice = new HuaweiDevice
                    {
                        token = revampDevice.token,
                        app = ((_transactionManager.CustomerRequestInfo.BusinessLine == "X" ? "On" : "Burgan") + (isPrep ? "Prep" : "")).TrimEnd()
                    };

                    var responseHuawei = await _huaweiSender.SendPushNotificationAsync(data, huaweiDevice);
                    return responseHuawei;
                }
            }
            catch (Exception ex)
            {
                _transactionManager.LogError("NativePushSender.SendPushNotificationAsync ex:" + ex.ToString());
                throw new WorkflowException("An Error Occured", System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public async Task<NativePushResponse> SendTemplatedPushNotificationAsync(TemplatedPushRequest data)
        {
            try
            {
                var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                RevampDevice revampDevice;
                bool isPrep = false;

                if (env.Equals("Development") || env.Equals("Test"))
                {
                    try
                    {
                        revampDevice = await _userApi.GetDeviceTokenAsync(data.CitizenshipNo);
                    }
                    catch
                    {
                        revampDevice = await _userApiPrep.GetDeviceTokenAsync(data.CitizenshipNo);
                        isPrep = true;
                    }
                }
                else
                {
                    revampDevice = await _userApi.GetDeviceTokenAsync(data.CitizenshipNo);
                }

                if (revampDevice.IsGoogleServiceAvailable)
                {
                    var responseFirebase = await _firebaseSender.SendTemplatedPushNotificationAsync(data, revampDevice);
                    return responseFirebase;
                }
                else
                {
                    var responseHuawei = await _huaweiSender.SendTemplatedPushNotificationAsync(data, revampDevice, isPrep);
                    return responseHuawei;
                }
            }
            catch (Exception ex)
            {
                _transactionManager.LogError("NativePushSender.SendTemplatedPushNotificationAsync ex:" + ex.ToString());
                throw new WorkflowException("An Error Occured", System.Net.HttpStatusCode.InternalServerError);
            }
        }
    }
}