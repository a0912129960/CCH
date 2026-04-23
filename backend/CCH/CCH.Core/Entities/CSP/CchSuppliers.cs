using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CCH.Core.Entities.CSP;

/// <summary>
/// Entity for CCHSuppliers table.
/// (ÁπÅÈ?‰∏≠Ê?) CCHSuppliers Ë≥áÊ?Ë°®ÂØ¶È´î„Ä?
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
