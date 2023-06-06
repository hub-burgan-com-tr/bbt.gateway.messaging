using Microsoft.AspNetCore.Components;
//using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Components.Authorization;
using Okta.AspNetCore;
using bbt.gateway.messaging.ui.Data;
using Newtonsoft.Json;
using System.Security.Claims;
using bbt.gateway.messaging.ui.Base;
using bbt.gateway.messaging.ui.Base.Token;
using Microsoft.JSInterop;

namespace bbt.gateway.messaging.ui.Pages.Authorize
{
    public partial class BaseAuthorizeView
    {
        [Parameter]
        public RenderFragment AuthorizedControl { get; set; }
        [Parameter]
        public RenderFragment NotAuthorizedControl { get; set; }
        [Parameter]
        public EventCallback<Dictionary<string, string>> navItems
        {
            get; set;
        }

        public RenderFragment Display { get; set; }

        public bool IsAuthorized { get; set; }=false;
        [CascadingParameter]
        Task<AuthenticationState> AuthenticationStated { get; set; }
        [Inject]
        public NavigationManager navigationManager { get; set; }
        [Inject]
        public IJSRuntime JS { get; set; }
        [Inject]
        public bbt.gateway.messaging.ui.Data.HttpContextAccessor httpContext { get; set; }
        protected override async  Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    string accessToken = string.Empty;
                    string responseContent = string.Empty; 
                    string sicil=string.Empty;
                    var user = (await AuthenticationStated).User;
                    sicil = user.Claims.Where(c => c.Type == "sicil")
                             .Select(c => c.Value).SingleOrDefault();
                    if (string.IsNullOrEmpty(sicil))
                    {
                        using (var client = new HttpClient())
                        {

                            accessToken = user.Claims.Where(c => c.Type == "access_token")
                                 .Select(c => c.Value).SingleOrDefault();
                            ITokenService tokenService = FrameworkDependencyHelper.Instance.Get<ITokenService>();
                            string clientBase= tokenService.GetOktaSettings().TokenUrl;
                            client.BaseAddress = new Uri(clientBase);
                            var content = new FormUrlEncodedContent(new[]
                            {
                        new KeyValuePair<string, string>("access_token", accessToken),
                    });


                            var result = await client.PostAsync("/ib/Resource", content);
                            responseContent = result.Content.ReadAsStringAsync().Result;
                            await JS.InvokeVoidAsync("console.log", responseContent);
                            AccessTokenResources? accessTokenResources =
              JsonConvert.DeserializeObject<AccessTokenResources>(responseContent);
                            if (accessTokenResources != null && !string.IsNullOrEmpty(accessTokenResources.sicil) && accessTokenResources.sicil.Length < 12)
                                sicil = accessTokenResources.sicil;

                        }
                    }
                       
                  
                    if (!string.IsNullOrEmpty(sicil))
                    {
                        await JS.InvokeVoidAsync("console.log", sicil);
                        var res = await MessagingGateway.GetUserControl(sicil);

                        if (res !=null&&res.Count>0)
                        {
                            navItems.InvokeAsync(res);
                            string url = System.Text.RegularExpressions.Regex.Replace(navigationManager.Uri, @"/+", @"/");
                            string baseUrl = System.Text.RegularExpressions.Regex.Replace(navigationManager.BaseUri, @"/+", @"/");
                            if( url != baseUrl)
                            {
                                string value;
                                url = url.Replace(baseUrl, "");

                                if (res.TryGetValue(url, out value))
                                {
                                    Display = AuthorizedControl;
                                }
                                else
                                {
                                    Display = NotAuthorizedControl;
                                }
                            }
                            else
                            {
                                Display = AuthorizedControl;
                            }
                            
                        }
                        else
                        {
                            Display = NotAuthorizedControl;
                        }
                    }
                    else
                    {
                        Display = NotAuthorizedControl;
                    }
                    StateHasChanged();

                }
                catch (Exception ex)
                {
                    Display = NotAuthorizedControl;
                    StateHasChanged();
                }

            }


            await base.OnAfterRenderAsync(firstRender);
        }

    }
}
