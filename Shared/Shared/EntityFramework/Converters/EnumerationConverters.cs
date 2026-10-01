using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using Vero.Shared.DDD;
using Vero.Shared.Extensions;

namespace Vero.Shared.EntityFramework.Converters
{
    public static class EnumerationConverters
    {
        public static PropertyBuilder<T> HasEnumerationConversion<T>(this PropertyBuilder<T> propertyBuilder)
            where T : Enumeration<T>
        {
            var converter = new ValueConverter<T, int>(o => o.Id, v => Enumeration<T>.FromId(v));

            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.HasColumnType("integer");

            return propertyBuilder;
        }

        public static PropertyBuilder<T?> HasEnumerationOrNullConversion<T>(this PropertyBuilder<T?> propertyBuilder)
            where T : Enumeration<T>
        {
            var converter = new ValueConverter<T?, int?>(o => o == null ? null : o.Id, v => !v.HasValue ? null : (T?)Enumeration<T>.FromId(v.Value));

            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.HasColumnType("integer");

            return propertyBuilder;
        }

        public static ComplexTypePropertyBuilder<T> HasEnumerationConversion<T>(this ComplexTypePropertyBuilder<T> propertyBuilder)
            where T : Enumeration<T>
        {
            var converter = new ValueConverter<T, int>(o => o.Id, v => Enumeration<T>.FromId(v));

            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.HasColumnType("integer");

            return propertyBuilder;
        }

        public static ComplexTypePropertyBuilder<T?> HasEnumerationOrNullConversion<T>(this ComplexTypePropertyBuilder<T?> propertyBuilder)
            where T : Enumeration<T>
        {
            var converter = new ValueConverter<T?, int?>(o => o == null ? null : o.Id, v => !v.HasValue ? null : (T?)Enumeration<T>.FromId(v.Value));

            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.HasColumnType("integer");

            return propertyBuilder;
        }

        public static PropertyBuilder<List<T>> HasEnumerationListConversion<T>(this PropertyBuilder<List<T>> propertyBuilder)
            where T : Enumeration<T>
        {
            var settings = JsonConfig.SerializerSettings();

            var converter = new ValueConverter<List<T>, string>(
                v => JsonConvert.SerializeObject(v.Select(s => s.Id), settings),
                v => (JsonConvert.DeserializeObject<IEnumerable<int>>(v) ?? Enumerable.Empty<int>()).Where(Enumeration<T>.Exists)
                    .Select(Enumeration<T>.FromId)
                    .ToList()
            );

            var comparer = new ValueComparer<T>(
                (l, r) => JsonConvert.SerializeObject(l) == JsonConvert.SerializeObject(r, settings),
                v => JsonConvert.SerializeObject(v, settings)
                    .GetHashCode(),
                v => JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(v, settings))!
            );

            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.HasColumnType("jsonb");

            return propertyBuilder;
        }

        public static PropertyBuilder<HashSet<T>> HasEnumerationHashSetConversion<T>(this PropertyBuilder<HashSet<T>> propertyBuilder)
            where T : Enumeration<T>
        {
            var settings = JsonConfig.SerializerSettings();

            var converter = new ValueConverter<HashSet<T>, string>(
                v => CollectionConverters.HashSetSerialize(v, settings),
                v => CollectionConverters.HashSetDeserialize<T>(v)
            );

            var comparer = new ValueComparer<HashSet<T>>(
                (l, r) => l != null && r != null && l.SequenceEqual(r),
                v => v.Aggregate(typeof(T).GetHashCode(), (hash, t) => HashCode.Combine(hash, t.GetHashCode())),
                v => v.ToHashSet()
            );

            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.Metadata.SetValueComparer(comparer);
            propertyBuilder.HasColumnType("jsonb");

            return propertyBuilder;
        }

        public static PropertyBuilder<IReadOnlyList<T>> HasEnumerationIReadOnlyListConversion<T>(this PropertyBuilder<IReadOnlyList<T>> propertyBuilder)
            where T : Enumeration<T>
        {
            var settings = JsonConfig.SerializerSettings();

            var converter = new ValueConverter<IReadOnlyList<T>, string>(
                v => JsonConvert.SerializeObject(v.Select(s => s.Id), settings),
                v => (JsonConvert.DeserializeObject<IEnumerable<int>>(v) ?? Enumerable.Empty<int>()).Where(w => Enumeration<T>.Exists(w))
                    .Select(u => Enumeration<T>.FromId(u))
                    .ToArray()
            );

            var comparer = new ValueComparer<IReadOnlyList<T>>(
                (l, r) => l != null && r != null && l.SequenceEqual(r),
                v => v.Aggregate(typeof(T).GetHashCode(), (hash, t) => HashCode.Combine(hash, t.GetHashCode())),
                v => v.ToArray()
            );

            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.Metadata.SetValueComparer(comparer);
            propertyBuilder.HasColumnType("jsonb");

            return propertyBuilder;
        }

        public static ComplexTypePropertyBuilder<IReadOnlyList<T>> HasEnumerationIReadOnlyListConversion<T>(
            this ComplexTypePropertyBuilder<IReadOnlyList<T>> propertyBuilder
        )
            where T : Enumeration<T>
        {
            var settings = JsonConfig.SerializerSettings();

            var converter = new ValueConverter<IReadOnlyList<T>, string>(
                v => JsonConvert.SerializeObject(v.Select(s => s.Id), settings),
                v => (JsonConvert.DeserializeObject<IEnumerable<int>>(v) ?? Enumerable.Empty<int>()).Where(w => Enumeration<T>.Exists(w))
                    .Select(u => Enumeration<T>.FromId(u))
                    .ToArray()
            );

            var comparer = new ValueComparer<IReadOnlyList<T>>(
                (l, r) => l != null && r != null && l.SequenceEqual(r),
                v => v.Aggregate(typeof(T).GetHashCode(), (hash, t) => HashCode.Combine(hash, t.GetHashCode())),
                v => v.ToArray()
            );

            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.Metadata.SetValueComparer(comparer);
            propertyBuilder.HasColumnType("jsonb");

            return propertyBuilder;
        }

        public static ComplexTypePropertyBuilder<HashSet<T>> HasEnumerationHashSetConversion<T>(this ComplexTypePropertyBuilder<HashSet<T>> propertyBuilder)
            where T : Enumeration<T>
        {
            var settings = JsonConfig.SerializerSettings();

            var converter = new ValueConverter<HashSet<T>, string>(
                v => CollectionConverters.HashSetSerialize(v, settings),
                v => CollectionConverters.HashSetDeserialize<T>(v)
            );

            var comparer = new ValueComparer<HashSet<T>>(
                (l, r) => l != null && r != null && l.SequenceEqual(r),
                v => v.Aggregate(typeof(T).GetHashCode(), (hash, t) => HashCode.Combine(hash, t.GetHashCode())),
                v => v.ToHashSet()
            );

            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.Metadata.SetValueComparer(comparer);
            propertyBuilder.HasColumnType("jsonb");

            return propertyBuilder;
        }
    }
}