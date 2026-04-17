using CCH.Core.DTOs;
using CCH.Core.Entities;
using CCH.Core.Interfaces;
using CCH.Core.Interfaces.Repositories;
using CCH.Services.Repositories;
using CCH.Services.Services;
using Moq;
using Xunit;

namespace CCH.Tests;

/// <summary>
/// Tests for PartService implementation using Relational Repository.
/// (繁體中文) 使用關聯式 Repository 的 PartService 實作測試。
/// </summary>
public class PartServiceTests : IDisposable
{
    private readonly string _testBaseDir;
    private readonly string _testPartsPath;
    private readonly PartService _partService;
    private readonly PartRepository _repository;
    private readonly Mock<IUserContext> _mockUserContext;

    public PartServiceTests()
    {
        _testBaseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Test_Service_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testBaseDir);
        _testPartsPath = Path.Combine(_testBaseDir, "parts.json");

        var mockCommonRepo = new Mock<ICommonRepository>();
        mockCommonRepo.Setup(r => r.GetCustomers()).Returns(new List<CustomerEntity> { new() { ID = 101, Name = "Customer A" } });
        mockCommonRepo.Setup(r => r.GetCountries()).Returns(new List<CountryEntity> { new() { ID = 1, Name = "Taiwan" } });

        _repository = new PartRepository(mockCommonRepo.Object, _testPartsPath);

        _mockUserContext = new Mock<IUserContext>();
        _mockUserContext.Setup(u => u.Role).Returns("admin"); // Use admin to bypass role filters initially

        _partService = new PartService(_repository, _mockUserContext.Object);
    }
    public void Dispose()
    {
        if (Directory.Exists(_testBaseDir))
        {
            Directory.Delete(_testBaseDir, true);
        }
    }

    [Fact]
    public void SearchParts_NoFilters_ReturnsMappedData()
    {
        // Act
        var result = _partService.SearchParts(null, null, null, null, 1, 20);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(17, result.Total);
        Assert.Equal("Customer A", result.Data.First().Customer);
    }
}
