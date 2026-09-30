using WordAnalytics.Counting.Services;

namespace WordAnalytics.Tests
{
    public class WordCounterTests
    {
        private readonly WordCounter _service = new();

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("!!! ...")]
        public void CountWords_WithNoWords_ReturnsEmpty(string input)
        {
            // Act
            var result = _service.CountWords(input);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void CountWords_WithCorrectText_ReturnsCounts()
        {
            // Act
            var result = _service.CountWords("Hello, hello world! Code code code.");

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal(3, result["code"]);
            Assert.Equal(2, result["hello"]);
            Assert.Equal(1, result["world"]);
        }

        [Fact]
        public void CountWords_IgnoresCase()
        {
            // Act
            var result = _service.CountWords("Hello HELLO hello");

            // Assert
            Assert.Single(result);
            Assert.Equal(3, result["hello"]);
        }

        [Fact]
        public void CountWords_TreatsPunctuationAsSeparator()
        {
            // Act
            var result = _service.CountWords("hello,world");

            // Assert
            Assert.Equal(2, result.Count);
        }
    }
}
