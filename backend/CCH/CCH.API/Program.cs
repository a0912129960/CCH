using System.Text;
using CCH.Core.Features.Parts.Interfaces;
using CCH.Core.Features.Auth.Interfaces;
using CCH.Core.Features.Common.Interfaces;
using CCH.Core.Features.Dashboard.Interfaces;
using CCH.Core.Interfaces;
using CCH.Core.Interfaces.Repositories;
using CCH.Core.Shared;
using CCH.Services.Repositories;
using CCH.Services.Features.Parts;
using CCH.Services.Features.Auth;
using CCH.Services.Features.Common;
using CCH.Services.Features.Dashboard;
using CCH.Services.Infrastructure;
using CCH.Services.Repositories.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Register DbContexts / 註冊資料庫內容
builder.Services.AddDbContext<CspY2023DbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CspConnection")));
builder.Services.AddDbContext<ReSmDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ReSmConnection")));

// Add services to the container.
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICommonService, CommonService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

// Register Repositories / 註冊倉儲
builder.Services.AddScoped<IPartRepository, PartRepository>();
builder.Services.AddScoped<ICommonRepository, CommonRepository>();

// Register split Part interfaces / 註冊拆分後的零件服務
builder.Services.AddScoped<IPartQueryService, PartQueryService>();
builder.Services.AddScoped<IPartLifecycleService, PartLifecycleService>();
builder.Services.AddScoped<IPartExcelService, PartExcelService>();

// INTERNAL-AI-20260416: Override default [ApiController] 400 response to use ApiResponse format.
// (INTERNAL-AI-20260416: 覆寫 [ApiController] 預設 400 回應，改用 ApiResponse 格式。)
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage);
            var message = string.Join(" | ", errors);
            return new BadRequestObjectResult(ApiResponse<object>.FailureResponse(message));
        };
    });

// Add HttpContextAccessor and UserContext / 新增 HttpContextAccessor 與 UserContext
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContext, UserContext>();

// Add CORS configuration / 新增 CORS 配置
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Add JWT Authentication / 新增 JWT 認證
// Update by AI (2026-04-15): Configure JWT Bearer authentication
// (繁體中文) 由 AI 更新 (2026-04-15)：配置 JWT 持有者認證
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use CORS middleware / 使用 CORS 中介軟體
// Update by AI (2026-04-15): Apply "AllowAll" policy
// (繁體中文) 由 AI 更新 (2026-04-15)：套用 "AllowAll" 策略
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
