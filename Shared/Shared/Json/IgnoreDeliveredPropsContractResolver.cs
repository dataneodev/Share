using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Vero.Shared.Json
{
    public sealed class IgnoreDeliveredPropsContractResolver<T> : DefaultContractResolver
    {
        private readonly IReadOnlySet<string> _ignorePropertyDuringFilter;
        private readonly Type _type;

        public IgnoreDeliveredPropsContractResolver(IEnumerable<string>? ignorePropertyDuringFilter = null)
        {
            NamingStrategy = new CamelCaseNamingStrategy { ProcessDictionaryKeys = true };

            _type = typeof(T);
            _ignorePropertyDuringFilter = new HashSet<string>(ignorePropertyDuringFilter ?? Array.Empty<string>(), StringComparer.InvariantCultureIgnoreCase);
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            property.Ignored = IsIgnored(property);
            return property;
        }

        private bool IsIgnored(JsonProperty jsonProperty)
        {
            var propertyType = jsonProperty.DeclaringType;
            if (IsIgnoredType(propertyType))
            {
                if (_ignorePropertyDuringFilter.Contains(jsonProperty.PropertyName!))
                    return false;

                return true;
            }

            return false;
        }

        private bool IsIgnoredType(Type? propertyType) =>
            !(propertyType?.Name.Equals(_type.Name) ?? false) && (propertyType?.BaseType?.Name?.Equals(_type.Name) ?? false);
    }
}