using System.ComponentModel.DataAnnotations;

namespace Weather.Web.Administration.ViewModels
{
    public class UserResponse
    {
        public string Id { get; set; } = string.Empty;
        public string? UserName { get; set; }

        [MaxLength(25)]
        public string? GivenName { get; set; }

        [MaxLength(25)]
        public string? FamilyName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public bool Suspended { get; set; }
        public bool LockoutEnabled { get; set; }
        public string ProfilePhotoPath { get; set; } = string.Empty;
        public int CountryId { get; set; }

        // Navigation property to access the related Country entity
        public Country? Country { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public void SetUpdatedAt()
        {
            UpdatedAt = DateTime.UtcNow;
        }

    }
}
