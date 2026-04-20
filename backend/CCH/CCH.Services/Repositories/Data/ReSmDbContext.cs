using Microsoft.EntityFrameworkCore;

namespace CCH.Services.Repositories.Data;

/// <summary>
/// Database context for the ReSM database.
/// (繁體中文) ReSM 資料庫的資料庫內容。
/// </summary>
public class ReSmDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReSmDbContext"/> class.
    /// (繁體中文) 初始化 ReSmDbContext 類別的新執行個體。
    /// </summary>
    /// <param name="options">The options to be used by the DbContext. (資料庫內容所使用的選項)</param>
    public ReSmDbContext(DbContextOptions<ReSmDbContext> options)
        : base(options)
    {
    }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Add configurations for ReSM entities here (在此加入 ReSM 實體的組態)
    }
}
