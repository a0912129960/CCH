namespace CCH.Core.Entities;

/// <summary>
/// Part entity matching the target SQL schema.
/// (繁體中文) 符合目標 SQL 結構的零件實體。
/// </summary>
public class PartEntity
{
    public int ID { get; set; }
    public int CustomerID { get; set; }
    public string PartNo { get; set; } = string.Empty;
    public int CountryID { get; set; }
    public string PartDescription { get; set; } = string.Empty;
    public string Division { get; set; } = string.Empty;
    public string Supplier { get; set; } = string.Empty;
    public string HTSCode { get; set; } = string.Empty;
    public decimal DutyRate { get; set; }
    public string? AddHTSCode1 { get; set; }
    public decimal? AddDutyRate1 { get; set; }
    public string? AddHTSCode2 { get; set; }
    public decimal? AddDutyRate2 { get; set; }
    public string? AddHTSCode3 { get; set; }
    public decimal? AddDutyRate3 { get; set; }
    public string? AddHTSCode4 { get; set; }
    public decimal? AddDutyRate4 { get; set; }
    public string Remark { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public string UpdatedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public string? SlaStatus { get; set; }
}
