using CCH.Core.DTOs;

namespace CCH.Core.Interfaces;

/// <summary>
/// Part query service interface.
/// (繁體中文) 零件查詢服務介面。
/// </summary>
public interface IPartQueryService
{
    PartListResponseDto SearchParts(string? customerId, string? status, string? partNo, string? supplier, int page, int pageSize);
    PartDetailResponseDto GetPartDetail(int partId);
    IEnumerable<MilestoneDto> GetMilestones(int partId);
    IEnumerable<PartDetailDto> GetHistory(int partId);
}
