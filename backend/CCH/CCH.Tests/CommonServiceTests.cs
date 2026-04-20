using CCH.Core.Entities;
using CCH.Core.Interfaces.Repositories;
using CCH.Services.Features.Common;
using Moq;
using Xunit;

namespace CCH.Tests;

/// <summary>
/// Tests for CommonService with CommonRepository.
/// (繁體中文) 結合 CommonRepository 的 CommonService 測試。
/// </summary>
public class CommonServiceTests
{
    private readonly Mock<ICommonRepository> _mockRepo;
    private readonly CommonService _service;

    public CommonServiceTests()
    {
        _mockRepo = new Mock<ICommonRepository>();
        _service = new CommonService(_mockRepo.Object);
    }

    [Fact]
    public void GetStatus_ReturnsMappedStatusesFromRepository()
    {
        // Arrange
        var mockStatuses = new List<StatusEntity>
        {
            new() { Code = "S01", Description = "New" },
            new() { Code = "S02", Description = "Pending Dimerco Review" }
        };
        _mockRepo.Setup(r => r.GetStatuses()).Returns(mockStatuses);

        // Act
        var result = _service.GetStatus();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Equal("S01", result.First().Key);
        Assert.Equal("New", result.First().Value);
        Assert.Equal("Pending Dimerco Review", result.Last().Value);
    }

    [Fact]
    public void GetCustomers_ReturnsMappedCustomersFromRepository()
    {
        // ... (previous code)
    }

    [Fact]
    public void GetSuppliers_WithCustomerId_ReturnsFilteredSuppliers()
    {
        // Arrange
        var mockSuppliers = new List<SupplierEntity>
        {
            new() { ID = 1, CustomerID = 101, Name = "Supplier A1" },
            new() { ID = 2, CustomerID = 102, Name = "Supplier B1" }
        };
        _mockRepo.Setup(r => r.GetSuppliers(101)).Returns(mockSuppliers.Where(s => s.CustomerID == 101));

        // Act
        var result = _service.GetSuppliers("101");

        // Assert
        Assert.Single(result);
        Assert.Equal("1", result.First().Key);
        Assert.Equal("Supplier A1", result.First().Value);
    }
}
