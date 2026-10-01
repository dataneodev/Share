using System.Reflection;
using System.Text.RegularExpressions;
using Vero.Shared.Abstractions.DDD;
using Vero.Shared.DDD;
using Vero.Shared.Extensions;

namespace Vero.Shared.AutoMapper
{
    public static class DomainEventToIntegrationEventAssembly
    {
        private static readonly Regex _domainRegex = new("^[A-Za-z0-9]+?(?=DomainEvent$)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex _integrationRegex = new("^[A-Za-z0-9]+?(?=(Event|IntegrationEvent)$)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static void AddDomainEventToIntegrationEventMap(
            this EventProfile profile,
            Assembly domain,
            Assembly contract,
            bool throwOnNoMatchedEvents = false,
            bool mapNestedEventTypes = false
        )
        {
            var domainEventTypes = domain.GetInheritingTypes<DomainEvent>();
            var integrationEventTypes = contract.GetInheritingTypes<IntegrationEvent>();
            var eventPairs = GetEventPairs(domainEventTypes, integrationEventTypes);

#if DEVELOPMENT
            if (throwOnNoMatchedEvents)
                eventPairs.Validate(domainEventTypes, integrationEventTypes);
#endif
            if (mapNestedEventTypes)
                AddNestedTypesToProfile(eventPairs.Where(w => !profile.Events.ContainsKey(w.Key)), profile, throwOnNoMatchedEvents);

            AddTypesToProfile(eventPairs.Where(w => !profile.Events.ContainsKey(w.Key)), profile);
        }

        private static void AddTypesToProfile(IEnumerable<KeyValuePair<Type, Type>> types, EventProfile profile)
        {
            foreach (var type in types)
            {
                profile.Events.Add(type.Key, type.Value);
                profile.CreateMap(type.Key, type.Value);
            }
        }

        private static void AddNestedTypesToProfile(IEnumerable<KeyValuePair<Type, Type>> eventPairs, EventProfile profile, bool throwOnNoMatchedEvents)
        {
            var eventNestedTypePattern = new Regex("^[0-9A-Z]+_(?'event'[0-9A-Z]+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            foreach (var pair in eventPairs)
            {
                var integrationNestedTypes = GetNestedTypes(pair.Value, eventNestedTypePattern);
                if (!integrationNestedTypes.Any())
                    continue;

                var nestedTypesPair = GetNestedTypesPair(pair.Key, integrationNestedTypes, eventNestedTypePattern)
                    .ToArray();
#if DEVELOPMENT
                if (throwOnNoMatchedEvents && integrationNestedTypes.Count != nestedTypesPair.Length)
                    throw new Exception(GetNestedTypeMissingExceptionMessage(integrationNestedTypes, nestedTypesPair));
#endif

                AddNestedTypesToProfile(nestedTypesPair, profile);
            }
        }

        private static string GetNestedTypeMissingExceptionMessage(
            IReadOnlyList<Type> integrationNestedTypes,
            IReadOnlyList<KeyValuePair<Type, Type>> nestedTypesPair
        )
        {
            var missingTypes = integrationNestedTypes.Where(w => !nestedTypesPair.Any(a => a.Value == w))
                .SelectReadonlyList(s => s.Name);

            return $"Wewnętrzny typ eventu integracyjnego nie posiada mapowania: {string.Join(Environment.NewLine, missingTypes)}";
        }

        private static void AddNestedTypesToProfile(IEnumerable<KeyValuePair<Type, Type>> eventPairs, EventProfile profile)
        {
            foreach (var pair in eventPairs)
            {
                profile.CreateMap(pair.Key, pair.Value);
            }
        }

        private static IReadOnlyList<Type> GetNestedTypes(Type integrationEvent, Regex eventTypePattern)
        {
            return integrationEvent.GetNestedTypes()
                .Where(w => eventTypePattern.IsMatch(w.Name))
                .ToArray();
        }

        private static IEnumerable<KeyValuePair<Type, Type>> GetNestedTypesPair(
            Type domainEvent,
            IReadOnlyList<Type> integrationNestedTypes,
            Regex eventTypePattern
        )
        {
            var integrationNestedTypesDic = integrationNestedTypes.ToDictionary(k => k.Name, v => v);

            var domainPropertyTypes = domainEvent.GetProperties()
                .Select(prop => prop.PropertyType)
                .Select(s => s.IsGenericType && IsGenericCollection(s) ? s.GenericTypeArguments[0] : s)
                .Where(w => !w.IsValueType)
                .DistinctBy(prop => prop.Name)
                .ToDictionary(k => k.Name, v => v);

            return integrationNestedTypes.Select(s => GetIntegrationNestedTypePair(s, domainEvent.Name, domainPropertyTypes, eventTypePattern))
                .Where(w => w.HasValue)
                .Select(s => s.GetValueOrThrow());
        }

        private static bool IsGenericCollection(Type type)
        {
            var gen = type.GetGenericTypeDefinition();
            if (gen == typeof(IReadOnlyList<>))
                return true;

            if (gen == typeof(IEnumerable<>))
                return true;

            if (gen == typeof(List<>))
                return true;

            if (gen == typeof(HashSet<>))
                return true;

            if (gen == typeof(Array))
                return true;

            return false;
        }

        private static KeyValuePair<Type, Type>? GetIntegrationNestedTypePair(
            Type integrationNestedType,
            string domainEventName,
            Dictionary<string, Type> domainPropertyTypes,
            Regex eventTypePattern
        )
        {
            var match = eventTypePattern.Match(integrationNestedType.Name);
            if (!match.Success)
                return null;

            if (domainPropertyTypes.TryGetValue(match.Groups["event"].Value, out var domainPropType))
                return KeyValuePair.Create(domainPropType, integrationNestedType);

            if (domainPropertyTypes.TryGetValue($"{domainEventName}_{match.Groups["event"].Value}", out var domainPropType2))
                return KeyValuePair.Create(domainPropType2, integrationNestedType);

            return null;
        }

        private static Dictionary<Type, Type> GetEventPairs(IEnumerable<Type> domainEvents, IEnumerable<Type> integrationEvents)
        {
            var integrationEventDic = integrationEvents.Select(@event => new { Event = @event, Match = _integrationRegex.Match(@event.Name) })
                .Where(e => e.Match.Success)
                .ToDictionary(k => k.Match.Value, v => v.Event, StringComparer.InvariantCultureIgnoreCase);

            return domainEvents.Select(@event => new { Event = @event, Match = _domainRegex.Match(@event.Name) })
                .Where(e => e.Match.Success && integrationEventDic.ContainsKey(e.Match.Value))
                .ToDictionary(k => k.Event, v => integrationEventDic[v.Match.Value]);
        }

        private static void Validate(this Dictionary<Type, Type> eventPairs, IEnumerable<Type> domainEvents, IEnumerable<Type> integrationEvents)
        {
            var noMatchedDomainEvents = domainEvents.Where(@event => !eventPairs.ContainsKey(@event))
                .ToArray();

            var noMatchedIntegrationEvents = integrationEvents.Where(@event => !eventPairs.ContainsValue(@event))
                .ToArray();

            var errors = GetEventErrors(noMatchedDomainEvents, noMatchedIntegrationEvents)
                .ToArray();

            if (errors.Any())
                throw new Exception(string.Join(Environment.NewLine, errors));
        }

        private static string GetTypesName(IReadOnlyList<Type> types)
        {
            return string.Join(",\r\n", types.Select(s => $"-{s.FullName}"));
        }

        private static IEnumerable<string> GetEventErrors(IReadOnlyList<Type> domains, IReadOnlyList<Type> integrations)
        {
            if (domains.Any())
                yield return $"\r\nIstnieją eventy domenowe nie posiadające mapowania na integracyjne:\r\n{GetTypesName(domains)}\r\n";

            if (integrations.Any())
                yield return $"\r\nIstnieją eventy integracyjne nie posiadające mapowania z domenowych:\r\n{GetTypesName(integrations)}\r\n";
        }
    }
}