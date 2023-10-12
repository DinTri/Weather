using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Weather.CustomExceptions;
using Weather.Web.Administration.Models;
using Weather.Web.Administration.ViewModels;

namespace Weather.Web.Controllers
{
    [Authorize(Policy = "AdminCanManageRoles")]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public AdminController(IHttpClientFactory httpClientFactory, ILogger<AdminController> logger, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger;
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        }

        [HttpGet]
        public async Task<IActionResult> ListAllUsers()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var httpClient = _httpClientFactory.CreateClient("APIClient");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var apiBaseUrl = _configuration["WeatherAPIRoot"];
            var apiUrl = $"{apiBaseUrl}/api/Users/all";
            var response = await httpClient.GetAsync(apiUrl);
            if (!response.IsSuccessStatusCode)
            {
                // Handle the error response and throw a custom exception
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("User API Error: {ErrorContent}", errorContent);

                // Customize the error message and status code based on the specific error
                var errorMessage = "An error occurred while fetching user data. Please try again later.";

                throw new WeatherException(errorMessage, (int)response.StatusCode);
            }
            var content = await response.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<List<UserResponse>>(content);

            if (users != null)
            {
                // Store the user data in TempData
                TempData["UserData"] = JsonConvert.SerializeObject(users);
            }
            else
            {
                _logger.LogError("Users are null.");
            }
            return View("ListAllUsers");
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                var httpClient = _httpClientFactory.CreateClient("APIClient");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var apiBaseUrl = _configuration["WeatherAPIRoot"];
                var userApiUrl = $"{apiBaseUrl}/api/Users/{id}";
                var rolesApiUrl = $"{apiBaseUrl}/api/Users/{id}/roles";
                var isSuspendedUri = $"{apiBaseUrl}/api/Users/{id}/issuspended";
                var IsUserLockedUri = $"{apiBaseUrl}/api/Users/IsUserLocked/{id}";
                var isTwoFactorUrl = $"{apiBaseUrl}/api/Users/IsTwoFactorEnabled/{id}";
                // Send an HTTP GET request to retrieve user details
                var userResponse = await httpClient.GetAsync(userApiUrl);

                if (!userResponse.IsSuccessStatusCode)
                {
                    ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                    return View("NotFound");
                }

                var userJson = await userResponse.Content.ReadAsStringAsync();

                // Deserialize the JSON response into EditUserViewModel
                var userViewModel = JsonConvert.DeserializeObject<EditUserViewModel>(userJson);
                if (userViewModel == null)
                {
                    _logger.LogError("Deserialization of EditUserViewModel failed. JSON: " + userJson);
                    // Handle the error or return an appropriate response
                }
                //User Suspended
                var isSuspendedResponse = await httpClient.GetAsync($"{isSuspendedUri}");
                if (isSuspendedResponse.IsSuccessStatusCode)
                {
                    var isSuspended =
                        JsonConvert.DeserializeObject<string>(await isSuspendedResponse.Content.ReadAsStringAsync());
                    if (userViewModel != null)
                    {
                        userViewModel.Suspended = isSuspended switch
                        {
                            "User is suspended." => true,
                            "User is not suspended." => false,
                            _ => userViewModel.Suspended
                        };
                    }
                }
                //User Locked
                var isLockedResponse = await httpClient.GetAsync($"{IsUserLockedUri}");
                if (isLockedResponse.IsSuccessStatusCode)
                {
                    var isLocked =
                        JsonConvert.DeserializeObject<string>(await isLockedResponse.Content.ReadAsStringAsync());
                    if (userViewModel != null)
                    {
                        userViewModel.LockoutEnabled = isLocked switch
                        {
                            "User is locked!" => true,
                            "User is not locked" => false,
                            _ => userViewModel.LockoutEnabled
                        };
                    }
                }

                //Two Factor
                var isTwoFactorResponse = await httpClient.GetAsync($"{isTwoFactorUrl}");
                if (isTwoFactorResponse.IsSuccessStatusCode)
                {
                    var responseContent = await isTwoFactorResponse.Content.ReadAsStringAsync();
                    if (bool.TryParse(responseContent, out bool isTwoFactor) && userViewModel != null)
                    {
                        userViewModel.TwoFactorEnabled = isTwoFactor;
                    }

                }

