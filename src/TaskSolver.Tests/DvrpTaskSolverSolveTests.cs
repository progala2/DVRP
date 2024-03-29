﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dvrp.Ucc.Commons.Exceptions;
using Xunit.Abstractions;

namespace Dvrp.Ucc.TaskSolver.Tests
{
	public class DvrpTaskSolverSolveTests
    {
	    private readonly ITestOutputHelper _testOutputHelper;

	    public DvrpTaskSolverSolveTests(ITestOutputHelper testOutputHelper)
	    {
		    _testOutputHelper = testOutputHelper;
	    }

	    [Fact]
        public void TestSolvingSimpleProblem()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            _testOutputHelper.WriteLine(stopwatch.ElapsedTicks + ": " + "problem created ");
            Assert.True(HelpingFunctionForTests(DvrpProblems.io2_8_plain_a_D, new TimeSpan(1, 0, 0), 2, 680.09,
                stopwatch));
            stopwatch.Stop();
        }

        [Fact]
        public void TestSolvingElevenClients()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            _testOutputHelper.WriteLine(stopwatch.ElapsedTicks + ": " + "problem created ");
            Assert.True(HelpingFunctionForTests(DvrpProblems.io2_11_plain_a_D, new TimeSpan(1, 0, 0), 4, 721.38, stopwatch));
            stopwatch.Stop();
        }

        [Fact]
        public void TestSolvingTwelveClients()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            _testOutputHelper.WriteLine(stopwatch.ElapsedTicks + ": " + "problem created ");
            Assert.True(HelpingFunctionForTests(DvrpProblems.okul12D, new TimeSpan(1, 0, 0), 4, 976.27, stopwatch));
            stopwatch.Stop();
        }

        [Fact]
        public void TestSolvingThirteenClients()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            _testOutputHelper.WriteLine(stopwatch.ElapsedTicks + ": " + "problem created ");
            Assert.True(HelpingFunctionForTests(DvrpProblems.okul13D, new TimeSpan(1, 0, 0), 4, 1154.38, stopwatch));
            stopwatch.Stop();
        }

        [Fact]
        public void TestSolvingFourteenClients()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            _testOutputHelper.WriteLine(stopwatch.ElapsedTicks + ": " + "problem created ");
            Assert.True(HelpingFunctionForTests(DvrpProblems.okul14D, new TimeSpan(1, 0, 0), 4, 948.59, stopwatch));
            stopwatch.Stop();
        }

        [Fact]
        public void TestSolvingWithTimeout()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            _testOutputHelper.WriteLine(stopwatch.ElapsedTicks + ": " + "problem created ");
            HelpingFunctionForTests(DvrpProblems.okul17D, new TimeSpan(0, 0, 3), 4, 44, stopwatch);
            stopwatch.Stop();
        }

        private bool HelpingFunctionForTests(byte[] problem, TimeSpan timeout, int threadsCount,
            double expectedResult, Stopwatch stopwatch)
        {
	        _testOutputHelper.WriteLine(stopwatch.ElapsedMilliseconds/1000.0 + ": " + "problem serialized");

            byte[] problemData;
            using (var memoryStream = new MemoryStream())
            {
                JsonSerializer.Serialize(memoryStream, Encoding.UTF8.GetString(problem));
                problemData = memoryStream.ToArray();
            }
            var taskSolver = new DvrpTaskSolver(problemData);
            var partialProblemsData = taskSolver.DivideProblem(threadsCount);
            _testOutputHelper.WriteLine(stopwatch.ElapsedMilliseconds/1000.0 + ": " + "problem divided; threadsCount=" +
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
            _testOutputHelper.WriteLine(stopwatch.ElapsedMilliseconds/1000.0 + ": " + "partial solutions solved");

            var finalSolutionData = taskSolver.MergeSolution(tasks.Select(task => task.Result).ToArray());
            _testOutputHelper.WriteLine(stopwatch.ElapsedMilliseconds/1000.0 + ": " + "problems merged");

            bool result;
            using (var memoryStream = new MemoryStream(finalSolutionData))
            {
                var finalSolution = JsonSerializer.Deserialize<string>(memoryStream) ?? throw new ParsingNullException(nameof(finalSolutionData));
                _testOutputHelper.WriteLine(stopwatch.ElapsedMilliseconds / 1000.0 + ": " + finalSolution);
                var reg = new Regex(@"[-+]?\d+([,.]\d+)?");
                MatchCollection m;
                m = reg.Matches(finalSolution);
                result = Math.Abs(Math.Round((double)new decimal(double.Parse(m[0].Value)), 2) - expectedResult) < double.Epsilon;
            }
            _testOutputHelper.WriteLine(stopwatch.ElapsedMilliseconds / 1000.0 + ": " + "final solution deserialized");

            return result;
        }
    }
}
