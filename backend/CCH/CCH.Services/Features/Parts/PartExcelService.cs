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
            "Customer", "Part No", "Description", "Country", "HTS Code", "Duty Rate (%)", "Status",
            "301 Duty Code", "301 Duty Rate (%)", "IEEPA Duty Code", "IEEPA Duty Rate (%)", 
            "232 Aluminum Code", "232 Aluminum Rate (%)", "Reciprocal Tariff Code", "Reciprocal Tariff Rate (%)",
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
    public BulkUploadPreviewDto PreviewBulkUpload(int customerId, Stream fileStream)
    {
        var result = new BulkUploadPreviewDto { Summary = new BulkUploadSummaryDto() };
        
        // Use GroupBy/First to handle duplicate names in DB safely (使用 GroupBy 處理資料庫中的重複名稱)
        var countries = _commonRepository.GetCountries()
            .Where(c => !string.IsNullOrEmpty(c.Name))
            .GroupBy(c => c.Name, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First().ID, StringComparer.OrdinalIgnoreCase);

        var suppliers = _commonRepository.GetSuppliers()
            .Where(s => !string.IsNullOrEmpty(s.SupplierName))
            .GroupBy(s => s.SupplierName!, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First().ID, StringComparer.OrdinalIgnoreCase);
        
        using var workbook = new XLWorkbook(fileStream);
        var worksheet = workbook.Worksheet(1);
        var rows = worksheet.RowsUsed().Skip(1); // Skip header

        foreach (var row in rows)
        {
            var previewRow = new PartPreviewRowDto
            {
                RowIndex = row.RowNumber(),
                NewData = new PartDto
                {
                    CustomerId = customerId,
                    PartNo = row.Cell(1).GetValue<string>().Trim(),
                    Country = row.Cell(2).GetValue<string>().Trim(),
                    Division = row.Cell(3).GetValue<string>().Trim(),
                    Supplier = row.Cell(4).GetValue<string>().Trim(),
                    PartDesc = row.Cell(5).GetValue<string>().Trim(),
                    HtsCode = row.Cell(6).GetValue<string>().Trim(),
                    Rate = GetDecimalValue(row.Cell(7)),
                    HtsCode1 = row.Cell(8).GetValue<string>().Trim(),
                    Rate1 = GetDecimalValue(row.Cell(9)),
                    HtsCode2 = row.Cell(10).GetValue<string>().Trim(),
                    Rate2 = GetDecimalValue(row.Cell(11)),
                    HtsCode3 = row.Cell(12).GetValue<string>().Trim(),
                    Rate3 = GetDecimalValue(row.Cell(13)),
                    HtsCode4 = row.Cell(14).GetValue<string>().Trim(),
                    Rate4 = GetDecimalValue(row.Cell(15)),
                    Remark = row.Cell(16).GetValue<string>().Trim()
                }
            };

            // Mandatory Validation (必填驗證)
            if (string.IsNullOrEmpty(previewRow.NewData.PartNo)) previewRow.Errors.Add("Part No is required.");
            if (string.IsNullOrEmpty(previewRow.NewData.Division)) previewRow.Errors.Add("Division is required.");
            if (string.IsNullOrEmpty(previewRow.NewData.Supplier)) previewRow.Errors.Add("Supplier is required.");
            if (string.IsNullOrEmpty(previewRow.NewData.PartDesc)) previewRow.Errors.Add("Description is required.");

            // Country Mapping
            if (string.IsNullOrEmpty(previewRow.NewData.Country)) previewRow.Errors.Add("Country is required.");
            else if (countries.TryGetValue(previewRow.NewData.Country, out var countryId)) previewRow.NewData.CountryId = countryId;
            else previewRow.Errors.Add($"Country '{previewRow.NewData.Country}' not found.");

            if (previewRow.Errors.Any())
            {
                previewRow.RowStatus = "Error";
                result.Summary.ErrorCount++;
            }
            else
            {
                // Status Mapping (狀態判定)
                previewRow.NewData.Status = string.IsNullOrEmpty(previewRow.NewData.HtsCode) ? "S01" : "S02";

                // Supplier Mapping
                if (suppliers.TryGetValue(previewRow.NewData.Supplier, out var supplierId))
                    previewRow.NewData.SupplierId = supplierId;
                else
                    previewRow.NewData.SupplierId = 0; // Mark for creation

                // Key Matching (Key 比對: PartNo + CustomerID)
                var existing = _repository.GetPartByNo(customerId, previewRow.NewData.PartNo);
                if (existing == null)
                {
                    previewRow.RowStatus = "New";
                    result.Summary.NewCount++;
                }
                else
                {
                    previewRow.OriginalData = MapToDto(existing);
                    if (IsModified(previewRow.OriginalData, previewRow.NewData))
                    {
                        previewRow.RowStatus = "Modified";
                        result.Summary.ModifiedCount++;
                    }
                    else
                    {
                        previewRow.RowStatus = "NoChange";
                        result.Summary.NoChangeCount++;
                    }
                }
            }

            result.Rows.Add(previewRow);
            result.Summary.TotalRows++;
        }

        return result;
    }

    /// <inheritdoc/>
    public BulkUploadConfirmResponseDto ConfirmBulkUpload(List<PartDto> parts)
    {
        var response = new BulkUploadConfirmResponseDto();
        var user = _userContext.UserName ?? "system";
        var now = DateTime.Now;

        // 1. Handle Supplier Deduplication (處理供應商去重)
        var newSuppliers = parts
            .Where(p => p.SupplierId == 0 && !string.IsNullOrEmpty(p.Supplier))
            .Select(p => p.Supplier)
            .Distinct()
            .ToList();

        foreach (var supplierName in newSuppliers)
        {
            var existing = _commonRepository.GetSuppliers().FirstOrDefault(s => s.SupplierName == supplierName);
            if (existing == null)
            {
                var newSupplier = new SupplierEntity
                {
                    Name = supplierName,
                    SupplierName = supplierName,
                    CustomerID = parts.FirstOrDefault(p => p.Supplier == supplierName)?.CustomerId,
                    Status = "Active",
                    CreatedBy = user,
                    CreatedDate = now
                };
                _commonRepository.CreateSupplier(newSupplier);
                
                // Update IDs in the list
                foreach (var p in parts.Where(x => x.Supplier == supplierName)) p.SupplierId = newSupplier.ID;
            }
            else
            {
                foreach (var p in parts.Where(x => x.Supplier == supplierName)) p.SupplierId = existing.ID;
            }
        }

        // 2. Upsert Parts (更新或新增零件)
        foreach (var dto in parts)
        {
            try
            {
                var entity = _repository.GetPartByNo(dto.CustomerId ?? 0, dto.PartNo);
                if (entity == null)
                {
                    entity = new PartEntity
                    {
                        CustomerID = dto.CustomerId ?? 0,
                        PartNo = dto.PartNo,
                        CreatedBy = user,
                        CreatedDate = now
                    };
                    response.Inserted++;
                }
                else
                {
                    response.Updated++;
                }

                entity.CountryID = dto.CountryId ?? 0;
                entity.Division = dto.Division;
                entity.SupplierID = dto.SupplierId ?? 0;
                entity.PartDescription = dto.PartDesc;
                entity.HTSCode = dto.HtsCode;
                entity.DutyRate = dto.Rate;
                entity.AddHTSCode1 = dto.HtsCode1;
                entity.AddDutyRate1 = dto.Rate1;
                entity.AddHTSCode2 = dto.HtsCode2;
                entity.AddDutyRate2 = dto.Rate2;
                entity.AddHTSCode3 = dto.HtsCode3;
                entity.AddDutyRate3 = dto.Rate3;
                entity.AddHTSCode4 = dto.HtsCode4;
                entity.AddDutyRate4 = dto.Rate4;
                entity.Remark = dto.Remark;
                entity.Status = dto.Status;
                entity.UpdatedBy = user;
                entity.UpdatedDate = now;

                if (entity.ID == 0)
                    _repository.CreatePart(entity);
                else
                    _repository.UpdatePart(entity);
            }
            catch (Exception ex)
            {
                response.Failed++;
                response.Errors.Add($"Error processing Part {dto.PartNo}: {ex.Message}");
            }
        }

        return response;
    }

    private decimal GetDecimalValue(IXLCell cell)
    {
        if (cell.IsEmpty()) return 0;
        try { return cell.GetValue<decimal>(); }
        catch { return 0; }
    }

    private PartDto MapToDto(PartEntity entity)
    {
        var countryMap = _commonRepository.GetCountries().ToDictionary(c => c.ID, c => c.Name);
        var supplierMap = _commonRepository.GetSuppliers().ToDictionary(s => s.ID, s => s.SupplierName ?? s.Name);
        return new PartDto
        {
            Id = entity.ID,
            CustomerId = entity.CustomerID,
            PartNo = entity.PartNo ?? "",
            CountryId = entity.CountryID,
            Country = countryMap.TryGetValue(entity.CountryID, out var coName) ? coName : "Unknown",
            Division = entity.Division ?? "",
            SupplierId = entity.SupplierID,
            Supplier = supplierMap.TryGetValue(entity.SupplierID, out var sName) ? sName : "Unknown",
            PartDesc = entity.PartDescription ?? "",
            HtsCode = entity.HTSCode ?? "",
            Rate = entity.DutyRate,
            HtsCode1 = entity.AddHTSCode1,
            Rate1 = entity.AddDutyRate1,
            HtsCode2 = entity.AddHTSCode2,
            Rate2 = entity.AddDutyRate2,
            HtsCode3 = entity.AddHTSCode3,
            Rate3 = entity.AddDutyRate3,
            HtsCode4 = entity.AddHTSCode4,
            Rate4 = entity.AddDutyRate4,
            Remark = entity.Remark ?? "",
            Status = entity.Status ?? ""
        };
    }

    private bool IsModified(PartDto old, PartDto @new)
    {
        return old.CountryId != @new.CountryId ||
               old.Division != @new.Division ||
               old.SupplierId != @new.SupplierId ||
               old.PartDesc != @new.PartDesc ||
               old.HtsCode != @new.HtsCode ||
               old.Rate != @new.Rate ||
               old.HtsCode1 != @new.HtsCode1 ||
               old.Rate1 != @new.Rate1 ||
               old.HtsCode2 != @new.HtsCode2 ||
               old.Rate2 != @new.Rate2 ||
               old.HtsCode3 != @new.HtsCode3 ||
               old.Rate3 != @new.Rate3 ||
               old.HtsCode4 != @new.HtsCode4 ||
               old.Rate4 != @new.Rate4 ||
               old.Remark != @new.Remark;
    }

    /// <inheritdoc/>
    public List<BulkUploadResponseDto> BulkUpload(Stream fileStream) => throw new NotImplementedException("Use PreviewBulkUpload instead.");

    /// <inheritdoc/>
    public byte[] GetUploadTemplate()
    {
        var countries = _commonRepository.GetCountries().ToList();

        using var workbook = new XLWorkbook();
        var templateSheet = workbook.Worksheets.Add("Template");
        var dataSheet = workbook.Worksheets.Add("DataLists");
        dataSheet.Visibility = XLWorksheetVisibility.Hidden;

        // 1. Populate DataLists with Countries (填寫國家清單)
        for (int i = 0; i < countries.Count; i++)
        {
            dataSheet.Cell(i + 1, 1).Value = countries[i].Name;
        }
        
        // Ensure there's at least one cell for validation range even if list is empty
        var countryCount = Math.Max(1, countries.Count);

        // 2. Setup Template Headers (設定範本表頭)
        string[] headers = {
            "Part No", "Country", "Division", "Supplier", "Description", "HTS Code", "Duty Rate (%)",
            "301 Duty Code", "301 Duty Rate (%)", "IEEPA Duty Code", "IEEPA Duty Rate (%)",
            "232 Aluminum Code", "232 Aluminum Rate (%)", "Reciprocal Tariff Code", "Reciprocal Tariff Rate (%)", "Remark"
        };

        for (int i = 0; i < headers.Length; i++)
        {
            var cell = templateSheet.Cell(1, i + 1);
            cell.Value = headers[i];
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = XLColor.LightGray;
        }

        // 3. Add Data Validation for countryName (Column B) (為國家名稱加入資料驗證)
        var validationRange = templateSheet.Range("B2:B1000");
        var validation = validationRange.CreateDataValidation();
        validation.AllowedValues = XLAllowedValues.List;
        validation.List($"='DataLists'!$A$1:$A${countryCount}");
        validation.IgnoreBlanks = true;
        validation.InCellDropdown = true;
        validation.ShowErrorMessage = true;
        validation.ErrorTitle = "Invalid Selection";
        validation.ErrorMessage = "Please select a country from the dropdown list.";

        templateSheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
