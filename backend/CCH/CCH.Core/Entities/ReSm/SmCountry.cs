using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CCH.Core.Entities.ReSm;

/// <summary>
/// Entity for the SMCountry table in the ReSm database.
/// (ç¹é?ä¸­æ?) ReSm è³‡æ?åº«ä¸­ SMCountry è³‡æ?è¡¨ç?å¯¦é???
/// </summary>
[Table("SMCountry")]
public class SmCountry
{
    /// <summary>
    /// HQ ID (Primary Key, Identity).
    /// (ç¹é?ä¸­æ?) ç¸½éƒ¨ ID (ä¸»éµ, ?ªå??å?)??
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int HQID { get; set; }

    /// <summary>
    /// Country Code.
    /// (ç¹é?ä¸­æ?) ?‹å®¶ä»?¢¼??
    /// </summary>
    [Required]
    [MaxLength(5)]
    [Column(TypeName = "varchar(5)")]
    public string CountryCode { get; set; } = string.Empty;

    /// <summary>
    /// Country Name.
    /// (ç¹é?ä¸­æ?) ?‹å®¶?ç¨±??
    /// </summary>
    [MaxLength(255)]
    public string? CountryName { get; set; }

    /// <summary>
    /// Area ID.
    /// (ç¹é?ä¸­æ?) ?€??ID??
    /// </summary>
    public int? AreaID { get; set; }

    /// <summary>
    /// Global Region ID.
    /// (ç¹é?ä¸­æ?) ?¨ç??€??ID??
    /// </summary>
    public int? GRegionID { get; set; }

    /// <summary>
    /// Prefix Number.
    /// (ç¹é?ä¸­æ?) ?ç¶´?Ÿç¢¼??
    /// </summary>
    [MaxLength(10)]
    [Column(TypeName = "varchar(10)")]
    public string? PrefixNumber { get; set; }

    /// <summary>
    /// Status.
    /// (ç¹é?ä¸­æ?) ?€?‹ã€?
    /// </summary>
    [Required]
    [MaxLength(10)]
    [Column(TypeName = "varchar(10)")]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Created By.
    /// (ç¹é?ä¸­æ?) å»ºç??…ã€?
    /// </summary>
    [MaxLength(6)]
    [Column(TypeName = "varchar(6)")]
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Created Date.
    /// (ç¹é?ä¸­æ?) å»ºç??¥æ???
    /// </summary>
    [Required]
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Updated By.
    /// (ç¹é?ä¸­æ?) ?´æ–°?…ã€?
    /// </summary>
    [MaxLength(6)]
    [Column(TypeName = "varchar(6)")]
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Updated Date.
    /// (ç¹é?ä¸­æ?) ?´æ–°?¥æ???
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Row Version (Timestamp).
    /// (ç¹é?ä¸­æ?) è³‡æ??—ç???(?‚é??³è?)??
    /// </summary>
    [Timestamp]
    public byte[] Version { get; set; } = null!;

    /// <summary>
    /// Currency.
    /// (ç¹é?ä¸­æ?) å¹?ˆ¥??
    /// </summary>
    [MaxLength(3)]
    [Column(TypeName = "varchar(3)")]
    public string? Currency { get; set; }

    /// <summary>
    /// Length Unit of Measure ID.
    /// (ç¹é?ä¸­æ?) ?·åº¦è¨ˆé??®ä? ID??
    /// </summary>
    public int? LengthUOMID { get; set; }

    /// <summary>
    /// Weight Unit of Measure ID.
    /// (ç¹é?ä¸­æ?) ?é?è¨ˆé??®ä? ID??
    /// </summary>
    public int? WeightUOMID { get; set; }

    /// <summary>
    /// Volume Unit of Measure ID.
    /// (ç¹é?ä¸­æ?) é«”ç?è¨ˆé??®ä? ID??
    /// </summary>
    public int? VolumeUOMID { get; set; }

    /// <summary>
    /// Show State.
    /// (ç¹é?ä¸­æ?) é¡¯ç¤ºå·åˆ¥/?ä»½??
    /// </summary>
    [MaxLength(20)]
    public string? ShowState { get; set; }

    /// <summary>
    /// Mandatory State.
    /// (ç¹é?ä¸­æ?) å¼·åˆ¶å·åˆ¥/?ä»½??
    /// </summary>
    public bool? MandatoryState { get; set; }

