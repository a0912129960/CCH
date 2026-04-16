using CCH.Core.DTOs;
using CCH.Core.Interfaces;
using CCH.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CCH.API.Controllers;

/// <summary>
/// Parts management controller.
/// (繁體中文) 零件管理控制器。
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PartsController : ControllerBase
{
    private readonly IPartQueryService _queryService;
    private readonly IPartLifecycleService _lifecycleService;
    private readonly IPartExcelService _excelService;

    public PartsController(IPartQueryService queryService, IPartLifecycleService lifecycleService, IPartExcelService excelService)
    {
        _queryService = queryService;
        _lifecycleService = lifecycleService;
        _excelService = excelService;
    }

    [HttpGet]
    public ActionResult<ApiResponse<PartListResponseDto>> SearchParts([FromQuery] string? customerId, [FromQuery] string? status, [FromQuery] string? partNo, [FromQuery] string? supplier, [FromQuery] int page = 1, [FromQuery] int pageSize = 10) =>
        Ok(ApiResponse<PartListResponseDto>.SuccessResponse(_queryService.SearchParts(customerId, status, partNo, supplier, page, pageSize)));

    [HttpGet("export")]
    public IActionResult ExportParts([FromQuery] string? customerId, [FromQuery] string? status, [FromQuery] string? partNo, [FromQuery] string? supplier)
    {
        var file = _excelService.ExportParts(customerId, status, partNo, supplier);
        return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "parts_export.xlsx");
    }

    [HttpPost("batch-accept")]
    public ActionResult<ApiResponse<object>> BatchAccept([FromBody] IEnumerable<int> partIds) =>
        Ok(ApiResponse<object>.SuccessResponse(_lifecycleService.BatchAccept(partIds)));

    [HttpPost("bulk-upload")]
    public ActionResult<ApiResponse<List<BulkUploadResponseDto>>> BulkUpload(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        return Ok(ApiResponse<List<BulkUploadResponseDto>>.SuccessResponse(_excelService.BulkUpload(stream)));
    }

    [HttpGet("bulk-upload/template")]
    public IActionResult GetUploadTemplate()
    {
        var file = _excelService.GetUploadTemplate();
        return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "parts_template.xlsx");
    }

    [HttpPost]
    public ActionResult<ApiResponse<object>> CreatePart([FromBody] PartSaveRequest request) =>
        Created($"/api/parts/{request.PartNo}", ApiResponse<object>.SuccessResponse(_lifecycleService.CreatePart(request, "S01")));

    [HttpPost("submit")]
    public ActionResult<ApiResponse<object>> CreateAndSubmitPart([FromBody] PartSaveRequest request) =>
        Created($"/api/parts/{request.PartNo}", ApiResponse<object>.SuccessResponse(_lifecycleService.CreatePart(request, "S02")));

    // INTERNAL-AI-20260416: Added 404 handling when part is not found.
    // (INTERNAL-AI-20260416: 新增零件不存在時的 404 回應處理。)
    /* [HttpGet("{partId}")]
    public ActionResult<ApiResponse<PartDetailResponseDto>> GetPartDetail(int partId) =>
        Ok(ApiResponse<PartDetailResponseDto>.SuccessResponse(_queryService.GetPartDetail(partId))); */
    [HttpGet("{partId}")]
    public ActionResult<ApiResponse<PartDetailResponseDto>> GetPartDetail(int partId)
    {
        // Attempt to retrieve part detail; return 404 if not found (嘗試取得零件詳細資料；若不存在則回傳 404)
        var result = _queryService.GetPartDetail(partId);
        if (result == null)
            return NotFound(ApiResponse<object>.FailureResponse("Part not found. / 零件不存在。"));
        return Ok(ApiResponse<PartDetailResponseDto>.SuccessResponse(result));
    }

    // INTERNAL-AI-20260416: Added Customer-only role restriction and 400 validation error handling.
    // (INTERNAL-AI-20260416: 新增僅限 Customer 角色存取限制與 400 驗證錯誤處理。)
    /* [HttpPut("{partId}")]
    public ActionResult<ApiResponse<object>> UpdatePart(int partId, [FromBody] PartSaveRequest request) =>
        Ok(ApiResponse<object>.SuccessResponse(_lifecycleService.UpdatePart(partId, request))); */
    [HttpPut("{partId}")]
    [Authorize(Roles = "customer")]
    public ActionResult<ApiResponse<object>> UpdatePart(int partId, [FromBody] PartSaveRequest request)
    {
        // Return 400 if any required fields or format validation fails (若驗證失敗則回傳 400)
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage);
            return BadRequest(ApiResponse<object>.FailureResponse(string.Join(" | ", errors)));
        }

        return Ok(ApiResponse<object>.SuccessResponse(_lifecycleService.UpdatePart(partId, request)));
    }

    [HttpPost("{partId}/submit")]
    public ActionResult<ApiResponse<object>> SubmitPart(int partId, [FromBody] PartSaveRequest request) =>
        Ok(ApiResponse<object>.SuccessResponse(_lifecycleService.SubmitPart(partId, request)));

    [HttpPost("{partId}/accept")]
    public ActionResult<ApiResponse<object>> AcceptPart(int partId) =>
        Ok(ApiResponse<object>.SuccessResponse(_lifecycleService.AcceptPart(partId)));

    [HttpPost("{partId}/return")]
    public ActionResult<ApiResponse<object>> ReturnPart(int partId, [FromBody] string returnReason) =>
        Ok(ApiResponse<object>.SuccessResponse(_lifecycleService.ReturnPart(partId, returnReason)));

    [HttpPost("{partId}/inactive")]
    public ActionResult<ApiResponse<object>> InactivatePart(int partId) =>
        Ok(ApiResponse<object>.SuccessResponse(_lifecycleService.InactivatePart(partId)));

    [HttpGet("{partId}/milestones")]
    public ActionResult<ApiResponse<IEnumerable<MilestoneDto>>> GetMilestones(int partId) =>
        Ok(ApiResponse<IEnumerable<MilestoneDto>>.SuccessResponse(_queryService.GetMilestones(partId)));

    [HttpGet("{partId}/history")]
    public ActionResult<ApiResponse<IEnumerable<PartDetailDto>>> GetHistory(int partId) =>
        Ok(ApiResponse<IEnumerable<PartDetailDto>>.SuccessResponse(_queryService.GetHistory(partId)));
}
