using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using Weather.CustomExceptions;
using Weather.Web.Models;

namespace Weather.Web.Controllers
{
    [Authorize]
    public class WeatherController : Controller
    {
        private readonly ILogger<WeatherController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public WeatherController(IHttpClientFactory httpClientFactory, ILogger<WeatherController> logger, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger;
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            await LogIdentityInformation();
            return View();
        }

        [HttpGet]
        [Authorize(Policy = "UserCanSeeWeather")]
        public async Task<IActionResult> GetWeather(string city)
        {
            if (string.IsNullOrEmpty(city))
            {
                // Return the view without making the API call since the city is null
                return View("GetWeather");
            }


            try
            {
                var season = GetSeasonImage();
                string? seasonImage = GetImageUrlForSeason(season);

                // Set the selected image URL in TempData
                TempData["SeasonImage"] = seasonImage;
                var weatherResponse = await CallWeatherServiceAsync(city); // Pass city parameter
                TempData["WeatherData"] = JsonConvert.SerializeObject(weatherResponse);

                var ownerId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
                if (string.IsNullOrEmpty(ownerId))
                {
                    throw new InvalidOperationException("User identifier is missing from token.");
                }

                return View("GetWeather");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetWeatherData action.");
                ModelState.AddModelError("An error occurred.", ex.Message);
                return View("GetWeather");
            }
        }
        private async Task<List<WeatherResponse>> CallWeatherServiceAsync(string city)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var httpClient = _httpClientFactory.CreateClient("APIClient");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var apiBaseUrl = _configuration["WeatherAPIRoot"];
            var apiUrl = $"{apiBaseUrl}/api/weather/weather={city}";
            var response = await httpClient.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
            {
                // Handle the error response and throw a custom exception
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Weather API Error: {ErrorContent}", errorContent);

                // Customize the error message and status code based on the specific error
                var errorMessage = "An error occurred while fetching weather data. Please try again later.";

                throw new WeatherException(errorMessage, (int)response.StatusCode);
            }

            var content = await response.Content.ReadAsStringAsync();
            var weatherForecasts = JsonConvert.DeserializeObject<List<WeatherResponse>>(content);

            if (weatherForecasts != null)
            {
                return weatherForecasts;
            }

            // Handle the case when weatherForecasts is null
            // Log the issue and throw a custom exception
            _logger.LogError("Weather forecasts are null.");
            throw new WeatherException("Weather forecasts are null.", StatusCodes.Status500InternalServerError);
        }

        private string? GetSeasonImage()
        {
            // Get the current date
            var currentDate = DateTime.Now;
            var dateWithZeroTime = currentDate.Date;
            // Define the start dates and end dates for each season
            DateTime springStart = DateTime.SpecifyKind(new DateTime(currentDate.Year, 3, 22), DateTimeKind.Local);
            DateTime summerStart = DateTime.SpecifyKind(new DateTime(currentDate.Year, 6, 22), DateTimeKind.Local);
            DateTime fallStart = DateTime.SpecifyKind(new DateTime(currentDate.Year, 9, 22), DateTimeKind.Local);
            DateTime winterStart = DateTime.SpecifyKind(new DateTime(currentDate.Year, 12, 22), DateTimeKind.Local);

            // Determine the current season
            string? seasonImage = "";

            if (dateWithZeroTime >= springStart && currentDate < summerStart)
            {
                seasonImage = "Picture 2"; // Spring
            }
            else if (dateWithZeroTime >= summerStart && currentDate < fallStart)
            {
                seasonImage = "Picture 3"; // Summer
            }
            else if (dateWithZeroTime >= fallStart && currentDate < winterStart)
            {
                seasonImage = "Picture 4"; // Fall
            }
            else
            {
                seasonImage = "Picture 1"; // Winter
            }

            return seasonImage;
        }
        private string? GetImageUrlForSeason(string? seasonImage)
        {
            // Define a mapping of seasonImage values to image URLs
            Dictionary<string, string?> seasonImageMapping = new Dictionary<string, string?>
            {
                { "Picture 1", "~/images/winter.jpg" },
                { "Picture 2", "~/images/spring.jpg" },
                { "Picture 3", "~/images/summer.jpg" },
                { "Picture 4", "~/images/autumn.jpg" }
            };

            // Check if seasonImage exists in the mapping, and return the corresponding image URL
            return seasonImage != null && seasonImageMapping.TryGetValue(seasonImage, out string? imageUrl) ? imageUrl :
                // Default to a fallback image URL if the seasonImage is not recognized
                "~/images/doggy.jpg"; // Replace with your fallback image URL
        }
        public async Task LogIdentityInformation()
        {
            // get the saved identity token
            var identityToken = await HttpContext
                .GetTokenAsync(OpenIdConnectParameterNames.IdToken);

            // get the saved access token
            var accessToken = await HttpContext
                .GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            // get the refresh token
            var refreshToken = await HttpContext
                .GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
            var userClaimsStringBuilder = new StringBuilder();
            foreach (var claim in User.Claims)
            {
                userClaimsStringBuilder.AppendLine($"Claim type: {claim.Type} - Claim value: {claim.Value}");
            }

            _logger.LogInformation($"Identity token & user claims: " +
                                   $"\n{identityToken} \n{userClaimsStringBuilder}");
            _logger.LogInformation($"Access token: " +
                                   $"\n{accessToken}");
            _logger.LogInformation($"Refresh token: " +
                                   $"\n{refreshToken}");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}