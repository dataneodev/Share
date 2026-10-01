using System.Reflection;
using Vero.Shared.DDD;

namespace Vero.Shared.Extensions
{
    public static class StreamExtensions
    {
        public static async Task<string> ReadToEndAsync(this Stream stream)
        {
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }

        public static async Task<string> ReadResourceFile(string name) => await ReadResourceFile(Assembly.GetExecutingAssembly(), name);

        public static async Task<string> ReadResourceFile(this Assembly assembly, string name)
        {
            var stream = assembly.GetManifestResourceStream(name) ?? throw new FileNotFoundException(name);
            return await stream.ReadToEndAsync();
        }

    }
}