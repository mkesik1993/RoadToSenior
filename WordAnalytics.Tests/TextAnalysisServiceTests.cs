using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using WordAnalytics.Cli;
using WordAnalytics.Counting.Interfaces;
using WordAnalytics.Ranking.Interfaces;

namespace WordAnalytics.Tests
{
    public class TextAnalysisServiceTests
    {
        private readonly Mock<IWordCounter> _counter;
        private readonly Mock<IWordRanker> _ranker;
        private readonly TextAnalysisService _service;

        public TextAnalysisServiceTests()
        {
            _counter = new Mock<IWordCounter>();
            _ranker = new Mock<IWordRanker>();

            _service = new TextAnalysisService(_counter.Object, _ranker.Object, NullLogger<TextAnalysisService>.Instance);
        }

        [Fact]
        public void Analyze_PassesCounterResultToRanker()
        {
            // Arrange
            var counts = new Dictionary<string, int> { ["hello"] = 2 };
            var expected = new List<KeyValuePair<string, int>> { new("hello", 2) };

            _counter.Setup(c => c.CountWords(It.IsAny<string>())).Returns(counts);
            _ranker.Setup(r => r.GetTop(counts)).Returns(expected);

            // Act
            var result = _service.Analyze(It.IsAny<string>());

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Analyze_WithNullInput_PassesEmptyStringToCounter()
        {
            // Arrange
            _counter.Setup(c => c.CountWords(It.IsAny<string>())).Returns(new Dictionary<string, int>());
            _ranker.Setup(r => r.GetTop(It.IsAny<Dictionary<string, int>>())).Returns(new List<KeyValuePair<string, int>>());

            // Act
            _service.Analyze(null);

            // Assert
            _counter.Verify(c => c.CountWords(It.IsAny<string>()), Times.Once);
        }
    }
}
