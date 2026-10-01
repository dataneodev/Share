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
        public void AllDomainEventShouldBePublicTest()
        {
            var assemblies = AssembliesContainer.CreateFromUnitTestAssembly(GetAssembly());

            var result = Types.InAssemblies(assemblies.Available)
                .That()
                .Inherit(typeof(DomainEvent))
                .Should()
                .MeetCustomRule(new PublicClassRule())
                .GetResult();

            result.IsSuccessful
                .Should()
                .BeTrue("Projectors should be public!: " + result.GetFailingTypesMessage());
        }

        [Fact]
        [DevTest]
        public void AllDomainEventShouldNotContainFieldsTest()
        {
            var assemblies = AssembliesContainer.CreateFromUnitTestAssembly(GetAssembly());

            var result = Types.InAssemblies(assemblies.Available)
                .That()
                .Inherit(typeof(DomainEvent))
                .GetTypes()
                .ToArray();

            var canWrite = result.Where(w => w.GetFields()
                    .Any()
                )
                .ToArray();

            canWrite.Should()
                .BeEmpty("Domain events should not contains fields!: " + canWrite.GetTypesMessage());
        }

        [Fact]
        [DevTest]
        public void AllDomainEventShouldResideInDomainTest()
        {
            var assemblies = AssembliesContainer.CreateFromUnitTestAssembly(GetAssembly());

            var result = Types.InAssemblies(assemblies.Available)
                .That()
                .Inherit(typeof(DomainEvent))
                .Should()
                .ResideInNamespaceContaining(".Domain.")
                .GetResult();

            result.IsSuccessful
                .Should()
                .BeTrue("Domain events should reside only in domain layer!: " + result.GetFailingTypesMessage());
        }

        [Fact]
        [DevTest]
        public void AllEventsShouldBeSealedTest()
        {
            var assemblies = AssembliesContainer.CreateFromUnitTestAssembly(GetAssembly());

            var result = Types.InAssemblies(assemblies.Available)
                .That()
                .Inherit(typeof(DomainEvent))
                .And()
                .AreNotAbstract()
                .And()
                .DoNotHaveNameEndingWith("BaseDomainEvent")
                .Should()
                .BeSealed()
                .GetResult();

            result.IsSuccessful
                .Should()
                .BeTrue("Not all events are sealed!: " + result.FailingTypes?.GetTypesMessage());
        }

        [Fact]
        [DevTest]
        public void AllEventsShouldEndWithDomainEventTest()
        {
            var assemblies = AssembliesContainer.CreateFromUnitTestAssembly(GetAssembly());

            var result = Types.InAssemblies(assemblies.Available)
                .That()
                .Inherit(typeof(DomainEvent))
                .Should()
                .HaveNameEndingWith("DomainEvent")
                .GetResult();

            result.IsSuccessful
                .Should()
                .BeTrue("Not all events are ending with DomainEvent!: " + result.FailingTypes?.GetTypesMessage());
        }
    }
}