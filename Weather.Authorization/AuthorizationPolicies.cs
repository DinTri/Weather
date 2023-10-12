using Microsoft.AspNetCore.Authorization;

namespace Weather.Authorization
{
    public static class AuthorizationPolicies
    {
        public static AuthorizationPolicy CanSeeWeather()
        {
            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireClaim("country", "BG", "IE", "GB", "US", "NI", "AT", "DK", "DE")
                .Build();
        }

        public static AuthorizationPolicy AdminCanManageRoles()
        {
            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireRole("Admin")
                .Build();
        }
        public static AuthorizationPolicy DeleteRolePolicy()
        {
            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireRole("Admin")
                .Build();
        }
        public static AuthorizationPolicy EditRolePolicy()
        {
            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireRole("Admin")
                .Build();
        }

    }
}