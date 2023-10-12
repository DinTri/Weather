namespace Weather.Api.Models
{
    public class Day
    {
        public double maxtemp_c { get; set; }
        public double mintemp_c { get; set; }
        public double maxtemp_f { get; set; }
        public double mintemp_f { get; set; }
        public double maxwind_mph { get; set; }
        public double maxwind_kph { get; set; }
        public double uv { get; set; }
        public Condition? condition { get; set; }
    }
}