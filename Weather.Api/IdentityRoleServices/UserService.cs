using Dapper;
using Serilog;
using System.Data;
using System.Security.Claims;
using Weather.Api.Data;
using Weather.Api.Entities;
using Weather.Api.Entities.Models;

namespace Weather.Api.IdentityRoleServices
{
    public class UserService : IUserService
    {

        private readonly IDapperContext _dapperContext;

        public UserService(IDapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<bool> CreateUserAsync(AspNetUsers user, string password, IFormFile? photo)
        {

            using var connection = _dapperContext.CreateConnection(); // Create a connection

            try
            {
                var hashedPassword = Utils.PasswordHasher.HashPassword(password);
                user.PasswordHash = hashedPassword;
                user.NormalizedUserName = user.UserName?.ToUpper();
                user.NormalizedEmail = user.Email?.ToUpper();
                var random = new Random();
                string securityCode = random.Next(100000, 999999).ToString();

                user.SecurityCode = securityCode;
                user.SecurityCodeExpirationDate = DateTime.UtcNow.AddHours(24);

                // Check if a photo was uploaded
                if (photo is { Length: > 0 })
                {
                    // Generate a unique file name for the photo based on the original file name
                    var uniqueFileName = $"{user.Id}_{Path.GetFileName(photo.FileName)}";

                    // Define the path to save the uploaded photo
                    var filePath = Path.Combine("wwwroot", "images", "profile", uniqueFileName);

                    // Save the photo to the specified path
                    await using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await photo.CopyToAsync(stream);
                    }

                    // Store the photo path in the database
                    user.ProfilePhotoPath = Path.Combine("images", "profile", uniqueFileName);
                }

                const string insertUserSql = @"
            INSERT INTO AspNetUsers (Id, Subject, GivenName, FamilyName, PasswordHash, Suspended, ProfilePhotoPath, 
                                    SecurityCode, SecurityCodeExpirationDate, CountryId, CreatedAt, UpdatedAt, 
                                    DeletedAt, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, 
                                    PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount) 
            VALUES (@Id, @Subject, @GivenName, @FamilyName, @PasswordHash, @Suspended, @ProfilePhotoPath, 
                    @SecurityCode, @SecurityCodeExpirationDate, @CountryId, @CreatedAt, @UpdatedAt, @DeletedAt, 
                    @UserName, @NormalizedUserName, @Email, @NormalizedEmail, @EmailConfirmed, @PhoneNumber, 
                    @PhoneNumberConfirmed, @TwoFactorEnabled, @LockoutEnabled, @AccessFailedCount)";

                var result = 0; // Initialize result variable

                result = await connection.ExecuteAsync(insertUserSql, user);



                if (result == 1)
                {
                    await AddClaimsForUserAsync(user);
                    Log.Information("User created successfully: {@User}", user);
                    return true;
                }
                Log.Error("Failed to create user: {@User}", user);

                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error creating user: {ex.Message}");
                return false;
            }
            finally
            {
                // Close the connection to release resources
                connection.Close();
            }
        }

