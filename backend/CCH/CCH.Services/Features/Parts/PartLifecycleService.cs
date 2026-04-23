using CCH.Core.Entities.CSP;
using CCH.Core.Features.Parts.DTOs;
using CCH.Core.Features.Parts.Interfaces;
using CCH.Core.Interfaces;
using CCH.Core.Interfaces.Repositories;

namespace CCH.Services.Features.Parts;

/// <summary>
/// Implementation of Part lifecycle operations (Update, Status Change, History).
/// (繁體中文) 零件生命週期操作實作（更新、狀態變更、歷程）。
/// </summary>
public class PartLifecycleService : IPartLifecycleService
{
    private readonly IPartRepository _repository;
    private readonly ICommonRepository _commonRepository;
    private readonly IUserContext _userContext;

    public PartLifecycleService(IPartRepository repository, ICommonRepository commonRepository, IUserContext userContext)
    {
        _repository = repository;
        _commonRepository = commonRepository;
        _userContext = userContext;
    }

    // Use UserId (short login ID) for DB fields that have MaxLength(10). (使用簡短的登入 ID 寫入有長度限制的 DB 欄位。)
    private string CurrentUser => (_userContext.UserId ?? "system")[..Math.Min((_userContext.UserId ?? "system").Length, 10)];

    /// <inheritdoc/>
    public object CreatePart(PartCreateRequest request, string status)
    {
        var entity = new CchParts
        {
            ProjectID = request.ProjectId ?? 101,
            PartNo = request.PartNo,
            CountryID = request.CountryId ?? 1,
            PartDescription = request.PartDesc,
            Division = request.Division ?? string.Empty,
            SupplierID = !string.IsNullOrEmpty(request.Supplier) ? ResolveSupplierIdByName(request.Supplier) : 1,
            HTSCode = request.HtsCode,
            DutyRate = request.Rate ?? 0m,
            AddHTSCode1 = request.HtsCode1,
            AddDutyRate1 = request.Rate1,
            AddHTSCode2 = request.HtsCode2,
            AddDutyRate2 = request.Rate2,
            AddHTSCode3 = request.HtsCode3,
            AddDutyRate3 = request.Rate3,
            AddHTSCode4 = request.HtsCode4,
            AddDutyRate4 = request.Rate4,
            Remark = request.Remark ?? string.Empty,
            Status = status,
            CreatedBy = CurrentUser,
            UpdatedBy = CurrentUser
        };

        var id = _repository.CreatePart(entity);
        // INTERNAL-AI-20260420: Record creation in history + take initial snapshot. (記錄建立歷程並拍攝初始快照。)
        RecordHistory(id, "Created", null, status);
        RecordSnapshot(entity);
        return new { partId = id, partNo = request.PartNo, status };
    }

    /// <inheritdoc/>
    public object UpdatePart(int partId, PartSaveRequest request)
    {
        var existing = GetExistingPart(partId);
        var oldStatus = existing.Status;
        MapRequestToEntity(request, existing);
        _repository.UpdatePart(existing);
        
        RecordHistory(partId, "Modified", oldStatus, existing.Status);
        RecordSnapshot(existing); // Restore Snapshot call (還原快照呼叫)
        
        return new { partId };
    }

    /// <inheritdoc/>
    public object SubmitPart(int partId, PartSaveRequest request)
    {
        var existing = GetExistingPart(partId);
        var oldStatus = existing.Status;
        MapRequestToEntity(request, existing);
        existing.Status = "S02";
        _repository.UpdatePart(existing);
        _repository.UpdateStatus(partId, "S02", CurrentUser);
        
        RecordHistory(partId, "Submitted", oldStatus, "S02");
        RecordSnapshot(existing); // Restore Snapshot call (還原快照呼叫)
        
        return new { partId, status = "S02" };
    }

    /// <inheritdoc/>
    public object AcceptPart(int partId)
    {
        var existing = _repository.GetPartById(partId);
        var oldStatus = existing?.Status;
        _repository.UpdateStatus(partId, "S04", CurrentUser);
        RecordHistory(partId, "Accepted", oldStatus, "S04");
        // No snapshot for simple status change (單純狀態變更不需紀錄欄位快照，除非業務需求)
        return new { partId, status = "S04" };
    }

    /// <inheritdoc/>
    public object ReturnPart(int partId, string returnReason)
    {
        var existing = _repository.GetPartById(partId);
        var oldStatus = existing?.Status;
        _repository.UpdateStatus(partId, "S03", CurrentUser);
        RecordHistory(partId, "Returned", oldStatus, "S03", returnReason);
        return new { partId, status = "S03" };
    }

    /// <inheritdoc/>
    public object InactivatePart(int partId)
    {
        var existing = _repository.GetPartById(partId);
        var oldStatus = existing?.Status;
        _repository.UpdateStatus(partId, "Inactive", CurrentUser);
        RecordHistory(partId, "Inactivated", oldStatus, "Inactive");
        return new { partId, status = "Inactive" };
    }

