using CCH.Core.Constants;
using CCH.Core.Entities;
using CCH.Core.Interfaces.Repositories;
using CCH.Services.Repositories.Data;
using System.Text.Json;

namespace CCH.Services.Repositories;

/// <summary>
/// Implementation of Common repository using SQL Database for Countries (ReSm),
/// Code Constants for Statuses, and JSON files for other common entities.
/// (繁體中文) 共用倉儲實作：國家資料使用 SQL 資料庫 (ReSm)，狀態使用程式碼常數，其餘實體維持使用 JSON 檔案。
/// </summary>
public class CommonRepository : ICommonRepository
{
    private readonly ReSmDbContext _resmContext;
    private readonly string _customersPath;
    private readonly string _suppliersPath;

    private List<CustomerEntity> _customers = new();
    private List<SupplierEntity> _suppliers = new();

    private static readonly object _fileLock = new();

    /// <summary>
    /// Initializes a new instance of CommonRepository.
    /// (繁體中文) 初始化 CommonRepository 的新執行個體。
    /// </summary>
    public CommonRepository(ReSmDbContext resmContext)
    {
        _resmContext = resmContext;
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var projectRootDir = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", ".."));
        var dataDir = Path.Combine(projectRootDir, "Data");

        _customersPath = Path.Combine(dataDir, "customers.json");
        _suppliersPath = Path.Combine(dataDir, "suppliers.json");

        // Seed remaining JSON data (國家與狀態已移除)
        DataSeeder.SeedCustomers(_customersPath);
        DataSeeder.SeedSuppliers(_suppliersPath);

        LoadJsonData();
    }

    private void LoadJsonData()
    {
        lock (_fileLock)
        {
            try
            {
                if (File.Exists(_customersPath))
                    _customers = JsonSerializer.Deserialize<List<CustomerEntity>>(File.ReadAllText(_customersPath)) ?? new();
                if (File.Exists(_suppliersPath))
                    _suppliers = JsonSerializer.Deserialize<List<SupplierEntity>>(File.ReadAllText(_suppliersPath)) ?? new();
            }
            catch (Exception ex) { Console.WriteLine($"Error loading common JSON data: {ex.Message}"); }
        }
    }

    /// <inheritdoc/>
    public IEnumerable<CustomerEntity> GetCustomers() => _customers;

    /// <inheritdoc/>
    public IEnumerable<CountryEntity> GetCountries() => 
        _resmContext.SmCountry.Where(x => x.Status == "Active").AsEnumerable().Select(MapToCountryDomain).ToList();

    /// <inheritdoc/>
    public IEnumerable<StatusEntity> GetStatuses() => PartStatusConstants.AllStatuses;

    /// <inheritdoc/>
    public IEnumerable<SupplierEntity> GetSuppliers(int? customerId = null) => 
        customerId == null ? _suppliers : _suppliers.Where(s => s.CustomerID == customerId.Value);

    /// <inheritdoc/>
    public int CreateSupplier(SupplierEntity entity)
    {
        lock (_fileLock)
        {
            entity.ID = _suppliers.Any() ? _suppliers.Max(s => s.ID) + 1 : 1;
            _suppliers.Add(entity);
            SaveSuppliers();
            return entity.ID;
        }
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

    private void SaveSuppliers()
    {
        lock (_fileLock)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                File.WriteAllText(_suppliersPath, JsonSerializer.Serialize(_suppliers, options));
            }
            catch (Exception ex) { Console.WriteLine($"Error saving suppliers data: {ex.Message}"); }
        }
    }
}
