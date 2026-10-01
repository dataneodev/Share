using Vero.Shared.Abstractions.Bus;

namespace Vero.Shared.Abstractions.DDD
{
    public abstract class IntegrationEvent
    {
        public DateTimeOffset OccuredAt { get; set; }

        public int? InitiatorId { get; set; }
        

        public Guid CorrelationId { get; set; }

        public void Enrich(int? initiatorId, Guid correlationId)
        {
            InitiatorId = initiatorId;
            CorrelationId = correlationId;
            OccuredAt = DateTimeOffset.UtcNow;
        }
    }
}