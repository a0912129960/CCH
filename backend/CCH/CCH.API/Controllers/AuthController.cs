using CCH.Core.Features.Auth.DTOs;
using CCH.Core.Features.Auth.Interfaces;
using CCH.Core.Interfaces;
using CCH.Core.Shared;
using Microsoft.AspNetCore.Authorization;
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
    /// Authenticates a user and returns a JWT token.
    /// (繁體中文) 驗證使用者並回傳 JWT token。
    /// </summary>
    [HttpPost("login")]
    public ActionResult<ApiResponse<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var response = _authService.Login(request);
        // Update by AI (2026-04-15): Return Unauthorized if login fails
        // (繁體中文) 由 AI 更新 (2026-04-15)：若登入失敗則回傳 Unauthorized
        if (response == null)
        {
            return Unauthorized(ApiResponse<LoginResponse>.FailureResponse("Invalid username or password"));
        }
        return Ok(ApiResponse<LoginResponse>.SuccessResponse(response));
    }

    /// <summary>
    /// Retrieves information about the currently authenticated user.
    /// (繁體中文) 取得目前已驗證使用者的資訊。
    /// </summary>
    [Authorize]
    [HttpGet("whoami")]
    public ActionResult<ApiResponse<object>> WhoAmI([FromServices] IUserContext userContext)
    {
        return Ok(ApiResponse<object>.SuccessResponse(new
        {
            userContext.UserId,
            userContext.UserName,
            userContext.Role,
            userContext.IsAuthenticated
        }));
    }
}
