using System.Drawing;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using Vero.Shared.DDD;
using Vero.Shared.Extensions;

namespace Vero.Shared.EntityFramework.Converters
{
    public static class DefaultExtensions
    {
        public static PropertyBuilder<T> HasJsonConversion<T>(this PropertyBuilder<T> propertyBuilder)
            where T : class, new()
        {
            var settings = JsonConfig.SerializerSettings();

            var converter = new ValueConverter<T, string>(v => JsonConvert.SerializeObject(v, settings), v => JsonConvert.DeserializeObject<T>(v) ?? new T());

            var comparer = new ValueComparer<T>(
                (l, r) => JsonConvert.SerializeObject(l) == JsonConvert.SerializeObject(r, settings),
                v => JsonConvert.SerializeObject(v, settings)
                    .GetHashCode(),
                v => JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(v, settings))!
            );

            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.Metadata.SetValueComparer(comparer);
            propertyBuilder.HasColumnType("jsonb");

            return propertyBuilder;
        }

        public static PropertyBuilder<T> HasColorConversion<T>(this PropertyBuilder<T> propertyBuilder)
            where T : ValueObjectOf<Color>
        {
            var converter = new ValueConverter<T, int>(o => o.Value.ToArgb(), v => (T)Activator.CreateInstance(typeof(T), Color.FromArgb(v))!);

            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.HasColumnType("integer");

            return propertyBuilder;
        }

        public static PropertyBuilder<T> HasLongConversion<T>(this PropertyBuilder<T> propertyBuilder)
            where T : ValueObjectOf<long>
        {
            var converter = new ValueConverter<T, long>(o => o.Value, v => (T)Activator.CreateInstance(typeof(T), v)!);

            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.HasColumnType("bigint");

            return propertyBuilder;
        }

        public static PropertyBuilder<T> HasBooleanConversion<T>(this PropertyBuilder<T> propertyBuilder)
            where T : ValueObjectOf<bool>
        {
            var converter = new ValueConverter<T, bool>(o => o != null && o.Value, v => (T)Activator.CreateInstance(typeof(T), new object[] { v })!);

            propertyBuilder.HasConversion(converter!);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.HasColumnType("boolean");

            return propertyBuilder;
        }

        public static PropertyBuilder<T> HasGuidConversion<T>(this PropertyBuilder<T> propertyBuilder)
            where T : ValueObjectOf<Guid>
        {
            var converter = new ValueConverter<T, Guid>(o => o.Value, v => (T)Activator.CreateInstance(typeof(T), v)!);

            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.HasColumnType("uuid");

            return propertyBuilder;
        }

        public static ModelBuilder HasSequences<T>(this ModelBuilder builder)
        {
            var fields = typeof(T).GetFields(BindingFlags.Static | BindingFlags.NonPublic);
            foreach (var field in fields)
                builder.HasSequence<int>(field.GetValue(null)!.ToString()!);

            return builder;
        }
    }
}