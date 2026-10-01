namespace Vero.Shared.Extensions
{
    public static class DictionaryExtensions
    {
        public static IEnumerable<K> KeysWhere<K, V>(this IReadOnlyDictionary<K, V> dictionary, Func<KeyValuePair<K, V>, bool> predicate) => dictionary
            .Where(predicate)
            .Select(s => s.Key);
    }
}