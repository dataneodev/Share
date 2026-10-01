using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using Vero.Shared.DDD;
using Vero.Shared.Extensions;

namespace Vero.Shared.EntityFramework.Converters
{
    public static class CollectionConverters
    {
        public static string HashSetSerialize<T>(HashSet<T> v, JsonSerializerSettings settings)
            where T : Enumeration<T> => JsonConvert.SerializeObject(v.Select(s => s.Id), settings);

        public static HashSet<T> HashSetDeserialize<T>(string v)
            where T : Enumeration<T> => new(
            (JsonConvert.DeserializeObject<IEnumerable<int>>(v) ?? Enumerable.Empty<int>()).Where(w => Enumeration<T>.Exists(w))
            .Select(u => Enumeration<T>.FromId(u))
        );

        public static PropertyBuilder<HashSet<T>> HasValueObjectOfIntHashSetConversion<T>(this PropertyBuilder<HashSet<T>> propertyBuilder)
            where T : ValueObjectOf<int>
        {
            var settings = JsonConfig.SerializerSettings();

            var converter = new ValueConverter<HashSet<T>, string>(
                v => JsonConvert.SerializeObject(v.Select(s => s.Value), settings),
                v => (JsonConvert.DeserializeObject<IEnumerable<int>>(v) ?? Array.Empty<int>()).Select(x => (T)Activator.CreateInstance(typeof(T), x)!)
                    .ToHashSet()
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

        public static PropertyBuilder<IReadOnlyList<T>> HasValueObjectOfIntIReadOnlyListConversion<T>(this PropertyBuilder<IReadOnlyList<T>> propertyBuilder)
            where T : ValueObjectOf<int>
        {
            var settings = JsonConfig.SerializerSettings();

            var converter = new ValueConverter<IReadOnlyList<T>, string>(
                v => JsonConvert.SerializeObject(v.Select(s => s.Value), settings),
                v => (JsonConvert.DeserializeObject<IEnumerable<int>>(v) ?? Array.Empty<int>()).Select(x => (T)Activator.CreateInstance(typeof(T), x)!)
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

        public static ComplexTypePropertyBuilder<HashSet<T>> HasValueObjectOfIntHashSetConversion<T>(
            this ComplexTypePropertyBuilder<HashSet<T>> propertyBuilder
        )
            where T : ValueObjectOf<int>
        {
            var settings = JsonConfig.SerializerSettings();

            var converter = new ValueConverter<HashSet<T>, string>(
                v => JsonConvert.SerializeObject(v.Select(s => s.Value), settings),
                v => (JsonConvert.DeserializeObject<IEnumerable<int>>(v) ?? Array.Empty<int>()).Select(x => (T)Activator.CreateInstance(typeof(T), x)!)
                    .ToHashSet()
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

        public static ComplexTypePropertyBuilder<IReadOnlyList<T>> HasValueObjectOfIntIReadOnlyListConversion<T>(
            this ComplexTypePropertyBuilder<IReadOnlyList<T>> propertyBuilder
        )
            where T : ValueObjectOf<int>
        {
            var settings = JsonConfig.SerializerSettings();

            var converter = new ValueConverter<IReadOnlyList<T>, string>(
                v => JsonConvert.SerializeObject(v.Select(s => s.Value), settings),
                v => (JsonConvert.DeserializeObject<IEnumerable<int>>(v, settings) ?? Array.Empty<int>())
                    .Select(x => (T)Activator.CreateInstance(typeof(T), x)!)
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
    }
}