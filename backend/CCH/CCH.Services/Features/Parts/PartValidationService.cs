using CCH.Core.Features.Parts.DTOs;
using CCH.Core.Features.Parts.Interfaces;
using CCH.Core.Features.Hts.Interfaces;
using System.Text.RegularExpressions;

namespace CCH.Services.Features.Parts;

/// <summary>
/// Implementation of Part validation rules.
/// (繁體中文) 零件驗證規則實作。
/// </summary>
public class PartValidationService : IPartValidationService
{
    // Strict regex: exactly 10 digits (dotted or plain) for all HTS fields
    private static readonly Regex HtsFormatRegex = new(@"^(\d{4}\.\d{2}\.\d{4}|\d{10})$", RegexOptions.Compiled);
    
    private readonly IHtsRecommendationService _htsService;

    public PartValidationService(IHtsRecommendationService htsService)
    {
        _htsService = htsService;
    }

    /// <inheritdoc/>
    public async Task<ValidationResult> ValidateExcelRowAsync(PartDto newData, PartDto? originalData, string role, bool isNew)
    {
        var result = new ValidationResult();
        var action = isNew ? "CREATE_SAVE" : "UPDATE_SAVE";

        // 0. Only New Entries Restriction (Excel specific logic handled by isNew flag)
        if (!isNew)
        {
            result.Errors.Add(("Part No", "This part already exists. Bulk upload only supports new entries."));
            return result;
        }

        // 1. Required Fields
        if (isNew)
        {
            if (newData.ProjectId == null || newData.ProjectId == 0) result.Errors.Add(("Customer", "Customer is required."));
            if (string.IsNullOrWhiteSpace(newData.PartNo)) result.Errors.Add(("Part No", "Part No is required."));
            if (newData.CountryId == null || newData.CountryId == 0) result.Errors.Add(("Country", "Country is required."));
        }
        else
        {
            if (newData.ProjectId == null || newData.ProjectId == 0) result.Errors.Add(("Customer", "Customer is required."));
            if (string.IsNullOrWhiteSpace(newData.PartNo)) result.Errors.Add(("Part No", "Part No is required."));
            if (newData.CountryId == null || newData.CountryId == 0) result.Errors.Add(("Country", "Country is required."));
        }

        // 3. Immutability Constraint
        if (!isNew && originalData != null)
        {
            if (newData.ProjectId != originalData.ProjectId) result.Errors.Add(("Customer", "Customer cannot be changed after creation."));
            if (newData.PartNo != originalData.PartNo) result.Errors.Add(("Part No", "Part No cannot be changed after creation."));
            if (newData.CountryId != originalData.CountryId) result.Errors.Add(("Country", "Country cannot be changed after creation."));
        }

        // 4. Status-based Editability
        if (!isNew && originalData != null)
        {
            var status = newData.Status;
            bool isBasicEditable = status == "S01" || status == "S03" || status == "S04" || string.IsNullOrEmpty(status);
            if (!isBasicEditable)
            {
                if (newData.Division != originalData.Division) result.Errors.Add(("Division", $"Cannot edit Division in status {status}."));
                if (newData.Supplier != originalData.Supplier) result.Errors.Add(("Supplier", $"Cannot edit Supplier in status {status}."));
                if (newData.PartDesc != originalData.PartDesc) result.Errors.Add(("Description", $"Cannot edit Description in status {status}."));
                if (newData.HtsCode != originalData.HtsCode) result.Errors.Add(("HTS Code", $"Cannot edit HTS Code in status {status}."));
                if (newData.Rate != originalData.Rate) result.Errors.Add(("Duty Rate", $"Cannot edit Duty Rate in status {status}."));
            }

            bool isAddEditable = status == "S02" || status == "S04";
            if (!isAddEditable)
            {
                if (newData.HtsCode1 != originalData.HtsCode1) result.Errors.Add(("301 Duty Code", $"Cannot edit 301 Duty Code in status {status}."));
                if (newData.Rate1 != originalData.Rate1) result.Errors.Add(("301 Duty Rate", $"Cannot edit 301 Duty Rate in status {status}."));
                if (newData.HtsCode2 != originalData.HtsCode2) result.Errors.Add(("IEEPA Duty Code", $"Cannot edit IEEPA Duty Code in status {status}."));
                if (newData.Rate2 != originalData.Rate2) result.Errors.Add(("IEEPA Duty Rate", $"Cannot edit IEEPA Duty Rate in status {status}."));
                if (newData.HtsCode3 != originalData.HtsCode3) result.Errors.Add(("232 Aluminum Code", $"Cannot edit 232 Aluminum Code in status {status}."));
                if (newData.Rate3 != originalData.Rate3) result.Errors.Add(("232 Aluminum Rate", $"Cannot edit 232 Aluminum Rate in status {status}."));
                if (newData.HtsCode4 != originalData.HtsCode4) result.Errors.Add(("Reciprocal Tariff Code", $"Cannot edit Reciprocal Tariff Code in status {status}."));
                if (newData.Rate4 != originalData.Rate4) result.Errors.Add(("Reciprocal Tariff Rate", $"Cannot edit Reciprocal Tariff Rate in status {status}."));
                if (newData.Remark != originalData.Remark) result.Errors.Add(("Remark", $"Cannot edit Remark in status {status}."));
            }
        }

        // 5. Format & Existence Validation (HTS)
        // Only the primary HTS Code undergoes existence verification (只需判斷 HTS Code 是否存在)
        bool htsExists = await ValidateHtsInternalAsync(newData.HtsCode, "HTS Code", result, checkExistence: true);
        
        // Other codes undergo strict format validation (必須為 10 位)
        CheckHtsFormatOnly(newData.HtsCode1, "301 Duty Code", result);
        CheckHtsFormatOnly(newData.HtsCode2, "IEEPA Duty Code", result);
        CheckHtsFormatOnly(newData.HtsCode3, "232 Aluminum Code", result);
        CheckHtsFormatOnly(newData.HtsCode4, "Reciprocal Tariff Code", result);

        result.IsHTSExists = htsExists;

        if (newData.Rate < 0) result.Errors.Add(("Duty Rate", "Duty Rate must be a non-negative number."));
        if (newData.Rate1 < 0) result.Errors.Add(("301 Duty Rate", "301 Duty Rate must be a non-negative number."));
        if (newData.Rate2 < 0) result.Errors.Add(("IEEPA Duty Rate", "IEEPA Duty Rate must be a non-negative number."));
        if (newData.Rate3 < 0) result.Errors.Add(("232 Aluminum Rate", "232 Aluminum Rate must be a non-negative number."));
        if (newData.Rate4 < 0) result.Errors.Add(("Reciprocal Tariff Rate", "Reciprocal Tariff Rate must be a non-negative number."));

        return result;
    }

