using System.Threading.Tasks;

namespace CCH.Core.Features.Hts.Interfaces;

public interface IHtsRecommendationService
{
    Task<HtsRecommendationResponseDto> GetRecommendationAsync(string htsCode);
}