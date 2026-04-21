using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CCH.Core.Entities;

/// <summary>
/// Entity for the SMCountry table in the ReSm database.
/// (繁體中文) ReSm 資料庫中 SMCountry 資料表的實體。
/// </summary>
[Table("SMCountry")]
public class SmCountry
{
    /// <summary>
    /// HQ ID (Primary Key, Identity).
    /// (繁體中文) 總部 ID (主鍵, 自動遞增)。
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int HQID { get; set; }

    /// <summary>
    /// Country Code.
    /// (繁體中文) 國家代碼。
    /// </summary>
    [Required]
    [MaxLength(5)]
    [Column(TypeName = "varchar(5)")]
    public string CountryCode { get; set; } = string.Empty;

    /// <summary>
    /// Country Name.
    /// (繁體中文) 國家名稱。
    /// </summary>
    [MaxLength(255)]
    public string? CountryName { get; set; }

    /// <summary>
    /// Area ID.
    /// (繁體中文) 區域 ID。
    /// </summary>
    public int? AreaID { get; set; }

    /// <summary>
    /// Global Region ID.
    /// (繁體中文) 全球區域 ID。
    /// </summary>
    public int? GRegionID { get; set; }

    /// <summary>
    /// Prefix Number.
    /// (繁體中文) 前綴號碼。
    /// </summary>
    [MaxLength(10)]
    [Column(TypeName = "varchar(10)")]
    public string? PrefixNumber { get; set; }

    /// <summary>
    /// Status.
    /// (繁體中文) 狀態。
    /// </summary>
    [Required]
    [MaxLength(10)]
    [Column(TypeName = "varchar(10)")]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Created By.
    /// (繁體中文) 建立者。
    /// </summary>
    [MaxLength(6)]
    [Column(TypeName = "varchar(6)")]
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Created Date.
    /// (繁體中文) 建立日期。
    /// </summary>
    [Required]
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Updated By.
    /// (繁體中文) 更新者。
    /// </summary>
    [MaxLength(6)]
    [Column(TypeName = "varchar(6)")]
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Updated Date.
    /// (繁體中文) 更新日期。
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Row Version (Timestamp).
    /// (繁體中文) 資料列版本 (時間戳記)。
    /// </summary>
    [Timestamp]
    public byte[] Version { get; set; } = null!;

    /// <summary>
    /// Currency.
    /// (繁體中文) 幣別。
    /// </summary>
    [MaxLength(3)]
    [Column(TypeName = "varchar(3)")]
    public string? Currency { get; set; }

    /// <summary>
    /// Length Unit of Measure ID.
    /// (繁體中文) 長度計量單位 ID。
    /// </summary>
    public int? LengthUOMID { get; set; }

    /// <summary>
    /// Weight Unit of Measure ID.
    /// (繁體中文) 重量計量單位 ID。
    /// </summary>
    public int? WeightUOMID { get; set; }

    /// <summary>
    /// Volume Unit of Measure ID.
    /// (繁體中文) 體積計量單位 ID。
    /// </summary>
    public int? VolumeUOMID { get; set; }

    /// <summary>
    /// Show State.
    /// (繁體中文) 顯示州別/省份。
    /// </summary>
    [MaxLength(20)]
    public string? ShowState { get; set; }

    /// <summary>
    /// Mandatory State.
    /// (繁體中文) 強制州別/省份。
    /// </summary>
    public bool? MandatoryState { get; set; }

    /// <summary>
    /// Show Zip.
    /// (繁體中文) 顯示郵遞區號。
    /// </summary>
    [MaxLength(20)]
    public string? ShowZip { get; set; }

    /// <summary>
    /// Mandatory Zip.
    /// (繁體中文) 強制郵遞區號。
    /// </summary>
    public bool? MandatoryZip { get; set; }

    /// <summary>
    /// VAT Name.
    /// (繁體中文) 增值稅名稱。
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string VATName { get; set; } = string.Empty;

    /// <summary>
    /// Local County Name.
    /// (繁體中文) 當地縣市名稱。
    /// </summary>
    [MaxLength(255)]
    public string? LocalCountyName { get; set; }

    /// <summary>
    /// Mandatory AMS Zip.
    /// (繁體中文) 強制 AMS 郵遞區號。
    /// </summary>
    public bool? MandatoryAMSZip { get; set; }

    /// <summary>
    /// Is Number For Customs Clearance Price.
    /// (繁體中文) 是否為報關價格數字。
    /// </summary>
    public bool? IsNumberForCustomsClearancePrice { get; set; }

    /// <summary>
    /// Is Mandatory HSCode AMS Export.
    /// (繁體中文) AMS 出口是否強制 HSCode。
    /// </summary>
    public bool? IsMandatoryHSCodeAMSExport { get; set; }

    /// <summary>
    /// Is Mandatory HSCode AMS Import.
    /// (繁體中文) AMS 進口是否強制 HSCode。
    /// </summary>
    public bool? IsMandatoryHSCodeAMSImport { get; set; }

    /// <summary>
    /// Is EU (European Union).
    /// (繁體中文) 是否為歐盟。
    /// </summary>
    public bool? IsEU { get; set; }

    /// <summary>
    /// Check Contact Person.
    /// (繁體中文) 檢查聯絡人。
    /// </summary>
    [MaxLength(200)]
    [Column(TypeName = "varchar(200)")]
    public string? CheckContactPerson { get; set; }

    /// <summary>
    /// Check Enterprise Code.
    /// (繁體中文) 檢查企業代碼。
    /// </summary>
    [MaxLength(200)]
    [Column(TypeName = "varchar(200)")]
    public string? CheckEnterpriseCode { get; set; }

    /// <summary>
    /// Check Telephone.
    /// (繁體中文) 檢查電話。
    /// </summary>
    [MaxLength(200)]
    [Column(TypeName = "varchar(200)")]
    public string? CheckTelephone { get; set; }

    /// <summary>
    /// Show Enterprise Code.
    /// (繁體中文) 顯示企業代碼。
    /// </summary>
    public bool? ShowEnterpriseCode { get; set; }

    /// <summary>
    /// Check Email.
    /// (繁體中文) 檢查電子郵件。
    /// </summary>
    [MaxLength(200)]
    [Column(TypeName = "varchar(200)")]
    public string? CheckeMail { get; set; }

    /// <summary>
    /// Show CUS (Customs).
    /// (繁體中文) 顯示海關。
    /// </summary>
    public bool? ShowCUS { get; set; }

    /// <summary>
    /// Check Establishment Date.
    /// (繁體中文) 檢查成立日期。
    /// </summary>
    [MaxLength(200)]
    [Column(TypeName = "varchar(200)")]
    public string? CheckEstablishmentDate { get; set; }
}
