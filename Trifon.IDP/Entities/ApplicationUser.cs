#nullable enable
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Trifon.IDP.Entities
{
    public class ApplicationUser : IdentityUser, IConcurrencyAware
    {
        [MaxLength(200)]
        [Required]
        public string? Subject { get; set; }

        [MaxLength(25)]
        public string? GivenName { get; set; }

        [MaxLength(25)]
        public string? FamilyName { get; set; }

        [Required]
        public bool Suspended { get; set; }

        public string ProfilePhotoPath { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? SecurityCode { get; set; }

        public DateTime SecurityCodeExpirationDate { get; set; }
        public int? CountryId { get; set; }

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

        public ICollection<IdentityUserClaim<string>> Claims { get; set; } = new List<IdentityUserClaim<string>>();
        public IEnumerable<IdentityUserLogin<string>> Logins { get; set; }
        public ICollection<UserSecret> Secrets { get; set; } = new List<UserSecret>();
    }
}
