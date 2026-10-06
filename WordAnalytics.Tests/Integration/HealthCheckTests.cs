using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using WordAnalytics.Counting.Interfaces;

namespace WordAnalytics.Tests.Integration
{
    public class HealthCheckTests(WordAnalyticsApiFactory factory) : IClassFixture<WordAnalyticsApiFactory>
    {
        private readonly HttpClient _client = factory.CreateClient();

        [Theory]
        [InlineData("/health/live")]
        [InlineData("/health/ready")]
        public async Task HealthEndpoint_WhenAppIsHealthy_Returns200AndHealthyStatus(string url)
        {
            // Act
            var response = await _client.GetAsync(url);

            var body = await response.Content.ReadFromJsonAsync<JsonElement>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("Healthy", body.GetProperty("status").GetString());
        }

        [Fact]
        public async Task Ready_WhenWordCounterThrows_Returns503_ButLiveStillReturns200()
        {
            // Arrange
            await using var failingFactory = new WordAnalyticsApiFactory()
                .WithServices(services =>
                {
                    services.RemoveAll<IWordCounter>();
                    services.AddSingleton<IWordCounter, ThrowingWordCounter>();
                });

            using var client = failingFactory.CreateClient();

            // Act
            var ready = await client.GetAsync("/health/ready");
            var live = await client.GetAsync("/health/live");
            var body = await ready.Content.ReadFromJsonAsync<JsonElement>();

            // Assert
            Assert.Equal(HttpStatusCode.ServiceUnavailable, ready.StatusCode);
            Assert.Equal("Unhealthy", body.GetProperty("status").GetString());
            Assert.Equal(HttpStatusCode.OK, live.StatusCode);
        }

        private sealed class ThrowingWordCounter : IWordCounter
        {
            // Act & Assert
            public Dictionary<string, int> CountWords(string text)
                => throw new InvalidOperationException("Simulated failure.");
        }
    }
}
