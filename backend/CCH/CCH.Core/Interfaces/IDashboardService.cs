using CCH.Core.DTOs;

namespace CCH.Core.Interfaces;

/// <summary>
/// Dashboard service interface.
/// (繁體中文) 儀表板服務介面。
/// </summary>
public interface IDashboardService
{
    PartStatusSummaryDto GetStatusSummary(string? customerId);
    IEnumerable<PendingReviewDto> GetPendingReviews(string? customerId);
}
