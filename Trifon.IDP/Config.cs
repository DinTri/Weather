using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace Trifon.IDP;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResource("roles",
                "Your role(s)",
                new [] { "role" }),
            new IdentityResource("country",
                "The country you're living in",
                new List<string>() { "country" })
        };

    public static IEnumerable<ApiResource> ApiResources =>
     new[]
         {
             new ApiResource("weatherapi",
                 "Weather API",
                 new [] { "role", "country" })
             {
                 Scopes = { "weatherapi.fullaccess",
                     "weatherapi.read",
                     "weatherapi.write"},
                ApiSecrets = { new Secret("apisecret".Sha256()) }
             }
         };


    public static IEnumerable<ApiScope> ApiScopes =>
        new[]
            {
                new ApiScope("weatherapi.fullaccess"),
                new ApiScope("weatherapi.read"),
                new ApiScope("weatherapi.write")};

    public static IEnumerable<Client> Clients =>
        new[]
            {
                new Client()
                {
                    ClientName = "Weather Forecast",
                    ClientId = "weatherclient",
                    AllowedGrantTypes = GrantTypes.Code,
                    AccessTokenType = AccessTokenType.Reference,
                    AllowOfflineAccess = true,
                    UpdateAccessTokenClaimsOnRefresh = true,
                    AccessTokenLifetime = 120,
                    // AuthorizationCodeLifetime = ...
                    // IdentityTokenLifetime = ...
                    RedirectUris =
                    {
                        "https://localhost:7184/signin-oidc"
                    },
                    PostLogoutRedirectUris =
                    {
                        "https://localhost:7184/signout-callback-oidc"
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "roles",
                        //"weatherapi.fullaccess",
                        "weatherapi.read",
                        "weatherapi.write",
                        "country"
                    },
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                   // RequireConsent = true
                }
            };
}