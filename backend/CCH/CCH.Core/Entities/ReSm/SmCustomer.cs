using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CCH.Core.Entities.ReSm;

/// <summary>
/// Entity for the SMCustomer table in the ReSm database.
/// (ç¹é?ä¸­æ?) ReSm è³‡æ?åº«ä¸­ SMCustomer è³‡æ?è¡¨ç?å¯¦é???
/// </summary>
[Table("SMCustomer")]
public class SmCustomer
{
    /// <summary>
    /// HQ ID (Primary Key, Identity).
    /// (ç¹é?ä¸­æ?) ç¸½éƒ¨ ID (ä¸»éµ, ?ªå??å?)??
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int HQID { get; set; }

    /// <summary>
    /// Station ID.
    /// (ç¹é?ä¸­æ?) ç«™é? ID??
    /// </summary>
    [Required]
    [MaxLength(3)]
    [Column(TypeName = "varchar(3)")]
    public string StationID { get; set; } = string.Empty;

    /// <summary>
    /// Customer Code.
    /// (ç¹é?ä¸­æ?) å®¢æˆ¶ä»?¢¼??
    /// </summary>
    [Required]
    [MaxLength(15)]
    [Column(TypeName = "varchar(15)")]
    public string CustomerCode { get; set; } = string.Empty;

    /// <summary>
    /// Customer Name.
    /// (ç¹é?ä¸­æ?) å®¢æˆ¶?ç¨±??
    /// </summary>
    [MaxLength(255)]
    public string? CustomerName { get; set; }

    /// <summary>
    /// City ID.
    /// (ç¹é?ä¸­æ?) ?å? ID??
    /// </summary>
    public int? CityID { get; set; }

    /// <summary>
    /// Industry ID.
    /// (ç¹é?ä¸­æ?) è¡Œæ¥­ ID??
    /// </summary>
    public int? IndustryID { get; set; }

    /// <summary>
    /// Global Code.
    /// (ç¹é?ä¸­æ?) ?¨ç?ä»?¢¼??
    /// </summary>
    [MaxLength(10)]
    [Column(TypeName = "varchar(10)")]
    public string? GlobalCode { get; set; }

    /// <summary>
    /// Customer Name 1.
    /// (ç¹é?ä¸­æ?) å®¢æˆ¶?ç¨± 1??
    /// </summary>
    [MaxLength(255)]
    public string? CustomerName1 { get; set; }

    /// <summary>
    /// Customer Address 1.
    /// (ç¹é?ä¸­æ?) å®¢æˆ¶?°å? 1??
    /// </summary>
    [MaxLength(255)]
    public string? CustomerAddress1 { get; set; }

    /// <summary>
    /// Customer Address 2.
    /// (ç¹é?ä¸­æ?) å®¢æˆ¶?°å? 2??
    /// </summary>
    [MaxLength(255)]
    public string? CustomerAddress2 { get; set; }

    /// <summary>
    /// Customer Address 3.
    /// (ç¹é?ä¸­æ?) å®¢æˆ¶?°å? 3??
    /// </summary>
    [MaxLength(255)]
    public string? CustomerAddress3 { get; set; }

    /// <summary>
    /// Customer Address 4.
    /// (ç¹é?ä¸­æ?) å®¢æˆ¶?°å? 4??
    /// </summary>
    [MaxLength(255)]
    public string? CustomerAddress4 { get; set; }

    /// <summary>
    /// Customer Address 5.
    /// (ç¹é?ä¸­æ?) å®¢æˆ¶?°å? 5??
    /// </summary>
    [MaxLength(255)]
    public string? CustomerAddress5 { get; set; }

    /// <summary>
    /// Phone.
    /// (ç¹é?ä¸­æ?) ?»è©±??
    /// </summary>
    [MaxLength(50)]
    [Column(TypeName = "varchar(50)")]
    public string? Phone { get; set; }

    /// <summary>
    /// Phone Extension.
    /// (ç¹é?ä¸­æ?) ?†æ???
    /// </summary>
    [MaxLength(10)]
    [Column(TypeName = "varchar(10)")]
    public string? PhoneExt { get; set; }

    /// <summary>
    /// Fax.
    /// (ç¹é?ä¸­æ?) ?³ç???
    /// </summary>
    [MaxLength(50)]
    [Column(TypeName = "varchar(50)")]
    public string? Fax { get; set; }

    /// <summary>
    /// Fax Extension.
    /// (ç¹é?ä¸­æ?) ?³ç??†æ???
    /// </summary>
    [MaxLength(10)]
    [Column(TypeName = "varchar(10)")]
    public string? FaxExt { get; set; }

