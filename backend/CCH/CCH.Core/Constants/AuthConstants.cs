namespace CCH.Core.Constants;

/// <summary>
/// Hardcoded authentication constants.
/// (繁體中文) 硬編碼認證常數。
/// </summary>
public static class AuthConstants
{
    // Update by AI (2026-04-21): Using encrypted/hashed passwords to match MyDimerco logic
    // (繁體中文) 由 AI 更新 (2026-04-21)：使用加密/雜湊後的密碼以符合 MyDimerco 邏輯

    
    // "888888" Rijndael: OEd54k9LJ69Ygi23eBFrVQ==
    // "888888" BCrypt: $2a$11$3hIMt6Vyn/nG0VcZc3xIpOQqKU5SeNa.LxUVeGQyb30XAUsNVNBDe
    public static readonly Dictionary<string, (string Password, string Role, string Name, bool IsInternal)> Users = new()
    {
        { "customer001", ("$2a$11$3hIMt6Vyn/nG0VcZc3xIpOQqKU5SeNa.LxUVeGQyb30XAUsNVNBDe", "customer", "Customer User", false) },
        { "dcb001", ("OEd54k9LJ69Ygi23eBFrVQ==", "dcb", "DCB User", true) },
        { "dimerco001", ("OEd54k9LJ69Ygi23eBFrVQ==", "dimerco", "Dimerco User", true) }
    };
}
