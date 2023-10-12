﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Trifon.IDP.Migrations
{
    /// <inheritdoc />
    public partial class dbinit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Iso = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    Iso3 = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Numcode = table.Column<int>(type: "int", nullable: false),
                    PhoneCode = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    GivenName = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    FamilyName = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    Suspended = table.Column<bool>(type: "bit", nullable: false),
                    ProfilePhotoPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SecurityCode = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SecurityCodeExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSecrets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Secret = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSecrets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSecrets_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "Iso", "Iso3", "Name", "Numcode", "PhoneCode" },
                values: new object[,]
                {
                    { 1, "AF", "AFG", "Afghanistan", 4, 93 },
                    { 2, "AL", "ALB", "Albania", 8, 355 },
                    { 3, "DZ", "DZA", "Algeria", 12, 213 },
                    { 4, "AS", "ASM", "American Samoa", 16, 1684 },
                    { 5, "AD", "AND", "Andorra", 20, 376 },
                    { 6, "AO", "AGO", "Angola", 24, 244 },
                    { 7, "AI", "AIA", "Anguilla", 660, 1264 },
                    { 8, "AQ", null, "Antarctica", -1, 0 },
                    { 9, "AG", "ATG", "Antigua and Barbuda", 28, 1268 },
                    { 10, "AR", "ARG", "Argentina", 32, 54 },
                    { 11, "AM", "ARM", "Armenia", 51, 374 },
                    { 12, "AW", "ABW", "Aruba", 533, 297 },
                    { 13, "AU", "AUS", "Australia", 36, 61 },
                    { 14, "AT", "AUT", "Austria", 40, 43 },
                    { 15, "AZ", "AZE", "Azerbaijan", 31, 994 },
                    { 16, "BS", "BHS", "Bahamas", 44, 1242 },
                    { 17, "BH", "BHR", "Bahrain", 48, 973 },
                    { 18, "BD", "BGD", "Bangladesh", 50, 880 },
                    { 19, "BB", "BRB", "Barbados", 52, 1246 },
                    { 20, "BY", "BLR", "Belarus", 112, 375 },
                    { 21, "BE", "BEL", "Belgium", 56, 32 },
                    { 22, "BZ", "BLZ", "Belize", 84, 501 },
                    { 23, "BJ", "BEN", "Benin", 204, 229 },
                    { 24, "BM", "BMU", "Bermuda", 60, 1441 },
                    { 25, "BT", "BTN", "Bhutan", 64, 975 },
                    { 26, "BO", "BOL", "Bolivia", 68, 591 },
                    { 27, "BA", "BIH", "Bosnia and Herzegovina", 70, 387 },
                    { 28, "BW", "BWA", "Botswana", 72, 267 },
                    { 29, "BV", null, "Bouvet Island", -1, 0 },
                    { 30, "BR", "BRA", "Brazil", 76, 55 },
                    { 31, "IO", null, "British Indian Ocean Territory", -1, 246 },
                    { 32, "BN", "BRN", "Brunei Darussalam", 96, 673 },
                    { 33, "BG", "BGR", "Bulgaria", 100, 359 },
                    { 34, "BF", "BFA", "Burkina Faso", 854, 226 },
                    { 35, "BI", "BDI", "Burundi", 108, 257 },
                    { 36, "KH", "KHM", "Cambodia", 116, 855 },
                    { 37, "CM", "CMR", "Cameroon", 120, 237 },
                    { 38, "CA", "CAN", "Canada", 124, 1 },
                    { 39, "CV", "CPV", "Cape Verde", 132, 238 },
                    { 40, "KY", "CYM", "Cayman Islands", 136, 1345 },
                    { 41, "CF", "CAF", "Central African Republic", 140, 236 },
                    { 42, "TD", "TCD", "Chad", 148, 235 },
                    { 43, "CL", "CHL", "Chile", 152, 56 },
                    { 44, "CN", "CHN", "China", 156, 86 },
                    { 45, "CX", null, "Christmas Island", -1, 61 },
                    { 46, "CC", null, "Cocos (Keeling) Islands", -1, 672 },
                    { 47, "CO", "COL", "Colombia", 170, 57 },
                    { 48, "KM", "COM", "Comoros", 174, 269 },
                    { 49, "CG", "COG", "Congo", 178, 242 },
                    { 50, "CD", "COD", "Congo, the Democratic Republic of the", 180, 242 },
                    { 51, "CK", "COK", "Cook Islands", 184, 682 },
                    { 52, "CR", "CRI", "Costa Rica", 188, 506 },
                    { 53, "CI", "CIV", "Cote D'Ivoire", 384, 225 },
                    { 54, "HR", "HRV", "Croatia", 191, 385 },
                    { 55, "CU", "CUB", "Cuba", 192, 53 },
                    { 56, "CY", "CYP", "Cyprus", 196, 357 },
                    { 57, "CZ", "CZE", "Czech Republic", 203, 420 },
                    { 58, "DK", "DNK", "Denmark", 208, 45 },
                    { 59, "DJ", "DJI", "Djibouti", 262, 253 },
                    { 60, "DM", "DMA", "Dominica", 212, 1767 },
                    { 61, "DO", "DOM", "Dominican Republic", 214, 1809 },
                    { 62, "EC", "ECU", "Ecuador", 218, 593 },
                    { 63, "EG", "EGY", "Egypt", 818, 20 },
                    { 64, "SV", "SLV", "El Salvador", 222, 503 },
                    { 65, "GQ", "GNQ", "Equatorial Guinea", 226, 240 },
                    { 66, "ER", "ERI", "Eritrea", 232, 291 },
                    { 67, "EE", "EST", "Estonia", 233, 372 },
                    { 68, "ET", "ETH", "Ethiopia", 231, 251 },
                    { 69, "FK", "FLK", "Falkland Islands (Malvinas)", 238, 500 },
                    { 70, "FO", "FRO", "Faroe Islands", 234, 298 },
                    { 71, "FJ", "FJI", "Fiji", 242, 679 },
                    { 72, "FI", "FIN", "Finland", 246, 358 },
                    { 73, "FR", "FRA", "France", 250, 33 },
                    { 74, "GF", "GUF", "French Guiana", 254, 594 },
                    { 75, "PF", "PYF", "French Polynesia", 258, 689 },
                    { 76, "TF", null, "French Southern Territories", -1, 0 },
                    { 77, "GA", "GAB", "Gabon", 266, 241 },
                    { 78, "GM", "GMB", "Gambia", 270, 220 },
                    { 79, "GE", "GEO", "Georgia", 268, 995 },
                    { 80, "DE", "DEU", "Germany", 276, 49 },
                    { 81, "GH", "GHA", "Ghana", 288, 233 },
                    { 82, "GI", "GIB", "Gibraltar", 292, 350 },
                    { 83, "GR", "GRC", "Greece", 300, 30 },
                    { 84, "GL", "GRL", "Greenland", 304, 299 },
                    { 85, "GD", "GRD", "Grenada", 308, 1473 },
                    { 86, "GP", "GLP", "Guadeloupe", 312, 590 },
                    { 87, "GU", "GUM", "Guam", 316, 1671 },
                    { 88, "GT", "GTM", "Guatemala", 320, 502 },
                    { 89, "GN", "GIN", "Guinea", 324, 224 },
                    { 90, "GW", "GNB", "Guinea-Bissau", 624, 245 },
                    { 91, "GY", "GUY", "Guyana", 328, 592 },
                    { 92, "HT", "HTI", "Haiti", 332, 509 },
                    { 93, "HM", null, "Heard Island and Mcdonald Islands", -1, 0 },
                    { 94, "VA", "VAT", "Holy See (Vatican City State)", 336, 39 },
                    { 95, "HN", "HND", "Honduras", 340, 504 },
                    { 96, "HK", "HKG", "Hong Kong", 344, 852 },
                    { 97, "HU", "HUN", "Hungary", 348, 36 },
                    { 98, "IS", "ISL", "Iceland", 352, 354 },
                    { 99, "IN", "IND", "India", 356, 91 },
                    { 100, "ID", "IDN", "Indonesia", 360, 62 },
                    { 101, "IR", "IRN", "Iran, Islamic Republic of", 364, 98 },
                    { 102, "IQ", "IRQ", "Iraq", 368, 964 },
                    { 103, "IE", "IRL", "Ireland", 372, 353 },
                    { 104, "IL", "ISR", "Israel", 376, 972 },
                    { 105, "IT", "ITA", "Italy", 380, 39 },
                    { 106, "JM", "JAM", "Jamaica", 388, 1876 },
                    { 107, "JP", "JPN", "Japan", 392, 81 },
                    { 108, "JO", "JOR", "Jordan", 400, 962 },
                    { 109, "KZ", "KAZ", "Kazakhstan", 398, 7 },
                    { 110, "KE", "KEN", "Kenya", 404, 254 },
                    { 111, "KI", "KIR", "Kiribati", 296, 686 },
                    { 112, "KP", "PRK", "Korea, Democratic People's Republic of", 408, 850 },
                    { 113, "KR", "KOR", "Korea, Republic of", 410, 82 },
                    { 114, "KW", "KWT", "Kuwait", 414, 965 },
                    { 115, "KG", "KGZ", "Kyrgyzstan", 417, 996 },
                    { 116, "LA", "LAO", "Lao People's Democratic Republic", 418, 856 },
                    { 117, "LV", "LVA", "Latvia", 428, 371 },
                    { 118, "LB", "LBN", "Lebanon", 422, 961 },
                    { 119, "LS", "LSO", "Lesotho", 426, 266 },
                    { 120, "LR", "LBR", "Liberia", 430, 231 },
                    { 121, "LY", "LBY", "Libyan Arab Jamahiriya", 434, 218 },
                    { 122, "LI", "LIE", "Liechtenstein", 438, 423 },
                    { 123, "LT", "LTU", "Lithuania", 440, 370 },
                    { 124, "LU", "LUX", "Luxembourg", 442, 352 },
                    { 125, "MO", "MAC", "Macao", 446, 853 },
                    { 126, "MK", "MKD", "Macedonia, the Former Yugoslav Republic of", 807, 389 },
                    { 127, "MG", "MDG", "Madagascar", 450, 261 },
                    { 128, "MW", "MWI", "Malawi", 454, 265 },
                    { 129, "MY", "MYS", "Malaysia", 458, 60 },
                    { 130, "MV", "MDV", "Maldives", 462, 960 },
                    { 131, "ML", "MLI", "Mali", 466, 223 },
                    { 132, "MT", "MLT", "Malta", 470, 356 },
                    { 133, "MH", "MHL", "Marshall Islands", 584, 692 },
                    { 134, "MQ", "MTQ", "Martinique", 474, 596 },
                    { 135, "MR", "MRT", "Mauritania", 478, 222 },
                    { 136, "MU", "MUS", "Mauritius", 480, 230 },
                    { 137, "YT", "MYT", "Mayotte", 1758, 262 },
                    { 138, "MX", "MEX", "Mexico", 484, 52 },
                    { 139, "FM", "FSM", "Micronesia, Federated States of", 583, 691 },
                    { 140, "MD", "MDA", "Moldova, Republic of", 498, 373 },
                    { 141, "MC", "MCO", "Monaco", 492, 377 },
                    { 142, "MN", "MNG", "Mongolia", 496, 976 },
                    { 143, "MS", "MSR", "Montserrat", 500, 1664 },
                    { 144, "MA", "MAR", "Morocco", 504, 212 },
                    { 145, "MZ", "MOZ", "Mozambique", 508, 258 },
                    { 146, "MM", "MMR", "Myanmar", 104, 95 },
                    { 147, "NA", "NAM", "Namibia", 516, 264 },
                    { 148, "NR", "NRU", "Nauru", 520, 674 },
                    { 149, "NP", "NPL", "Nepal", 524, 977 },
                    { 150, "NL", "NLD", "Netherlands", 528, 31 },
                    { 151, "AN", "ANT", "Netherlands Antilles", 530, 599 },
                    { 152, "NC", "NCL", "New Caledonia", 540, 687 },
                    { 153, "NZ", "NZL", "New Zealand", 554, 64 },
                    { 154, "NI", "NIC", "Nicaragua", 558, 505 },
                    { 155, "NE", "NER", "Niger", 562, 227 },
                    { 156, "NG", "NGA", "Nigeria", 566, 234 },
                    { 157, "NU", "NIU", "Niue", 570, 683 },
                    { 158, "NF", "NFK", "Norfolk Island", 574, 672 },
                    { 159, "MP", "MNP", "Northern Mariana Islands", 580, 1670 },
                    { 160, "NO", "NOR", "Norway", 578, 47 },
                    { 161, "OM", "OMN", "Oman", 512, 968 },
                    { 162, "PK", "PAK", "Pakistan", 586, 92 },
                    { 163, "PW", "PLW", "Palau", 585, 680 },
                    { 164, "PS", "PSE", "Palestinian Territory, Occupied", 275, 970 },
                    { 165, "PA", "PAN", "Panama", 591, 507 },
                    { 166, "PG", "PNG", "Papua New Guinea", 598, 675 },
                    { 167, "PY", "PRY", "Paraguay", 600, 595 },
                    { 168, "PE", "PER", "Peru", 604, 51 },
                    { 169, "PH", "PHL", "Philippines", 608, 63 },
                    { 170, "PN", "PCN", "Pitcairn", 612, 0 },
                    { 171, "PL", "POL", "Poland", 616, 48 },
                    { 172, "PT", "PRT", "Portugal", 620, 351 },
                    { 173, "PR", "PRI", "Puerto Rico", 630, 1787 },
                    { 174, "QA", "QAT", "Qatar", 634, 974 },
                    { 175, "RE", "REU", "Reunion", 638, 262 },
                    { 176, "RO", "ROM", "Romania", 642, 40 },
                    { 177, "RU", "RUS", "Russian Federation", 643, 7 },
                    { 178, "RW", "RWA", "Rwanda", 646, 250 },
                    { 179, "SH", "SHN", "Saint Helena", 654, 290 },
                    { 180, "KN", "KNA", "Saint Kitts and Nevis", 659, 1869 },
                    { 181, "LC", "LCA", "Saint Lucia", 662, 1758 },
                    { 182, "PM", "SPM", "Saint Pierre and Miquelon", 666, 508 },
                    { 183, "VC", "VCT", "Saint Vincent and the Grenadines", 670, 1784 },
                    { 184, "WS", "WSM", "Samoa", 882, 685 },
                    { 185, "SM", "SMR", "San Marino", 674, 378 },
                    { 186, "ST", "STP", "Sao Tome and Principe", 678, 239 },
                    { 187, "SA", "SAU", "Saudi Arabia", 682, 966 },
                    { 188, "SN", "SEN", "Senegal", 686, 221 },
                    { 189, "RS", "SRB", "Serbia", -1, 381 },
                    { 190, "SC", "SYC", "Seychelles", 690, 248 },
                    { 191, "SL", "SLE", "Sierra Leone", 694, 232 },
                    { 192, "SG", "SGP", "Singapore", 702, 65 },
                    { 193, "SK", "SVK", "Slovakia", 703, 421 },
                    { 194, "SI", "SVN", "Slovenia", 705, 386 },
                    { 195, "SB", "SLB", "Solomon Islands", 90, 677 },
                    { 196, "SO", "SOM", "Somalia", 706, 252 },
                    { 197, "ZA", "ZAF", "South Africa", 710, 27 },
                    { 198, "GS", null, "South Georgia and the South Sandwich Islands", -1, 0 },
                    { 199, "ES", "ESP", "Spain", 724, 34 },
                    { 200, "LK", "LKA", "Sri Lanka", 144, 94 },
                    { 201, "SD", "SDN", "Sudan", 736, 249 },
                    { 202, "SR", "SUR", "Suriname", 740, 597 },
                    { 203, "SJ", "SJM", "Svalbard and Jan Mayen", 744, 47 },
                    { 204, "SZ", "SWZ", "Swaziland", 748, 268 },
                    { 205, "SE", "SWE", "Sweden", 752, 46 },
                    { 206, "CH", "CHE", "Switzerland", 756, 41 },
                    { 207, "SY", "SYR", "Syrian Arab Republic", 760, 963 },
                    { 208, "TW", "TWN", "Taiwan, Province of China", 158, 886 },
                    { 209, "TJ", "TJK", "Tajikistan", 762, 992 },
                    { 210, "TZ", "TZA", "Tanzania, United Republic of", 834, 255 },
                    { 211, "TH", "THA", "Thailand", 764, 66 },
                    { 212, "TL", "TLS", "Timor-Leste", 626, 670 },
                    { 213, "TG", "TGO", "Togo", 768, 228 },
                    { 214, "TK", "TKL", "Tokelau", 772, 690 },
                    { 215, "TO", "TON", "Tonga", 776, 676 },
                    { 216, "TT", "TTO", "Trinidad and Tobago", 780, 1868 },
                    { 217, "TN", "TUN", "Tunisia", 788, 216 },
                    { 218, "TR", "TUR", "Turkey", 792, 90 },
                    { 219, "TM", "TKM", "Turkmenistan", 795, 7370 },
                    { 220, "TC", "TCA", "Turks and Caicos Islands", 796, 1649 },
                    { 221, "TV", "TUV", "Tuvalu", 798, 688 },
                    { 222, "UG", "UGA", "Uganda", 800, 256 },
                    { 223, "UA", "UKR", "Ukraine", 804, 380 },
                    { 224, "AE", "ARE", "United Arab Emirates", 784, 971 },
                    { 225, "GB", "GBR", "United Kingdom", 826, 44 },
                    { 226, "US", "USA", "United States", 840, 1 },
                    { 227, "UM", null, "United States Minor Outlying Islands", -1, 0 },
                    { 228, "UY", "URY", "Uruguay", 858, 598 },
                    { 229, "UZ", "UZB", "Uzbekistan", 860, 998 },
                    { 230, "VU", "VUT", "Vanuatu", 548, 678 },
                    { 231, "VE", "VEN", "Venezuela", 862, 58 },
                    { 232, "VN", "VNM", "Viet Nam", 704, 84 },
                    { 233, "VG", "VGB", "Virgin Islands, British", 92, 1284 },
                    { 234, "VI", "VIR", "Virgin Islands, U.s.", 850, 1340 },
                    { 235, "WF", "WLF", "Wallis and Futuna", 876, 681 },
                    { 236, "EH", "ESH", "Western Sahara", 732, 0 },
                    { 237, "YE", "YEM", "Yemen", 887, 967 },
                    { 238, "ZM", "ZMB", "Zambia", 894, 260 },
                    { 239, "ZW", "ZWE", "Zimbabwe", 716, 263 },
                    { 240, "ME", "MNE", "Montenegro", -1, 382 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_ApplicationUserId",
                table: "AspNetUserLogins",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CountryId",
                table: "AspNetUsers",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Email",
                table: "AspNetUsers",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Subject",
                table: "AspNetUsers",
                column: "Subject",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UserName",
                table: "AspNetUsers",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserSecrets_UserId",
                table: "UserSecrets",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "UserSecrets");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
