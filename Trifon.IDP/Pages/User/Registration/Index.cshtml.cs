using Duende.IdentityServer.Services;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Trifon.IDP.EmailService;
using Trifon.IDP.EmailService.Dtos;
using Trifon.IDP.Entities;
using Trifon.IDP.Repositories;
using Trifon.IDP.Services;

namespace Trifon.IDP.Pages.User.Registration
{
    [AllowAnonymous]
    [SecurityHeaders]
    public class IndexModel : PageModel
    {
        private readonly ILocalUserService _localUserService;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly ICountryRepository _countryRepository;
        private readonly IEmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public IndexModel(
            ILocalUserService localUserService,
            IIdentityServerInteractionService interaction,
            ICountryRepository countryRepository,
            IEmailService emailService,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _localUserService = localUserService ??
                throw new ArgumentNullException(nameof(localUserService));
            _interaction = interaction ??
                throw new ArgumentNullException(nameof(interaction));
            _countryRepository = countryRepository ?? throw new ArgumentNullException(nameof(countryRepository));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public async Task<IActionResult> OnGetAsync(string returnUrl)
        {
            BuildModel(returnUrl);
            await LoadCountriesAsync();
            return Page();
        }

        private void BuildModel(string returnUrl)
        {
            Input = new InputModel
            {
                ReturnUrl = returnUrl
            };
        }

        private async Task LoadCountriesAsync()
        {
            var countries = await _countryRepository.GetAllCountriesAsync();
            var chosenCountryIsoCodes = new List<string> { "GB", "BG", "DE", "FR", "ES", "IT", "TR", "US", "CA" };
            ViewData["ChosenCountryIsoCodes"] = chosenCountryIsoCodes;
            // Manually reorder the countries based on chosenCountryIsoCodes (case-insensitive)
            var orderedCountries = new List<Country>();

            foreach (var iso in chosenCountryIsoCodes)
            {
                var country = countries.Find(c => c.Iso.Equals(iso, StringComparison.OrdinalIgnoreCase));
                if (country != null)
                {
                    orderedCountries.Add(country);
                }
            }

            // Add remaining countries (not in chosenCountryIsoCodes) in alphabetical order
            var remainingCountries = countries.Except(orderedCountries).OrderBy(c => c.Name);
            orderedCountries.AddRange(remainingCountries);


            TempData["OrderedCountries"] = orderedCountries;
        }
        public async Task<IActionResult> OnPost()
        {
            // Validate Confirm Password
            if (Input.Password != Input.ConfirmPassword)
            {
                ModelState.AddModelError("Input.ConfirmPassword", "The password and confirmation password do not match.");
                await LoadCountriesAsync();
                return Page();
            }

            // Retrieve ISO code from TempData
            var selectedCountry = await _countryRepository.GetCountryByIsoAsync(Input.Country);
            if (selectedCountry == null)
            {
                ModelState.AddModelError("Input.Country", "Invalid country selected.");
                await LoadCountriesAsync(); // Reload the country list
                BuildModel(Input.ReturnUrl);
                return Page();
            }


            // create user & claims
            var userToCreate = new ApplicationUser()
            {
                UserName = Input.UserName,
                GivenName = Input.GivenName,
                FamilyName = Input.FamilyName,
                Subject = Guid.NewGuid().ToString(),
                Email = Input.Email,
                Suspended = true,
                CountryId = selectedCountry.Id
            };
            // Add claims to the user
            var claims = new List<Claim>
            {
                new(JwtClaimTypes.GivenName, Input.GivenName),
                new(JwtClaimTypes.FamilyName, Input.FamilyName),
                new("country", Input.Country)
            };

            // Add the claims to the user
            _localUserService.AddUser(userToCreate,
                Input.Password);
            // Assign the "Admin" role to the user. Later on this must be changed so the user's role can be changed from the admin
            var adminRoleExists = await _roleManager.RoleExistsAsync("Admin");
            var customerRoleExists = await _roleManager.RoleExistsAsync("Customer");

            if (!adminRoleExists)
            {
                var adminRole = new IdentityRole("Admin")
                {
                    // Set the ConcurrencyStamp here (replace with your desired value)
                    ConcurrencyStamp = Guid.NewGuid().ToString() // You can use any unique value here
                };

                await _roleManager.CreateAsync(adminRole);
            }

            if (!customerRoleExists)
            {
                var customerRole = new IdentityRole("Customer")
                {
                    // Set the ConcurrencyStamp here (replace with your desired value)
                    ConcurrencyStamp = Guid.NewGuid().ToString() // You can use any unique value here
                };
                await _roleManager.CreateAsync(customerRole);
            }
            await _userManager.AddToRoleAsync(userToCreate, adminRoleExists ? "Customer" : "Admin"); // Assign the role
            await _userManager.AddClaimsAsync(userToCreate, claims); // Add claims
            var roleClaim = new Claim(ClaimTypes.Role, adminRoleExists ? "Customer" : "Admin");
            await _userManager.AddClaimAsync(userToCreate, roleClaim); // Add role claim

            await _localUserService.SaveChangesAsync(); // Save changes

            // create an activation link - we need an absolute URL, therefore
            // we use Url.PageLink instead of Url.Page
            var activationLink = Url.PageLink("/user/activation/index",
                values: new { securityCode = userToCreate.SecurityCode });
            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

            // Construct the complete activation link
            activationLink = $"{activationLink}";
            var request = new EmailDto
            {
                From = "tpd_bg@yahoo.com",
                FromWhom = "Weather Team",
                To = userToCreate.Email,
                ToWhom = userToCreate.GivenName,
                Subject = "Account Activation",
                Body = $"Please, click the following link to activate your account: <br> {activationLink}"
            };
            _emailService.SendEmail(request);

            //Console.WriteLine(activationLink);
            return Redirect("~/User/ActivationCodeSent");

            //// Issue authentication cookie (log the user in)
            //var isUser = new IdentityServerUser(userToCreate.Subject)
            //{
            //    DisplayName = userToCreate.UserName
            //};
            //await HttpContext.SignInAsync(isUser);

            //// continue with the flow     
            //if (_interaction.IsValidReturnUrl(Input.ReturnUrl) 
            //    || Url.IsLocalUrl(Input.ReturnUrl))
            //{
            //    return Redirect(Input.ReturnUrl);
            //}

            //return Redirect("~/");
        }
    }
}
