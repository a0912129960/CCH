using CCH.Core.Entities;
using CCH.Core.Features.Parts.DTOs;
using CCH.Core.Features.Parts.Interfaces;
using CCH.Core.Interfaces.Repositories;
// INTERNAL-AI-20260420: Added ICommonRepository to resolve Supplier name → ID for entity persistence.

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

    /// <summary>
    /// Initializes a new instance of PartLifecycleService.
    /// (繁體中文) 初始化 PartLifecycleService 的新執行個體。
    /// </summary>
    public PartLifecycleService(IPartRepository repository, ICommonRepository commonRepository)
    {
        _repository = repository;
        _commonRepository = commonRepository;
    }

    /// <summary>
    /// Resolves a supplier name to its ID, defaulting to 1 if not found.
    /// (繁體中文) 將供應商名稱轉換為 ID，若找不到則預設為 1。
    /// </summary>
    private int ResolveSupplierIdByName(string supplierName) =>
        _commonRepository.GetSuppliers().FirstOrDefault(s => s.Name == supplierName)?.ID ?? 1;

    /// <inheritdoc/>
    public object CreatePart(PartSaveRequest request, string status)
    {
        var entity = new PartEntity
        {
            CustomerID = request.CustomerId ?? 101,
            PartNo = request.PartNo,
            CountryID = request.CountryId ?? 1,
            PartDescription = request.PartDesc,
            Division = request.Division,
            SupplierID = ResolveSupplierIdByName(request.Supplier),
            HTSCode = request.HtsCode,
            DutyRate = request.Rate,
            AddHTSCode1 = request.HtsCode1,
            AddDutyRate1 = request.Rate1,
            AddHTSCode2 = request.HtsCode2,
            AddDutyRate2 = request.Rate2,
            AddHTSCode3 = request.HtsCode3,
            AddDutyRate3 = request.Rate3,
            AddHTSCode4 = request.HtsCode4,
            AddDutyRate4 = request.Rate4,
            Remark = request.Remark,
            Status = status,
            CreatedBy = "AI-System",
            UpdatedBy = "AI-System"
        };

        var partId = _repository.CreatePart(entity);
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
            existing.UpdatedBy = "AI-System";
            
            _repository.UpdatePart(existing);
        }
        return new { };
    }

    /// <inheritdoc/>
    public object SubmitPart(int partId, PartSaveRequest request)
    {
        UpdatePart(partId, request);
        _repository.UpdateStatus(partId, "S02");
        return new { partId, status = "S02" };
    }

    /// <inheritdoc/>
    public object AcceptPart(int partId)
    {
        _repository.UpdateStatus(partId, "S04");
        return new { partId, status = "S04" };
    }

    /// <inheritdoc/>
    public object ReturnPart(int partId, string returnReason)
    {
        _repository.UpdateStatus(partId, "S03");
        return new { partId, status = "S03" };
    }

    /// <inheritdoc/>
    public object InactivatePart(int partId)
    {
        _repository.UpdateStatus(partId, "Inactive");
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
            }
            catch (Exception ex)
            {
                failed.Add(new { partId = id, errorMessage = ex.Message });
            }
        }
        return new { failed };
    }
}
