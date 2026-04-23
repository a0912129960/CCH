using CCH.Core.Entities.CSP;
using CCH.Core.Features.Parts.DTOs;
using CCH.Core.Features.Parts.Interfaces;
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

    public PartExcelService(IPartRepository repository, ICommonRepository commonRepository)
    {
        _repository = repository;
        _commonRepository = commonRepository;
    }

    /// <inheritdoc/>
    public byte[] ExportParts(int? customerId, string? status, string? partNo, int? supplierId)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Parts");

        string[] headers = { "Customer", "Part No", "Description", "Country", "Supplier", "HTS Code", "Rate", "Status" };
        for (int i = 0; i < headers.Length; i++) worksheet.Cell(1, i + 1).Value = headers[i];

        var parts = _repository.SearchParts(customerId, status, partNo, supplierId).ToList();
        var customers = _commonRepository.GetCustomers().ToDictionary(c => c.HQID, c => c.CustomerName ?? "Unknown");
        var countries = _commonRepository.GetCountries().ToDictionary(c => c.ID, c => c.Name);
        var suppliers = _commonRepository.GetSuppliers().ToDictionary(s => s.ID, s => s.SupplierName ?? "Unknown");

        for (int i = 0; i < parts.Count; i++)
        {
            var p = parts[i];
            var row = i + 2;
            worksheet.Cell(row, 1).Value = customers.GetValueOrDefault(p.CustomerID ?? 0, "Unknown");
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
    public BulkUploadPreviewDto PreviewBulkUpload(int customerId, Stream fileStream)
    {
        using var workbook = new XLWorkbook(fileStream);
        var worksheet = workbook.Worksheet(1);
        var rows = worksheet.RowsUsed().Skip(1);

        var previewRows = new List<PartPreviewRowDto>();
        var summary = new BulkUploadSummaryDto();

        var countries = _commonRepository.GetCountries().ToDictionary(c => c.Name.ToLower(), c => c.ID);
        var countriesById = _commonRepository.GetCountries().ToDictionary(c => c.ID, c => c.Name);
        var suppliers = _commonRepository.GetSuppliers(customerId).ToDictionary(s => s.SupplierName?.ToLower() ?? "", s => s.ID);
        var suppliersById = _commonRepository.GetSuppliers(customerId).ToDictionary(s => s.ID, s => s.SupplierName ?? "Unknown");

        foreach (var row in rows)
        {
            summary.TotalRows++;
            var partNo = row.Cell(1).GetString();
            if (string.IsNullOrWhiteSpace(partNo)) { summary.TotalRows--; continue; }

            var errors = new List<string>();
            var countryName = row.Cell(3).GetString().ToLower();
            var supplierName = row.Cell(4).GetString().ToLower();

            if (!countries.ContainsKey(countryName)) errors.Add($"Country '{countryName}' not found.");
            if (!suppliers.ContainsKey(supplierName)) errors.Add($"Supplier '{supplierName}' not found.");

            row.Cell(6).TryGetValue(out decimal rate);

            var newData = new PartDto
            {
                CustomerId = customerId,
                PartNo = partNo,
                PartDesc = row.Cell(2).GetString(),
                CountryId = countries.GetValueOrDefault(countryName, 0),
                SupplierId = suppliers.GetValueOrDefault(supplierName, 0),
                HtsCode = row.Cell(5).GetString(),
                Rate = rate
            };

            var existing = _repository.GetPartByNo(customerId, partNo);
            PartDto? originalData = null;
            var rowStatus = "New";

            if (existing != null)
            {
                originalData = new PartDto
                {
                    CustomerId = existing.CustomerID ?? 0,
                    PartNo = existing.PartNo ?? "",
                    PartDesc = existing.PartDescription ?? "",
                    CountryId = existing.CountryID ?? 0,
                    SupplierId = existing.SupplierID ?? 0,
                    HtsCode = existing.HTSCode ?? "",
                    Rate = existing.DutyRate ?? 0
                };

                rowStatus = (newData.PartDesc == originalData.PartDesc && 
                            newData.CountryId == originalData.CountryId && 
                            newData.SupplierId == originalData.SupplierId && 
                            newData.HtsCode == originalData.HtsCode && 
                            newData.Rate == originalData.Rate) ? "NoChange" : "Modified";
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

    /// <inheritdoc/>
    public BulkUploadConfirmResponseDto ConfirmBulkUpload(List<PartDto> parts)
    {
        var result = new BulkUploadConfirmResponseDto();
        foreach (var p in parts)
        {
            try
            {
                var customerId = p.CustomerId ?? 0;
                var existing = _repository.GetPartByNo(customerId, p.PartNo);
                if (existing != null)
                {
                    existing.PartDescription = p.PartDesc;
                    existing.CountryID = p.CountryId;
                    existing.SupplierID = p.SupplierId;
                    existing.HTSCode = p.HtsCode;
                    existing.DutyRate = p.Rate;
                    existing.Status = "S01";
                    _repository.UpdatePart(existing);
                    result.Updated++;
                }
                else
                {
                    _repository.CreatePart(new CchParts
                    {
                        CustomerID = p.CustomerId,
                        PartNo = p.PartNo,
                        PartDescription = p.PartDesc,
                        CountryID = p.CountryId,
                        SupplierID = p.SupplierId,
                        HTSCode = p.HtsCode,
                        DutyRate = p.Rate,
                        Status = "S01"
                    });
                    result.Inserted++;
                }
            }
            catch (Exception ex)
            {
                result.Failed++;
                result.Errors.Add($"{p.PartNo}: {ex.Message}");
            }
        }
        return result;
    }

    /// <inheritdoc/>
    public byte[] GetUploadTemplate()
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Template");
        string[] headers = { "Part No*", "Description", "Country Name*", "Supplier Name*", "HTS Code", "Duty Rate" };
        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cell(1, i + 1).Value = headers[i];
            worksheet.Cell(1, i + 1).Style.Font.SetBold();
        }
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
