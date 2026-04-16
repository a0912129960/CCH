using CCH.Core.DTOs;
using CCH.Core.Interfaces.Repositories;

namespace CCH.Services.Repositories;

/// <summary>
/// Mock implementation of Part repository.
/// (繁體中文) 零件倉儲模擬實作。
/// </summary>
public class PartRepository : IPartRepository
{
    private readonly List<PartListItemDto> _mockParts = new()
    {
        new PartListItemDto { Id = 1, Customer = "Customer A", PartNo = "PART-001", PartDesc = "Desc 001", Country = "TW", HtsCode = "8471.30", Rate = 0, Status = "S04", UpdatedBy = "Admin", UpdatedDate = DateTime.Now.AddDays(-5), SlaStatus = "green" },
        new PartListItemDto { Id = 2, Customer = "Customer B", PartNo = "PART-002", PartDesc = "Desc 002", Country = "CN", HtsCode = "8471.41", Rate = 5, Status = "S02", UpdatedBy = "User X", UpdatedDate = DateTime.Now.AddDays(-1), SlaStatus = "yellow" },
        new PartListItemDto { Id = 3, Customer = "Customer A", PartNo = "PART-003", PartDesc = "Desc 003", Country = "US", HtsCode = "8517.12", Rate = 0, Status = "S01", UpdatedBy = "User Y", UpdatedDate = DateTime.Now.AddHours(-2), SlaStatus = "red" },
        new PartListItemDto { Id = 4, Customer = "Customer C", PartNo = "PART-004", PartDesc = "Desc 004", Country = "JP", HtsCode = "8517.62", Rate = 2.5m, Status = "S03", UpdatedBy = "User Z", UpdatedDate = DateTime.Now.AddMinutes(-30), SlaStatus = "orange" }
    };

    public IEnumerable<PartListItemDto> SearchParts(string? customerId, string? status, string? partNo, string? supplier)
    {
        var query = _mockParts.AsQueryable();

        if (!string.IsNullOrEmpty(customerId))
            query = query.Where(p => p.Customer.Contains(customerId, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrEmpty(status))
            query = query.Where(p => p.Status.Equals(status, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrEmpty(partNo))
            query = query.Where(p => p.PartNo.Contains(partNo, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrEmpty(supplier))
            query = query.Where(p => p.PartDesc.Contains(supplier, StringComparison.OrdinalIgnoreCase));

        return query.ToList();
    }

    public PartDetailResponseDto? GetPartDetail(int partId)
    {
        // Simulate database lookup (模擬資料庫查詢)
        if (partId <= 0) return null;

        return new PartDetailResponseDto
        {
            Before = new PartDetailDto { PartNo = "PART-001", Country = "TW", Division = "DIV1", Supplier = "SUP1", PartDesc = "Old Desc", HtsCode = "8471.30.0000", Rate = 0, Remark = "", UpdatedBy = "Customer001", UpdatedDate = DateTime.Now.AddDays(-5) },
            Modified = new PartDetailDto { PartNo = "PART-001", Country = "TW", Division = "DIV1", Supplier = "SUP1", PartDesc = "New Desc", HtsCode = "8471.30.0000", Rate = 0, HtsCode1 = "8517.12.0000", Rate1 = 5.5m, Remark = "Updated HTS code", UpdatedBy = "Customer001", UpdatedDate = DateTime.Now }
        };
    }

    public int CreatePart(PartSaveRequest request, string status)
    {
        // Simulate saving to DB and returning new ID (模擬存入資料庫並回傳新 ID)
        return 3;
    }

    public void UpdatePart(int partId, PartSaveRequest request)
    {
        // Simulate DB update (模擬資料庫更新)
    }

    public void UpdateStatus(int partId, string status)
    {
        // Simulate status update (模擬狀態更新)
    }
}
