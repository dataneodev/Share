using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Vero.Shared.DDD;

namespace Vero.Shared.EntityFramework.Converters
{
    public static class DecimalConverters
    {
        public static PropertyBuilder<T> HasDecimalConversion<T>(this PropertyBuilder<T> propertyBuilder)
            where T : ValueObjectOf<decimal>
        {
            var converter = new ValueConverter<T, decimal>(o => o.Value, v => (T)Activator.CreateInstance(typeof(T), v)!);

            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.HasColumnType("numeric");

            return propertyBuilder;
        }

        public static PropertyBuilder<T> HasDecimalNullableConversion<T>(this PropertyBuilder<T> propertyBuilder)
            where T : ValueObjectOf<decimal?>
        {
            var converter = new ValueConverter<T?, decimal?>(
                o => o == null ? null : o.Value,
                v => v == null ? null : (T?)Activator.CreateInstance(typeof(T), v),
                true
            );

            propertyBuilder.IsRequired(false);
            propertyBuilder.HasConversion(converter!);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.HasColumnType("numeric");

            return propertyBuilder;
        }
    }
}