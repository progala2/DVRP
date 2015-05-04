﻿using System;
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
        public void TestSolvingSimpleOneCarProblemWithFiveClients()
        { 
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            DvrpProblem problem = new DvrpProblem(1, 100, new Depot[]
            {
                new Depot(0, 0, 0, 700), 
            }, new Request[]
            {
                new Request(1, 0, -20, 0, 20),
                new Request(2, 0, -20, 0, 20),
                new Request(3, 0, -20, 0, 20),
                new Request(4, 0, -20, 0, 20),
                new Request(5, 0, -20, 0, 20),
                new Request(6, 0, -20, 0, 20),
                new Request(7, 0, -20, 0, 20),
                new Request(8, 0, -20, 0, 20),
                new Request(9, 0, -20, 0, 20),
                new Request(10, 0, -20, 0, 20),
                new Request(11, 0, -20, 0, 20),
            });

            Debug.WriteLine(stopwatch.ElapsedMilliseconds / 1000.0 + ": " + "problem created ");
            HelpingFunctionForTests(problem, 8, 152, stopwatch);
            stopwatch.Restart();
            problem = new DvrpProblem(1, 100, new Depot[]
            {
                new Depot(0, 0, 0, 700), 
            }, new Request[]
            {
                new Request(1, 0, -20, 125, 20),
                new Request(2, 0, -20, 512, 20),
                new Request(3, 0, -20, 340, 20),
                new Request(4, 0, -20, 222, 20),
                new Request(5, 0, -20, 134, 20),
            });

            Debug.WriteLine(stopwatch.ElapsedMilliseconds / 1000.0 + ": " + "problem created ");
            HelpingFunctionForTests(problem, 8, 110, stopwatch);
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

            var partialSolutionsData = new List<byte[]>();
            var tasks = new List<Task<byte[]>>(threadsCount);
            foreach (var partialProblemData in partialProblemsData)
            {
               tasks.Add(new Task<byte[]> (() =>
                    {
                        return taskSolver.Solve(partialProblemData, new TimeSpan());
                    }));
               tasks[tasks.Count - 1].Start();
            }
            Task.WaitAll(tasks.ToArray());
            foreach (var task in tasks)
            {
                partialSolutionsData.Add(task.Result);
            }
            Debug.WriteLine(stopwatch.ElapsedMilliseconds / 1000.0 + ": " + "partial solutions solved");

            var finalSolutionData = taskSolver.MergeSolution(partialSolutionsData.ToArray());
            Debug.WriteLine(stopwatch.ElapsedMilliseconds / 1000.0 + ": " + "problems merged");

            bool result;
            using (var memoryStream = new MemoryStream(finalSolutionData))
            {
                var finalSolution = (DvrpSolution)_formatter.Deserialize(memoryStream);
                Debug.WriteLine(stopwatch.ElapsedMilliseconds / 1000.0 + ": " + "final time: " + finalSolution.FinalTime);
                result = Math.Abs(finalSolution.FinalTime - expectectedResult) < Double.Epsilon;
            }
            Debug.WriteLine(stopwatch.ElapsedMilliseconds / 1000.0 + ": " + "final solution deserialized");

            return result;
        }
    }
}
