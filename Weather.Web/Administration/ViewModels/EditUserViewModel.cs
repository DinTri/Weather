using System.ComponentModel.DataAnnotations;

namespace Weather.Web.Administration.ViewModels
{
    public class EditUserViewModel
    {
        public string? Id { get; set; }

        [Required]
        public string? UserName { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        public string? FamilyName { get; set; }
        public string? PhoneNumber { get; set; }
        public bool Suspended { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool LockoutEnabled { get; set; }
        public IFormFile? Photo { get; set; }

        //public List<string> Claims { get; set; } = new List<string>();

        public IList<string>? Roles { get; set; } = new List<string>();
    }
}
