using FluentAssertions;
using NetArchTest.Rules;
using ProfiBiznes.Shared.Application.Bus.Queries;
using ProfiBiznes.Shared.Infrastructure;
using ProfiBiznes.Shared.UnitTests.Architecture.Rules;

namespace ProfiBiznes.Shared.UnitTests.Architecture
{
    public abstract partial class ArchTests
    {
        [Fact]
        [DevTest]
        public void AllQueriesShouldBeSealedTest()
        {
            var assemblies = AssembliesContainer.CreateFromUnitTestAssembly(GetAssembly());

            var result = Types.InAssemblies(assemblies.Available)
                .That()
                .Inherit(typeof(Query<>))
                .Should()
                .BeSealed()
                .GetResult();

            result.IsSuccessful
                .Should()
                .BeTrue("Not all command are sealed!: " + result.FailingTypes?.GetTypesMessage());
        }

        [Fact]
        [DevTest]
        public void AllQueriesShouldEndWithQueryTest()
        {
            var assemblies = AssembliesContainer.CreateFromUnitTestAssembly(GetAssembly());

            var result = Types.InAssemblies(assemblies.Available)
                .That()
                .Inherit(typeof(Query<>))
                .And()
                .AreNotAbstract()
                .Should()
                .HaveNameEndingWith("Query")
                .GetResult();

            result.IsSuccessful
                .Should()
                .BeTrue("Not all queries are ending with Query!: " + result.FailingTypes?.GetTypesMessage());
        }

        [Fact]
        [DevTest]
        public void AllQueriesShouldResideInCorrespondingFolderTest()
        {
            var assemblies = AssembliesContainer.CreateFromUnitTestAssembly(GetAssembly());

            var result = Types.InAssemblies(assemblies.Available)
                .That()
                .Inherit(typeof(Query<>))
                .And()
                .AreNotAbstract()
                .Should()
                .MeetCustomRule(new ResideInCorrespondingFolderRule("Query"))
                .GetResult();

            result.IsSuccessful
                .Should()
                .BeTrue("Not all query are reside in corresponding folder!: " + result.FailingTypes?.GetTypesMessage());
        }
    }
}