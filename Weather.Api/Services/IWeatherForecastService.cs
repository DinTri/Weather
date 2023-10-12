using Weather.Api.Models;

namespace Weather.Api.Services
{
    public interface IWeatherForecastService
    {
        Task<List<WeatherResponse>> GetWeatherForecastsAsync(string city);
    }
}
