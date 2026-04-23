using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CCH.Core.Entities.ReSm;

/// <summary>
/// Ported from MyDimerco. Represents a group role setting in the ReSM database.
/// (繁體中文) 從 MyDimerco 移植。代表 ReSM 資料庫中的群組角色設定。
/// </summary>
[Table("SMGroupRoleSetting")]
public partial class SmgroupRoleSetting
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? RoleNo { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? RoleType { get; set; }

    [Column("RoleID")]
    public int? RoleId { get; set; }

    [Column("UserID")]
    [StringLength(6)]
    [Unicode(false)]
    public string? UserId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? ApplyMode { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? Status { get; set; }

    [StringLength(6)]
    [Unicode(false)]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    [StringLength(6)]
    [Unicode(false)]
    public string? UpdatedBy { get; set; }

    [Column("UPdatedDate", TypeName = "datetime")]
    public DateTime? UpdatedDate { get; set; }
}
