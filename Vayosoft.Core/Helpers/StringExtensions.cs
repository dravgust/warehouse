namespace Vayosoft.Core.Helpers
{
    public static class StringExtensions
    {
        public static string ToLowerFirstChar(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return char.ToLowerInvariant(input[0]) + input[1..];
        }
    }
}
