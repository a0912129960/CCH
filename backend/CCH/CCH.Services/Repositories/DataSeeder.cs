using CCH.Core.Entities;
using CCH.Core.Entities.CSP;
using CCH.Core.Entities.ReSm;
using System.Text.Json;

namespace CCH.Services.Repositories;

/// <summary>
/// Centralized handler for mock data initialization.
/// (繁體中文) 測試資料初始化的集中處理器。
/// </summary>
public static class DataSeeder
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    private static readonly List<PartEntity> DefaultParts = new() {
        new() { ID = 1, CustomerID = 101, PartNo = "PART-001", CountryID = 1, PartDescription = "Electronic Controller Unit", Division = "Electronics", SupplierID = 1, HTSCode = "8537.10.9170", DutyRate = 0.0m, Status = "S04", UpdatedBy = "Admin", UpdatedDate = DateTime.Now.AddDays(-5), AddHTSCode1 = "8537.10.0000", AddDutyRate1 = 0.5m },
        new() { ID = 2, CustomerID = 102, PartNo = "PART-002", CountryID = 2, PartDescription = "Hydraulic Pump Assembly", Division = "Mechanical", SupplierID = 4, HTSCode = "8413.50.0010", DutyRate = 5.0m, Status = "S02", UpdatedBy = "User X", UpdatedDate = DateTime.Now.AddDays(-1), AddHTSCode1 = "8413.91.0000", AddDutyRate1 = 2.5m, AddHTSCode2 = "8413.92.0000", AddDutyRate2 = 1.0m },
        new() { ID = 3, CustomerID = 101, PartNo = "PART-003", CountryID = 3, PartDescription = "Precision Sensor Module", Division = "Electronics", SupplierID = 2, HTSCode = "9032.89.6085", DutyRate = 0.0m, Status = "S01", UpdatedBy = "User Y", UpdatedDate = DateTime.Now.AddHours(-2) },
        new() { ID = 4, CustomerID = 103, PartNo = "PART-004", CountryID = 4, PartDescription = "Steel Support Bracket", Division = "Structural", SupplierID = 7, HTSCode = "7326.90.8688", DutyRate = 2.5m, Status = "S03", UpdatedBy = "User Z", UpdatedDate = DateTime.Now.AddHours(-40), AddHTSCode1 = "7326.90.0000", AddDutyRate1 = 1.5m },
        new() { ID = 5, CustomerID = 101, PartNo = "PART-005", CountryID = 1, PartDescription = "Aluminum Housing Case", Division = "Mechanical", SupplierID = 3, HTSCode = "7616.99.5190", DutyRate = 0.0m, Status = "S04", UpdatedBy = "Admin", UpdatedDate = DateTime.Now.AddHours(-10) },
        new() { ID = 6, CustomerID = 102, PartNo = "PART-006", CountryID = 2, PartDescription = "Wiring Harness 12V", Division = "Electronics", SupplierID = 5, HTSCode = "8544.30.0000", DutyRate = 3.5m, Status = "S02", UpdatedBy = "User X", UpdatedDate = DateTime.Now.AddDays(-4) },
        new() { ID = 7, CustomerID = 103, PartNo = "PART-007", CountryID = 1, PartDescription = "LED Display Panel", Division = "Electronics", SupplierID = 8, HTSCode = "8531.20.0040", DutyRate = 0.0m, Status = "S01", UpdatedBy = "Admin", UpdatedDate = DateTime.Now },
        new() { ID = 8, CustomerID = 101, PartNo = "PART-008", CountryID = 3, PartDescription = "Gasket Seal Kit", Division = "Mechanical", SupplierID = 3, HTSCode = "4016.93.5050", DutyRate = 2.0m, Status = "S04", UpdatedBy = "Admin", UpdatedDate = DateTime.Now.AddHours(-35) },
        new() { ID = 9, CustomerID = 102, PartNo = "PART-009", CountryID = 2, PartDescription = "Circuit Breaker 50A", Division = "Electronics", SupplierID = 6, HTSCode = "8536.20.0020", DutyRate = 4.0m, Status = "S02", UpdatedBy = "User X", UpdatedDate = DateTime.Now.AddDays(-2) },
        new() { ID = 10, CustomerID = 103, PartNo = "PART-010", CountryID = 4, PartDescription = "Cooling Fan 120mm", Division = "Electronics", SupplierID = 9, HTSCode = "8414.59.6040", DutyRate = 0.0m, Status = "S01", UpdatedBy = "Admin", UpdatedDate = DateTime.Now.AddHours(-5) },
        new() { ID = 11, CustomerID = 101, PartNo = "PART-011", CountryID = 1, PartDescription = "Capacitor 100uF", Division = "Electronics", SupplierID = 1, HTSCode = "8532.22.0040", DutyRate = 0.0m, Status = "S04", UpdatedBy = "Admin", UpdatedDate = DateTime.Now.AddDays(-8) },
        new() { ID = 12, CustomerID = 102, PartNo = "PART-012", CountryID = 2, PartDescription = "Resistor 10k Ohm", Division = "Electronics", SupplierID = 1, HTSCode = "8533.21.0080", DutyRate = 0.0m, Status = "S04", UpdatedBy = "Admin", UpdatedDate = DateTime.Now.AddHours(-8) },
        new() { ID = 13, CustomerID = 103, PartNo = "PART-013", CountryID = 3, PartDescription = "Transformer 220V/12V", Division = "Electronics", SupplierID = 6, HTSCode = "8504.31.4035", DutyRate = 1.5m, Status = "S03", UpdatedBy = "User Z", UpdatedDate = DateTime.Now.AddDays(-3), AddHTSCode1 = "8504.90.0000", AddDutyRate1 = 0.5m },
        new() { ID = 14, CustomerID = 101, PartNo = "PART-014", CountryID = 4, PartDescription = "Fastener Bolt M8", Division = "Mechanical", SupplierID = 7, HTSCode = "7318.15.2095", DutyRate = 0.0m, Status = "S04", UpdatedBy = "Admin", UpdatedDate = DateTime.Now.AddDays(-6) },
        new() { ID = 15, CustomerID = 102, PartNo = "PART-015", CountryID = 1, PartDescription = "O-Ring Seal 20mm", Division = "Mechanical", SupplierID = 3, HTSCode = "4016.93.1050", DutyRate = 0.0m, Status = "S04", UpdatedBy = "Admin", UpdatedDate = DateTime.Now.AddHours(-6) },
        new() { ID = 16, CustomerID = 103, PartNo = "PART-016", CountryID = 2, PartDescription = "Ignition Coil", Division = "Mechanical", SupplierID = 7, HTSCode = "8511.30.0040", DutyRate = 2.5m, Status = "S02", UpdatedBy = "User X", UpdatedDate = DateTime.Now.AddHours(-4) },
        new() { ID = 17, CustomerID = 101, PartNo = "PART-017", CountryID = 3, PartDescription = "Fuel Injector Nozzle", Division = "Mechanical", SupplierID = 1, HTSCode = "8409.91.4000", DutyRate = 0.0m, Status = "S01", UpdatedBy = "Admin", UpdatedDate = DateTime.Now.AddHours(-1) }
    };

    /* INTERNAL-AI-20260421: DefaultCustomers and SeedCustomers removed as data migrated to ReSm SQL Database. */
    /* INTERNAL-AI-20260421: DefaultCountries and SeedCountries removed as data migrated to ReSm SQL Database. */
    /* INTERNAL-AI-20260421: DefaultStatuses and SeedStatuses removed as data migrated to Code Constants (PartStatusConstants). */

    private static readonly List<SupplierEntity> DefaultSuppliers = new() {
        new() { ID = 1, CustomerID = 101, Name = "TechSupply Corp" },
        new() { ID = 2, CustomerID = 101, Name = "SensorTech Solutions" },
        new() { ID = 3, CustomerID = 101, Name = "AluFab Co" },
        new() { ID = 4, CustomerID = 102, Name = "FluidDynamics Ltd" },
        new() { ID = 5, CustomerID = 102, Name = "CableConnect" },
        new() { ID = 6, CustomerID = 102, Name = "PowerGuard" },
        new() { ID = 7, CustomerID = 103, Name = "IronWorks Inc" },
        new() { ID = 8, CustomerID = 103, Name = "OpticView" },
        new() { ID = 9, CustomerID = 103, Name = "FanTech" }
    };

    public static void SeedParts(string path) => EnsureInitialized(path, DefaultParts);
    public static void SeedSuppliers(string path) => EnsureInitialized(path, DefaultSuppliers);

    public static void SeedPartSnapshots(string path)
    {
        if (File.Exists(path)) return;
        var supplierMap = DefaultSuppliers.ToDictionary(s => s.ID, s => s.Name);
        var snapshots = DefaultParts.Select((p, idx) => new PartSnapshotEntity
        {
            ID = idx + 1, PartID = p.ID, PartNo = p.PartNo,
            Country = "Unknown", // Country names resolved from DB at runtime
            Division = p.Division, Supplier = supplierMap.GetValueOrDefault(p.SupplierID, "Unknown"),
            PartDesc = p.PartDescription, HtsCode = p.HTSCode, Rate = p.DutyRate,
            Remark = p.Remark, UpdatedBy = p.UpdatedBy, UpdatedDate = p.UpdatedDate
        }).ToList();
        EnsureInitialized(path, snapshots);
    }

    public static void SeedPartHistory(string path)
    {
        if (File.Exists(path)) return;
        var history = new List<PartHistoryEntity>();
        int id = 1;
        foreach (var part in DefaultParts)
        {
            var baseDate = part.UpdatedDate;
            history.Add(new() { ID = id++, PartID = part.ID, Action = "Created", UpdatedBy = "System", UpdatedDate = baseDate.AddDays(-7), Remark = "" });
            if (part.Status == "S01") continue;
            history.Add(new() { ID = id++, PartID = part.ID, Action = "Submitted to Dimerco", UpdatedBy = "Customer", UpdatedDate = baseDate.AddDays(-5), Remark = "" });
            if (part.Status == "S04")
                history.Add(new() { ID = id++, PartID = part.ID, Action = "Accepted", UpdatedBy = "DCB", UpdatedDate = part.UpdatedDate, Remark = "" });
        }
        EnsureInitialized(path, history);
    }

    private static readonly object _fileLock = new();
    private static void EnsureInitialized<T>(string filePath, IEnumerable<T> defaultData)
    {
        if (File.Exists(filePath)) return;
        lock (_fileLock)
        {
            if (File.Exists(filePath)) return;
            var dir = Path.GetDirectoryName(filePath);
            if (dir != null && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
            try { File.WriteAllText(filePath, JsonSerializer.Serialize(defaultData, JsonOptions)); }
            catch (IOException ex) { Console.WriteLine($"Concurrent initialization handled: {ex.Message}"); }
        }
    }
}
