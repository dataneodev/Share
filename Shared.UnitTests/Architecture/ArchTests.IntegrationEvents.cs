using FluentAssertions;
using NetArchTest.Rules;
using ProfiBiznes.Shared.Application.Bus.Events;
using ProfiBiznes.Shared.Infrastructure;
using ProfiBiznes.Shared.UnitTests.Architecture.Rules;

namespace ProfiBiznes.Shared.UnitTests.Architecture
{
    public abstract partial class ArchTests
    {
        [Fact]
        [DevTest]
        public void AllEventsShouldEndWithIntegrationEventTest()
        {
            var assemblies = AssembliesContainer.CreateFromUnitTestAssembly(GetAssembly());

            var result = Types.InAssemblies(assemblies.Available)
                .That()
                .Inherit(typeof(IntegrationEvent))
                .Should()
                .HaveNameEndingWith("IntegrationEvent")
                .GetResult();

            result.IsSuccessful
                .Should()
                .BeTrue("Not all integration event are ending with IntegrationEvent!: " + result.FailingTypes?.GetTypesMessage());
        }

        [Fact]
        [DevTest]
        public void AllIntegrationEventShouldNotContainsFieldsTest()
        {
            var assemblies = AssembliesContainer.CreateFromUnitTestAssembly(GetAssembly());

            var result = Types.InAssemblies(assemblies.Available)
                .That()
                .Inherit(typeof(IntegrationEvent))
                .GetTypes()
                .ToArray();

            var canWrite = result.Where(w => w.GetFields()
                    .Any()
                )
                .ToArray();

            canWrite.Should()
                .BeEmpty("Integration events should not contains fields!: " + canWrite.GetTypesMessage());
        }

        [Fact]
        [DevTest]
        public void AllIntegrationEventsShouldBePublicTest()
        {
            var assemblies = AssembliesContainer.CreateFromUnitTestAssembly(GetAssembly());

            var result = Types.InAssemblies(assemblies.Available)
                .That()
                .Inherit(typeof(IntegrationEvent))
                .And()
                .AreNotAbstract()
                .Should()
                .MeetCustomRule(new PublicClassRule())
                .GetResult();

            result.IsSuccessful
                .Should()
                .BeTrue("Integration events should be public!: " + result.GetFailingTypesMessage());
        }

        [Fact]
        [DevTest]
        public void AllIntegrationEventsShouldBeSealedTest()
        {
            var assemblies = AssembliesContainer.CreateFromUnitTestAssembly(GetAssembly());

            var result = Types.InAssemblies(assemblies.Available)
                .That()
                .Inherit(typeof(IntegrationEvent))
                .And()
                .AreNotAbstract()
                .And()
                .DoNotHaveNameEndingWith("BaseIntegrationEvent")
                .Should()
                .BeSealed()
                .GetResult();

            result.IsSuccessful
                .Should()
                .BeTrue("Not all integration event are sealed!: " + result.FailingTypes?.GetTypesMessage());
        }

        [Fact]
        [DevTest]
        public void AllIntegrationEventsShouldResideInContractsTest()
        {
            var assemblies = AssembliesContainer.CreateFromUnitTestAssembly(GetAssembly());

            var result = Types.InAssemblies(assemblies.Available)
                .That()
                .Inherit(typeof(IntegrationEvent))
                .And()
                .AreNotAbstract()
                .Should()
                .ResideInNamespaceEndingWith(".Contracts")
                .Or()
                .ResideInNamespaceContaining(".Contracts.")
                .GetResult();

            result.IsSuccessful
                .Should()
                .BeTrue("Integration events should reside only in contracts layer!: " + result.GetFailingTypesMessage());
        }
    }
}