using FluentAssertions;
using NetArchTest.Rules;
using Vero.Shared.Extensions;
using Xunit;

namespace Shared.ArchTests
{
    public sealed class ClassTest
    {
        [Fact]
        public void AllClassShouldBeSealedTest()
        {
            var baseTypesHashSet = Types.InAssemblies(AssembliesHelper.GetAllAssemblies())
                .GetTypes()
                .Where(w => w.BaseType != null && !w.BaseType.IsGenericType && !w.BaseType.IsAbstract)
                .SelectHashSet(s => s.BaseType!.FullName);


            var parents = Types.InAssemblies(AssembliesHelper.GetAllAssemblies())
                .That()
                .AreNotSealed()
                .And()
                .AreNotGeneric()
                .And()
                .AreNotAbstract()
                .And()
                .AreNotStatic()
                .And()
                .ResideInNamespaceStartingWith("Vero.")
                .And()
                .DoNotResideInNamespaceStartingWith("Vero.Gateway.API.BFF")
                .GetTypes()
                .Where(w => !baseTypesHashSet.Contains(w.FullName))
                .ToArray();

            parents.Should()
                .BeEmpty("Not all class are sealed!: " + parents.GetTypesMessage());
        }
    }
}