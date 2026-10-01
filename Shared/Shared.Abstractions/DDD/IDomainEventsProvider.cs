namespace Vero.Shared.Abstractions.DDD
{
    public interface IDomainEventsProvider
    {
        IReadOnlyList<IDomainEvent> GetAllDomainEvents();

        IReadOnlyList<IDomainEvent> PopAllDomainEvents();

        void ClearAllDomainEvents();
    }
}