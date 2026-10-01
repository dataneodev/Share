using FluentAssertions;
using NSubstitute;
using ProfiBiznes.Shared.Application.Bus.Events;
using ProfiBiznes.Shared.Application.Database;
using ProfiBiznes.Shared.Application.Events;
using ProfiBiznes.Shared.Application.Security.Initiator;
using ProfiBiznes.Shared.Domain.DDD;
using ProfiBiznes.Shared.Domain.ValueObjects;
using ProfiBiznes.Shared.Infrastructure.Events;

namespace ProfiBiznes.Shared.UnitTests.Infrastructure.Events
{
    public sealed class DomainEventsDispatcherTests
    {
        [Fact]
        public async Task Dispatch_DomainEventWithIntegrationEventMapping_PublishesMappedIntegrationEvent()
        {
            // Arrange
            var domainEvent = new SomeDomainEvent();
            var initiatorId = new AccountId(1);
            var companyId = new CompanyId(1);
            var initiator = InitiatorBuilder.ForUser(initiatorId, companyId);
            var correlationId = Guid.NewGuid();
            var parentId = Guid.NewGuid();
            var integrationEvent = new SomeIntegrationEvent(initiator.ToJson(), correlationId, domainEvent.OccurredAt) { ParentCorrelationId = parentId };

            var mapper = Substitute.For<IDomainToIntegrationEventMapper>();
            mapper.Map(domainEvent, initiator, correlationId, companyId)
                .Returns(integrationEvent);

            var events = Substitute.For<IDomainEventsProvider>();
            events.PopAllDomainEvents()
                .Returns([domainEvent]);

            var publisher = Substitute.For<IIntegrationEventPublisher>();

            var uow = Substitute.For<IUnitOfWork>();

            var dispatcher = new DomainEventsDispatcher(mapper, events, publisher, uow);

            // Act
            await dispatcher.DispatchEvents(initiator, companyId, correlationId, parentId);

            // Assert
            integrationEvent.Initiator
                .AccountId
                .Should()
                .Be(initiatorId);

            integrationEvent.CorrelationId
                .Should()
                .Be(correlationId);

            integrationEvent.ParentCorrelationId
                .Should()
                .Be(parentId);

            await publisher.Received()
                .Publish(integrationEvent);
        }

        private sealed record SomeDomainEvent : DomainEvent
        {
        }

        private sealed record SomeIntegrationEvent(Initiator.InitiatorJson InitiatorJson, Guid CorrelationId, DateTimeOffset OccurredAt) : IntegrationEvent(
            InitiatorJson,
            CorrelationId,
            OccurredAt
        );
    }
}