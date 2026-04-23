using CCH.Core.Features.Dashboard.DTOs;
using CCH.Core.Features.Dashboard.Interfaces;
using CCH.Core.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CCH.API.Controllers;

/// <summary>
/// Dashboard controller.
/// (繁體中文) 儀表板控制器。
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    /// <summary>
    /// Retrieves a summary of part counts categorized by status.
    /// Returns data as a single-element array: [{ S01, S02, S03, S04, S05 }].
    /// Pass projectId to filter by customer; omit (or pass "all") for all customers.
    /// (繁體中文) 取得按狀態分類的零件數量摘要。data 以單元素陣列回傳。
    /// </summary>
    [HttpGet("part-status-summary")]
    public ActionResult<ApiResponse<IEnumerable<PartStatusSummaryDto>>> GetStatusSummary([FromQuery] string? projectId)
    {
        var effectiveId = string.IsNullOrWhiteSpace(projectId) || projectId == "all" ? null : projectId;
        var summary = _dashboardService.GetStatusSummary(effectiveId);
        return Ok(ApiResponse<IEnumerable<PartStatusSummaryDto>>.SuccessResponse(new[] { summary }));
    }

    /// <summary>
    /// Retrieves a list of parts pending review, filtered by role.
    /// role = "CUSTOMER" → S01 + S03; omit / any other value → S02.
    /// Pass projectId to filter by customer; omit (or pass "all") for all customers.
    /// (繁體中文) 依角色取得待審核零件清單：客戶 → S01+S03；員工 → S02。
    /// </summary>
    [HttpGet("pending-review")]
    public ActionResult<ApiResponse<IEnumerable<PendingReviewDto>>> GetPendingReviews(
        [FromQuery] string? projectId,
        [FromQuery] string? role)
    {
        var effectiveId = string.IsNullOrWhiteSpace(projectId) || projectId == "all" ? null : projectId;
        return Ok(ApiResponse<IEnumerable<PendingReviewDto>>.SuccessResponse(
            _dashboardService.GetPendingReviews(effectiveId, role)));
    }
}