                // Fetch user roles
                var rolesResponse = await httpClient.GetAsync(rolesApiUrl);
                if (rolesResponse.IsSuccessStatusCode)
                {
                    var rolesJson = await rolesResponse.Content.ReadAsStringAsync();
                    // Deserialize the JSON response into a collection of user roles
                    var userRoles = JsonConvert.DeserializeObject<List<string>>(rolesJson);
                    if (userViewModel != null) userViewModel.Roles = userRoles;
                }

                return View(userViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception Occurred: {ex}");
                ViewBag.ErrorTitle = "Error";
                ViewBag.ErrorMessage = "An error occurred while processing your request.";
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // If the model state is not valid, return the view with validation errors
                return View(model);
            }

            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                var httpClient = _httpClientFactory.CreateClient("APIClient");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var apiBaseUrl = _configuration["WeatherAPIRoot"];
                var apiUrl = $"{apiBaseUrl}/api/Users/update/{model.Id}";

                // Create a multipart form data content to send in the request body
                if (model is { UserName: not null, Email: not null, FamilyName: not null, PhoneNumber: not null, Id: not null })
                {
                    var formContent = new MultipartFormDataContent
                    {
                        // Add the model data as form fields
                        { new StringContent(model.Id), "Id" },
                        { new StringContent(model.UserName), "UserName" },
                        { new StringContent(model.Email), "Email" },
                        { new StringContent(model.FamilyName), "FamilyName" },
                        { new StringContent(model.PhoneNumber), "PhoneNumber" },
                        { new StringContent(model.Suspended.ToString()), "Suspended" },
                        { new StringContent(model.TwoFactorEnabled.ToString()), "TwoFactorEnabled" },
                        { new StringContent(model.LockoutEnabled.ToString()), "LockoutEnabled" }
                    };

                    // Add the photo as a file if it's provided
                    if (model.Photo is { Length: > 0 })
                    {
                        formContent.Add(new StreamContent(model.Photo.OpenReadStream())
                        {
                            Headers =
                            {
                                ContentLength = model.Photo.Length,
                                ContentType = new MediaTypeHeaderValue(model.Photo.ContentType)
                            }
                        }, "Photo", model.Photo.FileName);
                    }

                    // Send an HTTP PUT request to update the user's information
                    var response = await httpClient.PutAsync(apiUrl, formContent);

                    if (response.IsSuccessStatusCode)
                    {
                        // User updated successfully, you can redirect to a success page or action
                        return View(model);
                    }
                }

