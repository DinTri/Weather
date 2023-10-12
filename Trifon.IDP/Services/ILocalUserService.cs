using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Trifon.IDP.Entities;

namespace Trifon.IDP.Services
{

    public interface ILocalUserService
    {
        Task<UserSecret> GetUserSecretAsync(string subject, string name);
        Task<bool> AddUserSecret(string subject, string name, string secret);
        Task<ApplicationUser> GetUserByEmailAsync(string email);

        Task AddExternalProviderToUser(
            string subject,
            string provider,
            string providerIdentityKey);

        Task<ApplicationUser> FindUserByExternalProviderAsync(
            string provider,
            string providerIdentityKey);

        public ApplicationUser AutoProvisionUser(string provider,
            string providerIdentityKey,
            IEnumerable<Claim> claims);

        Task<bool> ValidateCredentialsAsync(
            string userName,
            string password);

        Task<IEnumerable<IdentityUserClaim<string>>> GetUserClaimsBySubjectAsync(string subject);

        Task<ApplicationUser> GetUserByUserNameAsync(
            string userName);

        Task<ApplicationUser> GetUserBySubjectAsync(
            string subject);

        void AddUser
        (ApplicationUser userToAdd,
            string password);

        Task<bool> IsUserActive(
            string subject);

        Task<bool> ActivateUserAsync(string securityCode);
        Task<LocationDetails_IpApi> GetCountryFromIpAddressAsync();
        Task<int> GetCountryIdFromCountryName(string countryName);

        Task<bool> SaveChangesAsync();
    }
}