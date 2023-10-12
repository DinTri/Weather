using System.Security.Claims;

namespace Weather.Web.Administration.Models
{
    public class UserToRolesRequest
    {
        public string? UserId { get; set; }
        public IEnumerable<(string RoleName, IEnumerable<Claim> Claims)>? RolesWithClaims { get; set; }
    }
}
