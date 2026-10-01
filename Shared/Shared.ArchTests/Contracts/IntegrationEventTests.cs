using System.Reflection;
using FluentAssertions;
using NetArchTest.Rules;
using Shared.ArchTests.Rules;
using Vero.Shared.Abstractions.DDD;
using Vero.Shared.DDD;
using Vero.Shared.Extensions;
using Xunit;

namespace Shared.ArchTests.Contracts
{
    public sealed class IntegrationEventTests
    {
        [Fact]
        public void AllIntegrationEventsShouldBeSealedTest()
        {
            var result = Types.InAssemblies(AssembliesHelper.GetAllAssemblies(AssembliesHelper.VeroAssembly.Contracts))
                .That()
                .Inherit(typeof(IntegrationEvent))
                .And()
                .DoNotHaveNameEndingWith("BaseIntegrationEvent")
                .Should()
                .BeSealed()
                .GetResult();

            result.IsSuccessful.Should()
                .BeTrue("Not all integration event are sealed!: " + result.FailingTypes?.GetTypesMessage());
        }

        [Fact]
        public void AllEventsShouldEndWithIntegrationEventTest()
        {
            var result = Types.InAssemblies(AssembliesHelper.GetAllAssemblies(AssembliesHelper.VeroAssembly.Contracts))
                .That()
                .Inherit(typeof(IntegrationEvent))
                .Should()
                .HaveNameEndingWith("IntegrationEvent")
                .GetResult();

            result.IsSuccessful.Should()
                .BeTrue("Not all integration event are ending with IntegrationEvent!: " + result.FailingTypes?.GetTypesMessage());
        }

        [Fact]
        public void AllIntegrationEventsShouldResideInContractsTest()
        {
            var result = Types.InAssemblies(AssembliesHelper.GetAllAssemblies())
                .That()
                .Inherit(typeof(IntegrationEvent))
                .Should()
                .ResideInNamespaceEndingWith(".Contracts")
                .Or()
                .ResideInNamespaceContaining(".Contracts.")
                .GetResult();

            result.IsSuccessful.Should()
                .BeTrue("Integration events should reside only in contracts layer!: " + result.GetFailingTypesMessage());
        }

        [Fact]
        public void AllIntegrationEventsShouldBePublicTest()
        {
            var result = Types.InAssemblies(AssembliesHelper.GetAllAssemblies(AssembliesHelper.VeroAssembly.Contracts))
                .That()
                .Inherit(typeof(IntegrationEvent))
                .And()
                .AreNotAbstract()
                .Should()
                .MeetCustomRule(new PublicClassRule())
                .GetResult();

            result.IsSuccessful.Should()
                .BeTrue("Integration events should be public!: " + result.GetFailingTypesMessage());
        }

        [Fact]
        public void AllIntegrationEventsShouldHaveCorrespondingDomainEventTest()
        {
            var integrationEventTypes = Types.InAssemblies(AssembliesHelper.GetAllAssemblies(AssembliesHelper.VeroAssembly.Contracts))
                .That()
                .Inherit(typeof(IntegrationEvent))
                .And()
                .AreNotAbstract()
                .GetTypes()
                .ToArray();

            var domainEventTypesName = Types.InAssemblies(AssembliesHelper.GetAllAssemblies(AssembliesHelper.VeroAssembly.Domain))
                .That()
                .Inherit(typeof(DomainEvent))
                .GetTypes()
                .SelectHashSet(s => s.Name.Replace("DomainEvent", ""));

            var eventsWithMissingDomainEvents = integrationEventTypes.Where(w => !domainEventTypesName.Contains(w.Name.Replace("IntegrationEvent", "")))
                .ToArray();

            eventsWithMissingDomainEvents.Should()
                .BeEmpty("Integration events  have corresponding domain event!: " + eventsWithMissingDomainEvents.GetTypesMessage());
        }

        [Fact]
        public void AllIntegrationEventShouldNotContainsWritablePropertiesTest()
        {
            var result = Types.InAssemblies(AssembliesHelper.GetAllAssemblies(AssembliesHelper.VeroAssembly.Contracts))
                .That()
                .Inherit(typeof(IntegrationEvent))
                .GetTypes()
                .ToArray();

            var canWrite = result.Where(
                    w => w.GetProperties()
                        .Any(a => a.Name != "OccuredAt" && a.Name != "InitiatorId" && a.Name != "CorrelationId" && a.CanWrite)
                )
                .ToArray();

            canWrite.Should()
                .BeEmpty("Integration events should not contains writable properties!: " + canWrite.GetTypesMessage());
        }

        [Fact]
        public void AllIntegrationEventShouldNotContainsPrivatePropertiesTest()
        {
            var result = Types.InAssemblies(AssembliesHelper.GetAllAssemblies(AssembliesHelper.VeroAssembly.Contracts))
                .That()
                .Inherit(typeof(IntegrationEvent))
                .GetTypes()
                .ToArray();

            var canWrite = result.Where(
                    w => w.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance)
                        .Any()
                )
                .ToArray();

            canWrite.Should()
                .BeEmpty("Integration events should not contains private properties!: " + canWrite.GetTypesMessage());
        }

        [Fact]
        public void AllIntegrationEventShouldNotContainsFieldsTest()
        {
            var result = Types.InAssemblies(AssembliesHelper.GetAllAssemblies(AssembliesHelper.VeroAssembly.Contracts))
                .That()
                .Inherit(typeof(IntegrationEvent))
                .GetTypes()
                .ToArray();

            var canWrite = result.Where(
                    w => w.GetFields()
                        .Any()
                )
                .ToArray();

            canWrite.Should()
                .BeEmpty("Integration events should not contains fields!: " + canWrite.GetTypesMessage());
        }
    }
}