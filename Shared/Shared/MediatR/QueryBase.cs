using Vero.Shared.Abstractions.Bus;

namespace Vero.Shared.MediatR
{
    public abstract record QueryBase<TResult> : IQuery<TResult>
    {
        public QueryBase(int? initiatorId, CancellationToken cancellationToken)
        {
            InitiatorId = initiatorId;
            CancellationToken = cancellationToken;
        }

        public Guid? CorrelationId { get; set; }

        public int? InitiatorId { get; set; }

        public CancellationToken CancellationToken { get; }
    }
}