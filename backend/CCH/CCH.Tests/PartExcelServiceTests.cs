using CCH.Core.Entities;
using CCH.Core.Interfaces;
using CCH.Core.Interfaces.Repositories;
using CCH.Services.Features.Parts;
using CCH.Core.Features.Parts.DTOs;
using Moq;
using Xunit;
using ClosedXML.Excel;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System;

namespace CCH.Tests;

public class PartExcelServiceTests
{
    private readonly Mock<IPartRepository> _mockPartRepo = new();
    private readonly Mock<ICommonRepository> _mockCommonRepo = new();
    private readonly Mock<IUserContext> _mockUserContext = new();

    [Fact]
    public void PreviewBulkUpload_ShouldIdentifyNewAndModifiedRows()
    {
        // Arrange
        var customerId = 1;
        _mockCommonRepo.Setup(r => r.GetCountries()).Returns(new List<CountryEntity> { new() { ID = 10, Name = "Taiwan" } });
        _mockCommonRepo.Setup(r => r.GetSuppliers(It.IsAny<int?>())).Returns(new List<SupplierEntity> { new() { ID = 20, SupplierName = "Supplier A" } });
        
        // Mock existing part
        _mockPartRepo.Setup(r => r.GetPartByNo(customerId, "P-Existing")).Returns(new PartEntity 
        { 
            ID = 100, 
            CustomerID = customerId, 
            PartNo = "P-Existing", 
            PartDescription = "Old Desc",
            CountryID = 10,
            SupplierID = 20,
            Division = "DIV",
            Status = "S02"
        });

        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("Template");
        sheet.Cell(1, 1).Value = "Part No"; sheet.Cell(1, 2).Value = "Country"; sheet.Cell(1, 3).Value = "Division";
        sheet.Cell(1, 4).Value = "Supplier"; sheet.Cell(1, 5).Value = "Description"; sheet.Cell(1, 6).Value = "HTS Code";

        // Row 2: Modified (Description changed)
        sheet.Cell(2, 1).Value = "P-Existing"; sheet.Cell(2, 2).Value = "Taiwan"; sheet.Cell(2, 3).Value = "DIV";
        sheet.Cell(2, 4).Value = "Supplier A"; sheet.Cell(2, 5).Value = "New Desc"; sheet.Cell(2, 6).Value = "1234.56.7890";

        // Row 3: New
        sheet.Cell(3, 1).Value = "P-New"; sheet.Cell(3, 2).Value = "Taiwan"; sheet.Cell(3, 3).Value = "DIV";
        sheet.Cell(3, 4).Value = "Supplier A"; sheet.Cell(3, 5).Value = "New Part"; sheet.Cell(3, 6).Value = "1234.56.7890";

        // Row 4: Error (Missing mandatory Part No)
        sheet.Cell(4, 1).Value = "";
        sheet.Cell(4, 2).Value = "Taiwan"; // Set something to make row used

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;

        var service = new PartExcelService(_mockPartRepo.Object, _mockCommonRepo.Object, _mockUserContext.Object);

        // Act
        var result = service.PreviewBulkUpload(customerId, stream);

        // Assert
        Assert.Equal(3, result.Summary.TotalRows);
        Assert.Equal(1, result.Summary.ModifiedCount);
        Assert.Equal(1, result.Summary.NewCount);
        Assert.Equal(1, result.Summary.ErrorCount);

        var modifiedRow = result.Rows.First(r => r.RowStatus == "Modified");
        Assert.Equal("Old Desc", modifiedRow.OriginalData?.PartDesc);
        Assert.Equal("New Desc", modifiedRow.NewData.PartDesc);

        var errorRow = result.Rows.First(r => r.RowStatus == "Error");
        Assert.Contains("Part No is required.", errorRow.Errors);
    }

