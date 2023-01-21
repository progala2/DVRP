using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _15pl04.Ucc.TaskSolver.Tests
{
    public class DvrpTaskSolverSolveTests
    {
	    [Fact]
        public void TestSolvingOkulewiczSimpleProblem()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Debug.WriteLine(stopwatch.ElapsedMilliseconds/1000.0 + ": " + "problem created ");
            Assert.True(HelpingFunctionForTests(DvrpProblems.io2_8_plain_a_D, new TimeSpan(1, 0, 0), 2, 680.09,
                stopwatch));
            stopwatch.Stop();
        }

        [Fact]
        public void TestSolvingOkulewiczElevenClients()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Debug.WriteLine(stopwatch.ElapsedMilliseconds / 1000.0 + ": " + "problem created ");
            Assert.True(HelpingFunctionForTests(DvrpProblems.io2_11_plain_a_D, new TimeSpan(1, 0, 0), 4, 721.38, stopwatch));
            stopwatch.Stop();
        }

        [Fact]
        public void TestSolvingOkulewiczTwelveClients()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Debug.WriteLine(stopwatch.ElapsedMilliseconds/1000.0 + ": " + "problem created ");
            Assert.True(HelpingFunctionForTests(DvrpProblems.okul12D, new TimeSpan(1, 0, 0), 4, 976.27, stopwatch));
            stopwatch.Stop();
        }

        [Fact]
        public void TestSolvingOkulewiczThirteenClients()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Debug.WriteLine(stopwatch.ElapsedMilliseconds/1000.0 + ": " + "problem created ");
            Assert.True(HelpingFunctionForTests(DvrpProblems.okul13D, new TimeSpan(1, 0, 0), 4, 1154.38, stopwatch));
            stopwatch.Stop();
        }

        [Fact]
        public void TestSolvingOkulewiczFourteenClients()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Debug.WriteLine(stopwatch.ElapsedMilliseconds / 1000.0 + ": " + "problem created ");
            Assert.True(HelpingFunctionForTests(DvrpProblems.okul14D, new TimeSpan(1, 0, 0), 4, 948.59, stopwatch));
            stopwatch.Stop();
        }

        [Fact]
        public void TestSolvingWithTimeout()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            Debug.WriteLine(stopwatch.ElapsedMilliseconds/1000.0 + ": " + "problem created ");
            HelpingFunctionForTests(DvrpProblems.okul17D, new TimeSpan(0, 0, 3), 4, 44, stopwatch);
            stopwatch.Stop();
        }

        private bool HelpingFunctionForTests(byte[] problem, TimeSpan timeout, int threadsCount,
            double expectectedResult, Stopwatch stopwatch)
        {
             Debug.WriteLine(stopwatch.ElapsedMilliseconds/1000.0 + ": " + "problem serialized");

            byte[] problemData;
            using (var memoryStream = new MemoryStream())
            {
                JsonSerializer.Serialize(memoryStream, Encoding.UTF8.GetString(problem));
                problemData = memoryStream.ToArray();
            }
            var taskSolver = new DvrpTaskSolver(problemData);
            var partialProblemsData = taskSolver.DivideProblem(threadsCount);
            Debug.WriteLine(stopwatch.ElapsedMilliseconds/1000.0 + ": " + "problem divided; threadsCount=" +
                            threadsCount);

            var tasks = new List<Task<byte[]>>(threadsCount);
            foreach (var partialProblemData in partialProblemsData)
            {
                var data = partialProblemData;
                tasks.Add(new Task<byte[]>(() =>
                {
                    var taskSolver2 = new DvrpTaskSolver(problemData);
                    return taskSolver2.Solve(data, timeout);
                }
                    ));
                tasks[^1].Start();
            }
            Task.WaitAll(tasks.ToArray());
            Debug.WriteLine(stopwatch.ElapsedMilliseconds/1000.0 + ": " + "partial solutions solved");

            var finalSolutionData = taskSolver.MergeSolution(tasks.Select(task => task.Result).ToArray());
            Debug.WriteLine(stopwatch.ElapsedMilliseconds/1000.0 + ": " + "problems merged");

            bool result;
            using (var memoryStream = new MemoryStream(finalSolutionData))
            {
                var finalSolution = JsonSerializer.Deserialize<string>(memoryStream);
                Debug.WriteLine(stopwatch.ElapsedMilliseconds / 1000.0 + ": " + finalSolution);
                var reg = new Regex(@"[-+]?\d+([,.]\d+)?");
                MatchCollection m;
                m = reg.Matches(finalSolution);
                result = Math.Abs(Math.Round((double)new decimal(double.Parse(m[0].Value)), 2) - expectectedResult) < double.Epsilon;
            }
            Debug.WriteLine(stopwatch.ElapsedMilliseconds / 1000.0 + ": " + "final solution deserialized");

            return result;
        }
    }
}
