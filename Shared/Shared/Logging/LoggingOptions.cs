namespace Vero.Shared.Logging
{
    public sealed class LoggingOptions
    {
        public bool Enabled { get; init; } = true;

        public bool Request { get; init; } = true;

        public bool Response { get; init; } = true;
    }
}