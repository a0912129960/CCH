using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CCH.Core.Entities;

/// <summary>
/// Entity for CCHSuppliers table.
/// (繁體中文) CCHSuppliers 資料表實體。
/// </summary>
[Table("CCHSuppliers")]
public class CchSupplierEntity
{
    /// <summary>
    /// ID (IDENTITY)
    /// </summary>
    [Key]
    public int ID { get; set; }

    /// <summary>
    /// Customer ID
    /// </summary>
    public int? CustomerID { get; set; }

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
