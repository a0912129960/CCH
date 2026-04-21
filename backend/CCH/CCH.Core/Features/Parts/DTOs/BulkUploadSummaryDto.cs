namespace CCH.Core.Features.Parts.DTOs;

/// <summary>
/// Summary information for bulk upload.
/// (繁體中文) 批次上傳的摘要資訊。
/// </summary>
public class BulkUploadSummaryDto
{
    public int TotalRows { get; set; }
    public int NewCount { get; set; }
    public int ModifiedCount { get; set; }
    public int ErrorCount { get; set; }
    public int NoChangeCount { get; set; }
}
