using TestResult = NetArchTest.Rules.TestResult;

namespace ProfiBiznes.Shared.UnitTests.Architecture
{
    internal static class TestResultExtensions
    {
        public static string GetFailingTypesMessage(this TestResult testResult) =>
            Environment.NewLine +
            Environment.NewLine +
            string.Join(
                Environment.NewLine,
                testResult.FailingTypes
                    ?.Take(100)
                    .Select(s => s.FullName) ??
                Enumerable.Empty<string>()
            );

        public static string GetTypesMessage(this IReadOnlyList<Type> types) =>
            Environment.NewLine +
            Environment.NewLine +
            string.Join(
                Environment.NewLine,
                types.Take(100)
                    .Select(s => s.FullName)
            );
    }
}