using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Vero.Shared.DDD;

namespace Vero.Shared.EntityFramework.Converters
{
    public static class ValueObjectOfStringConverters
    {
        public static PropertyBuilder<T> HasStringConversion<T>(this PropertyBuilder<T> propertyBuilder)
            where T : ValueObjectOf<string>
        {
            var converter = new ValueConverter<T, string>(o => o.Value, v => (T)Activator.CreateInstance(typeof(T), v)!);

            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.HasColumnType("text");

            return propertyBuilder;
        }

        public static PropertyBuilder<T> HasStringNullableConversion<T>(this PropertyBuilder<T> propertyBuilder)
            where T : ValueObjectOf<string?>
        {
            var converter = new ValueConverter<T, string?>(o => o != null ? o.Value : null, v => (T)Activator.CreateInstance(typeof(T), v)!, true);

            propertyBuilder.IsRequired(false);
            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.HasColumnType("text");

            return propertyBuilder;
        }

        public static PropertyBuilder<T?> HasStringOrNullConversion<T>(this PropertyBuilder<T?> propertyBuilder)
            where T : ValueObjectOf<string>?
        {
            var converter = new ValueConverter<T?, string?>(
                o => o == null ? null : o.Value,
                v => v != null ? (T?)Activator.CreateInstance(typeof(T), v) : null
            );

            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.HasColumnType("text");

            return propertyBuilder;
        }

        public static PropertyBuilder<T?> HasStringNullableOrNullConversion<T>(this PropertyBuilder<T?> propertyBuilder)
            where T : ValueObjectOf<string?>?
        {
            var converter = new ValueConverter<T?, string?>(o => o == null ? null : o.Value, v => (T)Activator.CreateInstance(typeof(T), v)!, true);
            propertyBuilder.IsRequired(false);
            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.HasColumnType("text");

            return propertyBuilder;
        }

        public static ComplexTypePropertyBuilder<T> HasStringConversion<T>(this ComplexTypePropertyBuilder<T> propertyBuilder)
            where T : ValueObjectOf<string>
        {
            var converter = new ValueConverter<T, string>(o => o.Value, v => (T)Activator.CreateInstance(typeof(T), v)!);

            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.HasColumnType("text");

            return propertyBuilder;
        }

        public static ComplexTypePropertyBuilder<T?> HasStringOrNullConversion<T>(this ComplexTypePropertyBuilder<T?> propertyBuilder)
            where T : ValueObjectOf<string>?
        {
            var converter = new ValueConverter<T?, string?>(
                o => o == null ? null : o.Value,
                v => v != null ? (T?)Activator.CreateInstance(typeof(T), v) : null
            );

            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.HasColumnType("text");

            return propertyBuilder;
        }

        public static ComplexTypePropertyBuilder<T> HasStringNullableConversion<T>(this ComplexTypePropertyBuilder<T> propertyBuilder)
            where T : ValueObjectOf<string?>
        {
            var converter = new ValueConverter<T, string?>(o => o != null ? o.Value : null, v => (T)Activator.CreateInstance(typeof(T), v)!, true);
            propertyBuilder.IsRequired(false);
            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.HasColumnType("text");

            return propertyBuilder;
        }
    }
}