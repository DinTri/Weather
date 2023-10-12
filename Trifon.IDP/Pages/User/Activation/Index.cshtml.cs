using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Trifon.IDP.Services;

namespace Trifon.IDP.Pages.User.Activation
{
    [SecurityHeaders]
    [AllowAnonymous]

    public class IndexModel : PageModel
    {
        private readonly ILocalUserService _localUserService;

        public IndexModel(ILocalUserService localUserService)
        {
            _localUserService = localUserService ??
                                throw new ArgumentNullException(nameof(localUserService));
        }

        [BindProperty] public InputModel Input { get; set; }

        public async Task<IActionResult> OnGet(string securityCode)
        {
            Input = new InputModel();
            var userActivated = await _localUserService.ActivateUserAsync(securityCode);

            if (userActivated)
            {
                Input.Message = "Your account was successfully activated. " +
                                "Navigate to login page to log in.";
                // Get the base URL
                //var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

                // Construct the login page URL
                var loginPageUrl = $"https://localhost:7184/Authentication/Logout";
                ViewData["LoginPageUrl"] = loginPageUrl;
            }
            else
            {
                Input.Message = "Your account couldn't be activated, " +
                                            "please contact your administrator.";
            }

            await _localUserService.SaveChangesAsync();

            return Page();
        }
    }
}
