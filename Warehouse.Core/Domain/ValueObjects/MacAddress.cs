using System.Text.RegularExpressions;

namespace Warehouse.Core.Domain.ValueObjects
{
    public record MacAddress : IComparable<MacAddress>
    {
        private static readonly Regex Pattern = new (@"^([0-9A-Fa-f]{2}[:-]?){5}([0-9A-Fa-f]{2})$");
        public string InputValue { get; init; }
        public string Value { get; init; }
        private MacAddress(string value)
        {
            if (!Pattern.IsMatch(value))
                throw new ArgumentException($"{nameof(value)} needs to be defined as valid MAC address.");

            InputValue = value;

            Value = value
                .Replace(":", "")
                .Replace("-", "")
                .ToUpper();
        }

        public static MacAddress Create(string value) => new(value);
        public override string ToString() => InputValue;

        public static implicit operator string(MacAddress mac) => mac.Value;

        public static implicit operator MacAddress(string value) => new(value);

        public int CompareTo(MacAddress? other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return string.Compare(Value, other.Value, StringComparison.Ordinal);
        }
    }
}
