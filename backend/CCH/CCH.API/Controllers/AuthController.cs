using CCH.Core.DTOs;
using CCH.Core.Interfaces;
using CCH.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace CCH.API.Controllers;

/// <summary>
/// Authentication controller.
/// (繁體中文) 認證控制器。
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// User login.
    /// (繁體中文) 使用者登入。
    /// </summary>
    [HttpPost("login")]
    public ActionResult<ApiResponse<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var response = _authService.Login(request);
        return Ok(ApiResponse<LoginResponse>.SuccessResponse(response));
    }

    /// <summary>
    /// User logout.
    /// (繁體中文) 使用者登出。
    /// </summary>
    [HttpPost("logout")]
    public ActionResult<ApiResponse<object>> Logout()
    {
        _authService.Logout();
        return Ok(ApiResponse<object>.SuccessResponse(new { message = "Logged out" }));
    }
}
