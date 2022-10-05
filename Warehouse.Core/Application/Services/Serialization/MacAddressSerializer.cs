using MongoDB.Bson.Serialization;
using Vayosoft.Core.SharedKernel.ValueObjects;

namespace Warehouse.Core.Application.Services.Serialization
{
    public class MacAddressSerializer : IBsonSerializer<MacAddress>
    {
        object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return Deserialize(context, args);
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, MacAddress value)
        {
            context.Writer.WriteString(value.Value);
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            if (value is MacAddress macAddress)
            {
                context.Writer.WriteString(macAddress.Value);
            }
            else
            {
                throw new NotSupportedException("This is not an MacAddress");
            }
        }

        public MacAddress Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var value = context.Reader.ReadString();
            return MacAddress.Create(value);
        }

        public Type ValueType => typeof(MacAddress);
    }

    //public class MacAddressConverter : JsonConverter<MacAddress>
    //{
    //    public override void WriteJson(JsonWriter writer, MacAddress value, JsonSerializer serializer)
    //    {
    //        writer.WriteValue(value.Value);
    //    }

    //    public override MacAddress ReadJson(JsonReader reader, Type objectType, MacAddress existingValue, bool hasExistingValue,
    //        JsonSerializer serializer)
    //    {
    //        if (reader.TokenType == JsonToken.String)
    //        {
    //            //if (reader.Value != null)
    //            return MacAddress.Create((reader.Value as string)!);
    //        }
    //        else
    //        {
    //            throw new NotSupportedException("This is not an MacAddress value");
    //        }
    //    }
    //}

    //public class MacAddressConverter : JsonConverter<MacAddress>
    //{
    //    public override MacAddress? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    //    {
    //        if (reader.TokenType == JsonTokenType.String)
    //        {
    //            return MacAddress.Create(reader.GetString()!);
    //        }
    //        else
    //        {
    //            throw new NotSupportedException("This is not an MacAddress value");
    //        }
    //    }

    //    public override void Write(Utf8JsonWriter writer, MacAddress value, JsonSerializerOptions options)
    //    {
    //        writer.WriteStringValue(value.ToString());
    //    }
    //}
}
