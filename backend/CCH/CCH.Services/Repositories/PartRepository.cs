using CCH.Core.DTOs;
using CCH.Core.Entities;
using CCH.Core.Interfaces.Repositories;
using System.Text.Json;

namespace CCH.Services.Repositories;

/// <summary>
/// Implementation of Part repository using relational JSON files with source path discovery.
/// (繁體中文) 具備原始碼路徑自動偵測與關聯式 JSON 持久化的零件倉儲實作。
/// </summary>
public class PartRepository : IPartRepository
{
    private readonly string _partsPath;
    private readonly string _customersPath;
    private readonly string _countriesPath;

    private List<PartEntity> _parts = new();
    private List<CustomerEntity> _customers = new();
    private List<CountryEntity> _countries = new();

    private static readonly object _fileLock = new();

    public PartRepository(string? overridePath = null)
    {
        string dataDir;
        if (!string.IsNullOrEmpty(overridePath))
        {
            dataDir = Path.GetDirectoryName(overridePath)!;
            _partsPath = overridePath;
        }
        else
        {
            // Force Project Root Discovery: Navigate up 5 levels from bin/Debug/net10.0/
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var projectRootDir = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", ".."));
            var sourceDir = Path.Combine(projectRootDir, "Data");


            dataDir = sourceDir;
            _partsPath = Path.Combine(dataDir, "parts.json");
        }

        _customersPath = Path.Combine(dataDir, "customers.json");
        _countriesPath = Path.Combine(dataDir, "countries.json");

        LoadAllData();
    }

