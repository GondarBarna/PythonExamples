using IronPython.Hosting;
using LastJudge.Configuration;
using LastJudge.Helpers;

namespace LastJudge
{
    internal class PythonRunner(ExerciseConfig exercise)
    {
        public async Task<object?> Run(IEnumerable<object> inputValues)
        {
            var engine = Python.CreateEngine();
            var scope = engine.CreateScope();

            try
            {
                engine.Execute(File.ReadAllText(exercise.FileName), scope);
                dynamic function = scope.GetVariable(exercise.Method);

                var task = Task.Run(() => engine.Operations.Invoke(function, inputValues.ToArray()));

                return await task.WaitAsync(TimeSpan.FromSeconds(exercise.ExecutionTimeoutSecond));
            }
            catch (MissingMemberException)
            {
                throw;
            }
            catch (TimeoutException)
            {
                throw new TestException($"A teszt futása meghaladta a megengedett időt ({exercise.ExecutionTimeoutSecond}mp)");
            }
            catch (Exception ex)
            {
                throw new TestException(ex.Message);
            }
            finally
            {
                try
                {
                    engine.Runtime.Shutdown();
                }
                catch { }
            }
        }
    }
}