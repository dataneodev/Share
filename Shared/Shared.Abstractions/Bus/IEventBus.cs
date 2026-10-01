using MediatR;

namespace Vero.Shared.Abstractions.Bus
{
    public interface IEventBus
    {
        Task Publish(IIntegrationEvent @event);
    }

    public interface IIntegrationEvent : INotification
    {
        public int? InitiatorId { get; }

        public Guid CorrelationId { get; }

        DateTimeOffset OccuredAt { get; }

        public void Enrich(int? initiatorId, Guid correlationId);
    }
}