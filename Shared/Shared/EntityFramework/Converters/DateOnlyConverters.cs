using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Vero.Shared.DDD;

namespace Vero.Shared.EntityFramework.Converters
{
    public static class DateOnlyConverters
    {
        public static PropertyBuilder<T> HasDateOnlyConversion<T>(this PropertyBuilder<T> propertyBuilder)
            where T : ValueObjectOf<DateOnly>
        {
            var converter = new ValueConverter<T, DateOnly>(o => o.Value, v => (T)Activator.CreateInstance(typeof(T), v)!);

            propertyBuilder.HasConversion(converter!);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.HasColumnType("date");

            return propertyBuilder;
        }

        public static PropertyBuilder<T> HasDateOnlyNullableConversion<T>(this PropertyBuilder<T> propertyBuilder)
            where T : ValueObjectOf<DateOnly?>
        {
            var converter = new ValueConverter<T, DateOnly?>(o => o == null ? null : o.Value, v => (T)Activator.CreateInstance(typeof(T), v)!, true);

            propertyBuilder.IsRequired(false);
            propertyBuilder.HasConversion(converter!);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.HasColumnType("date");

            return propertyBuilder;
        }
    }
}