using CCH.Core.Entities;
using CCH.Core.Interfaces.Repositories;
using System.Text.Json;

namespace CCH.Services.Repositories;

/// <summary>
/// Implementation of Part repository using JSON file persistence.
/// (繁體中文) 使用 JSON 檔案持久化的零件倉儲實作。
/// </summary>
public class PartRepository : IPartRepository
{
    private readonly string _partsPath;
    private List<PartEntity> _parts = new();
    private static readonly object _fileLock = new();

    /// <summary>
    /// Initializes a new instance of PartRepository.
    /// (繁體中文) 初始化 PartRepository 的新執行個體。
    /// </summary>
    /// <param name="overridePath">Optional path to override default storage. (選用的覆寫儲存路徑)</param>
    public PartRepository(string? overridePath = null)
    {
        if (!string.IsNullOrEmpty(overridePath))
        {
            _partsPath = overridePath;
        }
        else
        {
            // Default path discovery: Data/parts.json relative to project root
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var projectRootDir = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", ".."));
            var dataDir = Path.Combine(projectRootDir, "Data");
            _partsPath = Path.Combine(dataDir, "parts.json");
        }

        // Ensure data exists before loading (確保載入前資料已存在)
        DataSeeder.EnsureInitialized(_partsPath);
        LoadData();
    }

    private void LoadData()
    {
        lock (_fileLock)
        {
            try
            {
                if (File.Exists(_partsPath))
                {
                    var json = File.ReadAllText(_partsPath);
                    _parts = JsonSerializer.Deserialize<List<PartEntity>>(json) ?? new();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading parts data: {ex.Message}");
                _parts = new();
            }
        }
    }

    private void SaveData()
    {
        lock (_fileLock)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                File.WriteAllText(_partsPath, JsonSerializer.Serialize(_parts, options));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving parts data: {ex.Message}");
            }
        }
    }

    /// <inheritdoc/>
    public IEnumerable<PartEntity> SearchParts(int? customerId, string? status, string? partNo, int? supplierId)
    {
        var query = _parts.AsQueryable();

        if (customerId > 0)
            query = query.Where(p => p.CustomerID == customerId);

        if (!string.IsNullOrEmpty(status))
            query = query.Where(p => p.Status.Equals(status, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrEmpty(partNo))
        {
            var normalizedSearch = partNo.Replace(".", "");
            query = query.Where(p => p.PartNo.Contains(partNo, StringComparison.OrdinalIgnoreCase) || 
                                     p.HTSCode.Replace(".", "").Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase));
        }

        if (supplierId > 0)
            query = query.Where(p => p.SupplierID == supplierId);

        return query.ToList();
    }

    /// <inheritdoc/>
    public PartEntity? GetPartById(int partId) => _parts.FirstOrDefault(p => p.ID == partId);

    /// <inheritdoc/>
    public int CreatePart(PartEntity entity)
    {
        lock (_fileLock)
        {
            entity.ID = _parts.Any() ? _parts.Max(p => p.ID) + 1 : 1;
            entity.CreatedDate = DateTime.Now;
            entity.UpdatedDate = DateTime.Now;
            _parts.Add(entity);
            SaveData();
            return entity.ID;
        }
    }

    /// <inheritdoc/>
    public void UpdatePart(PartEntity entity)
    {
        lock (_fileLock)
        {
            var existing = _parts.FirstOrDefault(p => p.ID == entity.ID);
            if (existing != null)
            {
                // Update basic properties (更新基本屬性)
                existing.PartNo = entity.PartNo;
                existing.PartDescription = entity.PartDescription;
                existing.Division = entity.Division;
                existing.SupplierID = entity.SupplierID;
                existing.HTSCode = entity.HTSCode;
                existing.DutyRate = entity.DutyRate;
                existing.AddHTSCode1 = entity.AddHTSCode1;
                existing.AddDutyRate1 = entity.AddDutyRate1;
                existing.AddHTSCode2 = entity.AddHTSCode2;
                existing.AddDutyRate2 = entity.AddDutyRate2;
                existing.AddHTSCode3 = entity.AddHTSCode3;
                existing.AddDutyRate3 = entity.AddDutyRate3;
                existing.AddHTSCode4 = entity.AddHTSCode4;
                existing.AddDutyRate4 = entity.AddDutyRate4;
                existing.Remark = entity.Remark;
                existing.UpdatedBy = entity.UpdatedBy;
                existing.UpdatedDate = DateTime.Now;
                
                SaveData();
            }
        }
    }

    /// <inheritdoc/>
    public void UpdateStatus(int partId, string status)
    {
        lock (_fileLock)
        {
            var existing = _parts.FirstOrDefault(p => p.ID == partId);
            if (existing != null)
            {
                existing.Status = status;
                existing.UpdatedDate = DateTime.Now;
                SaveData();
            }
        }
    }
}
