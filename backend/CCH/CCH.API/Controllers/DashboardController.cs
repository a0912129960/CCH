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
    /// (繁體中文) 取得按狀態分類的零件數量摘要。
    /// </summary>
    [HttpGet("part-status-summary")]
    public ActionResult<ApiResponse<PartStatusSummaryDto>> GetStatusSummary([FromQuery] string? customerId) =>
        Ok(ApiResponse<PartStatusSummaryDto>.SuccessResponse(_dashboardService.GetStatusSummary(customerId)));

    /// <summary>
    /// Retrieves a list of parts that are pending review.
    /// (繁體中文) 取得待審核的零件清單。
    /// </summary>
    [HttpGet("pending-review")]
    public ActionResult<ApiResponse<IEnumerable<PendingReviewDto>>> GetPendingReviews([FromQuery] string? customerId) =>
        Ok(ApiResponse<IEnumerable<PendingReviewDto>>.SuccessResponse(_dashboardService.GetPendingReviews(customerId)));
}
