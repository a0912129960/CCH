using CCH.Core.Entities.CSP;
using CCH.Services.Repositories;
using CCH.Services.Repositories.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace CCH.Tests;

public class PartRepositoryTests
{
    private readonly CspDbContext _context;
    private readonly ReSmDbContext _resmContext;
    private readonly PartRepository _repository;

    public PartRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<CspDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        _context = new CspDbContext(options);
        
        var resmOptions = new DbContextOptionsBuilder<ReSmDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        _resmContext = new ReSmDbContext(resmOptions);

        _repository = new PartRepository(_context, _resmContext);
    }

    [Fact]
    public void CreatePart_ShouldAddPartToDb()
    {
        // Arrange
        var part = new CchParts { PartNo = "NEW-1", ProjectID = 1, Status = "S01" };

        // Act
        var id = _repository.CreatePart(part);

        // Assert
        id.Should().BeGreaterThan(0);
        _context.CchParts.Should().Contain(p => p.PartNo == "NEW-1");
    }

    [Fact]
    public void UpdatePart_ShouldModifyExistingPart()
    {
        // Arrange
        var part = new CchParts { PartNo = "EXIST-1", ProjectID = 1, Status = "S01" };
        _context.CchParts.Add(part);
        _context.SaveChanges();

        part.Status = "S02";

        // Act
        _repository.UpdatePart(part);

        // Assert
        var updated = _context.CchParts.First(p => p.ID == part.ID);
        updated.Status.Should().Be("S02");
    }
}
