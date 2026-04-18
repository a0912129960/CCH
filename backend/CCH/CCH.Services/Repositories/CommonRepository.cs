using CCH.Core.Entities;
using CCH.Core.Interfaces.Repositories;
using System.Text.Json;

namespace CCH.Services.Repositories;

/// <summary>
/// Implementation of Common repository using JSON file persistence and centralized seeding.
/// (繁體中文) 使用 JSON 檔案持久化與集中式種子資料的共用倉儲實作。
/// </summary>
public class CommonRepository : ICommonRepository
{
    private readonly string _customersPath;
    private readonly string _countriesPath;
    private readonly string _statusesPath;
    private readonly string _suppliersPath;

    private List<CustomerEntity> _customers = new();
    private List<CountryEntity> _countries = new();
    private List<StatusEntity> _statuses = new();
    private List<SupplierEntity> _suppliers = new();

    private static readonly object _fileLock = new();

    /// <summary>
    /// Initializes a new instance of CommonRepository.
    /// (繁體中文) 初始化 CommonRepository 的新執行個體。
    /// </summary>
    public CommonRepository()
    {
        // Path discovery relative to project root (相對於專案根目錄的路徑探索)
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var projectRootDir = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", ".."));
        var dataDir = Path.Combine(projectRootDir, "Data");

        _customersPath = Path.Combine(dataDir, "customers.json");
        _countriesPath = Path.Combine(dataDir, "countries.json");
        _statusesPath = Path.Combine(dataDir, "statuses.json");
        _suppliersPath = Path.Combine(dataDir, "suppliers.json");

        // Initialize only what this repository needs (僅初始化此倉儲需要的資料)
        DataSeeder.SeedCustomers(_customersPath);
        DataSeeder.SeedCountries(_countriesPath);
        DataSeeder.SeedStatuses(_statusesPath);
        DataSeeder.SeedSuppliers(_suppliersPath);

        LoadAllData();
    }

    private void LoadAllData()
    {
        lock (_fileLock)
        {
            try
            {
                _customers = JsonSerializer.Deserialize<List<CustomerEntity>>(File.ReadAllText(_customersPath)) ?? new();
                _countries = JsonSerializer.Deserialize<List<CountryEntity>>(File.ReadAllText(_countriesPath)) ?? new();
                _statuses = JsonSerializer.Deserialize<List<StatusEntity>>(File.ReadAllText(_statusesPath)) ?? new();
                _suppliers = JsonSerializer.Deserialize<List<SupplierEntity>>(File.ReadAllText(_suppliersPath)) ?? new();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading common data: {ex.Message}");
                _customers = new();
                _countries = new();
                _statuses = new();
                _suppliers = new();
            }
        }
    }

    /// <inheritdoc/>
    public IEnumerable<CustomerEntity> GetCustomers() => _customers;

    /// <inheritdoc/>
    public IEnumerable<CountryEntity> GetCountries() => _countries;

    /// <inheritdoc/>
    public IEnumerable<StatusEntity> GetStatuses() => _statuses;

    /// <inheritdoc/>
    public IEnumerable<SupplierEntity> GetSuppliers(int? customerId = null)
    {
        if (customerId == null) return _suppliers;
        return _suppliers.Where(s => s.CustomerID == customerId.Value);
    }
}
