using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Weather.Api.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(200)]
        [Required]
        public string? Subject { get; set; }

        [MaxLength(25)]
        public string? GivenName { get; set; }

        [MaxLength(25)]
        public string? FamilyName { get; set; }

        [MaxLength(200)]
        public string? Password { get; set; }

        [Required]
        public bool Suspended { get; set; }

        public string ProfilePhotoPath { get; set; } = string.Empty;
        public int CountryId { get; set; }

        // Navigation property to access the related Country entity
        public Country? Country { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public void SetUpdatedAt()
        {
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetDeletedAt()
        {
            DeletedAt = DateTime.UtcNow;
        }

    }
}
