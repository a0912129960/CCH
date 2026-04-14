using CCH.Core.DTOs;

namespace CCH.Core.Interfaces;

/// <summary>
/// Part lifecycle service interface.
/// (繁體中文) 零件生命週期服務介面。
/// </summary>
public interface IPartLifecycleService
{
    object CreatePart(PartSaveRequest request, string status);
    object UpdatePart(int partId, PartSaveRequest request);
    object SubmitPart(int partId, PartSaveRequest request);
    object AcceptPart(int partId);
    object ReturnPart(int partId, string returnReason);
    object InactivatePart(int partId);
    object BatchAccept(IEnumerable<int> partIds);
}
