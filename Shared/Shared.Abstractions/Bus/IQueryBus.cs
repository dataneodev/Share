using MediatR;

namespace Vero.Shared.Abstractions.Bus
{
    public interface IQuery<out TResult> : IRequest<TResult>
    {
        public int? InitiatorId { get; protected set; }
        
    }

    public interface IQueryBus
    {
        Task<TResult> Get<TResult>(IQuery<TResult> query);
    }
}