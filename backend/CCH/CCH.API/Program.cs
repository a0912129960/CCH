using System.Text;
using CCH.Core.Interfaces;
using CCH.Services.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICommonService, CommonService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

// Register split Part interfaces
builder.Services.AddScoped<IPartQueryService, PartService>();
builder.Services.AddScoped<IPartLifecycleService, PartService>();
builder.Services.AddScoped<IPartExcelService, PartService>();

builder.Services.AddControllers();

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
