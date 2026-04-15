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
        // Update by AI (2026-04-15): Return Unauthorized if login fails
        // (繁體中文) 由 AI 更新 (2026-04-15)：若登入失敗則回傳 Unauthorized
        if (response == null)
        {
            return Unauthorized(ApiResponse<LoginResponse>.ErrorResponse("Invalid username or password"));
        }
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
