using CCH.Core.DTOs;
using CCH.Core.Interfaces;
using CCH.Core.Interfaces.Repositories;

namespace CCH.Services.Services;

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
        _repository.GetCustomers().Select(c => new KeyValuePairDto { Key = c.ID.ToString(), Value = c.Name });

    public IEnumerable<KeyValuePairDto> GetCountries() => 
        _repository.GetCountries().Select(c => new KeyValuePairDto { Key = c.Code, Value = c.Name });

    public IEnumerable<KeyValuePairDto> GetSuppliers(string? customerId) => new[]
    {
        new KeyValuePairDto { Key = "S001", Value = "Supplier X" },
        new KeyValuePairDto { Key = "S002", Value = "Supplier Y" }
    };

    public IEnumerable<KeyValuePairDto> GetStatus() => 
        _repository.GetStatuses().Select(s => new KeyValuePairDto { Key = s.Code, Value = s.Description });
}
