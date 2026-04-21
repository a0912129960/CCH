using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CCH.Core.Entities.ReSm;

/// <summary>
/// Ported from MyDimerco. Represents a business entity in the ReSM database.
/// (繁體中文) 從 MyDimerco 移植。代表 ReSM 資料庫中的業務實體。
/// </summary>
[Table("SMBusinessEntity")]
public partial class SmBusinessEntity
{
    [Key]
    [Column("HQID")]
    public int Hqid { get; set; }

    [Required]
    [StringLength(3)]
    [Column("StationID")]
    public string StationId { get; set; } = null!;

    [Required]
    [StringLength(3)]
    [Column("ParentID")]
    public string ParentId { get; set; } = null!;

    [Column("EffectiveDate")]
    public DateTime EffectiveDate { get; set; }

    [StringLength(6)]
    [Column("CreatedBy")]
    public string? CreatedBy { get; set; }

    [Column("CreatedDate")]
    public DateTime CreatedDate { get; set; }

    [StringLength(6)]
    [Column("UpdatedBy")]
    public string? UpdatedBy { get; set; }

    [Column("UpdatedDate")]
    public DateTime? UpdatedDate { get; set; }

    [Timestamp]
    [Column("Version")]
    public byte[] Version { get; set; } = null!;

    [StringLength(50)]
    [Column("Status")]
    public string? Status { get; set; }
}
