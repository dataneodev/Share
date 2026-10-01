namespace Vero.Shared.Abstractions
{
    public interface IPagination
    {
        public int? Page { get; }

        public int? PageSize { get; }
    }

    public interface ISearch
    {
        public string? Search { get; }
    }
}