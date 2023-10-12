using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Weather.Api.Configurations;
using Weather.Api.Models;

namespace Weather.Api.Services
{
    public class WeatherForecastService : IWeatherForecastService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<WeatherForecastService> _logger;
        private readonly string? _apiKey;

        public WeatherForecastService(HttpClient httpClient, ILogger<WeatherForecastService> logger, IOptions<WeatherApiSettings> weatherApiSettings)
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiKey = weatherApiSettings.Value.ApiKey;
        }
        private WeatherResponse DeserializeForecastData(string jsonData)
        {
            try
            {
                var weatherApiResponse = JsonConvert.DeserializeObject<WeatherResponse>(jsonData);

                if (weatherApiResponse != null && IsValidForecast(weatherApiResponse.forecast))
                {
                    return weatherApiResponse;
                }

                if (weatherApiResponse != null)
                    return new WeatherResponse
                    {
                        location = weatherApiResponse.location,
                        current = weatherApiResponse.current,
                        forecast = new Forecast { Forecastday = new List<Forecastday>() }
                    };
                return new WeatherResponse();
            }
            catch (JsonSerializationException)
            {
                var root = JsonConvert.DeserializeObject<WeatherResponse>(jsonData);
                if (root != null) return root;
            }

            return new WeatherResponse();
        }

        private static bool IsValidForecast(Forecast? forecast)
        {
            const int zeroThreshold = 10;
            if (forecast != null)
            {
                int zeroCount = forecast.Forecastday.Count(forecastday => forecastday.day is { maxtemp_c: 0 });
                return zeroCount < zeroThreshold;
            }

            return false;
        }
        public async Task<List<WeatherResponse>> GetWeatherForecastsAsync(string city)
        {

            try
            {
                var response = await _httpClient.GetAsync($"https://api.weatherapi.com/v1/forecast.json?key={_apiKey}&q={city}&days={7}&aqi=no&alerts=no");
                response.EnsureSuccessStatusCode();
                var jsonData = await response.Content.ReadAsStringAsync();
                WeatherResponse weatherApiResponce = DeserializeForecastData(jsonData);

                var weather = new WeatherResponse
                {
                    current = weatherApiResponce.current,
                    location = weatherApiResponce.location,
                    forecast = weatherApiResponce.forecast
                };

                return new List<WeatherResponse> { weather };
            }
            catch (HttpRequestException ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred while fetching weather data.");

                // Rethrow the exception
                throw;
            }
        }
    }
}
