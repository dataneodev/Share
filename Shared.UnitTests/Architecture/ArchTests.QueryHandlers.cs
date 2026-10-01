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
        public void AllQueryHandlersShouldBeInternalTest()
        {
            var assemblies = AssembliesContainer.CreateFromUnitTestAssembly(GetAssembly());

            var result = Types.InAssemblies(assemblies.Available)
                .That()
                .Inherit(typeof(QueryHandler<,>))
                .Should()
                .MeetCustomRule(new InternalClassRule())
                .GetResult();

            result.IsSuccessful
                .Should()
                .BeTrue("Query handler should be internal!: " + result.GetFailingTypesMessage());
        }

        [Fact]
        [DevTest]
        public void AllQueryHandlersShouldBeSealedTest()
        {
            var assemblies = AssembliesContainer.CreateFromUnitTestAssembly(GetAssembly());

            var result = Types.InAssemblies(assemblies.Available)
                .That()
                .Inherit(typeof(QueryHandler<,>))
                .Should()
                .BeSealed()
                .GetResult();

            result.IsSuccessful
                .Should()
                .BeTrue("Not all query handler are sealed!: " + result.FailingTypes?.GetTypesMessage());
        }

        [Fact]
        [DevTest]
        public void AllQueryHandlersShouldContainsNameOfQueryTest()
        {
            var assemblies = AssembliesContainer.CreateFromUnitTestAssembly(GetAssembly());

            var result = Types.InAssemblies(assemblies.Available)
                .That()
                .Inherit(typeof(QueryHandler<,>))
                .GetTypes();

            var invalidQueryHandlers = result.Where(w =>
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
                .BeEmpty("Some query handler have wrong name!: " + invalidQueryHandlers.GetTypesMessage());
        }

        [Fact]
        [DevTest]
        public void AllQueryHandlersShouldEndWithQueryHandlerTest()
        {
            var assemblies = AssembliesContainer.CreateFromUnitTestAssembly(GetAssembly());

            var result = Types.InAssemblies(assemblies.Available)
                .That()
                .Inherit(typeof(QueryHandler<,>))
                .Should()
                .HaveNameEndingWith("QueryHandler")
                .GetResult();

            result.IsSuccessful
                .Should()
                .BeTrue("Not all command handler  are ending with QueryHandler!: " + result.FailingTypes?.GetTypesMessage());
        }

        [Fact]
        [DevTest]
        public void AllQueryHandlersShouldResideInCorrespondingFolderTest()
        {
            var assemblies = AssembliesContainer.CreateFromUnitTestAssembly(GetAssembly());

            var result = Types.InAssemblies(assemblies.Available)
                .That()
                .Inherit(typeof(QueryHandler<,>))
                .And()
                .AreNotAbstract()
                .Should()
                .MeetCustomRule(new ResideInCorrespondingFolderRule("QueryHandler"))
                .GetResult();

            result.IsSuccessful
                .Should()
                .BeTrue("Not all query handler are reside in corresponding folder!: " + result.FailingTypes?.GetTypesMessage());
        }

        //Mogą być query w innych częściach aplikacji
        // [Fact]
        // [DevTest]
        // public void AllQueryHandlersShouldResideInDataTest()
        // {
        //     var assemblies = AssembliesContainer.CreateFromUnitTestAssembly(GetAssembly());
        //
        //     var result = Types.InAssemblies(assemblies.Available)
        //         .That()
        //         .Inherit(typeof(QueryHandler<,>))
        //         .Should()
        //         .ResideInNamespaceContaining(".Data.")
        //         .GetResult();
        //
        //     result.IsSuccessful
        //         .Should()
        //         .BeTrue("Query handler should reside only in data layer!: " + result.GetFailingTypesMessage());
        // }
    }
}