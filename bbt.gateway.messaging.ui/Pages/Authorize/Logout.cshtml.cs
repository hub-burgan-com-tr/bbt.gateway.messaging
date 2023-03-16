using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace bbt.gateway.messaging.ui.Pages.Authorize
{
    public class LogoutModel : PageModel
    {
        public async Task OnGet(string redirectUri)
        {

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
        public override RedirectToPageResult RedirectToPage()
        {
            return base.RedirectToPage();
        }
    }
}
