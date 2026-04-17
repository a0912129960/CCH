using CCH.Core.Entities;
using CCH.Core.Interfaces.Repositories;
using CCH.Services.Services;
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
        // Arrange
        var mockCustomers = new List<CustomerEntity>
        {
            new() { ID = 101, Name = "Customer A" }
        };
        _mockRepo.Setup(r => r.GetCustomers()).Returns(mockCustomers);

        // Act
        var result = _service.GetCustomers();

        // Assert
        Assert.Single(result);
        Assert.Equal("101", result.First().Key);
        Assert.Equal("Customer A", result.First().Value);
    }
}
