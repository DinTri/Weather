namespace Weather.Web.Administration.Models
{
    public class AddRolesToUserRequest
    {
        public string UserId { get; set; }
        public List<string> RolesToAdd { get; set; }
    }
}
