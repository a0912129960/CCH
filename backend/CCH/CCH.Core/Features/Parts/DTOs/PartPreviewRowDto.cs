using System.Collections.Generic;

namespace CCH.Core.Features.Parts.DTOs;

/// <summary>
/// Comparison result for a single row in bulk upload.
/// (繁體中文) 批次上傳中單列的比對結果。
/// </summary>
public class PartPreviewRowDto
{
    public int RowIndex { get; set; }
    public string RowStatus { get; set; } = string.Empty; // New, Modified, Error, NoChange
    public List<string> Errors { get; set; } = new();
    public PartDto NewData { get; set; } = null!;
    public PartDto? OriginalData { get; set; }
}
