using CCH.Core.Entities;
using CCH.Core.Entities.CSP;
using CCH.Core.Entities.ReSm;
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

    public PartExcelService(IPartRepository repository, ICommonRepository commonRepository, IUserContext userContext)
    {
        _repository = repository;
        _commonRepository = commonRepository;
        _userContext = userContext;
    }

    private string[] GetPartExcelHeaders() => new[] { 
        "Customer", "Part No", "Country", "Division", "Supplier", "Description", "HTS Code", "Duty Rate (%)",
        "301 Duty Code", "301 Duty Rate (%)", "IEEPA Duty Code", "IEEPA Duty Rate (%)", 
        "232 Aluminum Code", "232 Aluminum Rate (%)", "Reciprocal Tariff Code", "Reciprocal Tariff Rate (%)",
        "Remark", "Updated By", "Last Updated"
    };

    /// <inheritdoc/>
    public byte[] ExportParts(int? customerId, string? status, string? partNo, int? supplierId)
    {
        var entities = _repository.SearchParts(customerId, status, partNo, supplierId);
        var ctx = GetLookupContext();
        var sMap = _commonRepository.GetStatuses().ToDictionary(s => s.Code, s => s.Description);
        var cMap = _commonRepository.GetCustomers().ToDictionary(c => c.HQID, c => c.CustomerName ?? "Unknown");
        
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Parts");
        string[] expHeaders = { "Customer", "Part No", "Description", "Country", "HTS Code", "Duty Rate (%)", "Status",
                                "301 Duty Code", "301 Duty Rate (%)", "IEEPA Duty Code", "IEEPA Duty Rate (%)", 
                                "232 Aluminum Code", "232 Aluminum Rate (%)", "Reciprocal Tariff Code", "Reciprocal Tariff Rate (%)",
                                "Updated By", "Last Updated" };

        for (int i = 0; i < expHeaders.Length; i++) { ws.Cell(1, i + 1).Value = expHeaders[i]; ws.Cell(1, i + 1).Style.Font.Bold = true; }
        
        var row = 2;
        foreach (var entity in entities) FillPartExcelRow(ws, row++, entity, sMap, cMap, ctx.Countries, ctx.Suppliers);
        
        ws.Columns().AdjustToContents();
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private void FillPartExcelRow(IXLWorksheet ws, int r, PartEntity e, Dictionary<string, string> sm, Dictionary<int, string> cm, Dictionary<string, int> com, Dictionary<string, int> sum)
    {
        ws.Cell(r, 1).Value = cm.TryGetValue(e.CustomerID, out var name) ? name : "Unknown";
        ws.Cell(r, 2).Value = e.PartNo;
        ws.Cell(r, 3).Value = e.PartDescription;
        ws.Cell(r, 4).Value = com.FirstOrDefault(x => x.Value == e.CountryID).Key ?? "Unknown";
        ws.Cell(r, 5).Value = e.HTSCode;
        ws.Cell(r, 6).Value = e.DutyRate;
        ws.Cell(r, 7).Value = sm.TryGetValue(e.Status ?? "", out var d) ? d : e.Status;
        ws.Cell(r, 8).Value = e.AddHTSCode1; ws.Cell(r, 9).Value = e.AddDutyRate1;
        ws.Cell(r, 10).Value = e.AddHTSCode2; ws.Cell(r, 11).Value = e.AddDutyRate2;
        ws.Cell(r, 12).Value = e.AddHTSCode3; ws.Cell(r, 13).Value = e.AddDutyRate3;
        ws.Cell(r, 14).Value = e.AddHTSCode4; ws.Cell(r, 15).Value = e.AddDutyRate4;
        ws.Cell(r, 16).Value = e.UpdatedBy; ws.Cell(r, 17).Value = e.UpdatedDate;
    }

    /// <inheritdoc/>
    public BulkUploadPreviewDto PreviewBulkUpload(int customerId, Stream fileStream)
    {
        /* INTERNAL-AI-20260421: Legacy implementation archived due to DRY/LOC violation. */
        var res = new BulkUploadPreviewDto { Summary = new BulkUploadSummaryDto() };
        var ctx = GetLookupContext();
        using var workbook = new XLWorkbook(fileStream);
        var rows = workbook.Worksheet(1).RowsUsed().Skip(1);

        foreach (var row in rows)
        {
            var pRow = new PartPreviewRowDto { RowIndex = row.RowNumber(), NewData = MapExcelRowToDto(row, customerId) };
            pRow.RowStatus = ValidateAndMapDto(pRow.NewData, ctx.Countries, ctx.Suppliers, pRow.Errors);
            UpdatePreviewSummary(res.Summary, pRow.RowStatus);

            if (pRow.RowStatus != "New" && pRow.RowStatus != "Error")
                pRow.OriginalData = MapToDto(_repository.GetPartByNo(customerId, pRow.NewData.PartNo)!);

            res.Rows.Add(pRow);
            res.Summary.TotalRows++;
        }
        return res;
    }

    private void UpdatePreviewSummary(BulkUploadSummaryDto s, string status)
    {
        if (status == "New") s.NewCount++;
        else if (status == "Modified") s.ModifiedCount++;
        else if (status == "NoChange") s.NoChangeCount++;
        else if (status == "Error") s.ErrorCount++;
    }

    private PartDto MapExcelRowToDto(IXLRow row, int customerId) => new()
    {
        CustomerId = customerId, PartNo = row.Cell(1).GetValue<string>().Trim(),
        Country = row.Cell(2).GetValue<string>().Trim(), Division = row.Cell(3).GetValue<string>().Trim(),
        Supplier = row.Cell(4).GetValue<string>().Trim(), PartDesc = row.Cell(5).GetValue<string>().Trim(),
        HtsCode = row.Cell(6).GetValue<string>().Trim(), Rate = GetDecimalValue(row.Cell(7)),
        HtsCode1 = row.Cell(8).GetValue<string>().Trim(), Rate1 = GetDecimalValue(row.Cell(9)),
        HtsCode2 = row.Cell(10).GetValue<string>().Trim(), Rate2 = GetDecimalValue(row.Cell(11)),
        HtsCode3 = row.Cell(12).GetValue<string>().Trim(), Rate3 = GetDecimalValue(row.Cell(13)),
        HtsCode4 = row.Cell(14).GetValue<string>().Trim(), Rate4 = GetDecimalValue(row.Cell(15)),
        Remark = row.Cell(16).GetValue<string>().Trim()
    };

    /// <inheritdoc/>
    public BulkUploadConfirmResponseDto ConfirmBulkUpload(List<PartDto> parts)
    {
        /* INTERNAL-AI-20260421: Legacy implementation archived due to DRY/LOC violation. */
        var resp = new BulkUploadConfirmResponseDto();
        var ctx = GetLookupContext();
        var user = _userContext.UserName ?? "system";
        var now = DateTime.Now;
        var validParts = new List<PartDto>();

        foreach (var dto in parts)
        {
            var errors = new List<string>();
            var status = ValidateAndMapDto(dto, ctx.Countries, ctx.Suppliers, errors);
            if (status == "Error") { resp.Failed++; resp.Errors.AddRange(errors.Select(e => $"Part {dto.PartNo}: {e}")); }
            else if (status == "New" || status == "Modified") validParts.Add(dto);
        }

        EnsureSuppliersExist(validParts, user, now);
        foreach (var dto in validParts)
        {
            try { UpsertPartEntity(dto, user, now, resp); }
            catch (Exception ex) { resp.Failed++; resp.Errors.Add($"Error processing Part {dto.PartNo}: {ex.Message}"); }
        }
        return resp;
    }

    private (Dictionary<string, int> Countries, Dictionary<string, int> Suppliers) GetLookupContext()
    {
        var co = _commonRepository.GetCountries()
            .Where(c => !string.IsNullOrEmpty(c.Name))
            .GroupBy(c => c.Name, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First().ID, StringComparer.OrdinalIgnoreCase);

        var su = _commonRepository.GetSuppliers()
            .Where(s => !string.IsNullOrEmpty(s.SupplierName))
            .GroupBy(s => s.SupplierName!, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First().ID, StringComparer.OrdinalIgnoreCase);

        return (co, su);
    }

    private string ValidateAndMapDto(PartDto dto, Dictionary<string, int> co, Dictionary<string, int> su, List<string> err)
    {
        if (string.IsNullOrEmpty(dto.PartNo)) err.Add("Part No is required.");
        if (string.IsNullOrEmpty(dto.Division)) err.Add("Division is required.");
        if (string.IsNullOrEmpty(dto.Supplier)) err.Add("Supplier is required.");
        if (string.IsNullOrEmpty(dto.PartDesc)) err.Add("Description is required.");

        if (string.IsNullOrEmpty(dto.Country)) err.Add("Country is required.");
        else if (co.TryGetValue(dto.Country, out var cid)) dto.CountryId = cid;
        else err.Add($"Country '{dto.Country}' not found.");

        if (err.Any()) return "Error";

        dto.Status = string.IsNullOrEmpty(dto.HtsCode) ? "S01" : "S02";
        dto.SupplierId = su.TryGetValue(dto.Supplier, out var sid) ? sid : 0;

        var existing = _repository.GetPartByNo(dto.CustomerId ?? 0, dto.PartNo);
        if (existing == null) return "New";
        return IsModified(MapToDto(existing), dto) ? "Modified" : "NoChange";
    }

    private void EnsureSuppliersExist(List<PartDto> parts, string user, DateTime now)
    {
        var names = parts.Where(p => p.SupplierId == 0 && !string.IsNullOrEmpty(p.Supplier))
            .Select(p => p.Supplier).Distinct().ToList();

        foreach (var name in names)
        {
            var exist = _commonRepository.GetSuppliers().FirstOrDefault(s => s.SupplierName == name);
            if (exist == null)
            {
                var ent = new SupplierEntity { 
                    Name = name, SupplierName = name, Status = "Active", 
                    CustomerID = parts.First(p => p.Supplier == name).CustomerId,
                    CreatedBy = user, CreatedDate = now 
                };
                _commonRepository.CreateSupplier(ent);
                exist = ent;
            }
            foreach (var p in parts.Where(x => x.Supplier == name)) p.SupplierId = exist.ID;
        }
    }

    private void UpsertPartEntity(PartDto dto, string user, DateTime now, BulkUploadConfirmResponseDto resp)
    {
        var e = _repository.GetPartByNo(dto.CustomerId ?? 0, dto.PartNo);
        if (e == null)
        {
            e = new PartEntity { CustomerID = dto.CustomerId ?? 0, PartNo = dto.PartNo, CreatedBy = user, CreatedDate = now };
            resp.Inserted++;
        }
        else resp.Updated++;

        MapDtoToEntity(dto, e, user, now);
        if (e.ID == 0) _repository.CreatePart(e);
        else _repository.UpdatePart(e);
    }

    private void MapDtoToEntity(PartDto d, PartEntity e, string u, DateTime n)
    {
        e.CountryID = d.CountryId ?? 0; e.Division = d.Division; e.SupplierID = d.SupplierId ?? 0;
        e.PartDescription = d.PartDesc; e.HTSCode = d.HtsCode; e.DutyRate = d.Rate;
        e.AddHTSCode1 = d.HtsCode1; e.AddDutyRate1 = d.Rate1; e.AddHTSCode2 = d.HtsCode2; e.AddDutyRate2 = d.Rate2;
        e.AddHTSCode3 = d.HtsCode3; e.AddDutyRate3 = d.Rate3; e.AddHTSCode4 = d.HtsCode4; e.AddDutyRate4 = d.Rate4;
        e.Remark = d.Remark; e.Status = d.Status; e.UpdatedBy = u; e.UpdatedDate = n;
    }

    private decimal GetDecimalValue(IXLCell cell)
    {
        if (cell.IsEmpty()) return 0;
        try { return cell.GetValue<decimal>(); } catch { return 0; }
    }

    /// <summary>
    /// Maps a PartEntity to a PartDto with resolved Country and Supplier names.
    /// (繁體中文) 將 PartEntity 映射至 PartDto，並解析國家與供應商名稱。
    /// </summary>
    private PartDto MapToDto(PartEntity e)
    {
        var coMap = _commonRepository.GetCountries().ToDictionary(c => c.ID, c => c.Name);
        var suMap = _commonRepository.GetSuppliers().ToDictionary(s => s.ID, s => s.SupplierName ?? s.Name);
        return new PartDto {
            Id = e.ID, CustomerId = e.CustomerID, PartNo = e.PartNo ?? "",
            CountryId = e.CountryID, Country = coMap.TryGetValue(e.CountryID, out var c) ? c : "Unknown",
            Division = e.Division ?? "", SupplierId = e.SupplierID, 
            Supplier = suMap.TryGetValue(e.SupplierID, out var s) ? s : "Unknown",
            PartDesc = e.PartDescription ?? "", HtsCode = e.HTSCode ?? "", Rate = e.DutyRate,
            HtsCode1 = e.AddHTSCode1, Rate1 = e.AddDutyRate1, HtsCode2 = e.AddHTSCode2, Rate2 = e.AddDutyRate2,
            HtsCode3 = e.AddHTSCode3, Rate3 = e.AddDutyRate3, HtsCode4 = e.AddHTSCode4, Rate4 = e.AddDutyRate4,
            Remark = e.Remark ?? "", Status = e.Status ?? ""
        };
    }

    private bool IsModified(PartDto o, PartDto n)
    {
        return o.CountryId != n.CountryId || o.Division != n.Division || o.SupplierId != n.SupplierId ||
               o.PartDesc != n.PartDesc || o.HtsCode != n.HtsCode || o.Rate != n.Rate ||
               o.HtsCode1 != n.HtsCode1 || o.Rate1 != n.Rate1 || o.HtsCode2 != n.HtsCode2 || o.Rate2 != n.Rate2 ||
               o.HtsCode3 != n.HtsCode3 || o.Rate3 != n.Rate3 || o.HtsCode4 != n.HtsCode4 || o.Rate4 != n.Rate4 ||
               o.Remark != n.Remark;
    }

    /// <inheritdoc/>
    public byte[] GetUploadTemplate()
    {
        var co = _commonRepository.GetCountries().ToList();
        using var workbook = new XLWorkbook();
        var ts = workbook.Worksheets.Add("Template");
        var ds = workbook.Worksheets.Add("DataLists");
        ds.Visibility = XLWorksheetVisibility.Hidden;
        for (int i = 0; i < co.Count; i++) ds.Cell(i + 1, 1).Value = co[i].Name;
        
        string[] tmpHeaders = { "Part No", "Country", "Division", "Supplier", "Description", "HTS Code", "Duty Rate (%)",
                                "301 Duty Code", "301 Duty Rate (%)", "IEEPA Duty Code", "IEEPA Duty Rate (%)", 
                                "232 Aluminum Code", "232 Aluminum Rate (%)", "Reciprocal Tariff Code", "Reciprocal Tariff Rate (%)", "Remark" };

        for (int i = 0; i < tmpHeaders.Length; i++) {
            var c = ts.Cell(1, i + 1); c.Value = tmpHeaders[i];
            c.Style.Font.Bold = true; c.Style.Fill.BackgroundColor = XLColor.LightGray;
        }

        SetupTemplateValidation(ts, Math.Max(1, co.Count));
        ts.Columns().AdjustToContents();
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private void SetupTemplateValidation(IXLWorksheet s, int c)
    {
        var v = s.Range("B2:B1000").CreateDataValidation();
        v.AllowedValues = XLAllowedValues.List; v.List($"='DataLists'!$A$1:$A${c}");
        v.IgnoreBlanks = true; v.InCellDropdown = true; v.ShowErrorMessage = true;
        v.ErrorTitle = "Invalid Selection"; v.ErrorMessage = "Please select a country.";
    }
}
