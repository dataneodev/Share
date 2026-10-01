using FluentAssertions;
using NetArchTest.Rules;
using ProfiBiznes.Shared.Infrastructure;

namespace ProfiBiznes.Shared.UnitTests.Architecture
{
    public abstract partial class ArchTests
    {
        [Fact]
        [DevTest]
        public void ApiLayerShouldNotContainDomainReference()
        {
            var assemblies = AssembliesContainer.CreateFromUnitTestAssembly(GetAssembly());

            if (assemblies.Domain == null)
                return;

            var apiNamespace = string.Concat("ProfiBiznes.", assemblies.Domain.FullName);

            var result = Types.InAssemblies(assemblies.Available)
                .That()
                .ResideInNamespaceContaining(".API.")
                .ShouldNot()
                .HaveDependencyOnAny(apiNamespace)
                .GetResult();

            result.IsSuccessful
                .Should()
                .BeTrue("API contains references to domain!: " + result.FailingTypes?.GetTypesMessage());
        }
    }
}