using Vero.Shared.Abstractions.DDD;

namespace Vero.Shared.DDD
{
    public abstract class DomainEvent : IDomainEvent
    {
        public DateTimeOffset OccuredOn { get; } = DateTimeOffset.UtcNow;
    }
}