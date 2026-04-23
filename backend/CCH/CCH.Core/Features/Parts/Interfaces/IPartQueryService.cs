using CCH.Core.Features.Parts.DTOs;

namespace CCH.Core.Features.Parts.Interfaces;

/// <summary>
/// Part query service interface.
/// (繁體中文) 零件查詢服務介面。
/// </summary>
public interface IPartQueryService
{
    PartListResponseDto SearchParts(int? projectId, string? status, string? partNo, int? supplierId, int page, int pageSize);

    // INTERNAL-AI-20260416: GetPartDetail returns nullable to support 404 when part does not exist.
    // (INTERNAL-AI-20260416: GetPartDetail 改為可為空回傳，以支援零件不存在時的 404 回應。)
    /* PartDetailResponseDto GetPartDetail(int partId); */
    PartDetailResponseDto? GetPartDetail(int partId);

    IEnumerable<MilestoneDto> GetMilestones(int partId);
    IEnumerable<PartDetailDto> GetHistory(int partId);
}
