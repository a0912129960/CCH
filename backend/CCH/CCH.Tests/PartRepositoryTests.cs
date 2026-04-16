using CCH.Core.DTOs;
using CCH.Services.Repositories;
using Xunit;

namespace CCH.Tests;

/// <summary>
/// Tests for PartRepository implementation.
/// (繁體中文) PartRepository 實作的測試。
/// </summary>
public class PartRepositoryTests
{
    private readonly PartRepository _repository;

    public PartRepositoryTests()
    {
        _repository = new PartRepository();
    }

    [Fact]
    public void SearchParts_FilterByCustomer_ReturnsFilteredItems()
    {
        // Act
        var result = _repository.SearchParts("Customer A", null, null, null);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, item => Assert.Contains("Customer A", item.Customer));
    }

    [Fact]
    public void GetPartDetail_ValidId_ReturnsDetail()
    {
        // Act
        var result = _repository.GetPartDetail(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("PART-001", result.Before.PartNo);
    }
}
