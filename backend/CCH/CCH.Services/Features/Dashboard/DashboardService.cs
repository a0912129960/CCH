using CCH.Core.Features.Dashboard.DTOs;
using CCH.Core.Features.Dashboard.Interfaces;
using CCH.Core.Interfaces.Repositories;

namespace CCH.Services.Features.Dashboard;

/// <summary>
/// Dashboard service — queries real data from IPartRepository.
/// (繁體中文) 儀表板服務，從 IPartRepository 查詢真實資料。
/// </summary>
public class DashboardService : IDashboardService
{
    private readonly IPartRepository _partRepository;
    private readonly ICommonRepository _commonRepository;

    public DashboardService(IPartRepository partRepository, ICommonRepository commonRepository)
    {
        _partRepository = partRepository;
        _commonRepository = commonRepository;
    }

    /// <inheritdoc/>
    public PartStatusSummaryDto GetStatusSummary(string? customerId)
    {
        var parsedId = TryParseCustomerId(customerId);
        var all = _partRepository.SearchParts(parsedId, null, null, null).ToList();

        return new PartStatusSummaryDto
        {
            S01 = all.Count(p => p.Status == "S01"),
            S02 = all.Count(p => p.Status == "S02"),
            S03 = all.Count(p => p.Status == "S03"),
            S04 = all.Count(p => p.Status == "S04"),
            S05 = all.Count(p => p.Status == "S05")
        };
    }

    /// <inheritdoc/>
    public IEnumerable<PendingReviewDto> GetPendingReviews(string? customerId, string? role = null)
    {
        var parsedId = TryParseCustomerId(customerId);

        // Determine which statuses to query based on the caller's role.
        var statuses = string.Equals(role, "CUSTOMER", StringComparison.OrdinalIgnoreCase)
            ? new[] { "S01", "S03" }
            : new[] { "S02" };

        // Build customer lookup map using SmCustomer properties (HQID, CustomerName)
        var customerMap = _commonRepository.GetCustomers()
            .ToDictionary(c => c.HQID, c => c.CustomerName ?? string.Empty);

        // Query each status separately and merge
        var parts = statuses
            .SelectMany(s => _partRepository.SearchParts(parsedId, s, null, null))
            .ToList();

        return parts.Select(p => new PendingReviewDto
        {
            Id          = p.ID,
            Customer    = customerMap.GetValueOrDefault(p.CustomerID ?? 0, string.Empty),
            PartNo      = p.PartNo ?? string.Empty,
            PartDesc    = p.PartDescription ?? string.Empty,
            HtsCode     = p.HTSCode ?? string.Empty,
            Status      = p.Status ?? string.Empty,
            UpdatedBy   = p.UpdatedBy ?? string.Empty,
            UpdatedDate = p.UpdatedDate ?? DateTime.MinValue,
            SlaStatus   = CalculateSlaStatus(p.UpdatedDate)
        });
    }

    /// <summary>
    /// Derives SLA urgency from the number of days a part has been waiting.
    /// (依零件等待天數計算 SLA 緊急程度。)
    /// </summary>
    private static string CalculateSlaStatus(DateTime? updatedDate)
    {
        if (!updatedDate.HasValue) return "green";
        
        var daysPending = (DateTime.Now - updatedDate.Value).TotalDays;
        return daysPending switch
        {
            < 3  => "green",
            < 7  => "yellow",
            _    => "red"
        };
    }

    /// <summary>
    /// Parses a string customerId to int?. Returns null for null / "all" / non-numeric values.
    /// (將字串 customerId 解析為 int?；null / "all" / 非數字一律回傳 null。)
    /// </summary>
    private static int? TryParseCustomerId(string? customerId) =>
        int.TryParse(customerId, out var id) ? id : null;
}
