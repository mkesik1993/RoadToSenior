using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using WordAnalytics.Api.Contracts.Requests;

namespace WordAnalytics.Tests.Integration
{
    public class RateLimitingTests
    {
        private static WordAnalyticsApiFactory CreateFactory(int permitLimit)
            => new WordAnalyticsApiFactory()
                .WithConfiguration("RateLimiting:PermitLimit", permitLimit.ToString())
                .WithConfiguration("RateLimiting:WindowSeconds", "3600");

        private static Task<HttpResponseMessage> PostAnalyze(HttpClient client)
            => client.PostAsJsonAsync("/api/analyze", new AnalyzeRequest { Text = "hello world" });

        [Fact]
        public async Task Analyze_WithinLimit_Returns200()
        {
            // Arrange
            await using var factory = CreateFactory(permitLimit: 2);
            using var client = factory.CreateClient();

            // Act
            var first = await PostAnalyze(client);
            var second = await PostAnalyze(client);

            // Assert
            Assert.Equal(HttpStatusCode.OK, first.StatusCode);
            Assert.Equal(HttpStatusCode.OK, second.StatusCode);
        }

        [Fact]
        public async Task Analyze_WhenLimitExceeded_Returns429ProblemDetailsWithRetryAfter()
        {
            // Arrange
            await using var factory = CreateFactory(permitLimit: 2);
            using var client = factory.CreateClient();

            // Act
            await PostAnalyze(client);
            await PostAnalyze(client);
            var rejected = await PostAnalyze(client);
            var problem = await rejected.Content.ReadFromJsonAsync<JsonElement>();

            // Assert
            Assert.Equal(HttpStatusCode.TooManyRequests, rejected.StatusCode);
            Assert.True(rejected.Headers.Contains("Retry-After"));
            Assert.Equal("application/problem+json", rejected.Content.Headers.ContentType?.MediaType);

            Assert.Equal(429, problem.GetProperty("status").GetInt32());
            Assert.Equal("Too many requests.", problem.GetProperty("title").GetString());
        }

        [Fact]
        public async Task HealthEndpoints_AreNotRateLimited()
        {
            // Arrange
            await using var factory = CreateFactory(permitLimit: 1);
            using var client = factory.CreateClient();

            // Act
            await PostAnalyze(client);

            // Assert
            Assert.Equal(HttpStatusCode.TooManyRequests, (await PostAnalyze(client)).StatusCode);

            for (var i = 0; i < 5; i++)
            {
                Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/health/live")).StatusCode);
                Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/health/ready")).StatusCode);
            }
        }
    }
}
