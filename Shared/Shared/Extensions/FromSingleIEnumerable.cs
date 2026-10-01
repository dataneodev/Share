using System.Collections;

namespace Vero.Shared.Extensions
{
    public readonly struct FromSingleIEnumerable<T> : IEnumerable<T>
    {
        private readonly FromSingleEnumerator<T> _fromSingleEnumerator;

        public FromSingleIEnumerable(T value)
        {
            _fromSingleEnumerator = new FromSingleEnumerator<T>(value);
        }

        public IEnumerator<T> GetEnumerator() => _fromSingleEnumerator;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private struct FromSingleEnumerator<Tin> : IEnumerator<Tin>
        {
            private readonly Tin _value;
            private readonly bool _valueSet;
            private bool _firstIncrement;

            public FromSingleEnumerator(Tin value)
            {
                _value = value;
                _valueSet = true;
                _firstIncrement = false;
            }

            public Tin Current => _value;

            object IEnumerator.Current => _value!;

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (_firstIncrement || !_valueSet)
                    return false;

                _firstIncrement = true;
                return true;
            }

            public void Reset() => _firstIncrement = false;
        }
    }

    public static class FromSingleIEnumerable
    {
        public static FromSingleIEnumerable<TSource> Get<TSource>(TSource value) => new FromSingleIEnumerable<TSource>(value);
    }
}