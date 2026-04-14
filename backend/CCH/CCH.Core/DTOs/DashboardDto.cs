namespace CCH.Core.DTOs;

/// <summary>
/// Dashboard summary of part counts by status.
/// (繁體中文) 儀表板各狀態零件數量摘要。
/// </summary>
public class PartStatusSummaryDto
{
    public int S01 { get; set; }
    public int S02 { get; set; }
    public int S03 { get; set; }
    public int S04 { get; set; }
    public int S05 { get; set; }
}

/// <summary>
/// Pending review part item.
/// (繁體中文) 待審核零件項目。
/// </summary>
public class PendingReviewDto
{
    public string Customer { get; set; } = string.Empty;
    public string PartNo { get; set; } = string.Empty;
    public string PartDesc { get; set; } = string.Empty;
    public string HtsCode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string UpdatedBy { get; set; } = string.Empty;
    public DateTime UpdatedDate { get; set; }
    public string SlaStatus { get; set; } = "green"; // green, yellow, orange, red
}
