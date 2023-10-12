using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Weather.Api.Entities;
using Weather.Api.Entities.Models;
using Weather.Api.IdentityRoleServices;

namespace Weather.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RolesController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<RolesController> _logger;

        public RolesController(IUserService userService, ILogger<RolesController> logger)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger;
        }

        [HttpPost("create/{Name}")]
        public async Task<IActionResult> CreateRole(string Name)
        {
            try
            {
                var result = await _userService.CreateRoleAsync(Name);
                if (result)
                {
                    return Ok("Role created successfully.");
                }

                return BadRequest("Role creation failed.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating role: {ex}");
                return BadRequest("An error occurred while creating the role. Please try again later.");
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateRole([FromBody] AspNetRoles role)
        {
            var result = await _userService.UpdateRoleAsync(role);
            if (result)
            {
                return Ok("Role updated successfully.");
            }

            return BadRequest("Role update failed.");
        }

        [HttpPost("adduserToRole")]
        public async Task<IActionResult> AddUserToRole([FromBody] UserToRoleRequest request)
        {
            var user = await _userService.FindUserByIdAsync(request.UserId);

            if (user == null)
            {
                return NotFound($"User with ID {request.UserId} not found.");
            }

            var result = await _userService.AddUserToRoleAsync(user, request.RoleName);

            if (result.Succeeded)
            {
                return Ok($"User '{user.UserName}' added to role '{request.RoleName}' successfully.");
            }
            if (!result.Succeeded)
            {
                return Ok($"User '{user.UserName}' already has the role '{request.RoleName}'!");
            }

            return BadRequest(result.FailureReason);
        }

        [HttpPost("adduserToRoles")]
        public async Task<IActionResult> AddUserToRoles([FromBody] AddRolesToUserRequest request)
        {
            try
            {
                var user = await _userService.FindUserByIdAsync(request.UserId);

                if (user == null)
                {
                    return NotFound($"User with ID {request.UserId} not found.");
                }

                var results = await _userService.AddUserToRolesAsync(user, request.RolesToAdd);

                if (results.TrueForAll(result => result.Succeeded))
                {
                    return Ok($"User '{user.UserName}' added to roles '{string.Join(", ", request.RolesToAdd)}' successfully.");
                }

                var failureReasons = results
                    .Where(result => !result.Succeeded)
                    .Select(result => result.FailureReason)
                    .Distinct()
                    .ToList();

                return !failureReasons.TrueForAll(reason => reason == "User already has the role") ?
                    // Return a 409 Conflict status code for duplicate roles
                    StatusCode(200, $"User '{user.UserName}' already has the roles '{string.Join(", ", request.RolesToAdd)}'!") :
                    // Return a 400 Bad Request status code for other failure reasons
                    BadRequest(string.Join("; ", failureReasons));
            }
            catch (Exception ex)
            {
                // Log and handle the exception as needed
                _logger.LogError($"Error creating role: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }


        [HttpPost("removeuser")]
        public async Task<IActionResult> RemoveUserFromRole([FromBody] UserToRoleRequest request)
        {
            var user = await _userService.FindUserByIdAsync(request.UserId);
            if (user != null)
            {
                var result = await _userService.RemoveUserFromRoleAsync(user, request.RoleName);
                if (result)
                {
                    return Ok($"User '{user.UserName}' removed from role '{request.RoleName}' successfully.");
                }
                return BadRequest($"Failed to remove user '{user.UserName}' from role '{request.RoleName}'.");
            }
            return NotFound($"User with ID {request.UserId} not found.");
        }

        [HttpPost("removerolesfromuser")]
        public async Task<IActionResult> RemoveRolesFromUser([FromBody] RemoveRolesFromUserRequest request)
        {
            var user = await _userService.FindUserByIdAsync(request.UserId);
            if (user != null)
            {
                var result = await _userService.RemoveRolesFromUserAsync(user, request.RolesToRemove);
                if (result)
                {
                    return Ok($"Roles removed from user '{user.UserName}' successfully.");
                }
                return BadRequest($"Failed to remove roles from user '{user.UserName}'.");
            }
            return NotFound($"User with ID {request.UserId} not found.");
        }

        // GET: api/roles
        [HttpGet]
        public async Task<IActionResult> GetAllRolesAsync()
        {
            try
            {
                var roles = await _userService.GetAllRolesAsync();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                // Handle exceptions and log errors
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }

        // DELETE: api/roles/{roleId}
        [HttpDelete("{roleId}")]
        public async Task<IActionResult> DeleteRoleAsync(string roleId)
        {
            try
            {
                var role = await _userService.GetRoleByIdAsync(roleId);
                if (role == null)
                {
                    return NotFound($"Role with ID {roleId} not found.");
                }

                var result = await _userService.DeleteRoleAsync(role);
                if (result)
                {
                    _logger.LogInformation($"Role with ID '{roleId}' has been deleted.");
                    return NoContent();
                }
                _logger.LogInformation($"Failed to delete role with ID '{roleId}'.");
                return BadRequest("Failed to delete the role.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting role: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }

        // GET: api/roles/{roleId}
        [HttpGet("{roleId}")]
        public async Task<IActionResult> GetRoleByIdAsync(string roleId)
        {
            try
            {
                var role = await _userService.GetRoleByIdAsync(roleId);
                if (role == null)
                {
                    return NotFound($"Role with ID {roleId} not found.");
                }

                return Ok(role);
            }
            catch (Exception ex)
            {
                // Handle exceptions and log errors
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }

        // GET: api/roles/{roleName}/users
        [HttpGet("{roleName}/users")]
        public async Task<IActionResult> GetUsersInRoleAsync(string roleName)
        {
            try
            {
                var users = await _userService.GetUsersInRoleAsync(roleName);
                return Ok(users);
            }
            catch (Exception ex)
            {
                // Handle exceptions and log errors
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }
    }
}
