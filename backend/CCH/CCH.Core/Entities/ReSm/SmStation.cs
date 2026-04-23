using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CCH.Core.Entities.ReSm;

/// <summary>
/// Ported from MyDimerco. Represents a station in the ReSM database.
/// (繁體中文) 從 MyDimerco 移植。代表 ReSM 資料庫中的站點。
/// </summary>
[Table("SMStation")]
public partial class SmStation
{
    [Key]
    [Column("HQID")]
    public int Hqid { get; set; }

    [Required]
    [StringLength(3)]
    [Column("StationID")]
    public string StationId { get; set; } = null!;

    [Required]
    [StringLength(6)]
    [Column("StationCode")]
    public string StationCode { get; set; } = null!;

    [StringLength(255)]
    [Column("StationName")]
    public string? StationName { get; set; }
}
