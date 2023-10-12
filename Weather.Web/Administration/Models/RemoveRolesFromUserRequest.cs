namespace Weather.Web.Administration.Models
{
    public class RemoveRolesFromUserRequest
    {
        public string UserId { get; set; }
        public List<string> RolesToRemove { get; set; }
    }
}
