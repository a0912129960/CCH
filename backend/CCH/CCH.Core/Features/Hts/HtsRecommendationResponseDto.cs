using System.Text.Json.Serialization;

namespace CCH.Core.Features.Hts;

public class HtsRecommendationResponseDto
{
    [JsonPropertyName("input_hts_code")]
    public string InputHtsCode { get; set; } = string.Empty;

    [JsonPropertyName("matched_keyword")]
    public string? MatchedKeyword { get; set; }

    [JsonPropertyName("fallback_used")]
    public bool FallbackUsed { get; set; }

    [JsonPropertyName("message")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Message { get; set; }

    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public HtsRecommendationDataDto? Data { get; set; }
}

public class HtsRecommendationDataDto
{
    [JsonPropertyName("general")]
    public string? General { get; set; }

    [JsonPropertyName("special")]
    public string? Special { get; set; }

    [JsonPropertyName("other")]
    public string? Other { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}