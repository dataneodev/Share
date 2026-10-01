using Mono.Cecil;
using NetArchTest.Rules;

namespace ProfiBiznes.Shared.UnitTests.Architecture.Rules
{
    public sealed class PublicClassRule : ICustomRule
    {
        public bool MeetsRule(TypeDefinition type) => type.IsClass && type.IsPublic;
    }
}