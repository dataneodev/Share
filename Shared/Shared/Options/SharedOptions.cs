using Vero.Shared.Logging;
using Vero.Shared.Security;

namespace Vero.Shared.Options
{
    public sealed class SharedOptions
    {
        public LoggingOptions Logging { get; init; } = new();

        public AuthOptions Auth { get; init; } = new();

        public Dictionary<Type, Type> Events { get; init; } = new();

        public bool UseUnitOfWork { get; init; } = true;

        public bool UseBus { get; init; } = true;

        public bool UseValidation { get; init; } = true;

        public bool UseSwagger { get; init; } = true;

        public bool ThrowOnEventMappingNotFound { get; init; } = true;

        public bool MapNestedEventTypes { get; init; } = false;

        public HashSet<Type> SwaggerTypesForPolymorphism { get; init; }
    }
}