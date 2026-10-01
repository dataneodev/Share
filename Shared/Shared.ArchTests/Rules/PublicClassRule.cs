using Mono.Cecil;
using NetArchTest.Rules;

namespace Shared.ArchTests.Rules
{
    public sealed class PublicClassRule : ICustomRule
    {
        public bool MeetsRule(TypeDefinition type) => type.IsClass && type.IsPublic;
    }
}