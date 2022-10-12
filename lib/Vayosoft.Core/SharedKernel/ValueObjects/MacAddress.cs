using System;
using System.Buffers.Text;
using System.Buffers;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Vayosoft.Core.SharedKernel.ValueObjects
{
    [JsonConverter(typeof(MacAddressTypeConverter))]
    public record MacAddress : IComparable<MacAddress>
    {
        private static readonly Regex Pattern = new (@"^([0-9A-Fa-f]{2}[:-]?){5}([0-9A-Fa-f]{2})$");
        public string InputValue { get; init; }
        public string Value { get; init; }

        private MacAddress()
        {
            Value = "000000000000";
        }

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
        public static MacAddress Empty => new();
        public override string ToString() => InputValue;

        public static implicit operator string(MacAddress mac) => mac.Value;    

        public static implicit operator MacAddress(string value) => new(value);

        public int CompareTo(MacAddress other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return string.Compare(Value, other.Value, StringComparison.Ordinal);
        }
    }

    public class MacAddressTypeConverter : JsonConverter<MacAddress>
    {
        public override MacAddress Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            return !string.IsNullOrEmpty(value) ? MacAddress.Create(value) : MacAddress.Empty;
        }

        public override void Write(Utf8JsonWriter writer, MacAddress value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.InputValue ?? string.Empty);
        }
    }

    //public class ServerDateTimeConverter : JsonConverter
    //{
    //    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    //    {
    //        writer.WriteValue(value is ServerDateTime dt ? dt.ToDouble(DateTimeKind.Utc) : string.Empty);
    //    }

    //    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    //    {
    //        if (reader.Value == null || !double.TryParse($"{reader.Value}", out var value))
    //            return null;

    //        return ServerDateTime.FromDouble(value, DateTimeKind.Utc);
    //    }

    //    public override bool CanConvert(Type objectType)
    //    {
    //        return objectType == typeof(ServerDateTime);
    //    }
    //}

    //public class MacAddressTypeConverter : TypeConverter
    //{
    //    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    //    {
    //        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    //    }

    //    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    //    {
    //        if (value is string)
    //        {
    //            return MacAddress.Create(value);
    //        }
    //        return base.ConvertFrom(context, culture, value);
    //    }
    //}
}
