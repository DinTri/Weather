using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Weather.Api.IdentityRoleServices;

namespace Weather.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CountriesController : ControllerBase
    {
        private readonly IUserService _userService;

        private readonly ILogger<RolesController> _logger;

        public CountriesController(IUserService userService, ILogger<RolesController> logger)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCountries()
        {
            var countries = await _userService.GetAllCountriesAsync();
            return Ok(countries);
        }

        [HttpGet("{countryId}")]
        public async Task<IActionResult> GetCountryById(int countryId)
        {
            var country = await _userService.GetCountryByIdAsync(countryId);
            if (country != null)
            {
                return Ok(country);
            }
            return NotFound($"Country with ID {countryId} not found.");
        }

        [HttpGet("country/{userId}")]
        public async Task<IActionResult> GetCountryByUserIdAsync(string userId)
        {
            try
            {
                var country = await _userService.GetCountryByUserIdAsync(userId);

                _logger.LogInformation($"Country retrieved for userId: {userId}");
                return Ok(country);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving country for userId: {userId}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
