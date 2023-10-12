namespace Weather.Api.Entities.Models
{
    public class AddRolesToUserRequest
    {
        public string? UserId { get; set; }
        public IEnumerable<string> RolesToAdd { get; set; } = new List<string>();
    }
}
