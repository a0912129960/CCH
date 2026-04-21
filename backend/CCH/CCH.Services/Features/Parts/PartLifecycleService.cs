using CCH.Core.Entities;
using CCH.Core.Features.Parts.DTOs;
using CCH.Core.Features.Parts.Interfaces;
using CCH.Core.Interfaces;
using CCH.Core.Interfaces.Repositories;
// INTERNAL-AI-20260420: Added ICommonRepository to resolve Supplier name → ID for entity persistence.
// INTERNAL-AI-20260420: Added IUserContext to record the acting user in history entries.

namespace CCH.Services.Features.Parts;

/// <summary>
/// Implementation of Part Lifecycle operations (Create, Update, Accept, Return).
/// (繁體中文) 零件生命週期操作實作（建立、修改、接受、退回）。
/// </summary>
public class PartLifecycleService : IPartLifecycleService
{
    private readonly IPartRepository _repository;
    // INTERNAL-AI-20260420: Injected ICommonRepository to resolve Supplier name → SupplierID for entity FK.
    // (INTERNAL-AI-20260420: 注入 ICommonRepository 以從供應商名稱查找對應的 SupplierID 外鍵。)
    private readonly ICommonRepository _commonRepository;
    // INTERNAL-AI-20260420: Injected IUserContext to record the acting user in history entries.
    // (INTERNAL-AI-20260420: 注入 IUserContext 以在歷程記錄中紀錄操作使用者。)
    private readonly IUserContext _userContext;

    /// <summary>
    /// Initializes a new instance of PartLifecycleService.
    /// (繁體中文) 初始化 PartLifecycleService 的新執行個體。
    /// </summary>
    public PartLifecycleService(IPartRepository repository, ICommonRepository commonRepository, IUserContext userContext)
    {
        _repository = repository;
        _commonRepository = commonRepository;
        _userContext = userContext;
    }

    private string CurrentUser => _userContext.UserName ?? _userContext.UserId ?? "System";

    /// <summary>
    /// Appends a milestone event (status/action label) for a part.
    /// (繁體中文) 為零件附加一筆里程碑事件（狀態/動作標籤）。
    /// </summary>
    private void RecordHistory(int partId, string action, string remark = "")
    {
        _repository.AddHistory(new PartHistoryEntity
        {
            PartID = partId,
            Action = action,
            UpdatedBy = CurrentUser,
            UpdatedDate = DateTime.Now,
            Remark = remark
        });
    }

    // INTERNAL-AI-20260420: Take a full data snapshot of a part after every save operation.
    // (INTERNAL-AI-20260420: 每次儲存操作後為零件拍攝完整資料快照。)
    private void RecordSnapshot(int partId, PartEntity entity)
    {
        var countryName = _commonRepository.GetCountries().FirstOrDefault(c => c.ID == entity.CountryID)?.Name ?? string.Empty;
        var supplierName = _commonRepository.GetSuppliers().FirstOrDefault(s => s.ID == entity.SupplierID)?.Name ?? string.Empty;

        _repository.AddSnapshot(new PartSnapshotEntity
        {
            PartID = partId,
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
            UpdatedBy = entity.UpdatedBy,
            UpdatedDate = DateTime.Now
        });
    }

    /// <summary>
    /// Resolves a supplier name to its ID, defaulting to 1 if not found.
    /// (繁體中文) 將供應商名稱轉換為 ID，若找不到則預設為 1。
    /// </summary>
    private int ResolveSupplierIdByName(string supplierName) =>
        _commonRepository.GetSuppliers().FirstOrDefault(s => s.Name == supplierName)?.ID ?? 1;

    /// <inheritdoc/>
    public object CreatePart(PartCreateRequest request, string status)
    {
        var entity = new PartEntity
        {
            CustomerID = request.CustomerId ?? 101,
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

        var partId = _repository.CreatePart(entity);
        // INTERNAL-AI-20260420: Record creation in history + take initial snapshot. (記錄建立歷程並拍攝初始快照。)
        RecordHistory(partId, "Created");
        RecordSnapshot(partId, entity);
        return new { partId, partNo = request.PartNo, status };
    }

    /// <inheritdoc/>
    public object UpdatePart(int partId, PartSaveRequest request)
    {
        var existing = _repository.GetPartById(partId);
        if (existing != null)
        {
            existing.PartNo = request.PartNo;
            existing.PartDescription = request.PartDesc;
            existing.Division = request.Division;
            existing.SupplierID = !string.IsNullOrEmpty(request.Supplier)
                ? ResolveSupplierIdByName(request.Supplier)
                : existing.SupplierID;
            existing.HTSCode = request.HtsCode;
            existing.DutyRate = request.Rate;
            existing.AddHTSCode1 = request.HtsCode1;
            existing.AddDutyRate1 = request.Rate1;
            existing.Remark = request.Remark;
            existing.UpdatedBy = _userContext.UserName ?? _userContext.UserId ?? "System";

            _repository.UpdatePart(existing);
            // INTERNAL-AI-20260420: Record save in history + take data snapshot. (記錄儲存歷程並拍攝資料快照。)
            RecordHistory(partId, "Updated");
            RecordSnapshot(partId, existing);
        }
        return new { };
    }

    /// <inheritdoc/>
    public object SubmitPart(int partId, PartSaveRequest request)
    {
        UpdatePart(partId, request);
        _repository.UpdateStatus(partId, "S02");
        // INTERNAL-AI-20260420: Record submit in history. (記錄送審歷程。)
        RecordHistory(partId, "Submitted to Dimerco");
        return new { partId, status = "S02" };
    }

    /// <inheritdoc/>
    public object AcceptPart(int partId)
    {
        _repository.UpdateStatus(partId, "S04");
        // INTERNAL-AI-20260420: Record accept in history. (記錄接受歷程。)
        RecordHistory(partId, "Accepted");
        return new { partId, status = "S04" };
    }

    /// <inheritdoc/>
    public object ReturnPart(int partId, string returnReason)
    {
        _repository.UpdateStatus(partId, "S03");
        // INTERNAL-AI-20260420: Record return in history, storing the reason as remark. (記錄退回歷程，退回原因存入備註。)
        RecordHistory(partId, "Returned to Customer", returnReason);
        return new { partId, status = "S03" };
    }

    /// <inheritdoc/>
    public object InactivatePart(int partId)
    {
        _repository.UpdateStatus(partId, "Inactive");
        // INTERNAL-AI-20260420: Record inactivation in history. (記錄停用歷程。)
        RecordHistory(partId, "Inactivated");
        return new { partId, status = "Inactive" };
    }

    /// <inheritdoc/>
    public object BatchAccept(IEnumerable<int> partIds)
    {
        var failed = new List<object>();
        foreach (var id in partIds)
        {
            try
            {
                var entity = _repository.GetPartById(id);
                if (entity == null)
                {
                    failed.Add(new { partId = id, errorMessage = "Part not found. / 找不到零件。" });
                    continue;
                }

                if (entity.Status != "S02" && entity.Status != "S03")
                {
                    failed.Add(new { partId = id, errorMessage = $"Invalid status: {entity.Status}. Expected S02 or S03. / 錯誤的狀態：{entity.Status}。預期為 S02 或 S03。" });
                    continue;
                }

                _repository.UpdateStatus(id, "S04");
                RecordHistory(id, "Accepted (Batch)");
            }
            catch (Exception ex)
            {
                failed.Add(new { partId = id, errorMessage = ex.Message });
            }
        }
        return new { failed };
    }
}
