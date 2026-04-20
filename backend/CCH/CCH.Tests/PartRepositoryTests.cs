using CCH.Core.Entities;
using CCH.Core.Interfaces.Repositories;
using CCH.Services.Repositories;
using Moq;
using Xunit;

namespace CCH.Tests;

/// <summary>
/// Tests for PartRepository implementation with JSON file persistence.
/// (繁體中文) 具備 JSON 檔案持久化的 PartRepository 實作測試。
/// </summary>
public class PartRepositoryTests : IDisposable
{
    private readonly string _testBaseDir;
    private readonly string _testPartsPath;
    private readonly PartRepository _repository;

    public PartRepositoryTests()
    {
        _testBaseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Test_Repo_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testBaseDir);
        _testPartsPath = Path.Combine(_testBaseDir, "parts.json");

        // Repository now only handles file I/O and depends on DataSeeder for initialization
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
    public void SearchParts_FilterByCustomer_ReturnsEntityWithCorrectId()
    {
        // Act - Search for Customer ID 101 (Initial seed data includes this)
        var result = _repository.SearchParts(101, null, null, null);

        // Assert
        Assert.NotEmpty(result);
        Assert.All(result, p => Assert.Equal(101, p.CustomerID));
    }

    [Fact]
    public void CreatePart_IncrementsIdAndPersistsToFile()
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
        
        // Assert - Verify in a fresh instance to ensure file persistence
        var secondRepo = new PartRepository(_testPartsPath);
        var part = secondRepo.GetPartById(newId);
        
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
