using Vero.Shared.Exceptions;

namespace Vero.Shared.DDD
{
    public abstract record EntityId : ValueObjectOf<int>, IComparable<EntityId>
    {
        private static Comparer<int> _comparer = Comparer<int>.Default;

        public EntityId(int value) : base(value)
        {
            if (value <= 0)
                throw new InvalidEntityIdValueException();
        }

        public int CompareTo(EntityId? other) => other is not null ? _comparer.Compare(Value, other.Value) : 1;
    }
}