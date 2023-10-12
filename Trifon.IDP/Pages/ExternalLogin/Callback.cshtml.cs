using Duende.IdentityServer;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Services;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Trifon.IDP.Data;
using Trifon.IDP.Services;
using Weather.CustomExceptions;

namespace Trifon.IDP.Pages.ExternalLogin;

[AllowAnonymous]
[SecurityHeaders]
public class Callback : PageModel
{
    private readonly IIdentityServerInteractionService _interaction;
    private readonly ILogger<Callback> _logger;
    private readonly ILocalUserService _localUserService;
    private readonly IEventService _events;
    private readonly ApplicationDbContext _context;

    private readonly Dictionary<string, string> _facebookClaimTypeMap = new()
    {
        {
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
            JwtClaimTypes.Name
        },
        {
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname",
            JwtClaimTypes.GivenName
        },
        {
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname",
            JwtClaimTypes.FamilyName
        },
        {
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress",
            JwtClaimTypes.Email
        }
    };

    public Callback(
        IIdentityServerInteractionService interaction,
        IEventService events,
        ILogger<Callback> logger,
        ILocalUserService localUserService, ApplicationDbContext context)
    {
        _interaction = interaction;
        _logger = logger;
        _localUserService = localUserService ??
                            throw new ArgumentNullException(nameof(localUserService));
        _events = events;
        _context = context;
    }

    public static string LocationClaimType { get; private set; }

    public async Task<IActionResult> OnGet()
    {
        // read external identity from the temporary cookie
        var result = await HttpContext.AuthenticateAsync(
            IdentityServerConstants.ExternalCookieAuthenticationScheme);
        if (!result.Succeeded)
        {
            throw new WeatherException("External authentication error");
        }

        var externalUser = result.Principal;

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            var externalClaims = externalUser.Claims
                .Select(c => $"{c.Type}: {c.Value}");
            _logger.LogDebug("External claims: {@claims}", externalClaims);
        }

        // lookup our user and external provider info
        // try to determine the unique id of the external user (issued by the provider)
        // the most common claim type for that are the sub claim and the NameIdentifier
        // depending on the external provider, some other claim type might be used
        var userIdClaim = externalUser.FindFirst(JwtClaimTypes.Subject) ??
                          externalUser.FindFirst(ClaimTypes.NameIdentifier) ??
                          throw new Exception("Unknown userid");

        var provider = result.Properties.Items["scheme"];
        var providerUserId = userIdClaim.Value;
        // find external user
        var user = await _localUserService
                .FindUserByExternalProviderAsync(provider, providerUserId);
        if (user == null)
        {
            // remove the userid claim: that information is
            // stored in the UserLogins table
            var claims = externalUser.Claims.ToList();
            claims.Remove(userIdClaim);
            user = _localUserService.AutoProvisionUser(provider, providerUserId, claims.ToList());
            // different external login providers often require different
        }


        // this allows us to collect any additional claims or properties
        // for the specific protocols used and store them in the local auth cookie.
        // this is typically used to store data needed for signout from those protocols.
        var additionalLocalClaims = new List<Claim>();
        var localSignInProps = new AuthenticationProperties();
        CaptureExternalLoginContext(result, additionalLocalClaims,
            localSignInProps);



        var isuser = new IdentityServerUser(user.Subject)
        {
            DisplayName = user.UserName,
            IdentityProvider = provider,
            AdditionalClaims = additionalLocalClaims
        };

        await HttpContext.SignInAsync(isuser, localSignInProps);



        // delete temporary cookie used during external authentication
        await HttpContext.SignOutAsync(
            IdentityServerConstants.ExternalCookieAuthenticationScheme);

        // retrieve return URL
        var returnUrl = result.Properties.Items["returnUrl"] ?? "~/";

        // check if external login is in the context of an OIDC request
        var context = await _interaction.GetAuthorizationContextAsync(
            returnUrl);
        await _events.RaiseAsync(
            new UserLoginSuccessEvent(provider, providerUserId,
                user.Subject, user.UserName, true, context?.Client.ClientId));

        if (context != null && context.IsNativeClient())
        {
            // The client is native, so this change in how to
            // return the response is for better UX for the end user.
            return this.LoadingPage(returnUrl);
        }

        return Redirect(returnUrl);
    }

    // if the external login is OIDC-based, there are certain things we need to preserve to make logout work
    // this will be different for WS-Fed, SAML2p or other protocols
    private void CaptureExternalLoginContext(
        AuthenticateResult externalResult, List<Claim> localClaims,
        AuthenticationProperties localSignInProps)
    {
        // if the external system sent a session id claim, copy it over
        // so we can use it for single sign-out
        var sid = externalResult.Principal?.Claims
            .FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
        if (sid != null)
        {
            localClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
        }

        // if the external provider issued an id_token, we'll keep it for signout
        if (externalResult.Properties != null)
        {
            var idToken = externalResult.Properties.GetTokenValue("id_token");
            if (idToken != null)
            {
                localSignInProps.StoreTokens(
                    new[]
                    {
                        new AuthenticationToken
                        {
                            Name = "id_token", Value = idToken
                        }
                    });
            }
        }
    }
}