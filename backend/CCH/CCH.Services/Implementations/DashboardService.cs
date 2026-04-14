using CCH.Core.DTOs;
using CCH.Core.Interfaces;

namespace CCH.Services.Implementations;

/// <summary>
/// Mock dashboard service.
/// (繁體中文) 模擬儀表板服務。
/// </summary>
public class DashboardService : IDashboardService
{
    public PartStatusSummaryDto GetStatusSummary(string? customerId) => new()
    {
        S01 = 10, S02 = 5, S03 = 2, S04 = 50, S05 = 3
    };

    public IEnumerable<PendingReviewDto> GetPendingReviews(string? customerId) => new[]
    {
        new PendingReviewDto
        {
            Customer = "Customer A",
            PartNo = "P001",
            PartDesc = "Description for P001",
            HtsCode = "8471.30",
            Status = "S02",
            UpdatedBy = "User X",
            UpdatedDate = DateTime.Now.AddDays(-1),
            SlaStatus = "yellow"
        },
        new PendingReviewDto
        {
            Customer = "Customer B",
            PartNo = "P002",
            PartDesc = "Description for P002",
            HtsCode = "8471.41",
            Status = "S01",
            UpdatedBy = "User Y",
            UpdatedDate = DateTime.Now.AddHours(-2),
            SlaStatus = "green"
        }
    };
}
