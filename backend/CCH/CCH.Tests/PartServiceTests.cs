using CCH.Core.Features.Parts.DTOs;
using CCH.Core.Entities;
using CCH.Core.Entities.CSP;
using CCH.Core.Entities.ReSm;
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

        var resmOptions = new DbContextOptionsBuilder<ReSmDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var resmContext = new ReSmDbContext(resmOptions);

        SeedData();

        var mockCommonRepo = new Mock<ICommonRepository>();
        mockCommonRepo.Setup(r => r.GetProjects()).Returns(new List<CpProject> { new() { Id = 101, ProjectName = "Project A" } });
        mockCommonRepo.Setup(r => r.GetCountries()).Returns(new List<CountryEntity> { new() { ID = 1, Name = "Taiwan" } });
        mockCommonRepo.Setup(r => r.GetSuppliers(It.IsAny<int?>())).Returns(new List<CchSuppliers> { new() { ID = 1, SupplierName = "TechSupply Corp" } });
        mockCommonRepo.Setup(r => r.GetStatuses()).Returns(new List<StatusEntity> { new() { Code = "S01", Description = "Active" } });
        mockCommonRepo.Setup(r => r.GetUserName(It.IsAny<string>())).Returns((string s) => s);
        mockCommonRepo.Setup(r => r.GetUserNames(It.IsAny<IEnumerable<string>>())).Returns((IEnumerable<string> ids) => ids.ToDictionary(id => id, id => id));

        _repository = new PartRepository(_context, resmContext);

        _mockUserContext = new Mock<IUserContext>();
        _mockUserContext.Setup(u => u.Role).Returns("admin"); 

        _queryService = new PartQueryService(_repository, mockCommonRepo.Object, _mockUserContext.Object);
        // Corrected constructor: PartExcelService needs userContext as well (修正建構函式)
        _excelService = new PartExcelService(_repository, mockCommonRepo.Object, _mockUserContext.Object);
    }

    private void SeedData()
    {
        for (int i = 1; i <= 17; i++)
        {
            _context.CchParts.Add(new CchParts 
            { 
                ID = i, ProjectID = 101, PartNo = $"PART-{i:000}", Status = "S01", 
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
        Assert.Equal("Project A", result.Data.First().Project); 
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
    }

    [Fact]
    public void GetMilestones_ReturnsDatabaseHistory()
    {
        // Arrange
        _context.CchPartMilestones.Add(new CchPartMilestones
        {
            PartID = 1,
            Action = "Test Action",
            CreatedBy = "Tester",
            CreatedDate = DateTime.Now,
            Remark = "Test Remark"
        });
        _context.SaveChanges();

        // Act
        var result = _queryService.GetMilestones(1);

        // Assert
        Assert.Single(result);
        Assert.Equal("Test Action", result.First().Action);
        Assert.Equal("Tester", result.First().UpdatedBy);
    }
}
