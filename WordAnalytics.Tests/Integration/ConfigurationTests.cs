using System.Net.Http.Json;
using WordAnalytics.Api.Contracts.Requests;
using WordAnalytics.Api.Contracts.Responses;

namespace WordAnalytics.Tests.Integration
{
    public class ConfigurationTests
    {
        [Fact]
        public async Task TopCount_DefaultsToConfiguredValue_WhenNotProvidedInRequest()
        {
            await using var factory = new WordAnalyticsApiFactory()
                .WithConfiguration("Ranking:TopCount", "2");

            using var client = factory.CreateClient();

            var httpResponse = await client.PostAsJsonAsync("/api/analyze",
                new AnalyzeRequest { Text = "a a b b c c d d e" });

            var response = await httpResponse.Content
                .ReadFromJsonAsync<AnalyzeResponse>();

            Assert.Equal(2, response!.Top.Count);
        }

        [Fact]
        public async Task RequestTopCount_OverridesConfiguration()
        {
            using var factory = new WordAnalyticsApiFactory()
                .WithConfiguration("Ranking:TopCount", "2");

            using var client = factory.CreateClient();

            var httpResponse = await client.PostAsJsonAsync("/api/analyze",
                new AnalyzeRequest { Text = "a a b b c c d d e", TopCount = 4 });

            var response = await httpResponse.Content
                .ReadFromJsonAsync<AnalyzeResponse>();

            Assert.Equal(4, response!.Top.Count);
        }
    }
}
