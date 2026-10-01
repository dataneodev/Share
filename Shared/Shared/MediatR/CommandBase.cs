using MediatR;
using Vero.Shared.Abstractions.Bus;

namespace Vero.Shared.MediatR
{
    public abstract class CommandBase : ICommand<Unit>
    {
        protected CommandBase(int? initiatorId, Guid correlationId)
        {
            InitiatorId = initiatorId;
            CorrelationId = correlationId;
        }
     

        public int? InitiatorId { get; set; }


        public Guid CorrelationId { get; set; }
    }

    public abstract class CommandBase<TResult> : ICommand<TResult>
    {
        protected CommandBase(int? initiatorId, Guid correlationId)
        {
            InitiatorId = initiatorId;
            CorrelationId = correlationId;
        }
       
        public int? InitiatorId { get; set; }
        
        public Guid CorrelationId { get; set; }
    }
}