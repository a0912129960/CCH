using System.Collections.Generic;

namespace CCH.Core.Features.Parts.DTOs;

/// <summary>
/// Response for bulk upload confirmation.
/// (繁體中文) 批次上傳確認後的回應。
/// </summary>
public class BulkUploadConfirmResponseDto
{
    public int Inserted { get; set; }
    public int Updated { get; set; }
    public int Failed { get; set; }
    public List<string> Errors { get; set; } = new();
}
