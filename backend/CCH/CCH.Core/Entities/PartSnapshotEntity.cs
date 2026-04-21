namespace CCH.Core.Entities;

/// <summary>
/// Full data snapshot of a part taken each time its fields are saved.
/// Backs the GET /api/parts/{partId}/history endpoint.
/// (繁體中文) 每次儲存零件欄位時所拍攝的完整資料快照，支援歷程 API。
/// </summary>
// INTERNAL-AI-20260420: Created to support real field-change history per API spec.
// (INTERNAL-AI-20260420: 依 API 規格建立以支援真實欄位變更歷程。)
public class PartSnapshotEntity
{
    public int ID { get; set; }
    public int PartID { get; set; }

    // Resolved display values (resolved names, not IDs) (已解析的顯示名稱，非 ID)
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
