using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Vero.Shared.MessageBroker
{
    public abstract class RequestClient<TRequest, TResponse>
        where TRequest : class, IRequestBus<TResponse>
        where TResponse : class
    {
        private readonly IServiceProvider _service;

        protected RequestClient(IServiceProvider service)
        {
            _service = service;
        }

        protected async Task<TResponse> GetResponseAsync(TRequest request, CancellationToken cancellationToken = default)
        {
            var client = _service.GetRequiredService<IRequestClient<TRequest>>();
            var response = await client.GetResponse<TResponse>(request, cancellationToken);
            return response.Message;
        }
    }
}