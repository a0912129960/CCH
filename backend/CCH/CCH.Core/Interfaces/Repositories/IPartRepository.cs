using CCH.Core.Entities;
using CCH.Core.Entities.CSP;
using CCH.Core.Entities.ReSm;

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
    /// <param name="projectId">Customer ID. (客戶 ID)</param>
    /// <param name="status">Part status. (零件狀態)</param>
    /// <param name="partNo">Part number. (零件編號)</param>
    /// <param name="supplierId">Supplier ID. (供應商 ID)</param>
    /// <returns>A collection of part entities. (零件實體集合)</returns>
    IEnumerable<CchParts> SearchParts(int? projectId, string? status, string? partNo, int? supplierId);

    /// <summary>
    /// Retrieves a specific part entity by Customer ID and Part No.
    /// (繁體中文) 根據客戶 ID 與零件編號取得特定零件實體。
    /// </summary>
    CchParts? GetPartByNo(int projectId, string partNo);

    /// <summary>
    /// Retrieves a specific part entity by ID.
    /// (繁體中文) 根據 ID 取得特定零件實體。
    /// </summary>
    /// <param name="partId">Part ID. (零件 ID)</param>
    /// <returns>The part entity if found; otherwise, null. (若找到則為零件實體；否則為 null)</returns>
    CchParts? GetPartById(int partId);

    /// <summary>
    /// Creates a new part entity.
    /// (繁體中文) 建立新零件實體。
    /// </summary>
    /// <param name="entity">The part entity to create. (要建立的零件實體)</param>
    /// <returns>The ID of the newly created part. (新建立零件的 ID)</returns>
    int CreatePart(CchParts entity);

    /// <summary>
    /// Updates an existing part entity.
    /// (繁體中文) 更新現有零件實體。
    /// </summary>
    /// <param name="entity">The part entity to update. (要更新的零件實體)</param>
    void UpdatePart(CchParts entity);

    /// <summary>
    /// Updates the status of a specific part.
    /// (繁體中文) 更新特定零件的狀態。
    /// </summary>
    /// <param name="partId">Part ID. (零件 ID)</param>
    /// <param name="status">New status code. (新狀態代碼)</param>
    /// <param name="updatedBy">The user ID who performed the update. (執行更新的使用者 ID)</param>
    void UpdateStatus(int partId, string status, string updatedBy);

    /// <summary>
    /// Updates status for multiple parts in a single operation.
    /// (繁體中文) 在單次操作中更新多個零件的狀態。
    /// </summary>
    void BatchUpdateStatus(IEnumerable<int> partIds, string status, string updatedBy);

    // INTERNAL-AI-20260420: Milestone history methods — action event log for the timeline.
    // (INTERNAL-AI-20260420: 里程碑歷程方法 — 動作事件日誌，用於時間軸顯示。)

    /// <summary>
    /// Appends a milestone event (status change) for a part.
    /// (繁體中文) 為零件新增一筆里程碑事件（狀態變更）。
    /// </summary>
    void AddHistory(Entities.CSP.CchPartMilestones entity);

    /// <summary>
    /// Adds multiple history entries in a single operation.
    /// (繁體中文) 在單次操作中新增多筆歷程記錄。
    /// </summary>
    void AddHistoryBatch(IEnumerable<Entities.CSP.CchPartMilestones> entities);

    /// <summary>
    /// Returns all milestone events for a part, stored in insertion order (caller sorts as needed).
    /// (繁體中文) 回傳零件的所有里程碑事件（依插入順序，由呼叫方決定排序）。
    /// </summary>
    IEnumerable<Entities.CSP.CchPartMilestones> GetHistoryByPartId(int partId);

    // INTERNAL-AI-20260420: Snapshot methods — full field-level data snapshots for the history API.
    // (INTERNAL-AI-20260420: 快照方法 — 完整欄位資料快照，用於歷程 API。)

    /// <summary>
    /// Appends a full-data snapshot of a part at a given point in time.
    /// (繁體中文) 為零件在特定時間點附加一筆完整資料快照。
    /// </summary>
    void AddSnapshot(CchPartHistories entity);

    /// <summary>
    /// Returns all snapshots for a part, stored in insertion order (caller sorts as needed).
    /// (繁體中文) 回傳零件的所有快照（依插入順序，由呼叫方決定排序）。
    /// </summary>
    IEnumerable<CchPartHistories> GetSnapshotsByPartId(int partId);
}
