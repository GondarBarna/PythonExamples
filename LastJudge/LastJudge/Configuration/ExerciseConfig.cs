namespace LastJudge.Configuration
{
    internal class ExerciseConfig
    {
        public required string FileName { get; set; }
        public required string Method { get; set; }
        public List<TestConfig> Tests { get; set; } = [];
    }
}
