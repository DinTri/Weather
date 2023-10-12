using System.Security.Claims;
using Weather.Api.Entities;
using Weather.Api.Entities.Models;

namespace Weather.Api.IdentityRoleServices
{
    public interface IUserService
    {
        // Methods related to users
        Task<bool> CreateUserAsync(AspNetUsers user, string password, IFormFile? photo);
        Task<AspNetUsers?> FindUserByIdAsync(string? userId);
        Task<AspNetUsers?> GetUserByUsernameAsync(string username);
        Task<bool> UploadProfilePhotoAsync(string userId, IFormFile? photo);
        Task<bool> UpdateUserAsync(AspNetUsers user, IFormFile? photo, string? userName, string? familyName, string? email, string? phoneNumber,
            bool suspended, bool twoFactorEnabled, bool lockoutEnabled);
        Task<bool> DeleteUserAsync(AspNetUsers user);
        Task<List<AspNetUsers>> GetAllUsersAsync();
        Task<bool> SuspendUser(string userId);
        Task<bool> IsUserSuspended(string userId);
        Task<bool> UnSuspendUser(string userId);
        Task<bool> IsUserLockedAsync(string userId);
        Task<bool> LockUser(string userId);
        Task<bool> UnlockUser(string userId);
        Task<bool> IsTwoFactorEnabledAsync(string userId);
        Task<bool> EnableTwoFactorAsync(string userId);
        Task<bool> DisableTwoFactorAsync(string userId);
        Task<IEnumerable<string>> GetUserRolesAsync(AspNetUsers user);
        Task<IList<Claim>> GetUserClaimsAsync(AspNetUsers user);
        Task<bool> IsUserInRoleAsync(AspNetUsers user, string roleName);

        // Methods related to roles
        Task<IList<AspNetRoles>> GetAllRolesAsync();
        Task<AddUserToRoleResult> AddUserToRoleAsync(AspNetUsers user, string? roleName);

        Task<List<AddUserToRoleResult>> AddUserToRolesAsync(AspNetUsers user, IEnumerable<string> roleNames);
        Task<bool> RemoveUserFromRoleAsync(AspNetUsers user, string? roleName);
        Task<bool> RemoveRolesFromUserAsync(AspNetUsers user, IEnumerable<string> roles);
        Task<bool> CreateRoleAsync(string roleName);
        Task<bool> DeleteRoleAsync(AspNetRoles role);
        Task<bool> UpdateRoleAsync(AspNetRoles role);
        Task<AspNetRoles?> GetRoleByIdAsync(string roleId);
        Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName);


        // Methods related to countries
        Task<IList<Country>> GetAllCountriesAsync();
        Task<Country?> GetCountryByIdAsync(int countryId);
        Task<Country> GetCountryByUserIdAsync(string userId);

    }
}
