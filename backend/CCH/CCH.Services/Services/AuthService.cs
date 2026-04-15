using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CCH.Core.Constants;
using CCH.Core.DTOs;
using CCH.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CCH.Services.Services;

/// <summary>
/// Authentication service with real JWT implementation.
/// (繁體中文) 真實 JWT 實作的認證服務。
/// </summary>
public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;

    public AuthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public LoginResponse? Login(LoginRequest request)
    {
        // Update by AI (2026-04-15): Validate against hardcoded users and generate real JWT
        // (繁體中文) 由 AI 更新 (2026-04-15)：對照硬編碼使用者進行驗證並產生真實 JWT
        if (AuthConstants.Users.TryGetValue(request.Username, out var userInfo) && userInfo.Password == request.Password)
        {
            var token = GenerateJwtToken(request.Username, userInfo.Name, userInfo.Role);
            return new LoginResponse
            {
                Token = token,
                User = new UserProfile
                {
                    UserId = request.Username,
                    Name = userInfo.Name,
                    Role = userInfo.Role
                }
            };
        }
        return null;
    }

    private string GenerateJwtToken(string userId, string username, string role)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.Now.AddDays(double.Parse(jwtSettings["DurationInDays"]!)),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public bool Logout()
    {
        return true;
    }
}
