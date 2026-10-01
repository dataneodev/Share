using FluentAssertions;
using NetArchTest.Rules;
using ProfiBiznes.Shared.Application.Bus.Commands;
using ProfiBiznes.Shared.Infrastructure;

namespace ProfiBiznes.Shared.UnitTests.Architecture
{
    public abstract partial class ArchTests
    {
        [Fact]
        [DevTest]
        public void AllCommandsShouldBeSealedTest()
        {
            var assemblies = AssembliesContainer.CreateFromUnitTestAssembly(GetAssembly());

            var result = Types.InAssemblies(assemblies.Available)
                .That()
                .Inherit(typeof(Command))
                .And()
                .AreNotAbstract()
                .Should()
                .BeSealed()
                .GetResult();

            result.IsSuccessful
                .Should()
                .BeTrue("Not all command are sealed!: " + result.FailingTypes?.GetTypesMessage());
        }

        [Fact]
        [DevTest]
        public void AllCommandsShouldEndWithCommandTest()
        {
            var assemblies = AssembliesContainer.CreateFromUnitTestAssembly(GetAssembly());

            var result = Types.InAssemblies(assemblies.Available)
                .That()
                .Inherit(typeof(Command))
                .And()
                .AreNotAbstract()
                .Should()
                .HaveNameEndingWith("Command")
                .Or()
                .HaveNameEndingWith("BaseCommand")
                .GetResult();

            result.IsSuccessful
                .Should()
                .BeTrue("Not all command are ending with Command!: " + result.FailingTypes?.GetTypesMessage());
        }
    }
}