using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Weather.Api.Models;
using Weather.Api.Services;

namespace Weather.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherForecastService _weatherService;
        private readonly ILogger<WeatherController> _logger;
        public WeatherController(IWeatherForecastService weatherService, ILogger<WeatherController> logger)
        {
            _weatherService = weatherService ?? throw new ArgumentNullException(nameof(weatherService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("weather={city}")]
        //[Authorize(Roles = "Admin")]
        // [Authorize(Policy = "UserCanSeeWeather")]
        public async Task<ActionResult<List<WeatherResponse>>> GetWeatherForecast(string city)
        {
            try
            {

                var weatherData = await _weatherService.GetWeatherForecastsAsync(city);
                return Ok(weatherData);
            }
            catch (HttpRequestException ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred while fetching weather data.");

                return StatusCode(500, "An error occurred while fetching weather data.");
            }
        }
    }
}
