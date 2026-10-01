using FluentAssertions;
using NetArchTest.Rules;
using ProfiBiznes.Shared.Application.Bus.Commands;
using ProfiBiznes.Shared.Infrastructure;
using ProfiBiznes.Shared.UnitTests.Architecture.Rules;

namespace ProfiBiznes.Shared.UnitTests.Architecture
{
    public abstract partial class ArchTests
    {
        [Fact]
        [DevTest]
        public void AllCommandHandlersShouldBeInternalTest()
        {
            var assemblies = AssembliesContainer.CreateFromUnitTestAssembly(GetAssembly());

            var result = Types.InAssemblies(assemblies.Available)
                .That()
                .Inherit(typeof(CommandHandler<>))
                .Or()
                .Inherit(typeof(CommandHandler<,>))
                .Should()
                .MeetCustomRule(new InternalClassRule())
                .GetResult();

            result.IsSuccessful
                .Should()
                .BeTrue("Command handler should be internal!: " + result.GetFailingTypesMessage());
        }

        [Fact]
        [DevTest]
        public void AllCommandHandlersShouldContainsNameOfCommandTest()
        {
            var assemblies = AssembliesContainer.CreateFromUnitTestAssembly(GetAssembly());

            var commandHandlers = Types.InAssemblies(assemblies.Available)
                .That()
                .Inherit(typeof(CommandHandler<>))
                .Or()
                .Inherit(typeof(CommandHandler<,>))
                .GetTypes();

            var invalidCommandHandlers = commandHandlers.Where(w =>
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
                .BeEmpty("Some command handler have wrong name!: " + invalidCommandHandlers.GetTypesMessage());
        }

        [Fact]
        [DevTest]
        public void AllCommandHandlersShouldEndWithCommandHandlerTest()
        {
            var assemblies = AssembliesContainer.CreateFromUnitTestAssembly(GetAssembly());

            var result = Types.InAssemblies(assemblies.Available)
                .That()
                .Inherit(typeof(CommandHandler<>))
                .Or()
                .Inherit(typeof(CommandHandler<,>))
                .Should()
                .HaveNameEndingWith("CommandHandler")
                .GetResult();

            result.IsSuccessful
                .Should()
                .BeTrue("Not all command handlers are ending with CommandHandler!: " + result.FailingTypes?.GetTypesMessage());
        }

        [Fact]
        [DevTest]
        public void AllCommandHandlersShouldResideInApplicationTest()
        {
            var assemblies = AssembliesContainer.CreateFromUnitTestAssembly(GetAssembly());

            var result = Types.InAssemblies(assemblies.Available)
                .That()
                .Inherit(typeof(CommandHandler<>))
                .Or()
                .Inherit(typeof(CommandHandler<,>))
                .Should()
                .ResideInNamespaceContaining(".Application.")
                .GetResult();

            result.IsSuccessful
                .Should()
                .BeTrue("Command handler should reside only in application layer!: " + result.GetFailingTypesMessage());
        }
    }
}