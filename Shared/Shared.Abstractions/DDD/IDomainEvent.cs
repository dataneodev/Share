namespace Vero.Shared.Abstractions.DDD
{
    public interface IDomainEvent
    {
        public DateTimeOffset OccuredOn { get; }
    }
}