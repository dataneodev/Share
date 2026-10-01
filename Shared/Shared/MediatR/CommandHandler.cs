using MediatR;
using Vero.Shared.Abstractions.Bus;
using Vero.Shared.Metrics;

namespace Vero.Shared.MediatR
{
    public abstract class CommandHandler<TCommand> : IRequestHandler<TCommand, Unit>
        where TCommand : ICommand<Unit>
    {
        public async Task<Unit> Handle(TCommand request, CancellationToken cancellationToken)
        {
            await Handle(request);
            return Unit.Value;
        }

        protected abstract Task Handle(TCommand command);
    }

    public abstract class CommandHandler<TCommand, TResult> : IRequestHandler<TCommand, TResult>
        where TCommand : ICommand<TResult>
    {
        public async Task<TResult> Handle(TCommand request, CancellationToken cancellationToken)
        {
            var name = GetType()
                .FullName!;
            
            var correlationId = request.CorrelationId;
            var initiator = request.InitiatorId;
            
            
            try
            {
                var result = await Handle(request);
                VeroMetrics.CommandHandlers.CommandHandlersSuccess.Record(name, correlationId, initiator);
                return result;
            }
            catch (Exception ex)
            {
                VeroMetrics.CommandHandlers.CommandHandlersFail.Record(ex, name, correlationId, initiator);
                throw;
            }
            finally
            {
                VeroMetrics.CommandHandlers.CommandHandlersRun.Record(name, correlationId, initiator);
            }
        }

        protected abstract Task<TResult> Handle(TCommand command);
    }
}