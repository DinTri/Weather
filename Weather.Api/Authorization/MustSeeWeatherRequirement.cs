using Microsoft.AspNetCore.Authorization;

namespace Weather.Api.Authorization
{
    public class MustSeeWeatherRequirement : IAuthorizationRequirement
    {
        public MustSeeWeatherRequirement()
        {

        }
    }
}
