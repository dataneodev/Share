using FluentAssertions;
using NetArchTest.Rules;
using Shared.ArchTests.Rules;
using Vero.Shared.MediatR;
using Xunit;

namespace Shared.ArchTests.Application
{
    public sealed class CommandHandlerTest
    {
        [Fact]
        public void AllCommandHandlersShouldBeSealedTest()
        {
            var result = Types.InAssemblies(AssembliesHelper.GetAllAssemblies(AssembliesHelper.VeroAssembly.Application))
                .That()
                .Inherit(typeof(CommandHandler<>))
                .Or()
                .Inherit(typeof(CommandHandler<,>))
                .Should()
                .BeSealed()
                .GetResult();

            result.IsSuccessful.Should()
                .BeTrue("Not all command handlers are sealed!: " + result.FailingTypes?.GetTypesMessage());
        }

        [Fact]
        public void AllCommandHandlersShouldEndWithCommandHandlerTest()
        {
            var result = Types.InAssemblies(AssembliesHelper.GetAllAssemblies(AssembliesHelper.VeroAssembly.Application))
                .That()
                .Inherit(typeof(CommandHandler<>))
                .Or()
                .Inherit(typeof(CommandHandler<,>))
                .Should()
                .HaveNameEndingWith("CommandHandler")
                .GetResult();

            result.IsSuccessful.Should()
                .BeTrue("Not all command handlers are ending with CommandHandler!: " + result.FailingTypes?.GetTypesMessage());
        }

        [Fact]
        public void AllCommandHandlersShouldContainsNameOfCommandTest()
        {
            var commandHandlers = Types.InAssemblies(AssembliesHelper.GetAllAssemblies(AssembliesHelper.VeroAssembly.Application))
                .That()
                .Inherit(typeof(CommandHandler<>))
                .Or()
                .Inherit(typeof(CommandHandler<,>))
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
                .BeEmpty("Some command handler have wrong name!: " + invalidCommandHandlers?.GetTypesMessage());
        }

        [Fact]
        public void AllCommandHandlersShouldBeInternalTest()
        {
            var result = Types.InAssemblies(AssembliesHelper.GetAllAssemblies(AssembliesHelper.VeroAssembly.Application))
                .That()
                .Inherit(typeof(CommandHandler<>))
                .Or()
                .Inherit(typeof(CommandHandler<,>))
                .Should()
                .MeetCustomRule(new InternalClassRule())
                .GetResult();

            result.IsSuccessful.Should()
                .BeTrue("Command handler should be internal!: " + result.GetFailingTypesMessage());
        }

        [Fact]
        public void AllCommandHandlersShouldResideInApplicationTest()
        {
            var result = Types.InAssemblies(AssembliesHelper.GetAllAssemblies())
                .That()
                .Inherit(typeof(CommandHandler<>))
                .Or()
                .Inherit(typeof(CommandHandler<,>))
                .Should()
                .ResideInNamespaceContaining(".Application.")
                .GetResult();

            result.IsSuccessful.Should()
                .BeTrue("Command handler should reside only in application layer!: " + result.GetFailingTypesMessage());
        }

        [Fact]
        public void AllCommandHandlersShouldResideInCorrespondingFolderTest()
        {
            var result = Types.InAssemblies(AssembliesHelper.GetAllAssemblies())
                .That()
                .Inherit(typeof(CommandHandler<>))
                .Or()
                .Inherit(typeof(CommandHandler<,>))
                .Should()
                .MeetCustomRule(new ResideInCorrespondingFolderRule("CommandHandler"))
                .GetResult();

            result.IsSuccessful.Should()
                .BeTrue("Not all command handler are reside in corresponding folder!: " + result.FailingTypes?.GetTypesMessage());
        }
    }
}