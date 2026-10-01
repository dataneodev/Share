using System.Dynamic;
using System.Reflection;

namespace Vero.Shared.Json
{
    /// <summary>
    ///     Zwaraca obiekt z properties które znajdują się w klasie dziedziczącej jeśli jason'a robimy z
    ///     klasy bazowej
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    public sealed class ContentObject<TInput> : DynamicObject
        where TInput : notnull, new()
    {
        private static readonly IReadOnlyList<PropertyInfo> _propertyInfos = typeof(TInput)
            .GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
            .Where(property => property.CanRead)
            .ToArray();

        private readonly Dictionary<string, object?> _properties;

        public ContentObject(TInput input, IEnumerable<string>? ignoredProperties = null)
        {
            var ignoredPropertiesSet = new HashSet<string>(ignoredProperties ?? Array.Empty<string>(), StringComparer.InvariantCultureIgnoreCase);

            _properties = _propertyInfos.Where(property => !ignoredPropertiesSet.Contains(property.Name))
                .ToDictionary(k => k.Name, v => v.GetValue(input));
        }

        public override IEnumerable<string> GetDynamicMemberNames() => _properties.Keys;

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (_properties.ContainsKey(binder.Name))
            {
                result = _properties[binder.Name]!;
                return true;
            }

            result = null!;
            return false;
        }

        public override bool TrySetMember(SetMemberBinder binder, object? value) => false;
    }
}