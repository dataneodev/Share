using FluentAssertions;
using NetArchTest.Rules;
using Shared.ArchTests.Rules;
using Vero.Shared.DDD;
using Xunit;

namespace Shared.ArchTests.Domain
{
    public sealed class RuleTests
    {
        [Fact]
        public void AllRulesShouldEndWithRuleTest()
        {
            var result = Types.InAssemblies(AssembliesHelper.GetAllAssemblies())
                .That()
                .Inherit(typeof(BusinessRule))
                .Should()
                .HaveNameEndingWith("Rule")
                .GetResult();

            result.IsSuccessful.Should()
                .BeTrue("Not all rules are ending with Rule!: " + result.FailingTypes?.GetTypesMessage());
        }

        //[Fact]
        //public void AllRulesShouldResideInDomainTest()
        //{
        //    var result = Types.InAssemblies(AssembliesHelper.GetAllAssemblies())
        //        .That()
        //        .Inherit(typeof(BusinessRule))
        //        .Should()
        //        .ResideInNamespaceContaining(".Domain.")
        //        .GetResult();

        //    result.IsSuccessful.Should()
        //        .BeTrue("Rules should reside only in domain layer!: " + result.GetFailingTypesMessage());
        //}

        [Fact]
        public void AllRulesShouldBeSealedTest()
        {
            var result = Types.InAssemblies(AssembliesHelper.GetAllAssemblies())
                .That()
                .Inherit(typeof(BusinessRule))
                .Should()
                .BeSealed()
                .GetResult();

            result.IsSuccessful.Should()
                .BeTrue("Not all rules are sealed!: " + result.FailingTypes?.GetTypesMessage());
        }

        [Fact]
        public void AllRulesShouldBePublicTest()
        {
            var result = Types.InAssemblies(AssembliesHelper.GetAllAssemblies())
                .That()
                .Inherit(typeof(BusinessRule))
                .And()
                .AreNotNested()
                .Should()
                .MeetCustomRule(new PublicClassRule())
                .GetResult();

            result.IsSuccessful.Should()
                .BeTrue("Rules should be public!: " + result.GetFailingTypesMessage());
        }
    }
}