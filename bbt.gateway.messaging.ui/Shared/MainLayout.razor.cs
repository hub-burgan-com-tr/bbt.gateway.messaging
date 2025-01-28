using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace bbt.gateway.messaging.ui.Shared
{

    public partial class MainLayout
    {
        [CascadingParameter]
        protected Task<AuthenticationState> AuthenticationState { get; set; }
        [Inject]
        public Data.HttpContextAccessor httpContext { get; set; }
        [Inject]
        public NavigationManager navigationManager { get; set; }
        public Dictionary<string, string> NavItems { get; set; }
        protected override async Task OnParametersSetAsync()
        {
            var user = (await AuthenticationState).User;
            string access_token = user.Claims.Where(c => c.Type == "access_token").Select(c => c.Value).SingleOrDefault();
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