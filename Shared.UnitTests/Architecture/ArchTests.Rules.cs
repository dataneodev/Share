using FluentAssertions;
using NetArchTest.Rules;
using ProfiBiznes.Shared.Domain.DDD;
using ProfiBiznes.Shared.Infrastructure;
using ProfiBiznes.Shared.UnitTests.Architecture.Rules;

namespace ProfiBiznes.Shared.UnitTests.Architecture
{
    public abstract partial class ArchTests
    {
        [Fact]
        [DevTest]
        public void AllRulesShouldBePublicTest()
        {
            var assemblies = AssembliesContainer.CreateFromUnitTestAssembly(GetAssembly());

            var result = Types.InAssemblies(assemblies.Available)
                .That()
                .Inherit(typeof(BusinessRule))
                .And()
                .AreNotNested()
                .Should()
                .MeetCustomRule(new PublicClassRule())
                .GetResult();

            result.IsSuccessful
                .Should()
                .BeTrue("Rules should be public!: " + result.GetFailingTypesMessage());
        }

        [Fact]
        [DevTest]
        public void AllRulesShouldBeSealedTest()
        {
            var assemblies = AssembliesContainer.CreateFromUnitTestAssembly(GetAssembly());

            var result = Types.InAssemblies(assemblies.Available)
                .That()
                .Inherit(typeof(BusinessRule))
                .And()
                .AreNotAbstract()
                .Should()
                .BeSealed()
                .GetResult();

            result.IsSuccessful
                .Should()
                .BeTrue("Not all rules are sealed!: " + result.FailingTypes?.GetTypesMessage());
        }

        [Fact]
        [DevTest]
        public void AllRulesShouldEndWithRuleTest()
        {
            var assemblies = AssembliesContainer.CreateFromUnitTestAssembly(GetAssembly());

            var result = Types.InAssemblies(assemblies.Available)
                .That()
                .Inherit(typeof(BusinessRule))
                .And()
                .AreNotAbstract()
                .Should()
                .HaveNameEndingWith("Rule")
                .GetResult();

            result.IsSuccessful
                .Should()
                .BeTrue("Not all rules are ending with Rule!: " + result.FailingTypes?.GetTypesMessage());
        }

        [Fact]
        [DevTest]
        public void AllRulesShouldResideInDomainTest()
        {
            var assemblies = AssembliesContainer.CreateFromUnitTestAssembly(GetAssembly());

            var result = Types.InAssemblies(assemblies.Available)
                .That()
                .Inherit(typeof(BusinessRule))
                .Should()
                .ResideInNamespaceContaining(".Domain.")
                .GetResult();

            result.IsSuccessful
                .Should()
                .BeTrue("Rules should reside only in domain layer!: " + result.GetFailingTypesMessage());
        }
    }
}