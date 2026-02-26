using Xunit;
using BakeryApi.Services;
using BakeryApi.Models.Auth;
using BakeryApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;
using System;

namespace BakeryApi.Tests;

public class AuthServiceTests
{
    [Fact]
    public async Task RegisterAsync_ValidDto_ReturnsSuccess()
    {
        // Arrange
        var userManagerMock = MockUserManager();
        var configMock = new Mock<IConfiguration>();

        var dto = new RegisterDto { Email = "a@b.com", Password = "P@ssw0rd" };

        userManagerMock.Setup(u => u.CreateAsync(It.IsAny<User>(), dto.Password))
            .ReturnsAsync(IdentityResult.Success);
        userManagerMock.Setup(u => u.AddToRoleAsync(It.IsAny<User>(), "Admin"))
            .ReturnsAsync(IdentityResult.Success);

        var service = new AuthService(userManagerMock.Object, configMock.Object);

        // Act
        var result = await service.RegisterAsync(dto);

        // Assert
        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task LoginAsync_InvalidCredentials_ReturnsNull()
    {
        var userManagerMock = MockUserManager();
        var configMock = new Mock<IConfiguration>();

        var dto = new LoginDto { Email = "x@x.com", Password = "wrong" };

        userManagerMock.Setup(u => u.FindByEmailAsync(dto.Email)).ReturnsAsync((User?)null);

        var service = new AuthService(userManagerMock.Object, configMock.Object);

        var result = await service.LoginAsync(dto);

        Assert.Null(result.Token);
    }

    private Mock<UserManager<User>> MockUserManager()
    {
        var store = new Mock<IUserStore<User>>();
        var mgr = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
        return mgr;
    }
}
