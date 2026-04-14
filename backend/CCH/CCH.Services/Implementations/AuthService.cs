using CCH.Core.DTOs;
using CCH.Core.Interfaces;

namespace CCH.Services.Implementations;

/// <summary>
/// Mock authentication service.
/// (繁體中文) 模擬認證服務。
/// </summary>
public class AuthService : IAuthService
{
    public LoginResponse Login(LoginRequest request)
    {
        return new LoginResponse
        {
            Token = "mock_jwt_token_for_" + request.Username,
            User = new UserProfile
            {
                Id = "U001",
                Name = "Mock User",
                Role = request.Username.Contains("dcb") ? "DCB" : "Customer"
            }
        };
    }

    public bool Logout()
    {
        return true;
    }
}
