using FluentAssertions;
using NetArchTest.Rules;
using ProfiBiznes.Shared.Domain.Extensions;
using ProfiBiznes.Shared.Infrastructure;

namespace ProfiBiznes.Shared.UnitTests.Architecture
{
    public abstract partial class ArchTests
    {
        [Fact]
        [DevTest]
        public void AllClassShouldBeSealedTest()
        {
            var assemblies = AssembliesContainer.CreateFromUnitTestAssembly(GetAssembly());

            var baseTypesHashSet = Types.InAssemblies(assemblies.Available)
                .GetTypes()
                .Where(w => w.BaseType is { IsGenericType: false, IsAbstract: false })
                .SelectHashSet(s => s.BaseType!.FullName);

            var parents = Types.InAssemblies(assemblies.Available)
                .That()
                .AreNotSealed()
                .And()
                .AreNotGeneric()
                .And()
                .AreNotAbstract()
                .And()
                .AreNotStatic()
                .And()
                .ResideInNamespaceStartingWith("ProfiBiznes.")
                .And()
                .DoNotResideInNamespaceStartingWith("ProfiBiznes.Gateway")
                .And()
                .DoNotResideInNamespaceStartingWith("ProfiBiznes.Shared.Translation")
                .GetTypes()
                .Where(w => !baseTypesHashSet.Contains(w.FullName))
                .ToArray();

            parents.Should()
                .BeEmpty("Not all class are sealed!: " + parents.GetTypesMessage());
        }
    }
}