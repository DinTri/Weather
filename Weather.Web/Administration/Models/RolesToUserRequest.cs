namespace Weather.Web.Administration.Models
{
    public class RolesToUserRequest
    {
        public string? UserId { get; set; }
        public IEnumerable<string> Roles { get; set; } = new List<string>();
    }
}
