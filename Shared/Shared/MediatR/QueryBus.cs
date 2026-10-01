using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Vero.Shared.Abstractions.Bus;
using Vero.Shared.Metrics;

namespace Vero.Shared.MediatR
{
    internal sealed class QueryBus : IQueryBus
    {
        private readonly IServiceScopeFactory _scope;

        public QueryBus(IServiceScopeFactory scope)
        {
            _scope = scope;
        }

        public async Task<TResult> Get<TResult>(IQuery<TResult> query)
        {
            using var scope = _scope.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            return await mediator.Send(query);
        }
    }

    public abstract class QueryHandler<TQuery, TResult> : IRequestHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        public async Task<TResult> Handle(TQuery request, CancellationToken cancellationToken)
        {
            var name = GetType().FullName!;
            var initiator = request.InitiatorId;
    
            try
            {
                var result = await Handle(request);
                VeroMetrics.QueryHandlers.QueryHandlersSuccess.Record(name, initiator);
                return result;
            }
            catch (Exception ex)
            {
                VeroMetrics.QueryHandlers.QueryHandlersFail.Record(ex, name, initiator);
                throw;
            }
            finally
            {
                VeroMetrics.QueryHandlers.QueryHandlersRun.Record(name, initiator);
            }
        }

        protected abstract Task<TResult> Handle(TQuery request);
    }
}