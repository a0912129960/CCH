using CCH.Core.Features.Parts.DTOs;
using CCH.Core.Features.Parts.Interfaces;
using CCH.Core.Interfaces;
using CCH.Core.Interfaces.Repositories;
using ClosedXML.Excel;

namespace CCH.Services.Features.Parts;

/// <summary>
/// Implementation of Part Excel operations (Export, Upload).
/// (繁體中文) 零件 Excel 操作實作（匯出、上傳）。
/// </summary>
public class PartExcelService : IPartExcelService
{
    private readonly IPartRepository _repository;
    private readonly ICommonRepository _commonRepository;
    private readonly IUserContext _userContext;

    public PartExcelService(IPartRepository repository, ICommonRepository commonRepository, IUserContext userContext)
    {
        _repository = repository;
        _commonRepository = commonRepository;
        _userContext = userContext;
    }

    public byte[] ExportParts(int? customerId, string? status, string? partNo, int? supplierId)
    {
        var parts = _repository.SearchParts(customerId, status, partNo, supplierId, _userContext.Role);
        var statusMap = _commonRepository.GetStatuses().ToDictionary(s => s.Code, s => s.Description);
        
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
        foreach (var part in parts)
        {
            worksheet.Cell(row, 1).Value = part.Customer;
            worksheet.Cell(row, 2).Value = part.PartNo;
            worksheet.Cell(row, 3).Value = part.PartDesc;
            worksheet.Cell(row, 4).Value = part.Country;
            worksheet.Cell(row, 5).Value = part.HtsCode;
            worksheet.Cell(row, 6).Value = part.Rate;
            
            var statusDesc = statusMap.TryGetValue(part.Status ?? "", out var desc) ? desc : (part.Status == "Inactive" ? "Inactive" : part.Status);
            worksheet.Cell(row, 7).Value = statusDesc;

            worksheet.Cell(row, 8).Value = part.HtsCode1;
            worksheet.Cell(row, 9).Value = part.Rate1;
            worksheet.Cell(row, 10).Value = part.HtsCode2;
            worksheet.Cell(row, 11).Value = part.Rate2;
            worksheet.Cell(row, 12).Value = part.HtsCode3;
            worksheet.Cell(row, 13).Value = part.Rate3;
            worksheet.Cell(row, 14).Value = part.HtsCode4;
            worksheet.Cell(row, 15).Value = part.Rate4;
            worksheet.Cell(row, 16).Value = part.UpdatedBy;
            worksheet.Cell(row, 17).Value = part.UpdatedDate;
            row++;
        }
        
        worksheet.Columns().AdjustToContents();
        
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public List<BulkUploadResponseDto> BulkUpload(Stream fileStream) => new List<BulkUploadResponseDto>()
    {
        new() { PartId = 1, Error = "error" },
        new() { PartId = 2, Error = "error" }
    };

    public byte[] GetUploadTemplate() => System.Text.Encoding.UTF8.GetBytes("Mock Template Content");
}
