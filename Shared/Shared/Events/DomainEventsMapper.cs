using AutoMapper;
using Vero.Shared.Abstractions.DDD;

namespace Vero.Shared.Events
{
    internal sealed class DomainEventsMapper : IDomainEventsMapper
    {
        private readonly Dictionary<Type, Type> _events;
        private readonly IMapper _mapper;

        public DomainEventsMapper(IMapper mapper, Dictionary<Type, Type> events)
        {
            _mapper = mapper;
            _events = events;
        }

        public IntegrationEvent? Map(IDomainEvent e)
        {
            if (!_events.TryGetValue(e.GetType(), out var type))
                return null;

            return (IntegrationEvent)_mapper.Map(e, e.GetType(), type);
        }
    }
}