using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CCH.Core.Entities.CSP;

/// <summary>
/// Ported from MyDimerco. Represents a project contactor in the CSP database.
/// (繁體中文) 從 MyDimerco 移植。代表 CSP 資料庫中的專案聯絡人。
/// </summary>
[Table("CPProjectContactor")]
public partial class CpprojectContactor
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }
    [Column("ProjectID")]
    public int? ProjectId { get; set; }
    [Column("MemberID")]
    public int? MemberId { get; set; }
    [Column("ContactorHQID")]
    public int? ContactorHqid { get; set; }
    [StringLength(30)]
    public string? Password { get; set; }
    [StringLength(100)]
    public string? Comments { get; set; }
    [StringLength(10)]
    [Unicode(false)]
    public string? Status { get; set; }
    [StringLength(100)]
    [Unicode(false)]
    public string? CreatedBy { get; set; }
    [Column("CreatedDT", TypeName = "datetime")]
    public DateTime? CreatedDt { get; set; }
    [StringLength(100)]
    [Unicode(false)]
    public string? UpdatedBy { get; set; }
    [Column("UpdatedDT", TypeName = "datetime")]
    public DateTime? UpdatedDt { get; set; }
    [StringLength(10)]
    [Unicode(false)]
    public string Role { get; set; }
    public string InviteToken { get; set; }
    [StringLength(10)]
    [Unicode(false)]
    public string InviteStatus { get; set; }
    [StringLength(10)]
    [Unicode(false)]
    public string LaneType { get; set; }
}
