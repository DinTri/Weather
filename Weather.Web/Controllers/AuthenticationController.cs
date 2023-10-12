using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Weather.CustomExceptions;

namespace Weather.Web.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthenticationController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ??
                throw new ArgumentNullException(nameof(httpClientFactory));
        }

        [Authorize]
        public async Task Logout()
        {
            var client = _httpClientFactory.CreateClient("IDPClient");

            var discoveryDocumentResponse = await client
                .GetDiscoveryDocumentAsync();
            if (discoveryDocumentResponse.IsError)
            {
                throw new WeatherException(discoveryDocumentResponse.Error);
            }

            var accessTokenRevocationResponse = await client
                .RevokeTokenAsync(new()
                {
                    Address = discoveryDocumentResponse.RevocationEndpoint,
                    ClientId = "weatherclient",
                    ClientSecret = "secret",
                    Token = await HttpContext.GetTokenAsync(
                        OpenIdConnectParameterNames.AccessToken) ?? throw new InvalidOperationException()
                });

            if (accessTokenRevocationResponse.IsError)
            {
                throw new WeatherException(accessTokenRevocationResponse.Error);
            }

            var refreshTokenRevocationResponse = await client
                .RevokeTokenAsync(new()
                {
                    Address = discoveryDocumentResponse.RevocationEndpoint,
                    ClientId = "weatherclient",
                    ClientSecret = "secret",
                    Token = await HttpContext.GetTokenAsync(
                        OpenIdConnectParameterNames.RefreshToken) ?? throw new InvalidOperationException()
                });

            if (refreshTokenRevocationResponse.IsError)
            {
                throw new WeatherException(accessTokenRevocationResponse.Error);
            }

            // Clears the  local cookie
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            // Redirects to the IDP linked to scheme
            // "OpenIdConnectDefaults.AuthenticationScheme" (oidc)
            // so it can clear its own session/cookie
            await HttpContext.SignOutAsync(
                OpenIdConnectDefaults.AuthenticationScheme);
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}