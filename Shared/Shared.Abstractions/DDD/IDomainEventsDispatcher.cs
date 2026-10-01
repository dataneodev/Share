namespace Vero.Shared.Abstractions.DDD
{
    public interface IDomainEventsDispatcher
    {
        Task DispatchEvents(int? initiatorId, Guid correlationId);
    }
}