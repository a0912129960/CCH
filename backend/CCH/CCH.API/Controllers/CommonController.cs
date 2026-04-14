using CCH.Core.DTOs;
using CCH.Core.Interfaces;
using CCH.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace CCH.API.Controllers;

/// <summary>
/// Common data controller.
/// (繁體中文) 共用資料控制器。
/// </summary>
[ApiController]
[Route("api/common")]
public class CommonController : ControllerBase
{
    private readonly ICommonService _commonService;

    public CommonController(ICommonService commonService)
    {
        _commonService = commonService;
    }

    [HttpGet("customers")]
    public ActionResult<ApiResponse<IEnumerable<KeyValuePairDto>>> GetCustomers() =>
        Ok(ApiResponse<IEnumerable<KeyValuePairDto>>.SuccessResponse(_commonService.GetCustomers()));

    [HttpGet("countries")]
    public ActionResult<ApiResponse<IEnumerable<KeyValuePairDto>>> GetCountries() =>
        Ok(ApiResponse<IEnumerable<KeyValuePairDto>>.SuccessResponse(_commonService.GetCountries()));

    [HttpGet("suppliers")]
    public ActionResult<ApiResponse<IEnumerable<KeyValuePairDto>>> GetSuppliers([FromQuery] string? customerId) =>
        Ok(ApiResponse<IEnumerable<KeyValuePairDto>>.SuccessResponse(_commonService.GetSuppliers(customerId)));

    [HttpGet("status")]
    public ActionResult<ApiResponse<IEnumerable<KeyValuePairDto>>> GetStatus() =>
        Ok(ApiResponse<IEnumerable<KeyValuePairDto>>.SuccessResponse(_commonService.GetStatus()));
}
