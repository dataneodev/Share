using Microsoft.EntityFrameworkCore;
using Vero.Shared.Abstractions.DDD;
using Vero.Shared.DDD;

namespace Vero.Shared.Events
{
    internal sealed class DomainEventsProvider<T> : IDomainEventsProvider
        where T : DbContext
    {
        private readonly T _context;

        public DomainEventsProvider(T context)
        {
            _context = context;
        }

        public IReadOnlyList<IDomainEvent> GetAllDomainEvents()
        {
            var events = _context.ChangeTracker.Entries<Entity>()
                .Where(x => x.Entity.DomainEvents.Any())
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            return events;
        }

        public void ClearAllDomainEvents() => _context.ChangeTracker.Entries<Entity>()
            .Where(x => x.Entity.DomainEvents.Any())
            .ToList()
            .ForEach(entity => entity.Entity.ClearDomainEvents());

        public IReadOnlyList<IDomainEvent> PopAllDomainEvents()
        {
            var events = GetAllDomainEvents();
            ClearAllDomainEvents();
            return events;
        }
    }
}