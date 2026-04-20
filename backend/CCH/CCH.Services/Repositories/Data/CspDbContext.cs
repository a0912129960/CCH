using CCH.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace CCH.Services.Repositories.Data;

/// <summary>
/// Database context for the CSP_Y2023 database.
/// (繁體中文) CSP_Y2023 資料庫的資料庫內容。
/// </summary>
public class CspDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CspDbContext"/> class.
    /// (繁體中文) 初始化 CspDbContext 類別的新執行個體。
    /// </summary>
    /// <param name="options">The options to be used by the DbContext. (資料庫內容所使用的選項)</param>
    public CspDbContext(DbContextOptions<CspDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets or sets parts. (繁體中文) 零件。
    /// </summary>
    public DbSet<CchPartEntity> Parts { get; set; }

    /// <summary>
    /// Gets or sets part milestones. (繁體中文) 零件里程碑。
    /// </summary>
    public DbSet<CchPartMilestoneEntity> PartMilestones { get; set; }

    /// <summary>
    /// Gets or sets suppliers. (繁體中文) 供應商。
    /// </summary>
    public DbSet<CchSupplierEntity> Suppliers { get; set; }

    /// <summary>
    /// Gets or sets logs. (繁體中文) 日誌。
    /// </summary>
    public DbSet<CchLogEntity> Logs { get; set; }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Add configurations for CSP_Y2023 entities here (在此加入 CSP_Y2023 實體的組態)
    }
}
