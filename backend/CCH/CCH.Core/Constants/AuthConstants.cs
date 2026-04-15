namespace CCH.Core.Constants;

/// <summary>
/// Hardcoded authentication constants.
/// (繁體中文) 硬編碼認證常數。
/// </summary>
public static class AuthConstants
{
    public static readonly Dictionary<string, (string Password, string Role, string Name)> Users = new()
    {
        { "customer001", ("888888", "customer", "Customer User") },
        { "dcb001", ("888888", "dcb", "DCB User") },
        { "dimerco001", ("888888", "dimerco", "Dimerco User") }
    };
}
