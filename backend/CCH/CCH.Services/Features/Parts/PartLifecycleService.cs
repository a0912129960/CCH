using CCH.Core.Entities;
using CCH.Core.Entities.CSP;
using CCH.Core.Entities.ReSm;
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

    /// <inheritdoc/>
    public object CreatePart(PartSaveRequest request, string status)
    {
        var entity = new PartEntity { Status = status, CreatedBy = _userContext.UserName ?? "system" };
        MapRequestToEntity(request, entity);
        var id = _repository.CreatePart(entity);
        RecordHistory(id, "Created");
        return new { partId = id, status };
    }

    /// <inheritdoc/>
    public object UpdatePart(int partId, PartSaveRequest request)
    {
        var existing = GetExistingPart(partId);
        MapRequestToEntity(request, existing);
        _repository.UpdatePart(existing);
        RecordHistory(partId, "Modified");
        return new { partId };
    }

    /// <inheritdoc/>
    public object SubmitPart(int partId, PartSaveRequest request)
    {
        var existing = GetExistingPart(partId);
        MapRequestToEntity(request, existing);
        existing.Status = "S02";
        _repository.UpdatePart(existing);
        _repository.UpdateStatus(partId, "S02");
        RecordHistory(partId, "Submitted");
        return new { partId, status = "S02" };
    }

    /// <inheritdoc/>
    public object AcceptPart(int partId)
    {
        _repository.UpdateStatus(partId, "S04");
        RecordHistory(partId, "Accepted");
        return new { partId, status = "S04" };
    }

    /// <inheritdoc/>
    public object ReturnPart(int partId, string returnReason)
    {
        _repository.UpdateStatus(partId, "S03");
        RecordHistory(partId, $"Returned: {returnReason}");
        return new { partId, status = "S03" };
    }

    /// <inheritdoc/>
    public object InactivatePart(int partId)
    {
        _repository.UpdateStatus(partId, "Inactive");
        RecordHistory(partId, "Inactivated");
        return new { partId, status = "Inactive" };
    }

    /// <inheritdoc/>
    public object BatchAccept(IEnumerable<int> partIds)
    {
        var failed = new List<object>();
        var validIds = new List<int>();
        var user = _userContext.UserId ?? "system";
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
            _repository.AddHistoryBatch(validIds.Select(id => new PartHistoryEntity
            {
                PartID = id, Action = "Accepted (Batch)", UpdatedBy = user, UpdatedDate = now, Remark = ""
            }));
        }
        return new { failed };
    }

    private PartEntity GetExistingPart(int id)
    {
        var existing = _repository.GetPartById(id);
        if (existing == null) throw new Exception("Part not found.");
        return existing;
    }

    private void RecordHistory(int partId, string action)
    {
        var history = new PartHistoryEntity
        {
            PartID = partId, Action = action, Remark = "",
            UpdatedBy = _userContext.UserName ?? "system", UpdatedDate = DateTime.Now
        };
        _repository.AddHistory(history);
    }

    private void MapRequestToEntity(PartSaveRequest req, PartEntity e)
    {
        e.PartNo = req.PartNo; e.CountryID = req.CountryId ?? 0; e.Division = req.Division;
        e.PartDescription = req.PartDesc; e.HTSCode = req.HtsCode;
        e.DutyRate = req.Rate; e.AddHTSCode1 = req.HtsCode1; e.AddDutyRate1 = req.Rate1;
        e.AddHTSCode2 = req.HtsCode2; e.AddDutyRate2 = req.Rate2; e.AddHTSCode3 = req.HtsCode3;
        e.AddDutyRate3 = req.Rate3; e.AddHTSCode4 = req.HtsCode4; e.AddDutyRate4 = req.Rate4;
        e.Remark = req.Remark; e.UpdatedBy = _userContext.UserName ?? "system";

        // Map Supplier Name string to SupplierID FK (SSoT from Common Repository)
        var supplier = _commonRepository.GetSuppliers().FirstOrDefault(s => s.SupplierName == req.Supplier);
        e.SupplierID = supplier?.ID ?? 0;
    }
}
