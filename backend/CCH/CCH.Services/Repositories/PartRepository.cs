using CCH.Core.Entities.CSP;
using CCH.Core.Interfaces.Repositories;
using CCH.Services.Repositories.Data;

namespace CCH.Services.Repositories;

/// <summary>
/// Implementation of Part repository using SQL Database for all entities.
/// (繁體中文) 零件倉儲實作：所有實體皆使用 SQL 資料庫。
/// </summary>
public class PartRepository : IPartRepository
{
    private readonly CspDbContext _context;
    private readonly ReSmDbContext _resmContext;

    public PartRepository(CspDbContext context, ReSmDbContext resmContext)
    {
        _context = context;
        _resmContext = resmContext;
    }

    /// <inheritdoc/>
    public IEnumerable<CchParts> SearchParts(int? customerId, string? status, string? partNo, int? supplierId)
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
        return query.ToList();
    }

    /// <inheritdoc/>
    public CchParts? GetPartByNo(int customerId, string partNo)
    {
        return _context.CchParts.FirstOrDefault(p => p.CustomerID == customerId && p.PartNo == partNo);
    }

    /// <inheritdoc/>
    public CchParts? GetPartById(int partId)
    {
        return _context.CchParts.FirstOrDefault(p => p.ID == partId);
    }

    /// <inheritdoc/>
    public int CreatePart(CchParts entity)
    {
        entity.CreatedDate = DateTime.Now;
        entity.UpdatedDate = DateTime.Now;
        _context.CchParts.Add(entity);
        _context.SaveChanges();
        return entity.ID;
    }

    /// <inheritdoc/>
    public void UpdatePart(CchParts entity)
    {
        var existing = _context.CchParts.FirstOrDefault(p => p.ID == entity.ID);
        if (existing != null)
        {
            // Sync fields (EF tracking) (同步欄位，利用 EF 追蹤)
            _context.Entry(existing).CurrentValues.SetValues(entity);
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

    /// <inheritdoc/>
    public void AddHistory(CchPartMilestones entity)
    {
        _context.CchPartMilestones.Add(entity);
        _context.SaveChanges();
    }

    /// <inheritdoc/>
    public void AddHistoryBatch(IEnumerable<CchPartMilestones> entities)
    {
        _context.CchPartMilestones.AddRange(entities);
        _context.SaveChanges();
    }

    /// <inheritdoc/>
    public IEnumerable<CchPartMilestones> GetHistoryByPartId(int partId) =>
        _context.CchPartMilestones.Where(h => h.PartID == partId).OrderBy(h => h.CreatedDate).ToList();

    /// <inheritdoc/>
    public void AddSnapshot(CchPartHistories entity)
    {
        _context.CchPartHistories.Add(entity);
        _context.SaveChanges();
    }

    /// <inheritdoc/>
    public IEnumerable<CchPartHistories> GetSnapshotsByPartId(int partId) =>
        _context.CchPartHistories.Where(s => s.PartID == partId).ToList();
}
