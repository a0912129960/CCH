using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CCH.Core.Entities.CSP;

/// <summary>
/// Entity for CCHPartHistories table (Snapshot history).
/// (繁體中文) CCHPartHistories 資料表實體（快照歷程紀錄）。
/// </summary>
[Table("CCHPartHistories")]
public class CchPartHistories
{
    /// <summary>
    /// ID (IDENTITY)
    /// </summary>
    [Key]
    public int ID { get; set; }

    /// <summary>
    /// Foreign Key to CCHParts
    /// (繁體中文) 關聯至 CCHParts 的外鍵。
    /// </summary>
    public int PartID { get; set; }

    /// <summary>
    /// Part Number (Snapshot)
    /// (繁體中文) 零件編號快照。
    /// </summary>
    [MaxLength(100)]
    public string? PartNo { get; set; }

    /// <summary>
    /// Country Name (Snapshot)
    /// (繁體中文) 國家名稱快照。
    /// </summary>
    [MaxLength(200)]
    public string? Country { get; set; }

    /// <summary>
    /// Division (Snapshot)
    /// (繁體中文) 部門快照。
    /// </summary>
    [MaxLength(100)]
    public string? Division { get; set; }

    /// <summary>
    /// Supplier Name (Snapshot)
    /// (繁體中文) 供應商名稱快照。
    /// </summary>
    [MaxLength(200)]
    public string? Supplier { get; set; }

    /// <summary>
    /// Part Description (Snapshot)
    /// (繁體中文) 零件描述快照。
    /// </summary>
    [MaxLength(500)]
    public string? PartDesc { get; set; }

    /// <summary>
    /// HTS Code (Snapshot)
    /// (繁體中文) HTS 代碼快照。
    /// </summary>
    [MaxLength(30)]
    public string? HtsCode { get; set; }

    /// <summary>
    /// Duty Rate (Snapshot)
    /// (繁體中文) 稅率快照。
    /// </summary>
    [Column(TypeName = "decimal(7, 4)")]
    public decimal? Rate { get; set; }

    /// <summary>
    /// Additional HTS Code 1 (Snapshot)
    /// </summary>
    [MaxLength(30)]
    public string? HtsCode1 { get; set; }

    /// <summary>
    /// Additional Duty Rate 1 (Snapshot)
    /// </summary>
    [Column(TypeName = "decimal(7, 4)")]
    public decimal? Rate1 { get; set; }

    /// <summary>
    /// Additional HTS Code 2 (Snapshot)
    /// </summary>
    [MaxLength(30)]
    public string? HtsCode2 { get; set; }

    /// <summary>
    /// Additional Duty Rate 2 (Snapshot)
    /// </summary>
    [Column(TypeName = "decimal(7, 4)")]
    public decimal? Rate2 { get; set; }

    /// <summary>
    /// Additional HTS Code 3 (Snapshot)
    /// </summary>
    [MaxLength(30)]
    public string? HtsCode3 { get; set; }

    /// <summary>
    /// Additional Duty Rate 3 (Snapshot)
    /// </summary>
    [Column(TypeName = "decimal(7, 4)")]
    public decimal? Rate3 { get; set; }

    /// <summary>
    /// Additional HTS Code 4 (Snapshot)
    /// </summary>
    [MaxLength(30)]
    public string? HtsCode4 { get; set; }

    /// <summary>
    /// Additional Duty Rate 4 (Snapshot)
    /// </summary>
    [Column(TypeName = "decimal(7, 4)")]
    public decimal? Rate4 { get; set; }

    /// <summary>
    /// Remark (Snapshot)
    /// (繁體中文) 備註快照。
    /// </summary>
    [MaxLength(2000)]
    public string? Remark { get; set; }

    /// <summary>
    /// Created By (Who triggered the snapshot)
    /// (繁體中文) 建立者（觸發快照的人）。
    /// </summary>
    [MaxLength(10)]
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Created Date (When the snapshot was taken)
    /// (繁體中文) 建立日期（拍攝快照的時間）。
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Whether the HTS Code was found on hts.usitc.gov (null = not checked, true = found, false = not found).
    /// TODO: Remove [NotMapped] once CCHPartHistories.IsHTSExists column is added to the DB.
    /// </summary>
    [NotMapped]
    public bool? IsHTSExists { get; set; }
}
