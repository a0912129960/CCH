using CCH.Core.DTOs;

namespace CCH.Core.Interfaces.Repositories;

/// <summary>
/// Part repository interface.
/// (繁體中文) 零件倉儲介面。
/// </summary>
public interface IPartRepository
{
    IEnumerable<PartListItemDto> SearchParts(string? customerId, string? status, string? partNo, string? supplier, string? role = null);
    PartDetailResponseDto? GetPartDetail(int partId);
    int CreatePart(PartSaveRequest request, string status);
    void UpdatePart(int partId, PartSaveRequest request);
    void UpdateStatus(int partId, string status);
}
