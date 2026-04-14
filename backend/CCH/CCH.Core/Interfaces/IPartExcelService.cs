using CCH.Core.DTOs;

namespace CCH.Core.Interfaces;

/// <summary>
/// Part Excel operation service interface.
/// (繁體中文) 零件 Excel 操作服務介面。
/// </summary>
public interface IPartExcelService
{
    byte[] ExportParts(string? customerId, string? status, string? partNo, string? supplier);
    List<BulkUploadResponseDto> BulkUpload(Stream fileStream);
    byte[] GetUploadTemplate();
}
