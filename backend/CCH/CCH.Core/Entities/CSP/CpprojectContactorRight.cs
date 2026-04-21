using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CCH.Core.Entities.CSP;

/// <summary>
/// Ported from MyDimerco. Represents a project contactor right in the CSP database.
/// (繁體中文) 從 MyDimerco 移植。代表 CSP 資料庫中的專案聯絡人權限。
/// </summary>
[Table("CPProjectContactorRight")]
public partial class CpprojectContactorRight
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("ProjectID")]
    public int ProjectId { get; set; }

    [Column("CustomerContactID")]
    public int CustomerContactId { get; set; }

    [Column("RightID")]
    public int RightId { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string CreatedBy { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string UpdatedBy { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime UpdatedDate { get; set; }
}
