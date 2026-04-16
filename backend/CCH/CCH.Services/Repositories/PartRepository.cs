using CCH.Core.DTOs;
using CCH.Core.Entities;
using CCH.Core.Interfaces.Repositories;
using System.Text.Json;

namespace CCH.Services.Repositories;

/// <summary>
/// Implementation of Part repository using relational JSON files.
/// (繁體中文) 使用關聯式 JSON 檔案的零件倉儲實作。
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

    public PartRepository(string? basePath = null)
    {
        // Define directory for JSON data files
        var dataDir = basePath != null ? Path.GetDirectoryName(basePath)! : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Repositories", "Data");
        
        _partsPath = basePath ?? Path.Combine(dataDir, "parts.json");
        _customersPath = Path.Combine(dataDir, "customers.json");
        _countriesPath = Path.Combine(dataDir, "countries.json");

        LoadAllData();
    }

    private void LoadAllData()
    {
        lock (_fileLock)
        {
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
            new() { ID = 1, CustomerID = 101, PartNo = "PART-001", CountryID = 1, PartDescription = "Desc 001", HTSCode = "8471.30", DutyRate = 0, Status = "S04", UpdatedBy = "Admin", UpdatedDate = DateTime.Now.AddDays(-5), SlaStatus = "green" },
            new() { ID = 2, CustomerID = 102, PartNo = "PART-002", CountryID = 2, PartDescription = "Desc 002", HTSCode = "8471.41", DutyRate = 5, Status = "S02", UpdatedBy = "User X", UpdatedDate = DateTime.Now.AddDays(-1), SlaStatus = "yellow" }
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
                var dataDir = Path.GetDirectoryName(_partsPath);
                if (dataDir != null && !Directory.Exists(dataDir)) Directory.CreateDirectory(dataDir);

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
            SlaStatus = entity.SlaStatus ?? "green",
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
                // Fallback: match customer name if ID is not provided
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
            UpdatedDate = DateTime.Now,
            SlaStatus = "green"
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
