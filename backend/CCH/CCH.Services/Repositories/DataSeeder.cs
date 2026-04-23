using CCH.Core.Entities;
using System.Text.Json;
// INTERNAL-AI-20260420: PartHistoryEntity used for SeedPartHistory. (PartHistoryEntity 用於種子歷程。)

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

    private static readonly List<CustomerEntity> DefaultCustomers = new() {
        new() { ID = 101, Name = "Customer A" },
        new() { ID = 102, Name = "Customer B" },
        new() { ID = 103, Name = "Customer C" }
    };

    private static readonly List<CountryEntity> DefaultCountries = new() {
        new() { ID = 1, Name = "Taiwan", Code = "TW" },
        new() { ID = 2, Name = "China", Code = "CN" },
        new() { ID = 3, Name = "USA", Code = "US" },
        new() { ID = 4, Name = "Japan", Code = "JP" }
    };

    private static readonly List<StatusEntity> DefaultStatuses = new() {
        new() { Code = "S01", Description = "Unknow" },
        new() { Code = "S02", Description = "Pending Dimerco Review" },
        new() { Code = "S03", Description = "Pending Customer Review" },
        new() { Code = "S04", Description = "Reviewed" },
        new() { Code = "S05", Description = "Flagged" }
    };

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
    public static void SeedCustomers(string path) => EnsureInitialized(path, DefaultCustomers);
    public static void SeedCountries(string path) => EnsureInitialized(path, DefaultCountries);
    public static void SeedStatuses(string path) => EnsureInitialized(path, DefaultStatuses);
    public static void SeedSuppliers(string path) => EnsureInitialized(path, DefaultSuppliers);

    // INTERNAL-AI-20260420: Generate one initial data snapshot per seeded part for the /history endpoint.
    // (INTERNAL-AI-20260420: 為每筆種子零件產生一筆初始資料快照，供 /history API 使用。)
    public static void SeedPartSnapshots(string path)
    {
        if (File.Exists(path)) return;

        // Resolve lookup maps from seeded reference data (從參考資料解析查找對應)
        var countryMap = DefaultCountries.ToDictionary(c => c.ID, c => c.Name);
        var supplierMap = DefaultSuppliers.ToDictionary(s => s.ID, s => s.Name);

        var snapshots = DefaultParts.Select((p, idx) => new PartSnapshotEntity
        {
            ID = idx + 1,
            PartID = p.ID,
            PartNo = p.PartNo,
            Country = countryMap.GetValueOrDefault(p.CountryID, "Unknown"),
            Division = p.Division,
            Supplier = supplierMap.GetValueOrDefault(p.SupplierID, "Unknown"),
            PartDesc = p.PartDescription,
            HtsCode = p.HTSCode,
            Rate = p.DutyRate,
            HtsCode1 = p.AddHTSCode1,
            Rate1 = p.AddDutyRate1,
            HtsCode2 = p.AddHTSCode2,
            Rate2 = p.AddDutyRate2,
            HtsCode3 = p.AddHTSCode3,
            Rate3 = p.AddDutyRate3,
            HtsCode4 = p.AddHTSCode4,
            Rate4 = p.AddDutyRate4,
            Remark = p.Remark,
            UpdatedBy = p.UpdatedBy,
            UpdatedDate = p.UpdatedDate
        }).ToList();

        EnsureInitialized(path, snapshots);
    }

    // INTERNAL-AI-20260420: Generate realistic history entries for each seeded part based on its status.
    // (INTERNAL-AI-20260420: 依各零件的狀態為每筆種子零件產生合理的歷程記錄。)
    public static void SeedPartHistory(string path)
    {
        if (File.Exists(path)) return;

        var history = new List<PartHistoryEntity>();
        int id = 1;

        foreach (var part in DefaultParts)
        {
            // All parts start as Created (所有零件起點皆為建立)
            var baseDate = part.UpdatedDate;

            history.Add(new() { ID = id++, PartID = part.ID, Action = "Created", UpdatedBy = "System", UpdatedDate = baseDate.AddDays(-7), Remark = "" });

            if (part.Status == "S01") continue; // S01 → only Created entry (S01 僅有建立記錄)

            history.Add(new() { ID = id++, PartID = part.ID, Action = "Submitted to Dimerco", UpdatedBy = "Customer", UpdatedDate = baseDate.AddDays(-5), Remark = "" });

            if (part.Status == "S02") continue; // S02 → waiting for review (S02 等待審核中)

            if (part.Status == "S03")
            {
                history.Add(new() { ID = id++, PartID = part.ID, Action = "Returned to Customer", UpdatedBy = "DCB", UpdatedDate = part.UpdatedDate, Remark = "Please provide additional documentation." });
                continue;
            }

            if (part.Status == "S04")
            {
                history.Add(new() { ID = id++, PartID = part.ID, Action = "Accepted", UpdatedBy = "DCB", UpdatedDate = part.UpdatedDate, Remark = "" });
                continue;
            }

            if (part.Status == "Inactive")
            {
                history.Add(new() { ID = id++, PartID = part.ID, Action = "Inactivated", UpdatedBy = "Customer", UpdatedDate = part.UpdatedDate, Remark = "" });
            }
        }

        EnsureInitialized(path, history);
    }

    private static readonly object _fileLock = new();

    private static void EnsureInitialized<T>(string filePath, IEnumerable<T> defaultData)
    {
        // First check outside the lock for performance (鎖外第一次檢查以提升效能)
        if (File.Exists(filePath)) return;

        lock (_fileLock)
        {
            // Double-check inside the lock to handle race conditions (鎖內第二次檢查以處理競爭情況)
            if (File.Exists(filePath)) return;

            var dir = Path.GetDirectoryName(filePath);
            if (dir != null && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            try
            {
                File.WriteAllText(filePath, JsonSerializer.Serialize(defaultData, JsonOptions));
            }
            catch (IOException ex)
            {
                // Log or handle the case where another process/thread might have just finished writing
                // (記錄或處理另一個行程/執行序可能剛好完成寫入的情況)
                Console.WriteLine($"Concurrect initialization handled: {ex.Message}");
            }
        }
    }
}
