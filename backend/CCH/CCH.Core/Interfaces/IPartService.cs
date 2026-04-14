using CCH.Core.DTOs;

namespace CCH.Core.Interfaces;

/// <summary>
/// Part service interface.
/// (繁體中文) 零件服務介面。
/// </summary>
public interface IPartService
{
    PartListResponseDto SearchParts(string? customerId, string? status, string? partNo, string? supplier, int page, int pageSize);
    byte[] ExportParts(string? customerId, string? status, string? partNo, string? supplier);
    object BatchAccept(IEnumerable<int> partIds);
    List<BulkUploadResponseDto> BulkUpload(Stream fileStream);
    byte[] GetUploadTemplate();
    PartDetailResponseDto GetPartDetail(int partId);
    object CreatePart(PartSaveRequest request, string status);
    object UpdatePart(int partId, PartSaveRequest request);
    object SubmitPart(int partId, PartSaveRequest request);
    object AcceptPart(int partId);
    object ReturnPart(int partId, string returnReason);
    object InactivatePart(int partId);
    IEnumerable<MilestoneDto> GetMilestones(int partId);
    IEnumerable<PartDetailDto> GetHistory(int partId);
}
