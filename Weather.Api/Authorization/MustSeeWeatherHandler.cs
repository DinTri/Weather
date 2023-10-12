using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Weather.Api.Authorization
{
    public class MustSeeWeatherHandler : AuthorizationHandler<MustSeeWeatherRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MustSeeWeatherRequirement requirement)
        {
            var ownerId = context.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            if (ownerId == null)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            if (context.User.HasClaim(c => c is { Type: ClaimTypes.Role, Value: "Admin" }))
            {
                context.Succeed(requirement); // Mark the requirement as successful
            }
            else
            {
                context.Fail(); // Mark the requirement as unsuccessful
            }

            return Task.CompletedTask;
        }
    }
}
