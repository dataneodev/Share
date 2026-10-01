using AutoMapper;
using Vero.Shared.DDD;

namespace Vero.Shared.AutoMapper
{
    public abstract class EventProfile : Profile
    {
        public Dictionary<Type, Type> Events { get; } = new();

        protected IMappingExpression<TDomainEvent, TIntegrationEvent> AddEventPair<TDomainEvent, TIntegrationEvent>()
            where TDomainEvent : DomainEvent
        {
            Events.Add(typeof(TDomainEvent), typeof(TIntegrationEvent));
            return CreateMap<TDomainEvent, TIntegrationEvent>();
        }
    }
}