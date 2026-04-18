using CCH.Core.Features.Parts.DTOs;

namespace CCH.Core.Features.Parts.Interfaces;

/// <summary>
/// Part Excel operation service interface.
/// (繁體中文) 零件 Excel 操作服務介面。
/// </summary>
public interface IPartExcelService
{
    byte[] ExportParts(int? customerId, string? status, string? partNo, int? supplierId);
    List<BulkUploadResponseDto> BulkUpload(Stream fileStream);
    byte[] GetUploadTemplate();
}
