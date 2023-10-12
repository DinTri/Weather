namespace Weather.Api.Entities.Models
{
    public class RemoveRolesFromUserRequest
    {
        public string UserId { get; set; }
        public List<string> RolesToRemove { get; set; }
    }
}
