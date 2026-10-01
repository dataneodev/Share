using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Vero.Shared.DDD;

namespace Vero.Shared.EntityFramework.Converters
{
    public static class FloatConverters
    {
        public static PropertyBuilder<T> HasFloatConversion<T>(this PropertyBuilder<T> propertyBuilder)
            where T : ValueObjectOf<float>
        {
            var converter = new ValueConverter<T, float>(o => o.Value, v => (T)Activator.CreateInstance(typeof(T), v)!);

            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.HasColumnType("real");

            return propertyBuilder;
        }

        public static PropertyBuilder<T?> HasFloatOrNullConversion<T>(this PropertyBuilder<T?> propertyBuilder)
            where T : ValueObjectOf<float>?
        {
            var converter = new ValueConverter<T?, float?>(
                o => o != null ? o.Value : null,
                v => v.HasValue ? (T?)Activator.CreateInstance(typeof(T), v) : null
            );

            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.HasColumnType("real");

            return propertyBuilder;
        }
    }
}