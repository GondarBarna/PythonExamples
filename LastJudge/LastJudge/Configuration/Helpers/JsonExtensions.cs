using System.Text.Json;

namespace LastJudge.Configuration.Helpers
{
    internal static class JsonExtensions
    {
        public static object? ToObject(this JsonTokenType tokentType, Utf8JsonReader reader, JsonSerializerOptions options) => tokentType switch
        {
            JsonTokenType.True => true,
            JsonTokenType.False => false,
            JsonTokenType.Number => reader.TryGetInt64(out long l) ? l : reader.GetDouble(),
            JsonTokenType.String => reader.GetString() ?? string.Empty,
            JsonTokenType.StartObject => JsonSerializer.Deserialize<Dictionary<string, object>>(ref reader, options),
            JsonTokenType.StartArray => JsonSerializer.Deserialize<List<object>>(ref reader, options),
            JsonTokenType.Null => null,
            _ => throw new JsonException($"Unexpected token {reader.TokenType}")
        };
    }
}
