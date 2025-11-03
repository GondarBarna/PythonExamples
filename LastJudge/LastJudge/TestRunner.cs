using IronPython.Hosting;
using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using LastJudge.Configuration;
using LastJudge.Report;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

namespace LastJudge
{
    internal class TestRunner
    {
        private readonly List<ExerciseConfig> exercises;

        private readonly ScriptEngine engine;
        private readonly ScriptScope scope;
        private readonly CodeContext context;

        public TestRunner(List<ExerciseConfig> exercises)
        {
            this.exercises = exercises;

            engine = Python.CreateEngine();
            scope = engine.CreateScope();
            context = new CodeContext([], new ModuleContext([], DefaultContext.DefaultPythonContext));
        }

        public void Run()
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

                engine.Execute(File.ReadAllText(exercise.FileName), scope);

                dynamic methodToTest;

                try
                {
                    methodToTest = scope.GetVariable(exercise.Method);
                }
                catch (MissingMemberException)
                {
                    reportBuilder.AddErrorToExercise(exercise.FileName, $"Nem található függvény {exercise.Method} néven");
                    continue;
                }

                foreach (var test in exercise.Tests)
                {
                    ExecuteTest(methodToTest, exercise.FileName, test, reportBuilder);
                }
            }

            var report = reportBuilder.Build();

            File.WriteAllText("eredmeny.html", report);
        }

        private void ExecuteTest(dynamic methodToTest, string exercise, TestConfig test, ReportBuilder reportbuilder)
        {
            dynamic result;

            try
            {
                result = methodToTest.__call__(context, test.InputValues.ToArray());
            }
            catch (TypeErrorException)
            {
                reportbuilder.AddTestResultFail(exercise, test.Name, "A függvény paraméterei eltérnek az elvárttól");
                return;
            }
            catch (ArgumentTypeException)
            {
                reportbuilder.AddTestResultFail(exercise, test.Name, $"A függvény paramétereinek száma nem megfelelő. " +
                    $"(Evárt: {test.InputValues.Count})");
                return;
            }
            catch (Exception ex)
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
