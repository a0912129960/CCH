using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CCH.Services.Features.Hts;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Xunit;

namespace CCH.Tests;

public class HtsRecommendationServiceTests
{
    [Fact]
    public async Task GetRecommendationAsync_InvalidCode_ThrowsArgumentException()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<HtsRecommendationService>>();
        var service = new HtsRecommendationService(new HttpClient(), mockLogger.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.GetRecommendationAsync("123456789")); // Not 10 digits
        await Assert.ThrowsAsync<ArgumentException>(() => service.GetRecommendationAsync("ABCDEFGHIJ")); // Non-numeric
    }

    [Fact]
    public async Task GetRecommendationAsync_Valid10Digit_ReturnsData()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<HtsRecommendationService>>();
        var jsonResponse = @"{
            ""results"": [
                { ""general"": ""5.3%"", ""special"": ""Free"", ""other"": ""25%"", ""description"": ""Springs"" }
            ]
        }";

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse)
            });

        var client = new HttpClient(handlerMock.Object);
        var service = new HtsRecommendationService(client, mockLogger.Object);

        // Act
        var result = await service.GetRecommendationAsync("7320905060");

        // Assert
        Assert.False(result.FallbackUsed);
        Assert.Equal("7320905060", result.InputHtsCode);
        Assert.Equal("7320905060", result.MatchedKeyword);
        Assert.NotNull(result.Data);
        Assert.Equal("5.3%", result.Data.General);
    }
}