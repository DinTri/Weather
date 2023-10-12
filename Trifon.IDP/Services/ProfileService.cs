using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Trifon.IDP.Entities;

namespace Trifon.IDP.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileService(
            UserManager<ApplicationUser> userMgr,
            RoleManager<IdentityRole> roleMgr,
            IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory)
        {
            _userManager = userMgr;
            _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            string subjectId = context.Subject.GetSubjectId();
            ApplicationUser user = await _userManager.FindByIdAsync(subjectId);

            if (user != null)
            {
                await _userClaimsPrincipalFactory.CreateAsync(user);

                var claims = new List<Claim>
                {
                    new("given_name", user.GivenName),
                    new("family_name", user.FamilyName),
                    new("country", user.Country.Name)
                };
                // Add role claims
                var roles = await _userManager.GetRolesAsync(user);
                claims.AddRange(roles.Select(role => new Claim(JwtClaimTypes.Role, role)));

                context.IssuedClaims = claims;
            }
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            string subjectId = context.Subject.GetSubjectId();
            ApplicationUser user = await _userManager.FindByIdAsync(subjectId);
            if (user != null) context.IsActive = !user.Suspended;
        }
    }
}
