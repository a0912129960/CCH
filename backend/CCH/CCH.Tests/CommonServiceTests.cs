using CCH.Core.Entities;
using CCH.Core.Entities.CSP;
using CCH.Core.Interfaces;
using CCH.Core.Interfaces.Repositories;
using CCH.Services.Features.Common;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace CCH.Tests;

public class CommonServiceTests
{
    private readonly Mock<IUserContext> _mockUserContext;
    private readonly IMemoryCache _cache;

    public CommonServiceTests()
    {
        _mockUserContext = new Mock<IUserContext>();
        _cache = new MemoryCache(new MemoryCacheOptions());
    }

    [Fact]
    public void GetProjects_ReturnsMappedDtosAndUsesCache()
    {
        var mockRepo = new Mock<ICommonRepository>();
        _mockUserContext.Setup(u => u.UserId).Returns("user1");
        _mockUserContext.Setup(u => u.Role).Returns("admin");

        mockRepo.Setup(r => r.GetProjects("user1", "admin")).Returns(new List<CpProject>
        {
            new() { Id = 1, ProjectName = "Project A", Status = "Active" },
            new() { Id = 2, ProjectName = "Project B", Status = "Active" }
        });
        var service = new CommonService(mockRepo.Object, _mockUserContext.Object, _cache);

        // First call - should call repository (第一次呼叫 - 應呼叫倉儲)
        var result = service.GetProjects().ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("1", result[0].Key);
        Assert.Equal("Project A", result[0].Value);
        mockRepo.Verify(r => r.GetProjects("user1", "admin"), Times.Once);

        // Second call - should use cache (第二次呼叫 - 應使用快取)
        var result2 = service.GetProjects().ToList();
        Assert.Equal(2, result2.Count);
        mockRepo.Verify(r => r.GetProjects("user1", "admin"), Times.Once); // Still Once
    }

    [Fact]
    public void GetCountries_ReturnsMappedDtos()
    {
        var mockRepo = new Mock<ICommonRepository>();
        mockRepo.Setup(r => r.GetCountries()).Returns(new List<CountryEntity>
        {
            new() { ID = 1, Code = "US", Name = "United States" },
            new() { ID = 2, Code = "TW", Name = "Taiwan" }
        });
        var service = new CommonService(mockRepo.Object, _mockUserContext.Object, _cache);

        var result = service.GetCountries().ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("1", result[0].Key);
        Assert.Equal("United States", result[0].Value);
    }

    [Fact]
    public void GetSuppliers_ReturnsMappedDtos()
    {
        var mockRepo = new Mock<ICommonRepository>();
        var mockSuppliers = new List<CchSuppliers>
        {
            new() { ID = 1, SupplierName = "Supplier A", ProjectID = 101 },
            new() { ID = 2, SupplierName = "Supplier B", ProjectID = 101 }
        };
        mockRepo.Setup(r => r.GetSuppliers(101)).Returns(mockSuppliers);
        var service = new CommonService(mockRepo.Object, _mockUserContext.Object, _cache);

        var result = service.GetSuppliers("101").ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("1", result[0].Key);
        Assert.Equal("Supplier A", result[0].Value);
    }

    [Fact]
    public void GetStatus_ReturnsMappedDtos()
    {
        var mockRepo = new Mock<ICommonRepository>();
        mockRepo.Setup(r => r.GetStatuses()).Returns(new List<StatusEntity>
        {
            new() { Code = "S01", Description = "Draft" }
        });
        var service = new CommonService(mockRepo.Object, _mockUserContext.Object, _cache);

        var result = service.GetStatus().ToList();

        Assert.Single(result);
        Assert.Equal("S01", result[0].Key);
        Assert.Equal("Draft", result[0].Value);
    }
}