    private void CheckHtsFormatOnly(string? hts, string label, ValidationResult result)
    {
        if (!string.IsNullOrWhiteSpace(hts) && !HtsFormatRegex.IsMatch(hts))
        {
            result.Errors.Add((label, $"Invalid {label}. Must be exactly 10 digits. Format: 1234.56.7890 or 1234567890"));
        }
    }

    private async Task<bool> ValidateHtsInternalAsync(string? hts, string label, ValidationResult result, bool checkExistence)
    {
        if (string.IsNullOrWhiteSpace(hts)) return false;

        // 1. Format check - All HTS fields require 10 digits
        if (!HtsFormatRegex.IsMatch(hts))
        {
            result.Errors.Add((label, $"Invalid {label}. Must be exactly 10 digits. Format: 1234.56.7890 or 1234567890"));
            return false;
        }

        if (!checkExistence) return false;

        // 2. Existence check (Normalize to numeric before calling service) - Missing is a Warning (Hint)
        var numericHts = hts.Replace(".", "");
        try
        {
            var recommendation = await _htsService.GetRecommendationAsync(numericHts);
            if (recommendation.Message == "No recommendation data")
            {
                result.Warnings.Add($"{label} '{hts}' not found in USITC database.");
                return false;
            }
            return true; // Found
        }
        catch (Exception ex)
        {
            result.Warnings.Add($"Unable to verify {label} '{hts}' existence: {ex.Message}");
            return false;
        }
    }
}
