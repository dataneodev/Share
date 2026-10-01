using MediatR;

namespace Vero.Shared.Abstractions.Bus
{
    public interface ICommand<out TResult> : IRequest<TResult>
    {
        public int? InitiatorId { get; protected set; }
        
        public Guid CorrelationId { get; protected set; }
    }

    public interface ICommand : IRequest<Unit>
    {
        public int? InitiatorId { get; protected set; }

        public Guid CorrelationId { get; protected set; }
    }

    public interface ICommandBus
    {
        Task<TResult> Execute<TResult>(ICommand<TResult> command);
    }



}