using CCH.Core.DTOs;
using CCH.Core.Interfaces;
using CCH.Core.Interfaces.Repositories;
using System.Text;

namespace CCH.Services.Services;

/// <summary>
/// Part service implementation.
/// (繁體中文) 零件服務實作。
/// </summary>
public class PartService : IPartQueryService, IPartLifecycleService, IPartExcelService
{
    private readonly IPartRepository _repository;
    private readonly IUserContext _userContext;

    public PartService(IPartRepository repository, IUserContext userContext)
    {
        _repository = repository;
        _userContext = userContext;
    }

    public PartListResponseDto SearchParts(int? customerId, string? status, string? partNo, int? supplierId, int page, int pageSize)
    {
        var filtered = _repository.SearchParts(customerId, status, partNo, supplierId);
        var total = filtered.Count();
        var data = filtered.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return new PartListResponseDto
        {
            Total = total,
            Page = page,
            Data = data
        };
    }

    public byte[] ExportParts(string? customerId, string? status, string? partNo, string? supplier) => 
        Encoding.UTF8.GetBytes("Mock Excel Content");

    public object BatchAccept(IEnumerable<int> partIds) => new { failed = new List<object>() };

    public List<BulkUploadResponseDto> BulkUpload(Stream fileStream) => new List<BulkUploadResponseDto>()
    {
        new() { PartId = 1, Error = "error" },
        new() { PartId = 2, Error = "error" }
    };

    public byte[] GetUploadTemplate() => Encoding.UTF8.GetBytes("Mock Template Content");

    // INTERNAL-AI-20260416: Use IPartRepository for GetPartDetail.
    // (INTERNAL-AI-20260416: GetPartDetail 改用 IPartRepository。)
    public PartDetailResponseDto? GetPartDetail(int partId) => _repository.GetPartDetail(partId);

    public object CreatePart(PartSaveRequest request, string status)
    {
        var partId = _repository.CreatePart(request, status);
        return new { partId, partNo = request.PartNo, status };
    }

    public object UpdatePart(int partId, PartSaveRequest request)
    {
        _repository.UpdatePart(partId, request);
        return new { };
    }

    public object SubmitPart(int partId, PartSaveRequest request)
    {
        _repository.UpdatePart(partId, request);
        _repository.UpdateStatus(partId, "S02");
        return new { partId, status = "S02" };
    }

    public object AcceptPart(int partId)
    {
        _repository.UpdateStatus(partId, "S04");
        return new { partId, status = "S04" };
    }

    public object ReturnPart(int partId, string returnReason)
    {
        _repository.UpdateStatus(partId, "S03");
        return new { partId, status = "S03" };
    }

    public object InactivatePart(int partId)
    {
        _repository.UpdateStatus(partId, "Inactive");
        return new { partId, status = "Inactive" };
    }

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
