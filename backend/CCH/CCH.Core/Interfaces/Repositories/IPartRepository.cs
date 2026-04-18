using CCH.Core.Features.Parts.DTOs;

namespace CCH.Core.Interfaces.Repositories;

/// <summary>
/// Part repository interface.
/// (繁體中文) 零件倉儲介面。
/// </summary>
public interface IPartRepository
{
    IEnumerable<PartListItemDto> SearchParts(int? customerId, string? status, string? partNo, int? supplierId, string? role = null);
    PartDetailResponseDto? GetPartDetail(int partId);
    int CreatePart(PartSaveRequest request, string status);
    void UpdatePart(int partId, PartSaveRequest request);
    void UpdateStatus(int partId, string status);
}
