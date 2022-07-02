using System.Text.RegularExpressions;

namespace Warehouse.Core.Entities.ValueObjects
{
    public record Email
    {
        private static readonly Regex Pattern = new(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");

        public string Address { get; init; }

        private Email(string address)
        {
            if (!Pattern.IsMatch(address))
                throw new ArgumentException($"{nameof(address)} needs to be defined as valid email.");

            Address = address;
        }

        public static Email Create(string source)
        {
            return new(source);
        }
    }
}
