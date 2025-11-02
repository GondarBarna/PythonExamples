using LastJudge.Configuration.Converters;
using System.Text.Json.Serialization;

namespace LastJudge.Configuration
{
    internal class Test
    {
        public required string Name { get; set; }

        [JsonConverter(typeof(ObjectListConverter))]
        public List<object> InputValues { get; set; } = [];

        [JsonConverter(typeof(ObjectConverter))]
        public required object? ExpectedOutput { get; set; }
    }
}
