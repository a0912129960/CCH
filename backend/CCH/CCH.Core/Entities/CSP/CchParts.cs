using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CCH.Core.Entities.CSP;

/// <summary>
/// Entity for CCHParts table.
/// (繁�?中�?) CCHParts 資�?表實體�?
/// </summary>
[Table("CCHParts")]
public class CchParts
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
    /// Part Number
    /// </summary>
    [MaxLength(100)]
    public string? PartNo { get; set; }

    /// <summary>
    /// Country ID
    /// </summary>
    public int? CountryID { get; set; }

    /// <summary>
    /// Part Description
    /// </summary>
    [MaxLength(500)]
    public string? PartDescription { get; set; }

    /// <summary>
    /// Division
    /// </summary>
    [MaxLength(100)]
    public string? Division { get; set; }

    /// <summary>
    /// Supplier ID
    /// </summary>
    public int? SupplierID { get; set; }

    /// <summary>
    /// HTS Code
    /// </summary>
    [MaxLength(30)]
    public string? HTSCode { get; set; }

    /// <summary>
    /// Duty Rate
    /// </summary>
    [Column(TypeName = "decimal(7, 4)")]
    public decimal? DutyRate { get; set; }

    /// <summary>
    /// Additional HTS Code 1
    /// </summary>
    [MaxLength(30)]
    public string? AddHTSCode1 { get; set; }

    /// <summary>
    /// Additional Duty Rate 1
    /// </summary>
    [Column(TypeName = "decimal(7, 4)")]
    public decimal? AddDutyRate1 { get; set; }

    /// <summary>
    /// Additional HTS Code 2
    /// </summary>
    [MaxLength(30)]
    public string? AddHTSCode2 { get; set; }

    /// <summary>
    /// Additional Duty Rate 2
    /// </summary>
    [Column(TypeName = "decimal(7, 4)")]
    public decimal? AddDutyRate2 { get; set; }

    /// <summary>
    /// Additional HTS Code 3
    /// </summary>
    [MaxLength(30)]
    public string? AddHTSCode3 { get; set; }

    /// <summary>
    /// Additional Duty Rate 3
    /// </summary>
    [Column(TypeName = "decimal(7, 4)")]
    public decimal? AddDutyRate3 { get; set; }

    /// <summary>
    /// Additional HTS Code 4
    /// </summary>
    [MaxLength(30)]
    public string? AddHTSCode4 { get; set; }

    /// <summary>
    /// Additional Duty Rate 4
    /// </summary>
    [Column(TypeName = "decimal(7, 4)")]
    public decimal? AddDutyRate4 { get; set; }

    /// <summary>
    /// Remark
    /// </summary>
    [MaxLength(2000)]
    public string? Remark { get; set; }

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
    /// Updated By
    /// </summary>
    [MaxLength(10)]
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Created Date
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Updated Date
    /// </summary>
    public DateTime? UpdatedDate { get; set; }
}
