using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CCH.Core.Entities.ReSm;

/// <summary>
/// Ported from MyDimerco. Represents a station in the ReSM database.
/// (繁體中文) 從 MyDimerco 移植。代表 ReSM 資料庫中的站點。
/// </summary>
[Table("SMStation")]
public partial class SmStation
    {
        [Key]
        public int HQID { get; set; }

        [Required]
        [StringLength(3)]
        public string StationID { get; set; }

        public int CustomerID { get; set; }

        [Required]
        [StringLength(6)]
        public string StationCode { get; set; }

        [StringLength(255)]
        public string StationName { get; set; }

        public int? CityID { get; set; }

        public int? DimRegionID { get; set; }

        public int? DimDistrictID { get; set; }

        public int? DimCountryID { get; set; }

        [StringLength(50)]
        public string StationType { get; set; }

        [StringLength(255)]
        public string StationName1 { get; set; }

        [StringLength(50)]
        public string StationAddress1 { get; set; }

        [StringLength(50)]
        public string StationAddress2 { get; set; }

        [StringLength(50)]
        public string StationAddress3 { get; set; }

        [StringLength(50)]
        public string StationAddress4 { get; set; }

        [StringLength(50)]
        public string StationAddress5 { get; set; }

        [StringLength(10)]
        public string HomeCurrency { get; set; }

        public int? HomeWeightUOMID { get; set; }

        public int? HomeDimensionUOMID { get; set; }

        [StringLength(10)]
        public string ReportCurrency { get; set; }

        public int? ReportWeightUOMID { get; set; }

        [StringLength(50)]
        public string StationPhone { get; set; }

        [StringLength(10)]
        public string StationPhoneExt { get; set; }

        [StringLength(50)]
        public string StationFax { get; set; }

        [StringLength(10)]
        public string StationFaxExt { get; set; }

        [StringLength(10)]
        public string StationZIP { get; set; }

        public DateTime? StartDate { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? MonthFrom { get; set; }

        public DateTime? ARDate { get; set; }

        public DateTime? APDate { get; set; }

        public DateTime? GLDate { get; set; }

        public DateTime? GLDate2 { get; set; }

        [StringLength(50)]
        public string GLNOARBank { get; set; }

        [StringLength(50)]
        public string GLNOAPBank { get; set; }

        [StringLength(50)]
        public string GLNOExch { get; set; }

        [StringLength(50)]
        public string GLNOUnexch { get; set; }

        [StringLength(50)]
        public string GLNOYE { get; set; }

        public DateTime? GLDateYE { get; set; }

        [StringLength(255)]
        public string SiteURL { get; set; }

        [StringLength(50)]
        public string IATACode { get; set; }

        [StringLength(50)]
        public string Registration1 { get; set; }

        [StringLength(50)]
        public string RegistrationName1 { get; set; }

        [StringLength(50)]
        public string Registration2 { get; set; }

        [StringLength(50)]
        public string RegistrationName2 { get; set; }

        [StringLength(50)]
        public string Registration3 { get; set; }

        [StringLength(50)]
        public string RegistrationName3 { get; set; }

        [StringLength(50)]
        public string Registration4 { get; set; }

        [StringLength(50)]
        public string RegistrationName4 { get; set; }

        [StringLength(50)]
        public string Registration5 { get; set; }

        [StringLength(50)]
        public string RegistrationName5 { get; set; }

        [Required]
        [StringLength(10)]
        public string Status { get; set; }

        [StringLength(6)]
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        [StringLength(6)]
        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] Version { get; set; }

        [StringLength(50)]
        public string DBName { get; set; }

        [StringLength(50)]
        public string StationIP { get; set; }

        [StringLength(10)]
        public string StationLevel { get; set; }

        [StringLength(3)]
        public string ParentStationID { get; set; }

        [StringLength(30)]
        public string Longitute { get; set; }

        [StringLength(30)]
        public string Latitude { get; set; }

        public int? TimeZoneID { get; set; }

        public int? FirstSeaportID { get; set; }

        public int? SecondSeaportID { get; set; }

        public int? ThirdSeaportID { get; set; }

        [Column(TypeName = "ntext")]
        public string InvoiceRemark { get; set; }

        [StringLength(50)]
        public string InboxName { get; set; }

        [StringLength(50)]
        public string eMailAcct { get; set; }

        [StringLength(100)]
        public string ProductLine { get; set; }

        public int? IsWMS { get; set; }

        [StringLength(50)]
        public string CubeDBName { get; set; }

        [StringLength(500)]
        public string OverdueService { get; set; }

        [StringLength(500)]
        public string TermsConditions { get; set; }

        public bool? IsPrintContact { get; set; }

        public int? GateWayCityID { get; set; }

        [StringLength(500)]
        public string ArrNoticeStandRemark { get; set; }

        [StringLength(500)]
        public string ArrNoticeUserRemark { get; set; }

        [Required]
        [StringLength(1)]
        public string UsedWMSSYS { get; set; }

        [Required]
        [StringLength(1)]
        public string UsedTMSSYS { get; set; }

        [StringLength(500)]
        public string LocalWebURL { get; set; }

        [StringLength(500)]
        public string HBLRemarks { get; set; }

        [Required]
        [StringLength(1)]
        public string TMSLanguage { get; set; }

        public bool? HandlingCode { get; set; }

        [StringLength(10)]
        public string LocalWebPort { get; set; }

        [Required]
        [StringLength(50)]
        public string FTPRoot { get; set; }

        public bool? ShowOriginCurrency { get; set; }

        [StringLength(100)]
        public string LocalWebURL2 { get; set; }

        [StringLength(10)]
        public string LocalWebPort2 { get; set; }

        [StringLength(100)]
        public string LocalWebURL3 { get; set; }

        [StringLength(10)]
        public string LocalWebPort3 { get; set; }

        [StringLength(100)]
        public string LocalWebURL4 { get; set; }

        [StringLength(10)]
        public string LocalWebPort4 { get; set; }

        public int? ReplicatedID { get; set; }

        public int? LERegion { get; set; }

        public int? LELevel1 { get; set; }

        public int? LELevel2 { get; set; }

        public int? IsUsedTaxed { get; set; }

        public int? IsUsedDraftInvoice { get; set; }

        public int? CreditRule { get; set; }

        public int? IsDutyException { get; set; }

        [StringLength(50)]
        public string ExternalReportGroup { get; set; }

        public bool? SeparatePLUnit { get; set; }

        public bool? iseCallFreight { get; set; }
        public bool? IsNeedPrintOutMultiPage { get; set; }
        public int? BondUsage { get; set; }
        public int? UseHBLToken { get; set; }

        public int? VWTTypeOfRounding { get; set; }

        public int? GWTTypeOfRounding { get; set; }

        public int? CWTTypeOfRounding { get; set; }

        public bool? IsAllowPrintOBL { get; set; }
    }
