namespace LastJudge.Configuration
{
    internal class Exercise
    {
        public required string FileName { get; set; }
        public required string Method { get; set; }
        public List<Test> Tests { get; set; } = [];
    }
}
