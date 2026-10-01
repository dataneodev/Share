namespace Vero.Shared.Extensions
{
    public static class LinqExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (var item in enumeration)
            {
                action(item);
            }
        }

        public static List<TResult> SelectList<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector) => source.Select(selector)
            .ToList();

        public static IReadOnlyList<TResult> SelectReadonlyList<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector) => source
            .Select(selector)
            .ToArray();

        public static TResult[] SelectArray<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector) => source.Select(selector)
            .ToArray();

        public static HashSet<TResult> SelectHashSet<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector) => source
            .Select(selector)
            .ToHashSet();

        public static async Task<List<TResult>> SelectListAsync<TSource, TResult>(this Task<IEnumerable<TSource>> source, Func<TSource, TResult> selector)
        {
            var result = await source;
            return result.SelectList(selector);
        }

        public static async Task<List<TResult>> SelectListAsync<TSource, TResult>(this Task<List<TSource>> source, Func<TSource, TResult> selector)
        {
            var result = await source;
            return result.SelectList(selector);
        }

        public static async Task<HashSet<TResult>> SelectHashSetAsync<TSource, TResult>(this Task<IEnumerable<TSource>> source, Func<TSource, TResult> selector)
        {
            var result = await source;
            return result.SelectHashSet(selector);
        }
    }
}