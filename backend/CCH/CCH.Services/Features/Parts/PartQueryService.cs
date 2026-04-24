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
        var entities = _repository.SearchParts(customerId, status, partNo, supplierId).ToList();

        var total = entities.Count;
        
        // Batch resolve UserNames (批次解析使用者名稱)
        var userIds = entities.Select(e => e.UpdatedBy).Where(id => !string.IsNullOrEmpty(id)).Distinct()!;
        var userNames = _commonRepository.GetUserNames(userIds!);

        // Pagination and Mapping (分頁與映射)
        var data = entities
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => MapToListItemDto(e, role, userNames))
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
        listItem.UpdatedBy = _commonRepository.GetUserName(entity.UpdatedBy ?? "");

        // INTERNAL-AI-20260421: Before = second-most-recent snapshot (state before last save).
        // (INTERNAL-AI-20260421: Before 改為倒數第二筆快照（上次存檔前的狀態）。)
        var snapshots = _repository.GetSnapshotsByPartId(partId)
            .OrderByDescending(s => s.CreatedDate)
            .ToList();

        var beforeSnapshot = snapshots.Count > 1 ? snapshots[1] : null;

        return new PartDetailResponseDto
        {
            Status = entity.Status,
            SlaStatus = listItem.SlaStatus,
            Before = beforeSnapshot != null ? new PartDetailDto
            {
                PartNo    = beforeSnapshot.PartNo ?? "",
                Country   = beforeSnapshot.Country ?? "",
                Division  = beforeSnapshot.Division ?? "",
                Supplier  = beforeSnapshot.Supplier ?? "",
                PartDesc  = beforeSnapshot.PartDesc ?? "",
                HtsCode   = beforeSnapshot.HtsCode ?? "",
                Rate      = beforeSnapshot.Rate ?? 0,
                HtsCode1  = beforeSnapshot.HtsCode1,
                Rate1     = beforeSnapshot.Rate1,
                HtsCode2  = beforeSnapshot.HtsCode2,
                Rate2     = beforeSnapshot.Rate2,
                HtsCode3  = beforeSnapshot.HtsCode3,
                Rate3     = beforeSnapshot.Rate3,
                HtsCode4  = beforeSnapshot.HtsCode4,
                Rate4     = beforeSnapshot.Rate4,
                Remark    = beforeSnapshot.Remark ?? "",
                UpdatedBy   = _commonRepository.GetUserName(beforeSnapshot.CreatedBy ?? ""),
                UpdatedDate = beforeSnapshot.CreatedDate ?? DateTime.MinValue
            } : new PartDetailDto(),
            Modified = new PartDetailDto
            {
                PartNo    = listItem.PartNo,
                CountryId = entity.CountryID,
                Country   = listItem.Country,
                Division  = entity.Division ?? "",
                Supplier  = listItem.Supplier,
                PartDesc  = listItem.PartDesc,
                HtsCode   = listItem.HtsCode,
                Rate      = listItem.Rate,
                HtsCode1  = listItem.HtsCode1,
                Rate1     = listItem.Rate1,
                HtsCode2  = listItem.HtsCode2,
                Rate2     = listItem.Rate2,
                HtsCode3  = listItem.HtsCode3,
                Rate3     = listItem.Rate3,
                HtsCode4  = listItem.HtsCode4,
                Rate4     = listItem.Rate4,
                Remark    = entity.Remark ?? "",
                UpdatedBy   = listItem.UpdatedBy,
                UpdatedDate = listItem.UpdatedDate
            }
        };
    }

    private PartListItemDto MapToListItemDto(CchParts entity, string? role = null, Dictionary<string, string>? userNames = null)
    {
        var customerName = _commonRepository.GetCustomers().FirstOrDefault(c => c.HQID == entity.CustomerID)?.CustomerName ?? "Unknown";
        var countryName = _commonRepository.GetCountries().FirstOrDefault(c => c.ID == entity.CountryID)?.Name ?? "Unknown";
        var supplierName = _commonRepository.GetSuppliers().FirstOrDefault(s => s.ID == entity.SupplierID)?.SupplierName ?? "Unknown";

        // Resolve UpdatedBy name (解析 UpdatedBy 名稱)
        string updatedByName = entity.UpdatedBy ?? "";
        if (userNames != null && userNames.TryGetValue(updatedByName, out var name)) updatedByName = name;
        else if (!string.IsNullOrEmpty(updatedByName)) updatedByName = _commonRepository.GetUserName(updatedByName);

        // SLA Calculation Logic (SLA 計算邏輯)
        // Customer role → S01 (draft) + S03 (returned): apply customer thresholds
        // Employee role → S02 (pending review): apply employee thresholds
        // All other combinations → no SLA indicator
        string slaStatus = "";
        var hoursElapsed = (DateTime.Now - (entity.UpdatedDate ?? DateTime.MinValue)).TotalHours;

        if (role == "customer" && (entity.Status == "S01" || entity.Status == "S03"))
        {
            if (hoursElapsed > 72) slaStatus = "red";
            else if (hoursElapsed > 48) slaStatus = "orange";
            else if (hoursElapsed > 36) slaStatus = "yellow";
            else slaStatus = "green";
        }
        else if (role != "customer" && entity.Status == "S02")
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
            UpdatedBy = updatedByName,
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

    /// <inheritdoc/>
    public IEnumerable<MilestoneDto> GetMilestones(int partId) =>
        _repository.GetHistoryByPartId(partId)
            .OrderByDescending(h => h.CreatedDate)
            .Select(h => new MilestoneDto
            {
                Action = h.Action,
                UpdatedBy = _commonRepository.GetUserName(h.CreatedBy ?? ""),
                UpdatedDate = h.CreatedDate ?? DateTime.MinValue,
                Remark = h.Remark
            })
            .ToList();

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
                UpdatedBy = _commonRepository.GetUserName(s.CreatedBy ?? ""),
                UpdatedDate = s.CreatedDate ?? DateTime.MinValue
            })
            .ToList();
//}

    /// <inheritdoc/>
    public bool CheckDuplicate(int customerId, string partNo, int countryId) =>
        _repository.ExistsByPartNoAndCountry(customerId, partNo, countryId);
}
