namespace Vero.Shared.Extensions
{
    public static class IEnumerableExtensions
    {
        public static bool IsCollectionEquals<T>(this IEnumerable<T> l1, IEnumerable<T> l2)
            where T : IComparable<T>
        {
            return l1.OrderBy(o => o)
                .SequenceEqual(l2.OrderBy(o => o));
        }
    }
}