    /// <summary>
    /// Show Zip.
    /// (ç¹é?ä¸­æ?) é¡¯ç¤º?µé??€?Ÿã€?
    /// </summary>
    [MaxLength(20)]
    public string? ShowZip { get; set; }

    /// <summary>
    /// Mandatory Zip.
    /// (ç¹é?ä¸­æ?) å¼·åˆ¶?µé??€?Ÿã€?
    /// </summary>
    public bool? MandatoryZip { get; set; }

    /// <summary>
    /// VAT Name.
    /// (ç¹é?ä¸­æ?) å¢å€¼ç??ç¨±??
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string VATName { get; set; } = string.Empty;

    /// <summary>
    /// Local County Name.
    /// (ç¹é?ä¸­æ?) ?¶åœ°ç¸???ç¨±??
    /// </summary>
    [MaxLength(255)]
    public string? LocalCountyName { get; set; }

    /// <summary>
    /// Mandatory AMS Zip.
    /// (ç¹é?ä¸­æ?) å¼·åˆ¶ AMS ?µé??€?Ÿã€?
    /// </summary>
    public bool? MandatoryAMSZip { get; set; }

    /// <summary>
    /// Is Number For Customs Clearance Price.
    /// (ç¹é?ä¸­æ?) ?¯å¦?ºå ±?œåƒ¹?¼æ•¸å­—ã€?
    /// </summary>
    public bool? IsNumberForCustomsClearancePrice { get; set; }

    /// <summary>
    /// Is Mandatory HSCode AMS Export.
    /// (ç¹é?ä¸­æ?) AMS ?ºå£?¯å¦å¼·åˆ¶ HSCode??
    /// </summary>
    public bool? IsMandatoryHSCodeAMSExport { get; set; }

    /// <summary>
    /// Is Mandatory HSCode AMS Import.
    /// (ç¹é?ä¸­æ?) AMS ?²å£?¯å¦å¼·åˆ¶ HSCode??
    /// </summary>
    public bool? IsMandatoryHSCodeAMSImport { get; set; }

    /// <summary>
    /// Is EU (European Union).
    /// (ç¹é?ä¸­æ?) ?¯å¦?ºæ??Ÿã€?
    /// </summary>
    public bool? IsEU { get; set; }

    /// <summary>
    /// Check Contact Person.
    /// (ç¹é?ä¸­æ?) æª¢æŸ¥?¯çµ¡äººã€?
    /// </summary>
    [MaxLength(200)]
    [Column(TypeName = "varchar(200)")]
    public string? CheckContactPerson { get; set; }

    /// <summary>
    /// Check Enterprise Code.
    /// (ç¹é?ä¸­æ?) æª¢æŸ¥ä¼æ¥­ä»?¢¼??
    /// </summary>
    [MaxLength(200)]
    [Column(TypeName = "varchar(200)")]
    public string? CheckEnterpriseCode { get; set; }

    /// <summary>
    /// Check Telephone.
    /// (ç¹é?ä¸­æ?) æª¢æŸ¥?»è©±??
    /// </summary>
    [MaxLength(200)]
    [Column(TypeName = "varchar(200)")]
    public string? CheckTelephone { get; set; }

    /// <summary>
    /// Show Enterprise Code.
    /// (ç¹é?ä¸­æ?) é¡¯ç¤ºä¼æ¥­ä»?¢¼??
    /// </summary>
    public bool? ShowEnterpriseCode { get; set; }

    /// <summary>
    /// Check Email.
    /// (ç¹é?ä¸­æ?) æª¢æŸ¥?»å??µä»¶??
    /// </summary>
    [MaxLength(200)]
    [Column(TypeName = "varchar(200)")]
    public string? CheckeMail { get; set; }

    /// <summary>
    /// Show CUS (Customs).
    /// (ç¹é?ä¸­æ?) é¡¯ç¤ºæµ·é???
    /// </summary>
    public bool? ShowCUS { get; set; }

    /// <summary>
    /// Check Establishment Date.
    /// (ç¹é?ä¸­æ?) æª¢æŸ¥?ç??¥æ???
    /// </summary>
    [MaxLength(200)]
    [Column(TypeName = "varchar(200)")]
    public string? CheckEstablishmentDate { get; set; }
}
