using System.ComponentModel.DataAnnotations;

namespace Trifon.IDP.Entities
{
    public class UserSecret : IConcurrencyAware
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Secret { get; set; }

        [Required]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public string ConcurrencyStamp { get; set; } =
            Guid.NewGuid().ToString();
    }
}
