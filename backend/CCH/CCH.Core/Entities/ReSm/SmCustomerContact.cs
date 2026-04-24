using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CCH.Core.Entities.ReSm;

/// <summary>
/// Ported from MyDimerco. Represents a customer contact in the ReSM database.
/// (繁體中文) 從 MyDimerco 移植。代表 ReSM 資料庫中的客戶聯絡人。
/// </summary>
[Table("SMCustomerContact")]
public partial class SmCustomerContact
{
    [Key]
    [Column("HQID")]
    public int Hqid { get; set; }

    [Column("CustomerID")]
    public int CustomerId { get; set; }

    [StringLength(100)]
    public string? Email { get; set; }

    [StringLength(50)]
    public string? FullName { get; set; }

    [StringLength(10)]
    public string? Status { get; set; }
}
