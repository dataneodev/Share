using FluentAssertions;
using NetArchTest.Rules;
using Shared.ArchTests.Rules;
using Vero.Shared.MediatR;
using Xunit;

namespace Shared.ArchTests.Application
{
    public sealed class QueryHandlerTest
    {
        [Fact]
        public void AllQueryHandlersShouldBeSealedTest()
        {
            var result = Types.InAssemblies(AssembliesHelper.GetAllAssemblies(AssembliesHelper.VeroAssembly.Infrastructure))
                .That()
                .Inherit(typeof(QueryHandler<,>))
                .Should()
                .BeSealed()
                .GetResult();

            result.IsSuccessful.Should()
                .BeTrue("Not all query handler are sealed!: " + result.FailingTypes?.GetTypesMessage());
        }

        [Fact]
        public void AllQueryHandlersShouldEndWithQueryHandlerTest()
        {
            var result = Types.InAssemblies(AssembliesHelper.GetAllAssemblies(AssembliesHelper.VeroAssembly.Infrastructure))
                .That()
                .Inherit(typeof(QueryHandler<,>))
                .Should()
                .HaveNameEndingWith("QueryHandler")
                .GetResult();

            result.IsSuccessful.Should()
                .BeTrue("Not all command handler  are ending with QueryHandler!: " + result.FailingTypes?.GetTypesMessage());
        }

        [Fact]
        public void AllQueryHandlersShouldContainsNameOfQueryTest()
        {
            var queryHandlers = Types.InAssemblies(AssembliesHelper.GetAllAssemblies(AssembliesHelper.VeroAssembly.Infrastructure))
                .That()
                .Inherit(typeof(QueryHandler<,>))
                .GetTypes();

            var invalidQueryHandlers = queryHandlers.Where(
                    w =>
                    {
                        var genericTypes = w.BaseType?.GenericTypeArguments;
                        if ((genericTypes?.Length ?? 0) != 2)
                            return true;

                        var command = genericTypes[0];

                        return !w.Name.StartsWith(command.Name);
                    }
                )
                .ToArray();

            invalidQueryHandlers.Should()
                .BeEmpty("Some query handler have wrong name!: " + invalidQueryHandlers?.GetTypesMessage());
        }

        [Fact]
        public void AllQueryHandlersShouldBeInternalTest()
        {
            var result = Types.InAssemblies(AssembliesHelper.GetAllAssemblies(AssembliesHelper.VeroAssembly.Infrastructure))
                .That()
                .Inherit(typeof(QueryHandler<,>))
                .Should()
                .MeetCustomRule(new InternalClassRule())
                .GetResult();

            result.IsSuccessful.Should()
                .BeTrue("Query handler should be internal!: " + result.GetFailingTypesMessage());
        }

        [Fact]
        public void AllQueryHandlersShouldResideInInfrastructureOrApplicationTest()
        {
            var result = Types.InAssemblies(AssembliesHelper.GetAllAssemblies())
                .That()
                .Inherit(typeof(QueryHandler<,>))
                .Should()
                .ResideInNamespaceContaining(".Infrastructure.")
                .Or()
                .ResideInNamespaceContaining(".Application.")
                .GetResult();

            result.IsSuccessful.Should()
                .BeTrue("Query handler should reside only in infrastructure or application layer!: " + result.GetFailingTypesMessage());
        }

        [Fact]
        public void AllQueryHandlersShouldResideInCorrespondingFolderTest()
        {
            var result = Types.InAssemblies(AssembliesHelper.GetAllAssemblies(AssembliesHelper.VeroAssembly.Infrastructure))
                .That()
                .Inherit(typeof(QueryHandler<,>))
                .And()
                .AreNotAbstract()
                .Should()
                .MeetCustomRule(new ResideInCorrespondingFolderRule("QueryHandler"))
                .GetResult();

            result.IsSuccessful.Should()
                .BeTrue("Not all query handler are reside in corresponding folder!: " + result.FailingTypes?.GetTypesMessage());
        }
    }
}