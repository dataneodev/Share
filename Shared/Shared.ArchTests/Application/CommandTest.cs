using FluentAssertions;
using NetArchTest.Rules;
using Shared.ArchTests.Rules;
using Vero.Shared.MediatR;
using Xunit;

namespace Shared.ArchTests.Application
{
    public sealed class CommandTest
    {
        [Fact]
        public void AllCommandsShouldBeSealedTest()
        {
            var result = Types.InAssemblies(AssembliesHelper.GetAllAssemblies(AssembliesHelper.VeroAssembly.Application))
                .That()
                .Inherit(typeof(CommandBase))
                .And()
                .DoNotHaveNameEndingWith("BaseCommand")
                .Should()
                .BeSealed()
                .GetResult();

            result.IsSuccessful.Should()
                .BeTrue("Not all command are sealed!: " + result.FailingTypes?.GetTypesMessage());
        }

        [Fact]
        public void AllCommandsShouldEndWithCommandTest()
        {
            var result = Types.InAssemblies(AssembliesHelper.GetAllAssemblies(AssembliesHelper.VeroAssembly.Application))
                .That()
                .Inherit(typeof(CommandBase))
                .And()
                .AreNotAbstract()
                .Should()
                .HaveNameEndingWith("Command")
                .Or()
                .HaveNameEndingWith("BaseCommand")
                .GetResult();

            result.IsSuccessful.Should()
                .BeTrue("Not all command are ending with Command!: " + result.FailingTypes?.GetTypesMessage());
        }

        [Fact]
        public void AllCommandsShouldResideInCorrespondingFolderTest()
        {
            var result = Types.InAssemblies(AssembliesHelper.GetAllAssemblies(AssembliesHelper.VeroAssembly.Application))
                .That()
                .Inherit(typeof(CommandBase))
                .And()
                .AreNotAbstract()
                .Should()
                .MeetCustomRule(new ResideInCorrespondingFolderRule("Command"))
                .GetResult();

            result.IsSuccessful.Should()
                .BeTrue("Not all command are reside in corresponding folder!: " + result.FailingTypes?.GetTypesMessage());
        }
    }
}