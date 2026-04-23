using CCH.Core.Features.Common.DTOs;

namespace CCH.Core.Features.Common.Interfaces;

/// <summary>
/// Common data service interface.
/// (繁體中文) 共用資料服務介面。
/// </summary>
public interface ICommonService
{
    IEnumerable<KeyValuePairDto> GetCustomers();
    IEnumerable<KeyValuePairDto> GetCountries();
    IEnumerable<KeyValuePairDto> GetStatus();
    IEnumerable<KeyValuePairDto> GetSuppliers(string? customerId);

    /// <summary>
    /// Gets the full name of a user by ID.
    /// (繁體中文) 根據 ID 取得使用者全名。
    /// </summary>
    string GetUserName(string userId);

    /// <summary>
    /// Gets a dictionary of user IDs to full names for a batch.
    /// (繁體中文) 批次取得使用者 ID 與全名的對照字典。
    /// </summary>
    Dictionary<string, string> GetUserNames(IEnumerable<string> userIds);
}
