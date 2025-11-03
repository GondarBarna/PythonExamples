using LastJudge.Configuration;
using LastJudge.Helpers;
using LastJudge.Report;

namespace LastJudge
{
    internal class TestRunner(List<ExerciseConfig> exercises)
    {
        public async Task Run()
        {
            var reportBuilder = new ReportBuilder();

            foreach (var exercise in exercises)
            {
                reportBuilder.AddExercise(exercise.FileName);

                if (!File.Exists(exercise.FileName))
                {
                    reportBuilder.AddErrorToExercise(exercise.FileName, $"Nem található .py file {exercise.FileName} néven");
                    continue;
                }

                var runner = new PythonRunner(exercise);

                try
                {
                    foreach (var test in exercise.Tests)
                    {
                        await ExecuteTest(runner, exercise.FileName, test, reportBuilder);
                    }
                }
                catch (MissingMemberException)
                {
                    reportBuilder.AddErrorToExercise(exercise.FileName, $"Nem található függvény {exercise.Method} néven");
                    continue;
                }
            }

            var report = reportBuilder.Build();

            File.WriteAllText("eredmeny.html", report);
        }

        private static async Task ExecuteTest(PythonRunner runner, string exercise, TestConfig test, ReportBuilder reportbuilder)
        {
            dynamic? result;

            try
            {
                result = await runner.Run(test.InputValues);
            }
            catch (TestException ex)
            {
                reportbuilder.AddTestResultFail(exercise, test.Name, $"Váratlan hiba történt a teszt végrehajtása közben: " +
                    $"{ex.Message}");
                return;
            }

            if (object.Equals(result, test.ExpectedOutput))
            {
                reportbuilder.AddTestResultSuccess(exercise, test.Name);
            }
            else
            {
                reportbuilder.AddTestResultFail(exercise, test.Name, $"A visszadott érték nem megfelelő. " +
                    $"Elvárt: {test.ExpectedOutput}, kapott: {result}");
            }
        }
    }
}
