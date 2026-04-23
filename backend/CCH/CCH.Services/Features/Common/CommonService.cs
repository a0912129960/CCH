using CCH.Core.Features.Common.DTOs;
using CCH.Core.Features.Common.Interfaces;
using CCH.Core.Interfaces.Repositories;

namespace CCH.Services.Features.Common;

/// <summary>
/// Mock common service.
/// (繁體中文) 模擬共用資料服務。
/// </summary>
public class CommonService : ICommonService
{
    private readonly ICommonRepository _repository;

    public CommonService(ICommonRepository repository)
    {
        _repository = repository;
    }

    public IEnumerable<KeyValuePairDto> GetCustomers() =>
        _repository.GetCustomers().Select(c => new KeyValuePairDto { Key = c.HQID.ToString(), Value = c.CustomerName ?? "Unknown" });
    public IEnumerable<KeyValuePairDto> GetCountries() => 
        _repository.GetCountries().Select(c => new KeyValuePairDto { Key = c.ID.ToString(), Value = c.Name });

    public IEnumerable<KeyValuePairDto> GetSuppliers(string? customerId)
    {
        int? cId = null;
        if (int.TryParse(customerId, out int parsedId)) cId = parsedId;

        return _repository.GetSuppliers(cId).Select(s => new KeyValuePairDto { Key = s.ID.ToString(), Value = s.SupplierName ?? "Unknown" });
    }

    public IEnumerable<KeyValuePairDto> GetStatus() => 
        _repository.GetStatuses().Select(s => new KeyValuePairDto { Key = s.Code, Value = s.Description });

    /// <inheritdoc/>
    public string GetUserName(string userId) => _repository.GetUserName(userId);

    /// <inheritdoc/>
    public Dictionary<string, string> GetUserNames(IEnumerable<string> userIds) => _repository.GetUserNames(userIds);
}
