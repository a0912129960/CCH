using CCH.Core.Features.Common.DTOs;
using CCH.Core.Features.Common.Interfaces;
using CCH.Core.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CCH.API.Controllers;

/// <summary>
/// Common data controller.
/// (繁體中文) 共用資料控制器。
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CommonController : ControllerBase
{
    private readonly ICommonService _commonService;

    public CommonController(ICommonService commonService)
    {
        _commonService = commonService;
    }

    /// <summary>
    /// Retrieves a list of all customers.
    /// (繁體中文) 取得所有客戶清單。
    /// </summary>
    [HttpGet("customers")]
    public ActionResult<ApiResponse<IEnumerable<KeyValuePairDto>>> GetCustomers() =>
        Ok(ApiResponse<IEnumerable<KeyValuePairDto>>.SuccessResponse(_commonService.GetCustomers()));

    /// <summary>
    /// Retrieves a list of all countries.
    /// (繁體中文) 取得所有國家清單。
    /// </summary>
    [HttpGet("countries")]
    public ActionResult<ApiResponse<IEnumerable<KeyValuePairDto>>> GetCountries() =>
        Ok(ApiResponse<IEnumerable<KeyValuePairDto>>.SuccessResponse(_commonService.GetCountries()));

    /// <summary>
    /// Retrieves a list of suppliers, optionally filtered by customer.
    /// (繁體中文) 取得供應商清單（可選擇依客戶過濾）。
    /// </summary>
    [HttpGet("suppliers")]
    public ActionResult<ApiResponse<IEnumerable<KeyValuePairDto>>> GetSuppliers([FromQuery] string? customerId) =>
        Ok(ApiResponse<IEnumerable<KeyValuePairDto>>.SuccessResponse(_commonService.GetSuppliers(customerId)));

    /// <summary>
    /// Retrieves a list of all part statuses.
    /// (繁體中文) 取得所有零件狀態清單。
    /// </summary>
    [HttpGet("status")]
    public ActionResult<ApiResponse<IEnumerable<KeyValuePairDto>>> GetStatus() =>
        Ok(ApiResponse<IEnumerable<KeyValuePairDto>>.SuccessResponse(_commonService.GetStatus()));
}
