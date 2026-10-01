using FluentAssertions;
using NetArchTest.Rules;
using Shared.ArchTests.Rules;
using Vero.Shared.Abstractions.Bus;
using Vero.Shared.MediatR;
using Xunit;

namespace Shared.ArchTests.Application
{
    public sealed class QueryTest
    {
        [Fact]
        public void AllQueriesShouldBeSealedTest()
        {
            var result = Types.InAssemblies(AssembliesHelper.GetAllAssemblies(AssembliesHelper.VeroAssembly.Infrastructure))
                .That()
                .ImplementInterface(typeof(IQuery<>))
                .Should()
                .BeSealed()
                .GetResult();

            result.IsSuccessful.Should()
                .BeTrue("Not all command are sealed!: " + result.FailingTypes?.GetTypesMessage());
        }

        [Fact]
        public void AllQueriesShouldEndWithQueryTest()
        {
            var result = Types.InAssemblies(AssembliesHelper.GetAllAssemblies(AssembliesHelper.VeroAssembly.Infrastructure))
                .That()
                .ImplementInterface(typeof(IQuery<>))
                .And()
                .AreNotAbstract()
                .Should()
                .HaveNameEndingWith("Query")
                .Or()
                .HaveNameEndingWith("BaseQuery")
                .GetResult();

            result.IsSuccessful.Should()
                .BeTrue("Not all queries are ending with Query!: " + result.FailingTypes?.GetTypesMessage());
        }

        [Fact]
        public void AllQueriesShouldResideInCorrespondingFolderTest()
        {
            var result = Types.InAssemblies(AssembliesHelper.GetAllAssemblies(AssembliesHelper.VeroAssembly.Infrastructure))
                .That()
                .ImplementInterface(typeof(IQuery<>))
                .And()
                .AreNotAbstract()
                .Should()
                .MeetCustomRule(new ResideInCorrespondingFolderRule("Query"))
                .GetResult();

            result.IsSuccessful.Should()
                .BeTrue("Not all query are reside in corresponding folder!: " + result.FailingTypes?.GetTypesMessage());
        }

        [Fact]
        public void AllQueriesShouldInheritFromQueryBaseTest()
        {
            var result = Types.InAssemblies(AssembliesHelper.GetAllAssemblies(AssembliesHelper.VeroAssembly.Infrastructure))
                .That()
                .ImplementInterface(typeof(IQuery<>))
                .And()
                .AreNotAbstract()
                .Should()
                .Inherit(typeof(QueryBase<>))
                .GetResult();

            result.IsSuccessful.Should()
                .BeTrue("Not all query inherit from QueryBase!: " + result.FailingTypes?.GetTypesMessage());
        }
    }
}