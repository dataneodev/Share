using MediatR;
using Vero.Shared.Abstractions;
using Vero.Shared.Abstractions.Bus;

namespace Vero.Shared.EntityFramework
{
    /// <summary>
    ///     Pipeline for commiting all changes in transactions for commands
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public sealed class UnitOfWorkPipeline<TRequest, TResponse>(IUnitOfWork uow) : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull, IRequest<TResponse>

    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var result = await next();

            if (request is ICommand || request is ICommand<TResponse>)
            {
                await uow.Save(cancellationToken);
            }

            return result;
        }
    }
}