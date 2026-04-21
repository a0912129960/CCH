using CCH.Core.Entities;
using CCH.Core.Interfaces;
using CCH.Core.Interfaces.Repositories;
using CCH.Services.Features.Parts;
using Moq;
using Xunit;
using ClosedXML.Excel;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace CCH.Tests;

public class PartExcelServiceTests
{
    private readonly Mock<IPartRepository> _mockPartRepo = new();
    private readonly Mock<ICommonRepository> _mockCommonRepo = new();
    private readonly Mock<IUserContext> _mockUserContext = new();

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
