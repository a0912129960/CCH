using CCH.Core.Features.Parts.DTOs;

namespace CCH.Core.Features.Parts.Interfaces;

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
    // INTERNAL-AI-20260421: S04 → S03 transition for Dimerco/Customer: save + notify customer for review.
    // (INTERNAL-AI-20260421: S04 → S03，Dimerco/Customer 儲存後通知客戶審核。)
    object SendToCustomerReview(int partId, PartSaveRequest request);
}
