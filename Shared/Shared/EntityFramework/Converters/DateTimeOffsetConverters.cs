using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Vero.Shared.DDD;

namespace Vero.Shared.EntityFramework.Converters
{
    public static class DateTimeOffsetConverters
    {
        public static PropertyBuilder<T> HasDateTimeOffsetConversion<T>(this PropertyBuilder<T> propertyBuilder)
            where T : ValueObjectOf<DateTimeOffset>
        {
            var converter = new ValueConverter<T, DateTimeOffset>(o => o.Value.ToUniversalTime(), v => (T)Activator.CreateInstance(typeof(T), v)!);

            propertyBuilder.HasConversion(converter!);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.HasColumnType("timestamptz");

            return propertyBuilder;
        }

        public static PropertyBuilder<T?> HasDateTimeOffsetOrNullConversion<T>(this PropertyBuilder<T?> propertyBuilder)
            where T : ValueObjectOf<DateTimeOffset>?
        {
            var converter = new ValueConverter<T?, DateTimeOffset?>(
                o => o == null ? null : o.Value.ToUniversalTime(),
                v => v == null ? null : (T?)Activator.CreateInstance(typeof(T), v)
            );

            propertyBuilder.HasConversion(converter!);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.HasColumnType("timestamptz");

            return propertyBuilder;
        }
    }
}