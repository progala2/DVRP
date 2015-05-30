using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _15pl04.Ucc.TaskSolver.Tests
{
    [TestClass]
    public class DvrpTaskSolverSolveTests
    {
        private readonly BinaryFormatter _formatter = new BinaryFormatter();

        [TestMethod]
        public void TestSolvingOkulewiczSimpleProblem()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Debug.WriteLine(stopwatch.ElapsedMilliseconds/1000.0 + ": " + "problem created ");
            Assert.IsTrue(HelpingFunctionForTests(DvrpProblems.io2_8_plain_a_D, new TimeSpan(1, 0, 0), 4, 680.09,
                stopwatch));
            stopwatch.Stop();
        }

        [TestMethod]
        public void TestSolvingOkulewiczTwelveClients()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Debug.WriteLine(stopwatch.ElapsedMilliseconds/1000.0 + ": " + "problem created ");
            Assert.IsTrue(HelpingFunctionForTests(DvrpProblems.okul12D, new TimeSpan(1, 0, 0), 4, 976.27, stopwatch));
            stopwatch.Stop();
        }

        [TestMethod]
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
                var formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, Encoding.UTF8.GetString(problem));
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
                tasks[tasks.Count - 1].Start();
            }
            Task.WaitAll(tasks.ToArray());
            Debug.WriteLine(stopwatch.ElapsedMilliseconds/1000.0 + ": " + "partial solutions solved");

            var finalSolutionData = taskSolver.MergeSolution(tasks.Select(task => task.Result).ToArray());
            Debug.WriteLine(stopwatch.ElapsedMilliseconds/1000.0 + ": " + "problems merged");

            bool result;
            using (var memoryStream = new MemoryStream(finalSolutionData))
            {
                var finalSolution = (string)_formatter.Deserialize(memoryStream);
                Debug.WriteLine(stopwatch.ElapsedMilliseconds / 1000.0 + ": " + finalSolution);
                var reg = new Regex(@"[-+]?\d+([,.]\d+)?");
                MatchCollection m;
                m = reg.Matches(finalSolution);
                result = Math.Abs(Math.Round((double)new decimal(double.Parse(m[0].Value)), 2) - expectectedResult) < Double.Epsilon;
            }
            Debug.WriteLine(stopwatch.ElapsedMilliseconds / 1000.0 + ": " + "final solution deserialized");

            return result;
        }
    }
}
