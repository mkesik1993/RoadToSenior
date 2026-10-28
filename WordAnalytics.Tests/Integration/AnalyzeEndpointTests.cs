using System.Net;
using System.Net.Http.Json;
using WordAnalytics.Api.Contracts.Requests;
using WordAnalytics.Api.Contracts.Responses;

namespace WordAnalytics.Tests.Integration
{
    public class AnalyzeEndpointTests(WordAnalyticsApiFactory factory) : IClassFixture<WordAnalyticsApiFactory>
    {
        private readonly HttpClient _client = factory.CreateClient();

        [Fact]
        public async Task Analyze_WithValidText_Returns200AndTopWords()
        {
            // Arrange
            var request = new AnalyzeRequest { Text = "Hello, hello world! Code code code." };

            // Act
            var httpResponse = await _client.PostAsJsonAsync("/api/analyze", request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);

            var json = await httpResponse.Content.ReadAsStringAsync();

            await Verifier.VerifyJson(json);
        }

        [Fact]
        public async Task Analyze_WithValidText_ReturnsExpectedJsonStructure()
        {
            // Arrange
            var request = new AnalyzeRequest { Text = "Hello, hello world! Code code code." };

            // Act
            var httpResponse = await _client.PostAsJsonAsync("/api/analyze", request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);

            var json = await httpResponse.Content.ReadAsStringAsync();

            await Verifier.VerifyJson(json)
                .IgnoreMembers("count");
        }

        [Fact]
        public async Task Analyze_WithTopCountOverride_ReturnsRequestedNumberOfWords()
        {
            // Arrange
            var request = new AnalyzeRequest { Text = "a a b b c c d", TopCount = 2 };

            // Act
            var httpResponse = await _client.PostAsJsonAsync("/api/analyze", request);
            var response = await httpResponse.Content
                .ReadFromJsonAsync<AnalyzeResponse>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
            Assert.Equal(2, response!.Top.Count);
        }


    }
}
