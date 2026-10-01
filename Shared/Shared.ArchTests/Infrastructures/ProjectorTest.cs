using FluentAssertions;
using NetArchTest.Rules;
using Shared.ArchTests.Rules;
using Vero.Shared.Events;
using Xunit;

namespace Shared.ArchTests.Infrastructures
{
    public sealed class ProjectorTest
    {
        [Fact]
        public void AllProjectorsShouldBeSealedTest()
        {
            var result = Types.InAssemblies(AssembliesHelper.GetAllAssemblies(AssembliesHelper.VeroAssembly.Infrastructure))
                .That()
                .Inherit(typeof(Projector<>))
                .Should()
                .BeSealed()
                .GetResult();

            result.IsSuccessful.Should()
                .BeTrue("Not all projectors are sealed!: " + result.FailingTypes?.GetTypesMessage());
        }

        [Fact]
        public void AllProjectorsShouldEndWithProjectorTest()
        {
            var result = Types.InAssemblies(AssembliesHelper.GetAllAssemblies(AssembliesHelper.VeroAssembly.Infrastructure))
                .That()
                .Inherit(typeof(Projector<>))
                .Should()
                .HaveNameEndingWith("Projector")
                .Or()
                .HaveNameMatching("IntegrationEventProjector_")
                .GetResult();

            result.IsSuccessful.Should()
                .BeTrue("Not all projectors are ending with Projector!: " + result.FailingTypes?.GetTypesMessage());
        }

        [Fact]
        public void AllProjectorsShouldContainsIntegrationEventTest()
        {
            var commandHandlers = Types.InAssemblies(AssembliesHelper.GetAllAssemblies(AssembliesHelper.VeroAssembly.Infrastructure))
                .That()
                .Inherit(typeof(Projector<>))
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
                .BeEmpty("Some projectors have wrong name!: " + invalidCommandHandlers?.GetTypesMessage());
        }

        [Fact]
        public void AllProjectorsShouldBeInternalTest()
        {
            var result = Types.InAssemblies(AssembliesHelper.GetAllAssemblies(AssembliesHelper.VeroAssembly.Infrastructure))
                .That()
                .Inherit(typeof(Projector<>))
                .Should()
                .MeetCustomRule(new InternalClassRule())
                .GetResult();

            result.IsSuccessful.Should()
                .BeTrue("Projectors should be internal!: " + result.GetFailingTypesMessage());
        }

        [Fact]
        public void AllProjectorsShouldResideInInfrastructureTest()
        {
            var result = Types.InAssemblies(AssembliesHelper.GetAllAssemblies())
                .That()
                .Inherit(typeof(Projector<>))
                .Should()
                .ResideInNamespaceContaining(".Infrastructure.")
                .GetResult();

            result.IsSuccessful.Should()
                .BeTrue("Projectors should reside only in application layer!: " + result.GetFailingTypesMessage());
        }
    }
}