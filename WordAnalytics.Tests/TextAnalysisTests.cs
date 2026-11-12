using WordAnalytics.Domain.Analysis;

namespace WordAnalytics.Tests;

public class TextAnalysisTests
{
    private static readonly Guid AnalysisId = Guid.NewGuid();
    private static readonly DateTimeOffset CreatedAtUtc = new(2026, 11, 12, 8, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Create_WithValidData_PreservesValuesAndWordOrder()
    {
        // Arrange
        const string text = "World, hello HELLO!";
        var topWords = new[]
        {
            new KeyValuePair<string, int>("hello", 2),
            new KeyValuePair<string, int>("world", 1)
        };

        // Act
        var analysis = TextAnalysis.Create(AnalysisId, text, CreatedAtUtc, 2, topWords);

        // Assert
        Assert.Equal(AnalysisId, analysis.Id);
        Assert.Equal(text, analysis.Text);
        Assert.Equal(CreatedAtUtc, analysis.CreatedAtUtc);
        Assert.Equal(2, analysis.TopCount);
        Assert.Equal(topWords, analysis.TopWords);
    }

    [Fact]
    public void Create_WithEmptyTopWords_AllowsEmptyResult()
    {
        // Arrange
        var topWords = Array.Empty<KeyValuePair<string, int>>();

        // Act
        var analysis = TextAnalysis.Create(AnalysisId, "!!! ...", CreatedAtUtc, 3, topWords);

        // Assert
        Assert.Empty(analysis.TopWords);
    }

    [Fact]
    public void Create_WithMutableInput_CopiesTopWords()
    {
        // Arrange
        var topWords = new[]
        {
            new KeyValuePair<string, int>("hello", 2)
        };

        // Act
        var analysis = TextAnalysis.Create(AnalysisId, "hello hello", CreatedAtUtc, 1, topWords);
        topWords[0] = new KeyValuePair<string, int>("changed", 1);

        // Assert
        Assert.Equal("hello", analysis.TopWords[0].Key);
    }

    [Fact]
    public void Create_WithEmptyId_ThrowsArgumentException()
    {
        // Arrange
        var id = Guid.Empty;
        var topWords = Array.Empty<KeyValuePair<string, int>>();

        // Act
        var create = () => TextAnalysis.Create(id, "hello", CreatedAtUtc, 1, topWords);

        // Assert
        Assert.Throws<ArgumentException>(create);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithMissingOrBlankText_ThrowsArgumentException(string? text)
    {
        // Arrange
        var topWords = Array.Empty<KeyValuePair<string, int>>();

        // Act
        var create = () => TextAnalysis.Create(AnalysisId, text!, CreatedAtUtc, 1, topWords);

        // Assert
        Assert.Throws<ArgumentException>(create);
    }

    [Fact]
    public void Create_WithTextAboveMaximumLength_ThrowsArgumentException()
    {
        // Arrange
        var text = new string('a', TextAnalysis.MaxTextLength + 1);
        var topWords = Array.Empty<KeyValuePair<string, int>>();

        // Act
        var create = () => TextAnalysis.Create(AnalysisId, text, CreatedAtUtc, 1, topWords);

        // Assert
        Assert.Throws<ArgumentException>(create);
    }

    [Fact]
    public void Create_WithNonUtcCreationTime_ThrowsArgumentException()
    {
        // Arrange
        var createdAt = new DateTimeOffset(2026, 11, 12, 9, 0, 0, TimeSpan.FromHours(1));
        var topWords = Array.Empty<KeyValuePair<string, int>>();

        // Act
        var create = () => TextAnalysis.Create(AnalysisId, "hello", createdAt, 1, topWords);

        // Assert
        Assert.Throws<ArgumentException>(create);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithInvalidTopCount_ThrowsArgumentOutOfRangeException(int topCount)
    {
        // Arrange
        var topWords = Array.Empty<KeyValuePair<string, int>>();

        // Act
        var create = () => TextAnalysis.Create(AnalysisId, "hello", CreatedAtUtc, topCount, topWords);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(create);
    }

    [Fact]
    public void Create_WithMoreWordsThanTopCount_ThrowsArgumentException()
    {
        // Arrange
        var topWords = new[]
        {
            new KeyValuePair<string, int>("hello", 2),
            new KeyValuePair<string, int>("world", 1)
        };

        // Act
        var create = () => TextAnalysis.Create(AnalysisId, "hello world", CreatedAtUtc, 1, topWords);

        // Assert
        Assert.Throws<ArgumentException>(create);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyWord_ThrowsArgumentException(string word)
    {
        // Arrange
        var topWords = new[]
        {
            new KeyValuePair<string, int>(word, 1)
        };

        // Act
        var create = () => TextAnalysis.Create(AnalysisId, "hello", CreatedAtUtc, 1, topWords);

        // Assert
        Assert.Throws<ArgumentException>(create);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithNonPositiveWordCount_ThrowsArgumentException(int count)
    {
        // Arrange
        var topWords = new[]
        {
            new KeyValuePair<string, int>("hello", count)
        };

        // Act
        var create = () => TextAnalysis.Create(AnalysisId, "hello", CreatedAtUtc, 1, topWords);

        // Assert
        Assert.Throws<ArgumentException>(create);
    }

    [Fact]
    public void Create_WithDuplicateWords_ThrowsArgumentException()
    {
        // Arrange
        var topWords = new[]
        {
            new KeyValuePair<string, int>("hello", 2),
            new KeyValuePair<string, int>("hello", 1)
        };

        // Act
        var create = () => TextAnalysis.Create(AnalysisId, "hello hello", CreatedAtUtc, 2, topWords);

        // Assert
        Assert.Throws<ArgumentException>(create);
    }
}
