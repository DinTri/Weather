using Microsoft.AspNetCore.Http;
using Moq;
using System.Data;
using Weather.Api.Data;
using Weather.Api.Entities;
using Weather.Api.IdentityRoleServices;

namespace Weather.ApiTest
{
    public class ServiceTests
    {
        [Fact]
        public async Task CreateUserAsync_ValidUser_ReturnsTrue()
        {
            // Arrange
            var user = new AspNetUsers
            {
                // Initialize user properties here
                Id = "8a063afa-707f-4b0b-bf4e-218b3c91c7c5",
                Subject = "0df152a6-9add-4689-aaaf-d08e1632be33",
                GivenName = "Melissa",
                FamilyName = "Thompson",
                Suspended = false,
                SecurityCodeExpirationDate = Convert.ToDateTime("2023-09-24 09:56:45.5100639"),
                CountryId = 33,
                CreatedAt = DateTime.UtcNow,
                UserName = "MelissaThompson",
                NormalizedUserName = "MelissaThompson".ToUpper(),
                Email = "melissa@gmail.com",
                NormalizedEmail = "melissa@gmail.com".ToUpper(),
                SecurityStamp = "d5394ee1-81e6-48bb-8d18-2cd02ee4a9a9",
                ConcurrencyStamp = "79fb3416-5ac4-4c58-a83c-e429db1ea59b"
            };
            var password = "some_password";
            IFormFile photo = null; // Optional photo

            // Mock the IDapperContext
            var dapperContextMock = new Mock<IDapperContext>();

            // Mock the CreateConnection method to return a valid IDbConnection
            var connectionMock = new Mock<IDbConnection>();
            dapperContextMock.Setup(d => d.CreateConnection()).Returns(connectionMock.Object);

            // Create an instance of your UserService with the mocked IDapperContext
            var userService = new UserService(dapperContextMock.Object);

            // Act: Attempt to create the user
            var result = await userService.CreateUserAsync(user, password, photo);

            // Assert: Ensure the result is true (user creation succeeded)
            Assert.True(result);
        }

        [Fact]
        public async Task FindUserByIdAsync_UserExists_ReturnsUser()
        {
            // Arrange
            const string userId = "ab4e9697-d006-4ba8-9674-0197eafe2048"; // Provide a valid user ID
            var expectedUser = new AspNetUsers
            {
                // Initialize the user properties as needed
                Id = userId,
                UserName = "Lina",
                GivenName = "Lina",
                // Add other properties...
            };

            // Mock the IDapperContext
            var dapperContextMock = new Mock<IDapperContext>();

            // Mock the CreateConnection method to return a valid IDbConnection
            var connectionMock = new Mock<IDbConnection>();
            dapperContextMock.Setup(d => d.CreateConnection()).Returns(connectionMock.Object);


            // Create an instance of UserService with the mocked IDapperContext
            var userService = new UserService(dapperContextMock.Object);

            // Act
            var result = await userService.FindUserByIdAsync(userId);

            // Assert
            Assert.NotNull(result); // Ensure a user is returned
            Assert.Equal(expectedUser.Id, result.Id); // Verify the user's ID
            // Add more assertions for other user properties as needed
        }
    }
}