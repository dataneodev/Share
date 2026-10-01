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
        public void AllIntegrationEventHandlersShouldBeInternalTest()
        {
            var assemblies = AssembliesContainer.CreateFromUnitTestAssembly(GetAssembly());

            var result = Types.InAssemblies(assemblies.Available)
                .That()
                .Inherit(typeof(IntegrationEventHandler<>))
                .And()
                .AreNotAbstract()
                .Should()
                .MeetCustomRule(new InternalClassRule())
                .GetResult();

            result.IsSuccessful
                .Should()
                .BeTrue("Integration event handlers should be internal!: " + result.GetFailingTypesMessage());
        }

        [Fact]
        [DevTest]
        public void AllIntegrationEventHandlersShouldBeSealedTest()
        {
            var assemblies = AssembliesContainer.CreateFromUnitTestAssembly(GetAssembly());

            var result = Types.InAssemblies(assemblies.Available)
                .That()
                .Inherit(typeof(IntegrationEventHandler<>))
                .And()
                .AreNotAbstract()
                .Should()
                .BeSealed()
                .GetResult();

            result.IsSuccessful
                .Should()
                .BeTrue("Not all integration event handlers are sealed!: " + result.FailingTypes?.GetTypesMessage());
        }

        [Fact]
        [DevTest]
        public void AllIntegrationEventHandlersShouldContainsNameOfIntegrationEventTest()
        {
            var assemblies = AssembliesContainer.CreateFromUnitTestAssembly(GetAssembly());

            var result = Types.InAssemblies(assemblies.Available)
                .That()
                .Inherit(typeof(IntegrationEventHandler<>))
                .And()
                .AreNotAbstract()
                .GetTypes();

            var invalidCommandHandlers = result.Where(w =>
                    {
                        var genericTypes = w.BaseType?.GenericTypeArguments;
                        if ((genericTypes?.Length ?? 0) < 1)
                            return true;

                        var command = genericTypes[0];

                        return !w.Name.StartsWith(command.Name);
                    }
                )
                .ToArray();

            invalidCommandHandlers.Should()
                .BeEmpty("Some integration event handlers have wrong name!: " + invalidCommandHandlers.GetTypesMessage());
        }

        [Fact]
        [DevTest]
        public void AllIntegrationEventHandlersShouldEndWithIntegrationEventHandlerTest()
        {
            var assemblies = AssembliesContainer.CreateFromUnitTestAssembly(GetAssembly());

            var result = Types.InAssemblies(assemblies.Available)
                .That()
                .Inherit(typeof(IntegrationEventHandler<>))
                .And()
                .AreNotAbstract()
                .Should()
                .HaveNameEndingWith("IntegrationEventHandler")
                .Or()
                .HaveNameMatching("IntegrationEventHandler_")
                .GetResult();

            result.IsSuccessful
                .Should()
                .BeTrue("Not all integration event handlers are ending with CommandHandler!: " + result.FailingTypes?.GetTypesMessage());
        }

        [Fact]
        [DevTest]
        public void AllIntegrationEventHandlersShouldNotDependOnMartenTest()
        {
            var assemblies = AssembliesContainer.CreateFromUnitTestAssembly(GetAssembly());

            var result = Types.InAssemblies(assemblies.Available)
                .That()
                .Inherit(typeof(IntegrationEventHandler<>))
                .And()
                .AreNotAbstract()
                .Should()
                .NotHaveDependencyOn("Marten")
                .GetResult();

            result.IsSuccessful
                .Should()
                .BeTrue("Integration event handlers should not depend on Marten!: " + result.FailingTypes?.GetTypesMessage());
        }

        [Fact]
        [DevTest]
        public void AllIntegrationEventHandlersShouldResideInInfrastructureOrApplicationTest()
        {
            var assemblies = AssembliesContainer.CreateFromUnitTestAssembly(GetAssembly());

            var result = Types.InAssemblies(assemblies.Available)
                .That()
                .Inherit(typeof(IntegrationEventHandler<>))
                .And()
                .AreNotAbstract()
                .Should()
                .ResideInNamespaceContaining(".Infrastructure.")
                .Or()
                .ResideInNamespaceContaining(".Application.")
                .GetResult();

            result.IsSuccessful
                .Should()
                .BeTrue("Integration event handlers should reside only in application layer!: " + result.GetFailingTypesMessage());
        }
    }
}