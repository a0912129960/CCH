using CCH.Core.DTOs;
using CCH.Core.Interfaces;
using CCH.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace CCH.API.Controllers;

/// <summary>
/// Parts management controller.
/// (繁體中文) 零件管理控制器。
/// </summary>
[ApiController]
[Route("api/parts")]
public class PartsController : ControllerBase
{
    private readonly IPartService _partService;

    public PartsController(IPartService partService)
    {
        _partService = partService;
    }

    [HttpGet]
    public ActionResult<ApiResponse<PartListResponseDto>> SearchParts([FromQuery] string? customerId, [FromQuery] string? status, [FromQuery] string? partNo, [FromQuery] string? supplier, [FromQuery] int page = 1, [FromQuery] int pageSize = 10) =>
        Ok(ApiResponse<PartListResponseDto>.SuccessResponse(_partService.SearchParts(customerId, status, partNo, supplier, page, pageSize)));

    [HttpGet("export")]
    public IActionResult ExportParts([FromQuery] string? customerId, [FromQuery] string? status, [FromQuery] string? partNo, [FromQuery] string? supplier)
    {
        var file = _partService.ExportParts(customerId, status, partNo, supplier);
        return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "parts_export.xlsx");
    }

    [HttpPost("batch-accept")]
    public ActionResult<ApiResponse<object>> BatchAccept([FromBody] IEnumerable<string> partIds) =>
        Ok(ApiResponse<object>.SuccessResponse(_partService.BatchAccept(partIds)));

    [HttpPost("bulk-upload")]
    public ActionResult<ApiResponse<BulkUploadResponseDto>> BulkUpload(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        return Ok(ApiResponse<BulkUploadResponseDto>.SuccessResponse(_partService.BulkUpload(stream)));
    }

    [HttpGet("bulk-upload/template")]
    public IActionResult GetUploadTemplate()
    {
        var file = _partService.GetUploadTemplate();
        return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "parts_template.xlsx");
    }

    [HttpPost]
    public ActionResult<ApiResponse<object>> CreatePart([FromBody] PartSaveRequest request) =>
        Created($"/api/parts/{request.PartNo}", ApiResponse<object>.SuccessResponse(_partService.CreatePart(request, "S01")));

    [HttpPost("submit")]
    public ActionResult<ApiResponse<object>> CreateAndSubmitPart([FromBody] PartSaveRequest request) =>
        Created($"/api/parts/{request.PartNo}", ApiResponse<object>.SuccessResponse(_partService.CreatePart(request, "S02")));

    [HttpGet("{partId}")]
    public ActionResult<ApiResponse<PartDetailResponseDto>> GetPartDetail(string partId) =>
        Ok(ApiResponse<PartDetailResponseDto>.SuccessResponse(_partService.GetPartDetail(partId)));

    [HttpPut("{partId}")]
    public ActionResult<ApiResponse<object>> UpdatePart(string partId, [FromBody] PartSaveRequest request) =>
        Ok(ApiResponse<object>.SuccessResponse(_partService.UpdatePart(partId, request)));

    [HttpPost("{partId}/submit")]
    public ActionResult<ApiResponse<object>> SubmitPart(string partId, [FromBody] PartSaveRequest request) =>
        Ok(ApiResponse<object>.SuccessResponse(_partService.SubmitPart(partId, request)));

    [HttpPost("{partId}/accept")]
    public ActionResult<ApiResponse<object>> AcceptPart(string partId) =>
        Ok(ApiResponse<object>.SuccessResponse(_partService.AcceptPart(partId)));

    [HttpPost("{partId}/return")]
    public ActionResult<ApiResponse<object>> ReturnPart(string partId, [FromBody] string returnReason) =>
        Ok(ApiResponse<object>.SuccessResponse(_partService.ReturnPart(partId, returnReason)));

    [HttpPost("{partId}/inactive")]
    public ActionResult<ApiResponse<object>> InactivatePart(string partId) =>
        Ok(ApiResponse<object>.SuccessResponse(_partService.InactivatePart(partId)));

    [HttpGet("{partId}/milestones")]
    public ActionResult<ApiResponse<IEnumerable<MilestoneDto>>> GetMilestones(string partId) =>
        Ok(ApiResponse<IEnumerable<MilestoneDto>>.SuccessResponse(_partService.GetMilestones(partId)));

    [HttpGet("{partId}/history")]
    public ActionResult<ApiResponse<IEnumerable<PartDetailDto>>> GetHistory(string partId) =>
        Ok(ApiResponse<IEnumerable<PartDetailDto>>.SuccessResponse(_partService.GetHistory(partId)));
}
