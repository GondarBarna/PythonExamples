using LastJudge.Configuration.Helpers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LastJudge.Configuration.Converters
{
    public class ObjectConverter : JsonConverter<object>
    {
        public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => reader.TokenType.ToObject(reader, options);

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
            => JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}
