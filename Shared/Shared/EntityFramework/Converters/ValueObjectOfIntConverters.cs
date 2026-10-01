using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Vero.Shared.DDD;

namespace Vero.Shared.EntityFramework.Converters
{
    public static class ValueObjectOfIntConverters
    {
        public static PropertyBuilder<T> HasIntConversion<T>(this PropertyBuilder<T> propertyBuilder)
            where T : ValueObjectOf<int>
        {
            var converter = new ValueConverter<T, int>(o => o.Value, v => (T)Activator.CreateInstance(typeof(T), v)!);

            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.HasColumnType("integer");

            return propertyBuilder;
        }

        public static PropertyBuilder<T> HasIntNullableConversion<T>(this PropertyBuilder<T> propertyBuilder)
            where T : ValueObjectOf<int?>
        {
            var converter = new ValueConverter<T, int?>(o => o != null ? o.Value : null, v => (T)Activator.CreateInstance(typeof(T), v)!, true);

            propertyBuilder.IsRequired(false);
            propertyBuilder.HasConversion(converter!);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.HasColumnType("integer");

            return propertyBuilder;
        }

        public static PropertyBuilder<T?> HasIntOrNullConversion<T>(this PropertyBuilder<T?> propertyBuilder)
            where T : ValueObjectOf<int>?
        {
            var converter = new ValueConverter<T?, int?>(
                o => o != null ? o.Value : null,
                v => v.HasValue ? (T?)Activator.CreateInstance(typeof(T), v.Value) : null
            );

            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.HasColumnType("integer");

            return propertyBuilder;
        }

        public static ComplexTypePropertyBuilder<T> HasIntConversion<T>(this ComplexTypePropertyBuilder<T> propertyBuilder)
            where T : ValueObjectOf<int>
        {
            var converter = new ValueConverter<T, int>(o => o.Value, v => (T)Activator.CreateInstance(typeof(T), v)!);

            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.HasColumnType("integer");

            return propertyBuilder;
        }

        public static ComplexTypePropertyBuilder<T?> HasIntOrNullConversion<T>(this ComplexTypePropertyBuilder<T?> propertyBuilder)
            where T : ValueObjectOf<int>?
        {
            var converter = new ValueConverter<T?, int?>(o => o != null ? o.Value : null, v => v.HasValue ? (T?)Activator.CreateInstance(typeof(T), v) : null);

            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.HasColumnType("integer");

            return propertyBuilder;
        }
    }
}