using System.Text.Json;
using CCH.Core.Entities;
using CCH.Core.Entities.CSP;
using CCH.Core.Entities.ReSm;
using CCH.Core.Interfaces.Repositories;
using CCH.Services.Repositories.Data;

namespace CCH.Services.Repositories;

/// <summary>
/// Implementation of Part repository using SQL Database (CCHParts) for Parts 
/// and JSON files for History/Snapshots.
/// (繁體中文) 零件倉儲實作：零件使用 SQL 資料庫 (CCHParts)，歷程與快照維持使用 JSON 檔案。
/// </summary>
public class PartRepository : IPartRepository
{
    private readonly CspDbContext _context;
    private readonly string _historyPath;
    private readonly string _snapshotPath;
    private List<PartHistoryEntity> _history = new();
    private List<PartSnapshotEntity> _snapshots = new();
    private static readonly object _fileLock = new();

    public PartRepository(CspDbContext context)
    {
        _context = context;
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var projectRootDir = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", ".."));
        var dataDir = Path.Combine(projectRootDir, "Data");
        _historyPath = Path.Combine(dataDir, "part_history.json");
        _snapshotPath = Path.Combine(dataDir, "part_snapshots.json");
        LoadJsonData();
    }

    private void LoadJsonData()
    {
        lock (_fileLock)
        {
            if (File.Exists(_historyPath))
                _history = JsonSerializer.Deserialize<List<PartHistoryEntity>>(File.ReadAllText(_historyPath)) ?? new();
            if (File.Exists(_snapshotPath))
                _snapshots = JsonSerializer.Deserialize<List<PartSnapshotEntity>>(File.ReadAllText(_snapshotPath)) ?? new();
        }
    }

    /// <inheritdoc/>
    public IEnumerable<PartEntity> SearchParts(int? customerId, string? status, string? partNo, int? supplierId)
    {
        var query = _context.CchParts.AsQueryable();
        if (customerId > 0) query = query.Where(p => p.CustomerID == customerId);
        if (!string.IsNullOrEmpty(status)) query = query.Where(p => p.Status == status);
        if (!string.IsNullOrEmpty(partNo))
        {
            var norm = partNo.Replace(".", "");
            query = query.Where(p => p.PartNo!.Contains(partNo) || p.HTSCode!.Replace(".", "").Contains(norm));
        }
        if (supplierId > 0) query = query.Where(p => p.SupplierID == supplierId);
        return query.AsEnumerable().Select(MapToDomain).ToList();
    }

    /// <inheritdoc/>
    public PartEntity? GetPartByNo(int customerId, string partNo)
    {
        var entity = _context.CchParts.FirstOrDefault(p => p.CustomerID == customerId && p.PartNo == partNo);
        return entity == null ? null : MapToDomain(entity);
    }

    /// <inheritdoc/>
    public PartEntity? GetPartById(int partId)
    {
        var entity = _context.CchParts.FirstOrDefault(p => p.ID == partId);
        return entity == null ? null : MapToDomain(entity);
    }

    /// <inheritdoc/>
    public int CreatePart(PartEntity domain)
    {
        var dbEntity = new CchParts();
        UpdateDbEntityFromDomain(domain, dbEntity);
        dbEntity.CreatedDate = DateTime.Now;
        dbEntity.UpdatedDate = DateTime.Now;
        _context.CchParts.Add(dbEntity);
        _context.SaveChanges();
        return dbEntity.ID;
    }

    /// <inheritdoc/>
    public void UpdatePart(PartEntity domain)
    {
        var existing = _context.CchParts.FirstOrDefault(p => p.ID == domain.ID);
        if (existing != null)
        {
            UpdateDbEntityFromDomain(domain, existing);
            existing.UpdatedDate = DateTime.Now;
            _context.SaveChanges();
        }
    }

    /// <inheritdoc/>
    public void UpdateStatus(int partId, string status)
    {
        var existing = _context.CchParts.FirstOrDefault(p => p.ID == partId);
        if (existing != null)
        {
            existing.Status = status;
            existing.UpdatedDate = DateTime.Now;
            _context.SaveChanges();
        }
    }

    /// <inheritdoc/>
    public void BatchUpdateStatus(IEnumerable<int> partIds, string status, string updatedBy)
    {
        var entities = _context.CchParts.Where(p => partIds.Contains(p.ID)).ToList();
        var now = DateTime.Now;
        foreach (var entity in entities)
        {
            entity.Status = status;
            entity.UpdatedBy = updatedBy;
            entity.UpdatedDate = now;
        }
        _context.SaveChanges();
    }

