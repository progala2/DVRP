using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using _15pl04.Ucc.Commons.Exceptions;
using _15pl04.Ucc.TaskSolver.DvrpAlgorithm;

namespace _15pl04.Ucc.TaskSolver
{
    /// <summary>
    /// Class responsible for dividing, solving and merging the Dynamic Vehicles Route Problem.
    /// </summary>
    public class DvrpTaskSolver: TaskSolver
    {
        private readonly DvrpProblem _dvrpProblem;

        private string ReadLineOrThrow(StreamReader r)
        {
            return r.ReadLine() ?? throw new InvalidOperationException("Read Line Can't be null");
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="problemData">Binary data with all information about the problem. String should be loaded from a .vrp file.</param>
        public DvrpTaskSolver(byte[] problemData)
        {
            State = TaskSolverState.Ok;
            try
            {
                string problem;
                using (var mem = new MemoryStream(problemData))
                {
                    problem = JsonSerializer.Deserialize<string>(mem) ?? throw new ParsingNullException(nameof(problemData));
                }
                var data = Encoding.UTF8.GetBytes(problem);
                using (var mem = new MemoryStream(data))
                using (var r = new StreamReader(mem))
                {
                    var reg = new Regex(@"-?\d+");
                    MatchCollection m;
                    for (var i = 0; i < 6; i++)
                        r.ReadLine();

                    var line = ReadLineOrThrow(r);
                    m = reg.Matches(line);
                    var numVisits = int.Parse(m[0].Value);
                    // num_location
                    r.ReadLine();

                    line = ReadLineOrThrow(r);
                    m = reg.Matches(line);
                    var numVehicles = int.Parse(m[0].Value);

                    line = ReadLineOrThrow(r);
                    m = reg.Matches(line);
                    var capacity = int.Parse(m[0].Value);

                    for (var i = 0; i < 4; i++)
                        r.ReadLine();
                    var demands = new int[numVisits];
                    for (var i = 0; i < demands.Length; i++)
                    {
                        line = ReadLineOrThrow(r);
                        m = reg.Matches(line);
                        demands[i] = int.Parse(m[1].Value);
                    }
                    // header
                    r.ReadLine();
                    var locationCoord = new int[numVisits + 1, 2];
                    for (var i = 0; i < locationCoord.GetLength(0); i++)
                    {
                        line = ReadLineOrThrow(r);
                        m = reg.Matches(line);
                        locationCoord[i, 0] = int.Parse(m[1].Value);
                        locationCoord[i, 1] = int.Parse(m[2].Value);
                    }

                    for (var i = -numVisits; i < 4; i++)
                        r.ReadLine();
                    var durations = new int[numVisits];
                    for (var i = 0; i < demands.Length; i++)
                    {
                        line = ReadLineOrThrow(r);
                        m = reg.Matches(line);
                        durations[i] = int.Parse(m[1].Value);
                    }
                    // header
                    r.ReadLine();
                    line = ReadLineOrThrow(r);
                    m = reg.Matches(line);
                    var depotStart = int.Parse(m[1].Value);
                    var depotEnd = int.Parse(m[2].Value);
                    for (var i = 0; i < 2; i++)
                        r.ReadLine();
                    var timeAvail = new int[numVisits];
                    for (var i = 0; i < timeAvail.Length; i++)
                    {
                        line = ReadLineOrThrow(r);
                        m = reg.Matches(line);
                        timeAvail[i] = int.Parse(m[1].Value);
                    }
                    var requests = new Request[numVisits];
                    for (var i = 0; i < requests.Length; i++)
                    {
                        requests[i] = new Request(locationCoord[i + 1, 0], locationCoord[i + 1, 1], demands[i],
                            timeAvail[i], durations[i]);
                    }
                    _dvrpProblem = new DvrpProblem(numVehicles, capacity, new[]
                    {
                        new Depot(locationCoord[0, 0], locationCoord[0, 1], depotStart, depotEnd)
                    }, requests);
                }
            }
            catch (Exception ex)
            {
                Exception = ex;
                _dvrpProblem = new DvrpProblem(0, 0, Array.Empty<Depot>(), Array.Empty<Request>());
            }
        }

        /// <summary>
        /// Problem type name solvable by this Task Solver.
        /// </summary>
        public override string Name => "UCC.Dvrp";

        /// <summary>
        /// Divide problem into smaller ones.
        /// </summary>
        /// <param name="threadCount">Number of divisions.</param>
        /// <returns>Binary serialized parts of the problem.</returns>
        public override byte[][] DivideProblem(int threadCount)
        {
            try
            {
                var divider = new DvrpDivider();
                var problems = divider.Divide(_dvrpProblem, threadCount);
                var result = new byte[threadCount][];
                for (var i = 0; i < threadCount; ++i)
                {
	                using var memoryStream = new MemoryStream();
	                JsonSerializer.Serialize(memoryStream, problems[i]);
	                result[i] = memoryStream.ToArray();
                }
                return result;
            }
            catch (Exception ex)
            {
                Exception = ex;
                State = TaskSolverState.Error;
                throw;
            }
        }

        /// <summary>
        /// Merge partial solutions into the final one.
        /// </summary>
        /// <param name="solutions">Binary serialized DVRP solutions <see cref="DvrpSolution"/>.</param>
        /// <returns>Binary serialized string with a description of the best solution.</returns>
        public override byte[] MergeSolution(byte[][] solutions)
        {
            try
            {
                var finalSolution = new DvrpSolution(double.MaxValue, Array.Empty<int[]>());
                foreach (var t in solutions)
                {
	                using var memoryStream = new MemoryStream(t);
	                var solution = JsonSerializer.Deserialize<DvrpSolution>(memoryStream) ?? throw new ArgumentNullException();
	                if (solution.FinalDistance < finalSolution.FinalDistance)
	                {
		                finalSolution = solution;
	                }
                }
                using (var memoryStream = new MemoryStream())
                {
                    var result = "Final Distance: " + finalSolution.FinalDistance;
                    foreach (var route in finalSolution.CarsRoutes)
                    {
                        result += "\n";
                        if (route.Length == 0)
                            break;
                        result = route.Aggregate(result, (current, i) => current + (" " + i));
                    }
                    JsonSerializer.Serialize(memoryStream, result);
                    return memoryStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                Exception = ex;
                State = TaskSolverState.Error;
                throw;
            }
        }

        /// <summary>
        /// Solve a partial problem with a given timeout.
        /// </summary>
        /// <param name="partialData">Binary serialized DVRP partial problem data.</param>
        /// <param name="timeout">Maximum time for computations.</param>
        /// <returns>Binary serialized solution.</returns>
        public override byte[] Solve(byte[] partialData, TimeSpan timeout)
        {
            try
            {
                DvrpPartialProblem problem;
                using (var memoryStream = new MemoryStream(partialData))
                {
                    problem = JsonSerializer.Deserialize<DvrpPartialProblem>(memoryStream) ?? throw new ParsingNullException(nameof(partialData));
                }
                var solver = new DvrpSolver(_dvrpProblem);
                using (var memoryStream = new MemoryStream())
                {
                    JsonSerializer.Serialize(memoryStream, solver.Solve(problem, timeout));
                    State = solver.State;
                    return memoryStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                Exception = ex;
                State = TaskSolverState.Error;
                throw;
            }
        }
    }
}