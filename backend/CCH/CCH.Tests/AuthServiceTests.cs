using CCH.Core.Entities.CSP;
using CCH.Core.Entities.ReSm;
using CCH.Core.Features.Auth.DTOs;
using CCH.Services.Features.Auth;
using CCH.Services.Repositories.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace CCH.Tests;

/// <summary>
/// Tests for AuthService real implementation.
/// (繁體中文) AuthService 真實實作的測試。
/// </summary>
public class AuthServiceTests
{
    private readonly IConfiguration _configuration;
    private readonly ReSmDbContext _reSmContext;
    private readonly CspDbContext _cspContext;
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

        var reSmOptions = new DbContextOptionsBuilder<ReSmDbContext>()
            .UseInMemoryDatabase(databaseName: "ReSmTest")
            .Options;
        _reSmContext = new ReSmDbContext(reSmOptions);

        var cspOptions = new DbContextOptionsBuilder<CspDbContext>()
            .UseInMemoryDatabase(databaseName: "CspTest")
            .Options;
        _cspContext = new CspDbContext(cspOptions);

        SeedTestData();

        _authService = new AuthService(_configuration, _reSmContext, _cspContext);
    }

    private void SeedTestData()
    {
        _reSmContext.Database.EnsureDeleted();
        _cspContext.Database.EnsureDeleted();

        // Seed Internal User
        _reSmContext.SmUser.Add(new SmUser
        {
            UserID = "dimerco001",
            FullName = "Dimerco User",
            Password = CCH.Core.Utilities.DataEncryption.Encryption("888888"),
            Status = "Active",
            Email = "dimerco001@dimerco.com",
            Admin = "N",
            FirstName = "Dimerco",
            LastName = "001",
            StationID = "TPE",
            Title = "Staff"
        });

        // Seed BIT User
        _reSmContext.SmUser.Add(new SmUser
        {
            UserID = "dcb001",
            FullName = "DCB User",
            Password = CCH.Core.Utilities.DataEncryption.Encryption("888888"),
            Status = "Active",
            Email = "dcb001@dimerco.com",
            Admin = "N",
            FirstName = "DCB",
            LastName = "User",
            StationID = "TPE",
            Title = "Staff"
        });
        _reSmContext.SmgroupRoleSetting.Add(new SmgroupRoleSetting
        {
            UserId = "dcb001",
            RoleNo = "BIT",
            Status = "Active"
        });

        // Seed External User
        _reSmContext.SmCustomerContact.Add(new SmCustomerContact
        {
            Hqid = 1001,
            Email = "customer001@client.com",
            FullName = "Customer User",
            Status = "Active"
        });
        _cspContext.CpprojectContactor.Add(new CpprojectContactor
        {
            ContactorHqid = 1001,
            Status = "Active",
            InviteStatus = "Registered",
            Role = "Customer",
            InviteToken = "token",
            LaneType = "All"
        });
        _cspContext.CpprojectContactorPassword.Add(new CpprojectContactorPassword
        {
            ContactorHqid = 1001,
            Password = BCrypt.Net.BCrypt.HashPassword("888888"),
            CreatedBy = "System",
            UpdatedBy = "System"
        });

        _reSmContext.SaveChanges();
        _cspContext.SaveChanges();
    }

    [Fact]
    public void Login_ValidInternalUser_ReturnsToken()
    {
        // Arrange
        var request = new LoginRequest { Username = "dimerco001", Password = "888888" };

        // Act
        var response = _authService.Login(request);

        // Assert
        Assert.NotNull(response);
        Assert.False(string.IsNullOrEmpty(response.Token));
        Assert.Equal("dimerco001", response.User.UserId);
        Assert.Equal("dimerco", response.User.Role);
    }

    [Fact]
    public void Login_ValidExternalUser_ReturnsToken()
    {
        // Arrange
        var request = new LoginRequest { Username = "customer001@client.com", Password = "888888" };

        // Act
        var response = _authService.Login(request);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("1001", response.User.UserId);
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

    [Fact]
    public void Login_UserInDCBStation_ReturnsDCBRole()
    {
        // Arrange
        // Seed a user that is not BIT but belongs to a DCB station
        _reSmContext.SmUser.Add(new SmUser
        {
            UserID = "dcb_station_user",
            FullName = "DCB Station User",
            Password = CCH.Core.Utilities.DataEncryption.Encryption("888888"),
            Status = "Active",
            Email = "dcb_station@dimerco.com",
            Admin = "N",
            FirstName = "DCB",
            LastName = "Station",
            StationID = "CHI", // Chicago
            Title = "Staff"
        });

        _reSmContext.SmStation.Add(new SmStation
        {
            StationId = "ORD",
            StationCode = "DCBORD",
            StationName = "Chicago DCB"
        });

        _reSmContext.SmBusinessEntity.Add(new SmBusinessEntity
        {
            StationId = "CHI",
            ParentId = "ORD",
            Version = new byte[8],
            EffectiveDate = DateTime.Now,
            CreatedDate = DateTime.Now
        });

        _reSmContext.SaveChanges();

        var request = new LoginRequest { Username = "dcb_station_user", Password = "888888" };

        // Act
        var response = _authService.Login(request);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("dcb", response.User.Role);
    }

    [Fact]
    public void Login_UrlEncodedPassword_ReturnsToken()
    {
        // Arrange
        var request = new LoginRequest { Username = "dimerco001", Password = "888%38%38%38" }; // "888888"

        // Act
        var response = _authService.Login(request);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("dimerco001", response.User.UserId);
    }
}
