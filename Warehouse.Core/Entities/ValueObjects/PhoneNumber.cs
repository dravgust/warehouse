using System.Text.RegularExpressions;

namespace Warehouse.Core.Entities.ValueObjects
{
    public record PhoneNumber   
    {
        private static readonly Regex Pattern = new("^[\\d]{5,21}$");
        public string Value { get; init; }

        public PhoneNumber(string value)
        {
            if (!Pattern.IsMatch(value))
                throw new ArgumentException($"{nameof(value)} needs to be defined as valid phone number.");

            Value = value;
        }
        public override string ToString() => Value;

        public static implicit operator string?(PhoneNumber? phoneNumber) => phoneNumber?.Value;
        public static explicit operator PhoneNumber(string value) => new(value);
    }
}