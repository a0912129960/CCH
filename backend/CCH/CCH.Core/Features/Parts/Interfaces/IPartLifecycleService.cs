using CCH.Core.Features.Parts.DTOs;

namespace CCH.Core.Features.Parts.Interfaces;

/// <summary>
/// Part lifecycle service interface.
/// (繁體中文) 零件生命週期服務介面。
/// </summary>
public interface IPartLifecycleService
{
    object CreatePart(PartCreateRequest request, string status);
    /// <summary>
    /// Creates a new part and immediately submits it to Dimerco (S02).
    /// Writes two CCHPartMilestones records:
    ///   1. Action="Created",              FromStatus=null, ToStatus="S01"
    ///   2. Action="Submitted to Dimerco", FromStatus="S01", ToStatus="S02"
    /// CCHPartHistories snapshot logic is unchanged.
    /// (繁體中文) 建立零件並立即送審至 Dimerco（S02）。
    /// 寫入兩筆 CCHPartMilestones：Created(null→S01)、Submitted to Dimerco(S01→S02)。
    /// </summary>
    object CreateAndSubmitPart(PartSaveRequest request);
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
