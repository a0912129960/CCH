using CCH.Core.Features.Parts.DTOs;
using CCH.Core.Entities;
using CCH.Core.Interfaces;
using CCH.Core.Interfaces.Repositories;
using CCH.Services.Features.Parts;
using CCH.Services.Repositories;
using CCH.Services.Repositories.Data;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CCH.Tests;

/// <summary>
/// Tests for PartQueryService and PartExcelService implementation with Database-backed Repository.
/// (繁體中文) 具備資料庫支援倉儲的 PartQueryService 與 PartExcelService 實作測試。
/// </summary>
public class PartServiceTests : IDisposable
{
    private readonly CspDbContext _context;
    private readonly PartQueryService _queryService;
    private readonly PartExcelService _excelService;
    private readonly PartRepository _repository;
    private readonly Mock<IUserContext> _mockUserContext;

    public PartServiceTests()
    {
        var options = new DbContextOptionsBuilder<CspDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new CspDbContext(options);
        SeedData();

        var mockCommonRepo = new Mock<ICommonRepository>();
        mockCommonRepo.Setup(r => r.GetCustomers()).Returns(new List<CustomerEntity> { new() { ID = 101, Name = "Customer A" } });
        mockCommonRepo.Setup(r => r.GetCountries()).Returns(new List<CountryEntity> { new() { ID = 1, Name = "Taiwan" } });
        mockCommonRepo.Setup(r => r.GetSuppliers(It.IsAny<int?>())).Returns(new List<SupplierEntity> { new() { ID = 1, Name = "TechSupply Corp" } });
        mockCommonRepo.Setup(r => r.GetStatuses()).Returns(new List<StatusEntity> { new() { Code = "S01", Description = "Active" } });

        _repository = new PartRepository(_context);

        _mockUserContext = new Mock<IUserContext>();
        _mockUserContext.Setup(u => u.Role).Returns("admin"); 

        _queryService = new PartQueryService(_repository, mockCommonRepo.Object, _mockUserContext.Object);
        _excelService = new PartExcelService(_repository, mockCommonRepo.Object, _mockUserContext.Object);
    }

    private void SeedData()
    {
        for (int i = 1; i <= 17; i++)
        {
            _context.CchParts.Add(new CchParts 
            { 
                ID = i, CustomerID = 101, PartNo = $"PART-{i:000}", Status = "S01", 
                CountryID = 1, SupplierID = 1, PartDescription = "Test Part",
                UpdatedDate = DateTime.Now
            });
        }
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public void SearchParts_NoFilters_ReturnsMappedDataWithSla()
    {
        // Act
        var result = _queryService.SearchParts(null, null, null, null, 1, 20);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(17, result.Total); // Matches SeedData count
        Assert.Equal("Customer A", result.Data.First().Customer); 
        Assert.NotNull(result.Data.First().SlaStatus); 
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
