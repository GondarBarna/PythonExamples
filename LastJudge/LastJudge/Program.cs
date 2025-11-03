using LastJudge.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace LastJudge
{
    internal class Program
    {
        private static readonly ILogger logger;

        static Program()
        {
            using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
            logger = factory.CreateLogger(nameof(Program));
        }

        private const string testCasesFileName = "TestCases.json";

        private static async Task<int> Main()
        {
            if (!File.Exists(testCasesFileName))
            {
                logger.LogError("Test cases file is not found");
                return 1;
            }

            var exercises = ParseExercises();
            var testRunner = new TestRunner(exercises);

            await testRunner.Run();

            return 0;
        }

        private static List<ExerciseConfig> ParseExercises()
        {
            List<ExerciseConfig>? exercises = null;

            try
            {
                var testCasesFileContent = File.ReadAllText(testCasesFileName);
                exercises = JsonSerializer.Deserialize<List<ExerciseConfig>>(testCasesFileContent);
            }
            catch (JsonException ex)
            {
                logger.LogError("Test cases file is structually invalid. {message}", ex.Message);
                Environment.Exit(1);
            }

            if (exercises is null)
            {
                Environment.Exit(1);
            }

            return exercises;
        }
    }
}
