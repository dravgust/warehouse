using System.Runtime.CompilerServices;

namespace Vayosoft.Core.Utilities
{
    public static class Guard
    {
        public static void Assert(bool condition, 
            [CallerArgumentExpression("condition")] string message = "")
        {
            if (!condition)
            {
                throw new ArgumentException(null, message);
            }
        }

        public static void Assert(Func<bool> condition,
            [CallerArgumentExpression("condition")] string message = "")
        {
            Assert(condition());
        }

        public static T NotNull<T>(T value,
            [CallerArgumentExpression("value")] string message = "")
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(message);
            }

            if (ReferenceEquals(value, null))
            {
                throw new ArgumentNullException(message);
            }

            return value;
        }

        public static string NotEmpty(string value,
            [CallerArgumentExpression("value")] string message = "")
        {
            NotNull(value, message);

            if (value.Trim().Length == 0)
            {
                throw new ArgumentException("Parameter cannot be empty.", message);
            }

            return value;
        }

        public static void NotEmpty<T>(IEnumerable<T> value,
            [CallerArgumentExpression("value")] string message = "")
        {
            if (!value.Any())
            {
                throw new ArgumentException("Enumerable cannot be empty.", message);
            }
        }
    }
}
