using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CCH.Core.Entities.CSP;

/// <summary>
/// Entity for CCHSuppliers table.
/// (繁�?中�?) CCHSuppliers 資�?表實體�?
/// </summary>
[Table("CCHSuppliers")]
public class CchSuppliers
{
    /// <summary>
    /// ID (IDENTITY)
    /// </summary>
    [Key]
    public int ID { get; set; }

    /// <summary>
    /// Project ID (Logical mapping to CPProject.Id, physical column remains CustomerID)
    /// (繁體中文) 專案 ID (邏輯上對應 CPProject.Id，實體欄位仍為 CustomerID)
    /// </summary>
    [Column("CustomerID")]
    public int? ProjectID { get; set; }

    /// <summary>
    /// Supplier Name
    /// </summary>
    [MaxLength(200)]
    public string? SupplierName { get; set; }

    /// <summary>
    /// Status
    /// </summary>
    [MaxLength(10)]
    public string? Status { get; set; }

    /// <summary>
    /// Created By
    /// </summary>
    [MaxLength(10)]
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Created Date
    /// </summary>
    public DateTime? CreatedDate { get; set; }
}
