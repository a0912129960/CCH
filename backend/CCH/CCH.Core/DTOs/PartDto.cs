using System.ComponentModel.DataAnnotations;

namespace CCH.Core.DTOs;

/// <summary>
/// Part list item.
/// (繁體中文) 零件清單項目。
/// </summary>
public class PartListItemDto
{
    public int Id { get; set; }
    public string Customer { get; set; } = string.Empty;
    public string PartNo { get; set; } = string.Empty;
    public string PartDesc { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string HtsCode { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public string Supplier { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string UpdatedBy { get; set; } = string.Empty;
    public DateTime UpdatedDate { get; set; }
    public string SlaStatus { get; set; } = "green";
    
    // Additional HTS/Rate 1-4
    public string? HtsCode1 { get; set; }
    public decimal? Rate1 { get; set; }
    public string? HtsCode2 { get; set; }
    public decimal? Rate2 { get; set; }
    public string? HtsCode3 { get; set; }
    public decimal? Rate3 { get; set; }
    public string? HtsCode4 { get; set; }
    public decimal? Rate4 { get; set; }
}

/// <summary>
/// Paginated list of parts.
/// (繁體中文) 分頁後的零件清單。
/// </summary>
public class PartListResponseDto
{
    public int Total { get; set; }
    public int Page { get; set; }
    public IEnumerable<PartListItemDto> Data { get; set; } = Enumerable.Empty<PartListItemDto>();
}

/// <summary>
/// Part details including current and modified states.
/// (繁體中文) 零件詳細資料（含變更前與變更後）。
/// </summary>
public class PartDetailResponseDto
{
    public PartDetailDto Before { get; set; } = new();
    public PartDetailDto Modified { get; set; } = new();
}

/// <summary>
/// Part detail fields.
/// (繁體中文) 零件詳細欄位。
/// </summary>
public class PartDetailDto
{
    public string PartNo { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Division { get; set; } = string.Empty;
    public string Supplier { get; set; } = string.Empty;
    public string PartDesc { get; set; } = string.Empty;
    public string HtsCode { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public string? HtsCode1 { get; set; }
    public decimal? Rate1 { get; set; }
    public string? HtsCode2 { get; set; }
    public decimal? Rate2 { get; set; }
    public string? HtsCode3 { get; set; }
    public decimal? Rate3 { get; set; }
    public string? HtsCode4 { get; set; }
    public decimal? Rate4 { get; set; }
    public string Remark { get; set; } = string.Empty;
    public string UpdatedBy { get; set; } = string.Empty;
    public DateTime UpdatedDate { get; set; }
}

/// <summary>
/// Request for creating or updating a part.
/// (繁體中文) 新增或修改零件的請求。
/// </summary>
// INTERNAL-AI-20260416: Added [Required] and HTS Code format validation per API spec.
// (INTERNAL-AI-20260416: 依 API 規格加入必填驗證與 HTS Code 格式驗證。)
public class PartSaveRequest
{
    public int? CustomerId { get; set; }

    // PartNo is required (零件編號為必填)
    [Required(ErrorMessage = "PartNo is required. / 零件編號為必填。")]
    public string PartNo { get; set; } = string.Empty;

    // Country of Origin — not editable via PUT (View Detail), kept nullable (原產地在 View Detail 不可編輯，保留 nullable)
    public int? CountryId { get; set; }

    // Division is required (部門為必填)
    [Required(ErrorMessage = "Division is required. / 部門為必填。")]
    public string Division { get; set; } = string.Empty;

    // Supplier is required (供應商為必填)
    [Required(ErrorMessage = "Supplier is required. / 供應商為必填。")]
    public string Supplier { get; set; } = string.Empty;

    // Part Description is required (零件描述為必填)
    [Required(ErrorMessage = "PartDesc is required. / 零件描述為必填。")]
    public string PartDesc { get; set; } = string.Empty;

    // HTS Code is required and must follow XXXX.XX.XXXX format (HTS Code 為必填且須符合格式)
    [Required(ErrorMessage = "HtsCode is required. / HTS Code 為必填。")]
    [RegularExpression(@"^\d{4}\.\d{2}\.\d{4}$", ErrorMessage = "HtsCode must be in XXXX.XX.XXXX format. / HTS Code 格式須為 XXXX.XX.XXXX。")]
    public string HtsCode { get; set; } = string.Empty;

    public decimal Rate { get; set; }

    // Optional additional HTS Codes 1-4 with format validation (選填的額外 HTS Code，若有填寫則驗證格式)
    [RegularExpression(@"^\d{4}\.\d{2}\.\d{4}$", ErrorMessage = "HtsCode1 must be in XXXX.XX.XXXX format. / HTS Code 1 格式須為 XXXX.XX.XXXX。")]
    public string? HtsCode1 { get; set; }
    public decimal? Rate1 { get; set; }

    [RegularExpression(@"^\d{4}\.\d{2}\.\d{4}$", ErrorMessage = "HtsCode2 must be in XXXX.XX.XXXX format.")]
    public string? HtsCode2 { get; set; }
    public decimal? Rate2 { get; set; }

    [RegularExpression(@"^\d{4}\.\d{2}\.\d{4}$", ErrorMessage = "HtsCode3 must be in XXXX.XX.XXXX format.")]
    public string? HtsCode3 { get; set; }
    public decimal? Rate3 { get; set; }

    [RegularExpression(@"^\d{4}\.\d{2}\.\d{4}$", ErrorMessage = "HtsCode4 must be in XXXX.XX.XXXX format.")]
    public string? HtsCode4 { get; set; }
    public decimal? Rate4 { get; set; }

    public string Remark { get; set; } = string.Empty;
}

/// <summary>
/// Response for bulk upload.
/// (繁體中文) 批次上傳回應。
/// </summary>
public class BulkUploadResponseDto
{
    public int PartId { get; set; }
    public string Error { get; set; } = string.Empty;
}

/// <summary>
/// Milestone (Action History) item.
/// (繁體中文) 里程碑（操作歷程）項目。
/// </summary>
public class MilestoneDto
{
    public string Action { get; set; } = string.Empty;
    public string UpdatedBy { get; set; } = string.Empty;
    public DateTime UpdatedDate { get; set; }
    public string Remark { get; set; } = string.Empty;
}
