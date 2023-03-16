using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace bbt.gateway.messaging.ui.Base.Token
{
    public class OktaTokenService : ITokenService
    {
        private OktaToken token = new OktaToken();
        public readonly IOptions<OktaSettings> oktaSettings;
        private IHttpContextAccessor httpContextAccessor;
        private IHttpClientFactory ClientFactory;
        public OktaTokenService(IOptions<OktaSettings> _oktaSettings, IHttpContextAccessor _httpContextAccessor, IHttpClientFactory _ClientFactory)
        {
            this.oktaSettings = _oktaSettings;
            httpContextAccessor = _httpContextAccessor;
            ClientFactory= _ClientFactory;
        }

        public async Task<string> GetToken()
        {
            if (!this.token.IsValidAndNotExpiring)
            {
                token= await  this.RefreshToken();
            }
            return token.AccessToken;
        }
        //private  async Task RefreshToken2()
        //{
        //    var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
        //       .WithRedirectUri("http://localhost:5066/callback")
        //       .Build();
        //    await httpContextAccessor.HttpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
        //}
            private async Task<OktaToken> RefreshToken()
        {
            var token = new OktaToken();

            try
            {
                var identity = (ClaimsIdentity)httpContextAccessor.HttpContext.User.Identity;
                var refreshtoken = identity.Claims.Where(c => c.Type == "refresh_token").Select(s=>s.Value).SingleOrDefault();
                var client = ClientFactory.CreateClient();
                var client_id = this.oktaSettings.Value.ClientId;
                var client_secret = this.oktaSettings.Value.ClientSecret;
                var clientCreds = System.Text.Encoding.UTF8.GetBytes($"{client_id}:{client_secret}");
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", System.Convert.ToBase64String(clientCreds));
                client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
                
                var postMessage = new Dictionary<string, string>();
                postMessage.Add("grant_type", "refresh_token");
                postMessage.Add("scope", "openid profile");
                postMessage.Add("redirect_uri", "http://localhost:5066/authorization-code/callback");
                postMessage.Add("refresh_token", refreshtoken);
                postMessage.Add("client_id", this.oktaSettings.Value.ClientId);
                postMessage.Add("client_secret", this.oktaSettings.Value.ClientSecret);

                var request = new HttpRequestMessage(HttpMethod.Post, this.oktaSettings.Value.OktaDomain + "/v1/token/")
                {
                    Content = new FormUrlEncodedContent(postMessage),
                   

                };
                //
                //    using (var httpClient2 = new HttpClient())
                //    {

                //        httpClient2.DefaultRequestHeaders.Authorization =
                //        new AuthenticationHeaderValue("Basic", System.Convert.ToBase64String(clientCreds));
                //        httpClient2.DefaultRequestHeaders.Accept.Add(
                //new MediaTypeWithQualityHeaderValue("application/json"));
                //        using (var content = new FormUrlEncodedContent(postMessage))
                //        {
                //            content.Headers.ContentType= new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                //            HttpResponseMessage response2 =  httpClient2.PostAsync(this.oktaSettings.Value.OktaDomain + "/v1/token/", content).Result;

                //            var json = await response2.Content.ReadAsStringAsync();
                //            token = JsonConvert.DeserializeObject<OktaToken>(json);
                //            //return await response2.Content.ReadAsAsync<TResult>();
                //        }
                //    }
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

                var response = client.Send(request);
                var jsontest = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    token = JsonConvert.DeserializeObject<OktaToken>(json);
                    token.ExpiresAt = DateTime.UtcNow.AddSeconds(this.token.ExpiresIn);
                }
                else
                {
                    throw new ApplicationException("Unable to retrieve access token from Okta");
                }
            }
            catch (Exception ex)
            {

            }
        

            return token;
        }

        public OktaSettings GetOktaSettings()
        {
            return oktaSettings.Value;
        }
    }
}
