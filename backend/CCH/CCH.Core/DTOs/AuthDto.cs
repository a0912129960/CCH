namespace CCH.Core.DTOs;

/// <summary>
/// Login request body.
/// (繁體中文) 登入請求。
/// </summary>
public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Login response data.
/// (繁體中文) 登入回應資料。
/// </summary>
public class LoginResponse
{
    public string Token { get; set; } = "mock_jwt_token";
    public UserProfile User { get; set; } = new();
}

/// <summary>
/// User profile data.
/// (繁體中文) 使用者基本資料。
/// </summary>
public class UserProfile
{
    public string UserId { get; set; } = "U001";
    public string Name { get; set; } = "Mock User";
    public string Role { get; set; } = "DCB"; // DCB or Customer
}
