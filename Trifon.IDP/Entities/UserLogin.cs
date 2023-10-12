using System.ComponentModel.DataAnnotations;

namespace Trifon.IDP.Entities
{
    public class UserLogin : IConcurrencyAware
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(200)]
        [Required]
        public string Provider { get; set; }

        [MaxLength(200)]
        [Required]
        public string ProviderIdentityKey { get; set; }

        [Required]
        public string UserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
    }
}
