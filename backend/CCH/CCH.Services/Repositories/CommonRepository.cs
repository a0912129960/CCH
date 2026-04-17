using CCH.Core.Entities;
using CCH.Core.Interfaces.Repositories;
using System.Text.Json;

namespace CCH.Services.Repositories;

/// <summary>
/// Implementation of Common repository using JSON files.
/// (繁體中文) 使用 JSON 檔案的共用倉儲實作。
/// </summary>
public class CommonRepository : ICommonRepository
{
    private readonly string _customersPath;
    private readonly string _countriesPath;
    private readonly string _statusesPath;

    private List<CustomerEntity> _customers = new();
    private List<CountryEntity> _countries = new();
    private List<StatusEntity> _statuses = new();

    private static readonly object _fileLock = new();

    public CommonRepository()
    {
        // Force Project Root Discovery: Navigate up 5 levels from bin/Debug/net10.0/
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var projectRootDir = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", ".."));
        var dataDir = Path.Combine(projectRootDir, "Data");

        _customersPath = Path.Combine(dataDir, "customers.json");
        _countriesPath = Path.Combine(dataDir, "countries.json");
        _statusesPath = Path.Combine(dataDir, "statuses.json");

        LoadAllData();
    }

    private void LoadAllData()
    {
        lock (_fileLock)
        {
            if (!File.Exists(_customersPath) || !File.Exists(_countriesPath) || !File.Exists(_statusesPath))
            {
                SeedData();
                return;
            }

            try
            {
                _customers = JsonSerializer.Deserialize<List<CustomerEntity>>(File.ReadAllText(_customersPath)) ?? new();
                _countries = JsonSerializer.Deserialize<List<CountryEntity>>(File.ReadAllText(_countriesPath)) ?? new();
                _statuses = JsonSerializer.Deserialize<List<StatusEntity>>(File.ReadAllText(_statusesPath)) ?? new();
            }
            catch
            {
                SeedData();
            }
        }
    }

    private void SeedData()
    {
        _customers = new List<CustomerEntity> {
            new() { ID = 101, Name = "Customer A" },
            new() { ID = 102, Name = "Customer B" },
            new() { ID = 103, Name = "Customer C" }
        };

        _countries = new List<CountryEntity> {
            new() { ID = 1, Name = "Taiwan", Code = "TW" },
            new() { ID = 2, Name = "China", Code = "CN" },
            new() { ID = 3, Name = "USA", Code = "US" },
            new() { ID = 4, Name = "Japan", Code = "JP" }
        };

        _statuses = new List<StatusEntity> {
            new() { Code = "S01", Description = "Unknow" },
            new() { Code = "S02", Description = "Pending Dimerco Review" },
            new() { Code = "S03", Description = "Pending Customer Review" },
            new() { Code = "S04", Description = "Reviewed" },
            new() { Code = "S05", Description = "Flagged" },
            new() { Code = "", Description = "Inactive" }
        };

        SaveAllData();
    }

    private void SaveAllData()
    {
        lock (_fileLock)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                File.WriteAllText(_customersPath, JsonSerializer.Serialize(_customers, options));
                File.WriteAllText(_countriesPath, JsonSerializer.Serialize(_countries, options));
                File.WriteAllText(_statusesPath, JsonSerializer.Serialize(_statuses, options));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving common data: {ex.Message}");
            }
        }
    }

    public IEnumerable<CustomerEntity> GetCustomers() => _customers;
    public IEnumerable<CountryEntity> GetCountries() => _countries;
    public IEnumerable<StatusEntity> GetStatuses() => _statuses;
}