    /// <summary>
    /// Zip Code.
    /// (ç¹é?ä¸­æ?) ?µé??€?Ÿã€?
    /// </summary>
    [MaxLength(100)]
    [Column(TypeName = "varchar(100)")]
    public string? Zip { get; set; }

    /// <summary>
    /// Web Site.
    /// (ç¹é?ä¸­æ?) ç¶²ç???
    /// </summary>
    [MaxLength(255)]
    public string? WebSite { get; set; }

    /// <summary>
    /// Trade Term.
    /// (ç¹é?ä¸­æ?) è²¿æ?æ¢æ¬¾??
    /// </summary>
    [MaxLength(10)]
    [Column(TypeName = "varchar(10)")]
    public string? TradeTerm { get; set; }

    /// <summary>
    /// Shipment Type.
    /// (ç¹é?ä¸­æ?) ?‹è¼¸é¡å???
    /// </summary>
    [MaxLength(10)]
    [Column(TypeName = "varchar(10)")]
    public string? ShipmentType { get; set; }

    /// <summary>
    /// Service Type.
    /// (ç¹é?ä¸­æ?) ?å?é¡å???
    /// </summary>
    [MaxLength(10)]
    [Column(TypeName = "varchar(10)")]
    public string? ServiceType { get; set; }

    /// <summary>
    /// Air Move.
    /// (ç¹é?ä¸­æ?) ç©ºé?ç§»å???
    /// </summary>
    [MaxLength(10)]
    [Column(TypeName = "varchar(10)")]
    public string? AirMove { get; set; }

    /// <summary>
    /// Ocean Move.
    /// (ç¹é?ä¸­æ?) æµ·é?ç§»å???
    /// </summary>
    [MaxLength(10)]
    [Column(TypeName = "varchar(10)")]
    public string? OceanMove { get; set; }

    /// <summary>
    /// Air Line Code.
    /// (ç¹é?ä¸­æ?) ?ªç©º?¬å¸ä»?¢¼??
    /// </summary>
    [MaxLength(10)]
    [Column(TypeName = "varchar(10)")]
    public string? AirLineCode { get; set; }

    /// <summary>
    /// TP Letter Code.
    /// (ç¹é?ä¸­æ?) TP å­—æ?ä»?¢¼??
    /// </summary>
    [MaxLength(10)]
    [Column(TypeName = "varchar(10)")]
    public string? TPLetterCode { get; set; }

    /// <summary>
    /// Marks 1.
    /// (ç¹é?ä¸­æ?) ?œé ­ 1??
    /// </summary>
    [MaxLength(255)]
    public string? Marks1 { get; set; }

    /// <summary>
    /// Marks 2.
    /// (ç¹é?ä¸­æ?) ?œé ­ 2??
    /// </summary>
    [MaxLength(255)]
    public string? Marks2 { get; set; }

    /// <summary>
    /// Marks 3.
    /// (ç¹é?ä¸­æ?) ?œé ­ 3??
    /// </summary>
    [MaxLength(255)]
    public string? Marks3 { get; set; }

    /// <summary>
    /// Marks 4.
    /// (ç¹é?ä¸­æ?) ?œé ­ 4??
    /// </summary>
    [MaxLength(255)]
    public string? Marks4 { get; set; }

    /// <summary>
    /// Marks 5.
    /// (ç¹é?ä¸­æ?) ?œé ­ 5??
    /// </summary>
    [MaxLength(255)]
    public string? Marks5 { get; set; }

    /// <summary>
    /// Nature of Goods 1.
    /// (ç¹é?ä¸­æ?) è²¨ç‰©?§è³ª 1??
    /// </summary>
    [MaxLength(255)]
    public string? NatureofGoods1 { get; set; }

    /// <summary>
    /// Nature of Goods 2.
    /// (ç¹é?ä¸­æ?) è²¨ç‰©?§è³ª 2??
    /// </summary>
    [MaxLength(255)]
    public string? NatureofGoods2 { get; set; }

    /// <summary>
    /// Commodity.
    /// (ç¹é?ä¸­æ?) ?†å???
    /// </summary>
    [MaxLength(255)]
    public string? Commodity { get; set; }

    /// <summary>
    /// VAT.
    /// (ç¹é?ä¸­æ?) å¢å€¼ç???
    /// </summary>
    [MaxLength(10)]
    [Column(TypeName = "varchar(10)")]
    public string? VAT { get; set; }

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
    /// Is To ACS.
    /// (ç¹é?ä¸­æ?) ?¯å¦?³é€è‡³ ACS??
    /// </summary>
    public bool IsToACS { get; set; }

