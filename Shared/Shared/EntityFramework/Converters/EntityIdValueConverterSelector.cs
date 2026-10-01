using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Vero.Shared.DDD;

namespace Vero.Shared.EntityFramework.Converters
{
    /// <summary>
    ///     Based on https://andrewlock.net/strongly-typed-ids-in-ef-core-using-strongly-typed-entity-ids-to-avoid-primitive-obsession-part-4/
    /// </summary>
    public sealed class EntityIdValueConverterSelector : ValueConverterSelector
    {
        private readonly ConcurrentDictionary<(Type ModelClrType, Type ProviderClrType), ValueConverterInfo> _converters = new();

        public EntityIdValueConverterSelector(ValueConverterSelectorDependencies dependencies) : base(dependencies)
        {
        }

        public override IEnumerable<ValueConverterInfo> Select(Type modelClrType, Type? providerClrType = null)
        {
            var baseConverters = base.Select(modelClrType, providerClrType);
            foreach (var converter in baseConverters)
            {
                yield return converter;
            }

            var underlyingModelType = UnwrapNullableType(modelClrType);
            var underlyingProviderType = UnwrapNullableType(providerClrType);

            if (underlyingProviderType is not null && underlyingProviderType != typeof(Guid))
            {
                yield break;
            }

            var isEntityIdValue = typeof(EntityId).IsAssignableFrom(underlyingModelType);
            if (!isEntityIdValue || underlyingModelType is null)
            {
                yield break;
            }

            var converterType = typeof(EntityIdConverter<>).MakeGenericType(underlyingModelType!);

            yield return _converters.GetOrAdd(
                (underlyingModelType, typeof(Guid)),
                _ =>
                {
                    return new ValueConverterInfo(
                        modelClrType,
                        typeof(int),
                        valueConverterInfo => (ValueConverter)Activator.CreateInstance(converterType, valueConverterInfo.MappingHints)!
                    );
                }
            );
        }

        private static Type? UnwrapNullableType(Type? type)
        {
            if (type is null)
            {
                return null;
            }

            return Nullable.GetUnderlyingType(type) ?? type;
        }
    }
}