using System.ComponentModel.DataAnnotations;

namespace Weather.Web.Administration
{
    public class Country
    {
        [Key]
        public int Id { get; set; }

        [StringLength(2)]
        public string? Iso { get; set; }

        [StringLength(3)]
        public string? Iso3 { get; set; }


        [StringLength(80)]
        [Required]
        public string? Name { get; set; }

        public int Numcode { get; internal set; }
        public int PhoneCode { get; set; }
    }

}
