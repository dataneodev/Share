using System.Reflection;
using FluentAssertions;
using NetArchTest.Rules;
using Shared.ArchTests.Rules;
using Vero.Shared.Abstractions.DDD;
using Vero.Shared.DDD;
using Vero.Shared.Extensions;
using Xunit;

namespace Shared.ArchTests.Domain
{
    public sealed class EventTests
    {
        [Fact]
        public void AllEventsShouldBeSealedTest()
        {
            var result = Types.InAssemblies(AssembliesHelper.GetAllAssemblies(AssembliesHelper.VeroAssembly.Domain))
                .That()
                .Inherit(typeof(DomainEvent))
                .And()
                .DoNotHaveNameEndingWith("BaseDomainEvent")
                .Should()
                .BeSealed()
                .GetResult();

            result.IsSuccessful.Should()
                .BeTrue("Not all events are sealed!: " + result.FailingTypes?.GetTypesMessage());
        }

        [Fact]
        public void AllEventsShouldEndWithDomainEventTest()
        {
            var result = Types.InAssemblies(AssembliesHelper.GetAllAssemblies(AssembliesHelper.VeroAssembly.Domain))
                .That()
                .Inherit(typeof(DomainEvent))
                .Should()
                .HaveNameEndingWith("DomainEvent")
                .GetResult();

            result.IsSuccessful.Should()
                .BeTrue("Not all events are ending with DomainEvent!: " + result.FailingTypes?.GetTypesMessage());
        }

        [Fact]
        public void EventShouldContainsInternalConstructorTest()
        {
            var allEntities = Types.InAssemblies(AssembliesHelper.GetAllAssemblies(AssembliesHelper.VeroAssembly.Domain))
                .That()
                .Inherit(typeof(DomainEvent))
                .GetTypes();

            var eventsWithPublicConstructor = allEntities.Where(
                    w => w.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                        .Any(c => c.IsPublic)
                )
                .ToArray();

            eventsWithPublicConstructor.Should()
                .BeEmpty("Events should not contains public constructor!: " + eventsWithPublicConstructor.GetTypesMessage());
        }

        [Fact]
        public void AllDomainEventShouldResideInDomainTest()
        {
            var result = Types.InAssemblies(AssembliesHelper.GetAllAssemblies())
                .That()
                .Inherit(typeof(DomainEvent))
                .Should()
                .ResideInNamespaceContaining(".Domain.")
                .GetResult();

            result.IsSuccessful.Should()
                .BeTrue("Domain events should reside only in domain layer!: " + result.GetFailingTypesMessage());
        }

        [Fact]
        public void AllDomainEventShouldBePublicTest()
        {
            var result = Types.InAssemblies(AssembliesHelper.GetAllAssemblies(AssembliesHelper.VeroAssembly.Domain))
                .That()
                .Inherit(typeof(DomainEvent))
                .Should()
                .MeetCustomRule(new PublicClassRule())
                .GetResult();

            result.IsSuccessful.Should()
                .BeTrue("Projectors should be public!: " + result.GetFailingTypesMessage());
        }

        [Fact]
        public void AllDomainEventsShouldHaveCorrespondingIntegrationEventTest()
        {
            var integrationEventTypesName = Types.InAssemblies(AssembliesHelper.GetAllAssemblies(AssembliesHelper.VeroAssembly.Contracts))
                .That()
                .Inherit(typeof(IntegrationEvent))
                .And()
                .AreNotAbstract()
                .GetTypes()
                .ToArray()
                .SelectHashSet(s => s.Name.Replace("IntegrationEvent", ""));

            var domainEventTypes = Types.InAssemblies(AssembliesHelper.GetAllAssemblies(AssembliesHelper.VeroAssembly.Domain))
                .That()
                .AreNotAbstract()
                .And()
                .Inherit(typeof(DomainEvent))
                .GetTypes();

            var eventsWithMissingIntegrationEvents = domainEventTypes.Where(w => !integrationEventTypesName.Contains(w.Name.Replace("DomainEvent", "")))
                .ToArray();

            eventsWithMissingIntegrationEvents.Should()
                .BeEmpty("Domain events should have corresponding integration event!: " + eventsWithMissingIntegrationEvents.GetTypesMessage());
        }

        [Fact]
        public void AllDomainEventShouldNotContainsWritablePropertiesTest()
        {
            var result = Types.InAssemblies(AssembliesHelper.GetAllAssemblies(AssembliesHelper.VeroAssembly.Domain))
                .That()
                .Inherit(typeof(DomainEvent))
                .GetTypes()
                .ToArray();

            var canWrite = result.Where(
                    w => w.GetProperties()
                        .Any(a => a.CanWrite)
                )
                .ToArray();

            canWrite.Should()
                .BeEmpty("Domain events should not contains writable properties!: " + canWrite.GetTypesMessage());
        }

        [Fact]
        public void AllDomainEventShouldNotContainsPrivatePropertiesTest()
        {
            var result = Types.InAssemblies(AssembliesHelper.GetAllAssemblies(AssembliesHelper.VeroAssembly.Domain))
                .That()
                .Inherit(typeof(DomainEvent))
                .GetTypes()
                .ToArray();

            var canWrite = result.Where(
                    w => w.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance)
                        .Any()
                )
                .ToArray();

            canWrite.Should()
                .BeEmpty("Domain events should not contains private properties!: " + canWrite.GetTypesMessage());
        }

        [Fact]
        public void AllDomainEventShouldNotContainsFieldsTest()
        {
            var result = Types.InAssemblies(AssembliesHelper.GetAllAssemblies(AssembliesHelper.VeroAssembly.Domain))
                .That()
                .Inherit(typeof(DomainEvent))
                .GetTypes()
                .ToArray();

            var canWrite = result.Where(
                    w => w.GetFields()
                        .Any()
                )
                .ToArray();

            canWrite.Should()
                .BeEmpty("Domain events should not contains fields!: " + canWrite.GetTypesMessage());
        }
    }
}