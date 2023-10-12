namespace Weather.Api.Entities.Models
{
    public class UserUpdateModel
    {
        //public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? FamilyName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public bool Suspended { get; set; } = false;
        public bool TwoFactorEnabled { get; set; } = false;
        public bool LockoutEnabled { get; set; } = false;
        public IFormFile? Photo { get; set; }
    }
}
