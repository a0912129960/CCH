using CCH.Core.DTOs;

namespace CCH.Core.Interfaces;

/// <summary>
/// Common data service interface.
/// (繁體中文) 共用資料服務介面。
/// </summary>
public interface ICommonService
{
    IEnumerable<KeyValuePairDto> GetCustomers();
    IEnumerable<KeyValuePairDto> GetCountries();
    IEnumerable<KeyValuePairDto> GetSuppliers(string? customerId);
    IEnumerable<KeyValuePairDto> GetStatus();
}
