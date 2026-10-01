namespace Vero.Shared.Extensions
{
    public static class NullableExtensions
    {
        public static T GetValueOrThrow<T>(this T? nullable)
            where T : struct
        {
            if (nullable.HasValue)
                return nullable.Value;

            throw new ArgumentOutOfRangeException();
        }
    }
}