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
    object BatchAccept(IEnumerable<string> partIds);
    BulkUploadResponseDto BulkUpload(Stream fileStream);
    byte[] GetUploadTemplate();
    PartDetailResponseDto GetPartDetail(string partId);
    object CreatePart(PartSaveRequest request, string status);
    object UpdatePart(string partId, PartSaveRequest request);
    object SubmitPart(string partId, PartSaveRequest request);
    object AcceptPart(string partId);
    object ReturnPart(string partId, string returnReason);
    object InactivatePart(string partId);
    IEnumerable<MilestoneDto> GetMilestones(string partId);
    IEnumerable<PartDetailDto> GetHistory(string partId);
}
