namespace Weather.Api.Entities
{
    public class AspNetUsers
    {
        public string? Id { get; set; } = Guid.NewGuid().ToString();
        public string? Subject { get; set; }

        public string? GivenName { get; set; }

        public string? FamilyName { get; set; }

        public string? Password { get; set; }

        public bool Suspended { get; set; } = false;

        public string? ProfilePhotoPath { get; set; }

        public string? SecurityCode { get; set; }

        public DateTime SecurityCodeExpirationDate { get; set; }

        public int CountryId { get; set; }

        public Country? Country { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public string? UserName { get; set; }

        public string? NormalizedUserName { get; set; }

        public string? Email { get; set; }

        public string? NormalizedEmail { get; set; }

        public bool EmailConfirmed { get; set; } = false;

        public string? PasswordHash { get; set; }

        public string? SecurityStamp { get; set; } = Guid.NewGuid().ToString();

        public string? ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

        public string? PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; } = false;

        public bool TwoFactorEnabled { get; set; } = false;

        public DateTimeOffset? LockoutEnd { get; set; }

        public bool LockoutEnabled { get; set; }

        public int AccessFailedCount { get; set; } = 0;
    }
}
