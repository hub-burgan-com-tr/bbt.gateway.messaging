using Auth0.AspNetCore.Authentication;
using bbt.gateway.messaging.ui.Base.Token;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Polly;
using RestEase;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Policy;

namespace bbt.gateway.messaging.ui.Base
{
    public abstract class BaseRefit<TInterface>
    {
        NavigationManager _navigationManager { get; set; }
        public TInterface api;
        public abstract string controllerName { get; }
        IHttpContextAccessor _httpContextAccessor { get; set; }
        ILogger baseLog;
        string _baseURL = string.Empty;
        public BaseRefit(string baseURL, IHttpContextAccessor httpContextAccessor,NavigationManager navigationManager)
        {
            var user = httpContextAccessor.HttpContext.User;
            string token = user.Claims.Where(c => c.Type == "access_token")
                   .Select(c => c.Value).SingleOrDefault();
            api = RestClient.For<TInterface>(baseURL, async (request, cancellationToken) =>
            {
                // See if the request has an authorize header
                var auth = request.Headers.Authorization;

                if (auth != null && !string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue(auth.Scheme, token);
                }
            });
            _httpContextAccessor = httpContextAccessor;
            _navigationManager = navigationManager;
            _baseURL = baseURL;
        }

        protected async Task<TReturnModel> ExecutePolly<TReturnModel>(Func<TReturnModel> action)
        {

            var policy = Policy
               .Handle<Exception>()
               .WaitAndRetry(new[]
                  {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                  }, async (exception, timeSpan, retryCount, context) =>
                  {
                      if (exception.Message.Contains("401 (Unauthorized)"))
                      {
                       

                        
                      }
                      else
                      {
                          //baseLog.LogError(exception.Message);
                      }
                  });


            return policy.Execute<TReturnModel>(action);
        }
    }
}
