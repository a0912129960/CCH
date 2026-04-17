using CCH.Core.Entities;

namespace CCH.Core.Interfaces.Repositories;

/// <summary>
/// Repository interface for common metadata like countries, customers, and statuses.
/// (繁體中文) 國家、客戶與狀態等共用元資料的倉儲介面。
/// </summary>
public interface ICommonRepository
{
    IEnumerable<CustomerEntity> GetCustomers();
    IEnumerable<CountryEntity> GetCountries();
    IEnumerable<StatusEntity> GetStatuses();
    IEnumerable<SupplierEntity> GetSuppliers(int? customerId = null);
}