                ViewBag.ErrorMessage = $"Error updating user with Id = {model.Id}";
                return View("Error"); // Handle the error as needed
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception Occurred: {ex}");
                ViewBag.ErrorTitle = "Error";
                ViewBag.ErrorMessage = "An error occurred while processing your request.";
                return View("Error");
            }
        }

        [HttpPost]
        [Authorize(Policy = "DeleteRolePolicy")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                var httpClient = _httpClientFactory.CreateClient("APIClient");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var apiBaseUrl = _configuration["WeatherAPIRoot"];
                var apiUrl = $"{apiBaseUrl}/api/Users/{id}";

                // Send an HTTP DELETE request to delete the role
                var response = await httpClient.DeleteAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("ListAllUsers");
                }

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                    return View("NotFound");
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Error deleting user: {errorContent}");
                ModelState.AddModelError("", "An error occurred while deleting the user.");
                return View("ListAllUsers");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception Occurred: {ex}");
                ViewBag.ErrorTitle = "Error";
                ViewBag.ErrorMessage = "An error occurred while processing your request.";
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListAllRoles()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var httpClient = _httpClientFactory.CreateClient("APIClient");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var apiBaseUrl = _configuration["WeatherAPIRoot"];
            var apiUrl = $"{apiBaseUrl}/api/Roles";
            var response = await httpClient.GetAsync(apiUrl);
            if (!response.IsSuccessStatusCode)
            {
                // Handle the error response and throw a custom exception
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("User API Error: {ErrorContent}", errorContent);

                // Customize the error message and status code based on the specific error
                var errorMessage = "An error occurred while fetching role data. Please try again later.";

                throw new WeatherException(errorMessage, (int)response.StatusCode);
            }
            var content = await response.Content.ReadAsStringAsync();
            var roles = JsonConvert.DeserializeObject<List<RoleResponse>>(content);

            if (roles != null)
            {
                // Store the user data in TempData
                TempData["RoleData"] = JsonConvert.SerializeObject(roles);
            }
            else
            {
                _logger.LogError("Roles are null.");
            }
            return View("ListAllRoles");
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                var httpClient = _httpClientFactory.CreateClient("APIClient");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var apiBaseUrl = _configuration["WeatherAPIRoot"];
                var apiUrl = $"{apiBaseUrl}/api/Roles/create/{model.Name}";
                // Prepare the data to send to the API
                var requestData = new { model.Name };
                var jsonRequestData = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(jsonRequestData, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(apiUrl, content);

                // Send a POST request to the API

                if (response.IsSuccessStatusCode)
                {
                    var roleData = new CreateRoleViewModel
                    {
                        Name = model.Name,
                        SuccessMessage = "Role created successfully!"
                    };

                    // Serialize the roleData object to store it in TempData
                    TempData["RoleData"] = JsonConvert.SerializeObject(roleData);
                    return RedirectToAction("ListAllRoles");
                }

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    // Handle validation errors returned by the API
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, "Validation error: " + errorContent);
                }
                else
                {
                    // Handle other types of errors, e.g., server errors
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Role API Error: {ErrorContent}", errorContent);
                    ModelState.AddModelError(string.Empty, "An error occurred while creating the role. Please try again later.");
                }

                // If there are errors, return the same view with model and errors
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating role: {ex}");
                ModelState.AddModelError(string.Empty, "An error occurred while creating the role. Please try again later.");
                return View(model);
            }
        }

        [HttpPost]
        [Authorize(Policy = "DeleteRolePolicy")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                var httpClient = _httpClientFactory.CreateClient("APIClient");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var apiBaseUrl = _configuration["WeatherAPIRoot"];
                var apiUrl = $"{apiBaseUrl}/api/Roles/{id}";

                // Send an HTTP DELETE request to delete the role
                var response = await httpClient.DeleteAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("ListAllRoles");
                }

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                    return View("NotFound");
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Error deleting role: {errorContent}");
                ModelState.AddModelError("", "An error occurred while deleting the role.");
                return View("ListAllRoles");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception Occurred: {ex}");
                ViewBag.ErrorTitle = "Error";
                ViewBag.ErrorMessage = "An error occurred while processing your request.";
                return View("Error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> EditRole(string id)
        {
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                var httpClient = _httpClientFactory.CreateClient("APIClient");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var apiBaseUrl = _configuration["WeatherAPIRoot"];
                var apiUrl = $"{apiBaseUrl}/api/Roles/{id}";
                var usersUrl = $"{apiBaseUrl}/api/Users/all";
                // Send an HTTP GET request to retrieve role details
                var response = await httpClient.GetAsync(apiUrl);

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                    return View("NotFound");
                }

                var roleDetailsJson = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON response into RoleDetailsViewModel
                var roleDetails = JsonConvert.DeserializeObject<EditRoleViewModel>(roleDetailsJson);
                if (roleDetails == null)
                {
                    ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                    return View("NotFound");
                }
                var model = new EditRoleViewModel
                {
                    Id = roleDetails.Id,
                    Name = roleDetails.Name
                };

                var allUsersResponse = await httpClient.GetAsync(usersUrl);
                if (!allUsersResponse.IsSuccessStatusCode)
                {
                    ViewBag.ErrorMessage = $"Users cannot be found";
                    return View(model);
                }

                var usersJson = await allUsersResponse.Content.ReadAsStringAsync();
                var userDetails = JsonConvert.DeserializeObject<List<UserResponse>>(usersJson);

                if (userDetails is null) return View(model);
                foreach (var user in userDetails)
                {
                    var isInRoleResponse = await httpClient.GetAsync($"{apiBaseUrl}/api/Users/{user.Id}/inrole/{model.Name}");

                    if (!isInRoleResponse.IsSuccessStatusCode)
                    {
                        continue;
                    }

                    var isInRole = JsonConvert.DeserializeObject<bool>(await isInRoleResponse.Content.ReadAsStringAsync());

                    // Check if the user is in the role and add their username if needed
                    if (isInRole && !model.Users.Contains(user.UserName))
                    {
                        model.Users.Add(user.UserName);
                    }
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception Occurred: {ex}");
                ViewBag.ErrorTitle = "Error";
                ViewBag.ErrorMessage = "An error occurred while processing your request.";
                return View("Error");
            }
        }



        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                var httpClient = _httpClientFactory.CreateClient("APIClient");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var apiBaseUrl = _configuration["WeatherAPIRoot"];
                var apiUrl = $"{apiBaseUrl}/api/Roles/{model.Id}";

                // Prepare the data to send to the Web API

                var requestData = new { RoleName = model.Name };
                var jsonRequestData = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(jsonRequestData, Encoding.UTF8, "application/json");

                // Send an HTTP PUT request to update the role details
                var response = await httpClient.PutAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("ListAllRoles");
                }

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    ViewBag.ErrorMessage = $"Role with Id = {model.Id} cannot be found";
                    return View("NotFound");
                }

                // Handle validation errors or other errors as needed
                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, "Validation error: " + errorContent);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception Occurred: {ex}");
                ViewBag.ErrorTitle = "Error";
                ViewBag.ErrorMessage = "An error occurred while processing your request.";
                return View("Error");
            }
        }

        [HttpGet("editUsersInRole/{roleId}")]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            try
            {
                ViewBag.roleId = roleId;
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                var httpClient = _httpClientFactory.CreateClient("APIClient");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var apiBaseUrl = _configuration["WeatherAPIRoot"];

                // Retrieve role details
                var roleViewModel = await GetRoleDetailsAsync(httpClient, apiBaseUrl, roleId);

                if (roleViewModel == null)
                {
                    ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                    return View("NotFound");
                }

                // Retrieve users and their role assignments
                var model = await GetUserRoleAssignmentsAsync(httpClient, apiBaseUrl, roleViewModel);

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception Occurred: {ex}");
                ViewBag.ErrorTitle = "Error";
                ViewBag.ErrorMessage = "An error occurred while processing your request.";
                return View("Error");
            }
        }

        private async Task<RoleResponse?> GetRoleDetailsAsync(HttpClient httpClient, string? apiBaseUrl, string roleId)
        {
            var roleApiUrl = $"{apiBaseUrl}/api/Roles/{roleId}";
            var roleResponse = await httpClient.GetAsync(roleApiUrl);

            if (!roleResponse.IsSuccessStatusCode)
            {
                return null;
            }

            var roleJson = await roleResponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<RoleResponse>(roleJson);
        }

        private async Task<List<UserRoleViewModel>> GetUserRoleAssignmentsAsync(HttpClient httpClient, string? apiBaseUrl, RoleResponse roleViewModel)
        {
            var usersApiUrl = $"{apiBaseUrl}/api/Users/all";
            var usersResponse = await httpClient.GetAsync(usersApiUrl);

            if (!usersResponse.IsSuccessStatusCode)
            {
                return new List<UserRoleViewModel>();
            }

            var usersJson = await usersResponse.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<List<UserResponse>>(usersJson);

            var model = new List<UserRoleViewModel>();

            if (users == null) return model;
            foreach (var user in users)
            {
                var isInRoleResponse =
                    await httpClient.GetAsync($"{apiBaseUrl}/api/Users/{user.Id}/inrole/{roleViewModel.Name}");

                if (!isInRoleResponse.IsSuccessStatusCode)
                {
                    continue;
                }

                var isInRole =
                    JsonConvert.DeserializeObject<bool>(await isInRoleResponse.Content.ReadAsStringAsync());

                if (user.UserName != null)
                {
                    var userRoleViewModel = new UserRoleViewModel
                    {
                        UserId = user.Id,
                        UserName = user.UserName,
                        IsSelected = isInRole
                    };
                    model.Add(userRoleViewModel);
                }
            }

            return model;
        }

        [HttpPost("editUsersInRole/{roleId}")]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                var httpClient = _httpClientFactory.CreateClient("APIClient");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var apiBaseUrl = _configuration["WeatherAPIRoot"];

                // Check if the role exists
                var roleResponse = await httpClient.GetAsync($"{apiBaseUrl}/api/Roles/{roleId}");
                if (!roleResponse.IsSuccessStatusCode)
                {
                    ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                    return View("NotFound");
                }

                var roleJson = await roleResponse.Content.ReadAsStringAsync();
                var roleViewModel = JsonConvert.DeserializeObject<RoleResponse>(roleJson);

                foreach (var userRoleViewModel in model)
                {
                    if (roleViewModel == null) continue;

                    var request = new UserToRoleRequest()
                    {
                        UserId = userRoleViewModel.UserId,
                        RoleName = roleViewModel.Name,
                    };

                    var jsonModel = JsonConvert.SerializeObject(request);
                    var content = new StringContent(jsonModel, Encoding.UTF8, "application/json");

                    var isInRoleResponse = await httpClient.GetAsync($"{apiBaseUrl}/api/Users/{userRoleViewModel.UserId}/inrole/{roleViewModel.Name}");

                    if (!isInRoleResponse.IsSuccessStatusCode) continue;
                    var isInRole = JsonConvert.DeserializeObject<bool>(await isInRoleResponse.Content.ReadAsStringAsync());

                    if (userRoleViewModel.IsSelected)
                    {
                        // Add the user to the role if they are not already in it
                        if (!isInRole)
                        {
                            _ = AddUserToRoleAsync(httpClient, apiBaseUrl, content);
                        }
                    }
                    else
                    {
                        // Remove the user from the role if they are in it
                        if (isInRole)
                        {
                            _ = RemoveUserFromRoleAsync(httpClient, apiBaseUrl, content);
                        }
                    }
                }

                // Redirect to the EditRole page
                return RedirectToAction("EditRole", new { Id = roleId });
            }
            catch (Exception ex)
            {
                // Log and handle the exception as needed
                _logger.LogError($"Exception Occurred: {ex}");
                ViewBag.ErrorTitle = "Error";
                ViewBag.ErrorMessage = "An error occurred while processing your request.";
                return View("Error");
            }
        }

        private async Task<bool> AddUserToRoleAsync(HttpClient httpClient, string? apiBaseUrl, StringContent content)
        {
            var apiUrlAddToRole = $"{apiBaseUrl}/api/Roles/addUserToRole";
            var response = await httpClient.PostAsync(apiUrlAddToRole, content);
            return response.IsSuccessStatusCode;
        }

        private async Task<bool> RemoveUserFromRoleAsync(HttpClient httpClient, string? apiBaseUrl, StringContent content)
        {
            var apiUrlRemoveFromRole = $"{apiBaseUrl}/api/Roles/removeuser";
            var response = await httpClient.PostAsync(apiUrlRemoveFromRole, content);
            return response.IsSuccessStatusCode;
        }


        [HttpGet]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> ManageUserRoles(string userId)
        {
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                var httpClient = _httpClientFactory.CreateClient("APIClient");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var apiBaseUrl = _configuration["WeatherAPIRoot"];

                // Fetch user details
                var userResponse = await httpClient.GetAsync($"{apiBaseUrl}/api/Users/{userId}");

                if (!userResponse.IsSuccessStatusCode)
                {
                    // Handle the error response
                    var errorContent = await userResponse.Content.ReadAsStringAsync();
                    _logger.LogError("User API Error: {ErrorContent}", errorContent);
                    ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                    return View("NotFound");
                }

                // Fetch roles for user
                var rolesResponse = await httpClient.GetAsync($"{apiBaseUrl}/api/Roles/");
                if (!rolesResponse.IsSuccessStatusCode)
                {
                    ViewBag.ErrorMessage = $"Roles cannot be found";
                    return View("NotFound");
                }

                var rolesJson = await rolesResponse.Content.ReadAsStringAsync();
                var userRolesViewModels = JsonConvert.DeserializeObject<List<UserRolesViewModel>>(rolesJson);
                var model = new List<UserRolesViewModel>();

                if (userRolesViewModels != null)
                    foreach (var role in userRolesViewModels)
                    {
                        var userRolesViewModel = new UserRolesViewModel
                        {
                            Id = role.Id,
                            Name = role.Name
                        };

                        var isInRoleResponse =
                            await httpClient.GetAsync(
                                $"{apiBaseUrl}/api/Users/{userId}/inrole/{userRolesViewModel.Name}");

                        if (isInRoleResponse.IsSuccessStatusCode)
                        {
                            var isInRole =
                                JsonConvert.DeserializeObject<bool>(await isInRoleResponse.Content.ReadAsStringAsync());
                            userRolesViewModel.IsSelected = isInRole;
                        }

                        model.Add(userRolesViewModel);
                    }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception Occurred: {ex}");
                ViewBag.ErrorTitle = "Error";
                ViewBag.ErrorMessage = "An error occurred while processing your request.";
                return View("Error");
            }
        }


        private async Task<List<UserRolesViewModel>> GetUsersInRoleAsync(HttpClient httpClient, string? apiBaseUrl, string userId)
        {
            var userRolesViewModels = new List<UserRolesViewModel>();

            // Make a GET request to your GetUserRoles endpoint
            var response = await httpClient.GetAsync($"{apiBaseUrl}/api/Users/{userId}/roles");

            if (!response.IsSuccessStatusCode) return userRolesViewModels;

            var rolesJson = await response.Content.ReadAsStringAsync();
            var roles = JsonConvert.DeserializeObject<List<string>>(rolesJson);

            // Convert the list of role names to UserRolesViewModel objects
            foreach (var roleName in roles)
            {
                var userRolesViewModel = new UserRolesViewModel
                {
                    Name = roleName
                };

                userRolesViewModels.Add(userRolesViewModel);
            }

            return userRolesViewModels;
        }


        [HttpPost]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> ManageUserRoles(List<UserRolesViewModel> model, string userId)
        {
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                var httpClient = _httpClientFactory.CreateClient("APIClient");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var apiBaseUrl = _configuration["WeatherAPIRoot"];
                var addRolesApiUrl = $"{apiBaseUrl}/api/Roles/addUserToRoles";
                var removeRolesApiUrl = $"{apiBaseUrl}/api/Roles/removerolesfromuser";


                // Process the roles for the user
                var rolesToAdd = model.Where(r => r.IsSelected).Select(r => r.Name).ToList();
                var rolesToRemove = model.Where(r => !r.IsSelected).Select(r => r.Name).ToList();

                // Send requests to add and remove roles
                var addRolesRequest = new AddRolesToUserRequest
                {
                    UserId = userId,
                    RolesToAdd = rolesToAdd!
                };
                var removeRolesRequest = new RemoveRolesFromUserRequest
                {
                    UserId = userId,
                    RolesToRemove = rolesToRemove!
                };

                // Add roles to the user
                var addRolesResponse = await httpClient.PostAsJsonAsync(addRolesApiUrl, addRolesRequest);
                if (!addRolesResponse.IsSuccessStatusCode)
                {
                    ModelState.AddModelError("", "Cannot add selected roles to user");
                    return View(model);
                }

                // Remove roles from the user
                var removeRolesResponse = await httpClient.PostAsJsonAsync(removeRolesApiUrl, removeRolesRequest);
                if (removeRolesResponse.IsSuccessStatusCode) return RedirectToAction("EditUser", new { Id = userId });
                ModelState.AddModelError("", "Cannot remove user existing roles");
                return View(model);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception Occurred: {ex}");
                ViewBag.ErrorTitle = "Error";
                ViewBag.ErrorMessage = "An error occurred while processing your request.";
                return View("Error");
            }
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
