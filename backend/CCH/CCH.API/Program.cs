using CCH.Core.Interfaces;
using CCH.Services.Services;

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

app.UseAuthorization();

app.MapControllers();

app.Run();
