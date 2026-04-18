using System.Security.Claims;
using CCH.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace CCH.Services.Infrastructure;

/// <summary>
/// User context implementation using IHttpContextAccessor.
/// (繁體中文) 使用 IHttpContextAccessor 的使用者上下文實作。
/// </summary>
public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public string? UserId => User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    public string? UserName => User?.Identity?.Name;
    public string? Role => User?.FindFirst(ClaimTypes.Role)?.Value;
    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;
}
