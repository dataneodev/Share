namespace Vero.Shared.Extensions
{
    public static class HashsetExtensions
    {
        public static HashSet<T> AddRange<T>(this HashSet<T> hashset, IEnumerable<T> items)
        {
            foreach (var item in items)
                hashset.Add(item);

            return hashset;
        }
    }
}