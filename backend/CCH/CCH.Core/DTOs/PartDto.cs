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
public class PartSaveRequest
{
    public int? CustomerId { get; set; }
    public string PartNo { get; set; } = string.Empty;
    public int? CountryId { get; set; }
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
