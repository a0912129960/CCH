using CCH.Core.Entities;
using CCH.Core.Features.Parts.DTOs;
using CCH.Core.Features.Parts.Interfaces;
using CCH.Core.Interfaces;
using CCH.Core.Interfaces.Repositories;

namespace CCH.Services.Features.Parts;

/// <summary>
/// Implementation of Part Query operations, handling DTO mapping and SLA logic.
/// (繁體中文) 零件查詢操作實作，處理 DTO 映射與 SLA 邏輯。
/// </summary>
public class PartQueryService : IPartQueryService
{
    private readonly IPartRepository _repository;
    private readonly ICommonRepository _commonRepository;
    private readonly IUserContext _userContext;

    /// <summary>
    /// Initializes a new instance of PartQueryService.
    /// (繁體中文) 初始化 PartQueryService 的新執行個體。
    /// </summary>
    public PartQueryService(IPartRepository repository, ICommonRepository commonRepository, IUserContext userContext)
    {
        _repository = repository;
        _commonRepository = commonRepository;
        _userContext = userContext;
    }

    /// <inheritdoc/>
    public PartListResponseDto SearchParts(int? customerId, string? status, string? partNo, int? supplierId, int page, int pageSize)
    {
        var role = _userContext.Role?.ToLower();
        
        // Fetch all matching entities from repository (從倉儲取得所有相符實體)
        var entities = _repository.SearchParts(customerId, status, partNo, supplierId);

        var total = entities.Count();
        
        // Pagination and Mapping (分頁與映射)
        var data = entities
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => MapToListItemDto(e, role))
            .ToList();

        return new PartListResponseDto
        {
            Total = total,
            Page = page,
            Data = data
        };
    }

    /// <inheritdoc/>
    public PartDetailResponseDto? GetPartDetail(int partId)
    {
        var entity = _repository.GetPartById(partId);
        if (entity == null) return null;

        var listItem = MapToListItemDto(entity);

        // INTERNAL-AI-20260420: Include SlaStatus in detail response so frontend can apply SLA badge color.
        // (INTERNAL-AI-20260420: 詳細回應中加入 SlaStatus，供前端套用 SLA 標籤顏色。)
        return new PartDetailResponseDto
        {
            Status = entity.Status,
            SlaStatus = listItem.SlaStatus,
            Before = new PartDetailDto
            { 
                PartNo = listItem.PartNo, 
                Country = listItem.Country, 
                Division = entity.Division, 
                Supplier = listItem.Supplier, 
                PartDesc = listItem.PartDesc, 
                HtsCode = listItem.HtsCode, 
                Rate = listItem.Rate, 
                Remark = entity.Remark, 
                UpdatedBy = listItem.UpdatedBy, 
                UpdatedDate = listItem.UpdatedDate 
            },
            Modified = new PartDetailDto 
            { 
                PartNo = listItem.PartNo, 
                Country = listItem.Country, 
                Division = entity.Division, 
                Supplier = listItem.Supplier, 
                PartDesc = listItem.PartDesc, 
                HtsCode = listItem.HtsCode, 
                Rate = listItem.Rate, 
                Remark = entity.Remark, 
                UpdatedBy = listItem.UpdatedBy, 
                UpdatedDate = listItem.UpdatedDate 
            }
        };
    }

    private PartListItemDto MapToListItemDto(PartEntity entity, string? role = null)
    {
        var customerName = _commonRepository.GetCustomers().FirstOrDefault(c => c.ID == entity.CustomerID)?.Name ?? "Unknown";
        var countryName = _commonRepository.GetCountries().FirstOrDefault(c => c.ID == entity.CountryID)?.Name ?? "Unknown";
        var supplierName = _commonRepository.GetSuppliers().FirstOrDefault(s => s.ID == entity.SupplierID)?.Name ?? "Unknown";

        // SLA Calculation Logic (SLA 計算邏輯)
        string slaStatus = "green";
        var hoursElapsed = (DateTime.Now - entity.UpdatedDate).TotalHours;

        if (role == "customer")
        {
            if (hoursElapsed > 72) slaStatus = "red";
            else if (hoursElapsed > 48) slaStatus = "orange";
            else if (hoursElapsed > 36) slaStatus = "yellow";
        }
        else 
        {
            if (hoursElapsed > 48) slaStatus = "red";
            else if (hoursElapsed > 36) slaStatus = "orange";
            else if (hoursElapsed > 24) slaStatus = "yellow";
        }

        return new PartListItemDto
        {
            Id = entity.ID,
            Customer = customerName,
            PartNo = entity.PartNo,
            PartDesc = entity.PartDescription,
            Country = countryName,
            Supplier = supplierName,
            HtsCode = entity.HTSCode,
            Rate = entity.DutyRate,
            Status = entity.Status,
            UpdatedBy = entity.UpdatedBy,
            UpdatedDate = entity.UpdatedDate,
            SlaStatus = slaStatus,
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

    // INTERNAL-AI-20260420: GetMilestones now reads real history from the repository instead of hardcoded data.
    // (INTERNAL-AI-20260420: GetMilestones 改為從倉儲讀取真實歷程，不再使用硬編碼資料。)
    /* public IEnumerable<MilestoneDto> GetMilestones(int partId) => new[]
    {
        new MilestoneDto { Action = "Unknown", ... },
        ...
    }; */
    /// <inheritdoc/>
    public IEnumerable<MilestoneDto> GetMilestones(int partId) =>
        _repository.GetHistoryByPartId(partId)
            .Select(h => new MilestoneDto
            {
                Action = h.Action,
                UpdatedBy = h.UpdatedBy,
                UpdatedDate = h.UpdatedDate,
                Remark = h.Remark
            })
            .ToList();

    /// <inheritdoc/>
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
