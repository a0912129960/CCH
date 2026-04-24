using CCH.Core.Features.Parts.DTOs;
using CCH.Core.Features.Parts.Interfaces;
using CCH.Core.Shared;
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

    /// <summary>
    /// Searches for parts based on various criteria.
    /// (繁體中文) 根據多種條件搜尋零件。
    /// </summary>
    [HttpGet]
    public ActionResult<ApiResponse<PartListResponseDto>> SearchParts([FromQuery] int? customerId, [FromQuery] string? status, [FromQuery] string? partNo, [FromQuery] int? supplier, [FromQuery] int page = 1, [FromQuery] int pageSize = 10) =>
        Ok(ApiResponse<PartListResponseDto>.SuccessResponse(_queryService.SearchParts(customerId, status, partNo, supplier, page, pageSize)));

    /// <summary>
    /// Checks whether the given PartNo + CountryId combination already exists for the customer.
    /// Returns { isDuplicate: true/false }.
    /// (繁體中文) 檢查指定客戶下 PartNo + CountryId 組合是否重複，回傳 { isDuplicate: true/false }。
    /// </summary>
    [HttpGet("check-duplicate")]
    public ActionResult<ApiResponse<object>> CheckDuplicate(
        [FromQuery] int customerId,
        [FromQuery] string partNo,
        [FromQuery] int countryId)
    {
        if (string.IsNullOrWhiteSpace(partNo))
            return BadRequest(ApiResponse<object>.FailureResponse("partNo is required. / partNo 為必填。"));

        var isDuplicate = _queryService.CheckDuplicate(customerId, partNo, countryId);
        return Ok(ApiResponse<object>.SuccessResponse(new { isDuplicate }));
    }

    /// <summary>
    /// Exports parts to an Excel file.
    /// (繁體中文) 將零件匯出至 Excel 檔案。
    /// </summary>
    [HttpGet("export")]
    public IActionResult ExportParts([FromQuery] int? customerId, [FromQuery] string? status, [FromQuery] string? partNo, [FromQuery] int? supplier)
    {
        var file = _excelService.ExportParts(customerId, status, partNo, supplier);
        return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "parts_export.xlsx");
    }

    /// <summary>
    /// Bulk accepts a list of parts.
    /// (繁體中文) 批次接受零件清單。
    /// </summary>
    [HttpPost("batch-accept")]
    public ActionResult<ApiResponse<object>> BatchAccept([FromBody] IEnumerable<int> partIds) =>
        Ok(ApiResponse<object>.SuccessResponse(_lifecycleService.BatchAccept(partIds)));

    /// <summary>
    /// Previews parts in bulk from an Excel file.
    /// (繁體中文) 從 Excel 檔案預覽批次上傳零件。
    /// </summary>
    [HttpPost("bulk-upload/preview")]
    public ActionResult<ApiResponse<BulkUploadPreviewDto>> PreviewBulkUpload([FromForm] int customerId, IFormFile file)
    {
        using var stream = file.OpenReadStream();
        return Ok(ApiResponse<BulkUploadPreviewDto>.SuccessResponse(_excelService.PreviewBulkUpload(customerId, stream)));
    }

    /// <summary>
    /// Confirms and persists the uploaded parts.
    /// (繁體中文) 確認並持久化上傳的零件。
    /// </summary>
    [HttpPost("bulk-upload/confirm")]
    public ActionResult<ApiResponse<BulkUploadConfirmResponseDto>> ConfirmBulkUpload([FromBody] List<PartDto> parts)
    {
        return Ok(ApiResponse<BulkUploadConfirmResponseDto>.SuccessResponse(_excelService.ConfirmBulkUpload(parts)));
    }

    /// <summary>
    /// Retrieves the Excel template for bulk upload.
    /// (繁體中文) 取得批次上傳用的 Excel 範本。
    /// </summary>
    [HttpGet("bulk-upload/template")]
    public IActionResult GetUploadTemplate()
    {
        var file = _excelService.GetUploadTemplate();
        return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "parts_template.xlsx");
    }

    /// <summary>
    /// Creates a new part in draft status (S01). Only PartNo and CountryId are required.
    /// (繁體中文) 建立草稿狀態的新零件（S01）。只需 PartNo 與 CountryId。
    /// </summary>
    [HttpPost]
    public ActionResult<ApiResponse<object>> CreatePart([FromBody] PartCreateRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage);
            return BadRequest(ApiResponse<object>.FailureResponse(string.Join(" | ", errors)));
        }
        return Created($"/api/parts/{request.PartNo}", ApiResponse<object>.SuccessResponse(_lifecycleService.CreatePart(request, "S01")));
    }

    /// <summary>
    /// Creates and submits a new part for review (S02).
    /// Requires PartNo, CountryId, Division, Supplier, PartDesc, and HtsCode.
    /// (繁體中文) 建立並送審新零件（S02）。須填 PartNo、CountryId、Division、Supplier、PartDesc、HtsCode。
    /// </summary>
    [HttpPost("submit")]
    public ActionResult<ApiResponse<object>> CreateAndSubmitPart([FromBody] PartSaveRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage);
            return BadRequest(ApiResponse<object>.FailureResponse(string.Join(" | ", errors)));
        }
        return Created($"/api/parts/{request.PartNo}", ApiResponse<object>.SuccessResponse(_lifecycleService.CreatePart(
            new PartCreateRequest
            {
                CustomerId  = request.CustomerId,
                PartNo      = request.PartNo,
                CountryId   = request.CountryId,
                Division    = request.Division,
                Supplier    = request.Supplier,
                PartDesc    = request.PartDesc,
                HtsCode     = request.HtsCode,
                Rate        = request.Rate,
                HtsCode1    = request.HtsCode1, Rate1 = request.Rate1,
                HtsCode2    = request.HtsCode2, Rate2 = request.Rate2,
                HtsCode3    = request.HtsCode3, Rate3 = request.Rate3,
                HtsCode4    = request.HtsCode4, Rate4 = request.Rate4,
                Remark      = request.Remark
            }, "S02")));
    }

    // INTERNAL-AI-20260416: Added 404 handling when part is not found.
    // (INTERNAL-AI-20260416: 新增零件不存在時的 404 回應處理。)
    /* [HttpGet("{partId}")]
    public ActionResult<ApiResponse<PartDetailResponseDto>> GetPartDetail(int partId) =>
        Ok(ApiResponse<PartDetailResponseDto>.SuccessResponse(_queryService.GetPartDetail(partId))); */
    /// <summary>
    /// Retrieves detailed information for a specific part.
    /// (繁體中文) 取得特定零件的詳細資訊。
    /// </summary>
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
    // INTERNAL-AI-20260420: Opened Save to all roles (Customer, DCB, Dimerco) per spec — removed Customer-only restriction.
    // (INTERNAL-AI-20260420: 依規格開放所有角色（Customer、DCB、Dimerco）可呼叫 Save，移除僅限 Customer 限制。)
    /* [Authorize(Roles = "customer")] */
    /// <summary>
    /// Updates an existing part (all authenticated roles).
    /// (繁體中文) 更新現有零件（所有已驗證角色）。
    /// </summary>
    [HttpPut("{partId}")]
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

    // INTERNAL-AI-20260416: Customer only; same validation as PUT. Status S01/S03 → S02.
    // (INTERNAL-AI-20260416: 僅限 Customer；與 PUT 相同驗證邏輯。狀態 S01/S03 → S02。)
    /* [HttpPost("{partId}/submit")]
    public ActionResult<ApiResponse<object>> SubmitPart(int partId, [FromBody] PartSaveRequest request) =>
        Ok(ApiResponse<object>.SuccessResponse(_lifecycleService.SubmitPart(partId, request))); */
    /// <summary>
    /// Submits a part for review (Customer only).
    /// (繁體中文) 提交零件以供審核（僅限客戶）。
    /// </summary>
    [HttpPost("{partId}/submit")]
    [Authorize(Roles = "customer")]
    public ActionResult<ApiResponse<object>> SubmitPart(int partId, [FromBody] PartSaveRequest request)
    {
        // Return 400 if validation fails (驗證失敗回傳 400)
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage);
            return BadRequest(ApiResponse<object>.FailureResponse(string.Join(" | ", errors)));
        }

        return Ok(ApiResponse<object>.SuccessResponse(_lifecycleService.SubmitPart(partId, request)));
    }

    // INTERNAL-AI-20260416: Both Dimerco and DCB can review (accept/return) parts temporarily.
    // (INTERNAL-AI-20260416: 暫時允許 Dimerco 與 DCB 兩個角色皆可審核（接受/退回）零件。)
    /* [HttpPost("{partId}/accept")]
    public ActionResult<ApiResponse<object>> AcceptPart(int partId) =>
        Ok(ApiResponse<object>.SuccessResponse(_lifecycleService.AcceptPart(partId))); */
    // INTERNAL-AI-20260416: Only DCB can review (accept/return) parts.
    // (INTERNAL-AI-20260416: 僅 DCB 角色可審核（接受/退回）零件。)
    /* [Authorize(Roles = "dimerco,dcb")] */
    // INTERNAL-AI-20260420: Added S02 status guard per spec (S02 → S04). (依規格加入 S02 狀態驗證。)
    /// <summary>
    /// Accepts a part after review (DCB only). Requires status S02.
    /// (繁體中文) 審核後接受零件（僅限 DCB）。狀態須為 S02。
    /// </summary>
    [HttpPost("{partId}/accept")]
    [Authorize(Roles = "dcb")]
    public ActionResult<ApiResponse<object>> AcceptPart(int partId)
    {
        var part = _queryService.GetPartDetail(partId);
        if (part == null)
            return NotFound(ApiResponse<object>.FailureResponse("Part not found. / 零件不存在。"));
        if (part.Status != "S02")
            return BadRequest(ApiResponse<object>.FailureResponse("Part must be in Pending Dimerco Review (S02) status. / 零件狀態須為 S02。"));
        return Ok(ApiResponse<object>.SuccessResponse(_lifecycleService.AcceptPart(partId)));
    }

    // INTERNAL-AI-20260420: Added 400 for missing returnReason and S02 status guard per spec.
    // (INTERNAL-AI-20260420: 依規格加入 returnReason 必填驗證與 S02 狀態驗證。)
    /// <summary>
    /// Returns a part to the customer with a reason (DCB only). Body: { returnReason }. Requires status S02.
    /// (繁體中文) 將零件退回給客戶並附上原因（僅限 DCB）。請求主體：{ returnReason }。狀態須為 S02。
    /// </summary>
    [HttpPost("{partId}/return")]
    [Authorize(Roles = "dcb")]
    public ActionResult<ApiResponse<object>> ReturnPart(int partId, [FromBody] ReturnReasonDto body)
    {
        if (string.IsNullOrWhiteSpace(body.ReturnReason))
            return BadRequest(ApiResponse<object>.FailureResponse("returnReason is required. / 退回原因為必填。"));
        var part = _queryService.GetPartDetail(partId);
        if (part == null)
            return NotFound(ApiResponse<object>.FailureResponse("Part not found. / 零件不存在。"));
        if (part.Status != "S02")
            return BadRequest(ApiResponse<object>.FailureResponse("Part must be in Pending Dimerco Review (S02) status. / 零件狀態須為 S02。"));
        return Ok(ApiResponse<object>.SuccessResponse(_lifecycleService.ReturnPart(partId, body.ReturnReason)));
    }

    // INTERNAL-AI-20260420: Added Customer-only role restriction per spec. (依規格加入僅限 Customer 角色限制。)
    /// <summary>
    /// Marks a part as inactive (Customer only).
    /// (繁體中文) 將零件標記為停用（僅限客戶）。
    /// </summary>
    [HttpPost("{partId}/inactive")]
    [Authorize(Roles = "customer")]
    public ActionResult<ApiResponse<object>> InactivatePart(int partId) =>
        Ok(ApiResponse<object>.SuccessResponse(_lifecycleService.InactivatePart(partId)));

    // INTERNAL-AI-20260421: S04 → S03 transition for Dimerco/Customer: saves data then sets Pending Customer Review.
    // (INTERNAL-AI-20260421: S04 → S03，Dimerco/Customer 儲存後通知客戶審核。)
    /// <summary>
    /// Saves part data and sets status to Pending Customer Review (Dimerco/Customer, S04 only).
    /// (繁體中文) 儲存零件資料並將狀態設為 Pending Customer Review（Dimerco/Customer，僅限 S04）。
    /// </summary>
    [HttpPost("{partId}/send-to-customer-review")]
    [Authorize(Roles = "customer,dimerco")]
    public ActionResult<ApiResponse<object>> SendToCustomerReview(int partId, [FromBody] PartSaveRequest request)
    {
        var part = _queryService.GetPartDetail(partId);
        if (part == null)
            return NotFound(ApiResponse<object>.FailureResponse("Part not found. / 零件不存在。"));
        if (part.Status != "S04")
            return BadRequest(ApiResponse<object>.FailureResponse("Part must be in Reviewed (S04) status. / 零件狀態須為 S04。"));
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return BadRequest(ApiResponse<object>.FailureResponse(string.Join(" | ", errors)));
        }
        return Ok(ApiResponse<object>.SuccessResponse(_lifecycleService.SendToCustomerReview(partId, request)));
    }

    /// <summary>
    /// Retrieves milestones for a specific part.
    /// (繁體中文) 取得特定零件的里程碑（歷程）。
    /// </summary>
    [HttpGet("{partId}/milestones")]
    public ActionResult<ApiResponse<IEnumerable<MilestoneDto>>> GetMilestones(int partId) =>
        Ok(ApiResponse<IEnumerable<MilestoneDto>>.SuccessResponse(_queryService.GetMilestones(partId)));

    /// <summary>
    /// Retrieves the change history of a specific part.
    /// (繁體中文) 取得特定零件的變更歷史。
    /// </summary>
    [HttpGet("{partId}/history")]
    public ActionResult<ApiResponse<IEnumerable<PartDetailDto>>> GetHistory(int partId) =>
        Ok(ApiResponse<IEnumerable<PartDetailDto>>.SuccessResponse(_queryService.GetHistory(partId)));
}
