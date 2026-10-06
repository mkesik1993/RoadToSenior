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
            var request = new AnalyzeRequest { Text = "Hello, hello world! Code code code." };

            var httpResponse = await _client.PostAsJsonAsync("/api/analyze", request);

            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);

            var response = await httpResponse.Content
                .ReadFromJsonAsync<AnalyzeResponse>();

            Assert.NotNull(response);
            Assert.Equal(3, response.Top.Count);

            Assert.Equal("code", response.Top[0].Word);
            Assert.Equal(3, response.Top[0].Count);
            Assert.Equal("hello", response.Top[1].Word);
            Assert.Equal(2, response.Top[1].Count);
        }

        [Fact]
        public async Task Analyze_WithTopCountOverride_ReturnsRequestedNumberOfWords()
        {
            var request = new AnalyzeRequest { Text = "a a b b c c d", TopCount = 2 };

            var httpResponse = await _client.PostAsJsonAsync("/api/analyze", request);
            var response = await httpResponse.Content
                .ReadFromJsonAsync<AnalyzeResponse>();

            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
            Assert.Equal(2, response!.Top.Count);
        }
    }
}
