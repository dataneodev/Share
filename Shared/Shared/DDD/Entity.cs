using Vero.Shared.Exceptions;

namespace Vero.Shared.DDD
{
    public abstract class Entity<T> : Entity
        where T : EntityId
    {
        protected internal Entity()
        {
        }

        public T Id { get; protected init; }
    }

    public abstract class Entity
    {
        private readonly List<DomainEvent> _events = new();

        public IReadOnlyCollection<DomainEvent> DomainEvents => _events;

        public int RowVersion { get; private set; }

        public void ClearDomainEvents()
        {
            _events.Clear();
        }

        protected void AddDomainEvent(DomainEvent @event)
        {
            _events.Add(@event);
            IncrementRowVersion();
        }

        protected static void CheckRule(BusinessRule rule)
        {
            if (rule.IsBroken())
                throw new BusinessRuleValidationException(rule);
        }

        protected void IncrementRowVersion()
        {
            RowVersion += 1;
        }
    }
}