namespace CCH.Core.Interfaces;

/// <summary>
/// User context interface to access current authenticated user information.
/// (繁體中文) 使用者上下文介面，用於存取目前已認證的使用者資訊。
/// </summary>
public interface IUserContext
{
    string? UserId { get; }
    string? UserName { get; }
    string? Role { get; }
    bool IsAuthenticated { get; }
}
