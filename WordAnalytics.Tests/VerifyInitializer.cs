using System.Runtime.CompilerServices;

namespace WordAnalytics.Tests
{
    public static class VerifyInitializer
    {
        [ModuleInitializer]
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
