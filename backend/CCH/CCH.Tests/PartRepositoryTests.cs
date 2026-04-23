using CCH.Core.Entities;
using CCH.Core.Entities.CSP;
using CCH.Core.Entities.ReSm;
using CCH.Core.Interfaces.Repositories;
using CCH.Services.Repositories;
using CCH.Services.Repositories.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CCH.Tests;

/// <summary>
/// Tests for PartRepository implementation with Database persistence.
/// (繁體中文) 具備資料庫持久化的 PartRepository 實作測試。
/// </summary>
public class PartRepositoryTests : IDisposable
{
    private readonly CspDbContext _context;
    private readonly PartRepository _repository;

    public PartRepositoryTests()
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
        _repository = new PartRepository(_context, resmContext);
    }

    private void SeedData()
    {
        _context.CchParts.Add(new CchParts { ID = 1, CustomerID = 101, PartNo = "PART-001", Status = "S01", HTSCode = "111.22.33" });
        _context.CchParts.Add(new CchParts { ID = 3, CustomerID = 101, PartNo = "PART-003", Status = "S01", HTSCode = "9032.89.6085" });
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public void SearchParts_FilterByCustomer_ReturnsEntityWithCorrectId()
    {
        // Act
        var result = _repository.SearchParts(101, null, null, null);

        // Assert
        Assert.NotEmpty(result);
        Assert.All(result, p => Assert.Equal(101, p.CustomerID));
    }

    [Fact]
    public void CreatePart_IncrementsIdAndPersistsToDb()
    {
        // Arrange
        var entity = new PartEntity 
        { 
            PartNo = "NEW-PART-001", 
            PartDescription = "New Test Part", 
            HTSCode = "1234.56.7890",
            CustomerID = 101,
            CountryID = 1
        };
        
        // Act
        var newId = _repository.CreatePart(entity);
        
        // Assert
        var part = _repository.GetPartById(newId);
        Assert.NotNull(part);
        Assert.Equal(newId, part.ID);
        Assert.Equal("NEW-PART-001", part.PartNo);
    }

    [Theory]
    [InlineData("9032.89.6085")]
    [InlineData("9032896085")]
    public void SearchParts_ByHTSCode_Normalized_ReturnsMatch(string search)
    {
        // Act
        var result = _repository.SearchParts(null, null, search, null);

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains(result, p => p.PartNo == "PART-003");
    }
}
