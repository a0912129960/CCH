namespace CCH.Core.Entities;

/// <summary>
/// Records a single status-change event for a part.
/// (繁體中文) 記錄零件的單一狀態變更事件。
/// </summary>
// INTERNAL-AI-20260420: Created to support the real timeline in the Detail view.
// (INTERNAL-AI-20260420: 建立以支援詳細頁面的真實時間軸。)
public class PartHistoryEntity
{
    public int ID { get; set; }
    public int PartID { get; set; }

    /// <summary>
    /// Human-readable action label shown in the timeline.
    /// (繁體中文) 顯示在時間軸上的可讀動作標籤。
    /// </summary>
    public string Action { get; set; } = string.Empty;

    public string UpdatedBy { get; set; } = string.Empty;
    public DateTime UpdatedDate { get; set; }

    /// <summary>
    /// Optional remark (e.g. return reason from DCB).
    /// (繁體中文) 選填備註（例如 DCB 的退回原因）。
    /// </summary>
    public string Remark { get; set; } = string.Empty;
}
