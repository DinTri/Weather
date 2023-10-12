namespace Weather.Web.Models
{
    public class Forecast
    {
        public IReadOnlyCollection<Forecastday> Forecastday { get; init; } = new List<Forecastday>();
    }
}