    /// <summary>
    /// City.
    /// (ç¹é?ä¸­æ?) ?å???
    /// </summary>
    [MaxLength(20)]
    public string? City { get; set; }

    /// <summary>
    /// State.
    /// (ç¹é?ä¸­æ?) å·åˆ¥/?ä»½??
    /// </summary>
    [MaxLength(20)]
    public string? State { get; set; }

    /// <summary>
    /// Exist Customer.
    /// (ç¹é?ä¸­æ?) ?¾æ?å®¢æˆ¶??
    /// </summary>
    public bool? ExistCustomer { get; set; }

    /// <summary>
    /// Created Station ID.
    /// (ç¹é?ä¸­æ?) å»ºç?ç«™é? ID??
    /// </summary>
    [MaxLength(3)]
    [Column(TypeName = "varchar(3)")]
    public string? CreatedStationID { get; set; }

    /// <summary>
    /// Vendor Posting GL Code.
    /// (ç¹é?ä¸­æ?) ä¾›æ??†é?å¸³ç¸½å¸³ä»£ç¢¼ã€?
    /// </summary>
    [MaxLength(10)]
    [Column(TypeName = "varchar(10)")]
    public string? VendorPostingGLCode { get; set; }

    /// <summary>
    /// Local Name.
    /// (ç¹é?ä¸­æ?) ?¶åœ°?ç¨±??
    /// </summary>
    [MaxLength(255)]
    public string? LocalName { get; set; }

    /// <summary>
    /// Lead Source ID.
    /// (ç¹é?ä¸­æ?) ç·šç´¢ä¾†æ? ID??
    /// </summary>
    public int? LeadSourceID { get; set; }

    /// <summary>
    /// Country.
    /// (ç¹é?ä¸­æ?) ?‹å®¶??
    /// </summary>
    [MaxLength(50)]
    public string? Country { get; set; }

    /// <summary>
    /// Industry Group ID.
    /// (ç¹é?ä¸­æ?) è¡Œæ¥­ç¾¤ç? ID??
    /// </summary>
    public int? IndustryGroupID { get; set; }

    /// <summary>
    /// Pay Term ID.
    /// (ç¹é?ä¸­æ?) ä»˜æ¬¾æ¢ä»¶ ID??
    /// </summary>
    public int? PayTermID { get; set; }

    /// <summary>
    /// Agent ID.
    /// (ç¹é?ä¸­æ?) ä»????ID??
    /// </summary>
    public int? AgentID { get; set; }

    /// <summary>
    /// Bill To Party.
    /// (ç¹é?ä¸­æ?) å¸³å–®å°è±¡??
    /// </summary>
    public int? BillToParty { get; set; }

    /// <summary>
    /// POD Flag.
    /// (ç¹é?ä¸­æ?) POD æ¨™è???
    /// </summary>
    public bool? PODFlag { get; set; }

    /// <summary>
    /// Freight Location.
    /// (ç¹é?ä¸­æ?) è²¨é??°é???
    /// </summary>
    [MaxLength(50)]
    public string? FreightLocation { get; set; }

    /// <summary>
    /// Is MNC (Multinational Corporation).
    /// (ç¹é?ä¸­æ?) ?¯å¦?ºè·¨?‹å…¬?¸ã€?
    /// </summary>
    public bool? IsMNC { get; set; }

    /// <summary>
    /// Customer Type.
    /// (ç¹é?ä¸­æ?) å®¢æˆ¶é¡å???
    /// </summary>
    [MaxLength(2)]
    [Column(TypeName = "varchar(2)")]
    public string? CustType { get; set; }

    /// <summary>
    /// Capital Currency ID.
    /// (ç¹é?ä¸­æ?) è³‡æœ¬å¹?ˆ¥ ID??
    /// </summary>
    public int? CapitalCurrencyID { get; set; }

    /// <summary>
    /// Capital Amount.
    /// (ç¹é?ä¸­æ?) è³‡æœ¬é¡ã€?
    /// </summary>
    [MaxLength(50)]
    [Column(TypeName = "varchar(50)")]
    public string? CapitalAmount { get; set; }

    /// <summary>
    /// Established Date.
    /// (ç¹é?ä¸­æ?) ?ç??¥æ???
    /// </summary>
    [MaxLength(50)]
    [Column(TypeName = "varchar(50)")]
    public string? EstablishedDate { get; set; }

    /// <summary>
    /// Annual Revenue.
    /// (ç¹é?ä¸­æ?) å¹´ç??¶ã€?
    /// </summary>
    [MaxLength(50)]
    [Column(TypeName = "varchar(50)")]
    public string? AnnualRevenue { get; set; }

