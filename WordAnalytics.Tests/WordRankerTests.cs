using Microsoft.Extensions.Options;
using WordAnalytics.Ranking.Services;
using WordAnalytics.Ranking.Settings;

namespace WordAnalytics.Tests
{
    public class WordRankerTests
    {
        private static WordRanker CreateService(int topCount = 3)
        {
            return new WordRanker(Options.Create(new RankingOptions { TopCount = topCount }));
        }

        private static Dictionary<string, int> SampleCounts() => new()
        {
            ["code"] = 3,
            ["hello"] = 2,
            ["world"] = 1,
            ["extra"] = 1
        };

        [Fact]
        public void GetTop_ReturnsKeysInDescendingOrder()
        {
            // Act
            var top = CreateService().GetTop(SampleCounts());

            // Assert
            Assert.Equal(["code", "hello"], top.Take(2).Select(kv => kv.Key));
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(3, 3)]
        [InlineData(10, 4)]
        public void GetTop_RespectsConfiguredTopCount(int topCount, int expectedCount)
        {
            // Act
            var top = CreateService(topCount).GetTop(SampleCounts());

            // Assert
            Assert.Equal(expectedCount, top.Count);
        }

        [Fact]
        public void GetTop_WithEmptyCounts_ReturnsEmpty()
        {
            // Act
            var top = CreateService().GetTop(new Dictionary<string, int>());

            // Assert
            Assert.Empty(top);
        }

        [Fact]
        public void GetTop_WithExplicitTake_OverridesConfiguration()
        {
            var top = CreateService(topCount: 3).GetTop(SampleCounts(), take: 1);

            Assert.Single(top);
        }
    }
}
