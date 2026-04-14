using CCH.Core.DTOs;
using CCH.Core.Interfaces;

namespace CCH.Services.Implementations;

/// <summary>
/// Mock common service.
/// (繁體中文) 模擬共用資料服務。
/// </summary>
public class CommonService : ICommonService
{
    public IEnumerable<KeyValuePairDto> GetCustomers() => new[]
    {
        new KeyValuePairDto { Key = "C001", Value = "Customer A" },
        new KeyValuePairDto { Key = "C002", Value = "Customer B" }
    };

    public IEnumerable<KeyValuePairDto> GetCountries() => new[]
    {
        new KeyValuePairDto { Key = "CN", Value = "China" },
        new KeyValuePairDto { Key = "TW", Value = "Taiwan" },
        new KeyValuePairDto { Key = "US", Value = "USA" }
    };

    public IEnumerable<KeyValuePairDto> GetSuppliers(string? customerId) => new[]
    {
        new KeyValuePairDto { Key = "S001", Value = "Supplier X" },
        new KeyValuePairDto { Key = "S002", Value = "Supplier Y" }
    };

    public IEnumerable<KeyValuePairDto> GetStatus() => new[]
    {
        new KeyValuePairDto { Key = "S01", Value = "New" },
        new KeyValuePairDto { Key = "S02", Value = "Pending Review" },
        new KeyValuePairDto { Key = "S03", Value = "Returned" },
        new KeyValuePairDto { Key = "S04", Value = "Accepted" },
        new KeyValuePairDto { Key = "S05", Value = "Inactive" }
    };
}
