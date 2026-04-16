using CCH.Core.DTOs;
using CCH.Services.Repositories;
using CCH.Services.Services;
using Xunit;

namespace CCH.Tests;

/// <summary>
/// Tests for PartService implementation using Repository.
/// (繁體中文) 使用 Repository 的 PartService 實作測試。
/// </summary>
public class PartServiceTests
{
    private readonly PartService _partService;
    private readonly PartRepository _repository;

    public PartServiceTests()
    {
        _repository = new PartRepository();
        _partService = new PartService(_repository);
    }

    [Fact]
    public void SearchParts_NoFilters_ReturnsAllMockItems()
    {
        // Act
        var result = _partService.SearchParts(null, null, null, null, 1, 10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(4, result.Total);
        Assert.Equal(4, result.Data.Count());
    }

    [Fact]
    public void SearchParts_FilterByCustomer_ReturnsFilteredItems()
    {
        // Act
        var result = _partService.SearchParts("Customer A", null, null, null, 1, 10);

        // Assert
        Assert.Equal(2, result.Total);
        Assert.All(result.Data, item => Assert.Contains("Customer A", item.Customer));
    }

    [Fact]
    public void SearchParts_Pagination_ReturnsCorrectPageSize()
    {
        // Act
        var result = _partService.SearchParts(null, null, null, null, 1, 2);

        // Assert
        Assert.Equal(4, result.Total);
        Assert.Equal(2, result.Data.Count());
    }
}
