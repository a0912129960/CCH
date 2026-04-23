using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CCH.Core.Entities.CSP;

/// <summary>
/// Ported from MyDimerco. Represents a project contactor password in the CSP database.
/// (繁體中文) 從 MyDimerco 移植。代表 CSP 資料庫中的專案聯絡人密碼。
/// </summary>
[Table("CPProjectContactorPassword")]
public partial class CpprojectContactorPassword
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("ContactorHQID")]
    public int ContactorHqid { get; set; }

    [StringLength(100)]
    public string Password { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string CreatedBy { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime UpdatedDate { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string UpdatedBy { get; set; } = null!;
}
