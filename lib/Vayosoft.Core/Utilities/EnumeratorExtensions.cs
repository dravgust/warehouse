
namespace Vayosoft.Core.Utilities
{
    public static class EnumeratorExtensions
    {
        public static CustomEnumerator GetEnumerator(this Range range)
        {
            return new CustomEnumerator(range);
        }

        public static CustomEnumerator GetEnumerator(this int number)
        {
            return new CustomEnumerator(new Range(0, number));
        }
    }

    public ref struct CustomEnumerator
    {
        private readonly int _end;

        public CustomEnumerator(Range range)
        {
            if (range.End.IsFromEnd)
            {
                throw new NotSupportedException();
            }

            Current = range.Start.Value - 1;

            _end = range.End.Value;
        }

        public int Current { get; private set; }

        public bool MoveNext()
        {
            Current++;

            return Current <= _end;
        }
    }
}
