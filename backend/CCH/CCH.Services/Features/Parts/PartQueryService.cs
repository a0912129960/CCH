using CCH.Core.Features.Parts.DTOs;
using CCH.Core.Features.Parts.Interfaces;
using CCH.Core.Interfaces;
using CCH.Core.Interfaces.Repositories;

namespace CCH.Services.Features.Parts;

/// <summary>
/// Implementation of Part Query operations.
/// (繁體中文) 零件查詢操作實作。
/// </summary>
public class PartQueryService : IPartQueryService
{
    private readonly IPartRepository _repository;
    private readonly IUserContext _userContext;

    public PartQueryService(IPartRepository repository, IUserContext userContext)
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

    public PartDetailResponseDto? GetPartDetail(int partId) => _repository.GetPartDetail(partId);

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
