using CCH.Core.DTOs;
using CCH.Core.Interfaces;
using CCH.Core.Models;
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

    [HttpGet("part-status-summary")]
    public ActionResult<ApiResponse<PartStatusSummaryDto>> GetStatusSummary([FromQuery] string? customerId) =>
        Ok(ApiResponse<PartStatusSummaryDto>.SuccessResponse(_dashboardService.GetStatusSummary(customerId)));

    [HttpGet("pending-review")]
    public ActionResult<ApiResponse<IEnumerable<PendingReviewDto>>> GetPendingReviews([FromQuery] string? customerId) =>
        Ok(ApiResponse<IEnumerable<PendingReviewDto>>.SuccessResponse(_dashboardService.GetPendingReviews(customerId)));
}
