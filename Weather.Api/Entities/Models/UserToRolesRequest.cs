namespace Weather.Api.Entities.Models
{
    public class UserToRolesRequest
    {
        public string UserId { get; set; }
        public List<string> RoleNames { get; set; } = new List<string>();
    }
}
