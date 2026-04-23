using CCH.Core.Entities.CSP;
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

    private PartListItemDto MapToListItemDto(CchParts entity, string? role = null)
    {
        var customerName = _commonRepository.GetCustomers().FirstOrDefault(c => c.HQID == entity.CustomerID)?.CustomerName ?? "Unknown";
        var countryName = _commonRepository.GetCountries().FirstOrDefault(c => c.ID == entity.CountryID)?.Name ?? "Unknown";
        var supplierName = _commonRepository.GetSuppliers().FirstOrDefault(s => s.ID == entity.SupplierID)?.SupplierName ?? "Unknown";

        // SLA Calculation Logic (SLA 計算邏輯)
        string slaStatus = "";
        var hoursElapsed = (DateTime.Now - (entity.UpdatedDate ?? DateTime.MinValue)).TotalHours;

        if (role == "customer" && (entity.Status == "S02" || entity.Status == "S03"))
        {
            if (hoursElapsed > 72) slaStatus = "red";
            else if (hoursElapsed > 48) slaStatus = "orange";
            else if (hoursElapsed > 36) slaStatus = "yellow";
            else slaStatus = "green";
        }
        else if(entity.Status == "S02")
        {
            if (hoursElapsed > 48) slaStatus = "red";
            else if (hoursElapsed > 36) slaStatus = "orange";
            else if (hoursElapsed > 24) slaStatus = "yellow";
            else slaStatus = "green";
        }

        return new PartListItemDto
        {
            Id = entity.ID,
            Customer = customerName,
            PartNo = entity.PartNo ?? "",
            PartDesc = entity.PartDescription ?? "",
            Country = countryName,
            Supplier = supplierName,
            HtsCode = entity.HTSCode ?? "",
            Rate = entity.DutyRate ?? 0,
            Status = entity.Status ?? "",
            UpdatedBy = entity.UpdatedBy ?? "",
            UpdatedDate = entity.UpdatedDate ?? DateTime.MinValue,
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
    // INTERNAL-AI-20260420: Milestones sorted DESC per spec (依修改時間由近到遠排序).
    // (INTERNAL-AI-20260420: 依規格將里程碑排序改為由近到遠 DESC。)
    /// <inheritdoc/>
    public IEnumerable<MilestoneDto> GetMilestones(int partId) =>
        _repository.GetHistoryByPartId(partId)
            .OrderByDescending(h => h.CreatedDate)
            .Select(h => new MilestoneDto
            {
                Action = h.Action,
                UpdatedBy = h.CreatedBy,
                UpdatedDate = h.CreatedDate ?? DateTime.MinValue,
                Remark = h.Remark
            })
            .ToList();

    // INTERNAL-AI-20260420: GetHistory now reads real snapshots instead of hardcoded mock data.
    // Sorted DESC per spec (依修改時間由近到遠排序). (改為從倉儲讀取真實快照，並依規格排序。)
    /* public IEnumerable<PartDetailDto> GetHistory(int partId) => new[] { ... }; */
    /// <inheritdoc/>
    public IEnumerable<PartDetailDto> GetHistory(int partId) =>
        _repository.GetSnapshotsByPartId(partId)
            .OrderByDescending(s => s.CreatedDate)
            .Select(s => new PartDetailDto
            {
                PartNo = s.PartNo ?? "",
                Country = s.Country ?? "",
                Division = s.Division ?? "",
                Supplier = s.Supplier ?? "",
                PartDesc = s.PartDesc ?? "",
                HtsCode = s.HtsCode ?? "",
                Rate = s.Rate ?? 0,
                HtsCode1 = s.HtsCode1,
                Rate1 = s.Rate1,
                HtsCode2 = s.HtsCode2,
                Rate2 = s.Rate2,
                HtsCode3 = s.HtsCode3,
                Rate3 = s.Rate3,
                HtsCode4 = s.HtsCode4,
                Rate4 = s.Rate4,
                Remark = s.Remark ?? "",
                UpdatedBy = s.CreatedBy ?? "",
                UpdatedDate = s.CreatedDate ?? DateTime.MinValue
            })
            .ToList();
}
