using CCH.Core.Features.Dashboard.DTOs;

namespace CCH.Core.Features.Dashboard.Interfaces;

/// <summary>
/// Dashboard service interface.
/// (繁體中文) 儀表板服務介面。
/// </summary>
public interface IDashboardService
{
    PartStatusSummaryDto GetStatusSummary(string? projectId);

    /// <summary>
    /// Returns pending review parts filtered by role.
    /// role = "CUSTOMER" → S01 + S03; otherwise → S02.
    /// (依角色回傳待審核零件：客戶 → S01+S03；員工 → S02。)
    /// </summary>
    IEnumerable<PendingReviewDto> GetPendingReviews(string? projectId, string? role = null);
}
