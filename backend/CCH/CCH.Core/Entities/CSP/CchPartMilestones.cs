using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CCH.Core.Entities.CSP;

/// <summary>
/// Entity for CCHPartMilestones table.
/// (ÁπÅÈ?‰∏≠Ê?) CCHPartMilestones Ë≥áÊ?Ë°®ÂØ¶È´î„Ä?
/// </summary>
[Table("CCHPartMilestones")]
public class CchPartMilestones
{
    /// <summary>
    /// ID (IDENTITY)
    /// </summary>
    [Key]
    public int ID { get; set; }

    /// <summary>
    /// Associated Part ID
    /// </summary>
    public int? PartID { get; set; }

    /// <summary>
    /// Action performed
    /// </summary>
    [MaxLength(20)]
    public string? Action { get; set; }

    /// <summary>
    /// Previous status
    /// </summary>
    [MaxLength(10)]
    public string? FromStatus { get; set; }

    /// <summary>
    /// Target status
    /// </summary>
    [MaxLength(10)]
    public string? ToStatus { get; set; }

    /// <summary>
    /// Milestone remark
    /// </summary>
    [MaxLength(2000)]
    public string? Remark { get; set; }

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
