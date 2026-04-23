using CCH.Core.Entities;
using CCH.Core.Entities.CSP;
using CCH.Core.Entities.ReSm;

namespace CCH.Core.Interfaces.Repositories;

/// <summary>
/// Repository interface for common metadata like countries, customers, and statuses.
/// (繁體中文) 國家、客戶與狀態等共用元資料的倉儲介面。
/// </summary>
public interface ICommonRepository
{
    IEnumerable<CpProject> GetProjects(string? userId = null, string? role = null);
    IEnumerable<CountryEntity> GetCountries();
    IEnumerable<StatusEntity> GetStatuses();
    IEnumerable<CchSuppliers> GetSuppliers(int? projectId = null);

    /// <summary>
    /// Creates a new supplier.
    /// (繁體中文) 建立新供應商。
    /// </summary>
    int CreateSupplier(CchSuppliers entity);

    /// <summary>
    /// Gets the full name of a user by ID (SmUser.UserID or SmCustomerContact.Hqid).
    /// (繁體中文) 根據 ID 取得使用者全名 (SmUser.UserID 或 SmCustomerContact.Hqid)。
    /// </summary>
    /// <param name="userId">The user ID (string or Hqid string). (使用者 ID)</param>
    /// <returns>The full name, or the original userId if not found. (全名，若未找到則回傳原始 ID)</returns>
    string GetUserName(string userId);

    /// <summary>
    /// Gets a dictionary of user IDs to full names for a batch of users.
    /// (繁體中文) 批次取得使用者 ID 與全名的對照字典。
    /// </summary>
    /// <param name="userIds">List of user IDs. (使用者 ID 清單)</param>
    /// <returns>A dictionary mapping ID to FullName. (ID 與全名的對照字典)</returns>
    Dictionary<string, string> GetUserNames(IEnumerable<string> userIds);
}
