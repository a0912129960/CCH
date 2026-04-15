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

    [HttpGet("{partId}")]
    public ActionResult<ApiResponse<PartDetailResponseDto>> GetPartDetail(int partId) =>
        Ok(ApiResponse<PartDetailResponseDto>.SuccessResponse(_queryService.GetPartDetail(partId)));

    [HttpPut("{partId}")]
    public ActionResult<ApiResponse<object>> UpdatePart(int partId, [FromBody] PartSaveRequest request) =>
        Ok(ApiResponse<object>.SuccessResponse(_lifecycleService.UpdatePart(partId, request)));

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
