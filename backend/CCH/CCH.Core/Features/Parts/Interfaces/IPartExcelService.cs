using CCH.Core.Features.Parts.DTOs;

namespace CCH.Core.Features.Parts.Interfaces;

/// <summary>
/// Part Excel operation service interface.
/// (繁體中文) 零件 Excel 操作服務介面。
/// </summary>
public interface IPartExcelService
{
    byte[] ExportParts(int? customerId, string? status, string? partNo, int? supplierId);

    /// <summary>
    /// Previews the bulk upload from an Excel stream.
    /// (繁體中文) 從 Excel 串流預覽批次上傳。
    /// </summary>
    BulkUploadPreviewDto PreviewBulkUpload(int customerId, Stream fileStream);

    /// <summary>
    /// Confirms and persists the uploaded parts.
    /// (繁體中文) 確認並持久化上傳的零件。
    /// </summary>
    BulkUploadConfirmResponseDto ConfirmBulkUpload(List<PartDto> parts);

    byte[] GetUploadTemplate();
}
