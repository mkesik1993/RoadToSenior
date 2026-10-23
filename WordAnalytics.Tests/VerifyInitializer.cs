using System.Runtime.CompilerServices;

namespace WordAnalytics.Tests
{
    public static class VerifyInitializer
    {
        [ModuleInitializer]
        public static void Init()
        {
            // Snapshots land in Snapshots\<TypeName>\ instead of next to the test file
            Verifier.DerivePathInfo((sourceFile, projectDirectory, type, method) =>
                new PathInfo(
                    directory: Path.Combine(projectDirectory, "Snapshots", type.Name),
                    typeName: type.Name,
                    methodName: method.Name));

            // Members generated at runtime that can't be predicted
            VerifierSettings.IgnoreMembers("testId");
        }
    }
}
