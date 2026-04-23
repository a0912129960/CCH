using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CCH.Core.Features.Hts;
using CCH.Core.Features.Hts.Interfaces;
using Microsoft.Extensions.Logging;

namespace CCH.Services.Features.Hts;

public class HtsRecommendationService : IHtsRecommendationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HtsRecommendationService> _logger;

    public HtsRecommendationService(HttpClient httpClient, ILogger<HtsRecommendationService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<HtsRecommendationResponseDto> GetRecommendationAsync(string htsCode)
    {
        if (string.IsNullOrWhiteSpace(htsCode) || htsCode.Length != 10 || !Regex.IsMatch(htsCode, @"^\d+$"))
        {
            throw new ArgumentException("HTS Code must be exactly 10 numeric digits");
        }

        _logger.LogInformation("Getting HTS recommendation for input code: {HtsCode}", htsCode);

        // Step 1: query 10-digit input code
        var result = await FetchFromExternalApiAsync(htsCode);

        // Empty [] means the code doesn't exist in USITC — don't attempt 8-digit fallback.
        if (!IsNonEmptyResponse(result))
        {
            _logger.LogWarning("USITC returned empty [] for input code: {HtsCode}. No fallback attempted.", htsCode);
            return new HtsRecommendationResponseDto
            {
                InputHtsCode = htsCode,
                MatchedKeyword = null,
                FallbackUsed = false,
                Message = "No recommendation data"
            };
        }

        // Initial code returned data — check if general is present.
        if (HasGeneralValue(result))
        {
            return BuildResponse(htsCode, htsCode, false, result.Value);
        }

        // Step 2: initial has data but no valid general → try 8-digit fallback.
        if (htsCode.Length > 8)
        {
            var code8 = htsCode.Substring(0, 8);
            _logger.LogInformation("No general value for {HtsCode}, falling back to 8 digits: {Code8}", htsCode, code8);
            var fallbackResult = await FetchFromExternalApiAsync(code8);

            if (HasGeneralValue(fallbackResult))
            {
                return BuildResponse(htsCode, code8, true, fallbackResult.Value);
            }
        }

        // Step 3: no usable general found
        _logger.LogWarning("No general duty rate found for input code: {HtsCode}", htsCode);
        return new HtsRecommendationResponseDto
        {
            InputHtsCode = htsCode,
            MatchedKeyword = null,
            FallbackUsed = true,
            Message = "No recommendation data"
        };
    }

    private async Task<JsonElement?> FetchFromExternalApiAsync(string keyword)
    {
        try
        {
            var url = $"https://hts.usitc.gov/reststop/search?keyword={keyword}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(jsonString);

            // The USITC API often returns an array or an object containing an array.
            // For resilience, we return the parsed root element and find "general" inside.
            return doc.RootElement.Clone();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "USITC API request failed for keyword: {Keyword}", keyword);
            throw new Exception("USITC API request failed", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse JSON response from USITC API for keyword: {Keyword}", keyword);
            throw new Exception("USITC API response parsing failed", ex);
        }
    }

    private static bool IsNonEmptyResponse(JsonElement? element)
    {
        if (!element.HasValue) return false;
        var el = element.Value;
        // USITC returns [] when the code has no match — treat as "no data".
        return !(el.ValueKind == JsonValueKind.Array && el.GetArrayLength() == 0);
    }

    private bool HasGeneralValue(JsonElement? element)
    {
        if (!element.HasValue) return false;

        // Recursively search for a valid "general" property
        return FindValidGeneralProperty(element.Value) != null;
    }

    private HtsRecommendationResponseDto BuildResponse(string inputCode, string matchedCode, bool fallbackUsed, JsonElement rawResult)
    {
        var generalValue = FindValidGeneralProperty(rawResult);
        var specialValue = FindProperty(rawResult, "special");
        var otherValue = FindProperty(rawResult, "other");
        var descriptionValue = FindProperty(rawResult, "description");

        return new HtsRecommendationResponseDto
        {
            InputHtsCode = inputCode,
            MatchedKeyword = matchedCode,
            FallbackUsed = fallbackUsed,
            Data = new HtsRecommendationDataDto
            {
                General = generalValue,
                Special = specialValue,
                Other = otherValue,
                Description = descriptionValue
            }
        };
    }

    private string? FindValidGeneralProperty(JsonElement element)
    {
        if (element.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in element.EnumerateArray())
            {
                var result = FindValidGeneralProperty(item);
                if (result != null) return result;
            }
        }
        else if (element.ValueKind == JsonValueKind.Object)
        {
            if (element.TryGetProperty("general", out var generalProp) && 
                generalProp.ValueKind == JsonValueKind.String && 
                !string.IsNullOrWhiteSpace(generalProp.GetString()))
            {
                return generalProp.GetString();
            }

            foreach (var prop in element.EnumerateObject())
            {
                var result = FindValidGeneralProperty(prop.Value);
                if (result != null) return result;
            }
        }

        return null;
    }

    private string? FindProperty(JsonElement element, string propertyName)
    {
        if (element.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in element.EnumerateArray())
            {
                var result = FindProperty(item, propertyName);
                if (result != null) return result;
            }
        }
        else if (element.ValueKind == JsonValueKind.Object)
        {
            if (element.TryGetProperty(propertyName, out var prop) && prop.ValueKind == JsonValueKind.String)
            {
                var value = prop.GetString();
                if (!string.IsNullOrWhiteSpace(value)) return value;
            }

            foreach (var objProp in element.EnumerateObject())
            {
                var result = FindProperty(objProp.Value, propertyName);
                if (result != null) return result;
            }
        }

        return null;
    }
}