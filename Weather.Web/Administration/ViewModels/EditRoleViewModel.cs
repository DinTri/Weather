using System.ComponentModel.DataAnnotations;

namespace Weather.Web.Administration.ViewModels
{
    public class EditRoleViewModel
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Role Name is required")]
        public string Name { get; set; } = string.Empty;

        public List<string> Users { get; set; } = new();

    }
}
