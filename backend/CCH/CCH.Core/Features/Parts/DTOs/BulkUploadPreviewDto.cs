using System.Collections.Generic;

namespace CCH.Core.Features.Parts.DTOs;

/// <summary>
/// Full preview result for bulk upload.
/// (繁體中文) 批次上傳的完整預覽結果。
/// </summary>
public class BulkUploadPreviewDto
{
    public BulkUploadSummaryDto Summary { get; set; } = null!;
    public List<PartPreviewRowDto> Rows { get; set; } = new();
}
