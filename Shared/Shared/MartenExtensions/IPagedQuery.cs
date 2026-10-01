namespace Vero.Shared.MartenExtensions
{
    public interface IPagedQuery
    {
        public int Page { get; }

        public int PageSize { get; }
    }
}