using WordAnalytics.Application.Features.AnalyzeText;
using WordAnalytics.Domain.Analysis;

namespace WordAnalytics.Tests;

public class ArchitectureTests
{
    [Fact]
    public void Domain_WithCompiledDependencies_ReferencesOnlyFrameworkAssemblies()
    {
        var references = typeof(WordCounter).Assembly.GetReferencedAssemblies();

        Assert.All(references, reference =>
            Assert.True(
                reference.Name is "System" or "mscorlib" or "netstandard"
                    || reference.Name!.StartsWith("System.", StringComparison.Ordinal),
                $"Domain must not reference {reference.Name}."));
    }

    [Fact]
    public void Application_WithCompiledProjectDependencies_ReferencesOnlyDomain()
    {
        var projectReferences = typeof(AnalyzeTextHandler).Assembly.GetReferencedAssemblies()
            .Select(reference => reference.Name)
            .Where(name => name!.StartsWith("WordAnalytics.", StringComparison.Ordinal));

        Assert.Equal(["WordAnalytics.Domain"], projectReferences);
    }
}
