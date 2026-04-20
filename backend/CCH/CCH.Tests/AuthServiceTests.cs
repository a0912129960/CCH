using CCH.Core.Features.Auth.DTOs;
using CCH.Services.Features.Auth;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace CCH.Tests;

/// <summary>
/// Tests for AuthService JWT implementation.
/// (繁體中文) AuthService JWT 實作的測試。
/// </summary>
public class AuthServiceTests
{
    private readonly IConfiguration _configuration;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        var settings = new Dictionary<string, string?>
        {
            { "Jwt:Key", "YourSuperSecretKeyForCCHProject2026" },
            { "Jwt:Issuer", "CCH.API" },
            { "Jwt:Audience", "CCH.Web" },
            { "Jwt:DurationInDays", "1" }
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();

        _authService = new AuthService(_configuration);
    }

    [Fact]
    public void Login_ValidUser_ReturnsToken()
    {
        // Arrange
        var request = new LoginRequest { Username = "customer001", Password = "888888" };

        // Act
        var response = _authService.Login(request);

        // Assert
        Assert.NotNull(response);
        Assert.False(string.IsNullOrEmpty(response.Token));
        Assert.Equal("customer001", response.User.UserId);
        Assert.Equal("customer", response.User.Role);
    }

    [Fact]
    public void Login_InvalidUser_ReturnsNull()
    {
        // Arrange
        var request = new LoginRequest { Username = "wrong_user", Password = "wrong_password" };

        // Act
        var response = _authService.Login(request);

        // Assert
        Assert.Null(response);
    }

    [Fact]
    public void Login_ValidUserDCB_ReturnsCorrectRole()
    {
        // Arrange
        var request = new LoginRequest { Username = "dcb001", Password = "888888" };

        // Act
        var response = _authService.Login(request);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("dcb", response.User.Role);
    }
}
