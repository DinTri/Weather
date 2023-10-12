using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Trifon.IDP.Entities;

namespace Trifon.IDP.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Country> Countries { get; set; }
        public DbSet<UserSecret> UserSecrets { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure identity tables
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("AspNetUsers");
                entity.HasIndex(u => u.Subject)
                    .IsUnique();
                entity.HasIndex(u => u.UserName)
                    .IsUnique();
                entity.HasIndex(u => u.Email)
                    .IsUnique();
            });


            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Claims)
                .WithOne()
                .HasForeignKey(c => c.UserId)
                .IsRequired();

            builder.Entity<IdentityUserClaim<string>>()
                .HasKey(uc => uc.Id);

            // Configure the relationship between ApplicationUser and Country
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Country)
                .WithMany()
                .HasForeignKey(u => u.CountryId);

            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("AspNetUserLogins");
                entity.HasKey(ul => new { ul.LoginProvider, ul.ProviderKey });
                entity.HasOne<ApplicationUser>().WithMany().HasForeignKey(ul => ul.UserId);
            });

            builder.Entity<UserSecret>()
                .HasOne(us => us.User)
                .WithMany(u => u.Secrets) // Assuming ApplicationUser has a Secrets property
                .HasForeignKey(us => us.UserId)
                .IsRequired();
            builder.Entity<Entities.ApplicationUser>()
                .Property(u => u.CreatedAt)
            .IsRequired();

            builder.Entity<Entities.ApplicationUser>()
                .Property(u => u.UpdatedAt);

            builder.Entity<Entities.ApplicationUser>()
                .Property(u => u.DeletedAt);

            builder.Entity<Entities.ApplicationUser>()
                .HasIndex(u => u.Subject)
                .IsUnique();
            builder.Entity<Entities.ApplicationUser>()
                .Property(u => u.UserName)
                .IsRequired();

            builder.Entity<Country>().HasData(
                new Country { Id = 1, Iso = "AF", Name = "Afghanistan", Iso3 = "AFG", Numcode = 4, PhoneCode = 93 },
                new Country { Id = 2, Iso = "AL", Name = "Albania", Iso3 = "ALB", Numcode = 8, PhoneCode = 355 },
                new Country { Id = 3, Iso = "DZ", Name = "Algeria", Iso3 = "DZA", Numcode = 12, PhoneCode = 213 },
                new Country
                {
                    Id = 4,
                    Iso = "AS",
                    Name = "American Samoa",
                    Iso3 = "ASM",
                    Numcode = 16,
                    PhoneCode = 1684
                },
                new Country { Id = 5, Iso = "AD", Name = "Andorra", Iso3 = "AND", Numcode = 20, PhoneCode = 376 },
                new Country { Id = 6, Iso = "AO", Name = "Angola", Iso3 = "AGO", Numcode = 24, PhoneCode = 244 },
                new Country { Id = 7, Iso = "AI", Name = "Anguilla", Iso3 = "AIA", Numcode = 660, PhoneCode = 1264 },
                new Country { Id = 8, Iso = "AQ", Name = "Antarctica", Iso3 = null, Numcode = -1, PhoneCode = 0 },
                new Country
                {
                    Id = 9,
                    Iso = "AG",
                    Name = "Antigua and Barbuda",
                    Iso3 = "ATG",
                    Numcode = 28,
                    PhoneCode = 1268
                },
                new Country { Id = 10, Iso = "AR", Name = "Argentina", Iso3 = "ARG", Numcode = 32, PhoneCode = 54 },
                new Country { Id = 11, Iso = "AM", Name = "Armenia", Iso3 = "ARM", Numcode = 51, PhoneCode = 374 },
                new Country { Id = 12, Iso = "AW", Name = "Aruba", Iso3 = "ABW", Numcode = 533, PhoneCode = 297 },
                new Country { Id = 13, Iso = "AU", Name = "Australia", Iso3 = "AUS", Numcode = 36, PhoneCode = 61 },
                new Country { Id = 14, Iso = "AT", Name = "Austria", Iso3 = "AUT", Numcode = 40, PhoneCode = 43 },
                new Country { Id = 15, Iso = "AZ", Name = "Azerbaijan", Iso3 = "AZE", Numcode = 31, PhoneCode = 994 },
                new Country { Id = 16, Iso = "BS", Name = "Bahamas", Iso3 = "BHS", Numcode = 44, PhoneCode = 1242 },
                new Country { Id = 17, Iso = "BH", Name = "Bahrain", Iso3 = "BHR", Numcode = 48, PhoneCode = 973 },
                new Country { Id = 18, Iso = "BD", Name = "Bangladesh", Iso3 = "BGD", Numcode = 50, PhoneCode = 880 },
                new Country { Id = 19, Iso = "BB", Name = "Barbados", Iso3 = "BRB", Numcode = 52, PhoneCode = 1246 },
                new Country { Id = 20, Iso = "BY", Name = "Belarus", Iso3 = "BLR", Numcode = 112, PhoneCode = 375 },
                new Country { Id = 21, Iso = "BE", Name = "Belgium", Iso3 = "BEL", Numcode = 56, PhoneCode = 32 },
                new Country { Id = 22, Iso = "BZ", Name = "Belize", Iso3 = "BLZ", Numcode = 84, PhoneCode = 501 },
                new Country { Id = 23, Iso = "BJ", Name = "Benin", Iso3 = "BEN", Numcode = 204, PhoneCode = 229 },
                new Country { Id = 24, Iso = "BM", Name = "Bermuda", Iso3 = "BMU", Numcode = 60, PhoneCode = 1441 },
                new Country { Id = 25, Iso = "BT", Name = "Bhutan", Iso3 = "BTN", Numcode = 64, PhoneCode = 975 },
                new Country { Id = 26, Iso = "BO", Name = "Bolivia", Iso3 = "BOL", Numcode = 68, PhoneCode = 591 },
                new Country
                {
                    Id = 27,
                    Iso = "BA",
                    Name = "Bosnia and Herzegovina",
                    Iso3 = "BIH",
                    Numcode = 70,
                    PhoneCode = 387
                },
                new Country { Id = 28, Iso = "BW", Name = "Botswana", Iso3 = "BWA", Numcode = 72, PhoneCode = 267 },
                new Country { Id = 29, Iso = "BV", Name = "Bouvet Island", Iso3 = null, Numcode = -1, PhoneCode = 0 },
                new Country { Id = 30, Iso = "BR", Name = "Brazil", Iso3 = "BRA", Numcode = 76, PhoneCode = 55 },
                new Country
                {
                    Id = 31,
                    Iso = "IO",
                    Name = "British Indian Ocean Territory",
                    Iso3 = null,
                    Numcode = -1,
                    PhoneCode = 246
                },
                new Country
                {
                    Id = 32,
                    Iso = "BN",
                    Name = "Brunei Darussalam",
                    Iso3 = "BRN",
                    Numcode = 96,
                    PhoneCode = 673
                },
                new Country { Id = 33, Iso = "BG", Name = "Bulgaria", Iso3 = "BGR", Numcode = 100, PhoneCode = 359 },
                new Country
                {
                    Id = 34,
                    Iso = "BF",
                    Name = "Burkina Faso",
                    Iso3 = "BFA",
                    Numcode = 854,
                    PhoneCode = 226
                },
                new Country { Id = 35, Iso = "BI", Name = "Burundi", Iso3 = "BDI", Numcode = 108, PhoneCode = 257 },
                new Country { Id = 36, Iso = "KH", Name = "Cambodia", Iso3 = "KHM", Numcode = 116, PhoneCode = 855 },
                new Country { Id = 37, Iso = "CM", Name = "Cameroon", Iso3 = "CMR", Numcode = 120, PhoneCode = 237 },
                new Country { Id = 38, Iso = "CA", Name = "Canada", Iso3 = "CAN", Numcode = 124, PhoneCode = 1 },
                new Country { Id = 39, Iso = "CV", Name = "Cape Verde", Iso3 = "CPV", Numcode = 132, PhoneCode = 238 },
                new Country
                {
                    Id = 40,
                    Iso = "KY",
                    Name = "Cayman Islands",
                    Iso3 = "CYM",
                    Numcode = 136,
                    PhoneCode = 1345
                },
                new Country
                {
                    Id = 41,
                    Iso = "CF",
                    Name = "Central African Republic",
                    Iso3 = "CAF",
                    Numcode = 140,
                    PhoneCode = 236
                },
                new Country { Id = 42, Iso = "TD", Name = "Chad", Iso3 = "TCD", Numcode = 148, PhoneCode = 235 },
                new Country { Id = 43, Iso = "CL", Name = "Chile", Iso3 = "CHL", Numcode = 152, PhoneCode = 56 },
                new Country { Id = 44, Iso = "CN", Name = "China", Iso3 = "CHN", Numcode = 156, PhoneCode = 86 },
                new Country
                {
                    Id = 45,
                    Iso = "CX",
                    Name = "Christmas Island",
                    Iso3 = null,
                    Numcode = -1,
                    PhoneCode = 61
                },
                new Country
                {
                    Id = 46,
                    Iso = "CC",
                    Name = "Cocos (Keeling) Islands",
                    Iso3 = null,
                    Numcode = -1,
                    PhoneCode = 672
                },
                new Country { Id = 47, Iso = "CO", Name = "Colombia", Iso3 = "COL", Numcode = 170, PhoneCode = 57 },
                new Country { Id = 48, Iso = "KM", Name = "Comoros", Iso3 = "COM", Numcode = 174, PhoneCode = 269 },
                new Country { Id = 49, Iso = "CG", Name = "Congo", Iso3 = "COG", Numcode = 178, PhoneCode = 242 },
                new Country
                {
                    Id = 50,
                    Iso = "CD",
                    Name = "Congo, the Democratic Republic of the",
                    Iso3 = "COD",
                    Numcode = 180,
                    PhoneCode = 242
                },
                new Country
                {
                    Id = 51,
                    Iso = "CK",
                    Name = "Cook Islands",
                    Iso3 = "COK",
                    Numcode = 184,
                    PhoneCode = 682
                },
                new Country { Id = 52, Iso = "CR", Name = "Costa Rica", Iso3 = "CRI", Numcode = 188, PhoneCode = 506 },
                new Country
                {
                    Id = 53,
                    Iso = "CI",
                    Name = "Cote D'Ivoire",
                    Iso3 = "CIV",
                    Numcode = 384,
                    PhoneCode = 225
                },
                new Country { Id = 54, Iso = "HR", Name = "Croatia", Iso3 = "HRV", Numcode = 191, PhoneCode = 385 },
                new Country { Id = 55, Iso = "CU", Name = "Cuba", Iso3 = "CUB", Numcode = 192, PhoneCode = 53 },
                new Country { Id = 56, Iso = "CY", Name = "Cyprus", Iso3 = "CYP", Numcode = 196, PhoneCode = 357 },
                new Country
                {
                    Id = 57,
                    Iso = "CZ",
                    Name = "Czech Republic",
                    Iso3 = "CZE",
                    Numcode = 203,
                    PhoneCode = 420
                },
                new Country { Id = 58, Iso = "DK", Name = "Denmark", Iso3 = "DNK", Numcode = 208, PhoneCode = 45 },
                new Country { Id = 59, Iso = "DJ", Name = "Djibouti", Iso3 = "DJI", Numcode = 262, PhoneCode = 253 },
                new Country { Id = 60, Iso = "DM", Name = "Dominica", Iso3 = "DMA", Numcode = 212, PhoneCode = 1767 },
                new Country
                {
                    Id = 61,
                    Iso = "DO",
                    Name = "Dominican Republic",
                    Iso3 = "DOM",
                    Numcode = 214,
                    PhoneCode = 1809
                },
                new Country { Id = 62, Iso = "EC", Name = "Ecuador", Iso3 = "ECU", Numcode = 218, PhoneCode = 593 },
                new Country { Id = 63, Iso = "EG", Name = "Egypt", Iso3 = "EGY", Numcode = 818, PhoneCode = 20 },
                new Country { Id = 64, Iso = "SV", Name = "El Salvador", Iso3 = "SLV", Numcode = 222, PhoneCode = 503 },
                new Country
                {
                    Id = 65,
                    Iso = "GQ",
                    Name = "Equatorial Guinea",
                    Iso3 = "GNQ",
                    Numcode = 226,
                    PhoneCode = 240
                },
                new Country { Id = 66, Iso = "ER", Name = "Eritrea", Iso3 = "ERI", Numcode = 232, PhoneCode = 291 },
                new Country { Id = 67, Iso = "EE", Name = "Estonia", Iso3 = "EST", Numcode = 233, PhoneCode = 372 },
                new Country { Id = 68, Iso = "ET", Name = "Ethiopia", Iso3 = "ETH", Numcode = 231, PhoneCode = 251 },
                new Country
                {
                    Id = 69,
                    Iso = "FK",
                    Name = "Falkland Islands (Malvinas)",
                    Iso3 = "FLK",
                    Numcode = 238,
                    PhoneCode = 500
                },
                new Country
                {
                    Id = 70,
                    Iso = "FO",
                    Name = "Faroe Islands",
                    Iso3 = "FRO",
                    Numcode = 234,
                    PhoneCode = 298
                },
                new Country { Id = 71, Iso = "FJ", Name = "Fiji", Iso3 = "FJI", Numcode = 242, PhoneCode = 679 },
                new Country { Id = 72, Iso = "FI", Name = "Finland", Iso3 = "FIN", Numcode = 246, PhoneCode = 358 },
                new Country { Id = 73, Iso = "FR", Name = "France", Iso3 = "FRA", Numcode = 250, PhoneCode = 33 },
                new Country
                {
                    Id = 74,
                    Iso = "GF",
                    Name = "French Guiana",
                    Iso3 = "GUF",
                    Numcode = 254,
                    PhoneCode = 594
                },
                new Country
                {
                    Id = 75,
                    Iso = "PF",
                    Name = "French Polynesia",
                    Iso3 = "PYF",
                    Numcode = 258,
                    PhoneCode = 689
                },
                new Country
                {
                    Id = 76,
                    Iso = "TF",
                    Name = "French Southern Territories",
                    Iso3 = null,
                    Numcode = -1,
                    PhoneCode = 0
                },
                new Country { Id = 77, Iso = "GA", Name = "Gabon", Iso3 = "GAB", Numcode = 266, PhoneCode = 241 },
                new Country { Id = 78, Iso = "GM", Name = "Gambia", Iso3 = "GMB", Numcode = 270, PhoneCode = 220 },
                new Country { Id = 79, Iso = "GE", Name = "Georgia", Iso3 = "GEO", Numcode = 268, PhoneCode = 995 },
                new Country { Id = 80, Iso = "DE", Name = "Germany", Iso3 = "DEU", Numcode = 276, PhoneCode = 49 },
                new Country { Id = 81, Iso = "GH", Name = "Ghana", Iso3 = "GHA", Numcode = 288, PhoneCode = 233 },
                new Country { Id = 82, Iso = "GI", Name = "Gibraltar", Iso3 = "GIB", Numcode = 292, PhoneCode = 350 },
                new Country { Id = 83, Iso = "GR", Name = "Greece", Iso3 = "GRC", Numcode = 300, PhoneCode = 30 },
                new Country { Id = 84, Iso = "GL", Name = "Greenland", Iso3 = "GRL", Numcode = 304, PhoneCode = 299 },
                new Country { Id = 85, Iso = "GD", Name = "Grenada", Iso3 = "GRD", Numcode = 308, PhoneCode = 1473 },
                new Country { Id = 86, Iso = "GP", Name = "Guadeloupe", Iso3 = "GLP", Numcode = 312, PhoneCode = 590 },
                new Country { Id = 87, Iso = "GU", Name = "Guam", Iso3 = "GUM", Numcode = 316, PhoneCode = 1671 },
                new Country { Id = 88, Iso = "GT", Name = "Guatemala", Iso3 = "GTM", Numcode = 320, PhoneCode = 502 },
                new Country { Id = 89, Iso = "GN", Name = "Guinea", Iso3 = "GIN", Numcode = 324, PhoneCode = 224 },
                new Country
                {
                    Id = 90,
                    Iso = "GW",
                    Name = "Guinea-Bissau",
                    Iso3 = "GNB",
                    Numcode = 624,
                    PhoneCode = 245
                },
                new Country { Id = 91, Iso = "GY", Name = "Guyana", Iso3 = "GUY", Numcode = 328, PhoneCode = 592 },
                new Country { Id = 92, Iso = "HT", Name = "Haiti", Iso3 = "HTI", Numcode = 332, PhoneCode = 509 },
                new Country
                {
                    Id = 93,
                    Iso = "HM",
                    Name = "Heard Island and Mcdonald Islands",
                    Iso3 = null,
                    Numcode = -1,
                    PhoneCode = 0
                },
                new Country
                {
                    Id = 94,
                    Iso = "VA",
                    Name = "Holy See (Vatican City State)",
                    Iso3 = "VAT",
                    Numcode = 336,
                    PhoneCode = 39
                },
                new Country { Id = 95, Iso = "HN", Name = "Honduras", Iso3 = "HND", Numcode = 340, PhoneCode = 504 },
                new Country { Id = 96, Iso = "HK", Name = "Hong Kong", Iso3 = "HKG", Numcode = 344, PhoneCode = 852 },
                new Country { Id = 97, Iso = "HU", Name = "Hungary", Iso3 = "HUN", Numcode = 348, PhoneCode = 36 },
                new Country { Id = 98, Iso = "IS", Name = "Iceland", Iso3 = "ISL", Numcode = 352, PhoneCode = 354 },
                new Country { Id = 99, Iso = "IN", Name = "India", Iso3 = "IND", Numcode = 356, PhoneCode = 91 },
                new Country { Id = 100, Iso = "ID", Name = "Indonesia", Iso3 = "IDN", Numcode = 360, PhoneCode = 62 },
                new Country
                {
                    Id = 101,
                    Iso = "IR",
                    Name = "Iran, Islamic Republic of",
                    Iso3 = "IRN",
                    Numcode = 364,
                    PhoneCode = 98
                },
                new Country { Id = 102, Iso = "IQ", Name = "Iraq", Iso3 = "IRQ", Numcode = 368, PhoneCode = 964 },
                new Country { Id = 103, Iso = "IE", Name = "Ireland", Iso3 = "IRL", Numcode = 372, PhoneCode = 353 },
                new Country { Id = 104, Iso = "IL", Name = "Israel", Iso3 = "ISR", Numcode = 376, PhoneCode = 972 },
                new Country { Id = 105, Iso = "IT", Name = "Italy", Iso3 = "ITA", Numcode = 380, PhoneCode = 39 },
                new Country { Id = 106, Iso = "JM", Name = "Jamaica", Iso3 = "JAM", Numcode = 388, PhoneCode = 1876 },
                new Country { Id = 107, Iso = "JP", Name = "Japan", Iso3 = "JPN", Numcode = 392, PhoneCode = 81 },
                new Country { Id = 108, Iso = "JO", Name = "Jordan", Iso3 = "JOR", Numcode = 400, PhoneCode = 962 },
                new Country { Id = 109, Iso = "KZ", Name = "Kazakhstan", Iso3 = "KAZ", Numcode = 398, PhoneCode = 7 },
                new Country { Id = 110, Iso = "KE", Name = "Kenya", Iso3 = "KEN", Numcode = 404, PhoneCode = 254 },
                new Country { Id = 111, Iso = "KI", Name = "Kiribati", Iso3 = "KIR", Numcode = 296, PhoneCode = 686 },
                new Country
                {
                    Id = 112,
                    Iso = "KP",
                    Name = "Korea, Democratic People's Republic of",
                    Iso3 = "PRK",
                    Numcode = 408,
                    PhoneCode = 850
                },
                new Country
                {
                    Id = 113,
                    Iso = "KR",
                    Name = "Korea, Republic of",
                    Iso3 = "KOR",
                    Numcode = 410,
                    PhoneCode = 82
                },
                new Country { Id = 114, Iso = "KW", Name = "Kuwait", Iso3 = "KWT", Numcode = 414, PhoneCode = 965 },
                new Country { Id = 115, Iso = "KG", Name = "Kyrgyzstan", Iso3 = "KGZ", Numcode = 417, PhoneCode = 996 },
                new Country
                {
                    Id = 116,
                    Iso = "LA",
                    Name = "Lao People's Democratic Republic",
                    Iso3 = "LAO",
                    Numcode = 418,
                    PhoneCode = 856
                },
                new Country { Id = 117, Iso = "LV", Name = "Latvia", Iso3 = "LVA", Numcode = 428, PhoneCode = 371 },
                new Country { Id = 118, Iso = "LB", Name = "Lebanon", Iso3 = "LBN", Numcode = 422, PhoneCode = 961 },
                new Country { Id = 119, Iso = "LS", Name = "Lesotho", Iso3 = "LSO", Numcode = 426, PhoneCode = 266 },
                new Country { Id = 120, Iso = "LR", Name = "Liberia", Iso3 = "LBR", Numcode = 430, PhoneCode = 231 },
                new Country
                {
                    Id = 121,
                    Iso = "LY",
                    Name = "Libyan Arab Jamahiriya",
                    Iso3 = "LBY",
                    Numcode = 434,
                    PhoneCode = 218
                },
                new Country
                {
                    Id = 122,
                    Iso = "LI",
                    Name = "Liechtenstein",
                    Iso3 = "LIE",
                    Numcode = 438,
                    PhoneCode = 423
                },
                new Country { Id = 123, Iso = "LT", Name = "Lithuania", Iso3 = "LTU", Numcode = 440, PhoneCode = 370 },
                new Country { Id = 124, Iso = "LU", Name = "Luxembourg", Iso3 = "LUX", Numcode = 442, PhoneCode = 352 },
                new Country { Id = 125, Iso = "MO", Name = "Macao", Iso3 = "MAC", Numcode = 446, PhoneCode = 853 },
                new Country
                {
                    Id = 126,
                    Iso = "MK",
                    Name = "Macedonia, the Former Yugoslav Republic of",
                    Iso3 = "MKD",
                    Numcode = 807,
                    PhoneCode = 389
                },
                new Country { Id = 127, Iso = "MG", Name = "Madagascar", Iso3 = "MDG", Numcode = 450, PhoneCode = 261 },
                new Country { Id = 128, Iso = "MW", Name = "Malawi", Iso3 = "MWI", Numcode = 454, PhoneCode = 265 },
                new Country { Id = 129, Iso = "MY", Name = "Malaysia", Iso3 = "MYS", Numcode = 458, PhoneCode = 60 },
                new Country { Id = 130, Iso = "MV", Name = "Maldives", Iso3 = "MDV", Numcode = 462, PhoneCode = 960 },
                new Country { Id = 131, Iso = "ML", Name = "Mali", Iso3 = "MLI", Numcode = 466, PhoneCode = 223 },
                new Country { Id = 132, Iso = "MT", Name = "Malta", Iso3 = "MLT", Numcode = 470, PhoneCode = 356 },
                new Country
                {
                    Id = 133,
                    Iso = "MH",
                    Name = "Marshall Islands",
                    Iso3 = "MHL",
                    Numcode = 584,
                    PhoneCode = 692
                },
                new Country { Id = 134, Iso = "MQ", Name = "Martinique", Iso3 = "MTQ", Numcode = 474, PhoneCode = 596 },
                new Country { Id = 135, Iso = "MR", Name = "Mauritania", Iso3 = "MRT", Numcode = 478, PhoneCode = 222 },
                new Country { Id = 136, Iso = "MU", Name = "Mauritius", Iso3 = "MUS", Numcode = 480, PhoneCode = 230 },
                new Country { Id = 137, Iso = "YT", Name = "Mayotte", Iso3 = "MYT", Numcode = 1758, PhoneCode = 262 },
                new Country { Id = 138, Iso = "MX", Name = "Mexico", Iso3 = "MEX", Numcode = 484, PhoneCode = 52 },
                new Country
                {
                    Id = 139,
                    Iso = "FM",
                    Name = "Micronesia, Federated States of",
                    Iso3 = "FSM",
                    Numcode = 583,
                    PhoneCode = 691
                },
                new Country
                {
                    Id = 140,
                    Iso = "MD",
                    Name = "Moldova, Republic of",
                    Iso3 = "MDA",
                    Numcode = 498,
                    PhoneCode = 373
                },
                new Country { Id = 141, Iso = "MC", Name = "Monaco", Iso3 = "MCO", Numcode = 492, PhoneCode = 377 },
                new Country { Id = 142, Iso = "MN", Name = "Mongolia", Iso3 = "MNG", Numcode = 496, PhoneCode = 976 },
                new Country
                {
                    Id = 143,
                    Iso = "MS",
                    Name = "Montserrat",
                    Iso3 = "MSR",
                    Numcode = 500,
                    PhoneCode = 1664
                },
                new Country { Id = 144, Iso = "MA", Name = "Morocco", Iso3 = "MAR", Numcode = 504, PhoneCode = 212 },
                new Country { Id = 145, Iso = "MZ", Name = "Mozambique", Iso3 = "MOZ", Numcode = 508, PhoneCode = 258 },
                new Country { Id = 146, Iso = "MM", Name = "Myanmar", Iso3 = "MMR", Numcode = 104, PhoneCode = 95 },
                new Country { Id = 147, Iso = "NA", Name = "Namibia", Iso3 = "NAM", Numcode = 516, PhoneCode = 264 },
                new Country { Id = 148, Iso = "NR", Name = "Nauru", Iso3 = "NRU", Numcode = 520, PhoneCode = 674 },
                new Country { Id = 149, Iso = "NP", Name = "Nepal", Iso3 = "NPL", Numcode = 524, PhoneCode = 977 },
                new Country { Id = 150, Iso = "NL", Name = "Netherlands", Iso3 = "NLD", Numcode = 528, PhoneCode = 31 },
                new Country
                {
                    Id = 151,
                    Iso = "AN",
                    Name = "Netherlands Antilles",
                    Iso3 = "ANT",
                    Numcode = 530,
                    PhoneCode = 599
                },
                new Country
                {
                    Id = 152,
                    Iso = "NC",
                    Name = "New Caledonia",
                    Iso3 = "NCL",
                    Numcode = 540,
                    PhoneCode = 687
                },
                new Country { Id = 153, Iso = "NZ", Name = "New Zealand", Iso3 = "NZL", Numcode = 554, PhoneCode = 64 },
                new Country { Id = 154, Iso = "NI", Name = "Nicaragua", Iso3 = "NIC", Numcode = 558, PhoneCode = 505 },
                new Country { Id = 155, Iso = "NE", Name = "Niger", Iso3 = "NER", Numcode = 562, PhoneCode = 227 },
                new Country { Id = 156, Iso = "NG", Name = "Nigeria", Iso3 = "NGA", Numcode = 566, PhoneCode = 234 },
                new Country { Id = 157, Iso = "NU", Name = "Niue", Iso3 = "NIU", Numcode = 570, PhoneCode = 683 },
                new Country
                {
                    Id = 158,
                    Iso = "NF",
                    Name = "Norfolk Island",
                    Iso3 = "NFK",
                    Numcode = 574,
                    PhoneCode = 672
                },
                new Country
                {
                    Id = 159,
                    Iso = "MP",
                    Name = "Northern Mariana Islands",
                    Iso3 = "MNP",
                    Numcode = 580,
                    PhoneCode = 1670
                },
                new Country { Id = 160, Iso = "NO", Name = "Norway", Iso3 = "NOR", Numcode = 578, PhoneCode = 47 },
                new Country { Id = 161, Iso = "OM", Name = "Oman", Iso3 = "OMN", Numcode = 512, PhoneCode = 968 },
                new Country { Id = 162, Iso = "PK", Name = "Pakistan", Iso3 = "PAK", Numcode = 586, PhoneCode = 92 },
                new Country { Id = 163, Iso = "PW", Name = "Palau", Iso3 = "PLW", Numcode = 585, PhoneCode = 680 },
                new Country
                {
                    Id = 164,
                    Iso = "PS",
                    Name = "Palestinian Territory, Occupied",
                    Iso3 = "PSE",
                    Numcode = 275,
                    PhoneCode = 970
                },
                new Country { Id = 165, Iso = "PA", Name = "Panama", Iso3 = "PAN", Numcode = 591, PhoneCode = 507 },
                new Country
                {
                    Id = 166,
                    Iso = "PG",
                    Name = "Papua New Guinea",
                    Iso3 = "PNG",
                    Numcode = 598,
                    PhoneCode = 675
                },
                new Country { Id = 167, Iso = "PY", Name = "Paraguay", Iso3 = "PRY", Numcode = 600, PhoneCode = 595 },
                new Country { Id = 168, Iso = "PE", Name = "Peru", Iso3 = "PER", Numcode = 604, PhoneCode = 51 },
                new Country { Id = 169, Iso = "PH", Name = "Philippines", Iso3 = "PHL", Numcode = 608, PhoneCode = 63 },
                new Country { Id = 170, Iso = "PN", Name = "Pitcairn", Iso3 = "PCN", Numcode = 612, PhoneCode = 0 },
                new Country { Id = 171, Iso = "PL", Name = "Poland", Iso3 = "POL", Numcode = 616, PhoneCode = 48 },
                new Country { Id = 172, Iso = "PT", Name = "Portugal", Iso3 = "PRT", Numcode = 620, PhoneCode = 351 },
                new Country
                {
                    Id = 173,
                    Iso = "PR",
                    Name = "Puerto Rico",
                    Iso3 = "PRI",
                    Numcode = 630,
                    PhoneCode = 1787
                },
                new Country { Id = 174, Iso = "QA", Name = "Qatar", Iso3 = "QAT", Numcode = 634, PhoneCode = 974 },
                new Country { Id = 175, Iso = "RE", Name = "Reunion", Iso3 = "REU", Numcode = 638, PhoneCode = 262 },
                new Country { Id = 176, Iso = "RO", Name = "Romania", Iso3 = "ROM", Numcode = 642, PhoneCode = 40 },
                new Country
                {
                    Id = 177,
                    Iso = "RU",
                    Name = "Russian Federation",
                    Iso3 = "RUS",
                    Numcode = 643,
                    PhoneCode = 7
                },
                new Country { Id = 178, Iso = "RW", Name = "Rwanda", Iso3 = "RWA", Numcode = 646, PhoneCode = 250 },
                new Country
                {
                    Id = 179,
                    Iso = "SH",
                    Name = "Saint Helena",
                    Iso3 = "SHN",
                    Numcode = 654,
                    PhoneCode = 290
                },
                new Country
                {
                    Id = 180,
                    Iso = "KN",
                    Name = "Saint Kitts and Nevis",
                    Iso3 = "KNA",
                    Numcode = 659,
                    PhoneCode = 1869
                },
                new Country
                {
                    Id = 181,
                    Iso = "LC",
                    Name = "Saint Lucia",
                    Iso3 = "LCA",
                    Numcode = 662,
                    PhoneCode = 1758
                },
                new Country
                {
                    Id = 182,
                    Iso = "PM",
                    Name = "Saint Pierre and Miquelon",
                    Iso3 = "SPM",
                    Numcode = 666,
                    PhoneCode = 508
                },
                new Country
                {
                    Id = 183,
                    Iso = "VC",
                    Name = "Saint Vincent and the Grenadines",
                    Iso3 = "VCT",
                    Numcode = 670,
                    PhoneCode = 1784
                },
                new Country { Id = 184, Iso = "WS", Name = "Samoa", Iso3 = "WSM", Numcode = 882, PhoneCode = 685 },
                new Country { Id = 185, Iso = "SM", Name = "San Marino", Iso3 = "SMR", Numcode = 674, PhoneCode = 378 },
                new Country
                {
                    Id = 186,
                    Iso = "ST",
                    Name = "Sao Tome and Principe",
                    Iso3 = "STP",
                    Numcode = 678,
                    PhoneCode = 239
                },
                new Country
                {
                    Id = 187,
                    Iso = "SA",
                    Name = "Saudi Arabia",
                    Iso3 = "SAU",
                    Numcode = 682,
                    PhoneCode = 966
                },
                new Country { Id = 188, Iso = "SN", Name = "Senegal", Iso3 = "SEN", Numcode = 686, PhoneCode = 221 },
                new Country
                {
                    Id = 189,
                    Iso = "RS",
                    Name = "Serbia",
                    Iso3 = "SRB",
                    Numcode = -1,
                    PhoneCode = 381
                },
                new Country { Id = 190, Iso = "SC", Name = "Seychelles", Iso3 = "SYC", Numcode = 690, PhoneCode = 248 },
                new Country
                {
                    Id = 191,
                    Iso = "SL",
                    Name = "Sierra Leone",
                    Iso3 = "SLE",
                    Numcode = 694,
                    PhoneCode = 232
                },
                new Country { Id = 192, Iso = "SG", Name = "Singapore", Iso3 = "SGP", Numcode = 702, PhoneCode = 65 },
                new Country { Id = 193, Iso = "SK", Name = "Slovakia", Iso3 = "SVK", Numcode = 703, PhoneCode = 421 },
                new Country { Id = 194, Iso = "SI", Name = "Slovenia", Iso3 = "SVN", Numcode = 705, PhoneCode = 386 },
                new Country
                {
                    Id = 195,
                    Iso = "SB",
                    Name = "Solomon Islands",
                    Iso3 = "SLB",
                    Numcode = 90,
                    PhoneCode = 677
                },
                new Country { Id = 196, Iso = "SO", Name = "Somalia", Iso3 = "SOM", Numcode = 706, PhoneCode = 252 },
                new Country
                {
                    Id = 197,
                    Iso = "ZA",
                    Name = "South Africa",
                    Iso3 = "ZAF",
                    Numcode = 710,
                    PhoneCode = 27
                },
                new Country
                {
                    Id = 198,
                    Iso = "GS",
                    Name = "South Georgia and the South Sandwich Islands",
                    Iso3 = null,
                    Numcode = -1,
                    PhoneCode = 0
                },
                new Country { Id = 199, Iso = "ES", Name = "Spain", Iso3 = "ESP", Numcode = 724, PhoneCode = 34 },
                new Country { Id = 200, Iso = "LK", Name = "Sri Lanka", Iso3 = "LKA", Numcode = 144, PhoneCode = 94 },
                new Country { Id = 201, Iso = "SD", Name = "Sudan", Iso3 = "SDN", Numcode = 736, PhoneCode = 249 },
                new Country { Id = 202, Iso = "SR", Name = "Suriname", Iso3 = "SUR", Numcode = 740, PhoneCode = 597 },
                new Country
                {
                    Id = 203,
                    Iso = "SJ",
                    Name = "Svalbard and Jan Mayen",
                    Iso3 = "SJM",
                    Numcode = 744,
                    PhoneCode = 47
                },
                new Country { Id = 204, Iso = "SZ", Name = "Swaziland", Iso3 = "SWZ", Numcode = 748, PhoneCode = 268 },
                new Country { Id = 205, Iso = "SE", Name = "Sweden", Iso3 = "SWE", Numcode = 752, PhoneCode = 46 },
                new Country { Id = 206, Iso = "CH", Name = "Switzerland", Iso3 = "CHE", Numcode = 756, PhoneCode = 41 },
                new Country
                {
                    Id = 207,
                    Iso = "SY",
                    Name = "Syrian Arab Republic",
                    Iso3 = "SYR",
                    Numcode = 760,
                    PhoneCode = 963
                },
                new Country
                {
                    Id = 208,
                    Iso = "TW",
                    Name = "Taiwan, Province of China",
                    Iso3 = "TWN",
                    Numcode = 158,
                    PhoneCode = 886
                },
                new Country { Id = 209, Iso = "TJ", Name = "Tajikistan", Iso3 = "TJK", Numcode = 762, PhoneCode = 992 },
                new Country
                {
                    Id = 210,
                    Iso = "TZ",
                    Name = "Tanzania, United Republic of",
                    Iso3 = "TZA",
                    Numcode = 834,
                    PhoneCode = 255
                },
                new Country { Id = 211, Iso = "TH", Name = "Thailand", Iso3 = "THA", Numcode = 764, PhoneCode = 66 },
                new Country
                {
                    Id = 212,
                    Iso = "TL",
                    Name = "Timor-Leste",
                    Iso3 = "TLS",
                    Numcode = 626,
                    PhoneCode = 670
                },
                new Country { Id = 213, Iso = "TG", Name = "Togo", Iso3 = "TGO", Numcode = 768, PhoneCode = 228 },
                new Country { Id = 214, Iso = "TK", Name = "Tokelau", Iso3 = "TKL", Numcode = 772, PhoneCode = 690 },
                new Country { Id = 215, Iso = "TO", Name = "Tonga", Iso3 = "TON", Numcode = 776, PhoneCode = 676 },
                new Country
                {
                    Id = 216,
                    Iso = "TT",
                    Name = "Trinidad and Tobago",
                    Iso3 = "TTO",
                    Numcode = 780,
                    PhoneCode = 1868
                },
                new Country { Id = 217, Iso = "TN", Name = "Tunisia", Iso3 = "TUN", Numcode = 788, PhoneCode = 216 },
                new Country { Id = 218, Iso = "TR", Name = "Turkey", Iso3 = "TUR", Numcode = 792, PhoneCode = 90 },
                new Country
                {
                    Id = 219,
                    Iso = "TM",
                    Name = "Turkmenistan",
                    Iso3 = "TKM",
                    Numcode = 795,
                    PhoneCode = 7370
                },
                new Country
                {
                    Id = 220,
                    Iso = "TC",
                    Name = "Turks and Caicos Islands",
                    Iso3 = "TCA",
                    Numcode = 796,
                    PhoneCode = 1649
                },
                new Country { Id = 221, Iso = "TV", Name = "Tuvalu", Iso3 = "TUV", Numcode = 798, PhoneCode = 688 },
                new Country { Id = 222, Iso = "UG", Name = "Uganda", Iso3 = "UGA", Numcode = 800, PhoneCode = 256 },
                new Country { Id = 223, Iso = "UA", Name = "Ukraine", Iso3 = "UKR", Numcode = 804, PhoneCode = 380 },
                new Country
                {
                    Id = 224,
                    Iso = "AE",
                    Name = "United Arab Emirates",
                    Iso3 = "ARE",
                    Numcode = 784,
                    PhoneCode = 971
                },
                new Country
                {
                    Id = 225,
                    Iso = "GB",
                    Name = "United Kingdom",
                    Iso3 = "GBR",
                    Numcode = 826,
                    PhoneCode = 44
                },
                new Country
                {
                    Id = 226,
                    Iso = "US",
                    Name = "United States",
                    Iso3 = "USA",
                    Numcode = 840,
                    PhoneCode = 1
                },
                new Country
                {
                    Id = 227,
                    Iso = "UM",
                    Name = "United States Minor Outlying Islands",
                    Iso3 = null,
                    Numcode = -1,
                    PhoneCode = 0
                },
                new Country { Id = 228, Iso = "UY", Name = "Uruguay", Iso3 = "URY", Numcode = 858, PhoneCode = 598 },
                new Country { Id = 229, Iso = "UZ", Name = "Uzbekistan", Iso3 = "UZB", Numcode = 860, PhoneCode = 998 },
                new Country { Id = 230, Iso = "VU", Name = "Vanuatu", Iso3 = "VUT", Numcode = 548, PhoneCode = 678 },
                new Country { Id = 231, Iso = "VE", Name = "Venezuela", Iso3 = "VEN", Numcode = 862, PhoneCode = 58 },
                new Country { Id = 232, Iso = "VN", Name = "Viet Nam", Iso3 = "VNM", Numcode = 704, PhoneCode = 84 },
                new Country
                {
                    Id = 233,
                    Iso = "VG",
                    Name = "Virgin Islands, British",
                    Iso3 = "VGB",
                    Numcode = 92,
                    PhoneCode = 1284
                },
                new Country
                {
                    Id = 234,
                    Iso = "VI",
                    Name = "Virgin Islands, U.s.",
                    Iso3 = "VIR",
                    Numcode = 850,
                    PhoneCode = 1340
                },
                new Country
                {
                    Id = 235,
                    Iso = "WF",
                    Name = "Wallis and Futuna",
                    Iso3 = "WLF",
                    Numcode = 876,
                    PhoneCode = 681
                },
                new Country
                {
                    Id = 236,
                    Iso = "EH",
                    Name = "Western Sahara",
                    Iso3 = "ESH",
                    Numcode = 732,
                    PhoneCode = 0
                },
                new Country { Id = 237, Iso = "YE", Name = "Yemen", Iso3 = "YEM", Numcode = 887, PhoneCode = 967 },
                new Country { Id = 238, Iso = "ZM", Name = "Zambia", Iso3 = "ZMB", Numcode = 894, PhoneCode = 260 },
                new Country { Id = 239, Iso = "ZW", Name = "Zimbabwe", Iso3 = "ZWE", Numcode = 716, PhoneCode = 263 },
                new Country
                {
                    Id = 240,
                    Iso = "ME",
                    Name = "Montenegro",
                    Iso3 = "MNE",
                    Numcode = -1,
                    PhoneCode = 382
                });
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // get updated entries
            var updatedConcurrencyAwareEntries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Modified)
                .OfType<IConcurrencyAware>();

            foreach (var entry in updatedConcurrencyAwareEntries)
            {
                entry.ConcurrencyStamp = Guid.NewGuid().ToString();
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
