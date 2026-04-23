using CCH.Core.Entities.CSP;
using CCH.Core.Features.Parts.DTOs;
using CCH.Core.Features.Parts.Interfaces;
using CCH.Core.Interfaces;
using CCH.Core.Interfaces.Repositories;
using ClosedXML.Excel;

namespace CCH.Services.Features.Parts;

/// <summary>
/// Implementation of Part Excel operations (Import, Export, Template) using ClosedXML.
/// (繁體中文) 零件 Excel 操作實作（匯入、匯出、範本），使用 ClosedXML。
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

    // Use UserId (short login ID) for DB fields that have MaxLength(10). (使用簡短的登入 ID 寫入有長度限制的 DB 欄位。)
    private string CurrentUser => (_userContext.UserId ?? "system")[..Math.Min((_userContext.UserId ?? "system").Length, 10)];

    /// <inheritdoc/>
    public byte[] ExportParts(int? projectId, string? status, string? partNo, int? supplierId)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Parts");

        string[] headers = { "Customer", "Part No", "Description", "Country", "Supplier", "HTS Code", "Rate", "Status" };
        for (int i = 0; i < headers.Length; i++) worksheet.Cell(1, i + 1).Value = headers[i];

        var parts = _repository.SearchParts(projectId, status, partNo, supplierId).ToList();
        var projects = _commonRepository.GetProjects().ToDictionary(p => p.Id, p => p.ProjectName ?? "Unknown");
        var countries = _commonRepository.GetCountries().ToDictionary(c => c.ID, c => c.Name);
        var suppliers = _commonRepository.GetSuppliers().ToDictionary(s => s.ID, s => s.SupplierName ?? "Unknown");

        for (int i = 0; i < parts.Count; i++)
        {
            var p = parts[i];
            var row = i + 2;
            worksheet.Cell(row, 1).Value = projects.GetValueOrDefault(p.ProjectID ?? 0, "Unknown");
            worksheet.Cell(row, 2).Value = p.PartNo;
            worksheet.Cell(row, 3).Value = p.PartDescription;
            worksheet.Cell(row, 4).Value = countries.GetValueOrDefault(p.CountryID ?? 0, "Unknown");
            worksheet.Cell(row, 5).Value = suppliers.GetValueOrDefault(p.SupplierID ?? 0, "Unknown");
            worksheet.Cell(row, 6).Value = p.HTSCode;
            worksheet.Cell(row, 7).Value = p.DutyRate ?? 0;
            worksheet.Cell(row, 8).Value = p.Status;
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    /// <inheritdoc/>
    public BulkUploadPreviewDto PreviewBulkUpload(int projectId, Stream fileStream)
    {
        using var workbook = new XLWorkbook(fileStream);
        var worksheet = workbook.Worksheet(1);
        var rows = worksheet.RowsUsed().Skip(1);

        var previewRows = new List<PartPreviewRowDto>();
        var summary = new BulkUploadSummaryDto();

        // Handle duplicates by taking the first entry for each name
        var countries = _commonRepository.GetCountries()
            .GroupBy(c => c.Name.ToLower())
            .ToDictionary(g => g.Key, g => g.First().ID);

        var suppliers = _commonRepository.GetSuppliers(projectId)
            .GroupBy(s => s.SupplierName?.ToLower() ?? "")
            .ToDictionary(g => g.Key, g => g.First().ID);

        foreach (var row in rows)
        {
            summary.TotalRows++;
            var partNo = row.Cell(1).GetString();
            if (string.IsNullOrWhiteSpace(partNo)) { summary.TotalRows--; continue; }

            var errors = new List<string>();
            var countryName = row.Cell(2).GetString().ToLower(); // Target Column B
            var supplierName = row.Cell(4).GetString(); 

            if (!countries.ContainsKey(countryName)) errors.Add($"Country '{countryName}' not found.");
            
            var sId = suppliers.GetValueOrDefault(supplierName.ToLower(), 0);

            var newData = new PartDto
            {
                ProjectId = projectId,
                PartNo = partNo,
                Country = row.Cell(2).GetString(),
                Division = row.Cell(3).GetString(),
                Supplier = supplierName,
                PartDesc = row.Cell(5).GetString(),
                HtsCode = row.Cell(6).GetString(),
                Rate = GetDecimalValue(row.Cell(7)),
                HtsCode1 = row.Cell(8).GetString(),
                Rate1 = GetDecimalValue(row.Cell(9)),
                HtsCode2 = row.Cell(10).GetString(),
                Rate2 = GetDecimalValue(row.Cell(11)),
                HtsCode3 = row.Cell(12).GetString(),
                Rate3 = GetDecimalValue(row.Cell(13)),
                HtsCode4 = row.Cell(14).GetString(),
                Rate4 = GetDecimalValue(row.Cell(15)),
                Remark = row.Cell(16).GetString(),
                CountryId = countries.GetValueOrDefault(countryName, 0),
                SupplierId = sId
            };

            var existing = _repository.GetPartByNo(projectId, partNo);
            PartDto? originalData = null;
            var rowStatus = "New";

            if (existing != null)
            {
                originalData = new PartDto
                {
                    ProjectId = existing.ProjectID ?? 0,
                    PartNo = existing.PartNo ?? "",
                    PartDesc = existing.PartDescription ?? "",
                    CountryId = existing.CountryID ?? 0,
                    Country = _commonRepository.GetCountries().FirstOrDefault(x => x.ID == existing.CountryID)?.Name ?? "Unknown",
                    Division = existing.Division ?? "",
                    SupplierId = existing.SupplierID ?? 0,
                    Supplier = _commonRepository.GetSuppliers().FirstOrDefault(x => x.ID == existing.SupplierID)?.SupplierName ?? "Unknown",
                    HtsCode = existing.HTSCode ?? "",
                    Rate = existing.DutyRate ?? 0,
                    HtsCode1 = existing.AddHTSCode1,
                    Rate1 = existing.AddDutyRate1,
                    HtsCode2 = existing.AddHTSCode2,
                    Rate2 = existing.AddDutyRate2,
                    HtsCode3 = existing.AddHTSCode3,
                    Rate3 = existing.AddDutyRate3,
                    HtsCode4 = existing.AddHTSCode4,
                    Rate4 = existing.AddDutyRate4,
                    Remark = existing.Remark ?? ""
                };

                rowStatus = IsModified(originalData, newData) ? "Modified" : "NoChange";
            }

            if (errors.Any()) rowStatus = "Error";

            switch (rowStatus)
            {
                case "New": summary.NewCount++; break;
                case "Modified": summary.ModifiedCount++; break;
                case "Error": summary.ErrorCount++; break;
                case "NoChange": summary.NoChangeCount++; break;
            }

            previewRows.Add(new PartPreviewRowDto
            {
                RowIndex = row.RowNumber(),
                RowStatus = rowStatus,
                Errors = errors,
                NewData = newData,
                OriginalData = originalData
            });
        }

        return new BulkUploadPreviewDto { Summary = summary, Rows = previewRows };
    }

    private bool IsModified(PartDto old, PartDto @new)
    {
        return old.CountryId != @new.CountryId ||
               old.Division != @new.Division ||
               old.Supplier != @new.Supplier || // Supplier is handled by name lookup during confirm
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
    public BulkUploadConfirmResponseDto ConfirmBulkUpload(List<PartDto> parts)
    {
        var result = new BulkUploadConfirmResponseDto();
        var now = DateTime.Now;

        // 1. Restore Auto-Create Suppliers with Deduplication
        var newSupplierNames = parts
            .Where(p => p.SupplierId == 0 && !string.IsNullOrEmpty(p.Supplier))
            .Select(p => p.Supplier).Distinct().ToList();

        foreach (var name in newSupplierNames)
        {
            var existing = _commonRepository.GetSuppliers().FirstOrDefault(s => s.SupplierName == name);
            if (existing == null)
            {
            var newS = new CchSuppliers
            {
                SupplierName = name,
                ProjectID = parts.FirstOrDefault(p => p.Supplier == name)?.ProjectId,
                Status = "Active",
                CreatedBy = CurrentUser,
                CreatedDate = now
            };
            _commonRepository.CreateSupplier(newS);
            existing = newS;
            }
            foreach (var p in parts.Where(x => x.Supplier == name)) p.SupplierId = existing.ID;
            }

            // 2. Persist Parts
            foreach (var p in parts)
            {
            try
            {
            var projectId = p.ProjectId ?? 0;
            var existing = _repository.GetPartByNo(projectId, p.PartNo);
            CchParts target;

            if (existing != null)
            {
                existing.PartDescription = p.PartDesc;
                existing.CountryID = p.CountryId;
                existing.Division = p.Division;
                existing.SupplierID = p.SupplierId;
                existing.HTSCode = p.HtsCode;
                existing.DutyRate = p.Rate;
                existing.AddHTSCode1 = p.HtsCode1;
                existing.AddDutyRate1 = p.Rate1;
                existing.AddHTSCode2 = p.HtsCode2;
                existing.AddDutyRate2 = p.Rate2;
                existing.AddHTSCode3 = p.HtsCode3;
                existing.AddDutyRate3 = p.Rate3;
                existing.AddHTSCode4 = p.HtsCode4;
                existing.AddDutyRate4 = p.Rate4;
                existing.Remark = p.Remark;
                existing.Status = "S01";
                existing.UpdatedBy = CurrentUser;
                _repository.UpdatePart(existing);
                target = existing;
                result.Updated++;
            }
            else
            {
                target = new CchParts
                {
                    ProjectID = p.ProjectId,
                    PartNo = p.PartNo,
                    PartDescription = p.PartDesc,
                    CountryID = p.CountryId,
                    Division = p.Division,
                    SupplierID = p.SupplierId,
                    HTSCode = p.HtsCode,
                    DutyRate = p.Rate,
                    AddHTSCode1 = p.HtsCode1,
                    AddDutyRate1 = p.Rate1,
                    AddHTSCode2 = p.HtsCode2,
                    AddDutyRate2 = p.Rate2,
                    AddHTSCode3 = p.HtsCode3,
                    AddDutyRate3 = p.Rate3,
                    AddHTSCode4 = p.HtsCode4,
                    AddDutyRate4 = p.Rate4,
                    Remark = p.Remark,
                    Status = "S01",
                    CreatedBy = CurrentUser,
                    UpdatedBy = CurrentUser
                };
                _repository.CreatePart(target);
                result.Inserted++;
            }

            RecordSnapshot(target);
            }            catch (Exception ex)
            {
                result.Failed++;
                result.Errors.Add($"{p.PartNo}: {ex.Message}");
            }
        }
        return result;
    }

    private void RecordSnapshot(CchParts entity)
    {
        var countryName = _commonRepository.GetCountries().FirstOrDefault(c => c.ID == entity.CountryID)?.Name ?? "Unknown";
        var supplierName = _commonRepository.GetSuppliers().FirstOrDefault(s => s.ID == entity.SupplierID)?.SupplierName ?? "Unknown";

        _repository.AddSnapshot(new CchPartHistories
        {
            PartID = entity.ID,
            PartNo = entity.PartNo,
            Country = countryName,
            Division = entity.Division,
            Supplier = supplierName,
            PartDesc = entity.PartDescription,
            HtsCode = entity.HTSCode,
            Rate = entity.DutyRate,
            HtsCode1 = entity.AddHTSCode1,
            Rate1 = entity.AddDutyRate1,
            HtsCode2 = entity.AddHTSCode2,
            Rate2 = entity.AddDutyRate2,
            HtsCode3 = entity.AddHTSCode3,
            Rate3 = entity.AddDutyRate3,
            HtsCode4 = entity.AddHTSCode4,
            Rate4 = entity.AddDutyRate4,
            Remark = entity.Remark,
            CreatedBy = entity.UpdatedBy ?? "system",
            CreatedDate = DateTime.Now
        });
    }

    private decimal GetDecimalValue(IXLCell cell)
    {
        if (cell.IsEmpty()) return 0;
        try { return cell.GetValue<decimal>(); } catch { return 0; }
    }

    /// <inheritdoc/>
    public byte[] GetUploadTemplate()
    {
        var countries = _commonRepository.GetCountries().ToList();

        using var workbook = new XLWorkbook();
        var templateSheet = workbook.Worksheets.Add("Template");
        var dataSheet = workbook.Worksheets.Add("DataLists");
        dataSheet.Visibility = XLWorksheetVisibility.Hidden;

        // 1. Populate DataLists with Countries
        for (int i = 0; i < countries.Count; i++)
        {
            dataSheet.Cell(i + 1, 1).Value = countries[i].Name;
        }
        
        var countryCount = Math.Max(1, countries.Count);

        // 2. Setup Template Headers
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

        // 3. Add Data Validation for countryName (Column B)
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
