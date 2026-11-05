namespace WordAnalytics.Application.Features.AnalyzeText;

public record AnalyzeTextInput(string? Text, int? TopCount = null);
