using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace bbt.gateway.messaging.ui.Shared
{
   
    public partial class MainLayout
    {
        [CascadingParameter]
        protected Task<AuthenticationState> AuthenticationState { get; set; }
        [Inject]
        public bbt.gateway.messaging.ui.Data.HttpContextAccessor httpContext { get; set; }
        [Inject]
        public NavigationManager navigationManager { get; set; }
        public Dictionary<string, string> NavItems { get; set; }
        protected override async Task OnParametersSetAsync()
            {
            var user = (await AuthenticationState).User;
            string access_token = user.Claims.Where(c => c.Type == "access_token")
                  .Select(c => c.Value).SingleOrDefault();
            //if (!string.IsNullOrEmpty(access_token))
            //{
            //    string sicil = user.Claims.Where(c => c.Type == "sicil")
            //                 .Select(c => c.Value).SingleOrDefault();
            //    if(string.IsNullOrEmpty(sicil))
            //    {
            //        var identity = (ClaimsIdentity)httpContext.Context.User.Identity;

            //    }
            //}
           


        }
        public async Task HandelNavItemsAsync(
           Dictionary<string, string> items)
        {
            NavItems = items;
        }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            httpContext.Context.Features.Get<HttpContext>();
        }
        public void LoginSite()
        {
            navigationManager.NavigateTo($"login?redirectUri=/", forceLoad: true);
        }
        public void LogoutSite()
        {
            navigationManager.NavigateTo($"logoutPage?redirectUri=/", forceLoad: true);
        }
    }
}
