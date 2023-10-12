namespace Weather.Web.Models
{
    public class Forecastday
    {
        public string? date { get; set; }
        public Day? day { get; set; }
        public Astro? astro { get; set; }
        public List<Hour>? hour { get; set; }
    }
}