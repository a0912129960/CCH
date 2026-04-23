using CCH.Core.Constants;
using CCH.Core.Entities;
using CCH.Core.Entities.CSP;
using CCH.Core.Entities.ReSm;
using CCH.Core.Interfaces.Repositories;
using CCH.Services.Repositories.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

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
    public IEnumerable<CpProject> GetProjects(string? userId = null, string? role = null)
    {
        // Update by AI (2026-04-23): Re-implemented using Raw SQL commands as requested for better consistency with MyDimerco logic
        // (繁體中文) 由 AI 更新 (2026-04-23)：依要求改用 Raw SQL 指令重新實作，以確保與 MyDimerco 邏輯的一致性

        var parameters = new List<SqlParameter>();
        // Fix: Use SELECT * because EF FromSqlRaw requires all columns defined in the entity to be present in the result set
        // (繁體中文) 修正：使用 SELECT *，因為 EF FromSqlRaw 要求結果集必須包含實體中定義的所有欄位
        var sql = "SELECT * FROM CPProject WHERE Status = 'Active'";

        if (!string.IsNullOrEmpty(role))
        {
            if (role == "customer" && int.TryParse(userId, out int hqid))
            {
                // External User SQL filtering (外部使用者 SQL 過濾)
                sql += " AND ID IN (SELECT ProjectID FROM CPProjectContactor WHERE ContactorHQID = @hqid)";
                parameters.Add(new SqlParameter("@hqid", hqid));
            }
            else if (role == "dimerco" || role == "dcb")
            {
                // Internal User: Fetch SmUser to get HQID and Admin status using SQL
                // (內部使用者：使用 SQL 取得 SmUser 以獲取 HQID 與管理者狀態)
                var user = _resmContext.SmUser
                    .FromSqlRaw("SELECT * FROM SMUser WHERE UserID = @uId", new SqlParameter("@uId", userId ?? ""))
                    .AsNoTracking()
                    .FirstOrDefault();

                if (user == null) return Enumerable.Empty<CpProject>();

                if (user.Admin != "Y")
                {
                    // Granular filtering SQL using numeric HQID (使用數值 HQID 進行細粒度過濾 SQL)
                    sql += @" AND ID IN (
                                SELECT ProjectID 
                                FROM CPProjectUser 
                                WHERE OwnerType='Group' 
                                  AND OwnerID IN (SELECT GroupID FROM rcgroupmemberv2 WHERE UserID = @hqid))";
                    parameters.Add(new SqlParameter("@hqid", user.HQID));
                }
            }
        }

        sql += " ORDER BY ProjectName ASC";

        // Execute raw SQL on CspDbContext
        return _cspContext.CpProject
            .FromSqlRaw(sql, parameters.ToArray())
            .AsNoTracking()
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
