using System;
using System.Text.RegularExpressions;

namespace Vayosoft.Core.SharedKernel.ValueObjects
{
    public record IMEI
    {
        private static readonly Regex Pattern = new("^[0-9]{15}$");
        public string Value { get; init; }

        public IMEI(string value)
        {
            //Contract.Requires<ArgumentException>(Pattern.IsMatch(value), nameof(value));

            if (!Pattern.IsMatch(value))
                throw new ArgumentException($"{nameof(value)} needs to be defined as valid IMEI.");

            Value = value;
        }
        public override string ToString() => Value;

        public static implicit operator string(IMEI imei) => imei?.Value;
        public static implicit operator IMEI(string value) => new(value);
    }
}
