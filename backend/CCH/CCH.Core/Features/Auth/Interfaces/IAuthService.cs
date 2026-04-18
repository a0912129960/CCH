using CCH.Core.Features.Auth.DTOs;

namespace CCH.Core.Features.Auth.Interfaces;

/// <summary>
/// Authentication service interface.
/// (繁體中文) 認證服務介面。
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// User login.
    /// (繁體中文) 使用者登入。
    /// </summary>
    LoginResponse? Login(LoginRequest request);
}
