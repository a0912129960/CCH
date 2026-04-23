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
    IEnumerable<SmCustomer> GetCustomers();
    IEnumerable<CountryEntity> GetCountries();
    IEnumerable<StatusEntity> GetStatuses();
    IEnumerable<CchSuppliers> GetSuppliers(int? customerId = null);

    /// <summary>
    /// Creates a new supplier.
    /// (繁體中文) 建立新供應商。
    /// </summary>
    int CreateSupplier(CchSuppliers entity);
}