        private async Task AddClaimsForUserAsync(AspNetUsers user)
        {
            using var connection = _dapperContext.CreateConnection();
            // Define SQL queries to insert claims for GivenName, FamilyName, and Country
            const string insertGivenNameClaimSql = "INSERT INTO AspNetUserClaims (UserId, ClaimType, ClaimValue) VALUES (@UserId, 'GivenName', @GivenName)";
            const string insertFamilyNameClaimSql = "INSERT INTO AspNetUserClaims (UserId, ClaimType, ClaimValue) VALUES (@UserId, 'FamilyName', @FamilyName)";
            const string insertCountryClaimSql = "INSERT INTO AspNetUserClaims (UserId, ClaimType, ClaimValue) VALUES (@UserId, 'Country', @Country)";

            try
            {
                // Execute the SQL queries to insert claims using Dapper
                await connection.ExecuteAsync(insertGivenNameClaimSql, new { UserId = user.Id, GivenName = user.GivenName });
                await connection.ExecuteAsync(insertFamilyNameClaimSql, new { UserId = user.Id, FamilyName = user.FamilyName });
                await connection.ExecuteAsync(insertCountryClaimSql, new { UserId = user.Id, Country = user.CountryId.ToString() });

                Log.Information("Claims added successfully for user: {@User}", user);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error adding claims for user: {@User}", user);
            }
        }
        public async Task<AspNetUsers?> FindUserByIdAsync(string? userId)
        {
            try
            {
                const string selectUserByIdSql = "SELECT * FROM AspNetUsers WHERE Id = @UserId";

                using var connection = _dapperContext.CreateConnection();
                var user = await connection.QuerySingleOrDefaultAsync<AspNetUsers>(selectUserByIdSql, new { UserId = userId });
                if (user != null)
                {
                    Log.Information("User found by Id: {@User}", user);
                }
                else
                {
                    Log.Warning("User not found for Id: {UserId}", userId);
                }
                return user;
            }
            catch (Exception? ex)
            {
                Log.Error(ex, "Error while finding user by Id: {UserId}", userId);
                return null;
            }
        }

        public async Task<AspNetUsers?> GetUserByUsernameAsync(string username)
        {
            try
            {
                // Define the SQL query to select a user by username
                const string selectUserSql = "SELECT * FROM AspNetUsers WHERE UserName = @Username";

                // Execute the SQL query using Dapper and provide the username as a parameter
                using var connection = _dapperContext.CreateConnection();
                var user = await connection.QuerySingleOrDefaultAsync<AspNetUsers>(selectUserSql, new { Username = username });
                if (user != null)
                {
                    Log.Information("User found by Name: {@UserName}", user);
                }
                else
                {
                    Log.Warning("User not found for Name: {UserName}", username);
                }
                return user;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error retrieving user by username: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> UploadProfilePhotoAsync(string userId, IFormFile? photo)
        {
            try
            {
                var user = FindUserByIdAsync(userId);

                // Check if a new photo was uploaded
                if (photo is not { Length: > 0 }) return false; // No new photo uploaded
                // Generate a unique file name for the new photo based on the original file name
                var uniqueFileName = $"{user.Id}_{Path.GetFileName(photo.FileName)}";
                var targetDirectory = Path.Combine("c:", "myimages", "profile");

                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }

                // Define the path to save the uploaded photo
                var filePath = Path.Combine(targetDirectory, uniqueFileName);

                // Save the new photo to the specified path
                await using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }

                // Update the user's profile photo path in the database using Dapper
                const string updatePhotoSql = @"
                UPDATE AspNetUsers
                SET ProfilePhotoPath = @ProfilePhotoPath
                WHERE Id = @UserId";

                using var connection = _dapperContext.CreateConnection();
                var affectedRows = await connection.ExecuteAsync(updatePhotoSql, new { UserId = userId, ProfilePhotoPath = Path.Combine("myimages", "profile", uniqueFileName) });

                return affectedRows > 0; // Check if the update was successful

            }
            catch (Exception ex)
            {
                Log.Error($"Error uploading profile photo for user '{userId}': {ex.Message}");
                return false;
            }
        }