    [Fact]
    public void ConfirmBulkUpload_ShouldDeduplicateSuppliersAndUpsertParts()
    {
        // Arrange
        _mockUserContext.Setup(u => u.UserName).Returns("test-user");
        _mockCommonRepo.Setup(r => r.GetSuppliers(It.IsAny<int?>())).Returns(new List<SupplierEntity>());
        _mockCommonRepo.Setup(r => r.GetCountries()).Returns(new List<CountryEntity> { new() { ID = 10, Name = "Taiwan" } });
        
        var parts = new List<PartDto>
        {
            // Part with new supplier
            new() { CustomerId = 1, PartNo = "P1", Country = "Taiwan", Supplier = "New Supplier", SupplierId = 0, CountryId = 10, Division = "D", PartDesc = "Desc", HtsCode = "1234.56.7890" },
            // Another part with same new supplier
            new() { CustomerId = 1, PartNo = "P2", Country = "Taiwan", Supplier = "New Supplier", SupplierId = 0, CountryId = 10, Division = "D", PartDesc = "Desc", HtsCode = "" }
        };

        var service = new PartExcelService(_mockPartRepo.Object, _mockCommonRepo.Object, _mockUserContext.Object);

        // Act
        var result = service.ConfirmBulkUpload(parts);

        // Assert
        _mockCommonRepo.Verify(r => r.CreateSupplier(It.IsAny<SupplierEntity>()), Times.Once); // Should be called only once for same name
        _mockPartRepo.Verify(r => r.CreatePart(It.IsAny<PartEntity>()), Times.Exactly(2));
        
        Assert.Equal(2, result.Inserted);
        Assert.Equal("S02", parts[0].Status); // Has HtsCode
        Assert.Equal("S01", parts[1].Status); // Empty HtsCode
    }

    [Fact]
    public void ConfirmBulkUpload_ShouldSkipNoChangeAndReturnErrors()
    {
        // Arrange
        _mockUserContext.Setup(u => u.UserName).Returns("test-user");
        _mockCommonRepo.Setup(r => r.GetCountries()).Returns(new List<CountryEntity> { new() { ID = 10, Name = "Taiwan" } });
        _mockCommonRepo.Setup(r => r.GetSuppliers(It.IsAny<int?>())).Returns(new List<SupplierEntity> { new() { ID = 20, SupplierName = "Supplier A" } });

        // P-NoChange: No changes
        _mockPartRepo.Setup(r => r.GetPartByNo(1, "P-NoChange")).Returns(new PartEntity
        {
            ID = 1, CustomerID = 1, PartNo = "P-NoChange", CountryID = 10, SupplierID = 20, Division = "D", PartDescription = "Desc", Status = "S01"
        });

        // P-Error: Missing Division
        var parts = new List<PartDto>
        {
            new() { CustomerId = 1, PartNo = "P-NoChange", Country = "Taiwan", Supplier = "Supplier A", SupplierId = 20, Division = "D", PartDesc = "Desc", HtsCode = ""},
            new() { CustomerId = 1, PartNo = "P-Error", Country = "Taiwan", Supplier = "Supplier A", SupplierId = 20, Division = "", PartDesc = "Desc", HtsCode = "1234.56.7890" },
            new() { CustomerId = 1, PartNo = "P-New", Country = "Taiwan", Supplier = "Supplier A", SupplierId = 20, Division = "D", PartDesc = "New", HtsCode = "1234.56.7890" }
        };

        var service = new PartExcelService(_mockPartRepo.Object, _mockCommonRepo.Object, _mockUserContext.Object);

        // Act
        var result = service.ConfirmBulkUpload(parts);

        // Assert
        Assert.Equal(1, result.Inserted); // Only P-New
        Assert.Equal(1, result.Failed);   // P-Error
        Assert.Contains("Part P-Error: Division is required.", result.Errors);
        
        // P-NoChange should be skipped silently (result.Inserted=1, Updated=0, Failed=1)
        Assert.Equal(0, result.Updated);
        
        _mockPartRepo.Verify(r => r.CreatePart(It.Is<PartEntity>(p => p.PartNo == "P-New")), Times.Once);
        _mockPartRepo.Verify(r => r.UpdatePart(It.IsAny<PartEntity>()), Times.Never);
    }

    [Fact]
    public void GetUploadTemplate_ShouldReturnValidExcelFile()
    {
        // Arrange
        _mockCommonRepo.Setup(r => r.GetCountries()).Returns(new List<CountryEntity>
        {
            new() { ID = 1, Name = "Taiwan" },
            new() { ID = 2, Name = "USA" }
        });

        var service = new PartExcelService(_mockPartRepo.Object, _mockCommonRepo.Object, _mockUserContext.Object);

        // Act
        var result = service.GetUploadTemplate();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Length > 0);

        using var stream = new MemoryStream(result);
        using var workbook = new XLWorkbook(stream);
        var sheet = workbook.Worksheet("Template");
        
        Assert.Equal("Part No", sheet.Cell(1, 1).Value);
        Assert.Equal("Country", sheet.Cell(1, 2).Value);
        Assert.Equal("Remark", sheet.Cell(1, 16).Value);
        
        var dataSheet = workbook.Worksheet("DataLists");
        Assert.Equal("Taiwan", dataSheet.Cell(1, 1).Value);
        Assert.Equal(XLWorksheetVisibility.Hidden, dataSheet.Visibility);
    }
}
