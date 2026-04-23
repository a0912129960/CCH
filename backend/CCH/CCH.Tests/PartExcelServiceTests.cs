using CCH.Core.Entities;
using CCH.Core.Entities.CSP;
using CCH.Core.Entities.ReSm;
using CCH.Core.Features.Parts.DTOs;
using CCH.Core.Interfaces;
using CCH.Core.Interfaces.Repositories;
using CCH.Services.Features.Parts;
using ClosedXML.Excel;
using Moq;
using Xunit;

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
        
        // Setup default mocks
        _mockCommonRepo.Setup(r => r.GetCountries()).Returns(new List<CountryEntity> { new() { ID = 1, Name = "USA", Code = "US" } });
        _mockCommonRepo.Setup(r => r.GetSuppliers(It.IsAny<int?>())).Returns(new List<CchSuppliers> { new() { ID = 20, SupplierName = "Supplier A" } });
        _mockCommonRepo.Setup(r => r.GetStatuses()).Returns(new List<StatusEntity> { new() { Code = "S01", Description = "Draft" } });
        _mockCommonRepo.Setup(r => r.GetCustomers()).Returns(new List<SmCustomer> { new() { HQID = 101, CustomerName = "Customer A" } });
        _mockUserContext.Setup(u => u.UserName).Returns("test_user");
    }

    [Fact]
    public void ExportParts_ReturnsValidExcel()
    {
        var parts = new List<PartEntity> { new() { ID = 1, PartNo = "P1", CustomerID = 101, CountryID = 1, SupplierID = 20 } };
        _mockRepo.Setup(r => r.SearchParts(It.IsAny<int?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<int?>())).Returns(parts);

        var result = _service.ExportParts(101, null, null, null);

        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    [Fact]
    public void ConfirmBulkUpload_InsertsNewSuppliers()
    {
        var parts = new List<PartDto>
        {
            new() { PartNo = "P1", Country = "USA", Division = "D1", Supplier = "New Supplier", PartDesc = "Desc", HtsCode = "123", CustomerId = 101 }
        };
        
        // Setup: Supplier doesn't exist initially
        _mockCommonRepo.Setup(r => r.GetSuppliers(It.IsAny<int?>())).Returns(new List<CchSuppliers>());
        _mockRepo.Setup(r => r.GetPartByNo(101, "P1")).Returns((PartEntity)null);
        
        // Confirm execution
        var result = _service.ConfirmBulkUpload(parts);

        // Verification
        Assert.Equal(0, result.Failed);
        Assert.Equal(1, result.Inserted);
        _mockCommonRepo.Verify(r => r.CreateSupplier(It.IsAny<CchSuppliers>()), Times.Once);
    }

    [Fact]
    public void ConfirmBulkUpload_DeduplicatesNewSuppliers()
    {
        var parts = new List<PartDto>
        {
            new() { PartNo = "P1", Country = "USA", Division = "D1", Supplier = "New Supplier", PartDesc = "Desc", HtsCode = "123", CustomerId = 101 },
            new() { PartNo = "P2", Country = "USA", Division = "D1", Supplier = "New Supplier", PartDesc = "Desc", HtsCode = "456", CustomerId = 101 }
        };
        
        _mockCommonRepo.Setup(r => r.GetSuppliers(It.IsAny<int?>())).Returns(new List<CchSuppliers>());
        _mockRepo.Setup(r => r.GetPartByNo(101, It.IsAny<string>())).Returns((PartEntity)null);
        
        _service.ConfirmBulkUpload(parts);

        _mockCommonRepo.Verify(r => r.CreateSupplier(It.IsAny<CchSuppliers>()), Times.Once); // Should be called only once for same name
    }

    [Fact]
    public void PreviewBulkUpload_IdentifiesModifiedRows()
    {
        var parts = new List<PartDto> { new() { PartNo = "P1", Country = "USA", Division = "D1", Supplier = "Supplier A", PartDesc = "New Desc", CustomerId = 101 } };
        var existing = new PartEntity { ID = 1, PartNo = "P1", CustomerID = 101, CountryID = 1, Division = "D1", SupplierID = 20, PartDescription = "Old Desc" };
        
        _mockCommonRepo.Setup(r => r.GetSuppliers(It.IsAny<int?>())).Returns(new List<CchSuppliers> { new() { ID = 20, SupplierName = "Supplier A" } });
        _mockRepo.Setup(r => r.GetPartByNo(101, "P1")).Returns(existing);

        // Simulate Excel stream
        using var stream = CreateTestExcel("P1", "USA", "D1", "Supplier A", "New Desc");
        var result = _service.PreviewBulkUpload(101, stream);

        Assert.Equal(1, result.Summary.ModifiedCount);
        Assert.Equal("Modified", result.Rows[0].RowStatus);
    }

    private MemoryStream CreateTestExcel(string partNo, string country, string division, string supplier, string desc)
    {
        var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Parts");
        ws.Cell(1, 1).Value = "Part No"; ws.Cell(1, 2).Value = "Country"; ws.Cell(1, 3).Value = "Division";
        ws.Cell(1, 4).Value = "Supplier"; ws.Cell(1, 5).Value = "Description";
        
        ws.Cell(2, 1).Value = partNo; ws.Cell(2, 2).Value = country; ws.Cell(2, 3).Value = division;
        ws.Cell(2, 4).Value = supplier; ws.Cell(2, 5).Value = desc;
        
        var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;
        return stream;
    }
}