        public async Task<bool> SuspendUser(string userId)
        {
            try
            {
                const string suspendUserSql = "UPDATE AspNetUsers SET Suspended = 1, UpdatedAt = GETUTCDATE() WHERE Id = @UserId";
                using var connection = _dapperContext.CreateConnection();
                var affectedRows = await connection.ExecuteAsync(suspendUserSql, new { UserId = userId });

                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error suspending user: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> UnSuspendUser(string userId)
        {
            try
            {
                const string unsuspendUserSql = "UPDATE AspNetUsers SET Suspended = 0, UpdatedAt = GETUTCDATE() \r\n WHERE Id = @UserId";
                using var connection = _dapperContext.CreateConnection();
                var affectedRows = await connection.ExecuteAsync(unsuspendUserSql, new { UserId = userId });

                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error unsuspending user: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> IsUserLockedAsync(string userId)
        {
            try
            {
                const string selectLockoutEnabledSql = "SELECT LockoutEnabled FROM AspNetUsers WHERE Id = @UserId";
                using var connection = _dapperContext.CreateConnection();

                // Execute the query and retrieve the LockoutEnd value
                var lockoutEnabled = await connection.ExecuteScalarAsync<bool>(selectLockoutEnabledSql, new { UserId = userId });

                return lockoutEnabled;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error checking user lock status: {ex.Message}");
                return false;
            }
        }


        public async Task<bool> IsUserSuspended(string userId)
        {
            try
            {
                const string selectSuspendedSql = "SELECT Suspended FROM AspNetUsers WHERE Id = @UserId";
                using var connection = _dapperContext.CreateConnection();
                var isSuspended = await connection.ExecuteScalarAsync<bool>(selectSuspendedSql, new { UserId = userId });

                return isSuspended;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error checking user suspension status: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> IsTwoFactorEnabledAsync(string userId)
        {
            try
            {
                const string selectTwoFactorEnabledSql = "SELECT TwoFactorEnabled FROM AspNetUsers WHERE Id = @UserId";
                using var connection = _dapperContext.CreateConnection();

                // Execute the query and retrieve the LockoutEnd value
                var twoFactorEnabled = await connection.ExecuteScalarAsync<bool>(selectTwoFactorEnabledSql, new { UserId = userId });

                return twoFactorEnabled;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error checking user TwoFactorEnabled status: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> EnableTwoFactorAsync(string userId)
        {
            try
            {
                const string enableTwoFactorSql = "UPDATE AspNetUsers SET TwoFactorEnabled = 1 WHERE Id = @UserId";
                using var connection = _dapperContext.CreateConnection();
                await connection.ExecuteAsync(enableTwoFactorSql, new { UserId = userId });
                return true; // Successfully enabled Two-Factor Authentication
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error enabling Two-Factor Authentication: {ex.Message}");
                return false; // Failed to enable Two-Factor Authentication
            }
        }

        public async Task<bool> DisableTwoFactorAsync(string userId)
        {
            try
            {
                const string disableTwoFactorSql = "UPDATE AspNetUsers SET TwoFactorEnabled = 0 WHERE Id = @UserId";
                using var connection = _dapperContext.CreateConnection();
                await connection.ExecuteAsync(disableTwoFactorSql, new { UserId = userId });
                return true; // Successfully disabled Two-Factor Authentication
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error disabling Two-Factor Authentication: {ex.Message}");
                return false; // Failed to disable Two-Factor Authentication
            }
        }

        public async Task<bool> UpdateUserAsync(AspNetUsers user, IFormFile? photo, string? userName, string? familyName, string? email, string? phoneNumber, bool suspended, bool twoFactorEnabled, bool lockoutEnabled)
        {
            using var connection = _dapperContext.CreateConnection();

            try
            {
                // Check if a new photo was uploaded
                if (photo is { Length: > 0 })
                {
                    // Generate a unique file name for the new photo based on the original file name
                    var uniqueFileName = $"{user.Id}_{Path.GetFileName(photo.FileName)}";
                    var targetDirectory = Path.Combine("c:", "myimages", "profile");

                    if (!Directory.Exists(targetDirectory))
                    {
                        Directory.CreateDirectory(targetDirectory);
                    }

                    // Define the path to save the uploaded photo
                    var filePath = Path.Combine(targetDirectory, uniqueFileName);

                    // Save the new photo to the specified path
                    await using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await photo.CopyToAsync(stream);
                    }

                    // Update the user's profile photo path
                    user.ProfilePhotoPath = Path.Combine("myimages", "profile", uniqueFileName);
                }

                // Set the provided values to update
                user.UserName = userName;
                user.FamilyName = familyName;
                user.Email = email;
                user.PhoneNumber = phoneNumber;
                user.Suspended = suspended;
                user.TwoFactorEnabled = twoFactorEnabled;
                user.LockoutEnabled = lockoutEnabled;
                user.UpdatedAt = DateTime.UtcNow;
                user.LockoutEnd = DateTimeOffset.UtcNow;
                user.NormalizedUserName = userName.ToUpper();
                user.NormalizedEmail = email.ToUpper();
                // Define the SQL query to update the user record with ConcurrencyStamp check
                const string updateUserSql = @"
            UPDATE AspNetUsers 
            SET UserName = @UserName, NormalizedUserName = @NormalizedUserName, FamilyName = @FamilyName, Email = @Email, NormalizedEmail = @NormalizedEmail, PhoneNumber = @PhoneNumber, 
                Suspended = @Suspended, TwoFactorEnabled = @TwoFactorEnabled,
                LockoutEnabled = @LockoutEnabled, LockoutEnd = @LockoutEnd,
                ProfilePhotoPath = @ProfilePhotoPath, UpdatedAt = @UpdatedAt 
            WHERE Id = @Id AND ConcurrencyStamp = @ConcurrencyStamp";

                // Execute the SQL query using Dapper and provide the updated user information as parameters
                var result = await connection.ExecuteAsync(updateUserSql, user);

                if (result == 1)
                {
                    Log.Information($"User updated successfully: {user.UserName}");
                    return true;
                }

                Log.Error($"Error updating user: {user.UserName}. No rows were affected.");
                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error updating user: {user.UserName}");
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(AspNetUsers user)
        {
            using var connection = _dapperContext.CreateConnection();
            try
            {
                // Delete the claims associated with the user
                const string deleteClaimsSql = "DELETE FROM AspNetUserClaims WHERE UserId = @UserId";
                await connection.ExecuteAsync(deleteClaimsSql, new { UserId = user.Id });

                // Delete the user
                const string deleteUserSql = "DELETE FROM AspNetUsers WHERE Id = @Id";
                var result = await connection.ExecuteAsync(deleteUserSql, new { Id = user.Id });

                if (result == 1)
                {
                    Log.Information($"User '{user.UserName}' with ID '{user.Id}' has been deleted at '{DateTime.UtcNow}'.");
                    return true;
                }

                Log.Error($"Failed to delete user '{user.UserName}' with ID '{user.Id}'  at '{DateTime.UtcNow}'.");
                return false;
            }
            catch (Exception ex)
            {
                Log.Error($"Error deleting user '{user.UserName}' with ID '{user.Id}': {ex}");
                return false;
            }
        }

        public async Task<List<AspNetUsers>> GetAllUsersAsync()
        {
            const string query = "SELECT * FROM AspNetUsers";
            using var connection = _dapperContext.CreateConnection();
            var users = await connection.QueryAsync<AspNetUsers>(query);

            return users.ToList();
        }

        public async Task<bool> LockUser(string userId)
        {
            try
            {
                const string lockUserSql = "UPDATE AspNetUsers SET LockoutEnabled = 1, UpdatedAt = GETUTCDATE()  WHERE Id = @UserId";
                using var connection = _dapperContext.CreateConnection();
                var affectedRows = await connection.ExecuteAsync(lockUserSql, new { UserId = userId });

                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error locking user: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UnlockUser(string userId)
        {
            try
            {
                // Set LockoutEnd to a future date, e.g., 1 hour from now
                var lockoutEnd = DateTime.UtcNow.AddHours(1);

                const string unlockUserSql = "UPDATE AspNetUsers SET LockoutEnabled = 0, LockoutEnd = @LockoutEnd, UpdatedAt = GETUTCDATE() WHERE Id = @UserId";
                using var connection = _dapperContext.CreateConnection();
                var affectedRows = await connection.ExecuteAsync(unlockUserSql, new { UserId = userId, LockoutEnd = lockoutEnd });

                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error unlocking user: {ex.Message}");
                return false;
            }
        }


        public async Task<IEnumerable<string>> GetUserRolesAsync(AspNetUsers user)
        {
            // Retrieve the roles for the user
            const string query = @"
            SELECT R.Name
            FROM AspNetRoles R
            INNER JOIN AspNetUserRoles UR ON R.Id = UR.RoleId
            WHERE UR.UserId = @UserId";
            using var connection = _dapperContext.CreateConnection();
            return await connection.QueryAsync<string>(query, new { UserId = user.Id });
        }

        public async Task<IList<Claim>> GetUserClaimsAsync(AspNetUsers user)
        {
            const string query = @"
            SELECT UserId,ClaimType, ClaimValue
            FROM AspNetUserClaims
            WHERE UserId = @UserId";

            using var connection = _dapperContext.CreateConnection();
            var userClaims = await connection.QueryAsync<UserClaimResponse>(query, new { UserId = user.Id });

            // Map the UserClaim objects to Claim objects
            var claims = userClaims.Select(uc => new Claim(uc.ClaimType, uc.ClaimValue)).ToList();

            return claims;
        }

        public async Task<bool> IsUserInRoleAsync(AspNetUsers user, string roleName)
        {
            // Check if the user is in the specified role
            const string query = @"
            SELECT 1
            FROM AspNetRoles R
            INNER JOIN AspNetUserRoles UR ON R.Id = UR.RoleId
            WHERE UR.UserId = @UserId AND R.Name = @RoleName";
            using var connection = _dapperContext.CreateConnection();
            var result = await connection.ExecuteScalarAsync<int>(query, new { UserId = user.Id, RoleName = roleName });

            return result == 1;
        }

        public async Task<Country> GetCountryByUserIdAsync(string userId)
        {
            // Define the SQL query to retrieve the country by userId using a join
            string sql = @"
            SELECT c.Id, c.Iso, c.Iso3, c.Name, c.Numcode, c.PhoneCode
            FROM Countries c
            JOIN AspNetUsers u ON c.Id = u.CountryId
            WHERE u.Id = @UserId";

            // Execute the query and retrieve the country
            using var connection = _dapperContext.CreateConnection();
            var country = await connection.QueryFirstOrDefaultAsync<Country>(sql, new { UserId = userId });

            return country;
        }

        public async Task<IList<AspNetRoles>> GetAllRolesAsync()
        {
            // Retrieve all roles
            const string query = "SELECT * FROM AspNetRoles";
            using var connection = _dapperContext.CreateConnection();
            return (await connection.QueryAsync<AspNetRoles>(query)).ToList();
        }

        public async Task<AddUserToRoleResult> AddUserToRoleAsync(AspNetUsers user, string? roleName)
        {
            using var connection = _dapperContext.CreateConnection();

            try
            {
                // Check if the user and role exist
                var userId = await connection.QueryFirstOrDefaultAsync<string>(
                    "SELECT Id FROM AspNetUsers WHERE Id = @UserId", new { UserId = user.Id });

                var roleId = await connection.QueryFirstOrDefaultAsync<string>(
                    "SELECT Id FROM AspNetRoles WHERE Name = @RoleName", new { RoleName = roleName });

                if (userId == null || roleId == null)
                {
                    // User or role not found, set the failureReason and return a failure result
                    return new AddUserToRoleResult
                    {
                        Succeeded = false,
                        FailureReason = "User or role not found"
                    };
                }
                // Check if the user already has the role
                var userHasRole = await connection.ExecuteScalarAsync<bool>(
                    "SELECT 1 FROM AspNetUserRoles WHERE UserId = @UserId AND RoleId = @RoleId",
                    new { UserId = userId, RoleId = roleId });

                if (userHasRole)
                {
                    // User already has the role; set the failureReason and return a failure result
                    return new AddUserToRoleResult
                    {
                        Succeeded = false,
                        FailureReason = $"User already has the role: {roleName}"
                    };
                }

                // Insert the user-role relationship
                await connection.ExecuteAsync(
                    "INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES (@UserId, @RoleId)",
                    new { UserId = userId, RoleId = roleId });

                // Insert the "role" claim for the user into the AspNetUserClaims table
                var claimType = "role";
                var claimValue = roleName;

                await connection.ExecuteAsync(
                    "INSERT INTO AspNetUserClaims (UserId, ClaimType, ClaimValue) " +
                    "VALUES (@UserId, @ClaimType, @ClaimValue)",
                    new { UserId = userId, ClaimType = claimType, ClaimValue = claimValue });

                // Operation succeeded, set failureReason to null
                return new AddUserToRoleResult
                {
                    Succeeded = true,
                    FailureReason = null
                };
            }
            catch (Exception ex)
            {
                Log.Error($"Error adding user to role: {ex.Message}");
                return new AddUserToRoleResult
                {
                    Succeeded = false,
                    FailureReason = $"Error adding user to role"
                };
            }
        }



        public async Task<List<AddUserToRoleResult>> AddUserToRolesAsync(AspNetUsers user, IEnumerable<string> roleNames)
        {
            using var connection = _dapperContext.CreateConnection();
            var results = new List<AddUserToRoleResult>();

            try
            {
                // Check if the user exists
                var userId = await connection.QueryFirstOrDefaultAsync<string>(
                    "SELECT Id FROM AspNetUsers WHERE Id = @UserId", new { UserId = user.Id });

                if (userId == null)
                {
                    Log.Warning($"User not found for Name: {user}");
                    results.Add(new AddUserToRoleResult
                    {
                        Succeeded = false,
                        FailureReason = "User not found"
                    });
                }
                else
                {
                    foreach (var roleName in roleNames)
                    {
                        // Check if the role exists
                        var roleId = await connection.QueryFirstOrDefaultAsync<string>(
                            "SELECT Id FROM AspNetRoles WHERE Name = @RoleName", new { RoleName = roleName });

                        if (roleId == null)
                        {
                            // Role not found; you can handle this according to your requirements, e.g., log an error
                            Log.Error($"Role not found: {roleName}");
                            results.Add(new AddUserToRoleResult
                            {
                                Succeeded = false,
                                FailureReason = $"Role not found: {roleName}"
                            });
                        }
                        else
                        {
                            // Check if the user already has the role
                            var userHasRole = await connection.ExecuteScalarAsync<bool>(
                                "SELECT 1 FROM AspNetUserRoles WHERE UserId = @UserId AND RoleId = @RoleId",
                                new { UserId = userId, RoleId = roleId });

                            if (userHasRole)
                            {
                                // User already has the role; you can handle this accordingly
                                Log.Warning($"User already has the role: {roleName}");
                                results.Add(new AddUserToRoleResult
                                {
                                    Succeeded = false,
                                    FailureReason = $"User already has the role: {roleName}"
                                });
                            }
                            else
                            {
                                // Insert the user-role relationship
                                await connection.ExecuteAsync(
                                    "INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES (@UserId, @RoleId)",
                                    new { UserId = userId, RoleId = roleId });

                                // Insert the "role" claim for the user into the AspNetUserClaims table
                                var claimType = "role";
                                var claimValue = roleName;

                                await connection.ExecuteAsync(
                                    "INSERT INTO AspNetUserClaims (UserId, ClaimType, ClaimValue) " +
                                    "VALUES (@UserId, @ClaimType, @ClaimValue)",
                                    new { UserId = userId, ClaimType = claimType, ClaimValue = claimValue });

                                results.Add(new AddUserToRoleResult
                                {
                                    Succeeded = true
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error adding roles and role claims to user: {ex.Message}");
                results.Add(new AddUserToRoleResult
                {
                    Succeeded = false,
                    FailureReason = $"Error adding roles and role claims to user: {ex.Message}"
                });
            }

            return results;
        }



        public async Task<bool> RemoveUserFromRoleAsync(AspNetUsers user, string? roleName)
        {
            using var connection = _dapperContext.CreateConnection();

            try
            {
                // Check if the user exists
                var userId = await connection.QueryFirstOrDefaultAsync<string>(
                    "SELECT Id FROM AspNetUsers WHERE Id = @UserId", new { UserId = user.Id });

                if (userId == null)
                {
                    // User not found; handle according to your requirements, e.g., log a warning
                    Log.Warning($"User not found for Name: {user}");
                    return false;
                }

                // Check if the role exists
                var roleId = await connection.QueryFirstOrDefaultAsync<string>(
                    "SELECT Id FROM AspNetRoles WHERE Name = @RoleName", new { RoleName = roleName });

                if (roleId == null)
                {
                    // Role not found; handle according to your requirements, e.g., log an error
                    Log.Error($"Role not found: {roleName}");
                    return false;
                }

                // Remove the user-role relationship
                await connection.ExecuteAsync(
                    "DELETE FROM AspNetUserRoles WHERE UserId = @UserId AND RoleId = @RoleId",
                    new { UserId = userId, RoleId = roleId });

                // Remove the claims for the role from AspNetUserClaims
                await connection.ExecuteAsync(
                    "DELETE FROM AspNetUserClaims WHERE UserId = @UserId AND ClaimType = 'role' AND ClaimValue = @RoleName",
                    new { UserId = userId, RoleName = roleName });

                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"Error removing user from role: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RemoveRolesFromUserAsync(AspNetUsers user, IEnumerable<string> roles)
        {
            using var connection = _dapperContext.CreateConnection();

            try
            {
                // Check if the user exists
                var userId = await connection.QueryFirstOrDefaultAsync<string>(
                    "SELECT Id FROM AspNetUsers WHERE Id = @UserId", new { UserId = user.Id });

                if (userId == null)
                {
                    // User not found; handle according to your requirements, e.g., log a warning
                    Log.Warning($"User not found for Name: {user}");
                    return false;
                }

                foreach (var roleName in roles)
                {
                    // Check if the role exists
                    var roleId = await connection.QueryFirstOrDefaultAsync<string>(
                        "SELECT Id FROM AspNetRoles WHERE Name = @RoleName", new { RoleName = roleName });

                    if (roleId == null)
                    {
                        // Role not found; handle according to your requirements, e.g., log an error
                        Log.Error($"Role not found: {roleName}");
                        continue; // Skip to the next role
                    }

                    // Remove the user-role relationship
                    await connection.ExecuteAsync(
                        "DELETE FROM AspNetUserRoles WHERE UserId = @UserId AND RoleId = @RoleId",
                        new { UserId = userId, RoleId = roleId });

                    // Remove the claims associated with the role from AspNetUserClaims
                    await connection.ExecuteAsync(
                        "DELETE FROM AspNetUserClaims WHERE UserId = @UserId AND ClaimType = 'role' AND ClaimValue = @RoleName",
                        new { UserId = userId, RoleName = roleName }
                        );
                }

                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"Error removing roles from user: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> CreateRoleAsync(string roleName)
        {
            try
            {
                // Check if the role already exists
                using var connection = _dapperContext.CreateConnection();
                var existingRole = await connection.QuerySingleOrDefaultAsync<AspNetRoles>(
                    "SELECT * FROM AspNetRoles WHERE NormalizedName = @NormalizedName",
                    new { NormalizedName = roleName.ToUpper() });

                if (existingRole != null)
                {
                    // Role with the same name already exists
                    Log.Warning($"Role '{roleName}' already exists.");
                    return false;
                }

                // Create a new role object with generated Id and ConcurrencyStamp
                var role = new AspNetRoles
                {
                    Name = roleName,
                    NormalizedName = roleName.ToUpper() // Generate a new concurrency stamp
                };

                // Define SQL query to insert the new role
                const string insertRoleSql = @"
            INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp) 
            VALUES (@Id, @Name, @NormalizedName, @ConcurrencyStamp)";

                // Execute the SQL query using Dapper and provide the role information as parameters
                var result = await connection.ExecuteAsync(insertRoleSql, role);

                if (result == 1)
                {
                    // Role created successfully
                    Log.Information($"Role '{roleName}' created successfully.");
                    return true;
                }

                // Role creation failed
                Log.Error($"Failed to create role '{roleName}'.");
                return false;
            }
            catch (Exception ex)
            {
                // Handle exceptions and log errors
                Log.Error($"Error creating role '{roleName}': {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteRoleAsync(AspNetRoles role)
        {
            try
            {
                // Check if the role is assigned to any users
                const string checkUserRoleSql = "SELECT COUNT(*) FROM AspNetUserRoles WHERE RoleId = @RoleId";
                using var connection = _dapperContext.CreateConnection();
                var userCount = await connection.ExecuteScalarAsync<int>(checkUserRoleSql, new { RoleId = role.Id });

                if (userCount > 0)
                {
                    // Role is assigned to users, cannot be deleted
                    Log.Warning($"Role '{role.Name}' is assigned to {userCount} user(s) and cannot be deleted.");
                    return false;
                }

                // Define SQL query to delete the role by Id
                const string deleteRoleSql = "DELETE FROM AspNetRoles WHERE Id = @Id";

                // Execute the SQL query using Dapper and provide the role's Id as a parameter
                var result = await connection.ExecuteAsync(deleteRoleSql, new { Id = role.Id });

                if (result == 1)
                {
                    // Role deleted successfully
                    Log.Information($"Role '{role.Name}' deleted successfully.");
                    return true;
                }

                // Role deletion failed
                Log.Error($"Failed to delete role '{role.Name}'.");
                return false;
            }
            catch (Exception ex)
            {
                // Handle exceptions and log errors
                Log.Error($"Error deleting role '{role.Name}': {ex.Message}");
                return false;
            }
        }


        public async Task<bool> UpdateRoleAsync(AspNetRoles role)
        {
            using var connection = _dapperContext.CreateConnection();
            try
            {
                // Update the role in AspNetRoles
                var affectedRows = await connection.ExecuteAsync(
                    "UPDATE AspNetRoles SET Name = @Name, NormalizedName = @NormalizedName, ConcurrencyStamp = @ConcurrencyStamp " +
                    "WHERE Id = @Id",
                    role);

                if (affectedRows == 0)
                {
                    // Role not found, return false
                    Log.Warning($"Role '{role.Name}' not found for update.");
                    return false;
                }

                // Update user roles for the updated role
                await connection.ExecuteAsync(
                    "UPDATE AspNetUserRoles SET RoleId = @Id WHERE RoleId = @Id",
                    role);

                // Update role claims for users in AspNetUserClaims
                await connection.ExecuteAsync(
                    "UPDATE AspNetUserClaims SET ClaimValue = @Name WHERE ClaimType = 'role' AND ClaimValue = @Name",
                    role);

                Log.Information($"Role '{role.Name}' updated successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"Error updating role '{role.Name}': {ex.Message}");
                return false;
            }
        }


        public async Task<AspNetRoles?> GetRoleByIdAsync(string roleId)
        {
            const string query = "SELECT * FROM AspNetRoles WHERE Id = @RoleId";
            using var connection = _dapperContext.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<AspNetRoles>(query, new { RoleId = roleId });
        }

        public async Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName)
        {
            // Retrieve users who belong to a specific role
            const string query = @"
            SELECT U.*
            FROM AspNetUsers U
            INNER JOIN AspNetUserRoles UR ON U.Id = UR.UserId
            INNER JOIN AspNetRoles R ON UR.RoleId = R.Id
            WHERE R.Name = @RoleName";
            using var connection = _dapperContext.CreateConnection();
            return (await connection.QueryAsync<ApplicationUser>(query, new { RoleName = roleName })).ToList();
        }

        public async Task<IList<Country>> GetAllCountriesAsync()
        {
            const string query = "SELECT * FROM Countries";
            using var connection = _dapperContext.CreateConnection();
            return (await connection.QueryAsync<Country>(query)).AsList();
        }

        public async Task<Country?> GetCountryByIdAsync(int countryId)
        {
            const string query = "SELECT * FROM Countries WHERE Id = @CountryId";
            using var connection = _dapperContext.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<Country>(query, new { CountryId = countryId });
        }
    }
}
