using CCH.Core.Features.Parts.DTOs;

namespace CCH.Core.Features.Parts.Interfaces;

/// <summary>
/// Result of the part validation process.
/// (繁體中文) 零件驗證流程的結果。
/// </summary>
public class ValidationResult
{
    public List<(string Field, string Message)> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public bool? IsHTSExists { get; set; }
}

/// <summary>
/// Service for validating part data against business rules.
/// (繁體中文) 根據業務規則驗證零件資料的服務。
/// </summary>
public interface IPartValidationService
{
    /// <summary>
    /// Validates an Excel row (PartDto) based on the specified action, current state, and user role.
    /// (繁體中文) 根據指定動作、目前狀態與使用者角色驗證 Excel 資料列 (PartDto)。
    /// </summary>
    /// <param name="newData">The incoming data from Excel. (來自 Excel 的新資料)</param>
    /// <param name="originalData">Existing data from DB if available. (來自 DB 的現有資料，若存在)</param>
    /// <param name="role">Current user role. (目前使用者角色)</param>
    /// <param name="isNew">Whether this is a new record or an update. (是否為新記錄或更新)</param>
    /// <returns>A validation result containing errors, warnings, and metadata. (包含錯誤、警告與中繼資料的驗證結果)</returns>
    Task<ValidationResult> ValidateExcelRowAsync(PartDto newData, PartDto? originalData, string role, bool isNew);
}
