using WordAnalytics.Application.Features.CountWords;
using WordAnalytics.Domain.Analysis;

namespace WordAnalytics.Tests;

public class CountWordsHandlerTests
{
    private readonly CountWordsHandler _handler = new(new WordCounter());

    [Fact]
    public void Handle_WithSeveralDistinctWords_ReturnsAllCountsInInputOrder()
    {
        // Arrange
        var input = new CountWordsInput("first second second third fourth fifth");

        // Act
        var result = _handler.Handle(input);

        // Assert
        Assert.Equal(
            [
                new KeyValuePair<string, int>("first", 1),
                new("second", 2),
                new("third", 1),
                new("fourth", 1),
                new("fifth", 1)
            ],
            result.Counts);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("!!! ...")]
    public void Handle_WithNoWords_ReturnsEmpty(string? text)
    {
        // Arrange
        var input = new CountWordsInput(text);

        // Act
        var result = _handler.Handle(input);

        // Assert
        Assert.Empty(result.Counts);
    }
}
