using CCH.Core.Features.Parts.DTOs;
using CCH.Core.Features.Parts.Interfaces;
using CCH.Core.Interfaces.Repositories;

namespace CCH.Services.Features.Parts;

/// <summary>
/// Implementation of Part Lifecycle operations (Create, Update, Accept, Return).
/// (繁體中文) 零件生命週期操作實作（建立、修改、接受、退回）。
/// </summary>
public class PartLifecycleService : IPartLifecycleService
{
    private readonly IPartRepository _repository;

    public PartLifecycleService(IPartRepository repository)
    {
        _repository = repository;
    }

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

    public object BatchAccept(IEnumerable<int> partIds)
    {
        var failed = new List<object>();
        foreach (var id in partIds)
        {
            try
            {
                var detail = _repository.GetPartDetail(id);
                if (detail == null)
                {
                    failed.Add(new { partId = id, errorMessage = "Part not found. / 找不到零件。" });
                    continue;
                }

                if (detail.Status != "S02" && detail.Status != "S03")
                {
                    failed.Add(new { partId = id, errorMessage = $"Invalid status: {detail.Status}. Expected S02 or S03. / 錯誤的狀態：{detail.Status}。預期為 S02 或 S03。" });
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
