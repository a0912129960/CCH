using CCH.Core.DTOs;
using CCH.Core.Interfaces;
using System.Text;

namespace CCH.Services.Implementations;

/// <summary>
/// Mock part service.
/// (繁體中文) 模擬零件服務。
/// </summary>
public class PartService : IPartService
{
    public PartListResponseDto SearchParts(string? customerId, string? status, string? partNo, string? supplier, int page, int pageSize) => new()
    {
        Total = 100,
        Page = page,
        Data = new[]
        {
            new PartListItemDto { Id = "P001", Customer = "Customer A", PartNo = "PART-001", PartDesc = "Desc 001", Country = "TW", HtsCode = "8471.30", Rate = 0, Status = "S04", UpdatedBy = "Admin", UpdatedDate = DateTime.Now, SlaStatus = "green" },
            new PartListItemDto { Id = "P002", Customer = "Customer B", PartNo = "PART-002", PartDesc = "Desc 002", Country = "CN", HtsCode = "8471.41", Rate = 5, Status = "S02", UpdatedBy = "User X", UpdatedDate = DateTime.Now.AddDays(-1), SlaStatus = "yellow" }
        }
    };

    public byte[] ExportParts(string? customerId, string? status, string? partNo, string? supplier) => 
        Encoding.UTF8.GetBytes("Mock Excel Content");

    public object BatchAccept(IEnumerable<string> partIds) => new { failed = new List<object>() };

    public BulkUploadResponseDto BulkUpload(Stream fileStream) => new()
    {
        Inserted = 10, Updated = 5, Skipped = 2, Errors = new List<string>()
    };

    public byte[] GetUploadTemplate() => Encoding.UTF8.GetBytes("Mock Template Content");

    public PartDetailResponseDto GetPartDetail(string partId) => new()
    {
        Before = new PartDetailDto { PartNo = "PART-001", Country = "TW", Division = "DIV1", Supplier = "SUP1", PartDesc = "Old Desc", HtsCode = "8471.30", Rate = 0 },
        Modified = new PartDetailDto { PartNo = "PART-001", Country = "TW", Division = "DIV1", Supplier = "SUP1", PartDesc = "New Desc", HtsCode = "8471.30", Rate = 0 }
    };

    public object CreatePart(PartSaveRequest request, string status) => new { partId = "P999", partNo = request.PartNo, status };

    public object UpdatePart(string partId, PartSaveRequest request) => new { };

    public object SubmitPart(string partId, PartSaveRequest request) => new { partId, status = "S02" };

    public object AcceptPart(string partId) => new { partId, status = "S04" };

    public object ReturnPart(string partId, string returnReason) => new { partId, status = "S03" };

    public object InactivatePart(string partId) => new { partId, status = "Inactive" };

    public IEnumerable<MilestoneDto> GetMilestones(string partId) => new[]
    {
        new MilestoneDto { Action = "Created", UpdatedBy = "User A", UpdatedDate = DateTime.Now.AddDays(-5), Remark = "Initial entry" },
        new MilestoneDto { Action = "Submitted", UpdatedBy = "User A", UpdatedDate = DateTime.Now.AddDays(-4), Remark = "Ready for review" }
    };

    public IEnumerable<PartDetailDto> GetHistory(string partId) => new[]
    {
        new PartDetailDto { PartNo = "PART-001", PartDesc = "Version 1" },
        new PartDetailDto { PartNo = "PART-001", PartDesc = "Version 2" }
    };
}
