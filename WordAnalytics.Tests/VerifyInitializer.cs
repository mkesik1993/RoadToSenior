using System.Runtime.CompilerServices;

namespace WordAnalytics.Tests
{
    public static class VerifyInitializer
    {
#pragma warning disable CA2255 // Verify requires a module initializer in test projects
        [ModuleInitializer]
#pragma warning restore CA2255
        public static void Init()
        {
            Verifier.DerivePathInfo((sourceFile, projectDirectory, type, method) =>
                new PathInfo(
                    directory: Path.Combine(projectDirectory, "Snapshots", type.Name),
                    typeName: type.Name,
                    methodName: method.Name));

            VerifierSettings.IgnoreMembers("testId");
        }
    }
}
