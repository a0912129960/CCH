using CCH.Core.Features.Parts.DTOs;

namespace CCH.Core.Features.Parts.Interfaces;

/// <summary>
/// Part lifecycle service interface.
/// (繁體中文) 零件生命週期服務介面。
/// </summary>
public interface IPartLifecycleService
{
    object CreatePart(PartCreateRequest request, string status);
    object UpdatePart(int partId, PartSaveRequest request);
    object SubmitPart(int partId, PartSaveRequest request);
    object AcceptPart(int partId);
    object ReturnPart(int partId, string returnReason);
    object InactivatePart(int partId);
    object BatchAccept(IEnumerable<int> partIds);
}
