using MassTransit;
using Microsoft.Extensions.Logging;
using Vero.Shared.Abstractions.DDD;
using Vero.Shared.Security;

namespace Vero.Shared.Events
{
    internal sealed class DomainEventsDispatcher : IDomainEventsDispatcher
    {
        private readonly IDomainEventsProvider _events;
        private readonly ILogger<DomainEventsDispatcher> _logger;
        private readonly IContextAccessor _contextAccessor;
        private readonly IDomainEventsMapper _mapper;
        private readonly IPublishEndpoint _publisher;

        public DomainEventsDispatcher(
            IDomainEventsMapper mapper,
            IPublishEndpoint publisher,
            IDomainEventsProvider events,
            ILogger<DomainEventsDispatcher> logger,
            IContextAccessor contextAccessor
        )
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
            _events = events;
            _publisher = publisher;
            _mapper = mapper;
        }

        public async Task DispatchEvents(int? initiatorId, Guid correlationId)
        {
            var events = _events.PopAllDomainEvents();
            _logger.LogInformation("DomainEventsDispatcher accessed {Count} domain events", events.Count);

            foreach (var @event in events)
            {
                var name = @event.GetType()
                    .FullName;

                try
                {
                    var notification = _mapper.Map(@event);
                    if (notification is null)
                    {
                        _logger.LogWarning("Mapping was not found for domain event {Name}", name);
                        continue;
                    }

                    notification.Enrich(initiatorId, correlationId);

                    await _publisher.Publish((object)notification);

                    _logger.LogInformation("Published {Name}", name);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error publishing {Name}", name);
                }
            }
        }
    }
}