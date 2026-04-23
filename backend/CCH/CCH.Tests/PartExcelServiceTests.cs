using CCH.Core.Entities;
using CCH.Core.Entities.CSP;
using CCH.Core.Entities.ReSm;
using CCH.Core.Features.Parts.DTOs;
using CCH.Core.Interfaces;
using CCH.Core.Interfaces.Repositories;
using CCH.Services.Features.Parts;
using FluentAssertions;
using Moq;

namespace CCH.Tests;

public class PartExcelServiceTests
{
    private readonly Mock<IPartRepository> _mockRepo;
    private readonly Mock<ICommonRepository> _mockCommonRepo;
    private readonly Mock<IUserContext> _mockUserContext;
    private readonly PartExcelService _service;

    public PartExcelServiceTests()
    {
        _mockRepo = new Mock<IPartRepository>();
        _mockCommonRepo = new Mock<ICommonRepository>();
        _mockUserContext = new Mock<IUserContext>();
        _service = new PartExcelService(_mockRepo.Object, _mockCommonRepo.Object, _mockUserContext.Object);
    }

    [Fact]
    public void ExportParts_ShouldReturnByteArray()
    {
        // Arrange
        _mockCommonRepo.Setup(r => r.GetCustomers()).Returns(new List<SmCustomer> { new() { HQID = 1, CustomerName = "Test Cust" } });
        _mockCommonRepo.Setup(r => r.GetCountries()).Returns(new List<CountryEntity> { new() { ID = 1, Name = "Test Country" } });
        _mockCommonRepo.Setup(r => r.GetSuppliers(null)).Returns(new List<CchSuppliers> { new() { ID = 1, SupplierName = "Test Supp" } });
        _mockCommonRepo.Setup(r => r.GetStatuses()).Returns(new List<StatusEntity> { new() { Code = "S01", Description = "Draft" } });

        _mockRepo.Setup(r => r.SearchParts(It.IsAny<int?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<int?>()))
                 .Returns(new List<CchParts> { new() { ID = 1, PartNo = "P1", CustomerID = 1, CountryID = 1, SupplierID = 1, Status = "S01" } });

        // Act
        var result = _service.ExportParts(null, null, null, null);

        // Assert
        result.Should().NotBeNull();
        result.Length.Should().BeGreaterThan(0);
    }

    [Fact]
    public void ConfirmBulkUpload_ShouldCreateAndUpdateParts()
    {
        // Arrange
        var parts = new List<PartDto>
        {
            new() { CustomerId = 1, PartNo = "NEW-1", PartDesc = "New", CountryId = 1, SupplierId = 1, Rate = 0.1m },
            new() { CustomerId = 1, PartNo = "EXIST-1", PartDesc = "Updated", CountryId = 1, SupplierId = 1, Rate = 0.2m }
        };

        _mockRepo.Setup(r => r.GetPartByNo(1, "NEW-1")).Returns((CchParts?)null);
        _mockRepo.Setup(r => r.GetPartByNo(1, "EXIST-1")).Returns(new CchParts { ID = 99, PartNo = "EXIST-1" });

        // Act
        var result = _service.ConfirmBulkUpload(parts);

        // Assert
        result.Inserted.Should().Be(1);
        result.Updated.Should().Be(1);
        _mockRepo.Verify(r => r.CreatePart(It.IsAny<CchParts>()), Times.Once);
        _mockRepo.Verify(r => r.UpdatePart(It.IsAny<CchParts>()), Times.Once);
    }
}
