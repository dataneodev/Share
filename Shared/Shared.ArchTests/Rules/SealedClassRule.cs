using Mono.Cecil;
using NetArchTest.Rules;

namespace Shared.ArchTests.Rules
{
    internal sealed class SealedClassRule : ICustomRule
    {
        public bool MeetsRule(TypeDefinition type) => type.IsClass && !type.IsAbstract && type.IsSealed;
    }
}