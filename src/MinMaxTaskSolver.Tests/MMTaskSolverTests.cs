using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using _15pl04.Ucc.MinMaxTaskSolver;

namespace MinMaxTaskSolver.Tests
{
    [TestClass]
    public class MMTaskSolverTests
    {
        [TestMethod]
        public void AllComputationsOfMinMaxTaskSolverAreCorrect()
        {
            var numbersCount = 1000 * 1000;
            var threadsCount = 10;

            var formatter = new BinaryFormatter();
            var rand = new Random();

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var numbers = new List<int>(numbersCount);
            for (int i = 0; i < numbersCount; i++)
            {
                numbers.Add(rand.Next(int.MinValue, int.MaxValue));
            }
            Debug.WriteLine(stopwatch.ElapsedMilliseconds / 1000.0 + ": " + numbersCount + " numbers generated ");

            var expectedMinimum = numbers.Min();
            var expectedMaximum = numbers.Max();
            Debug.WriteLine(stopwatch.ElapsedMilliseconds / 1000.0 + ": " + " expected results found");

            var problem = new MMProblem(numbers);
            Debug.WriteLine(stopwatch.ElapsedMilliseconds / 1000.0 + ": " + "problem created ");

            byte[] problemData;
            using (var memoryStream = new MemoryStream())
            {
                formatter.Serialize(memoryStream, problem);
                problemData = memoryStream.ToArray();
            }
            Debug.WriteLine(stopwatch.ElapsedMilliseconds / 1000.0 + ": " + "problem serialized");

            var taskSolver = new MMTaskSolver(problemData);
            var partialProblemsData = taskSolver.DivideProblem(threadsCount);
            Debug.WriteLine(stopwatch.ElapsedMilliseconds / 1000.0 + ": " + "problem divided; threadsCount=" + threadsCount);

            var partialSolutionsData = new List<byte[]>(partialProblemsData.Length);
            foreach (var partialProblemData in partialProblemsData)
            {
                var partialSolutionData = taskSolver.Solve(partialProblemData, new TimeSpan());
                partialSolutionsData.Add(partialSolutionData);
            }
            Debug.WriteLine(stopwatch.ElapsedMilliseconds / 1000.0 + ": " + "partial solutions solved");

            var finalSolutionData = taskSolver.MergeSolution(partialSolutionsData.ToArray());
            Debug.WriteLine(stopwatch.ElapsedMilliseconds / 1000.0 + ": " + "problems merged");

            using (var memoryStream = new MemoryStream(finalSolutionData))
            {
                var finalSolution = (MMSolution)formatter.Deserialize(memoryStream);
                Assert.AreEqual(finalSolution.Min, expectedMinimum);
                Assert.AreEqual(finalSolution.Max, expectedMaximum);
            }
            Debug.WriteLine(stopwatch.ElapsedMilliseconds / 1000.0 + ": " + "final solution deserialized");

            stopwatch.Stop();
        }
    }
}
