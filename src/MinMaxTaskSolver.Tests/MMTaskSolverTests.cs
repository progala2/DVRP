﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using _15pl04.Ucc.MinMaxTaskSolver;
using Xunit;

namespace MinMaxTaskSolver.Tests
{
    public class MmTaskSolverTests
    {
        [Fact]
        public void AllComputationsOfMinMaxTaskSolverAreCorrect()
        {
            var numbersCount = 1000*1000;
            var threadsCount = 10;
            
            var rand = new Random();

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var numbers = new List<int>(numbersCount);
            for (var i = 0; i < numbersCount; i++)
            {
                numbers.Add(rand.Next(int.MinValue, int.MaxValue));
            }
            Debug.WriteLine(stopwatch.ElapsedMilliseconds/1000.0 + ": " + numbersCount + " numbers generated ");

            var expectedMinimum = numbers.Min();
            var expectedMaximum = numbers.Max();
            Debug.WriteLine(stopwatch.ElapsedMilliseconds/1000.0 + ": " + " expected results found");

            var problem = new MmProblem(numbers.ToArray());
            Debug.WriteLine(stopwatch.ElapsedMilliseconds/1000.0 + ": " + "problem created ");

            byte[] problemData;
            using (var memoryStream = new MemoryStream())
            {
                JsonSerializer.Serialize(memoryStream, problem);
                problemData = memoryStream.ToArray();
            }
            Debug.WriteLine(stopwatch.ElapsedMilliseconds/1000.0 + ": " + "problem serialized");

            var taskSolver = new MmTaskSolver(problemData);
            var partialProblemsData = taskSolver.DivideProblem(threadsCount);
            Debug.WriteLine(stopwatch.ElapsedMilliseconds/1000.0 + ": " + "problem divided; threadsCount=" +
                            threadsCount);

            var partialSolutionsData = new List<byte[]>(partialProblemsData.Length);
            foreach (var partialProblemData in partialProblemsData)
            {
                var partialSolutionData = taskSolver.Solve(partialProblemData, new TimeSpan());
                partialSolutionsData.Add(partialSolutionData);
            }
            Debug.WriteLine(stopwatch.ElapsedMilliseconds/1000.0 + ": " + "partial solutions solved");

            var finalSolutionData = taskSolver.MergeSolution(partialSolutionsData.ToArray());
            Debug.WriteLine(stopwatch.ElapsedMilliseconds/1000.0 + ": " + "problems merged");

            using (var memoryStream = new MemoryStream(finalSolutionData))
            {
                var finalSolution = JsonSerializer.Deserialize<MmSolution>(memoryStream) ?? throw new Exception();
                Assert.Equal(finalSolution.Min, expectedMinimum);
                Assert.Equal(finalSolution.Max, expectedMaximum);
            }
            Debug.WriteLine(stopwatch.ElapsedMilliseconds/1000.0 + ": " + "final solution deserialized");

            stopwatch.Stop();
        }
    }
}