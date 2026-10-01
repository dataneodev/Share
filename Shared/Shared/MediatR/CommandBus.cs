using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Vero.Shared.Abstractions.Bus;

namespace Vero.Shared.MediatR
{
    internal sealed class CommandBus : ICommandBus
    {
        private readonly IServiceScopeFactory _scope;

        public CommandBus(IServiceScopeFactory scope)
        {
            _scope = scope;
        }

        public async Task<TResult> Execute<TResult>(ICommand<TResult> command)
        {
            using var scope = _scope.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            return await mediator.Send(command);
        }
    }
}