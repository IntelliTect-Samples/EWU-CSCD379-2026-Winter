using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using florist_api.Services;
using florist_api.Models;
using florist_api.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace florist_api.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            
            // FIX: Added ! to all nulls here to satisfy the compiler
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
            
            _mockConfiguration = new Mock<IConfiguration>();

            _mockConfiguration.Setup(c => c["Jwt:Key"]).Returns("a_very_long_secret_key_at_least_32_chars_long");
            _mockConfiguration.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
            _mockConfiguration.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");

            _authService = new AuthService(_mockUserManager.Object, _mockConfiguration.Object);
        }

        [Fact]
        public async Task LoginAsync_ReturnsLoginResponse_WhenCredentialsAreValid()
        {
            // ARRANGE
            var loginRequest = new LoginRequest { Username = "ChloeAdmin", Password = "Password123!" };
            var testUser = new ApplicationUser { UserName = "ChloeAdmin" };

            _mockUserManager.Setup(um => um.FindByNameAsync(loginRequest.Username))
                .ReturnsAsync(testUser);

            _mockUserManager.Setup(um => um.CheckPasswordAsync(testUser, loginRequest.Password))
                .ReturnsAsync(true);

            _mockUserManager.Setup(um => um.GetRolesAsync(testUser))
                .ReturnsAsync(new List<string> { "Admin" });

            // ACT
            var result = await _authService.LoginAsync(loginRequest);

            // ASSERT
            Assert.NotNull(result);
            // FIX: Added ! to result so it doesn't complain about possible nulls
            Assert.Equal("ChloeAdmin", result!.Username);
            Assert.Equal("Admin", result.Role);
            Assert.NotEmpty(result.Token); 
        }

        [Fact]
        public async Task LoginAsync_ReturnsNull_WhenPasswordIsIncorrect()
        {
            // ARRANGE
            var loginRequest = new LoginRequest { Username = "ChloeAdmin", Password = "WrongPassword" };
            var testUser = new ApplicationUser { UserName = "ChloeAdmin" };

            _mockUserManager.Setup(um => um.FindByNameAsync(loginRequest.Username))
                .ReturnsAsync(testUser);
            
            _mockUserManager.Setup(um => um.CheckPasswordAsync(testUser, loginRequest.Password))
                .ReturnsAsync(false); 

            // ACT
            var result = await _authService.LoginAsync(loginRequest);

            // ASSERT
            Assert.Null(result);
        }
    }
}