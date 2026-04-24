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
    private readonly IPartValidationService _validationService;

    public PartExcelService(IPartRepository repository, ICommonRepository commonRepository, IUserContext userContext, IPartValidationService validationService)
    {
        _repository = repository;
        _commonRepository = commonRepository;
        _userContext = userContext;
        _validationService = validationService;
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
    public async Task<BulkUploadPreviewDto> PreviewBulkUpload(int projectId, Stream fileStream)
    {
        using var workbook = new XLWorkbook(fileStream);
        var worksheet = workbook.Worksheet(1);
        var rows = worksheet.RowsUsed().Skip(1);

        var previewRows = new List<PartPreviewRowDto>();
        var summary = new BulkUploadSummaryDto();
        var role = _userContext.Role ?? "Unknown";

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

            var countryId = countries.GetValueOrDefault(countryName, 0);
            if (countryId == 0) errors.Add($"Country: '{countryName}' not found.");
            
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
                CountryId = countryId,
                SupplierId = sId
            };

            // Use the triple key (Project, PartNo, Country) for precise duplicate detection
            var existing = countryId != 0 ? _repository.GetPartByNoAndCountry(projectId, partNo, countryId) : null;
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
                    Remark = existing.Remark ?? "",
                    Status = existing.Status
                };

                newData.Status = existing.Status;
                // Since only new entries are allowed, set RowStatus to Error if found
                rowStatus = "Error";
            }

            // Perform Rule-based validation (ValidationService will add "Already exists" error if isNew = false)
            var validationResult = await _validationService.ValidateExcelRowAsync(newData, originalData, role, existing == null);
            
            // Add hard errors (Must be corrected)
            foreach (var ve in validationResult.Errors) errors.Add($"{ve.Field}: {ve.Message}");
            
            // Combine all messages for display (Hard Errors + Informational Warnings)
            var displayMessages = new List<string>(errors);
            foreach (var vw in validationResult.Warnings) displayMessages.Add(vw); // Remove "Hint: " prefix

            if (existing == null && countryId != 0)
            {
                if (_repository.ExistsByPartNoAndCountry(projectId, partNo, countryId))
                {
                    var msg = "Part No: This (Customer, Part No, Country) combination already exists.";
                    errors.Add(msg);
                    displayMessages.Add(msg);
                }
            }

            // Row is "Error" only if there are hard errors (Validation errors, Duplicate detection, or Metadata missing)
            if (validationResult.Errors.Any() || errors.Any(e => e.StartsWith("Country:") || e.StartsWith("Part No:")))
            {
                rowStatus = "Error";
            }

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
                Errors = displayMessages,
                NewData = newData,
                OriginalData = originalData
            });
        }

        return new BulkUploadPreviewDto { Summary = summary, Rows = previewRows };
    }
    
    /// <inheritdoc/>
    public async Task<BulkUploadConfirmResponseDto> ConfirmBulkUpload(List<PartDto> parts)
    {
        var result = new BulkUploadConfirmResponseDto();
        var now = DateTime.Now;
        var role = _userContext.Role ?? "Unknown";

        // 2. Persist Parts with final validation gate
        foreach (var p in parts)
        {
            try
            {
                var projectId = p.ProjectId ?? 0;
                var countryId = p.CountryId ?? 0;
                
                // Precise lookup using the triple key (Only New entries allowed)
                var existing = countryId != 0 ? _repository.GetPartByNoAndCountry(projectId, p.PartNo, countryId) : null;
                PartDto? originalData = null;
                
                if (existing != null)
                {
                    originalData = new PartDto
                    {
                        ProjectId = existing.ProjectID ?? 0,
                        PartNo = existing.PartNo ?? "",
                        CountryId = existing.CountryID ?? 0,
                        Division = existing.Division ?? "",
                        Supplier = _commonRepository.GetSuppliers().FirstOrDefault(x => x.ID == existing.SupplierID)?.SupplierName ?? "Unknown",
                        PartDesc = existing.PartDescription ?? "",
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
                        Remark = existing.Remark ?? "",
                        Status = existing.Status
                    };
                    p.Status = existing.Status;
                }

                // Pass isNew = (existing == null) to ValidationService (which enforces Only New)
                var validationResult = await _validationService.ValidateExcelRowAsync(p, originalData, role, existing == null);
                if (validationResult.Errors.Any())
                {
                    result.Failed++;
                    result.Errors.Add($"{p.PartNo}: {string.Join(" | ", validationResult.Errors.Select(ve => ve.Message))}");
                    continue;
                }

                CchParts target;
                // Auto-resolve or create supplier if valid
                var sId = p.SupplierId ?? 0;
                if (sId == 0 && !string.IsNullOrEmpty(p.Supplier))
                {
                    var s = _commonRepository.GetSuppliers().FirstOrDefault(x => x.SupplierName == p.Supplier);
                    if (s == null)
                    {
                        var newS = new CchSuppliers { SupplierName = p.Supplier, ProjectID = projectId, Status = "Active", CreatedBy = CurrentUser, CreatedDate = now };
                        _commonRepository.CreateSupplier(newS);
                        sId = newS.ID;
                    }
                    else sId = s.ID;
                }

                if (existing != null)
                {
                    // This block should technically be unreachable due to validationResult above,
                    // but keeping for structural integrity while marking failure just in case.
                    result.Failed++;
                    result.Errors.Add($"{p.PartNo}: This part already exists.");
                    continue;
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
                        SupplierID = sId,
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
                        UpdatedBy = CurrentUser,
                        IsHTSExists = validationResult.IsHTSExists ?? false
                    };
                    _repository.CreatePart(target);
                    result.Inserted++;
                }
                RecordHistory(target.ID, "Bulk Upload", null, "S01");
                RecordSnapshot(target);
            }
            catch (Exception ex)
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
            CreatedDate = DateTime.Now,
            IsHTSExists = entity.IsHTSExists
        });
    }

    private void RecordHistory(int partId, string action, string? fromStatus, string? toStatus, string remark = "")
    {
        _repository.AddHistory(new CchPartMilestones
        {
            PartID = partId,
            Action = action,
            FromStatus = fromStatus,
            ToStatus = toStatus,
            Remark = remark,
            CreatedBy = CurrentUser,
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

        // 4. Add Format Validation for HTS Columns
        // All HTS Columns (F, H, J, L, N): Strict 10 digits
        var htsColumns = new[] { "F", "H", "J", "L", "N" };
        foreach (var col in htsColumns)
        {
            var range = templateSheet.Range($"{col}2:{col}1000");
            var val = range.CreateDataValidation();
            val.AllowedValues = XLAllowedValues.Custom;
            // Formula: OR(ISBLANK(cell), AND(LEN(SUBSTITUTE(cell,".",""))=10, ISNUMBER(--SUBSTITUTE(cell,".",""))))
            val.List($"=OR(ISBLANK({col}2), AND(LEN(SUBSTITUTE({col}2,\".\",\"\"))=10, ISNUMBER(--SUBSTITUTE({col}2,\".\",\"\"))))");
            val.IgnoreBlanks = true;
            val.ShowInputMessage = true;
            val.InputTitle = "HTS Code Format";
            val.InputMessage = "Must be 10 digits. Format: 1234.56.7890 or 1234567890";
            val.ShowErrorMessage = true;
            val.ErrorTitle = "Invalid Format";
            val.ErrorMessage = "Invalid HTS Code. Must be exactly 10 digits.";
        }

        templateSheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
