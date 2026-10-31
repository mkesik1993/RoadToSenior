using WordAnalytics.Domain.Analysis;

namespace WordAnalytics.Tests
{
    public class WordRankerTests
    {
        private readonly WordRanker _service = new();

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
            var top = _service.GetTop(SampleCounts(), 3);

            Assert.Equal(["code", "hello"], top.Take(2).Select(kv => kv.Key));
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(3, 3)]
        [InlineData(10, 4)]
        public void GetTop_WithExplicitLimit_ReturnsRequestedNumberOfWords(int topCount, int expectedCount)
        {
            var top = _service.GetTop(SampleCounts(), topCount);

            Assert.Equal(expectedCount, top.Count);
        }

        [Fact]
        public void GetTop_WithEmptyCounts_ReturnsEmpty()
        {
            var top = _service.GetTop(new Dictionary<string, int>(), 3);

            Assert.Empty(top);
        }

        [Fact]
        public void GetTop_WithEqualCounts_PreservesInputOrder()
        {
            var top = _service.GetTop(SampleCounts(), 4);

            Assert.Equal(["code", "hello", "world", "extra"], top.Select(kv => kv.Key));
        }
    }
}
