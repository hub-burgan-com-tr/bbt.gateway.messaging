using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.RazorPages;

using bbt.gateway.messaging.ui.Base;

namespace bbt.gateway.messaging.ui.Pages.Authorize
{
    public class LoginModel : PageModel
    {
        [Inject]
        bbt.gateway.messaging.ui.Data.HttpContextAccessor contextAccessor { get; set; }

        public async  Task OnGet(string redirectUri)
        {
           IConfiguration config= FrameworkDependencyHelper.Instance.Get<IConfiguration>();
          string path=  config.GetValue<string>("Base:path")+ "/searchMessages";
            //string redirect ="http://"+ HttpContext.Request.Host.Value+"/callback";
            //await JS.InvokeAsync<string>("console.log", redirect);
            var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
                .WithRedirectUri(path)
                .Build();
           await HttpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
        }
        
    }
}
