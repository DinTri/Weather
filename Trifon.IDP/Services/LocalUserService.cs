using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Trifon.IDP.Data;
using Trifon.IDP.Entities;
using Weather.CustomExceptions;

namespace Trifon.IDP.Services
{
    public class LocalUserService : ILocalUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<LocalUserService> _logger;

        public LocalUserService(
            ApplicationDbContext context,
            IPasswordHasher<ApplicationUser> passwordHasher,
            UserManager<ApplicationUser> userManager,
            ILogger<LocalUserService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> AddUserSecret(string subject, string name, string secret)
        {
            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new ArgumentNullException(nameof(subject));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrWhiteSpace(secret))
            {
                throw new ArgumentNullException(nameof(secret));
            }

            var user = await GetUserBySubjectAsync(subject);

            if (user == null)
            {
                return false;
            }

            user.Secrets.Add(new UserSecret { Name = name, Secret = secret });
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<UserSecret> GetUserSecretAsync(string subject, string name)
        {
            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new ArgumentNullException(nameof(subject));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            return await _context.UserSecrets
                .FirstOrDefaultAsync(u => u.User.Subject == subject && u.Name == name);
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            if (email is null)
            {
                throw new ArgumentNullException(nameof(email));
            }

            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddExternalProviderToUser(string subject, string provider, string providerIdentityKey)
        {
            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new ArgumentNullException(nameof(subject));
            }

            if (string.IsNullOrWhiteSpace(provider))
            {
                throw new ArgumentNullException(nameof(provider));
            }

            if (string.IsNullOrWhiteSpace(providerIdentityKey))
            {
                throw new ArgumentNullException(nameof(providerIdentityKey));
            }

            var user = await GetUserBySubjectAsync(subject);

            if (user != null)
            {
                var result = await _userManager.AddLoginAsync(user,
                    new UserLoginInfo(provider, providerIdentityKey, provider));

                if (result.Succeeded)
                {
                    _logger.LogInformation("Login added successfully for user: {UserId}", user.Id);
                }
                else
                {
                    _logger.LogError("Error adding login for user: {UserId}", user.Id);
                }
            }
        }

        public async Task<ApplicationUser> FindUserByExternalProviderAsync(string provider, string providerIdentityKey)
        {
            if (string.IsNullOrWhiteSpace(provider))
            {
                throw new ArgumentNullException(nameof(provider));
            }

            if (string.IsNullOrWhiteSpace(providerIdentityKey))
            {
                throw new ArgumentNullException(nameof(providerIdentityKey));
            }

            var userLogin = await _context.UserLogins
                .FirstOrDefaultAsync(ul => ul.LoginProvider == provider && ul.ProviderKey == providerIdentityKey);

            if (userLogin == null)
            {
                return null; // No matching login found
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userLogin.UserId);
            //user.UserName = user.GivenName + user.FamilyName;
            return user;
        }


        public ApplicationUser AutoProvisionUser(string provider, string providerIdentityKey, IEnumerable<Claim> claims)
        {
            if (string.IsNullOrWhiteSpace(provider))
            {
                throw new ArgumentNullException(nameof(provider));
            }

            if (string.IsNullOrWhiteSpace(providerIdentityKey))
            {
                throw new ArgumentNullException(nameof(providerIdentityKey));
            }

            if (claims is null)
            {
                throw new ArgumentNullException(nameof(claims));
            }

            var user = new ApplicationUser
            {
                Suspended = false,
                Subject = Guid.NewGuid().ToString()
            };

            // Log the state of the user object
            _logger.LogInformation(
                $"User object state: UserId={user.Id}, UserName={user.UserName}, Email={user.Email}, GivenName={user.GivenName}, FamilyName={user.FamilyName}");

            foreach (var claim in claims)
            {
                user.Claims.Add(new IdentityUserClaim<string>
                {
                    ClaimType = claim.Type,
                    ClaimValue = claim.Value
                });
            }
            var updatedNameClaim = user.Claims.FirstOrDefault(c => c.ClaimType == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
            if (updatedNameClaim != null)
            {
                var cleanedName = Regex.Replace(updatedNameClaim.ClaimValue, @"\s+", "");
                updatedNameClaim.ClaimValue = cleanedName; // Update the "name" claim value
            }
            var userLogin = new IdentityUserLogin<string>
            {
                LoginProvider = provider,
                ProviderKey = providerIdentityKey,
                UserId = user.Id,
                ProviderDisplayName = "Facebook"
            };

            user.Logins = new List<IdentityUserLogin<string>> { userLogin };

            // Log the state of the user object before adding to context
            _logger.LogInformation(
                $"User object state before adding to context: UserId={user.Id}, UserName={user.UserName}, Email={user.Email}, GivenName={user.GivenName}, FamilyName={user.FamilyName}");

            _context.Users.Add(user);

            // Log that the user has been saved
            _logger.LogInformation($"User has been saved. UserId={user.Id}");

            return user;
        }

        public async Task<bool> ValidateCredentialsAsync(string userName, string password)
        {
            const string pattern = @"\s+"; // Matches one or more whitespace characters
            const string replacement = "";
            var uName = Regex.Replace(userName, pattern, replacement);
            if (string.IsNullOrWhiteSpace(uName) || string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            var user = await GetUserByUserNameAsync(userName);

            if (user == null || user.Suspended)
            {
                return false;
            }

            if (user.PasswordHash == null) return false;
            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            return verificationResult == PasswordVerificationResult.Success;
        }

        public async Task<ApplicationUser> GetUserByUserNameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentNullException(nameof(userName));
            }

            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == userName);
        }

