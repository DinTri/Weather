
namespace Weather.Web.Models
{
    public class WeatherResponse
    {
        public Location? location { get; set; }
        public Current? current { get; set; }
        public Forecast? forecast { get; set; }
    }
}
