using FluentValidation;
using MediatR;

namespace Vero.Shared.Validation
{
    public sealed class ValidationPipeline<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationPipeline(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);
                var errors = _validators.Select(x => x.Validate(context))
                    .SelectMany(x => x.Errors)
                    .Where(x => x != null)
                    .GroupBy(
                        x => x.PropertyName,
                        x => x.ErrorMessage,
                        (propertyName, errorMessages) => new
                        {
                            Key = propertyName,
                            Values = errorMessages.Distinct()
                                .ToArray()
                        }
                    )
                    .ToDictionary(x => x.Key, x => x.Values);

                if (errors.Any())
                    throw new ValidatorException(errors);
            }

            return await next();
        }
    }
}