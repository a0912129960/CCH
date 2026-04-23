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
    public IEnumerable<CpProject> GetProjects()
    {
        // Fetch projects using Select projection to ensure stability even if some columns have unexpected NULLs.
        // (繁體中文) 使用 Select 投影取得專案，確保即使部分欄位有非預期的 NULL，系統仍能穩定運作。
        return _cspContext.CpProject
            .Where(p => p.Status == "Active")
            .Select(p => new CpProject
            {
                Id = p.Id,
                ProjectName = p.ProjectName ?? "Unknown",
                Status = p.Status
            })
            .ToList();
    }

    /// <inheritdoc/>
    public IEnumerable<CountryEntity> GetCountries() => 
        _resmContext.SmCountry.Where(x => x.Status == "Active").AsEnumerable().Select(MapToCountryDomain).ToList();

    /// <inheritdoc/>
    public IEnumerable<StatusEntity> GetStatuses() => PartStatusConstants.AllStatuses;

    /// <inheritdoc/>
    public IEnumerable<CchSuppliers> GetSuppliers(int? projectId = null) => 
        projectId == null 
            ? _cspContext.CchSuppliers.ToList() 
            : _cspContext.CchSuppliers.Where(s => s.ProjectID == projectId.Value).ToList();

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

        // 1. Try SmUser by UserID (Internal) - Use Select to avoid loading all columns (使用 Select 避免載入所有欄位)
        var smUserName = _resmContext.SmUser
            .Where(u => u.UserID == userId)
            .Select(u => u.FullName)
            .FirstOrDefault();
        
        if (smUserName != null) return smUserName;

        // 2. Try SmCustomerContact by HQID (External)
        if (int.TryParse(userId, out int hqid))
        {
            var contactName = _resmContext.SmCustomerContact
                .Where(c => c.Hqid == hqid)
                .Select(c => c.FullName)
                .FirstOrDefault();
            
            if (contactName != null) return contactName;
        }

        return userId; // Fallback
    }

    /// <inheritdoc/>
    public Dictionary<string, string> GetUserNames(IEnumerable<string> userIds)
    {
        var distinctIds = userIds.Where(id => !string.IsNullOrEmpty(id)).Distinct().ToList();
        if (!distinctIds.Any()) return new Dictionary<string, string>();

        var result = new Dictionary<string, string>();

        // Fetch only ID and FullName for matching internal users (僅抓取必要欄位)
        var smUsers = _resmContext.SmUser
            .Where(u => distinctIds.Contains(u.UserID))
            .Select(u => new { u.UserID, u.FullName })
            .ToList()
            .ToDictionary(u => u.UserID, u => u.FullName ?? "Unknown");

        // Fetch only HQID and FullName for matching external customers (僅抓取必要欄位)
        var hqids = distinctIds
            .Select(id => int.TryParse(id, out int hqid) ? (int?)hqid : null)
            .Where(h => h.HasValue)
            .Select(h => h!.Value)
            .ToList();

        var contacts = _resmContext.SmCustomerContact
            .Where(c => hqids.Contains(c.Hqid))
            .Select(c => new { c.Hqid, c.FullName })
            .ToList()
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
