using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CCH.Core.Entities.CSP;

/// <summary>
/// Entity for CCHLog table.
/// (ÁπÅÈ?‰∏≠Ê?) CCHLog Ë≥áÊ?Ë°®ÂØ¶È´î„Ä?
/// </summary>
[Table("CCHLog")]
public class CchLog
{
    /// <summary>
    /// ID (BIGINT, IDENTITY)
    /// </summary>
    [Key]
    public long ID { get; set; }

    /// <summary>
    /// Request data
    /// </summary>
    public string? Request { get; set; }

    /// <summary>
    /// Response data
    /// </summary>
    public string? Response { get; set; }

    /// <summary>
    /// HTTP Method (GET, POST, etc.)
    /// </summary>
    [MaxLength(10)]
    public string? HttpMethod { get; set; }

    /// <summary>
    /// Endpoint URL
    /// </summary>
    [MaxLength(2048)]
    public string? EndpointUrl { get; set; }

    /// <summary>
    /// Function Name
    /// </summary>
    [MaxLength(100)]
    public string? FunctionName { get; set; }

    /// <summary>
    /// HTTP Status Code
    /// </summary>
    public int? StatusCode { get; set; }

    /// <summary>
    /// Execution Time in milliseconds
    /// </summary>
    public int? ExecutionTime { get; set; }

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
