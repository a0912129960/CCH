using Microsoft.EntityFrameworkCore;

namespace CCH.Services.Repositories.Data;

/// <summary>
/// Database context for the CSP_Y2023 database.
/// (繁體中文) CSP_Y2023 資料庫的資料庫內容。
/// </summary>
public class CspY2023DbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CspY2023DbContext"/> class.
    /// (繁體中文) 初始化 CspY2023DbContext 類別的新執行個體。
    /// </summary>
    /// <param name="options">The options to be used by the DbContext. (資料庫內容所使用的選項)</param>
    public CspY2023DbContext(DbContextOptions<CspY2023DbContext> options)
        : base(options)
    {
    }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Add configurations for CSP_Y2023 entities here (在此加入 CSP_Y2023 實體的組態)
    }
}
