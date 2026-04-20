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
    // INTERNAL-AI-20260420: Added history file path for real timeline storage.
    // (INTERNAL-AI-20260420: 新增歷程檔案路徑以支援真實時間軸儲存。)
    private readonly string _historyPath;
    private List<PartEntity> _parts = new();
    private List<PartHistoryEntity> _history = new();
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
            _historyPath = overridePath.Replace("parts.json", "part_history.json");
        }
        else
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var projectRootDir = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", ".."));
            var dataDir = Path.Combine(projectRootDir, "Data");
            _partsPath = Path.Combine(dataDir, "parts.json");
            _historyPath = Path.Combine(dataDir, "part_history.json");
        }

        // Initialize only what this repository needs (僅初始化此倉儲需要的資料)
        DataSeeder.SeedParts(_partsPath);
        DataSeeder.SeedPartHistory(_historyPath);
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
                // INTERNAL-AI-20260420: Load history alongside parts. (一併載入歷程資料。)
                if (File.Exists(_historyPath))
                {
                    var hJson = File.ReadAllText(_historyPath);
                    _history = JsonSerializer.Deserialize<List<PartHistoryEntity>>(hJson) ?? new();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading parts data: {ex.Message}");
                _parts = new();
                _history = new();
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

    // INTERNAL-AI-20260420: Persist history to its own JSON file. (歷程資料持久化至獨立 JSON 檔案。)
    private void SaveHistory()
    {
        lock (_fileLock)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                File.WriteAllText(_historyPath, JsonSerializer.Serialize(_history, options));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving history data: {ex.Message}");
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

    // INTERNAL-AI-20260420: History methods for real timeline support.
    // (INTERNAL-AI-20260420: 支援真實時間軸的歷程方法。)

    /// <inheritdoc/>
    public void AddHistory(PartHistoryEntity entity)
    {
        lock (_fileLock)
        {
            entity.ID = _history.Any() ? _history.Max(h => h.ID) + 1 : 1;
            _history.Add(entity);
            SaveHistory();
        }
    }

    /// <inheritdoc/>
    public IEnumerable<PartHistoryEntity> GetHistoryByPartId(int partId) =>
        _history.Where(h => h.PartID == partId).OrderBy(h => h.UpdatedDate).ToList();
}
