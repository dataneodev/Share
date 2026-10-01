using FluentAssertions;
using NetArchTest.Rules;
using Shared.ArchTests.Rules;
using Vero.Shared.Events;
using Vero.Shared.MessageBroker;
using Xunit;

namespace Shared.ArchTests.Infrastructures
{
    public sealed class IntegrationEventHandlerTest
    {
        [Fact]
        public void AllIntegrationEventHandlersShouldBeSealedTest()
        {
            var result = Types.InAssemblies(AssembliesHelper.GetAllAssemblies())
                .That()
                .Inherit(typeof(IntegrationEventHandler<>))
                .And()
                .DoNotInherit(typeof(Projector<>))
                .And()
                .AreNotAbstract()
                .Should()
                .BeSealed()
                .GetResult();

            result.IsSuccessful.Should()
                .BeTrue("Not all integration event handlers are sealed!: " + result.FailingTypes?.GetTypesMessage());
        }

        [Fact]
        public void AllIntegrationEventHandlersShouldEndWithIntegrationEventHandlerTest()
        {
            var result = Types.InAssemblies(AssembliesHelper.GetAllAssemblies())
                .That()
                .Inherit(typeof(IntegrationEventHandler<>))
                .And()
                .DoNotInherit(typeof(Projector<>))
                .And()
                .AreNotAbstract()
                .Should()
                .HaveNameEndingWith("IntegrationEventHandler")
                .Or()
                .HaveNameMatching("IntegrationEventHandler_")
                .GetResult();

            result.IsSuccessful.Should()
                .BeTrue("Not all integration event handlers are ending with CommandHandler!: " + result.FailingTypes?.GetTypesMessage());
        }

        [Fact]
        public void AllIntegrationEventHandlersShouldContainsNameOfIntegrationEventTest()
        {
            var commandHandlers = Types.InAssemblies(AssembliesHelper.GetAllAssemblies())
                .That()
                .Inherit(typeof(IntegrationEventHandler<>))
                .And()
                .DoNotInherit(typeof(Projector<>))
                .And()
                .AreNotAbstract()
                .GetTypes();

            var invalidCommandHandlers = commandHandlers.Where(
                    w =>
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
                .BeEmpty("Some integration event handlers have wrong name!: " + invalidCommandHandlers?.GetTypesMessage());
        }

        [Fact]
        public void AllIntegrationEventHandlersShouldBeInternalTest()
        {
            var result = Types.InAssemblies(AssembliesHelper.GetAllAssemblies())
                .That()
                .Inherit(typeof(IntegrationEventHandler<>))
                .And()
                .DoNotInherit(typeof(Projector<>))
                .And()
                .AreNotAbstract()
                .Should()
                .MeetCustomRule(new InternalClassRule())
                .GetResult();

            result.IsSuccessful.Should()
                .BeTrue("Integration event handlers  should be internal!: " + result.GetFailingTypesMessage());
        }

        [Fact]
        public void AllIntegrationEventHandlersShouldResideInInfrastructureOrApplicationTest()
        {
            var result = Types.InAssemblies(AssembliesHelper.GetAllAssemblies())
                .That()
                .Inherit(typeof(IntegrationEventHandler<>))
                .And()
                .DoNotInherit(typeof(Projector<>))
                .And()
                .AreNotAbstract()
                .Should()
                .ResideInNamespaceContaining(".Infrastructure.")
                .Or()
                .ResideInNamespaceContaining(".Application.")
                .GetResult();

            result.IsSuccessful.Should()
                .BeTrue("Integration event handlers should reside only in application layer!: " + result.GetFailingTypesMessage());
        }

        [Fact]
        public void AllIntegrationEventHandlersShouldNotDependOnMartenTest()
        {
            var result = Types.InAssemblies(AssembliesHelper.GetAllAssemblies())
                .That()
                .Inherit(typeof(IntegrationEventHandler<>))
                .And()
                .DoNotInherit(typeof(Projector<>))
                .And()
                .AreNotAbstract()
                .Should()
                .NotHaveDependencyOn("Marten")
                .GetResult();

            result.IsSuccessful.Should()
                .BeTrue("Integration event handlers should not depend on Marten!: " + result.FailingTypes?.GetTypesMessage());
        }
    }
}