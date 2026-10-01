using Mono.Cecil;
using NetArchTest.Rules;

namespace Shared.ArchTests.Rules
{
    public sealed class InternalClassRule : ICustomRule
    {
        public bool MeetsRule(TypeDefinition type) => type.IsClass && !(type.IsPublic || type.IsNotPublic && type.IsNested);
    }
}