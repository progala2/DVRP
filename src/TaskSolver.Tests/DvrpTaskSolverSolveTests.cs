using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace _15pl04.Ucc.TaskSolver.Tests
{
    [TestClass]
    public class DvrpTaskSolverSolveTests
    {
        BinaryFormatter _formatter = new BinaryFormatter();
        [TestMethod]
        public void TestSolvingSimpleFourCarProblemWithTenClients()
        { 
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            DvrpProblem problem = new DvrpProblem(4, 100, new[]
            {
                new Depot(0, 0, 0, 700), 
            }, new[]
            {
                new Request(1, 0, -20, 0, 20),
                new Request(2, 0, -20, 0, 20),
                new Request(3, 0, -20, 0, 20),
                new Request(4, 0, -50, 0, 20),
                new Request(5, 0, -30, 0, 20),
                new Request(6, 0, -30, 0, 20),
                new Request(7, 0, -30, 0, 20),
                new Request(8, 0, -30, 0, 20),
                new Request(9, 0, -30, 0, 20),
                new Request(10, 0, -30, 0, 20),
            });

            Debug.WriteLine(stopwatch.ElapsedMilliseconds / 1000.0 + ": " + "problem created ");
            Assert.IsTrue(HelpingFunctionForTests(problem, 4, 44, stopwatch));
            stopwatch.Stop();
        }

        [TestMethod]
        public void TestSolvingOkulewiczSimpleProblem()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            DvrpProblem problem = new DvrpProblem(8, 100, new[]
            {
                new Depot(0, 0, 0, 560), 
            }, new[]
            {
                new Request(-39, 97, -29, 276, 20),
                new Request(34, -45, -21, 451, 20),
                new Request(77, -20, -28, 171, 20),
                new Request(-34, -99, -20, 365, 20),
                new Request(75, -43, -8, 479, 20),
                new Request(87, -66, -31, 546, 20),
                new Request(-55, 86, -13, 376, 20),
                new Request(-93, -3, -29, 289, 20),
            });

            Debug.WriteLine(stopwatch.ElapsedMilliseconds / 1000.0 + ": " + "problem created ");
            Assert.IsTrue(HelpingFunctionForTests(problem, 4, 680.09, stopwatch));
            stopwatch.Stop();
        }

        private bool HelpingFunctionForTests(DvrpProblem problem, int threadsCount, double expectectedResult, Stopwatch stopwatch)
        {
            
            byte[] problemData;
            using (var memoryStream = new MemoryStream())
            {
                _formatter.Serialize(memoryStream, problem);
                problemData = memoryStream.ToArray();
            }
            Debug.WriteLine(stopwatch.ElapsedMilliseconds / 1000.0 + ": " + "problem serialized");

            var taskSolver = new DvrpTaskSolver(problemData);
            var partialProblemsData = taskSolver.DivideProblem(threadsCount);
            Debug.WriteLine(stopwatch.ElapsedMilliseconds / 1000.0 + ": " + "problem divided; threadsCount=" + threadsCount);

            var tasks = new List<Task<byte[]>>(threadsCount);
            foreach (var partialProblemData in partialProblemsData)
            {
                var data = partialProblemData;
                tasks.Add(new Task<byte[]> (() =>
                {
                    DvrpTaskSolver taskSolver2 = new DvrpTaskSolver(problemData);
                    return taskSolver2.Solve(data, new TimeSpan());                   
                }
                ));
               tasks[tasks.Count - 1].Start();
            }
            Task.WaitAll(tasks.ToArray());
            Debug.WriteLine(stopwatch.ElapsedMilliseconds / 1000.0 + ": " + "partial solutions solved");

            var finalSolutionData = taskSolver.MergeSolution(tasks.Select(task => task.Result).ToArray());
            Debug.WriteLine(stopwatch.ElapsedMilliseconds / 1000.0 + ": " + "problems merged");

            bool result;
            using (var memoryStream = new MemoryStream(finalSolutionData))
            {
                var finalSolution = (DvrpSolution)_formatter.Deserialize(memoryStream);
                Debug.WriteLine(stopwatch.ElapsedMilliseconds / 1000.0 + ": " + "final time: " + finalSolution.FinalTime);
                foreach (int[] t in finalSolution.CarsRoutes)
                {
                    Debug.WriteLine(stopwatch.ElapsedMilliseconds/1000.0 + ": " + "route: " +
                                    string.Join(", ", t.Select(v => v.ToString())));
                }
                result = Math.Abs(Math.Round((double)new decimal(finalSolution.FinalTime), 2) - expectectedResult) < Double.Epsilon;
            }
            Debug.WriteLine(stopwatch.ElapsedMilliseconds / 1000.0 + ": " + "final solution deserialized");

            return result;
        }
    }
}
