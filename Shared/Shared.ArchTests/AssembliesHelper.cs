using System.Reflection;

namespace Shared.ArchTests
{
    internal static class AssembliesHelper
    {
        public enum VeroAssembly
        {
            API,
            Application,
            Domain,
            Contracts,
            Infrastructure,
            Translation
        }

        static AssembliesHelper()
        {
            _veroAssemblies = GetMatchedVeroAssemblies();
        }

        private static readonly IReadOnlyList<VeroAssemblyMatch> _veroAssemblies;

        public static IEnumerable<Assembly> GetAllAssemblies() => _veroAssemblies.Select(ass => ass.Assembly);

        public static IEnumerable<Assembly> GetAllAssemblies(VeroAssembly veroAssembly) => _veroAssemblies.Where(w => w.VeroAss == veroAssembly)
            .Select(ass => ass.Assembly);

        private static IReadOnlyList<VeroAssemblyMatch> GetMatchedVeroAssemblies()
        {
            var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (string.IsNullOrWhiteSpace(assemblyPath))
                return Array.Empty<VeroAssemblyMatch>();

            return Directory.GetFiles(assemblyPath, "*.dll", SearchOption.TopDirectoryOnly)
                     .Select(GetVeroAssemblyMatch)
                     .Where(w => w.HasValue)
                     .Select(s => s.Value)
                     .ToArray();
        }

        private readonly struct VeroAssemblyMatch(Assembly assembly, VeroAssembly veroAss)
        {
            public readonly Assembly Assembly = assembly;
            public readonly VeroAssembly VeroAss = veroAss;
        }

        private static VeroAssembly[] _veroAllAssemblies = Enum.GetValues(typeof(VeroAssembly)).Cast<VeroAssembly>().ToArray();

        private static VeroAssemblyMatch? GetVeroAssemblyMatch(string arg)
        {
            foreach (var veroAssembly in _veroAllAssemblies)
            {
                if (arg.EndsWith($".{veroAssembly}.dll"))
                    return new VeroAssemblyMatch(Assembly.LoadFile(arg), veroAssembly);
            }

            return null;
        }
    }
}