        public async Task<IEnumerable<IdentityUserClaim<string>>> GetUserClaimsBySubjectAsync(string subject)
        {
            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new ArgumentNullException(nameof(subject));
            }

            return (await _context.Users
                .Where(u => u.Subject == subject)
                .SelectMany(u => u.Claims)
                .ToListAsync());
        }

        public async Task<ApplicationUser> GetUserBySubjectAsync(string subject)
        {
            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new ArgumentNullException(nameof(subject));
            }

            return await _context.Users
                .Include(u => u.Claims)
                .Include(u => u.Logins)
                .FirstOrDefaultAsync(u => u.Subject == subject);
        }

        public void AddUser(ApplicationUser userToAdd, string password)
        {
            if (userToAdd == null)
            {
                throw new ArgumentNullException(nameof(userToAdd));
            }

            if (_context.Users.Any(u => u.UserName == userToAdd.UserName))
            {
                throw new WeatherException("Username must be unique");
            }

            if (_context.Users.Any(u => u.Email == userToAdd.Email))
            {
                throw new WeatherException("Email must be unique");
            }

            userToAdd.SecurityCode = Convert.ToBase64String(RandomNumberGenerator.GetBytes(128));
            userToAdd.SecurityCodeExpirationDate = DateTime.UtcNow.AddHours(1);

            userToAdd.PasswordHash = _passwordHasher.HashPassword(userToAdd, password);

            _context.Users.Add(userToAdd);
            _context.SaveChanges();
        }

        public async Task<bool> IsUserActive(string subject)
        {
            if (string.IsNullOrWhiteSpace(subject))
            {
                return false;
            }

            var user = await GetUserBySubjectAsync(subject);

            if (user == null)
            {
                return false;
            }

            return !user.Suspended;
        }

        public async Task<bool> ActivateUserAsync(string securityCode)
        {
            if (string.IsNullOrWhiteSpace(securityCode))
            {
                throw new ArgumentNullException(nameof(securityCode));
            }

            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.SecurityCode == securityCode &&
                u.SecurityCodeExpirationDate >= DateTime.UtcNow);

            if (user == null)
            {
                return false;
            }

            user.Suspended = false;
            user.SecurityCode = null;

            await _context.SaveChangesAsync();
            return true;
        }

        public Task<int> GetCountryIdFromCountryName(string countryName)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() > 0);
        }

        public async Task<LocationDetails_IpApi> GetCountryFromIpAddressAsync()
        {
            string hostName = Dns.GetHostName();
            IPAddress[] addresses = (await Dns.GetHostEntryAsync(hostName)).AddressList;
            string myIP = addresses[0].ToString(); // You might need to loop through addresses to find the desired one

            using HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            // Define the base address of the geolocation API
            httpClient.BaseAddress = new Uri("localhost:*"); // Replace with the actual API URL

            // Make an asynchronous GET request to the geolocation API
            HttpResponseMessage httpResponse = await httpClient.GetAsync(myIP);

            // Initialize LocationDetails_IpApi with default values
            LocationDetails_IpApi locDetails = new LocationDetails_IpApi();

            if (httpResponse.IsSuccessStatusCode)
            {
                // Read and parse the JSON response
                string jsonContent = await httpResponse.Content.ReadAsStringAsync();

                // Deserialize the JSON response into LocationDetails_IpApi object
                locDetails = JsonConvert.DeserializeObject<LocationDetails_IpApi>(jsonContent);
            }

            return locDetails;
        }
    }
}
