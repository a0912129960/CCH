using CCH.Core.DTOs;
using CCH.Core.Interfaces;
using System.Text;

namespace CCH.Services.Services;

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
            new PartListItemDto { Id = 1, Customer = "Customer A", PartNo = "PART-001", PartDesc = "Desc 001", Country = "TW", HtsCode = "8471.30", Rate = 0, Status = "S04", UpdatedBy = "Admin", UpdatedDate = DateTime.Now, SlaStatus = "green" },
            new PartListItemDto { Id = 2, Customer = "Customer B", PartNo = "PART-002", PartDesc = "Desc 002", Country = "CN", HtsCode = "8471.41", Rate = 5, Status = "S02", UpdatedBy = "User X", UpdatedDate = DateTime.Now.AddDays(-1), SlaStatus = "yellow" }
        }
    };

    public byte[] ExportParts(string? customerId, string? status, string? partNo, string? supplier) => 
        Encoding.UTF8.GetBytes("Mock Excel Content");

    public object BatchAccept(IEnumerable<int> partIds) => new { failed = new List<object>() };

    public List<BulkUploadResponseDto> BulkUpload(Stream fileStream) => new List<BulkUploadResponseDto>()
    {
        new() { PartId = 1, Error = "error" },
        new() { PartId = 2, Error = "error" }
    };

    public byte[] GetUploadTemplate() => Encoding.UTF8.GetBytes("Mock Template Content");

    public PartDetailResponseDto GetPartDetail(int partId) => new()
    {
        Before = new PartDetailDto { PartNo = "PART-001", Country = "TW", Division = "DIV1", Supplier = "SUP1", PartDesc = "Old Desc", HtsCode = "8471.30", Rate = 0 },
        Modified = new PartDetailDto { PartNo = "PART-001", Country = "TW", Division = "DIV1", Supplier = "SUP1", PartDesc = "New Desc", HtsCode = "8471.30", Rate = 0 }
    };

    public object CreatePart(PartSaveRequest request, string status) => new { partId = 3, partNo = request.PartNo, status };

    public object UpdatePart(int partId, PartSaveRequest request) => new { };

    public object SubmitPart(int partId, PartSaveRequest request) => new { partId, status = "S02" };

    public object AcceptPart(int partId) => new { partId, status = "S04" };

    public object ReturnPart(int partId, string returnReason) => new { partId, status = "S03" };

    public object InactivatePart(int partId) => new { partId, status = "Inactive" };

    public IEnumerable<MilestoneDto> GetMilestones(int partId) => new[]
    {
        new MilestoneDto { Action = "Unknown", UpdatedBy = "Customer001", UpdatedDate = DateTime.Now.AddDays(-5), Remark = "" },
        new MilestoneDto { Action = "Pending Dimerco Review", UpdatedBy = "Customer001", UpdatedDate = DateTime.Now.AddDays(-4), Remark = "" },
        new MilestoneDto { Action = "Pending Customer Review", UpdatedBy = "DCB001", UpdatedDate = DateTime.Now.AddDays(-3), Remark = "test remark" },
        new MilestoneDto { Action = "Pending Dimerco Review", UpdatedBy = "Customer001", UpdatedDate = DateTime.Now.AddDays(-2), Remark = "" },
        new MilestoneDto { Action = "Reviewed", UpdatedBy = "DCB001", UpdatedDate = DateTime.Now.AddDays(-1), Remark = "" }
    };

    public IEnumerable<PartDetailDto> GetHistory(int partId) => new[]
    {
        new PartDetailDto 
        { 
            PartNo = "PART-001", 
            Country = "TW",
            Division = "DIV1",
            Supplier = "SUP1",
            PartDesc = "Version 2 (Current)", 
            HtsCode = "8471.30",
            Rate = 0,
            Remark = "Updated HTS description",
            UpdatedBy = "Customer001",
            UpdatedDate = DateTime.Now.AddDays(-1)
        },
        new PartDetailDto 
        { 
            PartNo = "PART-001", 
            Country = "TW",
            Division = "DIV1",
            Supplier = "SUP1",
            PartDesc = "Version 1 (Initial)", 
            HtsCode = "8471.30",
            Rate = 0,
            Remark = "Initial entry",
            UpdatedBy = "Customer001",
            UpdatedDate = DateTime.Now.AddDays(-5)
        }
    };
}