    private void LoadAllData()
    {
        lock (_fileLock)
        {
            // Ensure directory exists (確保目錄存在)
            var dir = Path.GetDirectoryName(_partsPath);
            if (dir != null && !Directory.Exists(dir)) Directory.CreateDirectory(dir);

            if (!File.Exists(_partsPath) || !File.Exists(_customersPath) || !File.Exists(_countriesPath))
            {
                SeedData();
                return;
            }

            try
            {
                _parts = JsonSerializer.Deserialize<List<PartEntity>>(File.ReadAllText(_partsPath)) ?? new();
                _customers = JsonSerializer.Deserialize<List<CustomerEntity>>(File.ReadAllText(_customersPath)) ?? new();
                _countries = JsonSerializer.Deserialize<List<CountryEntity>>(File.ReadAllText(_countriesPath)) ?? new();
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

        _parts = new List<PartEntity> {
            new() { ID = 1, CustomerID = 101, PartNo = "PART-001", CountryID = 1, PartDescription = "Electronic Controller Unit", Division = "Electronics", Supplier = "TechSupply Corp", HTSCode = "8537.10.9170", DutyRate = 0.0m, Status = "S04", UpdatedBy = "Admin", UpdatedDate = DateTime.Now.AddDays(-5), AddHTSCode1 = "8537.10.0000", AddDutyRate1 = 0.5m },
            new() { ID = 2, CustomerID = 102, PartNo = "PART-002", CountryID = 2, PartDescription = "Hydraulic Pump Assembly", Division = "Mechanical", Supplier = "FluidDynamics Ltd", HTSCode = "8413.50.0010", DutyRate = 5.0m, Status = "S02", UpdatedBy = "User X", UpdatedDate = DateTime.Now.AddDays(-1), AddHTSCode1 = "8413.91.0000", AddDutyRate1 = 2.5m, AddHTSCode2 = "8413.92.0000", AddDutyRate2 = 1.0m },
            new() { ID = 3, CustomerID = 101, PartNo = "PART-003", CountryID = 3, PartDescription = "Precision Sensor Module", Division = "Electronics", Supplier = "SensorTech Solutions", HTSCode = "9032.89.6085", DutyRate = 0.0m, Status = "S01", UpdatedBy = "User Y", UpdatedDate = DateTime.Now.AddHours(-2) },
            new() { ID = 4, CustomerID = 103, PartNo = "PART-004", CountryID = 4, PartDescription = "Steel Support Bracket", Division = "Structural", Supplier = "IronWorks Inc", HTSCode = "7326.90.8688", DutyRate = 2.5m, Status = "S03", UpdatedBy = "User Z", UpdatedDate = DateTime.Now.AddMinutes(-30), AddHTSCode1 = "7326.90.0000", AddDutyRate1 = 1.5m },
            new() { ID = 5, CustomerID = 101, PartNo = "PART-005", CountryID = 1, PartDescription = "Aluminum Housing Case", Division = "Mechanical", Supplier = "AluFab Co", HTSCode = "7616.99.5190", DutyRate = 0.0m, Status = "S04", UpdatedBy = "Admin", UpdatedDate = DateTime.Now.AddDays(-10) },
            new() { ID = 6, CustomerID = 102, PartNo = "PART-006", CountryID = 2, PartDescription = "Wiring Harness 12V", Division = "Electronics", Supplier = "CableConnect", HTSCode = "8544.30.0000", DutyRate = 3.5m, Status = "S02", UpdatedBy = "User X", UpdatedDate = DateTime.Now.AddDays(-4) },
            new() { ID = 7, CustomerID = 103, PartNo = "PART-007", CountryID = 1, PartDescription = "LED Display Panel", Division = "Electronics", Supplier = "OpticView", HTSCode = "8531.20.0040", DutyRate = 0.0m, Status = "S01", UpdatedBy = "Admin", UpdatedDate = DateTime.Now },
            new() { ID = 8, CustomerID = 101, PartNo = "PART-008", CountryID = 3, PartDescription = "Gasket Seal Kit", Division = "Mechanical", Supplier = "SealPro", HTSCode = "4016.93.5050", DutyRate = 2.0m, Status = "S04", UpdatedBy = "Admin", UpdatedDate = DateTime.Now.AddDays(-20) },
            new() { ID = 9, CustomerID = 102, PartNo = "PART-009", CountryID = 2, PartDescription = "Circuit Breaker 50A", Division = "Electronics", Supplier = "PowerGuard", HTSCode = "8536.20.0020", DutyRate = 4.0m, Status = "S02", UpdatedBy = "User X", UpdatedDate = DateTime.Now.AddDays(-2) },
            new() { ID = 10, CustomerID = 103, PartNo = "PART-010", CountryID = 4, PartDescription = "Cooling Fan 120mm", Division = "Electronics", Supplier = "FanTech", HTSCode = "8414.59.6040", DutyRate = 0.0m, Status = "S01", UpdatedBy = "Admin", UpdatedDate = DateTime.Now.AddHours(-5) },
            new() { ID = 11, CustomerID = 101, PartNo = "PART-011", CountryID = 1, PartDescription = "Capacitor 100uF", Division = "Electronics", Supplier = "TechSupply Corp", HTSCode = "8532.22.0040", DutyRate = 0.0m, Status = "S04", UpdatedBy = "Admin", UpdatedDate = DateTime.Now.AddDays(-8) },
            new() { ID = 12, CustomerID = 102, PartNo = "PART-012", CountryID = 2, PartDescription = "Resistor 10k Ohm", Division = "Electronics", Supplier = "TechSupply Corp", HTSCode = "8533.21.0080", DutyRate = 0.0m, Status = "S04", UpdatedBy = "Admin", UpdatedDate = DateTime.Now.AddDays(-8) },
            new() { ID = 13, CustomerID = 103, PartNo = "PART-013", CountryID = 3, PartDescription = "Transformer 220V/12V", Division = "Electronics", Supplier = "PowerGuard", HTSCode = "8504.31.4035", DutyRate = 1.5m, Status = "S03", UpdatedBy = "User Z", UpdatedDate = DateTime.Now.AddDays(-3), AddHTSCode1 = "8504.90.0000", AddDutyRate1 = 0.5m },
            new() { ID = 14, CustomerID = 101, PartNo = "PART-014", CountryID = 4, PartDescription = "Fastener Bolt M8", Division = "Mechanical", Supplier = "IronWorks Inc", HTSCode = "7318.15.2095", DutyRate = 0.0m, Status = "S04", UpdatedBy = "Admin", UpdatedDate = DateTime.Now.AddDays(-6) },
            new() { ID = 15, CustomerID = 102, PartNo = "PART-015", CountryID = 1, PartDescription = "O-Ring Seal 20mm", Division = "Mechanical", Supplier = "SealPro", HTSCode = "4016.93.1050", DutyRate = 0.0m, Status = "S04", UpdatedBy = "Admin", UpdatedDate = DateTime.Now.AddDays(-6) },
            new() { ID = 16, CustomerID = 103, PartNo = "PART-016", CountryID = 2, PartDescription = "Ignition Coil", Division = "Mechanical", Supplier = "SparkParts", HTSCode = "8511.30.0040", DutyRate = 2.5m, Status = "S02", UpdatedBy = "User X", UpdatedDate = DateTime.Now.AddDays(-4) },
            new() { ID = 17, CustomerID = 101, PartNo = "PART-017", CountryID = 3, PartDescription = "Fuel Injector Nozzle", Division = "Mechanical", Supplier = "FuelTech", HTSCode = "8409.91.4000", DutyRate = 0.0m, Status = "S01", UpdatedBy = "Admin", UpdatedDate = DateTime.Now.AddHours(-1) }
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
                File.WriteAllText(_partsPath, JsonSerializer.Serialize(_parts, options));
                File.WriteAllText(_customersPath, JsonSerializer.Serialize(_customers, options));
                File.WriteAllText(_countriesPath, JsonSerializer.Serialize(_countries, options));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving relational data: {ex.Message}");
            }
        }
    }

    private PartListItemDto MapToDto(PartEntity entity)
    {
        var customerName = _customers.FirstOrDefault(c => c.ID == entity.CustomerID)?.Name ?? "Unknown";
        var countryName = _countries.FirstOrDefault(c => c.ID == entity.CountryID)?.Name ?? "Unknown";

        return new PartListItemDto
        {
            Id = entity.ID,
            Customer = customerName,
            PartNo = entity.PartNo,
            PartDesc = entity.PartDescription,
            Country = countryName,
            HtsCode = entity.HTSCode,
            Rate = entity.DutyRate,
            Status = entity.Status,
            UpdatedBy = entity.UpdatedBy,
            UpdatedDate = entity.UpdatedDate,
            SlaStatus = (DateTime.Now - entity.UpdatedDate).TotalDays > 7 ? "red" : 
                        (DateTime.Now - entity.UpdatedDate).TotalDays > 3 ? "yellow" : "green",
            HtsCode1 = entity.AddHTSCode1,
            Rate1 = entity.AddDutyRate1,
            HtsCode2 = entity.AddHTSCode2,
            Rate2 = entity.AddDutyRate2,
            HtsCode3 = entity.AddHTSCode3,
            Rate3 = entity.AddDutyRate3,
            HtsCode4 = entity.AddHTSCode4,
            Rate4 = entity.AddDutyRate4
        };
    }

    public IEnumerable<PartListItemDto> SearchParts(string? customerId, string? status, string? partNo, string? supplier)
    {
        var query = _parts.AsQueryable();

        if (!string.IsNullOrEmpty(customerId))
        {
            if (int.TryParse(customerId, out int cId))
                query = query.Where(p => p.CustomerID == cId);
            else
            {
                var customerIds = _customers.Where(c => c.Name.Contains(customerId, StringComparison.OrdinalIgnoreCase)).Select(c => c.ID);
                query = query.Where(p => customerIds.Contains(p.CustomerID));
            }
        }

        if (!string.IsNullOrEmpty(status))
            query = query.Where(p => p.Status.Equals(status, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrEmpty(partNo))
            query = query.Where(p => p.PartNo.Contains(partNo, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrEmpty(supplier))
            query = query.Where(p => p.Supplier.Contains(supplier, StringComparison.OrdinalIgnoreCase));

        return query.ToList().Select(MapToDto);
    }

    public PartDetailResponseDto? GetPartDetail(int partId)
    {
        if (partId <= 0) return null;
        
        var part = _parts.FirstOrDefault(p => p.ID == partId);
        if (part == null) return null;

        var dto = MapToDto(part);

        return new PartDetailResponseDto
        {
            Before = new PartDetailDto { PartNo = dto.PartNo, Country = dto.Country, Division = part.Division, Supplier = part.Supplier, PartDesc = dto.PartDesc, HtsCode = dto.HtsCode, Rate = dto.Rate, Remark = part.Remark, UpdatedBy = dto.UpdatedBy, UpdatedDate = dto.UpdatedDate },
            Modified = new PartDetailDto { PartNo = dto.PartNo, Country = dto.Country, Division = part.Division, Supplier = part.Supplier, PartDesc = dto.PartDesc, HtsCode = dto.HtsCode, Rate = dto.Rate, Remark = part.Remark, UpdatedBy = dto.UpdatedBy, UpdatedDate = dto.UpdatedDate }
        };
    }

    public int CreatePart(PartSaveRequest request, string status)
    {
        var newId = _parts.Any() ? _parts.Max(p => p.ID) + 1 : 1;
        var entity = new PartEntity
        {
            ID = newId,
            CustomerID = request.CustomerId ?? 101,
            PartNo = request.PartNo,
            CountryID = request.CountryId ?? 1,
            PartDescription = request.PartDesc,
            Division = request.Division,
            Supplier = request.Supplier,
            HTSCode = request.HtsCode,
            DutyRate = request.Rate,
            AddHTSCode1 = request.HtsCode1,
            AddDutyRate1 = request.Rate1,
            AddHTSCode2 = request.HtsCode2,
            AddDutyRate2 = request.Rate2,
            AddHTSCode3 = request.HtsCode3,
            AddDutyRate3 = request.Rate3,
            AddHTSCode4 = request.HtsCode4,
            AddDutyRate4 = request.Rate4,
            Remark = request.Remark,
            Status = status,
            CreatedBy = "AI-System",
            UpdatedBy = "AI-System",
            CreatedDate = DateTime.Now,
            UpdatedDate = DateTime.Now
        };
        
        _parts.Add(entity);
        SaveAllData();
        return newId;
    }

    public void UpdatePart(int partId, PartSaveRequest request)
    {
        var part = _parts.FirstOrDefault(p => p.ID == partId);
        if (part != null)
        {
            part.PartNo = request.PartNo;
            part.PartDescription = request.PartDesc;
            part.Division = request.Division;
            part.Supplier = request.Supplier;
            part.HTSCode = request.HtsCode;
            part.DutyRate = request.Rate;
            part.AddHTSCode1 = request.HtsCode1;
            part.AddDutyRate1 = request.Rate1;
            part.Remark = request.Remark;
            part.UpdatedDate = DateTime.Now;
            SaveAllData();
        }
    }

    public void UpdateStatus(int partId, string status)
    {
        var part = _parts.FirstOrDefault(p => p.ID == partId);
        if (part != null)
        {
            part.Status = status;
            part.UpdatedDate = DateTime.Now;
            SaveAllData();
        }
    }
}
