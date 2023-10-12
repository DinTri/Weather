using System.ComponentModel.DataAnnotations;

namespace Weather.Web.Administration.ViewModels
{
    public class CreateRoleViewModel
    {
        [Required]
        [Display(Name = "Role")]
        [StringLength(25)]
        public string Name { get; set; }
        public string SuccessMessage { get; set; } = string.Empty;
    }
}
