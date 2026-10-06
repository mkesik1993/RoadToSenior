using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using WordAnalytics.Api.Contracts.Requests;

namespace WordAnalytics.Tests.Integration
{
    public class AnalyzeValidationTests(WordAnalyticsApiFactory factory) : IClassFixture<WordAnalyticsApiFactory>
    {
        private readonly HttpClient _client = factory.CreateClient();

        [Fact]
        public async Task Analyze_WithEmptyText_Returns400WithTextError()
        {
            // Arrange
            var response = await _client.PostAsJsonAsync("/api/analyze",
                new AnalyzeRequest { Text = string.Empty });

            // Act
            var problem = await response.Content
                .ReadFromJsonAsync<ValidationProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(problem);
            Assert.Equal(400, problem.Status);
            Assert.True(problem.Errors.ContainsKey("Text"));
            Assert.Contains("Text is required.", problem.Errors["Text"]);
        }

        [Fact]
        public async Task Analyze_WithMissingTextProperty_Returns400()
        {
            // Arrange
            var content = new StringContent("""{"topCount":5}""", Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/analyze", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(101)]
        public async Task Analyze_WithTopCountOutOfRange_Returns400(int topCount)
        {
            // Act
            var response = await _client.PostAsJsonAsync("/api/analyze",
                new AnalyzeRequest { Text = "hello world", TopCount = topCount });
            var problem = await response.Content
                .ReadFromJsonAsync<ValidationProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.True(problem!.Errors.ContainsKey("TopCount"));
        }

        [Fact]
        public async Task Analyze_WithMultipleErrors_ReturnsAllOfThem()
        {
            // Act
            var response = await _client.PostAsJsonAsync("/api/analyze",
                new AnalyzeRequest { Text = string.Empty, TopCount = 999 });
            var problem = await response.Content
                .ReadFromJsonAsync<ValidationProblemDetails>();

            // Assert
            Assert.Equal(2, problem!.Errors.Count);
            Assert.True(problem.Errors.ContainsKey("Text"));
            Assert.True(problem.Errors.ContainsKey("TopCount"));
        }
    }
}
