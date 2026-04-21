using CCH.Core.Entities;

namespace CCH.Core.Interfaces.Repositories;

/// <summary>
/// Part repository interface.
/// (繁體中文) 零件倉儲介面。
/// </summary>
public interface IPartRepository
{
    /// <summary>
    /// Searches for part entities based on various criteria.
    /// (繁體中文) 根據多種條件搜尋零件實體。
    /// </summary>
    /// <param name="customerId">Customer ID. (客戶 ID)</param>
    /// <param name="status">Part status. (零件狀態)</param>
    /// <param name="partNo">Part number. (零件編號)</param>
    /// <param name="supplierId">Supplier ID. (供應商 ID)</param>
    /// <returns>A collection of part entities. (零件實體集合)</returns>
    IEnumerable<PartEntity> SearchParts(int? customerId, string? status, string? partNo, int? supplierId);

    /// <summary>
    /// Retrieves a specific part entity by ID.
    /// (繁體中文) 根據 ID 取得特定零件實體。
    /// </summary>
    /// <param name="partId">Part ID. (零件 ID)</param>
    /// <returns>The part entity if found; otherwise, null. (若找到則為零件實體；否則為 null)</returns>
    PartEntity? GetPartById(int partId);

    /// <summary>
    /// Creates a new part entity.
    /// (繁體中文) 建立新零件實體。
    /// </summary>
    /// <param name="entity">The part entity to create. (要建立的零件實體)</param>
    /// <returns>The ID of the newly created part. (新建立零件的 ID)</returns>
    int CreatePart(PartEntity entity);

    /// <summary>
    /// Updates an existing part entity.
    /// (繁體中文) 更新現有零件實體。
    /// </summary>
    /// <param name="entity">The part entity to update. (要更新的零件實體)</param>
    void UpdatePart(PartEntity entity);

    /// <summary>
    /// Updates the status of a specific part.
    /// (繁體中文) 更新特定零件的狀態。
    /// </summary>
    /// <param name="partId">Part ID. (零件 ID)</param>
    /// <param name="status">New status code. (新狀態代碼)</param>
    void UpdateStatus(int partId, string status);

    // INTERNAL-AI-20260420: Milestone history methods — action event log for the timeline.
    // (INTERNAL-AI-20260420: 里程碑歷程方法 — 動作事件日誌，用於時間軸顯示。)

    /// <summary>
    /// Appends a milestone event (status change) for a part.
    /// (繁體中文) 為零件新增一筆里程碑事件（狀態變更）。
    /// </summary>
    void AddHistory(PartHistoryEntity entity);

    /// <summary>
    /// Returns all milestone events for a part, stored in insertion order (caller sorts as needed).
    /// (繁體中文) 回傳零件的所有里程碑事件（依插入順序，由呼叫方決定排序）。
    /// </summary>
    IEnumerable<PartHistoryEntity> GetHistoryByPartId(int partId);

    // INTERNAL-AI-20260420: Snapshot methods — full field-level data snapshots for the history API.
    // (INTERNAL-AI-20260420: 快照方法 — 完整欄位資料快照，用於歷程 API。)

    /// <summary>
    /// Appends a full-data snapshot of a part at a given point in time.
    /// (繁體中文) 為零件在特定時間點附加一筆完整資料快照。
    /// </summary>
    void AddSnapshot(PartSnapshotEntity entity);

    /// <summary>
    /// Returns all snapshots for a part, stored in insertion order (caller sorts as needed).
    /// (繁體中文) 回傳零件的所有快照（依插入順序，由呼叫方決定排序）。
    /// </summary>
    IEnumerable<PartSnapshotEntity> GetSnapshotsByPartId(int partId);
}
