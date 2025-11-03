using LastJudge.Helpers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LastJudge.Configuration.Converters
{
    public class ObjectListConverter : JsonConverter<List<object>>
    {
        public override List<object> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException("Expected StartArray token");
            }

            var objects = new List<object>();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    return objects;
                }

                var convertedValue = reader.TokenType.ToObject(reader, options);

                if (convertedValue is not null)
                {
                    objects.Add(convertedValue);
                }
            }

            throw new JsonException("Expected EndArray token");
        }

        public override void Write(Utf8JsonWriter writer, List<object> value, JsonSerializerOptions options)
            => JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}
