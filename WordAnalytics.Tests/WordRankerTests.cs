using WordAnalytics.Domain.Analysis;

namespace WordAnalytics.Tests
{
    public class WordRankerTests
    {
        private readonly WordRanker _service = new();

        private static Dictionary<string, int> SampleCounts()
        {
            return new Dictionary<string, int>
            {
                ["code"] = 3,
                ["hello"] = 2,
                ["world"] = 1,
                ["extra"] = 1
            };
        }

        [Fact]
        public void GetTop_ReturnsKeysInDescendingOrder()
        {
            // Arrange
            var counts = SampleCounts();

            // Act
            var top = _service.GetTop(counts, 3);

            // Assert
            Assert.Equal(["code", "hello"], top.Take(2).Select(kv => kv.Key));
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(3, 3)]
        [InlineData(10, 4)]
        public void GetTop_WithExplicitLimit_ReturnsRequestedNumberOfWords(int topCount, int expectedCount)
        {
            // Arrange
            var counts = SampleCounts();

            // Act
            var top = _service.GetTop(counts, topCount);

            // Assert
            Assert.Equal(expectedCount, top.Count);
        }

        [Fact]
        public void GetTop_WithEmptyCounts_ReturnsEmpty()
        {
            // Arrange
            var counts = new Dictionary<string, int>();

            // Act
            var top = _service.GetTop(counts, 3);

            // Assert
            Assert.Empty(top);
        }

        [Fact]
        public void GetTop_WithEqualCounts_PreservesInputOrder()
        {
            // Arrange
            var counts = SampleCounts();

            // Act
            var top = _service.GetTop(counts, 4);

            // Assert
            Assert.Equal(["code", "hello", "world", "extra"], top.Select(kv => kv.Key));
        }
    }
}
