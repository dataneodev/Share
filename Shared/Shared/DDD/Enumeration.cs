using System.Reflection;
using Vero.Shared.Exceptions;

namespace Vero.Shared.DDD
{
    public abstract class Enumeration<T> : IComparable, IComparable<T>
        where T : Enumeration<T>
    {
        private static readonly IReadOnlyList<T> _all = GenerateEnumerationCollection();

        protected Enumeration(int id, string value)
        {
            Id = id;
            Value = value;
        }

        public int Id { get; }

        public string Value { get; }

        public int CompareTo(object? obj) => obj is Enumeration<T> other ? Id.CompareTo(other.Id) : -1;

        public int CompareTo(T? other) => Id.CompareTo(other?.Id ?? -1);

        public static IReadOnlyList<T> GetAll() => _all;

        public static bool Exists(int id) => GetAll()
            .Any(x => x.Id == id);

        public static bool ValueExists(string? value) => GetAll()
            .Any(x => x.Value == value);

        public static T FromId(int id)
        {
            var all = GetAll();
            return all.FirstOrDefault(item => item.Id == id) ?? throw new InvalidEnumerationIdException(id);
        }

        public static T? FromId(int? id) => !id.HasValue ? null : FromId(id.Value);

        public static T FromValue(string value)
        {
            var all = GetAll();
            return all.FirstOrDefault(item => item.Value == value) ?? throw new InvalidEnumerationValueException(value);
        }

        public static bool operator ==(Enumeration<T>? obj1, Enumeration<T>? obj2) => obj1?.Equals(obj2) ?? Equals(obj2, null);

        public static bool operator !=(Enumeration<T>? obj1, Enumeration<T>? obj2) => !(obj1 == obj2);

        public static bool operator >=(Enumeration<T>? obj1, Enumeration<T>? obj2) => obj1 is not null && obj2 is not null && obj1.Id >= obj2.Id;

        public static bool operator >(Enumeration<T>? obj1, Enumeration<T>? obj2) => obj1 is not null && obj2 is not null && obj1.Id > obj2?.Id;

        public static bool operator <=(Enumeration<T>? obj1, Enumeration<T>? obj2) => obj1 is not null && obj2 is not null && obj1.Id <= obj2?.Id;

        public static bool operator <(Enumeration<T>? obj1, Enumeration<T>? obj2) => obj1 is not null && obj2 is not null && obj1.Id < obj2.Id;

        public override string ToString() => Value;

        public override bool Equals(object? obj) => obj is Enumeration<T> other && Id.Equals(other.Id);

        public override int GetHashCode() => Id.GetHashCode();

        private static IReadOnlyList<T> GenerateEnumerationCollection()
        {
            var all = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Where(f => f.FieldType == typeof(T))
                .Select(f => f.GetValue(null))
                .Cast<T>()
                .ToList();

            var duplicates = all.GroupBy(a => a.Id)
                .Where(x => x.Count() > 1)
                .Select(s => s.Key);

            if (duplicates.Any())
            {
                throw new DuplicateIdException(duplicates);
            }

            return all;
        }
    }

    public sealed class InvalidEnumerationIdException : AppException
    {
        public InvalidEnumerationIdException(int id) : base($"Lista dostępnych kluczy nie zawiera opcji z kluczem {id}")
        {
        }
    }

    public sealed class InvalidEnumerationValueException : AppException
    {
        public InvalidEnumerationValueException(string value) : base($"Lista dostępnych wartości nie zawiera opcji z wartością '{value}'!")
        {
        }
    }

    public sealed class DuplicateIdException : AppException
    {
        public DuplicateIdException(IEnumerable<int> ids) : base($"Lista dostępnych wartości zawiera zduplikowane identyfikatory {string.Join(", ", ids)}!")
        {
        }
    }
}