    // INTERNAL-AI-20260421: S04 → S03: save additional duty fields then set status to Pending Customer Review.
    // (INTERNAL-AI-20260421: S04 → S03：儲存附加關稅欄位後將狀態設為 Pending Customer Review。)
    /// <inheritdoc/>
    public object SendToCustomerReview(int partId, PartSaveRequest request)
    {
        var existing = GetExistingPart(partId);
        var oldStatus = existing.Status;
        MapRequestToEntity(request, existing);
        existing.Status = "S03";
        _repository.UpdatePart(existing);
        _repository.UpdateStatus(partId, "S03", CurrentUser);
        RecordHistory(partId, "Sent to Cust Review", oldStatus, "S03");
        RecordSnapshot(existing);
        return new { partId, status = "S03" };
    }

    /// <inheritdoc/>
    public object BatchAccept(IEnumerable<int> partIds)
    {
        var failed = new List<object>();
        var validIds = new List<int>();
        var user = CurrentUser;
        var now = DateTime.Now;

        foreach (var id in partIds)
        {
            var entity = _repository.GetPartById(id);
            if (entity == null) failed.Add(new { partId = id, errorMessage = "Part not found." });
            else if (entity.Status != "S02") failed.Add(new { partId = id, errorMessage = "Invalid status." });
            else validIds.Add(id);
        }

        if (validIds.Any())
        {
            _repository.BatchUpdateStatus(validIds, "S04", user);
            _repository.AddHistoryBatch(validIds.Select(id => new CchPartMilestones
            {
                PartID = id, 
                Action = "Accepted (Batch)", 
                FromStatus = "S02",
                ToStatus = "S04",
                CreatedBy = user, 
                CreatedDate = now, 
                Remark = ""
            }));
        }
        return new { failed };
    }

    private CchParts GetExistingPart(int id)
    {
        var existing = _repository.GetPartById(id);
        if (existing == null) throw new Exception("Part not found.");
        return existing;
    }

    private void RecordHistory(int partId, string action, string? fromStatus, string? toStatus, string remark = "")
    {
        var history = new CchPartMilestones
        {
            PartID = partId, 
            Action = action, 
            FromStatus = fromStatus,
            ToStatus = toStatus,
            Remark = remark,
            CreatedBy = CurrentUser, 
            CreatedDate = DateTime.Now
        };
        _repository.AddHistory(history);
    }

    /// <summary>
    /// Takes a full data snapshot of the part for field-level history tracking.
    /// (繁體中文) 為零件拍攝完整資料快照，用於欄位級別的歷程追蹤。
    /// </summary>
    private void RecordSnapshot(CchParts entity)
    {
        var countryName = _commonRepository.GetCountries().FirstOrDefault(c => c.ID == entity.CountryID)?.Name ?? "Unknown";
        var supplierName = _commonRepository.GetSuppliers().FirstOrDefault(s => s.ID == entity.SupplierID)?.SupplierName ?? "Unknown";

        var snapshot = new CchPartHistories
        {
            PartID = entity.ID,
            PartNo = entity.PartNo,
            Country = countryName,
            Division = entity.Division,
            Supplier = supplierName,
            PartDesc = entity.PartDescription,
            HtsCode = entity.HTSCode,
            Rate = entity.DutyRate,
            HtsCode1 = entity.AddHTSCode1,
            Rate1 = entity.AddDutyRate1,
            HtsCode2 = entity.AddHTSCode2,
            Rate2 = entity.AddDutyRate2,
            HtsCode3 = entity.AddHTSCode3,
            Rate3 = entity.AddDutyRate3,
            HtsCode4 = entity.AddHTSCode4,
            Rate4 = entity.AddDutyRate4,
            Remark = entity.Remark,
            CreatedBy = CurrentUser,
            CreatedDate = DateTime.Now
        };
        _repository.AddSnapshot(snapshot);
    }

    private void MapRequestToEntity(PartSaveRequest req, CchParts e)
    {
        e.PartNo = req.PartNo; e.CountryID = req.CountryId ?? 0; e.Division = req.Division;
        e.PartDescription = req.PartDesc; e.HTSCode = req.HtsCode;
        e.DutyRate = req.Rate; e.AddHTSCode1 = req.HtsCode1; e.AddDutyRate1 = req.Rate1;
        e.AddHTSCode2 = req.HtsCode2; e.AddDutyRate2 = req.Rate2; e.AddHTSCode3 = req.HtsCode3;
        e.AddDutyRate3 = req.Rate3; e.AddHTSCode4 = req.HtsCode4; e.AddDutyRate4 = req.Rate4;
        e.Remark = req.Remark; e.UpdatedBy = CurrentUser;

        // Map Supplier Name string to SupplierID FK (SSoT from Common Repository)
        var supplier = _commonRepository.GetSuppliers().FirstOrDefault(s => s.SupplierName == req.Supplier);
        e.SupplierID = supplier?.ID ?? 0;
    }

    private int ResolveSupplierIdByName(string supplierName) =>
        _commonRepository.GetSuppliers().FirstOrDefault(s => s.SupplierName == supplierName)?.ID ?? 1;
}
