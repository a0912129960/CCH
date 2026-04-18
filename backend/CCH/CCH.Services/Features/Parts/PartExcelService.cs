using CCH.Core.Entities;
using CCH.Core.Features.Parts.DTOs;
using CCH.Core.Features.Parts.Interfaces;
using CCH.Core.Interfaces;
using CCH.Core.Interfaces.Repositories;
using ClosedXML.Excel;

namespace CCH.Services.Features.Parts;

/// <summary>
/// Implementation of Part Excel operations (Export, Upload) with Entity-to-DTO mapping.
/// (繁體中文) 零件 Excel 操作實作（匯出、上傳），包含實體到 DTO 的映射。
/// </summary>
public class PartExcelService : IPartExcelService
{
    private readonly IPartRepository _repository;
    private readonly ICommonRepository _commonRepository;
    private readonly IUserContext _userContext;

    /// <summary>
    /// Initializes a new instance of PartExcelService.
    /// (繁體中文) 初始化 PartExcelService 的新執行個體。
    /// </summary>
    public PartExcelService(IPartRepository repository, ICommonRepository commonRepository, IUserContext userContext)
    {
        _repository = repository;
        _commonRepository = commonRepository;
        _userContext = userContext;
    }

    /// <inheritdoc/>
    public byte[] ExportParts(int? customerId, string? status, string? partNo, int? supplierId)
    {
        var entities = _repository.SearchParts(customerId, status, partNo, supplierId);
        var statusMap = _commonRepository.GetStatuses().ToDictionary(s => s.Code, s => s.Description);
        var customerMap = _commonRepository.GetCustomers().ToDictionary(c => c.ID, c => c.Name);
        var countryMap = _commonRepository.GetCountries().ToDictionary(c => c.ID, c => c.Name);
        var supplierMap = _commonRepository.GetSuppliers().ToDictionary(s => s.ID, s => s.Name);
        
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Parts");
        
        string[] headers = { 
            "Customer", "Part No", "Description", "Country", "HTS Code", "Duty Rate", "Status",
            "301 Duty Code", "301 Duty Rate", "IEEPA Duty Code", "IEEPA Duty Rate", 
            "232 Aluminum Code", "232 Aluminum Rate", "Reciprocal Tariff Code", "Reciprocal Tariff Rate",
            "Updated By", "Last Updated"
        };
        
        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cell(1, i + 1).Value = headers[i];
            worksheet.Cell(1, i + 1).Style.Font.Bold = true;
        }
        
        var row = 2;
        foreach (var entity in entities)
        {
            worksheet.Cell(row, 1).Value = customerMap.TryGetValue(entity.CustomerID, out var cName) ? cName : "Unknown";
            worksheet.Cell(row, 2).Value = entity.PartNo;
            worksheet.Cell(row, 3).Value = entity.PartDescription;
            worksheet.Cell(row, 4).Value = countryMap.TryGetValue(entity.CountryID, out var coName) ? coName : "Unknown";
            worksheet.Cell(row, 5).Value = entity.HTSCode;
            worksheet.Cell(row, 6).Value = entity.DutyRate;
            
            var statusDesc = statusMap.TryGetValue(entity.Status ?? "", out var desc) ? desc : (entity.Status == "Inactive" ? "Inactive" : entity.Status);
            worksheet.Cell(row, 7).Value = statusDesc;

            worksheet.Cell(row, 8).Value = entity.AddHTSCode1;
            worksheet.Cell(row, 9).Value = entity.AddDutyRate1;
            worksheet.Cell(row, 10).Value = entity.AddHTSCode2;
            worksheet.Cell(row, 11).Value = entity.AddDutyRate2;
            worksheet.Cell(row, 12).Value = entity.AddHTSCode3;
            worksheet.Cell(row, 13).Value = entity.AddDutyRate3;
            worksheet.Cell(row, 14).Value = entity.AddHTSCode4;
            worksheet.Cell(row, 15).Value = entity.AddDutyRate4;
            worksheet.Cell(row, 16).Value = entity.UpdatedBy;
            worksheet.Cell(row, 17).Value = entity.UpdatedDate;
            row++;
        }
        
        worksheet.Columns().AdjustToContents();
        
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    /// <inheritdoc/>
    public List<BulkUploadResponseDto> BulkUpload(Stream fileStream) => new List<BulkUploadResponseDto>()
    {
        new() { PartId = 1, Error = "Mock Error" },
        new() { PartId = 2, Error = "Mock Error" }
    };

    /// <inheritdoc/>
    public byte[] GetUploadTemplate() => System.Text.Encoding.UTF8.GetBytes("Mock Template Content");
}
