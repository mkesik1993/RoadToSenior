using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using WordAnalytics.Application.Analysis;
using WordAnalytics.Application.Features.AnalyzeText;
using WordAnalytics.Domain.Analysis;

namespace WordAnalytics.Tests
{
    public class AnalyzeTextHandlerTests
    {
        [Fact]
        public void Handle_WithConfiguredDefault_ReturnsExpectedTopWords()
        {
            var handler = CreateHandler(topCount: 2);

            var result = handler.Handle(new AnalyzeTextInput("Hello, hello world! Code code code."));

            Assert.Equal([new KeyValuePair<string, int>("code", 3), new("hello", 2)], result.Top);
        }

        [Fact]
        public void Handle_WithExplicitTopCount_OverridesConfiguredDefault()
        {
            var handler = CreateHandler(topCount: 1);

            var result = handler.Handle(new AnalyzeTextInput("hello hello world", TopCount: 2));

            Assert.Equal([new KeyValuePair<string, int>("hello", 2), new("world", 1)], result.Top);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("!!! ...")]
        public void Handle_WithNoWords_ReturnsEmpty(string? text)
        {
            var result = CreateHandler().Handle(new AnalyzeTextInput(text));

            Assert.Empty(result.Top);
        }

        private static AnalyzeTextHandler CreateHandler(int topCount = 3)
        {
            return new AnalyzeTextHandler(
                new WordCounter(),
                new WordRanker(),
                Options.Create(new RankingOptions { TopCount = topCount }),
                NullLogger<AnalyzeTextHandler>.Instance);
        }
    }
}
