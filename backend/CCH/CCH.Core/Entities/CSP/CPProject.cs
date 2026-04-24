using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CCH.Core.Entities.CSP;

[Table("CPProject")]
public partial class CpProject
{
    public int Id { get; set; }
    public string? ProjectName { get; set; }
    public string? ProjectDesc { get; set; }
    public string? AdminMail { get; set; }
    public string? Status { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? CreatedDt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedDt { get; set; }
    public int? NeedDetailMilestone { get; set; }
    public int? DetailContainer { get; set; }
    public int? DetailPo { get; set; }
    public int? DetailSo { get; set; }
    [Column("hasWarehouse")]
    public bool? HasWarehouse { get; set; }
    public bool? hasAir { get; set; }
    public bool? hasOcean { get; set; }
    public bool? hasTruck { get; set; }
    public bool? hasCourier { get; set; }
    public bool? hasCustomsClearance { get; set; }
    public string? courierStationID { get; set; }
    public bool? hasLoadPOBySetting { get; set; }
    public bool? IsAgent { get; set; }
}
