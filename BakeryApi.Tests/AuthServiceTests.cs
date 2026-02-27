using Xunit;
using BakeryApi.Services;
using BakeryApi.Models.Auth;
using BakeryApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace BakeryApi.Tests;

public class AuthServiceTests
{
    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsToken()
    {
        // Arrange
        var userManagerMock = MockUserManager();

        // Mock configuration with long JWT key
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["Jwt:Key"])
            .Returns("super-secret-test-key-for-unit-test-1234567890!!"); 
        configMock.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
        configMock.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");

        var dto = new LoginDto { Email = "a@b.com", Password = "P@ssw0rd" };

        var testUser = new User
        {
            Id = Guid.NewGuid().ToString(),
            Email = dto.Email,
            UserName = dto.Email
        };

        userManagerMock.Setup(u => u.FindByEmailAsync(dto.Email))
            .ReturnsAsync(testUser);

        userManagerMock.Setup(u => u.CheckPasswordAsync(testUser, dto.Password))
            .ReturnsAsync(true);

        userManagerMock.Setup(u => u.GetRolesAsync(testUser))
            .ReturnsAsync(new List<string> { "User" });

        var service = new AuthService(userManagerMock.Object, configMock.Object);

        // Act
        var result = await service.LoginAsync(dto);

        // Assert
        Assert.NotNull(result.Token);
    }

    [Fact]
    public async Task LoginAsync_InvalidCredentials_ReturnsNull()
    {
        // Arrange
        var userManagerMock = MockUserManager();
        var configMock = new Mock<IConfiguration>();
        var dto = new LoginDto { Email = "x@x.com", Password = "wrong" };

        userManagerMock.Setup(u => u.FindByEmailAsync(dto.Email))
            .ReturnsAsync((User?)null);

        var service = new AuthService(userManagerMock.Object, configMock.Object);

        // Act
        var result = await service.LoginAsync(dto);

        // Assert
        Assert.Null(result.Token);
    }

    private Mock<UserManager<User>> MockUserManager()
    {
        var store = new Mock<IUserStore<User>>();

        var mgr = new Mock<UserManager<User>>(
            store.Object,
            null, 
            null, 
            new IUserValidator<User>[0],
            new IPasswordValidator<User>[0],
            null, 
            null,
            null, 
            null  
        );

        mgr.Setup(m => m.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        mgr.Setup(m => m.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        mgr.Setup(m => m.GetRolesAsync(It.IsAny<User>()))
            .ReturnsAsync(new List<string> { "User" });

        mgr.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((string email) =>
            {
                return new User
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = email,
                    UserName = email
                };
            });

        return mgr;
    }
}