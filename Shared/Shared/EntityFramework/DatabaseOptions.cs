namespace Vero.Shared.EntityFramework
{
    public sealed class DatabaseOptions
    {
        public string ConnectionString { get; init; }

        public bool EnableDynamicJson { get; init; }
    }
}