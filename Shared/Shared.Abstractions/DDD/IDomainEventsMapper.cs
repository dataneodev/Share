namespace Vero.Shared.Abstractions.DDD
{
    public interface IDomainEventsMapper
    {
        IntegrationEvent? Map(IDomainEvent e);
    }
}