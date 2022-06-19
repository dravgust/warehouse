using System;

namespace Vayosoft.Core.Helpers
{
    public static class Guard
    {
        public static void Assert(bool condition)
        {
            if (!condition)
                throw new Exception("Assertion failed");
        }

        public static T NotNull<T>(T value, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(parameterName))
            {
                throw new ArgumentNullException(parameterName);
            }

            if (ReferenceEquals(value, null))
            {
                throw new ArgumentNullException(parameterName);
            }

            return value;
        }

        public static string NotEmpty(string value, string parameterName)
        {
            NotNull(value, parameterName);

            if (value.Trim().Length == 0)
            {
                throw new ArgumentException($"The string parameter {parameterName} cannot be empty.");
            }

            return value;
        }
    }
}
