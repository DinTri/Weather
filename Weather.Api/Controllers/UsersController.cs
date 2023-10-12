using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Weather.Api.Entities;
using Weather.Api.Entities.Models;
using Weather.Api.IdentityRoleServices;

namespace Weather.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] AspNetUsers user, string password, IFormFile photo)
        {
            var result = await _userService.CreateUserAsync(user, password, photo);
            if (result)
            {
                return Ok("User created successfully.");
            }
            return BadRequest("User creation failed.");
        }

        [HttpPut("update/{userId}")]
        public async Task<IActionResult> UpdateUser(string userId, [FromForm] UserUpdateModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Retrieve the user by ID from your data store
                var user = await _userService.FindUserByIdAsync(userId);

                if (user == null)
                {
                    return NotFound($"User with ID {userId} not found.");
                }

                // Call the service method to update the user's profile with the provided data
                var result = await _userService.UpdateUserAsync(
                    user,
                    model.Photo,
                    model.UserName,
                    model.FamilyName,
                    model.Email,
                    model.PhoneNumber,
                    model.Suspended,
                    model.TwoFactorEnabled,
                    model.LockoutEnabled
                    );

                if (result)
                {
                    return Ok("User updated successfully.");
                }

                return BadRequest("User update failed.");
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while updating the user profile. Please try again.");
            }
        }

        [HttpPost("UploadProfilePhoto/{userId}")]
        public async Task<IActionResult> UploadProfilePhoto(string userId, [FromForm] IFormFile? photo)
        {
            try
            {
                var isUploadSuccessful = await _userService.UploadProfilePhotoAsync(userId, photo);

                if (isUploadSuccessful)
                {
                    return Ok(new { Message = "Profile photo has been updated." });
                }

                return BadRequest(new { ErrorMessage = "Failed to upload profile photo." });
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error uploading profile photo for user '{userId}': {ex.Message}");
                return StatusCode(500, new { ErrorMessage = "An error occurred while uploading profile photo." });
            }
        }


        [HttpDelete("delete/{userId}")]
        public async Task<IActionResult> DeleteUser(string? userId)
        {
            var user = await _userService.FindUserByIdAsync(userId);
            if (user == null) return NotFound($"User with ID {userId} not found.");
            var result = await _userService.DeleteUserAsync(user);
            if (result)
            {
                return Ok($"User with ID {userId} has been deleted.");
            }
            return BadRequest("User deletion failed.");
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(string? userId)
        {
            var user = await _userService.FindUserByIdAsync(userId);
            if (user != null)
            {
                return Ok(user);
            }
            return NotFound($"User with ID {userId} not found.");
        }

        [HttpPost("{userId}/suspend")]
        public async Task<IActionResult> SuspendUser(string userId)
        {
            try
            {
                bool isSuspended = await _userService.SuspendUser(userId);

                if (isSuspended)
                {
                    return Ok("User suspended successfully.");
                }

                return BadRequest("Failed to suspend user.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{userId}/issuspended")]
        public async Task<IActionResult> IsUserSuspended(string userId)
        {
            try
            {
                bool isSuspended = await _userService.IsUserSuspended(userId);

                return Ok(isSuspended ? "User is suspended." : "User is not suspended.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{userId}/unsuspend")]
        public async Task<IActionResult> UnSuspendUser(string userId)
        {
            try
            {
                bool isUnsuspended = await _userService.UnSuspendUser(userId);

                if (isUnsuspended)
                {
                    return Ok("User unsuspended successfully.");
                }
                else
                {
                    return BadRequest("Failed to unsuspend user.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{userId}/lock")]
        public async Task<IActionResult> LockUser(string userId)
        {
            try
            {
                bool isLocked = await _userService.LockUser(userId);

                if (isLocked)
                {
                    return Ok("User locked successfully.");
                }

                return BadRequest("Failed to lock user.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{userId}/unlock")]
        public async Task<IActionResult> UnlockUser(string userId)
        {
            try
            {
                bool isUnlocked = await _userService.UnlockUser(userId);

                if (isUnlocked)
                {
                    return Ok("User unlocked successfully.");
                }

                return BadRequest("Failed to unlock user.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("IsUserLocked/{userId}")]
        public async Task<IActionResult> IsUserLocked(string userId)
        {
            try
            {
                var isLocked = await _userService.IsUserLockedAsync(userId);
                return Ok(isLocked ? "User is locked!" : "User is not locked");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error checking user lock status: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error checking user lock status");
            }
        }

        [HttpGet("IsTwoFactorEnabled/{userId}")]
        public async Task<IActionResult> IsTwoFactorEnabled(string userId)
        {
            try
            {
                var isTwoFactorEnabled = await _userService.IsTwoFactorEnabledAsync(userId);

                // Return the result as a JSON response
                return Ok(new { IsTwoFactorEnabled = isTwoFactorEnabled });
            }
            catch (Exception ex)
            {
                // Log the error and return an error response
                Log.Error(ex, $"Error checking user TwoFactorEnabled status: {ex.Message}");
                return StatusCode(500, new { ErrorMessage = "An error occurred while checking TwoFactorEnabled status." });
            }
        }

        [HttpPost("EnableTwoFactor/{userId}")]
        public async Task<IActionResult> EnableTwoFactor(string userId)
        {
            try
            {
                var isEnableSuccessful = await _userService.EnableTwoFactorAsync(userId);

                if (isEnableSuccessful)
                {
                    return Ok(new { Message = "Two-Factor Authentication has been enabled." });
                }

                return BadRequest(new { ErrorMessage = "Failed to enable Two-Factor Authentication." });
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error enabling Two-Factor Authentication: {ex.Message}");
                return StatusCode(500, new { ErrorMessage = "An error occurred while enabling Two-Factor Authentication." });
            }
        }

        [HttpPost("DisableTwoFactor/{userId}")]
        public async Task<IActionResult> DisableTwoFactor(string userId)
        {
            try
            {
                var isDisableSuccessful = await _userService.DisableTwoFactorAsync(userId);

                if (isDisableSuccessful)
                {
                    return Ok(new { Message = "Two-Factor Authentication has been disabled." });
                }

                return BadRequest(new { ErrorMessage = "Failed to disable Two-Factor Authentication." });
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error disabling Two-Factor Authentication: {ex.Message}");
                return StatusCode(500, new { ErrorMessage = "An error occurred while disabling Two-Factor Authentication." });
            }
        }

        [HttpGet("{userId}/roles")]
        public async Task<IActionResult> GetUserRoles(string? userId)
        {
            var user = await _userService.FindUserByIdAsync(userId);
            if (user == null) return NotFound($"User with ID {userId} not found.");
            var roles = await _userService.GetUserRolesAsync(user);
            return Ok(roles);
        }

        [HttpGet("{userId}/claims")]
        public async Task<IActionResult> GetUserClaims(string? userId)
        {
            var user = await _userService.FindUserByIdAsync(userId);
            if (user == null) return NotFound($"User with ID {userId} not found.");
            var claims = await _userService.GetUserClaimsAsync(user);
            return Ok(claims);
        }

        [HttpGet("{userId}/inrole/{roleName}")]
        public async Task<IActionResult> IsUserInRole(string? userId, string roleName)
        {
            var user = await _userService.FindUserByIdAsync(userId);
            if (user == null) return NotFound($"User with ID {userId} not found.");
            var isInRole = await _userService.IsUserInRoleAsync(user, roleName);
            return Ok(isInRole);
        }


    }
}
