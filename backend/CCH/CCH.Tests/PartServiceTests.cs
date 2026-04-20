using CCH.Core.Features.Parts.DTOs;
using CCH.Core.Entities;
using CCH.Core.Features.Parts.Interfaces;
using CCH.Core.Interfaces;
using CCH.Core.Interfaces.Repositories;
using CCH.Services.Features.Parts;
using CCH.Services.Repositories;
using Moq;
using Xunit;

namespace CCH.Tests;

/// <summary>
/// Tests for PartQueryService and PartExcelService implementation with separated responsibilities.
/// (繁體中文) 具備職責分離的 PartQueryService 與 PartExcelService 實作測試。
/// </summary>
public class PartServiceTests : IDisposable
{
    private readonly string _testBaseDir;
    private readonly string _testPartsPath;
    private readonly PartQueryService _queryService;
    private readonly PartExcelService _excelService;
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
        mockCommonRepo.Setup(r => r.GetSuppliers(It.IsAny<int?>())).Returns(new List<SupplierEntity> { new() { ID = 1, Name = "TechSupply Corp" } });

        // Repository handles raw entity persistence (倉儲處理原始實體持久化)
        _repository = new PartRepository(_testPartsPath);

        _mockUserContext = new Mock<IUserContext>();
        _mockUserContext.Setup(u => u.Role).Returns("admin"); 

        // QueryService now handles mapping and requires CommonRepository (Service 現在處理映射，需要 CommonRepository)
        _queryService = new PartQueryService(_repository, mockCommonRepo.Object, _mockUserContext.Object);
        _excelService = new PartExcelService(_repository, mockCommonRepo.Object, _mockUserContext.Object);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testBaseDir))
        {
            Directory.Delete(_testBaseDir, true);
        }
    }

    [Fact]
    public void SearchParts_NoFilters_ReturnsMappedDataWithSla()
    {
        // Act
        var result = _queryService.SearchParts(null, null, null, null, 1, 20);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(17, result.Total); // Verify initial seeder data count
        Assert.Equal("Customer A", result.Data.First().Customer); // Verify mapping from Service layer
        Assert.NotNull(result.Data.First().SlaStatus); // Verify SLA calculated in Service
    }

    [Fact]
    public void ExportParts_ReturnsValidExcelBinary()
    {
        // Act
        var result = _excelService.ExportParts(null, null, null, null);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Length > 0);
        Assert.Equal((byte)'P', result[0]); // Excel signature PK..
        Assert.Equal((byte)'K', result[1]);
    }
}
