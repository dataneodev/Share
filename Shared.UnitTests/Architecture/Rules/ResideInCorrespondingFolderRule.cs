using Mono.Cecil;
using NetArchTest.Rules;

namespace ProfiBiznes.Shared.UnitTests.Architecture.Rules
{
    public sealed class ResideInCorrespondingFolderRule : ICustomRule
    {
        private readonly string _ignoreSuffix;

        public ResideInCorrespondingFolderRule(string ignoreSuffix) => _ignoreSuffix = ignoreSuffix;

        public bool MeetsRule(TypeDefinition type)
        {
            var fullName = type.FullName;

            var parts = fullName.Split('.');
            if (parts.Length < 2)
                return true;

            var name = parts[^1];
            var folder = parts[^2] + _ignoreSuffix;

            return string.Equals(folder, name);
        }
    }
}