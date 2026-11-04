using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using WordAnalytics.Api.Features.AnalyzeText;

namespace WordAnalytics.Tests.Integration
{
    public class ConfigurationTests
    {
        [Theory]
        [InlineData("0")]
        [InlineData("1001")]
        public async Task CreateClient_WithInvalidConfiguredTopCount_RejectsStartup(string topCount)
        {
            // Arrange
            await using var factory = new WordAnalyticsApiFactory()
                .WithConfiguration("Ranking:TopCount", topCount);

            // Act
            var createClient = () => factory.CreateClient();

            // Assert
            Assert.Throws<OptionsValidationException>(createClient);
        }

        [Fact]
        public async Task TopCount_DefaultsToConfiguredValue_WhenNotProvidedInRequest()
        {
            // Arrange
            await using var factory = new WordAnalyticsApiFactory()
                .WithConfiguration("Ranking:TopCount", "2");
            using var client = factory.CreateClient();

            // Act
            var httpResponse = await client.PostAsJsonAsync("/api/analyze",
                new AnalyzeRequest { Text = "a a b b c c d d e" });

            var response = await httpResponse.Content
                .ReadFromJsonAsync<AnalyzeResponse>();

            // Assert
            Assert.Equal(2, response!.Top.Count);
        }

        [Fact]
        public async Task RequestTopCount_OverridesConfiguration()
        {
            // Arrange
            await using var factory = new WordAnalyticsApiFactory()
                .WithConfiguration("Ranking:TopCount", "2");

            using var client = factory.CreateClient();

            // Act
            var httpResponse = await client.PostAsJsonAsync("/api/analyze",
                new AnalyzeRequest { Text = "a a b b c c d d e", TopCount = 4 });

            var response = await httpResponse.Content
                .ReadFromJsonAsync<AnalyzeResponse>();

            // Assert
            Assert.Equal(4, response!.Top.Count);
        }
    }
}