    private PartEntity MapToDomain(CchParts e) => new()
    {
        ID = e.ID, CustomerID = e.CustomerID ?? 0, PartNo = e.PartNo ?? "",
        CountryID = e.CountryID ?? 0, PartDescription = e.PartDescription ?? "",
        Division = e.Division ?? "", SupplierID = e.SupplierID ?? 0,
        HTSCode = e.HTSCode ?? "", DutyRate = e.DutyRate ?? 0,
        AddHTSCode1 = e.AddHTSCode1, AddDutyRate1 = e.AddDutyRate1,
        AddHTSCode2 = e.AddHTSCode2, AddDutyRate2 = e.AddDutyRate2,
        AddHTSCode3 = e.AddHTSCode3, AddDutyRate3 = e.AddDutyRate3,
        AddHTSCode4 = e.AddHTSCode4, AddDutyRate4 = e.AddDutyRate4,
        Remark = e.Remark ?? "", Status = e.Status ?? "",
        CreatedBy = e.CreatedBy ?? "", UpdatedBy = e.UpdatedBy ?? "",
        CreatedDate = e.CreatedDate ?? DateTime.MinValue,
        UpdatedDate = e.UpdatedDate ?? DateTime.MinValue
    };

    private void UpdateDbEntityFromDomain(PartEntity d, CchParts e)
    {
        e.CustomerID = d.CustomerID; e.PartNo = d.PartNo; e.CountryID = d.CountryID;
        e.PartDescription = d.PartDescription; e.Division = d.Division;
        e.SupplierID = d.SupplierID; e.HTSCode = d.HTSCode; e.DutyRate = d.DutyRate;
        e.AddHTSCode1 = d.AddHTSCode1; e.AddDutyRate1 = d.AddDutyRate1;
        e.AddHTSCode2 = d.AddHTSCode2; e.AddDutyRate2 = d.AddDutyRate2;
        e.AddHTSCode3 = d.AddHTSCode3; e.AddDutyRate3 = d.AddDutyRate3;
        e.AddHTSCode4 = d.AddHTSCode4; e.AddDutyRate4 = d.AddDutyRate4;
        e.Remark = d.Remark; e.Status = d.Status; e.UpdatedBy = d.UpdatedBy;
        e.CreatedBy = string.IsNullOrEmpty(e.CreatedBy) ? d.CreatedBy : e.CreatedBy;
    }

    /// <inheritdoc/>
    public void AddHistory(PartHistoryEntity entity)
    {
        lock (_fileLock)
        {
            entity.ID = _history.Any() ? _history.Max(h => h.ID) + 1 : 1;
            _history.Add(entity);
            File.WriteAllText(_historyPath, JsonSerializer.Serialize(_history, new JsonSerializerOptions { WriteIndented = true }));
        }
    }

    /// <inheritdoc/>
    public void AddHistoryBatch(IEnumerable<PartHistoryEntity> entities)
    {
        lock (_fileLock)
        {
            var nextId = _history.Any() ? _history.Max(h => h.ID) + 1 : 1;
            foreach (var entity in entities)
            {
                entity.ID = nextId++;
                _history.Add(entity);
            }
            File.WriteAllText(_historyPath, JsonSerializer.Serialize(_history, new JsonSerializerOptions { WriteIndented = true }));
        }
    }

    /// <inheritdoc/>
    public IEnumerable<PartHistoryEntity> GetHistoryByPartId(int partId) =>
        _history.Where(h => h.PartID == partId).OrderBy(h => h.UpdatedDate).ToList();

    /// <inheritdoc/>
    public void AddSnapshot(PartSnapshotEntity entity)
    {
        lock (_fileLock)
        {
            entity.ID = _snapshots.Any() ? _snapshots.Max(s => s.ID) + 1 : 1;
            _snapshots.Add(entity);
            File.WriteAllText(_snapshotPath, JsonSerializer.Serialize(_snapshots, new JsonSerializerOptions { WriteIndented = true }));
        }
    }

    /// <inheritdoc/>
    public IEnumerable<PartSnapshotEntity> GetSnapshotsByPartId(int partId) =>
        _snapshots.Where(s => s.PartID == partId).OrderBy(s => s.UpdatedDate).ToList();
}
