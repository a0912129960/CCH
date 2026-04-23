using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CCH.Core.Constants;
using CCH.Core.Features.Auth.DTOs;
using CCH.Core.Features.Auth.Interfaces;
using CCH.Core.Interfaces;
using CCH.Services.Repositories.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CCH.Services.Features.Auth;

/// <summary>
/// Authentication service with real database-based implementation.
/// (繁體中文) 基於真實資料庫實作的認證服務。
/// </summary>
public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly ReSmDbContext _reSmContext;
    private readonly CspDbContext _cspContext;

    public AuthService(IConfiguration configuration, ReSmDbContext reSmContext, CspDbContext cspContext)
    {
        _configuration = configuration;
        _reSmContext = reSmContext;
        _cspContext = cspContext;
    }

    public LoginResponse? Login(LoginRequest request)
    {
        // Update by AI (2026-04-21): Implement real login using ported MyDimerco logic and database tables
        // (繁體中文) 由 AI 更新 (2026-04-21)：使用移植的 MyDimerco 邏輯與資料庫資料表實作真實登入
        
        // MyDimerco logic: URL-decode the password first
        string decodedPassword = System.Web.HttpUtility.UrlDecode(request.Password);

        // 1. Try Internal User (Dimerco)
        var smUser = _reSmContext.SmUser.FirstOrDefault(u => 
            u.Status == "Active" && (u.UserID == request.Username || u.Email == request.Username));

        if (smUser != null)
        {
            if (CCH.Core.Utilities.DataEncryption.Encryption(decodedPassword) == smUser.Password)
            {
                // Determine role (simplified BIT check)
                bool isBIT = _reSmContext.SmgroupRoleSetting.Any(x => 
                    x.UserId == smUser.UserID && x.RoleNo != null && x.RoleNo.ToLower().EndsWith("bit") && x.Status == "Active");
                
                // Add by AI (2026-04-21): Check if user belongs to DCB station based on business entity parent mapping
                // (繁體中文) 由 AI 新增 (2026-04-21)：根據業務實體父層對照檢查使用者是否屬於 DCB 站點
                bool isDCB = _reSmContext.SmBusinessEntity.Any(b => 
                    b.StationId == smUser.StationID && 
                    _reSmContext.SmStation.Any(s => s.StationId == b.ParentId && (s.StationCode == "DCBORD" || s.StationCode == "DCBLAX")));

                string role = (isBIT || isDCB) ? "dcb" : "dimerco";

                var token = GenerateJwtToken(smUser.UserID, smUser.FullName, role);
                return new LoginResponse
                {
                    Token = token,
                    User = new UserProfile
                    {
                        UserId = smUser.UserID,
                        Name = smUser.FullName,
                        Role = role
                    }
                };
            }
        }

        // 2. Try External User (Customer Contact)
        // Adjust by AI (2026-04-21): Match provided SQL subquery logic for password verification
        // (繁體中文) 由 AI 調整 (2026-04-21)：配合提供的 SQL 子查詢邏輯進行密碼比對
        var contactHqids = _reSmContext.SmCustomerContact
            .Where(c => c.Status == "Active" && c.Email == request.Username)
            .Select(c => c.Hqid)
            .ToList();

        if (contactHqids.Any())
        {
            var registeredHqids = _cspContext.CpprojectContactor
                .Where(p => contactHqids.Contains(p.ContactorHqid ?? 0) && p.InviteStatus == "Registered")
                .Select(p => p.ContactorHqid)
                .ToList();

            if (registeredHqids.Any())
            {
                var passwords = _cspContext.CpprojectContactorPassword
                    .Where(p => registeredHqids.Contains(p.ContactorHqid))
                    .ToList();

                foreach (var pwd in passwords)
                {
                    if (BCrypt.Net.BCrypt.Verify(decodedPassword, pwd.Password))
                    {
                        var contactInfo = _reSmContext.SmCustomerContact.First(c => c.Hqid == pwd.ContactorHqid);
                        var token = GenerateJwtToken(contactInfo.Email ?? contactInfo.Hqid.ToString(), contactInfo.FullName ?? "", "customer");
                        return new LoginResponse
                        {
                            Token = token,
                            User = new UserProfile
                            {
                                UserId = contactInfo.Email ?? contactInfo.Hqid.ToString(),
                                Name = contactInfo.FullName ?? "",
                                Role = "customer"
                            }
                        };
                    }
                }
            }
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
}
