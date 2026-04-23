using CCH.Core.Constants;
using CCH.Core.Entities;
using CCH.Core.Entities.CSP;
using CCH.Core.Entities.ReSm;
using CCH.Core.Interfaces.Repositories;
using CCH.Services.Repositories.Data;

namespace CCH.Services.Repositories;

/// <summary>
/// Implementation of Common repository using SQL Database for Countries (ReSm),
/// Code Constants for Statuses, and Database for CCHSuppliers.
/// (繁體中文) 共用倉儲實作：國家資料使用 SQL 資料庫 (ReSm)，狀態使用程式碼常數，供應商資料使用資料庫 (CCHSuppliers)。
/// </summary>
public class CommonRepository : ICommonRepository
{
    private readonly ReSmDbContext _resmContext;
    private readonly CspDbContext _cspContext;

    /// <summary>
    /// Initializes a new instance of CommonRepository.
    /// (繁體中文) 初始化 CommonRepository 的新執行個體。
    /// </summary>
    public CommonRepository(ReSmDbContext resmContext, CspDbContext cspContext)
    {
        _resmContext = resmContext;
        _cspContext = cspContext;
    }

    /// <inheritdoc/>
    public IEnumerable<SmCustomer> GetCustomers()
    {
        // 1. Get all CustomerIDs from CchParts where Status is not null/whitespace.
        // (繁體中文) 從 CchParts 取得所有 Status 非 null/空白的 CustomerID。
        var usedCustomerIds = _cspContext.CchParts
            .Where(p => !string.IsNullOrWhiteSpace(p.Status))
            .Select(p => p.CustomerID)
            .Distinct()
            .ToList();

        // 2. Join with active SmCustomers from ReSm database.
        // (繁體中文) 與 ReSm 資料庫中 Active 的 SmCustomer 關聯。
        return _resmContext.SmCustomer
            .Where(c => c.Status == "Active" && usedCustomerIds.Contains(c.HQID))
            .ToList();
    }

    /// <inheritdoc/>
    public IEnumerable<CountryEntity> GetCountries() => 
        _resmContext.SmCountry.Where(x => x.Status == "Active").AsEnumerable().Select(MapToCountryDomain).ToList();

    /// <inheritdoc/>
    public IEnumerable<StatusEntity> GetStatuses() => PartStatusConstants.AllStatuses;

    /// <inheritdoc/>
    public IEnumerable<CchSuppliers> GetSuppliers(int? customerId = null) => 
        customerId == null 
            ? _cspContext.CchSuppliers.ToList() 
            : _cspContext.CchSuppliers.Where(s => s.CustomerID == customerId.Value).ToList();

    /// <inheritdoc/>
    public int CreateSupplier(CchSuppliers entity)
    {
        _cspContext.CchSuppliers.Add(entity);
        _cspContext.SaveChanges();
        return entity.ID;
    }

    /// <inheritdoc/>
    public string GetUserName(string userId)
    {
        if (string.IsNullOrEmpty(userId)) return string.Empty;

        // 1. Try SmUser by UserID (Internal)
        var smUser = _resmContext.SmUser.FirstOrDefault(u => u.UserID == userId);
        if (smUser != null) return smUser.FullName;

        // 2. Try SmCustomerContact by HQID (External)
        if (int.TryParse(userId, out int hqid))
        {
            var contact = _resmContext.SmCustomerContact.FirstOrDefault(c => c.Hqid == hqid);
            if (contact != null) return contact.FullName ?? "Unknown";
        }

        return userId; // Fallback to original value for legacy data (Admin, etc.)
    }

    /// <inheritdoc/>
    public Dictionary<string, string> GetUserNames(IEnumerable<string> userIds)
    {
        var distinctIds = userIds.Where(id => !string.IsNullOrEmpty(id)).Distinct().ToList();
        if (!distinctIds.Any()) return new Dictionary<string, string>();

        var result = new Dictionary<string, string>();

        // Fetch all matching internal users
        var smUsers = _resmContext.SmUser
            .Where(u => distinctIds.Contains(u.UserID))
            .ToDictionary(u => u.UserID, u => u.FullName);

        // Fetch all matching external customers (HQIDs)
        var hqids = distinctIds
            .Select(id => int.TryParse(id, out int hqid) ? (int?)hqid : null)
            .Where(h => h.HasValue)
            .Select(h => h!.Value)
            .ToList();

        var contacts = _resmContext.SmCustomerContact
            .Where(c => hqids.Contains(c.Hqid))
            .ToDictionary(c => c.Hqid.ToString(), c => c.FullName ?? "Unknown");

        foreach (var id in distinctIds)
        {
            if (smUsers.TryGetValue(id, out var name)) result[id] = name;
            else if (contacts.TryGetValue(id, out name)) result[id] = name;
            else result[id] = id; // Fallback
        }

        return result;
    }

    /// <summary>
    /// Maps an SmCountry to a CountryEntity domain model. (SSoT)
    /// (繁體中文) 將 SmCountry 映射至 CountryEntity 領域模型 (單一事實來源)。
    /// </summary>
    private CountryEntity MapToCountryDomain(SmCountry e) => new()
    {
        ID = e.HQID,
        Name = e.CountryName ?? "Unknown",
        Code = e.CountryCode
    };
}
