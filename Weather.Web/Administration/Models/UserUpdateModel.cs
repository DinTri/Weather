namespace Weather.Web.Administration.Models
{
    public class UserUpdateModel
    {
        public string? UserId { get; set; }
        public string? FamilyName { get; set; }
        public string? PhoneNumber { get; set; }
        public bool Suspended { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public IFormFile? Photo { get; set; }
    }
}
