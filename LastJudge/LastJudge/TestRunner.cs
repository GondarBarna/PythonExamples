using IronPython.Hosting;
using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using LastJudge.Configuration;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

namespace LastJudge
{
    internal class TestRunner(List<Exercise> excercises)
    {
        public void Run()
        {
            foreach (var excercise in excercises)
            {
                if (!File.Exists(excercise.FileName))
                {
                    //report
                    continue;
                }

                var engine = Python.CreateEngine();
                ScriptScope scope = engine.CreateScope();
                engine.Execute(File.ReadAllText(excercise.FileName), scope);

                var context = new CodeContext([], new ModuleContext([], DefaultContext.DefaultPythonContext));

                dynamic methodToTest;

                try
                {
                    methodToTest = scope.GetVariable(excercise.Method);
                }
                catch (MissingMemberException)
                {
                    //report
                    continue;
                }

                foreach (var test in excercise.Tests)
                {
                    dynamic result;

                    try
                    {
                        result = methodToTest.__call__(context, test.InputValues.ToArray());
                    }
                    catch (TypeErrorException)
                    {
                        //report
                        continue;
                    }
                    catch (ArgumentTypeException)
                    {
                        //report
                        continue;
                    }

                    if (object.Equals(result, test.ExpectedOutput))
                    {
                        //report
                    }
                    else
                    {
                        //report
                    }
                }
            }
        }
    }
}
