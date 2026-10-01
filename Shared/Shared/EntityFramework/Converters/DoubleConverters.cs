using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Vero.Shared.DDD;

namespace Vero.Shared.EntityFramework.Converters
{
    public static class DoubleConverters
    {
        public static PropertyBuilder<T> HasDoubleConversion<T>(this PropertyBuilder<T> propertyBuilder)
            where T : ValueObjectOf<double>
        {
            var converter = new ValueConverter<T, double>(o => o.Value, v => (T)Activator.CreateInstance(typeof(T), v)!);

            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.HasColumnType("numeric");

            return propertyBuilder;
        }

        public static PropertyBuilder<T?> HasDoubleOrNullConversion<T>(this PropertyBuilder<T?> propertyBuilder)
            where T : ValueObjectOf<double>?
        {
            var converter = new ValueConverter<T?, double?>(
                o => o == null ? null : o.Value,
                v => v == null ? null : (T?)Activator.CreateInstance(typeof(T), v)
            );

            propertyBuilder.HasConversion(converter!);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.HasColumnType("numeric");

            return propertyBuilder;
        }

        public static PropertyBuilder<T> HasDoubleNullableConversion<T>(this PropertyBuilder<T> propertyBuilder)
            where T : ValueObjectOf<double?>
        {
            var converter = new ValueConverter<T, double?>(o => o == null ? null : o.Value, v => (T)Activator.CreateInstance(typeof(T), v)!, true);

            propertyBuilder.IsRequired(false);
            propertyBuilder.HasConversion(converter!);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.HasColumnType("numeric");

            return propertyBuilder;
        }
    }
}