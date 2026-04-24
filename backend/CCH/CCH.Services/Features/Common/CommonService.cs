using CCH.Core.Features.Common.DTOs;
using CCH.Core.Features.Common.Interfaces;
using CCH.Core.Interfaces;
using CCH.Core.Interfaces.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace CCH.Services.Features.Common;

/// <summary>
/// Common data service implementation with permission-based filtering and caching.
/// (繁體中文) 具備權限過濾與快取機制的共用資料服務實作。
/// </summary>
public class CommonService : ICommonService
{
    private readonly ICommonRepository _repository;
    private readonly IUserContext _userContext;
    private readonly IMemoryCache _cache;
    private const string ProjectsCacheKeyPrefix = "Projects_";

    public CommonService(ICommonRepository repository, IUserContext userContext, IMemoryCache cache)
    {
        _repository = repository;
        _userContext = userContext;
        _cache = cache;
    }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePairDto> GetProjects()
    {
        string userId = _userContext.UserId ?? "anonymous";
        string cacheKey = $"{ProjectsCacheKeyPrefix}{userId}";

        var projects = _cache.GetOrCreate(cacheKey, entry =>
        {
            // Set cache strategy (設定快取策略)
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
            
            return _repository.GetProjects(_userContext.UserId, _userContext.Role)
                .Select(p => new KeyValuePairDto { Key = p.Id.ToString(), Value = p.ProjectName ?? "Unknown" })
                .ToList();
        });

        return projects ?? Enumerable.Empty<KeyValuePairDto>();
    }

    public IEnumerable<KeyValuePairDto> GetCountries() => 
        _repository.GetCountries().Select(c => new KeyValuePairDto { Key = c.ID.ToString(), Value = c.Name });

    public IEnumerable<KeyValuePairDto> GetSuppliers(string? projectId)
    {
        int? pId = null;
        if (int.TryParse(projectId, out int parsedId)) pId = parsedId;

        return _repository.GetSuppliers(pId).Select(s => new KeyValuePairDto { Key = s.ID.ToString(), Value = s.SupplierName ?? "Unknown" });
    }

    public IEnumerable<KeyValuePairDto> GetStatus() => 
        _repository.GetStatuses().Select(s => new KeyValuePairDto { Key = s.Code, Value = s.Description });

    /// <inheritdoc/>
    public string GetUserName(string userId) => _repository.GetUserName(userId);

    /// <inheritdoc/>
    public Dictionary<string, string> GetUserNames(IEnumerable<string> userIds) => _repository.GetUserNames(userIds);
}
