using System.Text;

namespace LastJudge.Report
{
    internal class ReportBuilder
    {
        private readonly List<Exercise> exercises = [];

        public ReportBuilder AddExercise(string exercise)
        {
            exercises.Add(new(exercise, [], []));

            return this;
        }

        public ReportBuilder AddErrorToExercise(string exercise, string error)
        {
            var exerciseItem = exercises.SingleOrDefault(x => x.Name == exercise)
                ?? throw new InvalidOperationException($"Exercise {exercise} does not exist");

            exerciseItem.Errors.Add(error);

            return this;
        }

        public ReportBuilder AddTestResultSuccess(string exercise, string test)
        {
            var exerciseItem = exercises.SingleOrDefault(x => x.Name == exercise)
                ?? throw new InvalidOperationException($"Exercise {exercise} does not exist");


            exerciseItem.Tests.Add(new(test, true, []));

            return this;
        }

        public ReportBuilder AddTestResultFail(string exercise, string test, params string[] errors)
        {
            var exerciseItem = exercises.SingleOrDefault(x => x.Name == exercise)
                ?? throw new InvalidOperationException($"Exercise {exercise} does not exist");

            exerciseItem.Tests.Add(new(test, false, [.. errors]));

            return this;
        }

        public string Build()
        {
            var sb = new StringBuilder();

            sb.AppendLine("""
<html>
    <head>
        <title>Eredmény</title>
        
        <style>
            * {
                margin: 0;
                font-family: Arial;
            }

            body {
                display: flex;
                flex-direction: column;
                background-color: #e0e0e0;
                gap: 16px;
                margin: 8px;
            }

            .exercise {
                background-color: white;
                padding: 8px;
                border-radius: 8px;
                box-shadow: 2px 5px 10px #ababab;
                display: flex;
                flex-direction: column;
                gap: 8px;
                border: 3px solid #32a852;
            }

            .exercise.error {
                border-color: #e3001a;
            }

            .exercise.warning {
                border-color: #ffae17;
            }

            .header {
                display: flex;
                justify-content: center;
                font-size: 14pt;
                font-weight: bold;
                border-bottom: 1px solid grey;
                width: 100%;
                padding-bottom: 3px;
            }

            .tests {
                display: flex;
                flex-direction: column;
                gap: 10px
            }

            .test {
                background-color: #e1ffe1;
                padding: 5px;
                border-radius: 4px;
                display: flex;
                flex-direction: column;
                gap: 3px
            }

            .test.error {
                background-color: #ffe1e1;
            }

            .test-header {
                font-weight: 600;
                font-size: 12pt;
            }
        </style>
    </head>
    <body>
""");
            foreach (var exercise in exercises)
            {
                sb.AppendLine($"""
    <div class="exercise {(exercise.Errors.Count > 0 ? "error" : exercise.Tests.Any(x => !x.IsSucceded) ? "warning" : "")}">
        <div class="header">{exercise.Name}</div>
         {(string.Join(", ", exercise.Errors))}
        <div class="tests">
""");

                foreach (var test in exercise.Tests)
                {
                    sb.AppendLine($"""
            <div class="test {(!test.IsSucceded ? "error" : "")}">
                <div class="test-header">{test.Name}</div>
                <div class="test-body">
                    {(test.IsSucceded ? "OK" : string.Join(", ", test.Erros))}
                </div>
            </div>
""");
                }

                sb.AppendLine($"""

        </div>
    </div>
""");
            }

            sb.AppendLine("""
    </body>
</html>
""");

            return sb.ToString();
        }
    }
}