    /// <summary>
    /// Is Vendor.
    /// (ç¹é?ä¸­æ?) ?¯å¦?ºä??‰å???
    /// </summary>
    public bool? IsVendor { get; set; }

    /// <summary>
    /// Customer Level.
    /// (ç¹é?ä¸­æ?) å®¢æˆ¶ç­‰ç???
    /// </summary>
    [MaxLength(20)]
    [Column(TypeName = "varchar(20)")]
    public string? CustomerLevel { get; set; }

    /// <summary>
    /// Latitude.
    /// (ç¹é?ä¸­æ?) ç·¯åº¦??
    /// </summary>
    public double? lat { get; set; }

    /// <summary>
    /// Longitude.
    /// (ç¹é?ä¸­æ?) ç¶“åº¦??
    /// </summary>
    public double? lng { get; set; }

    /// <summary>
    /// SA Files ID.
    /// (ç¹é?ä¸­æ?) SA æª”æ? ID??
    /// </summary>
    public int? SAFilesID { get; set; }

    /// <summary>
    /// Control Billing Party.
    /// (ç¹é?ä¸­æ?) ?§åˆ¶å¸³å–®å°è±¡??
    /// </summary>
    public bool? ControlBillingParty { get; set; }

    /// <summary>
    /// Is Credit Base On MNC.
    /// (ç¹é?ä¸­æ?) ä¿¡ç”¨?¯å¦?ºæ–¼è·¨å??¬å¸??
    /// </summary>
    public bool? IsCreditBaseOnMNC { get; set; }

    /// <summary>
    /// Is Billing Party Available Air.
    /// (ç¹é?ä¸­æ?) å¸³å–®å°è±¡?¯å¦?¯ç”¨?¼ç©º?‹ã€?
    /// </summary>
    public int? IsBillingPartyAvailableAir { get; set; }

    /// <summary>
    /// Is Billing Party Available Ocean.
    /// (ç¹é?ä¸­æ?) å¸³å–®å°è±¡?¯å¦?¯ç”¨?¼æµ·?‹ã€?
    /// </summary>
    public int? IsBillingPartyAvailableOcean { get; set; }

    /// <summary>
    /// Event Code.
    /// (ç¹é?ä¸­æ?) äº‹ä»¶ä»?¢¼??
    /// </summary>
    [MaxLength(50)]
    [Column(TypeName = "varchar(50)")]
    public string? EventCode { get; set; }

    /// <summary>
    /// Is Auto Print Invoice.
    /// (ç¹é?ä¸­æ?) ?¯å¦?ªå??—å°?¼ç¥¨??
    /// </summary>
    public bool? IsAutoPrintInvoice { get; set; }

    /// <summary>
    /// Disqualify Reason.
    /// (ç¹é?ä¸­æ?) ?–æ?è³‡æ ¼?Ÿå???
    /// </summary>
    public string? DisqualifyReason { get; set; }

    /// <summary>
    /// Is HubSpot Disqualify.
    /// (ç¹é?ä¸­æ?) ?¯å¦??HubSpot ?–æ?è³‡æ ¼??
    /// </summary>
    public bool? IsHubSpotDisqualify { get; set; }

    /// <summary>
    /// HubSpot Disqualify By.
    /// (ç¹é?ä¸­æ?) HubSpot ?–æ?è³‡æ ¼?·è??…ã€?
    /// </summary>
    [MaxLength(10)]
    [Column(TypeName = "varchar(10)")]
    public string? HubSpotDisqualifyBy { get; set; }

    /// <summary>
    /// HubSpot Disqualify Date.
    /// (ç¹é?ä¸­æ?) HubSpot ?–æ?è³‡æ ¼?¥æ???
    /// </summary>
    public DateTime? HubSpotDisqualifyDate { get; set; }

    /// <summary>
    /// Need Show SLAC As PCS.
    /// (ç¹é?ä¸­æ?) ?¯å¦?€è¦å? SLAC é¡¯ç¤º??PCS??
    /// </summary>
    public int? NeedShowSLACAsPCS { get; set; }

    /// <summary>
    /// Estimated Revenue.
    /// (ç¹é?ä¸­æ?) ä¼°è??Ÿæ”¶??
    /// </summary>
    [MaxLength(50)]
    [Column(TypeName = "varchar(50)")]
    public string? EstRevenue { get; set; }

    /// <summary>
    /// Remark.
    /// (ç¹é?ä¸­æ?) ?™è¨»??
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// Is Batch Milestone.
    /// (ç¹é?ä¸­æ?) ?¯å¦?ºæ‰¹æ¬¡é?ç¨‹ç???
    /// </summary>
    public bool? IsBatchMilestone { get; set; }
}
