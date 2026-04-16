using CCH.Core.DTOs;
using CCH.Services.Repositories;
using Xunit;

namespace CCH.Tests;

/// <summary>
/// Tests for PartRepository implementation with relational file persistence.
/// (繁體中文) 具備關聯式檔案持久化的 PartRepository 實作測試。
/// </summary>
public class PartRepositoryTests : IDisposable
{
    private readonly string _testBaseDir;
    private readonly string _testPartsPath;
    private readonly PartRepository _repository;

    public PartRepositoryTests()
    {
        _testBaseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Test_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testBaseDir);
        _testPartsPath = Path.Combine(_testBaseDir, "parts.json");
        _repository = new PartRepository(_testPartsPath);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testBaseDir))
        {
            Directory.Delete(_testBaseDir, true);
        }
    }

    [Fact]
    public void SearchParts_FilterByCustomer_ReturnsMappedName()
    {
        // Act - Search for Customer A (ID 101)
        var result = _repository.SearchParts("101", null, null, null);

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal("Customer A", result.First().Customer);
    }

    [Fact]
    public void SearchParts_FilterByCountry_ReturnsMappedName()
    {
        // Act
        var result = _repository.SearchParts(null, null, "PART-001", null);

        // Assert
        Assert.Single(result);
        Assert.Equal("Taiwan", result.First().Country);
    }

    [Fact]
    public void CreatePart_IncrementsIdAndPreservesLookups()
    {
        // Arrange
        var request = new PartSaveRequest 
        { 
            PartNo = "RELATIONAL-TEST", 
            PartDesc = "Relational persistence", 
            HtsCode = "9999.99.9999",
            CustomerId = 103, // Customer C
            CountryId = 4     // Japan
        };
        
        // Act
        var newId = _repository.CreatePart(request, "S01");
        
        // Assert - Verify in a fresh instance
        var secondRepo = new PartRepository(_testPartsPath);
        var part = secondRepo.SearchParts(null, null, "RELATIONAL-TEST", null).FirstOrDefault();
        
        Assert.NotNull(part);
        Assert.Equal(newId, part.Id);
        Assert.Equal("Customer C", part.Customer);
        Assert.Equal("Japan", part.Country);
    }
}
