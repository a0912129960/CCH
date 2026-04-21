using CCH.Core.Entities;
using CCH.Core.Entities.CSP;
using CCH.Core.Entities.ReSm;
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
    public virtual DbSet<CchParts> CchParts { get; set; }

    /// <summary>
    /// Gets or sets part milestones. (繁體中文) 零件里程碑。
    /// </summary>
    public virtual DbSet<CchPartMilestones> CchPartMilestones { get; set; }

    /// <summary>
    /// Gets or sets suppliers. (繁體中文) 供應商。
    /// </summary>
    public virtual DbSet<CchSuppliers> CchSuppliers { get; set; }

    /// <summary>
    /// Gets or sets logs. (繁體中文) 日誌。
    /// </summary>
    public virtual DbSet<CchLog> CchLog { get; set; }

    /// <summary>
    /// Gets or sets projects. (繁體中文) 專案。
    /// </summary>
    public virtual DbSet<CpProject> CpProject { get; set; }


}
