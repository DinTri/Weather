namespace Weather.Api.Entities.Models
{
    public class AddUserToRoleResult
    {
        public bool Succeeded { get; set; }
        public string? FailureReason { get; set; }
    }

}
