using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Trifon.IDP.Pages.User.Registration
{
    public class InputModel
    {
        public string ReturnUrl { get; set; }

        [MaxLength(200)]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [MaxLength(200)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [MaxLength(250)]
        [Display(Name = "Given name")]
        public string GivenName { get; set; }

        [Required]
        [MaxLength(250)]
        [Display(Name = "Family name")]
        public string FamilyName { get; set; }

        [Required]
        [MaxLength(2)]
        [Display(Name = "Country")]
        public string Country { get; set; }

        [Required]
        [MaxLength(200)]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public List<SelectListItem> CountryCodes { get; set; }
        public List<string> ChosenCountryIsoCodes { get; set; } = new List<string> { "US", "UK", "CA", "BG", "DE", "FR", "ES", "IT", "TR" };

    }


}
