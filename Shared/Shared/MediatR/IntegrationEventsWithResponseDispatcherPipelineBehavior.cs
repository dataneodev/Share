using MediatR;
using Vero.Shared.Abstractions.DDD;

namespace Vero.Shared.MediatR
{
    public sealed class IntegrationEventsWithResponseDispatcherPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull, IRequest<TResponse>
    {
        private readonly IDomainEventsDispatcher _dispatcher;

        public IntegrationEventsWithResponseDispatcherPipelineBehavior(IDomainEventsDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var response = await next();

            if (request is CommandBase<TResponse> command)
            {
                if (command is null)
                    throw new InvalidOperationException("Command does not inherit from the base class Command");

                if (command.InitiatorId.HasValue && command.InitiatorId.Value < 1)
                    throw new InvalidOperationException("Invalid user id");

                await _dispatcher.DispatchEvents(command.InitiatorId,command.CorrelationId);
            }

            if (request is CommandBase commandBase)
            {
                if (commandBase is null)
                    throw new InvalidOperationException("Command does not inherit from the base class Command");

                if (commandBase.InitiatorId.HasValue && commandBase.InitiatorId.Value < 1)
                    throw new InvalidOperationException("Invalid user id");

                await _dispatcher.DispatchEvents(commandBase.InitiatorId,commandBase.CorrelationId);
            }

            return response;
        }
    }
}